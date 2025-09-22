using EShop.Domain.Identity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Repository.Interface
{
    public interface IUserRepository
    {
        EShopApplicationUser GetUserById(string id);
    }
}
