using System;
using Cloudents.Core.Entities;
using Xunit;

namespace Cloudents.Core.Test.Entities.Db
{
    public class TagTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("ttt,sdfks")]
        [InlineData("tt")]
        [InlineData("2. Greek civilization was flourished in te year")]

        public void CreateTag_InvalidTagName_Exception(string name)
        {
            void Action()
            {
                var tag = new Tag(name);
            }

            Assert.Throws<ArgumentException>((Action) Action);
        }


        [Theory]
        [InlineData("222")]
        [InlineData("www   ")]
        public void CreateTag_ValidTagName_Ok(string name)
        {
            var tag = new Tag(name);

            Assert.Equal(name.Trim(),tag.Name);
        }
    }
}