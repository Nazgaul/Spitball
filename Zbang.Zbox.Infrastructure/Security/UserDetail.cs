using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Security
{
    [Serializable]
    public class UserDetail
    {
        const string Delimiter = "@";

        public UserDetail(
            string language,
            int score,
            long? universityId,
            long? universityWrapperId)
        {
            Language = language;
            Score = score;
            UniversityId = universityId;
            UniversityWrapperId = universityWrapperId;

        }
        public string Language { get; set; }
        public int Score { get; set; }
        public long? UniversityId { get; set; }
        public long? UniversityWrapperId { get; set; }

        public static string Serialize(UserDetail user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            return string.Format("{1}{0}{4}{0}{2}{0}{3}",
                Delimiter,
                user.Language,
                user.UniversityId,
                user.UniversityWrapperId,
                user.Score);
        }

        public static UserDetail Deserialize(string data)
        {
            var array = data.Split(new[] { Delimiter }, StringSplitOptions.None);
            if (array.Length != 4)
            {
                return null;
            }
            string language = array[0];
            int score;
            int.TryParse(array[1], out score);

            long? universityId = null;
            long? universityWrapperId = null;
            long temp = -1;

            if (long.TryParse(array[2], out temp))
            {
                universityId = temp;
            }
            if (long.TryParse(array[3], out temp))
            {
                universityWrapperId = temp;
            }


            return new UserDetail(
                language, score,
                universityId, universityWrapperId);
        }
    }
}
