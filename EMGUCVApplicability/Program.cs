using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace EMGUCVApplicability
{
    class Program
    {
        static void Main()
        {
            var SilvTest = new Image<Bgr, byte>("H:/Desktop/BrawlRankSilver.png");
            var GoldTest = new Image<Bgr, byte>("H:/Desktop/BrawlRankGold.png");
            /*
            var outsilv = GetRanks(SilvTest);
            for (int i = 0; i < outsilv.Count; i++) {
                Console.WriteLine("Silver Test: " + outsilv[i]);
            }
            */
            var outgold = GetRanks(GoldTest);
            for (int i = 0; i < outgold.Count; i++) {
                Console.WriteLine("Gold Test: " + outgold[i]);
            }
            Console.ReadKey();
        }

        static void CapVid()
        {
            var Vid = new VideoCapture("H:/Desktop/BrawlVid.mp4");

            var OldFrame = Vid.QueryFrame().ToImage<Bgr, byte>();
            var NewestFrame = Vid.QueryFrame().ToImage<Bgr, byte>();

            var Something = GetSomething(NewestFrame);
            var OldSomething = GetSomething(OldFrame);
            Console.Write("Initialised\n");

            do {
                OldFrame = NewestFrame;
                NewestFrame = Vid.QueryFrame().ToImage<Bgr, byte>();

                OldSomething = GetSomething(OldFrame);
                Something = GetSomething(NewestFrame);

                OldSomething.Dilate(10);
                OldSomething.Erode(15);

                Something.Dilate(10);
                Something.Erode(15);

                CvInvoke.Imshow("Source", NewestFrame);
                CvInvoke.Imshow("Something", Something);
                CvInvoke.Imshow("Diff Something", GetDiff(Something, OldSomething));

                int key = CvInvoke.WaitKey(0);
                if (key == 27) { // esc key
                    Environment.Exit(0);
                }
            } while (NewestFrame != null);
            Console.Write("Done\n");
        }

        static Image<Gray, byte> GetSomething(Image<Bgr, byte> Input) {
            return Input.InRange(new Bgr(100, 100, 100), new Bgr(200, 200, 200));
        }

        static Image<Gray, byte> GetDiff(Image<Gray, byte> InputA, Image<Gray, byte> InputB) {
            return InputA.AbsDiff(InputB);
        }

        static List<string> GetRanks(Image<Bgr, byte> Frame)
        {
            var GoldThresh = Frame.ThresholdBinary(new Bgr(230, 184, 72), new Bgr(237, 193, 104));
            var SilvThresh = Frame.ThresholdBinary(new Bgr(159, 138, 130), new Bgr(167, 176, 176));

            var GoldFrameBgrAverage = GoldThresh.GetAverage();
            var GoldOut = GoldFrameBgrAverage.Blue + GoldFrameBgrAverage.Red + GoldFrameBgrAverage.Green;

            var SilverFrameBgrAverage = SilvThresh.GetAverage();
            var SilverOut = SilverFrameBgrAverage.Blue + SilverFrameBgrAverage.Red + SilverFrameBgrAverage.Green;

            CvInvoke.Imshow("Silver", SilvThresh);
            CvInvoke.Imshow("Gold", GoldThresh);

            if (Math.Abs(GoldOut - SilverOut) < 5.0) {
                return new List<string>() { "Gold", "Silver" };
            } else if (GoldOut > SilverOut) {
                return new List<string>() { "Gold" };
            } else if (SilverOut > GoldOut) {
                return new List<string>() { "Silver" };
            } else {
                return new List<string>();
            }
        }
    }
}
