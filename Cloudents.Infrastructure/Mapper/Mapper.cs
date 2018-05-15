using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Mapper
{
    public class Mapper : IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

        public Mapper(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
        }


        public T Map<T>(object source)
        {
            return _mapper.Map<T>(source);
        }
    }
}