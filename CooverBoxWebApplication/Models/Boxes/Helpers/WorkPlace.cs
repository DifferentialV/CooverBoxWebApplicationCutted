using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    public class WorkPlace
    {
        public WorkPlace()
        {
        }
        public WorkPlace(string name, string material, int cuting = 0, int ktr = 0, int plotter = 0, int saw = 0, int silk = 0, int cliche = 0, int fairy = 0, int lamm = 0, int discarton = 0, int isolon = 0, int isolonplus = 0, int drap = 0)
        {
            Name = name;
            Material = material;
            Way = "";
            Squary = 0.0;
            Cuting = cuting;
            KTR = ktr;
            Plotter = plotter;
            Saw = saw;
            Silk = silk;
            Cliche = cliche;
            Fairy = fairy;
            Lamm = lamm;
            DisCarton = discarton;
            Isolon = isolon;
            IsolonPlus = isolonplus;
            Drap = drap;
        }
        public WorkPlace Clone()
        {
            return new WorkPlace(Name, Material, Way, Squary, Cuting, KTR, Plotter, Saw, Silk, Cliche, Fairy, Lamm, DisCarton, Isolon, IsolonPlus, Drap);
        }
        public WorkPlace(string name, string material, string way = "", double squary = 0.0, int cuting = 0, int ktr = 0, int plotter = 0, int saw = 0, int silk = 0, int cliche = 0, int fairy = 0, int lamm = 0, int discarton = 0, int isolon = 0, int isolonplus = 0, int drap = 0)
        {
            Name = name;
            Material = material;
            Way = way;
            Squary = squary;
            Cuting = cuting;
            KTR = ktr;
            Plotter = plotter;
            Saw = saw;
            Silk = silk;
            Cliche = cliche;
            Fairy = fairy;
            Lamm = lamm;
            DisCarton = discarton;
            Isolon = isolon;
            IsolonPlus = isolonplus;
            Drap = drap;
        }
        //Имя
        public string Name { get; set; } = "";
        //Материал
        public string Material { get; set; } = "";
        //Куда
        public string Way { get; set; } = "";
        public double Squary { get; set; } = 0.0;
        //Формат
        public string Format { get; set; } = "";
        //Высечка
        public int Cuting { get { return works["Высечка"]; } set { works["Высечка"] = value; } }
        //КТР
        public int KTR { get { return works["КТР"]; } set { works["КТР"] = value; } }
        //Плоттер
        public int Plotter { get { return works["Плоттер"]; } set { works["Плоттер"] = value; } }
        //Распиловка
        public int Saw { get { return works["Распиловка"]; } set { works["Распиловка"] = value; } }
        //Шёлк
        public int Silk { get { return works["Шёлк"]; } set { works["Шёлк"] = value; } }
        //Тиснение
        public int Cliche { get { return works["Тиснение"]; } set { works["Тиснение"] = value; } }
        //Оклейка
        public int Fairy { get { return works["Оклейка"]; } set { works["Оклейка"] = value; } }
        //кашировка
        public int Lamm { get { return works["Кашировка"]; } set { works["кашировка"] = value; } }
        //Диз картон
        public int DisCarton { get { return works["Диз картон"]; } set { works["Диз картон"] = value; } }
        //Ложемент изалон
        public int Isolon { get { return works["Ложемент изолон"]; } set { works["Ложемент изолон"] = value; } }
        //Ложемент изалон с  покрытие
        public int IsolonPlus { get { return works["Ложемент изолон с покрытие"]; } set { works["Ложемент изолон с покрытие"] = value; } }
        //Ложемент с драпировкой ткани
        public int Drap { get { return works["Ложемент с драпировкой ткани"]; } set { works["Ложемент с драпировкой ткани"] = value; } }
        public Dictionary<string, int> works = new Dictionary<string, int>()
            {
                {"Высечка",0 },
                {"КТР",0 },
                {"Плоттер",0 },
                {"Распиловка",0 },
                {"Шёлк",0 },
                {"Тиснение",0 },
                {"Оклейка",0 },
                {"Кашировка",0 },
                {"Диз картон",0 },
                {"Ложемент изолон",0 },
                {"Ложемент изолон с покрытие",0 },
                {"Ложемент с драпировкой ткани",0 },
            };
    }
}
