using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Ico.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Ico.ViewComponents
{
    public class TeamComponent : ViewComponent
    {
        private readonly IList<Team> _teams = new List<Team>();
        

        public TeamComponent()
        {
            foreach (var type in FindDerivedTypes())
            {
                var t = (Team)Activator.CreateInstance(type);
                _teams.Add(t);
            }
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            //var items = await GetItemsAsync(maxPriority, isDone);
            return Task.FromResult<IViewComponentResult>(View(_teams));
        }


        private IEnumerable<Type> FindDerivedTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t != typeof(Team) &&
                                                  typeof(Team).IsAssignableFrom(t));
        }
    }
}
