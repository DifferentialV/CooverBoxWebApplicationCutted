using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    public class WorkCostFormat
    {
        private readonly List<WorkPlace> works;
        public int N { get; set; }
        public WorkCostFormat(int N)
        {
            this.N = N;
            works = new List<WorkPlace>();
        }
        public void AddWorkPlace(string name, string material, string way, double squary, int cuting = 0, int ktr = 0, int plotter = 0, int saw = 0, int silk = 0, int cliche = 0, int fairy = 0, int lamm = 0, int discarton = 0, int isolon = 0, int isolonplus = 0, int drap = 0)
        {
            if (works.Any(w => w.Name == name && w.Material == material))
                AddWorkWay(name, material, way, squary);
            else
                works.Add(new WorkPlace() { Name = name, Material = material, Way = way, Squary = squary, Cuting = cuting, KTR = ktr, Plotter = plotter, Saw = saw, Silk = silk, Cliche = cliche, Fairy = fairy, Lamm = lamm, DisCarton = discarton, Isolon = isolon, IsolonPlus = isolonplus, Drap = drap });
        }
        public void AddWorkPlace(string name, string material, int cuting = 0, int ktr = 0, int plotter = 0, int saw = 0, int silk = 0, int cliche = 0, int fairy = 0, int lamm = 0, int discarton = 0, int isolon = 0, int isolonplus = 0, int drap = 0)
        {
            if (works.Any(w => w.Name == name && w.Material == material))
                throw new Exception("sdfsdf");
            else
                works.Add(new WorkPlace() { Name = name, Material = material,  Cuting = cuting, KTR = ktr, Plotter = plotter, Saw = saw, Silk = silk, Cliche = cliche, Fairy = fairy, Lamm = lamm, DisCarton = discarton, Isolon = isolon, IsolonPlus = isolonplus, Drap = drap });
        }
        public void AddWorkWay(string name,string way,double squary)
        {
            WorkPlace temp = works.First(w => w.Name == name);
            temp.Way += $"|{way}|";
            temp.Squary += squary;
        }
        public void AddWorkWay(string name, string material, string way, double squary)
        {
            WorkPlace temp = works.First(w => w.Name == name && w.Material == material);
            temp.Way += $"|{way}|";
            temp.Squary += squary;
        }
        public List<WorkPlace> ToSheeldByName()
        {
            List<WorkPlace> result = new List<WorkPlace>();
            foreach(var workplace in works)
            {
                foreach(var format in Formats(workplace.Squary))
                {
                    WorkPlace clone = workplace.Clone();
                    clone.Format = format;
                    result.Add(clone);
                }
            }
            return result;
        }

        public static string[] Formats(double squary)
        {
            List<string> formats = new List<string>();
            int n = (int)Math.Ceiling(squary / 62370.00);
            while (n > 0)
            {
                if (n <= 1)
                {
                    formats.Add("A4");
                    n -= 1;
                }
                else if (n <= 2)
                {
                    formats.Add("A3");
                    n -= 2;
                }
                else if (n <= 4)
                {
                    formats.Add("A2");
                    n -= 4;
                }
                else if (n <= 8)
                {
                    formats.Add("A1");
                    n -= 8;
                }
                else
                {
                    formats.Add("A0");
                    n -= 16;
                }
            }
            return formats.ToArray();
        }
      
    }
}
