namespace Library.LMS.Models
{
    public enum Classification
    {
        Unknown,
        Freshman,
        Sophomore,
        Junior,
        Senior
    }

    public class Student : User
    {
        public Classification Classification { get; set; }
    }
}
