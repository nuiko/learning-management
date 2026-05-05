namespace Library.LMS.Models
{
    public class AssignmentGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }          // e.g. 0.30 = 30%
        public List<Assignment> Assignments { get; set; }

        public AssignmentGroup()
        {
            Assignments = new List<Assignment>();
        }

        // Deep copy constructor - no submissions
        public AssignmentGroup(AssignmentGroup source) : this()
        {
            Name = source.Name;
            Weight = source.Weight;
            foreach (var a in source.Assignments)
                Assignments.Add(new Assignment(a));
        }
    }
}
