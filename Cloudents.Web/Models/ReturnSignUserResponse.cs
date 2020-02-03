namespace Cloudents.Web.Models
{
    //public class ReturnSignUserResponse
    //{
    //    public ReturnSignUserResponse(NextStep step)
    //    {
    //        Step = step;
    //    }



    //    public NextStep? Step { get; set; }

    //    public string ReturnUrl { get; set; }

    //}

    public class ReturnSignUserResponse
    {
        public ReturnSignUserResponse(RegistrationStep step, object param = null)
        {
            Step = step;
            Param = param;
        }

        private ReturnSignUserResponse()
        {
            
        }

        public RegistrationStep Step { get;  }
        public object Param { get; }

        public bool IsSignedIn { get; set; }


        public static ReturnSignUserResponse SignIn()
        {
            return new ReturnSignUserResponse()
            {
                IsSignedIn =  true
            };
        }


    }
}