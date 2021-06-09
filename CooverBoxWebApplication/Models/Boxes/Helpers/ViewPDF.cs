using PDFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    //чтобы хранить Group которые надо вывести на главном чертеже
    public class ViewPDFGroup
    {
        public ViewPDFGroup(int priority,Group group)
        {
            Prioriry = priority;
            Group = group;
        }
        public int Prioriry { get; set; }
        public Group Group { get; set; } 
    }
}
