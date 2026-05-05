namespace Library.LMS.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double AvailablePoints { get; set; }
        public DateTime DueDate { get; set; }
        public List<Submission> Submissions { get; set; }

        public Assignment()
        {
            Submissions = new List<Submission>();
        }

        // Deep copy constructor - no submissions copied (Sprint 2/3)
        public Assignment(Assignment source) : this()
        {
            Name = source.Name;
            Description = source.Description;
            AvailablePoints = source.AvailablePoints;
            DueDate = source.DueDate;
        }
    }
}
