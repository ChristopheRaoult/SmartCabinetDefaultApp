using SmartDrawerDatabase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerAdmin.ViewModel
{
    public class UsersViewModel 
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BadgeId { get; set; }
        public int Fingerprints { get; set; }

        public string Password { get; set; }
        
    }
}
