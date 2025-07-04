﻿using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository.Products
{
    public interface IProductQueryable
    {
        Task<IQueryable<Product>> GetQueryable();
    }
}
