using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Helpers.Settings
{
    public class MailSettings
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string SmtpServer { get; set;}
        public int Port { get; set; }
        public required string DisplayedName { get; set; }

    }
}
