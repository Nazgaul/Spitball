using System;
using Dapper;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Url;

namespace Testing
{
    class ClusterflunkFilesCopy
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;
        public ClusterflunkFilesCopy(IocFactory iocFactory)
        {
            m_ZboxWriteService = iocFactory.Resolve<IZboxWriteService>();
            m_BlobProviderFiles = iocFactory.Resolve<IBlobProvider2<FilesContainerName>>();
        }

        private const int ClusterFlunkUniversityId = 49;
        private const long SpitballUniversityId = 173499;
        private const long SpitballUserId = 1067824;
        private const string LibraryId = "kI593f6a0kOsmKZtAL_6cw";
        public async Task BuildBoxesAsync()
        {

            IEnumerable<string> courses;
            using (var connection = await CreateConnectionAsync())
            {
                var query = @"select distinct c.name from networks n 
join courses c on c.network_id = n.id
join files f on f.course_id = c.id
where n.id = " + ClusterFlunkUniversityId;

                courses = await connection.QueryAsync<string>(query);
            }
            Console.WriteLine($"courses count {courses.Count()}");
            foreach (var course in courses)
            {
                try
                {
                    using (var spitballConnection = await DapperConnection.OpenConnectionAsync())
                    {
                        var boxId =
                            await spitballConnection.QuerySingleOrDefaultAsync<long>(
                                "select boxId from zbox.box where boxname = @BoxName and isdeleted = 0 and university=@universityId",
                                new {BoxName = course, universityId = SpitballUniversityId});
                        if (boxId > 0)
                        {
                            Console.WriteLine($"found box {course}");
                            continue;
                        }
                    }

                    var command = new CreateAcademicBoxCommand(SpitballUserId, course, null, null,
                        GuidEncoder.TryParseNullableGuid(LibraryId).Value, SpitballUniversityId);
                    await m_ZboxWriteService.CreateBoxAsync(command);
                    Console.WriteLine($"creating box {course}");
                }
                catch (BoxNameAlreadyExistsException)
                {
                    Console.WriteLine(course);
                }
            }
        }


        public async Task BuildFilesAsync()
        {
            IEnumerable<dynamic> files;
            using (var connection = await CreateConnectionAsync())
            {
                var query = @"
select distinct c.name as courseName,key,f.type,f.name,f.size,extension from networks n 
join courses c on c.network_id = n.id
join files f on f.course_id = c.id
where n.id = " + ClusterFlunkUniversityId;
                files = connection.Query(query);
            }
            Console.WriteLine($"courses count {files.Count()}");
            var index = 0;
            foreach (var file in files)
            {
                using (var spitballConnection = await DapperConnection.OpenConnectionAsync())
                {
                    Console.WriteLine($"processing index: {index}");
                    var courseName = file.coursename;
                    if (string.IsNullOrEmpty(file.name))
                    {
                        Console.WriteLine($"file name is empty index: {index}");
                        index++;
                        continue;
                    }
                    var boxId =
                       await spitballConnection.QuerySingleAsync<long>(
                            "select boxId from zbox.box where boxname = @BoxName and isdeleted = 0 and university=@universityId",
                            new {BoxName = courseName, universityId = SpitballUniversityId});

                    var itemId = await spitballConnection.QuerySingleOrDefaultAsync<long>(
                        "select itemId from zbox.item where name = @ItemName and size = @Size and isdeleted = 0 and boxId = @BoxId",
                        new {ItemName = file.name, BoxId = boxId, Size = file.size });
                    
                    if (itemId > 0)
                    {
                        Console.WriteLine($"already processed { itemId} index: {index}");
                        index++;
                        continue;
                    }
                    var blobName = $"{Guid.NewGuid()}.{file.extension}".ToLowerInvariant();
                    
                    if (await DownloadSomeFileAsync(file.key, blobName, file.type))
                    {
                        
                        //m_BlobProviderFiles.UploadFromLinkAsync
                        var command = new AddFileToBoxCommand(SpitballUserId, boxId, blobName, file.name, file.size,
                            null,
                            false);
                        await m_ZboxWriteService.AddItemToBoxAsync(command);
                    }
                    index++;
                }
            }
        }

        private static async Task<OdbcConnection> CreateConnectionAsync()
        {
            var connection = new OdbcConnection("DSN=PostgreSQL30;UID=postgres;PWD=123qwe");
            await connection.OpenAsync();
            return connection;
        }




        private async Task<bool> DownloadSomeFileAsync(string key, string blobName, string mimeType)
        {
            var credentials = new Amazon.Runtime.BasicAWSCredentials("AKIAIGYVFLOIXNXB6PRA",
                "jJkHDUT7XCcIjjMUlT1ZqpwE+aNJMwef1H3CQ/xI");
            using (var client = new AmazonS3Client(credentials, RegionEndpoint.USEast1))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = "clusterflunk",
                    Key = key
                };
                try
                {
                    using (GetObjectResponse response = await client.GetObjectAsync(request))
                    {
                        await
                            m_BlobProviderFiles.UploadStreamAsync(blobName, response.ResponseStream, mimeType,
                                default(CancellationToken));
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode == "NoSuchKey")
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
