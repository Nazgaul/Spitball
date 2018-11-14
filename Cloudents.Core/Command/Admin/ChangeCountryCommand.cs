﻿using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class ChangeCountryCommand : ICommand
    {
        public ChangeCountryCommand(long id, string country)
        {
            Id = id;
            Country = country;
        }

        public long Id { get; set; }
        public string Country { get; set; }
    }
}