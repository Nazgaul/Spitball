using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Dapper;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Url;

namespace Testing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m_ZboxWriteService = IocFactory.IocWrapper.Resolve<IZboxWriteService>();
            m_BlobProviderFiles = IocFactory.IocWrapper.Resolve<IBlobProvider2<FilesContainerName>>();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("starting to work");
            listBox1.Update();
            //textBoxLog.Text = string.Empty;
            m_ClusterFlunkUniversityId = int.Parse(textBoxClusterId.Text);
            m_SpitballUniversityId = long.Parse(textBoxUniId.Text);
            m_LibraryId = textBoxLibrary.Text;
            ChangeUserUniversity();
            await BuildBoxesAsync();
            await BuildFilesAsync();
            listBox1.Items.Add("Done");
            listBox1.Update();
            MessageBox.Show("Done");
        }


        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;


        private int m_ClusterFlunkUniversityId;
        private long m_SpitballUniversityId;
        private const long SpitballUserId = 1067824;
        private string m_LibraryId;

        private void ChangeUserUniversity()
        {
            var command = new UpdateUserUniversityCommand(long.Parse(textBoxUniId.Text), SpitballUserId, null);
            m_ZboxWriteService.UpdateUserUniversity(command);
        }
        public async Task BuildBoxesAsync()
        {
            //Console.WriteLine($"working on {ClusterFlunkUniversityId}");
            //ChangeUserUniversity();
            IEnumerable<string> courses;
            using (var connection = await CreateConnectionAsync())
            {
                var query = @"select distinct c.name from networks n 
join courses c on c.network_id = n.id
join files f on f.course_id = c.id
where n.id = " + m_ClusterFlunkUniversityId;

                courses = await connection.QueryAsync<string>(query);
            }
            listBox1.Items.Add($"courses count {courses.Count()}");
            listBox1.Update();
            //Console.WriteLine($"courses count {courses.Count()}");
            foreach (var course in courses)
            {
                try
                {
                    using (var spitballConnection = await DapperConnection.OpenConnectionAsync())
                    {
                        var boxId =
                            await spitballConnection.QuerySingleOrDefaultAsync<long>(
                                "select boxId from zbox.box where boxname = @BoxName and isdeleted = 0 and university=@universityId",
                                new { BoxName = course, universityId = m_SpitballUniversityId });
                        if (boxId > 0)
                        {
                            listBox1.Items.Add($"found box {course}");
                            listBox1.Update();
                            //Console.WriteLine($"found box {course}");
                            continue;
                        }
                    }

                    var command = new CreateAcademicBoxCommand(SpitballUserId, course, null, null,
                        GuidEncoder.TryParseNullableGuid(m_LibraryId).Value, m_SpitballUniversityId);
                    await m_ZboxWriteService.CreateBoxAsync(command);
                    listBox1.Items.Add($"creating box {course}");
                    listBox1.Update();
                    //Console.WriteLine($"creating box {course}");
                }
                catch (BoxNameAlreadyExistsException)
                {
                    listBox1.Items.Add(course);
                    listBox1.Update();
                    //textBoxLog.Text += course + "\n";
                    //Console.WriteLine(course);
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
where n.id = " + m_ClusterFlunkUniversityId;
                files = connection.Query(query);
            }
            listBox1.Items.Add($"courses count {files.Count()}");
            listBox1.Update();
            //textBoxLog.Text += $"courses count {files.Count()}\n";
            //Console.WriteLine($"courses count {files.Count()}");
            var index = 0;
            foreach (var file in files)
            {
                using (var spitballConnection = await DapperConnection.OpenConnectionAsync())
                {
                    listBox1.Items.Add($"processing index: {index}");
                    listBox1.Update();
                    //textBoxLog.Text += $"processing index: {index}\n";
                    //Console.WriteLine($"processing index: {index}");
                    var courseName = file.coursename;
                    if (string.IsNullOrEmpty(file.name))
                    {
                        listBox1.Items.Add($"file name is empty index: {index}");
                        listBox1.Update();
                        //textBoxLog.Text += $"file name is empty index: {index}\n";
                        //Console.WriteLine($"file name is empty index: {index}");
                        index++;
                        continue;
                    }
                    var boxId =
                       await spitballConnection.QuerySingleAsync<long>(
                            "select boxId from zbox.box where boxname = @BoxName and isdeleted = 0 and university=@universityId",
                            new { BoxName = courseName, universityId = m_SpitballUniversityId });

                    var itemId = await spitballConnection.QuerySingleOrDefaultAsync<long>(
                        "select itemId from zbox.item where name = @ItemName and size = @Size and isdeleted = 0 and boxId = @BoxId",
                        new { ItemName = file.name, BoxId = boxId, Size = file.size });

                    if (itemId > 0)
                    {
                        listBox1.Items.Add($"already processed { itemId} index: {index}");
                        listBox1.Update();
                        //textBoxLog.Text += $"already processed { itemId} index: {index}\n";
                        //Console.WriteLine($"already processed { itemId} index: {index}");
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
