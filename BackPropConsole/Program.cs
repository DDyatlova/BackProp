using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> numNeuro = new List<int>{ 10,50,15, 1 };
            Network net = new Network(numNeuro, 4, 0.7*Math.Pow(2, 1/10));
            List < List<double>> inX= new List<List<double>>();
            List<double> Ycorrect = new List<double>();
            double a = 100;
             Random random = new Random();
             double min = 0;
             double max = 1;

            List<double> allStuff = new List<double>();
             for (int i=0; i<1000; i++)
             {
                allStuff.Add((double)i / 1000);
                allStuff.Add((double)i / 1000);
                allStuff.Add((double)i / 1000);
                allStuff.Add((double)i / 1000);
             }
             allStuff = allStuff.OrderBy(x => Guid.NewGuid()).ToList();
             for (int i=0; i < 1000*4; i=i+4)
             {
                 inX.Add(new List<double> { allStuff[i], allStuff[i+1], allStuff[i+2], allStuff[i+3] });
                Ycorrect.Add(allStuff[i]);//(Math.Pow(allStuff[i], 2) + Math.Pow(allStuff[i+1], 2) + Math.Pow(allStuff[i+2], 2) + Math.Pow(allStuff[i+3], 2))/a);
             }
            allStuff.Clear();
            Teacher T = new Teacher(net, 0.00001);
            net=T.teach(inX, Ycorrect);

            List<double> expList = new List<double> {0.9, 0.6, 0.3, 0.4 };
            double result=net.CountResult(expList);
            double etalon = 0;
            for (int i = 0; i < 4; i++)
            {
                etalon = etalon + expList[i] * expList[i];
            }
            Console.WriteLine("test");
            Console.Write(result);
            Console.Write("    ");
            Console.Write(etalon/a);
        }
    }
}
