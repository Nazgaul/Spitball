using Autofac;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class FunctionsExtensions
    {
        private static CancellationToken token = CancellationToken.None;


        public static async Task MergeCourses(IContainer container, IBlobProvider<DocumentContainer> blobProvider)
        {
            var d = container.Resolve<DapperRepository>();
            var t = MigrateCoursesAndUni.Read();
            const string update = @"
                            update sb.Document
                            set CourseName = @newId
                            where CourseName = @oldId;

                            update sb.Question
                            set CourseId = @newId
                            where CourseId = @oldId;

                            update sb.UsersCourses 
                            set CourseId = @newId
                            where CourseId = @oldId
                            and UserId not in (select UserId from sb.UsersCourses where CourseId = @newId);

                            delete from sb.UsersCourses where CourseId = @oldId;

                            delete from sb.Course where [Name] = @oldId;";

            var counter = 2;
            foreach (var item in t)
            {
                if (!item.NewId.Equals("ok", StringComparison.InvariantCultureIgnoreCase) &&
                    !item.NewId.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                      //  var testnewId = t.Where(w => w.Id == x).Select(s => s.Name).FirstOrDefault();
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(update, new
                            {
                                newId = item.NewId,
                                oldId = item.Name
                            });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                        Thread.Sleep(1000);
                    }
                }
                else if (item.NewId.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {

                        var docs = await d.WithConnectionAsync(async f =>
                        {
                            return await f.QueryAsync<long?>(@"select Id from sb.Document where CourseName = @oldId and State = 'Ok';",
                                new { oldId = item.Name });

                        }, token);

                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(@"delete from sb.UsersCourses where CourseId = @oldId;
                                            delete from sb.Question where CourseId = @oldId;
                                            delete from sb.Document where CourseName = @oldId;
                                            delete from sb.Course where [Name] = @oldId;",
                                new { oldId = item.Name });

                        }, token);


                        //var z = await d.WithConnectionAsync(async f =>
                        //{
                        //    using (var multi = f.QueryMultiple(@"select Id from sb.Document where CourseName = @oldId;", 
                        //        new { oldId = item.Name }))
                        //{
                        //        var docs = multi.Read<long>();
                        //        var deleteStatment = await multi.Command.Connection.ExecuteAsync(
                        //            @"delete from sb.UsersCourses where CourseId = @oldId;
                        //                    delete from sb.Question where CourseId = @oldId;
                        //                    delete from sb.Document where CourseName = @oldId;
                        //                    delete from sb.Course where[Name] = @oldId;",
                        //            new { oldId = item.Name });
                        //        return docs;
                        //}
                        //}, token);


                        foreach (var doc in docs)
                        {
                            await blobProvider.DeleteDirectoryAsync(doc.ToString(), token);
                        }
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                        Thread.Sleep(1000);
                    }
                }
                else {
                    MigrateCoursesAndUni.WriteToSheet(counter);
                    Thread.Sleep(1000);
                }
                
                counter++;
            }
        }


        public static async Task MergeUniversity(IContainer container)
        {
            var d = container.Resolve<DapperRepository>();
            var t = MigrateCoursesAndUni.ReadUniversity();
            string update = @"update sb.[user] set UniversityId2 = @Newuni where UniversityId2 = @OldUni;
                                update sb.Document set UniversityId = @Newuni where UniversityId = @OldUni;
                                update sb.Question set UniversityId = @Newuni where UniversityId = @OldUni;
                                delete from sb.University where id =  @OldUni;";

            var counter = 2;
            foreach (var item in t)
            {
                if (Guid.TryParse(item.NewId, out Guid x))
                {
                    try
                    {
                        var testnewId = t.Where(w => w.UniId == x).Select(s => s.UniId).FirstOrDefault();
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(update, new
                            {
                                Newuni = x,
                                OldUni = item.UniId
                            });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToUniSheet(counter);
                        Thread.Sleep(1000);
                    }
                }
                else if (item.NewId.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(@"update sb.[user] set UniversityId2 = null where UniversityId2 = @OldUni;
                                                        delete from sb.University where id =  @OldUni;",
                                new { OldUni = item.UniId });
                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToUniSheet(counter);
                        Thread.Sleep(1000);
                    }
                }
                counter++;
            }
        }
    }
}
