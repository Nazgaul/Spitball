using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Cloudents.Core.Entities;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public  class RegularUserTests
    {

        [Fact]
        public void Stake_PositiveNumber_ThrowException()
        {
            var user = new RegularUser("some email", "some name", "some private key", CultureInfo.InvariantCulture);
            //user.Stake(100,null);


        }
    }
}
