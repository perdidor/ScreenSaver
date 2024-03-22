using System;
using System.Windows.Forms;

namespace ScreenSaver
{
	public class DotNETScreenSaver
	{
		[STAThread]
		static void Main(string[] args) 
		{
			if (args.Length > 0)
			{
				if (args[0].ToLower().Trim().Substring(0,2) == "/c")
				{
					MessageBox.Show("No options available.", "DotNetMatrix", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else if (args[0].ToLower() == "/s")
				{
					for (int i = Screen.AllScreens.GetLowerBound(0); i <= Screen.AllScreens.GetUpperBound(0); i++)
                        Application.Run(new ScreenSaverForm(i));				
				}
			}
			else
			{
				for (int i = Screen.AllScreens.GetLowerBound(0); i <= Screen.AllScreens.GetUpperBound(0); i++)
                    Application.Run(new ScreenSaverForm(i));				
			}
		}
	}
}
