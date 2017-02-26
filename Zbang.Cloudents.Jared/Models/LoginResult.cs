﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Zbang.Cloudents.Jared.Models
{
    public class LoginResult
    {
        [JsonProperty(PropertyName = "authenticationToken")]
        public string AuthenticationToken { get; set; }

        [JsonProperty(PropertyName = "user")]
        public LoginResultUser User { get; set; }
    }

    public class LoginResultUser
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}