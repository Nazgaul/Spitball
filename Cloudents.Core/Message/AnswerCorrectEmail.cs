using System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class AnswerCorrectEmail : BaseEmail
    {
        public AnswerCorrectEmail(string to) : base(to, null, "Congratz - answer correct")
        {
          
        }

       

        public override string ToString()
        {
            return $"Congratz one of your answers is correct";
        }
    }
}