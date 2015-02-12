using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class QuizSearchProvider
    {
        private readonly string m_IndexName = "quiz";
        private bool m_CheckIndexExists;

        public QuizSearchProvider()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                m_IndexName = m_IndexName + "-dev";
                return;
            }
            if (RoleEnvironment.IsEmulated)
            {
                m_IndexName = m_IndexName + "-dev";
            }
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string OwnerField = "owner";
        private const string UrlField = "url";
        private const string UniversityidField = "universityid";
        private const string UseridsField = "userids";
    }
}
