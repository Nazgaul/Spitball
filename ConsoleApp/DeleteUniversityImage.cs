using Cloudents.Persistence;
using Dapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class DeleteUniversityImage
    {
        private readonly DapperRepository _dapper;
        private readonly HttpClient _client;
        public DeleteUniversityImage(DapperRepository dapper, HttpClient client)
        {
            _dapper = dapper;
            _client = client;
        }
        public async Task DeleteBrokenUniversityImageAsync(CancellationToken token)
        {

            const string sql = @"select Id, ImageUrl 
                            from sb.University 
                            where ImageUrl is not null";

            const string updateSql = @"update sb.University set ImageUrl = null where Id = @Id";
            IEnumerable<UniversityImage> res;

            using (var conn = _dapper.OpenConnection())
            {
                res = await conn.QueryAsync<UniversityImage>(sql);
            }

            foreach (var university in res)
            {
                try
                {
                    var test = await _client.GetAsync(university.ImageUrl, token);
                    test.EnsureSuccessStatusCode();
                }

                catch (Exception)
                {
                    Console.WriteLine($"delete ImageUrl from university : {university.Id}");
                    using (var conn = _dapper.OpenConnection())
                    {
                        var update = conn.Execute(updateSql, new { university.Id });
                    }
                }
            }
        }

        private class UniversityImage
        {
            public Guid Id { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
