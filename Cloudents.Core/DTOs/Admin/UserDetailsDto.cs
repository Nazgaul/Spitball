﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserDetailsDto
    {
        public UserDetailsDto()
        {
        }
        [DtoToEntityConnection(nameof(RegularUser.Id))]
        public long Id { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Email))]
        public string Email { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.University))]
        public string University { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Country))]
        public string Country { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Score))]
        public int Score { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.FraudScore))]
        public int FraudScore { get; set; }
       
        public int ReferredCount { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Balance))]
        public long Balance { get; set; }
    }
}
