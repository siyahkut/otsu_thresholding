using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace OTSU
{
    public partial class MainForm : Form
    {
        private Bitmap selected;
        public MainForm()
        {
            InitializeComponent();
            this.numericUpDown1.Minimum = 2;
            this.numericUpDown1.Maximum = 4;
        }

        private void loadImg_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Bitmap.FromFile(openFileDialog1.FileName);
                selected = (Bitmap)pictureBox1.Image.Clone();
                selected = ConvertToNonIndexed(selected, PixelFormat.Format24bppRgb);
            }
        }
        private Bitmap ConvertToNonIndexed(Image img, PixelFormat fmt)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height, fmt);
            Graphics gr = Graphics.FromImage(bmp);
            gr.DrawImage(img, 0, 0);
            gr.Dispose();
            return bmp;
        }
        private void runOtsu_Click(object sender, EventArgs e)
        {
            Otsu otsu = new Otsu(selected,(int)this.numericUpDown1.Value, progressBar1);
            
            otsu.Run();
            listBox1.Items.Clear();
            for (int t=0;t<this.numericUpDown1.Value-1;t++)
            {
                listBox1.Items.Add(otsu.optimalThreshold[t]);
            }
            pictureBox2.Image = (Bitmap)otsu.outputBitmap.Clone() ;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
