using System.Drawing;

namespace ScreenSaver
{
    internal class ColBitMap
    {
        public Bitmap ColumnBitMap { get; set; }
        public bool IsModified { get; set; }
        private readonly int ColumnIndex;
        private readonly int ScrNumber;

        public ColBitMap(int columnIndex, int scrNumber)
        {
            ScrNumber = scrNumber;
            ColumnIndex = columnIndex;
            ColumnBitMap = new Bitmap(16, ScreenInstances.ScrForms[ScrNumber].SSBitmap.Height);
            IsModified = false;
        }

        private Bitmap GetKatakanaSprite(int index) 
        {
            Bitmap resbitmap = null;
            if (ScreenInstances.EntropySrc.Next(1000) >= 995)    //chance to glitch is 0.1%
            {
                resbitmap = ScreenInstances.katakana_glitch[index];
            }
            else
            {
                resbitmap = ScreenInstances.katakana[index];
            }
            return resbitmap;
        }

        public void InPlaceMutation()
        {
            var movesdown = ScreenInstances.EntropySrc.Next(1,4);
            for (int i = 0; i < movesdown; i++)
            {
                MoveDown(true);
            }
            using (Graphics graphics = Graphics.FromImage(ColumnBitMap))
            {
                for (int i = 0; i < ScreenInstances.ScrForms[ScrNumber].SpriteRows; i++)
                {
                    var mutationdice = ScreenInstances.EntropySrc.Next(2);
                    var mutationdice2 = ScreenInstances.EntropySrc.Next(3);
                    if (!ScreenInstances.CheckIndexEmptySprite(ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, i]) && mutationdice2 >= 0)
                    {
                        switch (mutationdice)
                        {
                            case 0:
                                {
                                    ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, i] = (ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, i] + 120) % 480;
                                }
                                break;
                            case 1:
                                {
                                    ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, i] = ScreenInstances.EntropySrc.Next(480);
                                }
                                break;
                            default:
                                break;
                        }
                        Bitmap resbitmap = GetKatakanaSprite(ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, i]);

                        Point destp = new Point(0, i * 24);
                        graphics.DrawImageUnscaled(resbitmap, destp);
                    }
                }
            }
        }

        public void MoveDown(bool clearfisrst = false)
        {
            Bitmap sprites = null;

            var findex = FirstSpriteIndex;

            if (findex == -1)
            {
                return;
            }

            var spritesheight = ScreenInstances.ScrForms[ScrNumber].SSBitmap.Height - (findex + 1) * 24;

            using (var cbm = new Bitmap(ColumnBitMap))
            {
                using (Graphics graphics = Graphics.FromImage(ColumnBitMap))
                {
                    Rectangle cloneRect = new Rectangle(0, findex * 24, 16, spritesheight);

                    sprites = cbm.Clone(cloneRect, ScreenInstances.ScrForms[ScrNumber].SSBitmap.PixelFormat);
                    Point clearp = new Point(0, findex * 24);
                    Point destp = new Point(0, (findex + 1) * 24);
                    graphics.DrawImageUnscaled(ScreenInstances.katakana[119], clearp);
                    graphics.DrawImageUnscaled(sprites, destp);
                    ScreenInstances.ScrForms[ScrNumber].ReorderColumnIndexesDownMove(ColumnIndex, clearfisrst);
                }
            }

            IsModified = true;
        }

        public void AddTopSprite(int index = -1)
        {
            MoveDown(false);
            using (Graphics graphics = Graphics.FromImage(ColumnBitMap))
            {
                Point sp = new Point(0, 0);
                var selectedindex = index == -1 ? ScreenInstances.EntropySrc.Next(ScreenInstances.katakana.Length) : index;
                Bitmap resbitmap = GetKatakanaSprite(selectedindex);
                graphics.DrawImageUnscaled(resbitmap, sp);
                ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, 0] = selectedindex;
            }
        }

        public int FirstSpriteIndex
        {
            get
            {
                var res = -1;
                for (int crow = 0; crow < ScreenInstances.ScrForms[ScrNumber].SpriteRows; crow++)
                {
                    if (!ScreenInstances.CheckIndexEmptySprite(ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, crow]))
                    {
                        res = crow;
                        break;
                    }
                }
                return res;
            }
        }

        public int LastSpriteIndex
        {
            get
            {
                var res = -1;
                for (int crow = ScreenInstances.ScrForms[ScrNumber].SpriteRows - 1; crow >= 0; crow--)
                {
                    if (!ScreenInstances.CheckIndexEmptySprite(ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, crow]))
                    {
                        res = crow;
                        break;
                    }
                }
                return res;
            }
        }

        public int SpriteCount
        {
            get
            {
                var res = 0;
                for (int crow = 0; crow < ScreenInstances.ScrForms[ScrNumber].SpriteRows; crow++)
                {
                    if (!ScreenInstances.CheckIndexEmptySprite(ScreenInstances.ScrForms[ScrNumber].UsedKatakanaIndexes[ColumnIndex, crow]))
                    {
                        res++;
                    }
                }
                return res;
            }
        }

    }
}
