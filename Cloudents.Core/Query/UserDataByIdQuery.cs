﻿using System;
using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Core.Query
{
    public class UserDataByIdQuery : IQuery<User>,
        IQuery<UserAccountDto>,
        IQuery<IEnumerable<BalanceDto>>,
        IQuery<IEnumerable<TransactionDto>>, 
        IQuery<ProfileDto>
        //IQuery<SuspendUserDto>
        
    {
        public UserDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }


    public class UserWithUniversityQuery : IQuery<UserProfile>
    {
        public UserWithUniversityQuery(long id, Guid? universityId)
        {
            Id = id;
            UniversityId = universityId;
        }

        public long Id { get; }
        public Guid? UniversityId { get; }
    }
}