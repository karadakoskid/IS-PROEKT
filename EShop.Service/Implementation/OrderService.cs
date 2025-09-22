using EShop.Domain.Domain_Models;
using EShop.Repository.Interface;
using EShop.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAll(selector: x => x,
                                           include: x => x.Include(z => z.ProductInOrders)
                                                          .ThenInclude(z => z.OrderedProduct)
                                                          .Include(z=>z.Owner)).ToList();
        }

        public Order GetOrder(Guid Id)
        {
            return _orderRepository.Get(selector: x => x,
                                        predicate: x => x.Id.Equals(Id),
                                        include: x => x.Include(z => z.ProductInOrders)
                                                       .ThenInclude(z => z.OrderedProduct)
                                                       .Include(z => z.Owner));
        }
    }
}
