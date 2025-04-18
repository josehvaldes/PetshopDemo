﻿using PetShop.Application.Auth;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public Task<User?> Create(User user)
        {
            user.guid = Guid.NewGuid().ToString();
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

        public async Task<User?> Retrieve(string domain, string username)
        {
            return await _userRepository.Retrieve(domain, username);
        }
    }
}
