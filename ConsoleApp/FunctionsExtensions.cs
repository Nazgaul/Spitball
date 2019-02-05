using Autofac;
using Cloudents.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class FunctionsExtensions
    {
        private static CancellationToken token = CancellationToken.None;


        public static async Task MergeCourses(IContainer _container)
        {
            var d = _container.Resolve<DapperRepository>();
            var t = MigrateCoursesAndUni.Read();
            string update = @"update sb.Document
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
                if (long.TryParse(item.NewId, out long x) && x != item.Id)
                {
                    try
                    {
                        var testnewId = t.Where(w => w.Id == x).Select(s => s.Name).FirstOrDefault();
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(update, new
                            {
                                newId = t.Where(w => w.Id == x).Select(s => s.Name)
                                .FirstOrDefault(),
                                oldId = item.Name
                            });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                    }
                }
                else if (item.NewId == "Delete")
                {
                    try
                    {
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync("delete from sb.Course where [Name] = @oldId;",
                                new { oldId = item.Name });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                    }
                }
                counter++;
            }
        }


        public static async Task MergeUniversity(IContainer _container)
        {
            var d = _container.Resolve<DapperRepository>();
            var t = MigrateCoursesAndUni.ReadUniversity();
            string update = @"update sb.[user] set UniversityId2 = @Newuni where UniversityId2 = @OldUni;
                                update sb.Document set UniversityId = @Newuni where UniversityId = @OldUni;
                                delete from sb.University where id =  @OldUni;";

            var counter = 2;
            foreach (var item in t)
            {
                if (long.TryParse(item.NewId, out long x) && x != item.OldId)
                {
                    try
                    {
                        var testnewId = t.Where(w => w.OldId == x).Select(s => s.UniId).FirstOrDefault();
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(update, new
                            {
                                Newuni = t.Where(w => w.OldId == x).Select(s => s.UniId)
                                .FirstOrDefault(),
                                OldUni = item.UniId
                            });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToUniSheet(counter);
                    }
                }
                else if (item.NewId == "Delete")
                {
                    try
                    {
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync("delete from sb.University where id =  @OldUni;",
                                new { OldUni = item.UniId });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToUniSheet(counter);
                    }
                }
                counter++;
            }
        }
    }
}
