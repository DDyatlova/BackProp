using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackPropConsole
{
    class Neuro
    {
        public List<double> X;
        public double Y;
        public double error;
        protected double alpha;
        public List<double> W;
        public Neuro()
        {
            X = new List<double>();
            W = new List<double>();
            alpha = 1.0;
        }
        public void setRandomW(int num)
        {
            W.Clear();
            double max = 1;
            double min = -1;
            Random random = new Random();
            for (int i=0; i<num;i++)
            {
                W.Add(random.NextDouble() * (max - min) + min);
            }
        }
        public void setX(List<double> inX)
        {
            this.X.Clear();
            for (int i=0; i<inX.Count();i++)
            {
                this.X.Add(inX[i]);
            }
        }
        public double countResult()
        {
            double x=0;
            for (int i=0; i<this.X.Count();i++)
            {
                x = x + X[i] * W[i];
            }
            Y = 1 / (1 + Math.Exp(-alpha*x));
            return (Y);
        }
        public double countError(List <double> backW, List<double> prevError)
        {
            error = 0;
            for (int i = 0; i < prevError.Count(); i++)
            {
                error = error + prevError[i] * backW[i];
            }
            error = error*Y * (1 - Y);
            //Y = 1 / (1 + Math.Exp(-x));
            return (error);
        }
        public void CorrectW(double h)
        {
            for (int i=0; i<this.W.Count();i++)
            {
                this.W[i] = this.W[i] + X[i] * this.error * h*alpha*this.Y*(1-this.Y);
            }
        }
    }

    class Lay
    {
        public List<Neuro> layout;
        public Lay(int neuroCount, int prevNeuroCount=1)
        {
            layout = new List<Neuro>();
            for (int i=0; i<neuroCount; i++)
            {
                layout.Add(new Neuro());
                layout[i].setRandomW(prevNeuroCount);
            }
        }
        public void setX(List<double> inX)
        {
            for (int i=0; i<layout.Count();i++)
            {
                layout[i].setX(inX);
            }
        }
        public List<double> countResult()
        {
            List<double> Y = new List<double>();
            for (int i=0; i<layout.Count();i++)
            {
                Y.Add(layout[i].countResult());
            }
            return Y;
        }
        public List<double> countError(List<Neuro> prevLay)
        {
            List<double> curError = new List<double>();
            for (int i=0; i<layout.Count();i++)
            {
                List<double> prevW = new List<double>();
                List<double> prevError = new List<double>();
                for (int j=0; j<prevLay.Count(); j++)
                {
                    prevW.Add(prevLay[j].W[i]);
                    prevError.Add(prevLay[j].error);
                }
                curError.Add(layout[i].countError(prevW, prevError));
            }
            return curError;
        }
        public void CorrectW( double h)
        {
            for (int i=0; i<layout.Count();i++)
            {
                /*List<double> input = new List<double>();
                for (int j=0; j<inputLay.Count(); j++)
                {
                    input.Add(inputLay.ElementAt(j).X.ElementAt(i));
                }*/
                this.layout[i].CorrectW(h);
            }
        }
    }

    class Network
    {
        List<Lay> net;
        List<double> X;
        public double Y;
        double h;
        public Network(List<int>numNeuro, int inputCount=4, double inH=0.85)
        {
            h = inH;
            X = new List<double>();
            net = new List<Lay>();
            int prevNeurons = inputCount;
            for (int i=0; i<numNeuro.Count(); i++)
            {
                net.Add(new Lay(numNeuro[i], prevNeurons));
                prevNeurons = numNeuro[i];
            }
        }
        public double CountResult(List<double> inX)
        {
            //X.Clear();
            X = inX;
            List<double> prevLayResult=inX;
            for (int i=0; i<net.Count(); i++)
            {     
                net[i].setX(prevLayResult);
                prevLayResult = net[i].countResult();
            }
            Y= net.ElementAt(net.Count()-1).layout.ElementAt(0).Y;
            return Y;
        }
        public void CountError(double Ycorrect)
        {
            net.ElementAt(net.Count() - 1).layout.ElementAt(0).error = (Ycorrect - this.Y)* this.Y*(1- this.Y);
            for (int i=net.Count()-2; i>=0; i--)
            {
                net[i].countError(net[i + 1].layout);
            }
        }
        public void CorrectW ()
        {
            for (int i = 0; i < net.Count(); i++)
            {
                net[i].CorrectW( h);
            }
        }
    }

    class Teacher
    {
        Network backNet;
        double threshold;
        double curError;
        public Teacher(Network inNet, double inThreshold= 0.00001)
        {
            backNet = inNet;
            threshold = inThreshold;
        }
        protected bool teachOneStep(List<double> inX, double Ycorrect)
        {
            double y = backNet.CountResult(inX);
            //Console.WriteLine("y=" + Convert.ToString(y) + "  etalon=" + Convert.ToString(Ycorrect));
            backNet.CountError(Ycorrect);
            backNet.CorrectW();
            curError = backNet.Y - Ycorrect;
            //Console.WriteLine(curError);
            return (Math.Abs(curError) < threshold);
        }
        public Network teach(List<List<double>>inX, List<double> Ycorrect)
        {
            bool end=false;
            for (int i=0; i<inX.Count();i++)
            {
                end = false;
                int j = 0;
                //Console.WriteLine(i);
                while (!end)
                {
                    end = teachOneStep(inX[i], Ycorrect[i]);
                    j++;
                    if (j>10000)
                    {
                        break;
                    }
                }
                //Console.WriteLine(Convert.ToString(backNet.Y)+"   "+Convert.ToString(Ycorrect[i]));
                Console.WriteLine(curError);
            }
            return backNet;
        }
    }
}
