using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateWithBasicOWIN.Utility
{
    public class BCryptHelper
    {
        public String HashPassword(String password)
        {           
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public Boolean Verify(String password, String storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
