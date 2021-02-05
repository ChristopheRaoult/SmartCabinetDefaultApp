using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Class MyDebug : Debug class for display error message during exception
    /// </summary>
    public class MyDebug
    {
               
        /// <summary>
        /// Method Assert : Check for condition
        /// </summary>
        /// <param name="bCondition">Condition to check</param>
        [Conditional("DEBUG")]
        public static void Assert(bool bCondition)
        {            
            if (!bCondition)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                else
                    Debug.Assert(false);
            }
        }
        /// <summary>
        ///  Method Assert : Check for condition and display message
        /// </summary>
        /// <param name="bCondition">Condition to check</param>
        /// <param name="failureMessage">Message to display</param>
        [Conditional("DEBUG")]
        public static void Assert(bool bCondition, string failureMessage)
        {
            if (!bCondition)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                else
                    Debug.Assert(false, failureMessage);
            }
        }
        /// <summary>
        /// Method Fail : Emits the error message
        /// </summary>
        /// <param name="failureMessage">Message to emits</param>
        [Conditional("DEBUG")]
        public static void Fail(string failureMessage)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
            else
                Debug.Fail(failureMessage);
        }      
    }
}
