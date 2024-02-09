using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JasperFx.Core;
using Marten.Internal.Sessions;
using Marten.Services;
using Marten.Storage;
using Npgsql;
using NpgsqlTypes;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Events.Daemon.Internals;

[Obsolete("Goes away in 7")]
internal interface IEventFetcher: IDisposable
{
    Task Load(EventRange range, CancellationToken token);
}


/// <summary>
///     Fetches ranges of event objects. Used within the asynchronous projections
/// </summary>
[Obsolete("Goes away in 7")]
internal class EventFetcher: IEventFetcher
{
    private readonly int _aggregateIndex;
    private readonly NpgsqlParameter _ceiling;
    private readonly NpgsqlCommand _command;
    private readonly IMartenDatabase _database;
    private readonly NpgsqlParameter _floor;
    private readonly IEventStorage _storage;
    private readonly IDocumentStore _store;

    public EventFetcher(IDocumentStore store, IMartenDatabase database,
        ISqlFragment[] filters)
    {
        _store = store;
        _database = database;

        using var session = querySession();
        _storage = session.EventStorage();

        var schemaName = store.Options.Events.DatabaseSchemaName;

        var builder = new CommandBuilder();
        builder.Append($"select {_storage.SelectFields().Select(x => "d." + x).Join(", ")}, s.type as stream_type");
        builder.Append(
            $" from {schemaName}.mt_events as d inner join {schemaName}.mt_streams as s on d.stream_id = s.id");

        if (_store.Options.Events.TenancyStyle == TenancyStyle.Conjoined)
        {
            builder.Append(" and d.tenant_id = s.tenant_id");
        }

        var parameters = builder.AppendWithParameters(" where d.seq_id > ? and d.seq_id <= ?");
        _floor = parameters[0];
        _ceiling = parameters[1];
        _floor.NpgsqlDbType = _ceiling.NpgsqlDbType = NpgsqlDbType.Bigint;

        foreach (var filter in filters)
        {
            builder.Append(" and ");
            filter.Apply(builder);
        }

        builder.Append(" order by d.seq_id");

        _command = builder.Compile();
        _aggregateIndex = _storage.SelectFields().Length;
    }


    public void Dispose()
    {
    }

    public async Task Load(EventRange range, CancellationToken token)
    {
        // There's an assumption here that this method is only called sequentially
        // and never at the same time on the same instance

        try
        {
            range.Events = new List<IEvent>();

            await using var session = querySession();
            _floor.Value = range.SequenceFloor;
            _ceiling.Value = range.SequenceCeiling;

            await using var reader = await session.ExecuteReaderAsync(_command, token).ConfigureAwait(false);
            while (await reader.ReadAsync(token).ConfigureAwait(false))
            {
                await handleEvent(range, token, reader).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            throw new EventLoaderException(range.ShardName, _database, e);
        }
    }

    private QuerySession querySession()
    {
        return (QuerySession)_store.QuerySession(SessionOptions.ForDatabase(_database));
    }

    protected virtual async Task handleEvent(EventRange range, CancellationToken token, DbDataReader reader)
    {
        var @event = await _storage.ResolveAsync(reader, token).ConfigureAwait(false);

        if (!await reader.IsDBNullAsync(_aggregateIndex, token).ConfigureAwait(false))
        {
            @event.AggregateTypeName =
                await reader.GetFieldValueAsync<string>(_aggregateIndex, token).ConfigureAwait(false);
        }

        range.Events.Add(@event);
    }
}