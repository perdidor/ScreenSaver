using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSaver
{
    public class ScreenSaverForm : Form
    {
        private Point MouseXY;
        private readonly int ScreenNumber;

        private PictureBox SSPictureBox;
        public Bitmap SSBitmap;

        public int SpriteColumns = 0;
        public int SpriteRows = 0;

        public int[,] UsedKatakanaIndexes = null;

        private ColBitMap[] ColBitMaps = null;

        int CurrentCreditLine = 0;
        int CurrentLinePos = 0;
        int CreditLineCyclesShown = 0;
        int BeforeChangeCyclesShown = 0;

        public bool DoShowCredits = true;  //show credits on start
        public bool DoShowConsole = false;

        private long Cycles = 0;

        public void ReorderColumnIndexesDownMove(int colIndex, bool clearfirst = false)
        {
            for (int i = SpriteRows - 1; i > 0; i--)
            {
                UsedKatakanaIndexes[colIndex, i] = UsedKatakanaIndexes[colIndex, i - 1];
            }
            if (clearfirst)
            {
                UsedKatakanaIndexes[colIndex, 0] = -1;
            }
        }



        public ScreenSaverForm(int scrn)
		{
            ScreenNumber = scrn;
            InitializeComponent();
        }

		protected override void Dispose(bool disposing)
		{
            ScreenInstances.ScrForms.Remove(this);
            base.Dispose(disposing);
		}

		private void ScreenSaverForm_Load(object sender, EventArgs e)
		{
            Bounds = Screen.AllScreens[ScreenNumber].Bounds;
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Cursor.Hide();
                TopMost = true;
            }
            SSBitmap = new Bitmap(Bounds.Width, Bounds.Height);
            SpriteColumns = (int)Math.Ceiling((double)SSBitmap.Width / 16);
            SpriteRows = (int)Math.Ceiling((double)SSBitmap.Height / 24);
            UsedKatakanaIndexes = new int[SpriteColumns, SpriteRows];
            for (int i = 0; i < SpriteColumns; i++)
            {
                for (int u = 0; u < SpriteRows; u++)
                {
                    UsedKatakanaIndexes[i,u] = -1;
                }
            }
            ColBitMaps = new ColBitMap[SpriteColumns];
            Array.Clear(ColBitMaps, 0, SpriteColumns);
        }

        public void DoShowScreenSaverCycle()
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    if (DoShowCredits)
                    {
                        ShowCredits();
                    }
                    else
                    if (DoShowConsole)
                    {
                        ShowConsoleStrings();
                    }
                    else
                    {
                        using (Graphics graphics = Graphics.FromImage(SSBitmap))
                        {
                            int modcount = SpriteColumns / 10;    //we will modify only 10% of columns per step
                            for (int i = 0; i < modcount; i++)
                            {
                                var selectedcol = ScreenInstances.EntropySrc.Next(SpriteColumns);
                                TransformColumn(selectedcol);
                                if (ColBitMaps[selectedcol].IsModified)
                                {
                                    Point sp = new Point(selectedcol * 16, 0);
                                    graphics.DrawImageUnscaled(ColBitMaps[selectedcol].ColumnBitMap, sp);
                                }
                            }
                        }
                    }

                    if (!DoShowCredits)
                    {
                        if (Cycles < 3000)
                        {
                            Cycles++;
                        }
                        else
                        {
                            Cycles = 0;
                            DoShowCredits = true;
                            using (Graphics graphics = Graphics.FromImage(SSBitmap))
                            {
                                graphics.Clear(Color.Black);
                            }
                        }
                    }

                    SSPictureBox.Image = SSBitmap;
                    SSPictureBox.Refresh();
                });
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                var s = ex.ToString();
            }
        }

        private void ShowCredits()
        {
            var startcol = (SpriteColumns - ScreenInstances.CreditsStrings[CurrentCreditLine].Length) / 2;
            var row = SpriteRows / 2 + 1;
            using (Graphics graphics = Graphics.FromImage(SSBitmap))
            {
                if (CreditLineCyclesShown < 10)
                {
                    for (int i = 0; i <= CurrentLinePos; i++)
                    {
                        if (i < CurrentLinePos)
                        {
                            Point sp = new Point((startcol + CurrentLinePos) * 16, row * 24);

                            graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(ScreenInstances.GetLetterSprite(ScreenInstances.CreditsStrings[CurrentCreditLine][i]), false), sp);

                        }
                        Point sp2 = new Point((startcol + CurrentLinePos + 1) * 16, row * 24);
                        graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(114 + 120 * (CreditLineCyclesShown % 4), false), sp2);
                        CreditLineCyclesShown++;
                    }
                }
                else
                {
                    CreditLineCyclesShown = 0;
                    if (CurrentLinePos < ScreenInstances.CreditsStrings[CurrentCreditLine].Length)
                    {
                        CurrentLinePos++;
                    }
                    else
                    {
                        if (BeforeChangeCyclesShown < 10)
                        {
                            Point sp2 = new Point((startcol + CurrentLinePos + 1) * 16, row * 24);
                            graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(114 + 120 * (BeforeChangeCyclesShown % 4), false), sp2);
                            BeforeChangeCyclesShown++;
                            return;
                        }
                        else
                        {
                            BeforeChangeCyclesShown = 0;
                        }
                        CurrentLinePos = 0;
                        if (CurrentCreditLine < ScreenInstances.CreditsStrings.Count)
                        {
                            SolidBrush shadowBrush = new SolidBrush(Color.Black);
                            Rectangle clearrect = new Rectangle(startcol * 16, row * 24, (ScreenInstances.CreditsStrings[CurrentCreditLine].Length + 2) * 16, 24);
                            graphics.FillRectangle(shadowBrush, clearrect);
                            CurrentCreditLine++;
                            if (CurrentCreditLine == ScreenInstances.CreditsStrings.Count)
                            {
                                CurrentCreditLine = 0;
                                DoShowCredits = false;
                                DoShowConsole = true;
                            }
                        }
                    }
                }
            }
        }

        private void ShowConsoleStrings()
        {
            using (Graphics graphics = Graphics.FromImage(SSBitmap))
            {
                if (CreditLineCyclesShown < 10)
                {
                    for (int i = 0; i <= CurrentLinePos; i++)
                    {
                        if (i < CurrentLinePos)
                        {
                            Point sp = new Point(CurrentLinePos * 16, 0);
                            graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(ScreenInstances.GetLetterSprite(ScreenInstances.ConsoleStrings[CurrentCreditLine][i]), false), sp);
                        }
                        Point sp2 = new Point((CurrentLinePos + 1) * 16, 0);
                        graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(114 + 120 * (CreditLineCyclesShown % 4), false), sp2);

                        CreditLineCyclesShown++;
                    }
                }
                else
                {
                    CreditLineCyclesShown = 0;
                    if (CurrentLinePos < ScreenInstances.ConsoleStrings[CurrentCreditLine].Length)
                    {
                        CurrentLinePos++;
                    }
                    else
                    {
                        if (BeforeChangeCyclesShown < 10)
                        {
                            Point sp2 = new Point((CurrentLinePos + 1) * 16, 0);
                            graphics.DrawImageUnscaled(ScreenInstances.GetKatakanaSprite(114 + 120 * (BeforeChangeCyclesShown % 4), false), sp2);
                            BeforeChangeCyclesShown++;
                            return;
                        }
                        else
                        {
                            BeforeChangeCyclesShown = 0;
                        }
                        CurrentLinePos = 0;
                        if (CurrentCreditLine < ScreenInstances.CreditsStrings.Count)
                        {
                            SolidBrush shadowBrush = new SolidBrush(Color.Black);
                            Rectangle clearrect = new Rectangle(0, 0, (ScreenInstances.ConsoleStrings[CurrentCreditLine].Length + 2) * 16, 24);
                            graphics.FillRectangle(shadowBrush, clearrect);

                            CurrentCreditLine++;
                            if (CurrentCreditLine == ScreenInstances.ConsoleStrings.Count)
                            {
                                CurrentCreditLine = 0;
                                DoShowConsole = false;
                            }
                        }
                    }
                }
            }
        }

        private void TransformColumn(int colIndex)
        {
            if (ColBitMaps[colIndex] == null)
            {
                ColBitMaps[colIndex] = new ColBitMap(colIndex, ScreenNumber);
            }

            ColBitMaps[colIndex].IsModified = false;

            var first_index = ColBitMaps[colIndex].FirstSpriteIndex;
            var sprites = ColBitMaps[colIndex].SpriteCount;

            if ((first_index == 0 //if we have symbol in top row
                || ScreenInstances.EntropySrc.Next(2) == 1)  //or we add more symbols randomly with 50% chance
                && (sprites <= (int)(SpriteRows * 0.5))) //main condition: column filled less or equal than 50% with non empty symbols
            {
                ColBitMaps[colIndex].AddTopSprite();    //move all column down by 1 row, insert symbol to top row
            }
            else
            {
                ColBitMaps[colIndex].InPlaceMutation(); //change symbols in-place
            }
        }

		private void OnMouseEvent(object sender, MouseEventArgs e)
		{
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                if (!MouseXY.IsEmpty)
                {
                    if (MouseXY != new Point(e.X, e.Y))
                    {
                        ScreenInstances.TerminateSS();
                    }
                    if (e.Clicks > 0)
                    {
                        ScreenInstances.TerminateSS();
                    }
                }
                MouseXY = new Point(e.X, e.Y);
            }
        }
		
		private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
		{
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                ScreenInstances.TerminateSS();
            }
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SSPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.SSPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SSPictureBox
            // 
            this.SSPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SSPictureBox.Location = new System.Drawing.Point(0, 0);
            this.SSPictureBox.Name = "SSPictureBox";
            this.SSPictureBox.Size = new System.Drawing.Size(1024, 768);
            this.SSPictureBox.TabIndex = 0;
            this.SSPictureBox.TabStop = false;
            this.SSPictureBox.WaitOnLoad = true;
            this.SSPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseEvent);
            this.SSPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseEvent);
            // 
            // ScreenSaverForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.SSPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreenSaverForm";
            this.Text = "ScreenSaver";
            this.Load += new System.EventHandler(this.ScreenSaverForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenSaverForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.SSPictureBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
	}
}
