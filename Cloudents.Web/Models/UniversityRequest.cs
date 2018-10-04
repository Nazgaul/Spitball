﻿using JetBrains.Annotations;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// University request object
    /// </summary>
    public class UniversityRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        public string Term { get; set; }

        /*/// <summary>
        /// the user location
        /// </summary>*/
        //[CanBeNull]
        //public GeographicCoordinate Location { get; set; }
    }
}