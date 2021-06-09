using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class PaperWithPrint
    {
        public DesignPaper Paper { get; set; }

        public int SilkWork { get; set; } = 0;
        public int ClicheWork { get; set; } = 0;
        public double ClicheX { get; set; } = 0;

        public double ClicheY { get; set; } = 0;
        public double SilkX { get; set; } = 0;
        public double SilkY { get; set; } = 0;

        public override string ToString()
        {
            string result = Paper.ToString();
            //if (Paper.Price > 0)
            //    result += $" {Paper.Price} грн/лист";
            if (SilkWork > 0)
                result += $" Шёлк: {SilkWork}";
            if (SilkX > 0 && SilkY > 0)
                result += $" Шёлк Разм печати: {SilkX}x{SilkY}";
            if (ClicheWork > 0)
                result += $" Тиснение: {ClicheWork}";
            if (ClicheX > 0 && ClicheY > 0)
                result += $" Разм клише: {ClicheX}x{ClicheY}";
            return result.Trim();
        }
    }
}
