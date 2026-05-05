namespace Library.LMS.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public string Content { get; set; }
        public DateTime SubmissionDate { get; set; }

        // Sprint 2: grading
        public double? Grade { get; set; }       // points earned
        public string Comment { get; set; }      // teacher feedback
    }
}
