using System;
using System.Resources;
using Cloudents.Ico.Resources.Model;

namespace Cloudents.Ico.Models
{
    public abstract class Team : IComparable<Team>
    {
        private readonly ResourceManager _rm;

        protected Team(string name)
        {
            _rm = new ResourceManager($"Cloudents.Ico.Resources.Model.Team{name}", typeof(TeamEidan).Assembly);
        }

        public string FirstName => _rm.GetString("FirstName");
        public string LastName => _rm.GetString("LastName");
        public string Title => _rm.GetString("Title");
        public string Description => _rm.GetString("Description");

        public abstract string Image { get; }
        public abstract string LinkdinLink { get; }
        protected abstract int Order { get; }

        public int CompareTo(Team other)
        {
            if (other == null)
                return 1;

            return Order.CompareTo(other.Order);
        }
    }
}
