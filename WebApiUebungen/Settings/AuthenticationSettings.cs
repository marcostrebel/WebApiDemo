using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiUebungen.Settings
{
    public class AuthenticationSettings
    {
        public bool Enable { get; set; }

        public string DefaultScheme { get; set; }

        public JwtBearerSettings Jwt { get; set; }
    }
}
