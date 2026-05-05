namespace Library.LMS.Models
{
    // Sprint 4: Quiz is a type of Assignment with a question prompt
    public class Quiz : Assignment
    {
        public string Question { get; set; }

        public Quiz() : base() { }

        public Quiz(Quiz source) : base(source)
        {
            Question = source.Question;
        }
    }
}
