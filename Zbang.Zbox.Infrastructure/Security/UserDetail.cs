using System;

namespace Zbang.Zbox.Infrastructure.Security
{
    [Serializable]
    public class UserDetail
    {
        const string Delimiter = "@";

        public UserDetail(
            string language,
            long? universityId
            )
        {
            Language = language;
            UniversityId = universityId;

        }
        public string Language { get; set; }
        public long? UniversityId { get; set; }

        public static string Serialize(UserDetail user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            return string.Format("{1}{0}{2}{0}",
                Delimiter,
                user.Language,
                user.UniversityId
               );
        }

        public static UserDetail Deserialize(string data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var array = data.Split(new[] { Delimiter }, StringSplitOptions.None);
            if (array.Length != 3)
            {
                return null;
            }
            string language = array[0];
            long? universityId = null;
            long temp;

            if (long.TryParse(array[1], out temp))
            {
                universityId = temp;
            }


            return new UserDetail(
                language,
                universityId);
        }
    }
}
