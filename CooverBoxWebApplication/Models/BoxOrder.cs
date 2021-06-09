using CooverBoxWebApplication.Models.WorkRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class BoxOrder
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string BoxId { get; set; }
        public string TypeBox { get; set; }
        public string BitrixLink { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateStartProcess { get; set; }
        public DateTime DateShipment { get; set; }
        public User User { get; set; }
        public bool Opening { get; set; } = false;
        public bool Complete { get; set; } = false;
        public bool Chosen { get; set; } = false;

        public List<TaskBox> TaskBoxes { get; set; }
        public BoxOrder()
        {
            TaskBoxes = new List<TaskBox>();
        }
    }
}
