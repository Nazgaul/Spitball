using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search.Book
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<BookSearch.BookDetailResult, IEnumerable<BookSearchDto>>()
                    .ConvertUsing((jo, _, c) =>
                    {
                        if (string.Equals(jo.Response.Status, "error", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return null;
                        }
                        return jo.Response.Page.Books?.Book.Select(json =>
                            c.Mapper.Map<BookSearch.BookDetail, BookSearchDto>(json));
                    });
            CreateMap<BookSearch.BookDetail, BookSearchDto>().ConvertUsing(jo => new BookSearchDto
            {
                Image = jo.Image?.Image,
                Author = jo.Author,
                Binding = jo.Binding,
                Edition = jo.Edition,
                Isbn10 = jo.Isbn10,
                Isbn13 = jo.Isbn13,
                Title = jo.Title

            });
            CreateMap<BookSearch.BookDetailResult, BookDetailsDto>().ConvertUsing<BookDetailConverter>();
        }
    }
}
