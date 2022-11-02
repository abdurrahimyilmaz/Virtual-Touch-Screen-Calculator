
//Abdurrahim Yılmaz 1606A041 a.rahim.yilmaz@hotmail.com

using System;
using System.Drawing;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
                
        private FilterInfoCollection cihazlar;     
        private VideoCaptureDevice kamera;         

        double? ilksayi = 0, sonsayi = 0;
        string islem = "";
        double? sonuc;
        int sayac1 = 0;         //İlk sayıyı denetler.
        int sayac = 0;          //Hesap makinesinin görünmesi için görüntüyü denetler.
        int sayac2 = 0;         //İşlemin sonucu hesaplanıp hesaplanmadığını denetler.
        string esittir;
        string sonuconcesi;

        public Form1()
        {
            
            InitializeComponent();
           
        }       

        private void Form1_Load(object sender, EventArgs e)
        {   
            Control.CheckForIllegalCrossThreadCalls = false;                
            cihazlar = new
            FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            kamera = new
            VideoCaptureDevice(cihazlar[0].MonikerString);                 
            
            kamera.NewFrame += new NewFrameEventHandler(kamera_klonlama);                   
            kamera.Start();            
        }        

        public void kamera_klonlama(object sender, NewFrameEventArgs eventArgs)      
        {
            Bitmap klon = (Bitmap)eventArgs.Frame.Clone();          //Yapılacak her faklı işlem için kameradan alınan görüntü bitmape atandı.
            Bitmap cisim = (Bitmap)eventArgs.Frame.Clone();           
            Bitmap asama = (Bitmap)eventArgs.Frame.Clone();

            say(asama);         //Kameranın önüne gelindiğinde hesap makinesinin görünmesini denetleyen fonksiyon.

            gorun(klon);        //Kameranın önüne gelindiğinde koşul sağlanırsa hesap makinesini gösterecek fonksiyon.

            ColorFiltering filtre = new ColorFiltering();          //Kırmızı cisimin tespiti için renk filtreleme yapıldı.
            filtre.Red = new IntRange(100, 255);            
            filtre.Blue = new IntRange(0, 25);              
            filtre.Green = new IntRange(0, 25);          
            filtre.ApplyInPlace(cisim);

            if (sayac == 1) anafonk(cisim);         //Hesap makinesinin fonksiyonu.Eşik geçilince görünecek.
        }               

        public void say(Bitmap image)
        {
            /*  
             *  Denetleme işlemi için alınan görüntü üzerinde eşikleme yaparak gri kanalın
             *  histogramının ortalaması ile bir eşik oluşturdum.Görüntüde beyaz haricinde ki
             *  renkler ne kadar artarsa ortalama o kadar düşeceğinden belli bir sayının altından
             *  sonrası için hesap makinesi görünüp işlem yapılabilecek.                           
             */

            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);                
            Bitmap grayImage = filter.Apply(image);
            
            Threshold filter1 = new Threshold(100);            
            filter1.ApplyInPlace(grayImage);

            ImageStatistics stat = new ImageStatistics(grayImage);            
            Histogram gri = stat.Gray;

            if (gri.Mean < 215) sayac = 1;
            else sayac = 0;

            string sayi = sayac.ToString();
            sayacmetin.Text = sayi;
        }

        public void gorun(Bitmap image)
        {           
            // Hesap makinesi çerçevelerinin ekran üzerinde çizdirilmesi.
             
            Pen pen = new Pen(Color.Red);
            Font font = new Font("Arial", 33);
            Graphics g = Graphics.FromImage(image);             

            if (sayac == 1)
            {                
                g.DrawRectangle(pen, 50, 45, 265, 370);

                g.DrawRectangle(pen, 55, 50, 255, 100);

                g.DrawRectangle(pen, 55, 155, 60, 60); g.DrawRectangle(pen, 120, 155, 60, 60); g.DrawRectangle(pen, 185, 155, 60, 60); g.DrawRectangle(pen, 250, 155, 60, 60);

                g.DrawRectangle(pen, 55, 220, 60, 60); g.DrawRectangle(pen, 120, 220, 60, 60); g.DrawRectangle(pen, 185, 220, 60, 60); g.DrawRectangle(pen, 250, 220, 60, 60);

                g.DrawRectangle(pen, 55, 285, 60, 60); g.DrawRectangle(pen, 120, 285, 60, 60); g.DrawRectangle(pen, 185, 285, 60, 60); g.DrawRectangle(pen, 250, 285, 60, 60);

                g.DrawRectangle(pen, 55, 350, 60, 60); g.DrawRectangle(pen, 120, 350, 60, 60); g.DrawRectangle(pen, 185, 350, 60, 60); g.DrawRectangle(pen, 250, 350, 60, 60);

                //      Çerçeve
                //      Ekran
                // 1    2    3    +
                // 4    5    6    -
                // 7    8    9    *
                // 0    C    =    /

                g.DrawString("1", font, Brushes.Red, 67, 162); g.DrawString("2", font, Brushes.Red, 132, 162); g.DrawString("3", font, Brushes.Red, 196, 162); g.DrawString("+", font, Brushes.Red, 259, 162);

                g.DrawString("4", font, Brushes.Red, 67, 227); g.DrawString("5", font, Brushes.Red, 132, 227); g.DrawString("6", font, Brushes.Red, 196, 227); g.DrawString("-", font, Brushes.Red, 266, 227);

                g.DrawString("7", font, Brushes.Red, 67, 292); g.DrawString("8", font, Brushes.Red, 132, 292); g.DrawString("9", font, Brushes.Red, 196, 292); g.DrawString("*", font, Brushes.Red, 265, 292);

                g.DrawString("0", font, Brushes.Red, 67, 357); g.DrawString("C", font, Brushes.Red, 130, 357); g.DrawString("=", font, Brushes.Red, 196, 357); g.DrawString("/", font, Brushes.Red, 266, 357);
            }
            pictureBox1.Image = image;

        }

        public void anafonk(Bitmap image)
        {
            /*
             *  Bu fonksiyon da kırmızı cismin tespit edilerek konum ve alan verisini kullanarak
             *  belli eşiklerin üzerine geçtiğinde bulunduğu konumda ki sayıyı alacak veya işlemi
             *  yapacak.
             */

            BlobCounter blobCounter = new BlobCounter();           
            blobCounter.FilterBlobs = true;       
            blobCounter.MinHeight = 7;        
            blobCounter.MinWidth = 7;         
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            Font font = new Font("Arial", 15);
            Graphics g = pictureBox1.CreateGraphics();
            
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);            
            blobCounter.ProcessImage(grayImage);

            Rectangle[] rects = blobCounter.GetObjectsRectangles();  

            Font font2 = new Font("Arial", 33);
            DateTime Zaman = DateTime.Now;

            int denetim = (Zaman.Millisecond / 35);            
            
            if (rects.Length > 0)         
            {
                Rectangle objectRect = rects[0];                

                Blob[] blobs = blobCounter.GetObjectsInformation();
                int alan = blobs[0].Area;
                if (alan > 700) alan = 701;
                float noktaYY = blobs[0].CenterOfGravity.Y - 70;
                float noktaY = blobs[0].CenterOfGravity.Y - 20;
                float noktaX = blobs[0].CenterOfGravity.X - 20;
                alanmetin.Text = alan.ToString();

                AForge.Point nokta = new AForge.Point();
                nokta = blobs[0].CenterOfGravity;                

                Font yeni = new Font("Arial", 50);
                g.DrawString(".", yeni, Brushes.Black, noktaX, noktaYY);

                if (nokta.X < 115 && nokta.X > 55 && noktaY > 155 && noktaY < 205 && alan > 700 && denetim == 5)
                {          
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "1";
                    else sonsayimetin.Text = sonsayimetin.Text + "1";
                }


                if (nokta.X < 180 && nokta.X > 120 && noktaY > 155 && noktaY < 205 && alan > 700 && denetim == 5)
                {   
                 
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "2";
                    else sonsayimetin.Text = sonsayimetin.Text + "2";
                }

                if (nokta.X < 245 && nokta.X > 185 && noktaY > 155 && noktaY < 205 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "3";
                    else sonsayimetin.Text = sonsayimetin.Text + "3";
                }

                if (nokta.X < 310 && nokta.X > 250 && noktaY > 155 && noktaY < 205 && alan > 700 && denetim == 5 && ilksayimetin.Text != "")
                {
                    islemmetin.Text = "+";
                    ilksayi = Convert.ToDouble(ilksayimetin.Text);
                    islem = "+";
                    sayac1 = 1;
                }

                if (nokta.X < 115 && nokta.X > 55 && noktaY > 220 && noktaY < 280 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "4";
                    else sonsayimetin.Text = sonsayimetin.Text + "4";
                }

                if (nokta.X < 180 && nokta.X > 120 && noktaY > 220 && noktaY < 280 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "5";
                    else sonsayimetin.Text = sonsayimetin.Text + "5";
                }

                if (nokta.X < 245 && nokta.X > 185 && noktaY > 220 && noktaY < 280 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "6";
                    else sonsayimetin.Text = sonsayimetin.Text + "6";
                }

                if (nokta.X < 310 && nokta.X > 250 && noktaY > 220 && noktaY < 280 && alan > 700 && denetim == 5 && ilksayimetin.Text != "")
                {
                    islemmetin.Text = "-";
                    ilksayi = Convert.ToDouble(ilksayimetin.Text);
                    islem = "-";
                    sayac1 = 1;
                }

                if (nokta.X < 115 && nokta.X > 55 && noktaY > 285 && noktaY < 345 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "7";
                    else sonsayimetin.Text = sonsayimetin.Text + "7";
                }

                if (nokta.X < 180 && nokta.X > 120 && noktaY > 285 && noktaY < 345 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "8";
                    else sonsayimetin.Text = sonsayimetin.Text + "8";
                }

                if (nokta.X < 245 && nokta.X > 185 && noktaY > 285 && noktaY < 345 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "9";
                    else sonsayimetin.Text = sonsayimetin.Text + "9";
                }

                if (nokta.X < 310 && nokta.X > 250 && noktaY > 285 && noktaY < 345 && alan > 700 && denetim == 5 && ilksayimetin.Text != "")
                {
                    ilksayi = Convert.ToDouble(ilksayimetin.Text);
                    islemmetin.Text = "*";
                    islem = "*";
                    sayac1 = 1;
                }

                if (nokta.X < 115 && nokta.X > 55 && noktaY > 350 && noktaY < 410 && alan > 700 && denetim == 5)
                {
                    if (sayac1 == 0) ilksayimetin.Text = ilksayimetin.Text + "0";
                    else sonsayimetin.Text = sonsayimetin.Text + "0";
                }

                if (nokta.X < 180 && nokta.X > 120 && noktaY > 350 && noktaY < 410 && alan > 700 && denetim == 5)
                {
                    islemmetin.Clear();                  
                    ilksayimetin.Clear();
                    sonucmetin.Clear();
                    sonsayimetin.Clear();
                    esittirmetin.Clear();
                    sonuc = null;
                }

                if (nokta.X < 310 && nokta.X > 250 && noktaY > 350 && noktaY < 410 && alan > 700 && denetim == 5 && ilksayimetin.Text != "")
                {
                    islemmetin.Text = "/";
                    ilksayi = Convert.ToDouble(ilksayimetin.Text);
                    islem = "/";
                    sayac1 = 1;
                }                

                if (nokta.X < 245 && nokta.X > 185 && noktaY > 350 && noktaY < 410 && alan > 700 && denetim == 5 && sonsayimetin.Text != "")
                {

                    esittirmetin.Text = "=";
                    sonsayi = Convert.ToDouble(sonsayimetin.Text);
                    ilksayimetin.Text = ilksayi.ToString();
                    if (islem == "+")
                    {
                        sonuc = ilksayi + sonsayi;
                    }
                    if (islem == "-")
                    {
                        sonuc = ilksayi - sonsayi;
                    }
                    if (islem == "*")
                    {
                        sonuc = ilksayi * sonsayi;
                    }
                    if (islem == "/")
                    {
                        sonuc = ilksayi / sonsayi;
                    }                   
                    
                    sayac1 = 0;
                    sayac2 = 1;
                }                 

                esittir = sonuc.ToString();
                sonuconcesi = ilksayimetin.Text + islemmetin.Text + sonsayimetin.Text + esittirmetin.Text;
                sonucmetin.Text = ilksayimetin.Text + islemmetin.Text + sonsayimetin.Text + esittirmetin.Text + esittir;
            }

            if (sayac2 == 0) g.DrawString(sonuconcesi, font2, Brushes.Black, 60, 60);
            else g.DrawString(sonucmetin.Text, font2, Brushes.Black, 60, 60);
            
        }        
    }
}
