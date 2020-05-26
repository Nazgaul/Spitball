﻿using System;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class DocumentTests
    {


        [Fact]
        public void InitDocument__NullCourse_Error()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Document("some name", null!, null!, 0, DocumentType.Document, null, PriceType.Free));
        }

        [Fact]
        public void InitDocument_NullUser_Error()
        {
            var course = new Course("Some name");
            Assert.Throws<ArgumentNullException>(() => new Document("some name", course, null!, 0,
                DocumentType.Document, null, PriceType.Free));
        }


        [Fact]
        public void InitDocument_NullName_Error()
        {
            var course = new Course("Some name");
            Assert.Throws<ArgumentNullException>(() => new Document(null!, course, null!, 0, DocumentType.Document, null,
                PriceType.Free));
        }


        [Fact]
        public void InitDocument_Ok_StateOk()
        {
            var course = new Course("Some name");
            var mockUser = new Mock<User>();
            var mockTutor = new Mock<Tutor>();
            mockTutor.Setup(s => s.User).Returns(mockUser.Object);

            var date = DateTime.UtcNow;
            var document = new Document("some name", course, mockTutor.Object, 0, DocumentType.Document, null, PriceType.Free);
            document.Status.State.Should().Be(ItemState.Ok);
            document.TimeStamp.CreationTime.Should().BeAfter(date);
        }


        [Fact]
        public void InitDocumentWithPrice_Ok_StateOk()
        {
            var course = new Course("Some name");
            var mockUser = new Mock<User>();
            var mockTutor = new Mock<Tutor>();
            mockTutor.Setup(s => s.User).Returns(mockUser.Object);
            var date = DateTime.UtcNow;
            var document = new Document("some name", 
                course, mockTutor.Object,
                10, DocumentType.Document, null, PriceType.Free);
            document.Status.State.Should().Be(ItemState.Ok);
            document.TimeStamp.CreationTime.Should().BeAfter(date);
        }

       
    }
}