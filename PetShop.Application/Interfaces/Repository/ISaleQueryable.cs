﻿using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Interfaces.Repository
{
    public interface ISaleQueryable
    {
        Task<IQueryable<Sale>> GetQueryable();
    }
}
