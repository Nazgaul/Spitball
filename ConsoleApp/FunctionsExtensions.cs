using Autofac;
using Cloudents.Query;
using Dapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class FunctionsExtensions
    {
        private static CancellationToken token = CancellationToken.None;


        public static async Task MergeCourses(IContainer container)
        {
            var d = container.Resolve<DapperRepository>();
            var t = MigrateCoursesAndUni.Read();
            const string update = @"BEGIN
                                       IF NOT EXISTS (select * from sb.Course where Name = @NewId)
                                       BEGIN
                                          insert into sb.Course values (@NewId, 0, GETUTCDATE())
                                       END
                                    END
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
                if (item.Action == "כן")
                {
                    try
                    {
                      //  var testnewId = t.Where(w => w.Id == x).Select(s => s.Name).FirstOrDefault();
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(update, new
                            {
                                newId = item.NewId,
                                oldId = item.NewName == item.NewId ? item.OldName : item.NewName
                            });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                        Thread.Sleep(1000);
                    }
                }
                /*else if (item.NewId == "delete")
                {
                    try
                    {
                        var z = await d.WithConnectionAsync(async f =>
                        {
                            return await f.ExecuteAsync(@"delete from sb.UsersCourses where CourseId = @oldId;
                                                        delete from sb.Course where [Name] = @oldId;",
                                new { oldId = item.Name });

                        }, token);
                    }
                    catch
                    {
                        MigrateCoursesAndUni.WriteToSheet(counter);
                        Thread.Sleep(1000);
                    }
                }*/
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
                        Thread.Sleep(1000);
                    }
                }
                else if (item.NewId == "Delete")
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
