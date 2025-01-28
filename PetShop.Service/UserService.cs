using Azure;
using PetShop.Data;
using PetShop.Model;

namespace PetShop.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public Task<UserEntity?> Create(UserEntity user)
        {
            user.guid = Guid.NewGuid().ToString();
            user.ETag = ETag.All;            
            // Only USER role by default
            user.roles = Role.User; 

            var response = _userRepository.Create(user);
            return response;
        }

        public async Task<CallResponse> Delete(string domain, string username)
        {
            var response = new CallResponse();
            var entity  = await _userRepository.Retrieve(domain, username);
            if (entity != null)
            {
                if (entity.roles.Contains(Role.Administrator))
                {
                    response.AddMessage($"User {entity.domain}/{entity.username} can't be deleted");
                }
                else 
                {
                    var deleted = await _userRepository.Delete(entity);
                    if (!deleted) 
                    {
                        response.AddMessage($"Deleting failed for {entity.domain}/{entity.username}. See logs for more details.");
                    }
                }
            }
            else 
            {
                response.AddMessage($"User {domain}/{username} not found");
            }

            return response;
        }

        public async Task<UserEntity?> Retrieve(string domain, string username)
        {
            return await _userRepository.Retrieve(domain, username);
        }
    }
}
