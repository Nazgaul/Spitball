namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class HomePage
   {
       public const string UniversityColors = @"select 
LargeImage as logo,
HeaderBackgroundColor,
BackgroundImage,
H1Text,
VideoBackgroundColor,
VideoFontColor,
SignupColor 
from zbox.University
where id = @UniversityId and IsDeleted = 0";


        public const string Stats = @" select 
 ROUND(sum(NoOfBoxes) * 1.22,0) as BoxesCount,
 ROUND((sum(NoOfItems) + sum(NoOfQuizzes)) * 1.22,0) as DocumentCount,
  Round(sum(NoOfUsers)*1.22,0) as StudentsCount
 from zbox.University where IsDeleted = 0";
    }
}
