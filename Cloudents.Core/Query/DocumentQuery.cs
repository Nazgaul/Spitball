﻿using Cloudents.Core.Models;
using System.Collections.Generic;

namespace Cloudents.Core.Query
{
    public class DocumentQuery : VerticalQuery
    {
        public DocumentQuery(UserProfile userProfile,
            string term,
            string course,
            IEnumerable<string> filters) :
            base(userProfile, term, course)
        {
            Filters = filters;
        }

        public IEnumerable<string> Filters { get; }

    }

    public abstract class VerticalQuery
    {


        protected VerticalQuery(UserProfile userProfile, string term, string course)
        {
            UserProfile = userProfile;
            Term = term;
            Course = course;
        }

        public string Term { get; }
        public string Course { get; }

        public int Page { get; set; }

        public UserProfile UserProfile { get; }


    }
}