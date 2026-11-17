using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public abstract class GanttItem
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string ItemName { get; set; }
        public string Color { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }

        public abstract GanttItemType ItemType { get; }

        public virtual bool IsDraggable
        {
            get { return true; }
        }

        public virtual bool RequiresApproval
        {
            get { return false; }
        }

        public virtual bool IsApproved
        {
            get { return false; }
        }
    }
}
