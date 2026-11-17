using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public enum GanttItemType
    {
        Assignment = 0,
        CourseEvent = 1
    }
    public enum AssignmentStatus
    {
        Planning = 0,
        Active = 1
    }
    public enum ViewMode
    {
        Planning = 0,
        Active = 1
    }
    public enum ViewScale
    {
        Month = 0,
        Quarter = 1
    }

    public enum CourseEventType
    {
        //Opening and Closing Course Dates
        CourseOpen = 1,
        CourseClose = 2,
        HearingTest = 3,

        //Specialization Classes
        WiringSkills = 10,
        MaintenanceSkills = 20,
        ElectronicsSkills = 30, 

        //Practical Work & Professional Tourse
        PracticalWork = 100,
        ProfessionalTour = 200

    }
}