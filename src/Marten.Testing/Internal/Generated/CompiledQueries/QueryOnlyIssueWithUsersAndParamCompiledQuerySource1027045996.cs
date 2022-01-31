// <auto-generated/>
#pragma warning disable
using Marten.Internal.CompiledQueries;
using Marten.Linq.QueryHandlers;
using Marten.Testing.Bugs;
using System;

namespace Marten.Generated.CompiledQueries
{
    // START: QueryOnlyIssueWithUsersAndParamCompiledQuery1027045996
    public class QueryOnlyIssueWithUsersAndParamCompiledQuery1027045996 : Marten.Internal.CompiledQueries.ComplexCompiledQuery<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>, Marten.Testing.Bugs.compiled_query_problem_with_includes_and_ICompiledQuery_reuse.IssueWithUsersAndParam>
    {
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _inner;
        private readonly Marten.Testing.Bugs.compiled_query_problem_with_includes_and_ICompiledQuery_reuse.IssueWithUsersAndParam _query;
        private readonly Marten.Internal.CompiledQueries.HardCodedParameters _hardcoded;

        public QueryOnlyIssueWithUsersAndParamCompiledQuery1027045996(Marten.Linq.QueryHandlers.IMaybeStatefulHandler inner, Marten.Testing.Bugs.compiled_query_problem_with_includes_and_ICompiledQuery_reuse.IssueWithUsersAndParam query, Marten.Internal.CompiledQueries.HardCodedParameters hardcoded) : base(inner, query, hardcoded)
        {
            _inner = inner;
            _query = query;
            _hardcoded = hardcoded;
        }



        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(@"drop table if exists mt_temp_id_list1;
            create temp table mt_temp_id_list1 as (
            select id, CAST(d.data ->> 'AssigneeId' as uuid) as id1 from public.mt_doc_issue as d  where CAST(d.data ->> 'AssigneeId' as uuid) = ?
            ); select d.data from public.mt_doc_user as d where id in (select id1 from mt_temp_id_list1);
             select d.data from public.mt_doc_issue as d where id in (select id from mt_temp_id_list1)");

            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[0].Value = _query.UserId;
        }


        public override Marten.Linq.QueryHandlers.IQueryHandler<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>> BuildHandler(Marten.Internal.IMartenSession session)
        {
            var cloned = _inner.CloneForSession(session, null);
            var includeWriters = new Marten.Linq.Includes.IIncludeReader[]{Marten.Linq.Includes.Include.ReaderToList(session, _query.Users)};
            var included = new Marten.Linq.Includes.IncludeQueryHandler<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>>((Marten.Linq.QueryHandlers.IQueryHandler<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>>)cloned, includeWriters);
            return included;
        }

    }

    // END: QueryOnlyIssueWithUsersAndParamCompiledQuery1027045996
    
    
    // START: QueryOnlyIssueWithUsersAndParamCompiledQuerySource1027045996
    public class QueryOnlyIssueWithUsersAndParamCompiledQuerySource1027045996 : Marten.Internal.CompiledQueries.CompiledQuerySource<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>, Marten.Testing.Bugs.compiled_query_problem_with_includes_and_ICompiledQuery_reuse.IssueWithUsersAndParam>
    {
        private readonly Marten.Internal.CompiledQueries.HardCodedParameters _hardcoded;
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _maybeStatefulHandler;

        public QueryOnlyIssueWithUsersAndParamCompiledQuerySource1027045996(Marten.Internal.CompiledQueries.HardCodedParameters hardcoded, Marten.Linq.QueryHandlers.IMaybeStatefulHandler maybeStatefulHandler)
        {
            _hardcoded = hardcoded;
            _maybeStatefulHandler = maybeStatefulHandler;
        }



        public override Marten.Linq.QueryHandlers.IQueryHandler<System.Collections.Generic.IEnumerable<Marten.Testing.Documents.Issue>> BuildHandler(Marten.Testing.Bugs.compiled_query_problem_with_includes_and_ICompiledQuery_reuse.IssueWithUsersAndParam query, Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.CompiledQueries.QueryOnlyIssueWithUsersAndParamCompiledQuery1027045996(_maybeStatefulHandler, query, _hardcoded);
        }

    }

    // END: QueryOnlyIssueWithUsersAndParamCompiledQuerySource1027045996
    
    
}
