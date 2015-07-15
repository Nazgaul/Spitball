﻿using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class Quiz : IDirty
    {
        protected Quiz()
        {
            ShouldMakeDirty = () => true;
        }
        public Quiz(string name, long id, Box box, User owner)
            : this()
        {
            Id = id;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = null;
            }
            if (name != null)
            {
                name = name.Trim();

            }
            if (owner == null) throw new ArgumentNullException("owner");
            Name = name;
            Owner = owner;
            Box = box;

            DateTimeUser = new UserTimeDetails(owner.Email);
        }
        public virtual long Id { get; private set; }
        public virtual string Name { get; private set; }

        public virtual bool Publish { get; set; }

        public virtual string Banner { get; set; }

        public virtual User Owner { get; private set; }
        public virtual Box Box { get; private set; }

        public virtual string Content { get; set; }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual float Rate { get; protected set; }

        public virtual int NumberOfViews { get; private set; }
        public virtual int NumberOfComments { get; private set; }
        public virtual UserTimeDetails DateTimeUser { get; private set; }
        public virtual ICollection<Question> Questions { get; protected set; }

        public virtual ICollection<SolvedQuiz> SolvedQuizes { get; protected set; }

        public virtual string Url { get; set; }

        public virtual int Average { get; set; }
        public virtual double Stdevp { get; set; }

        public virtual Comment Comment { get; set; }

        public virtual ICollection<Updates> Updates { get; set; }

        public virtual void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            var universityName = Box.GetUniversityName() ?? "my";
            Url = UrlConsts.BuildQuizUrl(Box.Id, Box.Name, Id, Name, universityName);
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual void UpdateText(string newText)
        {
            //Throw.OnNull(newText, "newText", false);
            if (newText != null)
            {
                newText = newText.Trim();
            }
            Name = newText;
            DateTimeUser.UpdateTime = DateTime.UtcNow;
        }

        public virtual void UpdateNumberOfComments(int count)
        {
            NumberOfComments = count;
        }
        public virtual void UpdateNumberOfViews()
        {
            NumberOfViews++;
        }

        public virtual void UpdateQuizStats(int average, double stdevp)
        {
            Average = average;
            Stdevp = stdevp;
        }

        public bool IsDirty {get;set;}

        public Func<bool> ShouldMakeDirty
        {
            get;
            set;
        }

        public bool IsDeleted {get;set;}

        public void DeleteAssociation()
        {
            Questions.Clear();
            SolvedQuizes.Clear();
            Updates.Clear();
        }
    }
}
