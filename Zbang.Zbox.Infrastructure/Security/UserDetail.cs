using System;

namespace Zbang.Zbox.Infrastructure.Security
{
    [Serializable]
    public class UserDetail
    {
        const string Delimiter = "@";

        public UserDetail(
            long? universityId,
            long? universityDataId)
        {
            UniversityId = universityId;
            UniversityDataId = universityDataId;

        }
        public long? UniversityId { get; set; }
        public long? UniversityDataId { get; set; }

        public static string Serialize(UserDetail user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            return string.Format("{1}{0}{2}",
                Delimiter,
                user.UniversityId,
                user.UniversityDataId);
        }

        public static UserDetail DeSerialize(string data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var array = data.Split(new[] { Delimiter }, StringSplitOptions.None);
            if (array.Length != 2)
            {
                return null;
            }
            long? universityId = null;
            long? universityWrapperId = null;
            long temp;

            if (long.TryParse(array[0], out temp))
            {
                universityId = temp;
            }
            if (long.TryParse(array[1], out temp))
            {
                universityWrapperId = temp;
            }

            return new UserDetail(
                universityId,
                universityWrapperId);
        }
    }
}
