using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Selectors;
using System.ServiceModel;

namespace SmartDrawerWpfApp.Wcf
{
    class WcfUserNamePasswordValidator : UserNamePasswordValidator
    {
        private string Login = "RFID";
        private string Pass = "Rf1dW@ll!";
        public override void Validate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) | string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            //validate the username and password here against the database
            if (userName != Login | password != Pass)
                // This throws an informative fault to the client.
                throw new FaultException("Unknown Username or Incorrect Password");
            // When you do not want to throw an informative fault to the client,
            // throw the following exception.
            // throw new SecurityTokenException("Unknown Username or Incorrect Password");
        }

    }
}
