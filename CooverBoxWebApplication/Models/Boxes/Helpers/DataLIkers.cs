using System;
using System.Collections.Generic;
using System.Text;

namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    //табличные данные для вывода себестоимости
    public class DataTableLiker
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Coment { get; set; }
        public List<DataLineLiker> dataLines = new List<DataLineLiker>();
    }

    public class DataLineLiker
    {
        public string Way { get; set; }
        public string Coment { get; set; }
        public double Count { get; set; }
        public string ValueType { get; set; }
        public double Price { get; set; }
    }
}
