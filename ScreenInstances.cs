﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ScreenSaver
{
    internal static class ScreenInstances
    {
        public static List<ScreenSaverForm> ScrForms = new List<ScreenSaverForm>();
        public static Random EntropySrc = new Random((int)DateTime.Now.Ticks);
        public static Bitmap[] katakana = new Bitmap[480];
        public static Bitmap[] katakana_glitch = new Bitmap[480];
        public static CancellationTokenSource GlobalCTS = new CancellationTokenSource();
        public static  List<int> EmptySprites = new List<int>() { -1, 100, 220, 340, 460, 26, 116, 117, 118, 119, 146, 136, 137, 138, 139, 266, 256, 257, 258, 259, 376, 386, 387, 388, 389 };
        
        static ScreenInstances()
        {
            LoadKatakanaSprites();
        }

        public static void TerminateSS()
        {
            GlobalCTS.Cancel();
            Application.Exit();
        }

        public static Bitmap GetKatakanaSprite(int index, bool canglitch = true)
        {
            Bitmap resbitmap = null;
            try
            {
                if (ScreenInstances.EntropySrc.Next(1000) >= 995 && canglitch)    //chance to glitch is 0.1%
                {
                    resbitmap = ScreenInstances.katakana_glitch[index];
                }
                else
                {
                    resbitmap = ScreenInstances.katakana[index];
                }
            }
            catch (Exception ex)
            {
                var s = ex.ToString();
            }
            return resbitmap;
        }

        private static void LoadKatakanaSprites()
        {
            using (var raw = new Bitmap(Properties.Resources.spd))
            {
                for (int i = 0; i < katakana.Length; i++)
                {
                    var xptr = i * 16 % 640;
                    var yptr = ((int)(i / 40)) * 24;
                    Rectangle cloneRect = new Rectangle(xptr, yptr, 15, 24);
                    katakana[i] = raw.Clone(cloneRect, PixelFormat.Format32bppArgb);
                }
            }
            using (var raw = new Bitmap(Properties.Resources.spd_glitch))
            {
                for (int i = 0; i < katakana.Length; i++)
                {
                    var xptr = i * 16 % 640;
                    var yptr = ((int)(i / 40)) * 24;
                    Rectangle cloneRect = new Rectangle(xptr, yptr, 15, 24);
                    katakana_glitch[i] = raw.Clone(cloneRect, PixelFormat.Format32bppArgb);
                }
            }
        }

        public static List<string> CreditsStrings = new List<string>()
        {
            "The Matrix®",
            ".NET WinForms screensaver",
            "© 2024 by Gordon Freeman [gfr20141201@gmail.com]",
            "inspired by and dedicated to Burning_Thornbush [burning_thornbush@yahoo.com]",
        };

        public static List<string> ConsoleStrings = new List<string>()
        {
            "Wake up, Neo...",
            "You have shitted. The Matrix is full of your shit...",
            "Follow the white powder",
            "Knock, knock, Neo",
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
            if (ch == ',')
            {
                res = 453;
            }
            if (ch == '_')
            {
                res = 460;
            }
            return res;
        }

        public static bool CheckIndexEmptySprite(int katakanaIndex)
        {
            return EmptySprites.Contains(katakanaIndex);
        }

    }
}
