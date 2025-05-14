using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(EmailRequest request);
    }
}
