using EShop.Domain.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
