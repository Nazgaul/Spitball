using System;
using System.Collections.Generic;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure
{
    /// <summary>The Range class.</summary>
    /// <typeparam name="T">Generic parameter.</typeparam>
    public class Range<T> where T : IComparable<T>
    {
        public Range(T minimun, T maximum)
        {
            Minimum = minimun;
            Maximum = maximum;
        }
        /// <summary>Minimum value of the range.</summary>
        public T Minimum { get; }

        /// <summary>Maximum value of the range.</summary>
        public T Maximum { get; }

        /// <summary>Presents the Range in readable format.</summary>
        /// <returns>String representation of the Range</returns>
        public override string ToString()
        {
            return $"[{Minimum} - {Maximum}]";
        }


        /// <summary>Determines if the range is valid.</summary>
        /// <returns>True if range is valid, else false</returns>
        public bool IsValid()
        {
            return Minimum.CompareTo(Maximum) <= 0;
        }

        /// <summary>Determines if the provided value is inside the range.</summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public bool ContainsValue(T value)
        {
            return (Minimum.CompareTo(value) <= 0) && (value.CompareTo(Maximum) <= 0);
        }

        /// <summary>Determines if this Range is inside the bounds of another range.</summary>
        /// <param name="range">The parent range to test on</param>
        /// <returns>True if range is inclusive, else false</returns>
        public bool IsInsideRange(Range<T> range)
        {
            return IsValid() && range.IsValid() && range.ContainsValue(Minimum) && range.ContainsValue(Maximum);
        }

        /// <summary>Determines if another range is inside the bounds of this range.</summary>
        /// <param name="range">The child range to test</param>
        /// <returns>True if range is inside, else false</returns>
        public bool ContainsRange(Range<T> range)
        {
            return IsValid() && range.IsValid() && ContainsValue(range.Minimum) && ContainsValue(range.Maximum);
        }





    }


    public static class GamificationLevels
    {

        [ResourceDescription(typeof(Gamification), "Level1")]
        private static readonly Range<int> Level1 = new Range<int>(0, 1000);
        [ResourceDescription(typeof(Gamification), "Level2")]
        private static readonly Range<int> Level2 = new Range<int>(Level1.Maximum + 1, 2000);
        [ResourceDescription(typeof(Gamification), "Level3")]
        private static readonly Range<int> Level3 = new Range<int>(Level2.Maximum + 1, 4000);
        [ResourceDescription(typeof(Gamification), "Level4")]
        private static readonly Range<int> Level4 = new Range<int>(Level3.Maximum + 1, 8000);
        [ResourceDescription(typeof(Gamification), "Level5")]
        private static readonly Range<int> Level5 = new Range<int>(Level4.Maximum + 1, int.MaxValue);

        //public static IEnumerable<LevelDescription> GetLevels()
        //{
        //    var levels = new List<LevelDescription>();
        //    int index = 0;
        //    foreach (
        //        var property in
        //            typeof(GamificationLevels).GetFields(System.Reflection.BindingFlags.Static |
        //                                                 System.Reflection.BindingFlags.NonPublic))
        //    {
        //        var val = property.GetValue(null) as Range<int>;
        //        if (val == null)
        //        {

        //            continue;
        //        }
        //        var attribute = property.GetCustomAttributes(typeof(ResourceDescriptionAttribute), false);
        //        if (attribute.Length == 0)
        //        {
        //            return null;
        //        }
        //        var att = attribute[0] as ResourceDescriptionAttribute;

        //        if (att?.ResourceType == null) levels.Add(new LevelDescription(att?.Description, val.Maximum, index));
        //        var levelName = new System.Resources.ResourceManager(att.ResourceType);
        //        index++;
        //        levels.Add(new LevelDescription(levelName.GetString(att.ResourceName, CultureInfo.CurrentCulture), val.Maximum, index));

        //    }
        //    return levels;
        //}

        public static LevelDescription GetLevel(int score)
        {
            int index = 0;
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
