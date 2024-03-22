using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ScreenSaver
{
    internal class ScreenInstances
    {
        public static List<ScreenSaverForm> ScrForms = new List<ScreenSaverForm>();
        public static Random EntropySrc = new Random((int)DateTime.Now.Ticks);
        public static Bitmap[] katakana = new Bitmap[480];
        public static CancellationTokenSource GlobalCTS = new CancellationTokenSource();
        public static  List<int> EmptySprites = new List<int>() { -1, 100, 220, 340, 460, 26, 116, 117, 118, 119, 146, 136, 137, 138, 139, 266, 256, 257, 258, 259, 376, 386, 387, 388, 389 };
        public static bool ShowCredits = true;
        public static List<string> CreditsStrings = new List<string>()
        {
            "The Matrix®",
            ".NET WinForms screensaver",
            "© 2024 by Gordon Freeman [gfr20141201@gmail.com]",
            "inspired by and dedicated to Burning_Thornbush [burning_thornbush@yahoo.com]",
        };

        public static int GetLetterSprite(char ch)
        {
            var res = -1;
            if (ch >= 'A' && ch <= 'Z')
            {
                res = ch + 321;
            }
            if (ch >= 'a' && ch <= 'z')
            {
                res = ch + 315;
            }
            if (ch >= '0' && ch <= '9')
            {
                res = ch + 390;
            }
            if (ch == ' ')
            {
                res = 25;
            }
            if (ch == '[')
            {
                res = 465;
            }
            if (ch == ']')
            {
                res = 466;
            }
            if (ch == '©')
            {
                res = 473;
            }
            if (ch == '®')
            {
                res = 472;
            }
            if (ch == '@')
            {
                res = 471;
            }
            if (ch == '.')
            {
                res = 452;
            }
            if (ch == '_')
            {
                res = 460;
            }
            return res;
        }
    }
}
