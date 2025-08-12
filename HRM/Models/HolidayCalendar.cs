namespace HRM.Models
{
    public class HolidayCalendar
    {
        //public int Id { get; set; }
        //public DateTime HolidayDate { get; set; }
        //public string Purpose { get; set; }

        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; } // 1-12
        public int Day { get; set; }   // 1-31
        public string HolidayName { get; set; } // e.g., "Eid"
    }
}
