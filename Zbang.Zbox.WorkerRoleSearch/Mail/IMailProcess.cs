﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public interface IMailProcess
    {
        Task<bool> ExcecuteAsync();
    }
}
