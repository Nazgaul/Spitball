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


        public const string UniversityBoxes = @"select top 6 Name,ItemCount,CourseCode,ProfessorName,Url from (
select 
    b.BoxName as Name,
	b.quizcount + b.itemcount + b.FlashcardCount as ItemCount,
	b.CourseCode as CourseCode,
	b.ProfessorName,
	b.Url as Url,
	Rank() over (partition BY libraryid order by b.ItemCount + b.QuizCount + b.CommentCount + b.FlashcardCount desc,b.updatetime desc) as x
	from zbox.box b join zbox.university u on b.university=u.id
	where Discriminator = 2
	and b.isdeleted = 0
	AND (@Universityid IS null OR b.University = @Universityid)
	AND (@country IS null OR country = @country)
 )   t1
where x = 1
order by ItemCount desc";
    }
}
