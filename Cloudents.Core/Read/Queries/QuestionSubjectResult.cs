using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Read
{

    public class QuestionSubjectDto : IQueryResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}