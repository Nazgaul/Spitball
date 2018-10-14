﻿namespace Cloudents.Core.Entities.Db
{
    public class QuestionSubject
    {
        public virtual int Id { get; set; }
        public virtual string Text  { get; set; }

        public virtual int Order { get; set; }

        public virtual string TextHebrew { get; set; }

    }
}
