using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coravel.Mailer.Mail.Exceptions {
    public class DriverNotFound : Exception {
        public DriverNotFound(string driver) : base($"The driver '{driver}' was not found. Please check the Driver configuration in the appsettings.") {

        }
    }
}
