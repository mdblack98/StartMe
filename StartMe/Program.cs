using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartMe
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(CustomExceptionHandler.OnThreadException);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Top level exception\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        // Create a class to handle the exception event.
        internal class CustomExceptionHandler
        {
            // Handle the exception event
            public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                DialogResult result = ShowThreadExceptionDialog(t.Exception);

                // Exit the program when the user clicks Abort.
                if (result == DialogResult.Abort)
                    Application.Exit();
            }

            // Create and display the error message.
            private static DialogResult ShowThreadExceptionDialog(Exception e)
            {
                string errorMsg = "An error occurred.  Please contact mdblack98@yahoo.com " +
                     "with the following information:\n\n";
                errorMsg += String.Format("Exception Type: {0}\n\n", e.GetType().Name);
                errorMsg += "\n\nStack Trace:\n" + e.StackTrace;
                return MessageBox.Show(errorMsg, "StartMe Application Error",
                     MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
            }
        }
    }
}
