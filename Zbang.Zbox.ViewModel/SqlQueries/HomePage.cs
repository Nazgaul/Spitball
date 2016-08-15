namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class HomePage
    {
        public const string UniversityColors = @"select 
        LargeImage as logo,
        HeaderBackgroundColor,
        BackgroundImage,
        UniversityName,
        VideoBackgroundColor,
        VideoFontColor,
        SignupColor,
        MainSignupColor
        from zbox.University
        where id = @UniversityId and IsDeleted = 0";


        public const string Stats = @" select 
 ROUND(sum(NoOfBoxes) * 1.22,0) as BoxesCount,
 ROUND((sum(NoOfItems) + sum(NoOfQuizzes)) * 1.22,0) as DocumentCount,
  Round(sum(NoOfUsers)*1.22,0) as StudentsCount
 from zbox.University where IsDeleted = 0";


        public const string UniversityBoxes = @"select top 16 Name,ItemCount,MembersCount,CourseCode,ProfessorName,Url from (
select 
    b.BoxName as Name,
	b.quizcount + b.itemcount as ItemCount,
	b.CourseCode as CourseCode,
	b.MembersCount as MembersCount,
	b.ProfessorName,
	b.Url as Url,
	Rank() over (partition BY libraryid order by  b.MembersCount + (b.ItemCount + b.QuizCount + b.CommentCount) / 3 desc,b.updatetime desc) as x
	from zbox.box b join zbox.university u on b.university=u.id
	where Discriminator = 2
	and b.isdeleted = 0
	AND country='US' 
 )   t1
where x = 1
order by ItemCount desc";
    }
}
