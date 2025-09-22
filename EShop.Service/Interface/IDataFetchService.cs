using EShop.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Interface
{
    public interface IDataFetchService
    {
        Task<List<Product>> FetchCoursesFromApi();
    }
}
