
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{    
    public class RegenerateBoxAccessTokenCommandHandler: ICommandHandler<RegenerateBoxAccessTokenCommand, RegenerateBoxAccessTokenCommandResult>
    {
        //Fields        
        private IRepository<Box> m_BoxRepository;

        //Ctors
        public RegenerateBoxAccessTokenCommandHandler(IRepository<Box> boxRepository)
        {            
            m_BoxRepository = boxRepository;
        }

        //Methods
        public RegenerateBoxAccessTokenCommandResult Execute(RegenerateBoxAccessTokenCommand command)
        {   
            //Get storage
            Box box = m_BoxRepository.Get(command.BoxId);

            string accessToken = box.RegenerateAccessToken(command.AccessToken);

            return new RegenerateBoxAccessTokenCommandResult(accessToken);
        }
    }
}
