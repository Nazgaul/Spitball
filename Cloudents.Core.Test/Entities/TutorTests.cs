﻿using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Entities;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class TutorTests
    {
        [Fact]
        public void InitTutor_CountryIL_NullPrice_RaiseException()
        {
            var user = new User("some email", Language.EnglishIsrael);
            

            Assert.Throws<ArgumentException>(() => new Tutor("Some bio", user, null));

        }

        [Fact]
        public void InitTutor_CountryIL_Price_NoSubside()
        {
            var user = new User("some email", Language.EnglishIsrael) ;
            
            var price = 50M;

           var tutor =  new Tutor("Some bio", user, price);
           tutor.Price.Price.Should().Be(price);
           tutor.Price.SubsidizedPrice.Should().BeNull();

        }

        [Fact]
        public void InitTutor_CountryIN_Price_NoSubside()
        {
            var user = new User("some email", Language.EnglishIsrael);
            var price = 50M;
            user.ChangeCountry("IN");
            var tutor = new Tutor("Some bio", user, price);
            tutor.Price.Price.Should().Be(100);
            tutor.Price.SubsidizedPrice.Should().Be(0);

        }
    }
}