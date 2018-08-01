using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Cloudents.Ico.Resources.Model;

namespace Cloudents.Ico.Models
{
    public abstract class Team
    {
        private readonly ResourceManager _rm;

        protected Team(string name)
        {
            _rm = new ResourceManager($"Cloudents.Ico.Resources.Model.Team{name}", typeof(TeamEidan).Assembly);
        }

        public string FirstName => _rm.GetString("FirstName");
        public string LastName => _rm.GetString("LastName");

        public abstract string Image { get; }
        public abstract string LinkdinLink { get; }
    }
}
