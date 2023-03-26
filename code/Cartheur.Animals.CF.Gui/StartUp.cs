using System.Windows.Forms;
using Cartheur.Animals.CF.Gui.Forms;

namespace Cartheur.Animals.CF.Gui
{
    /// <summary>
    /// Virtual wrapper for managed memory in the CLR.
    /// </summary>
    /// <remarks>http://blogs.msdn.com/b/robtiffany/archive/2009/04/09/memmaker-for-the-net-compact-framework.aspx</remarks>
    public class StartUp
    {
        /// <summary>
        /// The application entry point for emotional animals.
        /// </summary>
        public static void Animals()
        {
            Application.Run(new MainForm());
        }
    }
}
