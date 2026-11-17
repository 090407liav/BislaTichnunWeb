using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public class CourseEvent : GanttItem
    {
        public CourseEventType EventType { get; set; }

        public bool Draggable { get; set; }

        public override GanttItemType ItemType
        {
            get { return GanttItemType.CourseEvent; }
        }

        public override bool IsDraggable
        {
            get { return Draggable; }
        }
    }
}