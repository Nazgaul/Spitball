using System.Globalization;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure
{
    public static class GamificationLevels
    {

        [ResourceDescription(typeof(Gamification), "Level1")]
        private static readonly Range<int> Level1 = new Range<int>(0, 1000);
        [ResourceDescription(typeof(Gamification), "Level2")]
        private static readonly Range<int> Level2 = new Range<int>(Level1.Maximum + 1, 2500);
        [ResourceDescription(typeof(Gamification), "Level3")]
        private static readonly Range<int> Level3 = new Range<int>(Level2.Maximum + 1, 5000);
        [ResourceDescription(typeof(Gamification), "Level4")]
        private static readonly Range<int> Level4 = new Range<int>(Level3.Maximum + 1, 10000);
        [ResourceDescription(typeof(Gamification), "Level5")]
        private static readonly Range<int> Level5 = new Range<int>(Level4.Maximum + 1, int.MaxValue);


        public static LevelDescription GetLevel(int score)
        {
            var index = 0;
            foreach (var property in typeof(GamificationLevels).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                var val = property.GetValue(null) as Range<int>;
                if (val == null)
                {
                    continue;
                }
                if (!val.ContainsValue(score))
                {
                    index++;
                    continue;
                }

                var attribute = property.GetCustomAttributes(typeof(ResourceDescriptionAttribute), false);
                if (attribute.Length == 0)
                {
                    return null;
                }
                var att = attribute[0] as ResourceDescriptionAttribute;
                if (att?.ResourceType == null) return new LevelDescription(att?.Description, val.Maximum, index);
                var levelName = new System.Resources.ResourceManager(att.ResourceType);
                return new LevelDescription(levelName.GetString(att.ResourceName, CultureInfo.CurrentCulture), val.Maximum, index);

            }
            return null;
        }

        public class LevelDescription
        {
            public LevelDescription(string name, int nextLevel, int level)
            {
                Name = name;
                NextLevel = nextLevel;
                Level = level;
            }

            public string Name { get; private set; }
            public int NextLevel { get; private set; }
            public int Level { get; private set; }
        }
    }
}