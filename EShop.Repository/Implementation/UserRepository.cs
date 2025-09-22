using EShop.Domain.Identity_Models;
using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<EShopApplicationUser> entites;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            this.entites = _context.Set<EShopApplicationUser>();
        }

        public EShopApplicationUser GetUserById(string id)
        {
            return entites.First(ent => ent.Id == id);
        }
    }
}
