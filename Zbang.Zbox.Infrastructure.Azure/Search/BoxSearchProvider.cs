using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDog.Search.Model;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class BoxSearchProvider : IBoxReadSearchProvider, IBoxWriteSearchProvider
    {
        private const string IndexName = "box";
        private const string Id = "id";
        private const string Name = "name";
        private const string Image = "image";
        private const string Professor = "professor";
        private const string Course = "course";
        private const string Url = "url";
        private const string Universityid = "universityid";
        private const string Userids = "userids";

        private Index GetUniversityIndex()
        {
            return new Index(IndexName)
                .WithStringField(Id, f => f
                    .IsKey()
                    .IsRetrievable()
                )
                .WithStringField(Name, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(Image, f => f
                    .IsRetrievable())
                .WithStringField(Professor, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(Course, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(Url, f => f
                    .IsRetrievable())
                .WithField(Universityid, "Edm.Int64", f => f
                    .IsFilterable())
                .WithStringCollectionField(Userids, f => f
                    .IsFilterable());

        }
    }

    public interface IBoxWriteSearchProvider
    {
        //Task<bool> UpdateData(IEnumerable<BoxSearchDto> universityToUpload,
        //    IEnumerable<long> universityToDelete);
    }

    public interface IBoxReadSearchProvider
    {
    }
}
