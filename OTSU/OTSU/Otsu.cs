using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OTSU
{
    class Otsu
    {
        Bitmap grayBitmap;
        public Bitmap outputBitmap;
        uint size;        
        public int[] optimalThreshold = new int[3];
        Color otsuColor1;
        Color otsuColor2;
        Color otsuColor3;
        Color otsuColor4;
        public ProgressBar state;
        int _classCount;        
        int classCount
        {
            get
            {
                return _classCount;
            }
            set
            {
                _classCount = value<2 ? 2 : value > 4 ? 4 : value;
            }
        }
        int[] histogram = new int[256];

        public Otsu(Bitmap input,int classCount,ProgressBar state)
        {
            //Otsu gri imajlarda çalıştığı için öncelikle gelen imajı griye çeviriyoruz.
            this.grayBitmap = ConvertToGray(input);
            this.outputBitmap = (Bitmap)input.Clone();
            this.size = (uint)(input.Height * input.Width);
            this.classCount = classCount;
            this.state = state;

            //binary otsu kullanımı için 1 ve 2.renkleri siyah beyaz yapacağız, 3 4   kırmız , mavi  olsun
            otsuColor1 = Color.FromArgb(0,0,0);
             otsuColor2 = Color.FromArgb(255, 255, 255); ;
             otsuColor3 = Color.FromArgb(255, 0, 0);
             otsuColor4 = Color.FromArgb(0, 0, 255);
 
        }

        public void Run()
        {
            state.Refresh();
            state.Value = 1;
            state. Refresh();
            SetHistogram();
            state.Value++;
            state.Refresh();
            CalculateOtsu();
            state.Value++;
            state.Refresh();
            Console.WriteLine($"optimalThreshold1:\t{optimalThreshold[0]}");
            Console.WriteLine($"optimalThreshold2:\t{optimalThreshold[1]}");
            Console.WriteLine($"optimalThreshold3:\t{optimalThreshold[2]}");            
            CreateOutputBitmap();
            state.Value++;
            state.Refresh();
        }

        private Bitmap ConvertToGray(Bitmap input)
        {
            Bitmap bt = (Bitmap)input.Clone();
            for (int y = 0; y < bt.Height; y++)
            {
                for (int x = 0; x < bt.Width; x++)
                {
                    Color c = bt.GetPixel(x, y);

                    var avg = getColorAvgValue(c);

                    

                    bt.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            return bt;
        }
        private int getColorAvgValue(Color c)
        {
            int r = c.R;
            int g = c.G;
            int b = c.B;
            int result = (r + g + b) / 3;
            //aslında bu grileştirme işlemini insan gözü için yapsaydık R .3, G .59, B  .11 oranları daha anlamlı olabilirdi; biz histogram için yaptığımızdan doğrudan 3e böldük
           // result = (int) (0.3 * r + 0.59 * g + 0.11 * b);

            result = result >= 255 ? 255 : result <= 0 ? 0 : result;

            return result;
        }
        private void SetHistogram()
        {
            for (int i = 1; i < grayBitmap.Width - 1; i++)
            {
                for (int j = 1; j < grayBitmap.Height - 1; j++)
                {
                    var value = getColorAvgValue(grayBitmap.GetPixel(i, j));                

                   
                    histogram[value] += 1;
            
                }
            }
        }

        private int CheckClass(int classIndex)
        {
            return (this.classCount - classIndex >= 0 ? 1 : 0);
        }
        private void CalculateOtsu()
        {            
            double[] p = new double[256];

            //toplam olasıklık yoğunluklu ortalama gri seviye değeri
            double uT = 0;
            for (int i = 0; i < 256; i++)
            {
                p[i] = (double)histogram[i] / (double)size;
                uT += i * p[i];
            }
            double maxBetweenVar = 0;

            double w0 = 0;
            double m0 = 0;
            double c0 = 0;
            double p0 = 0;

            double w1 = 0;
            double m1 = 0;
            double c1 = 0;
            double p1 = 0;

            double w2 = 0;
            double m2 = 0;
            double c2 = 0;
            double p2 = 0;

            double w3 = 0;
            double m3 = 0;
            double c3 = 0;
            double p3 = 0;
            for (int tr1 = 0; tr1 < 256; tr1++)
            {
                p0 += p[tr1];
                w0 += (tr1 * p[tr1]);
                if (p0 != 0)
                {
                    m0 = w0 / p0;
                }

                c0 = p0 * (m0 - uT) * (m0 - uT);

                c1 = 0;
                w1 = 0;
                m1 = 0;
                p1 = 0;
                for (int tr2 = tr1+1 ; tr2 < 256; tr2++)
                {

                    p1 += p[tr2];
                    w1 += (tr2 * p[tr2]);
                    if (p1 != 0)
                    {
                        m1 = w1 / p1;
                    }

                    c1 = p1 * (m1 - uT) * (m1 - uT);


                    c2 = 0;
                    w2 = 0;
                    m2 = 0;
                    p2 = 0;
                    for (int tr3 = tr2 + 1; tr3 < 256; tr3++)
                    {

                        p2 += p[tr3];
                        w2 += (tr3 * p[tr3]);
                        if (p2 != 0)
                        {
                            m2 = w2 / p2;
                        }

                        c2 = p2 * (m2 - uT) * (m2 - uT);

                      p3 = 0;
                      w3 = 0;
                      m3 = 0;
                        c3 = 0;
                        for (int tr4 = tr3 + 1; tr4 < 256; tr4++)
                        {


                            p3 += p[tr4];
                            w3 += (tr4 * p[tr4]);
                            if (p3 != 0)
                            {
                                m3 = w3 / p3;
                            }

                            c3 = p3 * (m3 - uT) * (m3 - uT);



                            double p4 = 1 - (p0 + p1 + p2 + p3);
                            double w4 = uT - (w0 + w1 + w2 + w3);
                            double m4 = w4 / p4;
                            double c4 = p4 * (m4 - uT) * (m4 - uT);

                            double c = c0 + c1  + c2 * CheckClass(3) + c3 * CheckClass(4) + c4 * CheckClass(5);

                            if (maxBetweenVar < c)
                            {
                                maxBetweenVar = c;
                                optimalThreshold[0] = tr1;
                                optimalThreshold[1] = tr2;
                                optimalThreshold[2] = tr3;                                
                            }
                        }
                    }
                }
            }
        }

        private void CreateOutputBitmap()
        {
            
            

            for (int i = 0; i < outputBitmap.Width; i++)
            {
                for (int j = 0; j < outputBitmap.Height; j++)
                {
                    var value = getColorAvgValue(grayBitmap.GetPixel(i, j));
                    var resultColor = otsuColor1;
                    
                    if (value > optimalThreshold[0])
                    {
                        resultColor = otsuColor2;
                    }
                    if (value > optimalThreshold[1])
                    {
                        resultColor = otsuColor3;
                    }
                    if (value >= optimalThreshold[2])
                    {
                        resultColor = otsuColor4;
                    }


                    outputBitmap.SetPixel(i, j, resultColor);
                }
            }
            
        }
    }
}
