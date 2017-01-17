namespace Zbang.Zbox.Infrastructure.Ai
{
    [System.AttributeUsage(System.AttributeTargets.Property ,
                      
        AllowMultiple = true)  // Multiuse attribute.
    ]
    public class WitApiName : System.Attribute
    {
        public WitApiName(string name)
        {
            Name = name;
        }

        public string Name { get;private set; }
    }
}