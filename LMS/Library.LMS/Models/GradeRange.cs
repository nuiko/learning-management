namespace Library.LMS.Models
{
    public class GradeRange
    {
        public string Letter { get; set; }   // e.g. "A"
        public double MinPercent { get; set; } // e.g. 90.0
        public double MaxPercent { get; set; } // e.g. 100.0
        public string HexColor { get; set; }   // e.g. "#4CAF50" (Sprint 4)
    }
}
