using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Gamification
    {
        public Gamification()
        {
            Levels = new List<Level>();
            Badges = new List<Badge>();
        }

        public IList<Level> Levels { get; set; }
        public IList<Badge> Badges { get; set; }
    }

    public class Badge
    {
        public Badge(string name, string description, string badge)
        {
            Name = name;
            Description = description;
            BadgeSystem = badge;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string BadgeSystem { get; private set; }
    }

    public class Level
    {
        public Level(string name, string description, int index)
        {
            Name = name;
            Description = description;
            Index = index;
        }

        public string Name { get;private set; }
        public string Description { get;private set; }
        public int Index { get; private set; }
    }
}