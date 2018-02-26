namespace Cloudents.Core.Request
{
    public class SyncAzureQuery
    {
        private SyncAzureQuery(long version, int page)
        {
            Version = version;
            Page = page;
        }

        public long Version { get; }
        public int Page { get; }

        public static SyncAzureQuery ConvertFromString(string str)
        {
            var version = 0L;
            var page = 0;

            if (str == null)
            {
                return new SyncAzureQuery(version, page);
            }

            var vals = str.Split('|');
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

        public override string ToString()
        {
            return $"{Version}|{Page}";
        }
    }
}