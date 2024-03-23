using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScreenSaver
{
	public class DotNETScreenSaver
	{
		[STAThread]
		static void Main(string[] args) 
		{
			List<ScreenSaverForm> screenSaverForms = new List<ScreenSaverForm>();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (args.Length > 0)
			{
				if (args[0].ToLower().Trim().Substring(0,2) == "/c")
				{
					MessageBox.Show("No options available.", "DotNetMatrix", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else if (args[0].ToLower() == "/s")
				{
                    for (int i = Screen.AllScreens.GetLowerBound(0); i <= Screen.AllScreens.GetUpperBound(0); i++)
                        ScreenInstances.ScrForms.Add(new ScreenSaverForm(i));

                }
			}
			else
			{
				for (int i = Screen.AllScreens.GetLowerBound(0); i <= Screen.AllScreens.GetUpperBound(0); i++)
                    ScreenInstances.ScrForms.Add(new ScreenSaverForm(i));
            }
            if (ScreenInstances.ScrForms.Count > 0)
            {
                for (int i = 1; i < ScreenInstances.ScrForms.Count; i++)
                {
                    ScreenInstances.ScrForms[i].Show();
                }
            }
            Application.Run(ScreenInstances.ScrForms[0]);
        }

    }
}
