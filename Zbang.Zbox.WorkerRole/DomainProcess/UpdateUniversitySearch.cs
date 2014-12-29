using System;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    class UpdateUniversitySearch : IDomainProcess
    {
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;
        public UpdateUniversitySearch(IUniversityWriteSearchProvider2 universitySearchProvider)
        {
            m_UniversitySearchProvider = universitySearchProvider;
        }

        public bool Execute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as UniversityData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            m_UniversitySearchProvider.UpdateData(new[] {new ViewModel.Dto.Library.UniversitySearchDto
            {
                Id = parameters.Id,
                Image = parameters.Image,
                Name = parameters.Name
            }}, null
            );
            return true;
        }
    }
}
