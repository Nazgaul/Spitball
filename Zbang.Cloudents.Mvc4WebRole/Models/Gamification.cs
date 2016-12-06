using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Gamification
    {
        public Gamification()
        {
            Levels = new List<Level>();
        }
        public IList<Level> Levels { get; set; }
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