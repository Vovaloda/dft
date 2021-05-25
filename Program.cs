using System;
using System.IO;



/* для подключения System.Drawing в своем проекте правой в проекте нажать правой кнопкой по Ссылкам -> Добавить ссылку
    отметить галочкой сборку System.Drawing    */
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Linq;

namespace IMGapp
{
    class Coord {
        public float x = 0;
        public float y = 0;
        public float kx, b;
    }

    class Program
    {
        public static Complex[] DFT(Complex[] x, double n = 1)
        {
            int N = x.Length;
            Complex[] G = new Complex[N];
            for(int u = 0; u < N; ++u)
            {
                for(int k = 0; k < N; ++k)
                {
                    double fi = -2.0 * Math.PI * u * k / N;
                    G[u] += (new Complex(Math.Cos(fi), Math.Sin(fi)) * x[k]);
                }
                G[u] = n * G[u];
            }
            return G;
        }
        
        /////////////////////////////////////////////
        static void Main(string[] args)
        {
            using (var img_3 = new Bitmap("..\\..\\in3.jpg"))    //открываем картинку     
            {
                
                var w = img_3.Width;
                var h = img_3.Height;
                Complex[,] furier_arr = new Complex[h, w];
                
                for (int i = 0; i < h-1; ++i)
                {
                    double[] inputArr = new Double[h];
                    for (int j = 0; j < w-1; ++j)
                    {
                        var pix1 = img_3.GetPixel(j, i);
                        int r1 = (int)(pix1.R);
                        int g1 = (int)(pix1.G);
                        int b1 = (int)(pix1.B);

                        inputArr[j] = (r1 + g1 + b1) / 3 * Math.Pow(-1, i + j);

                    }
                    var N = inputArr.Length;
                    var c_input_arr = inputArr.Select(x => new Complex(x, 0)).ToArray();
                    var furier = DFT(c_input_arr, 1.0 / N);
                    for (int j = 0; j < w; ++j)
                    {
                        furier_arr[i, j] = furier[j];
                    }
                }
                for (int i = 0; i < h; ++i)
                {
                    Complex[] c_input_arr = new Complex[h];
                    var N = c_input_arr.Length;
                    for (int j = 0; j < w; ++j)
                    {
                        c_input_arr[j] = furier_arr[j, i];
                    }
                    var furier = DFT(c_input_arr, 1.0 / N);
                    for (int j = 0; j < w; ++j)
                    {
                        furier_arr[j, i] = furier[j];
                    }
                }




                using (var img_out = new Bitmap(w, h))   //создаем пустое изображение размером с исходное для сохранения результата
                {
                   
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {

                            int r1 = (int)Clamp(furier_arr[j, i].Magnitude * 100, 0, 255);
                            int g1 = (int)Clamp(furier_arr[j, i].Magnitude * 100, 0, 255);
                            int b1 = (int)Clamp(furier_arr[j, i].Magnitude * 100, 0, 255);

                            var pix1 = Color.FromArgb((int)r1, (int)g1, (int)b1);
                            img_out.SetPixel(j, i, pix1);
                        }
                    }

                    img_out.Save("..\\..\\Furier.jpg");
                }
                

    }
    



            Console.WriteLine("Закончил работу");
            Console.ReadKey();
                }//static void Main(string[] args)

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
