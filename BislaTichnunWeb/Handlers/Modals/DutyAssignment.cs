using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public class DutyAssignment : GanttItem
    {
        public int DutyTypeId { get; set; }

        public AssignmentStatus Status { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public override GanttItemType ItemType
        {
            get { return GanttItemType.Assignment; }
        }

        public override bool RequiresApproval
        {
            get { return true; }
        }

        public override bool IsApproved
        {
            get
            {
                return Status == AssignmentStatus.Active;
            }
        }
    }
}