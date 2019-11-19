//using Autofac;
//using Cloudents.Core.Storage;
//using Cloudents.Query;
//using Dapper;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//namespace ConsoleApp
//{
//    public class FunctionsExtensions
//    {
//        private static CancellationToken _token = CancellationToken.None;


//        public static async Task MergeCourses(IContainer container, IDocumentDirectoryBlobProvider blobProvider)
//        {
//            var d = container.Resolve<DapperRepository>();
//            var t = MigrateCoursesAndUni.Read();
//            const string update = @"
//                            delete from sb.Vote where DocumentId in 
//                                    (
//                                    select id from sb.Document where UniversityId = @oldId
//                                    );

//                            delete from sb.DocumentsTags where DocumentId in 
//                                        (
//                                        select id from sb.Document where CourseName = @oldId
//                                        );

//                            update sb.Document
//                            set CourseName = @newId
//                            where CourseName = @oldId;

//                            update sb.Question
//                            set CourseId = @newId
//                            where CourseId = @oldId;

//                            update sb.UsersCourses 
//                            set CourseId = @newId
//                            where CourseId = @oldId
//                            and UserId not in (select UserId from sb.UsersCourses where CourseId = @newId);

//                            delete from sb.UsersCourses where CourseId = @oldId;

//                            delete from sb.Course where [Name] = @oldId;";

//            var counter = 2;
//            foreach (var item in t)
//            {
//                if (!item.NewId.Equals("ok", StringComparison.InvariantCultureIgnoreCase) &&
//                    !item.NewId.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    try
//                    {
//                        //  var testnewId = t.Where(w => w.Id == x).Select(s => s.Name).FirstOrDefault();
//                        var z = await d.WithConnectionAsync(async f =>
//                        {
//                            return await f.ExecuteAsync(update, new
//                            {
//                                newId = item.NewId,
//                                oldId = item.NewName == item.NewId ? item.OldName : item.NewName
//                            });

//                        }, _token);
//                    }
//                    catch
//                    {
//                        MigrateCoursesAndUni.WriteToSheet(counter);
//                        Thread.Sleep(1000);
//                    }
//                }
//                else if (item.NewId.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    try
//                    {

//                        var docs = await d.WithConnectionAsync(async f =>
//                        {
//                            return await f.QueryAsync<long?>(@"select Id from sb.Document where CourseName = @oldId and State = 'Ok';",
//                                new { oldId = item.OldName});

//                        }, _token);

//                        var x = await d.WithConnectionAsync(async f =>
//                        {
//                            return await f.ExecuteAsync(@"
//                                                        delete from sb.Vote where DocumentId in 
//                                                                    (
//                                                                    select id from sb.Document where CourseName = @oldId
//                                                                    );
//                                                        delete from sb.DocumentsTags where DocumentId in 
//                                                                    (
//                                                                    select id from sb.Document where CourseName = @oldId
//                                                                    );",
//                                new { oldId = item.OldName });

//                        }, _token);

//                        var z = await d.WithConnectionAsync(async f =>
//                        {
//                            return await f.ExecuteAsync(@"delete from sb.UsersCourses where CourseId = @oldId;
//                                                        delete from sb.Question where CourseId = @oldId;
//                                                        delete from sb.Document where CourseName = @oldId;
//                                                        delete from sb.Course where [Name] = @oldId;",
//                                new { oldId = item.OldName });

//                        }, _token);

//                        foreach (var doc in docs)
//                        {
//                            await blobProvider.DeleteDirectoryAsync(doc.ToString(), _token);
//                        }
//                    }
//                    catch
//                    {
//                        MigrateCoursesAndUni.WriteToSheet(counter);
//                        Thread.Sleep(1000);
//                    }
//                }
//                else
//                {
//                    MigrateCoursesAndUni.WriteToSheet(counter);
//                    Thread.Sleep(1000);
//                }

//                counter++;
//            }
//        }


//        //public static async Task MergeUniversity(IContainer container)
//        //{
//        //    var d = container.Resolve<DapperRepository>();
//        //    var t = MigrateCoursesAndUni.ReadUniversity();
//        //    string update = @"update sb.[user] set UniversityId2 = @Newuni where UniversityId2 = @OldUni;
//        //                        update sb.Document set UniversityId = @Newuni where UniversityId = @OldUni;
//        //                        update sb.Question set UniversityId = @Newuni where UniversityId = @OldUni;
//        //                        delete from sb.University where id =  @OldUni;";

//        //    var counter = 2;
//        //    foreach (var item in t)
//        //    {
//        //        if (Guid.TryParse(item.NewId, out Guid x))
//        //        {
//        //            try
//        //            {
//        //                var testnewId = t.Where(w => w.UniId == x).Select(s => s.UniId).FirstOrDefault();
//        //                var z = await d.WithConnectionAsync(async f =>
//        //                {
//        //                    return await f.ExecuteAsync(update, new
//        //                    {
//        //                        Newuni = x,
//        //                        OldUni = item.UniId
//        //                    });

//        //                }, _token);
//        //            }
//        //            catch
//        //            {
//        //                MigrateCoursesAndUni.WriteToUniSheet(counter);
//        //                Thread.Sleep(1000);
//        //            }
//        //        }
//        //        else if (item.NewId.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
//        //        {
//        //            try
//        //            {
//        //                var z = await d.WithConnectionAsync(async f =>
//        //                {
//        //                    return await f.ExecuteAsync(@"update sb.[user] set UniversityId2 = null where UniversityId2 = @OldUni;
//        //                                                delete from sb.University where id =  @OldUni;",
//        //                        new { OldUni = item.UniId });
//        //                }, _token);
//        //            }
//        //            catch
//        //            {
//        //                MigrateCoursesAndUni.WriteToUniSheet(counter);
//        //                Thread.Sleep(1000);
//        //            }
//        //        }
//        //        counter++;
//        //    }
//        //}


////        public static async Task DeleteCourses2(IContainer container)
////        {
////            var d = container.Resolve<DapperRepository>();

////            IEnumerable<string> courses = null;
////            do
////            {
////                courses = await d.WithConnectionAsync(async f =>
////                {
////                    return await f.QueryAsync<string>(@"
////select top 100 name from sb.Course c
////where not exists ( select * from sb.Document d where d.CourseName = c.Name)
////and not exists ( select * from sb.question d where d.CourseId = c.Name)

////order by name");

////                }, default);
////                foreach (var item in courses)
////                {
////                    try
////                    {
////                        using (var conn = d.OpenConnection())
////                        {
////                            await conn.ExecuteAsync("delete from sb.userscourses where courseId=@name;" +
////                                                    "delete from sb.course where name=@name", new { name = item });
////                            Console.WriteLine($"Deleted course {item}");
////                        }
////                    }
////                    catch (SqlException e) when (e.Number == 547)
////                    {
////                        //Console.WriteLine(e);
////                        //throw;
////                    }
////                }


////                await Task.Delay(TimeSpan.FromMilliseconds(100));

////            } while (courses.Any());

////        }

//        public static async Task DeleteCourses(IContainer container)
//        {
//            var d = container.Resolve<DapperRepository>();
//            IEnumerable<string> courses = null;
//            do
//            {
//                courses = await d.WithConnectionAsync(async f =>
//                {
//                    return await f.QueryAsync<string>("select top 1000 Name from sb.DeletedCourses where State != 'Deleted'");

//                }, default);

//                foreach (var item in courses)
//                {
//                    try
//                    {
//                        var z = await d.WithConnectionAsync(async f =>
//                        {
//                            return await f.ExecuteAsync(@"delete from sb.Vote where DocumentId in 
//                                                                (
//                                                                select id from sb.Document where CourseName = @oldId
//                                                                );
//                                                    delete from sb.DocumentsTags where DocumentId in 
//                                                                (
//                                                                select id from sb.Document where CourseName = @oldId
//			                                                    )
//                                                    delete from sb.UsersCourses where CourseId = @oldId;
//                                                    delete from sb.Question where CourseId = @oldId;
//                                                    delete from sb.Document where CourseName = @oldId;
//                                                    delete from sb.Course where [Name] = @oldId;
//                                                    update sb.DeletedCourses set [State] = 'Deleted' where [name] = @oldId;",
//                                new { oldId = item });
//                        }, default);
//                        Console.WriteLine($"{item} Deleted");
//                    }
//                    catch
//                    {
//                        Console.WriteLine($"didn't process {item}");
//                    }
//                    Thread.Sleep(500);
//                }

//            } while (courses.Count() > 0 && courses != null);

//        }
//    }
//}
