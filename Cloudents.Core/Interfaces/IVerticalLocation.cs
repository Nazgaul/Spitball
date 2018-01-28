using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IVerticalLocation
    {
        string Location { get; }
        Location Cords { get; set; }
    }
}
