using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace myapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Exchange e = new Exchange();
            e.Name = "New York Stock Exchange";
            e.Symbol = "NYSE";

            Underlying u = new Underlying();
            u.Exchange = e;
            Console.WriteLine("Enter the ticker of selected stock: ");
            u.Symbol = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Enter the price");
            u.Price = Convert.ToDouble(Console.ReadLine());

            EuropeanOption euro = new EuropeanOption();
            Console.WriteLine("Is this a Call? Please enter true for call, false for put. ");
            euro.IsCall = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("Set the strike price ");
            euro.Strike = Convert.ToDouble(Console.ReadLine());
            euro.ExpirationDate = new DateTime(2022,06,10);
            euro.Undelying = u;

            Volatility vol = new Volatility();
            Console.WriteLine("Set volatility rate");
            vol.vol = Convert.ToDouble(Console.ReadLine());  
            var g = euro.GetPriceAndGreeks(252, 1000, vol);

            Console.WriteLine("Price of the option is {0}", g.Price);
        }
    }
    
    class Ratepoint
    {
        public double Tenor { get; set; }
        public double Rate { get; set; }
    }
    
    class YieldCurve
    {
        public List<Ratepoint> CurvePoints { get; set; }
    }
    class Exchange
    { 
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
    class Underlying
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public double Price { get; set; }
    }
    abstract class Option
    {
        public DateTime ExpirationDate { get; set; }
        public Underlying Underlying { get; set; }
    public abstract OptionResult GetPriceAndGreeks(long Steps, long Simulations, Volatility vol);
    }
    class OptionResult
    {
        public double Price { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
    }
    class Volatility
    {
        public double vol { get; set; }
    }
    class EuropeanOption: Option
    {
        public double Strike { get; set; }
        public bool IsCall { get; set; }
        public Underlying Undelying { get; set; }

        public override OptionResult GetPriceAndGreeks(long Steps, long Simulations, Volatility vol)
        {
            GaussianRandoms r = new GaussianRandoms();
            r.PopulateNRands(1, Simulations, Steps);
            SimulationParameters p = new SimulationParameters();
            p.S0 = Underlying.Price;
            p.r = 0.05;
            p.Steps = Steps;
            p.Simulations = Simulations;
            p.Tenor = (ExpirationDate = DateTime.Today).Day / 365d;
            p.Volatility = vol.vol;

            var result = MonteCarloSimulator.GeneratePaths(p, r);
            return new OptionResult() { Price = 11.33 };
        }
    }
    class GaussianRandoms
    {
        public double[,] NRands { get; set; }
        public void PopulateNRands(int seed, long rows, long cols)
        {
            NRands = new double[rows, cols];
            Random r = new Random(seed);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) 
                { 
                    NRands[i, j] = r.NextDouble(); 
                }
            }
        }
    }
    class SimulationResult
    {
        public double[,] SimulatedPaths { get; set; }
    }
    class SimulationParameters
    {
        public double S0 { get; set; }
        public double r { get; set; }
        public long Steps { get; set; }
        public long Simulations { get; set; }
        public double Tenor { get; set; }
        public double Volatility { get; set; }
    }
    static class MonteCarloSimulator
    {
        public static SimulationResult GeneratePaths(SimulationParameters p, GaussianRandoms rands)
        {
            SimulationResult result = new SimulationResult();
            result.SimulatedPaths = new double[p.Steps, p.Simulations];
            return result;
        }
    }
}
