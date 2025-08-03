using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.Models
{
    public class DutySlot
    {
        //public int Id { get; set; }
        //public string SlotName { get; set; }
        //public int StartHour { get; set; }
        //public int StartMinute { get; set; }
        //public int DutyHour { get; set; }
        //public int DutyMinute { get; set; }
        //public int EndHour { get; set; }
        //public int EndMinute { get; set; }
        //public string Branch { get; set; }
        //public int BranchId { get; set; }
        public int Id { get; set; }

        [Required(ErrorMessage = "Slot name is required")]
        [StringLength(100, ErrorMessage = "Slot name cannot exceed 100 characters")]
        public string SlotName { get; set; }

        [Range(0, 23, ErrorMessage = "Hour must be between 0 and 23")]
        public int StartHour { get; set; }

        [Range(0, 59, ErrorMessage = "Minute must be between 0 and 59")]
        public int StartMinute { get; set; }

        [Range(0, 23, ErrorMessage = "Duty hours must be between 0 and 23")]
        public int DutyHour { get; set; }

        [Range(0, 59, ErrorMessage = "Duty minutes must be between 0 and 59")]
        public int DutyMinute { get; set; }

        [Range(0, 23, ErrorMessage = "Hour must be between 0 and 23")]
        public int EndHour { get; set; }

        [Range(0, 59, ErrorMessage = "Minute must be between 0 and 59")]
        public int EndMinute { get; set; }


        // ✅ Computed properties to format times
        [NotMapped]
        public string DisplayTimeRange => $"{StartHour:D2}:{StartMinute:D2} Hr";

        [NotMapped]
        public string DutyDuration => $"{DutyHour:D2} Hrs {DutyMinute:D2} Mins";

        [NotMapped]
        public string DisplayEndTime => $"{EndHour:D2}:{EndMinute:D2} Hr";
    }
}
