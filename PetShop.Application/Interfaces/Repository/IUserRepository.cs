﻿using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User?> Retrieve(string domain, string username);
        Task<User?> Create(User user);

        Task<bool> Delete(User user);
    }
}
