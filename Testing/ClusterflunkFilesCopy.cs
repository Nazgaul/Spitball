﻿using System;
using Dapper;
using System.Collections.Generic;
using System.Data.Odbc;
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

        private const int ClusterFlunkUniversityId = 18;
        private const long SpitballUniversityId = 173437;
        private const long SpitballUserId = 1067238;
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
            foreach (var course in courses)
            {
                try
                {
                    var command = new CreateAcademicBoxCommand(SpitballUserId, course, null, null,
                        Guid.Parse("00F024BD-03B9-4AF0-9AED-A669006953C6"), SpitballUniversityId);
                    await m_ZboxWriteService.CreateBoxAsync(command);
                    Console.WriteLine(course);
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
            var index = 0;
            foreach (var file in files)
            {
                using (var spitballConnection = await DapperConnection.OpenConnectionAsync())
                {
                    var courseName = file.coursename;
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
                    await DownloadSomeFileAsync(file.key, blobName, file.type);
                    //m_BlobProviderFiles.UploadFromLinkAsync
                    var command = new AddFileToBoxCommand(SpitballUserId, boxId, blobName, file.name, file.size, null,
                        false);
                    await m_ZboxWriteService.AddItemToBoxAsync(command);
                    index++;
                }
            }
        }

        private static async Task<OdbcConnection> CreateConnectionAsync()
        {
            OdbcConnection connection = new OdbcConnection("DSN=PostgreSQL30;UID=postgres;PWD=123qwe");
            await connection.OpenAsync();
            return connection;
        }




        private async Task DownloadSomeFileAsync(string key, string blobName, string mimeType)
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

                using (GetObjectResponse response = await client.GetObjectAsync(request))
                {
                    await m_BlobProviderFiles.UploadStreamAsync(blobName, response.ResponseStream, mimeType, default(CancellationToken));
                }
            }
        }
    }
}
