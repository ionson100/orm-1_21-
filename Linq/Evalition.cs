﻿namespace ORM_1_21_.Linq
{
    internal enum Evolution
    {
        None = 0,
        First,
        Any,
        Last,
        Count,
        Where,
        GroupBy,
        OrderBy,
        Select,
        DistinctCore,
        SelectNew,
        Single,
        SingleOrDefault,
        SaveOrUpdate,
        Delete,
        FirstOrDefault,
        Join,
        SelectMany,
        All,
        ElementAt,
        ElementAtOrDefault,
        OrderByDescending,
        Reverse,
        LastOrDefault,
        Limit,
        Update,
        Contains,
        JoinNew,
        //OverCache,
        Timeout,
        FreeSql,
        CacheUsage,
        CacheOver,
        CacheKey,
        LongCount,
        Skip,


        TableCreate,
        DropTable,
        TableExists,

        ExecuteScalar,
        TruncateTable,
        ExecuteNonQuery,
        DataTable,
        Between,
        SelectJoin,
        SelectNewGroup
    }

}
