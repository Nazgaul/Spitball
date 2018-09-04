﻿using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{

    //public sealed class SyncAzureQuery<T> : SyncAzureQuery,
    //    IQuery<(IEnumerable<T> update, IEnumerable<long> delete, long version)> where T: AzureSyncBaseDto
    //{
    //    public SyncAzureQuery(long version, int page) : base(version, page)
    //    {
    //    }
    //}

    public class SyncAzureQuery //: System.IEquatable<SyncAzureQuery>
        : IQuery<(IEnumerable<Question> update, IEnumerable<long> delete, long version)>
            //IQuery<(IEnumerable<AzureSyncBaseDto<Entities.Db.Question>> update, IEnumerable<long> delete, long version)>
    {
        public SyncAzureQuery(long version, int page)
        {
            Version = version;
            Page = page;
        }

        public static SyncAzureQuery Empty()
        {
            return new SyncAzureQuery(0,0);
        }

        public long Version { get; }
        public int Page { get; set; }

        public static SyncAzureQuery ConvertFromString(string str)
        {
            var version = 0L;
            var page = 0;

            if (str == null)
            {
                return new SyncAzureQuery(version, page);
            }

            var vals = str.Split('|');
            if (vals.Length != 2)
            {
                return new SyncAzureQuery(version, page);
            }
            for (var i = 0; i < vals.Length; i++)
            {
                if (!int.TryParse(vals[i], out var p)) continue;
                switch (i)
                {
                    case 0:
                        version = p;
                        break;
                    case 1:
                        page = p;
                        break;
                }
            }
            return new SyncAzureQuery(version, page);
        }

        //public override bool Equals(object obj)
        //{
        //    return obj is SyncAzureQuery query
        //           && Version == query.Version
        //           && Page == query.Page;
        //}

        //public override int GetHashCode()
        //{
        //    var hashCode = -2075985307;
        //    hashCode = hashCode * -1521134295 + Version.GetHashCode();
        //    hashCode = hashCode * -1521134295 + Page.GetHashCode();
        //    return hashCode;
        //}

        public override string ToString()
        {
            return $"{Version}|{Page}";
        }

        //public bool Equals(SyncAzureQuery other)
        //{
        //    return Version == other.Version
        //               && Page == other.Page;
        //}
    }
}