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
        private static readonly List<Team> Teams = new List<Team>();

        static TeamComponent()
        {
            foreach (var type in FindDerivedTypes())
            {
                var t = (Team)Activator.CreateInstance(type);
                Teams.Add(t);
            }

            Teams.Sort();
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            //var items = await GetItemsAsync(maxPriority, isDone);
            return Task.FromResult<IViewComponentResult>(View(Teams));
        }


        private static IEnumerable<Type> FindDerivedTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t != typeof(Team) &&
                                                                         typeof(Team).IsAssignableFrom(t));
        }
    }
}
