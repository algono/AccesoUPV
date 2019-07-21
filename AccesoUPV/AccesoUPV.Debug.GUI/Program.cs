using AccesoUPV.Library.Services;
using System;
using System.Windows.Forms;

namespace AccesoUPV.Debug.GUI
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Principal(new AccesoUPVService()));
        }
    }
}
