using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL.Exceptions
{
    public class NonexistentConstantException : Exception
    {
        /// <summary>
        /// Default constructor set as private. Must not be used.
        /// </summary>
        private NonexistentConstantException()
        {
        }

        /// <summary>
        /// Construct a new instance contraining the product item level number as exception message.
        /// </summary>
        /// <param name="constant">Item level number of the product already associated with this tag.</param>
        public NonexistentConstantException(string constant)
            : base(constant)
        {
        }
    }
}
