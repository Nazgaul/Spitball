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


    public class Badge
    {
        public Badge(string name, string details, int points, string image, string imageHidden)
        {
            Name = name;
            Details = details;
            Points = points;
            Image = image;
            ImageHidden = imageHidden;
        }

        public string Name { get;private set; }

        public string Image { get; set; }
        public string Details { get; private set; }
        public int Points { get; private set; }
        public string ImageHidden { get; private set; }
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