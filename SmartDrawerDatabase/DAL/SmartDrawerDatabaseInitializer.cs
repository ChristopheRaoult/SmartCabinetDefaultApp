using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public class SmartDrawerDatabaseInitializer : DropCreateDatabaseIfModelChanges<SmartDrawerDatabaseContext>
    {
        protected override void Seed (SmartDrawerDatabaseContext context)
        {
            base.Seed(context);
        }
    }
}
