using EShop.Domain.Identity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Domain_Models
{
    public class Order : BaseEntity
    {
        public string? OwnerId { get; set; }
        public EShopApplicationUser? Owner { get; set; }

        public ICollection<ProductInOrder>? ProductInOrders { get; set; }
    }
}
