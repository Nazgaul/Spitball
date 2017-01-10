using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class WitResponse
    {
        //public List<Intent> Intent { get; set; }

        public Dictionary<string, List<Entity>> Entities { get; set; }

        public Entity Intent
        {
            get
            {
                List<Entity> entity;
                if (Entities.TryGetValue("Intent", out entity))
                {
                    return entity[0];
                }
                return null;
            }
        }
    }
}