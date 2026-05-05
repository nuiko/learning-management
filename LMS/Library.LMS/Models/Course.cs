namespace Library.LMS.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Semester { get; set; }
        public string Section { get; set; }
        public List<Student> Roster { get; set; }
        public List<Module> Modules { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<AssignmentGroup> AssignmentGroups { get; set; }

        public Course()
        {
            Roster = new List<Student>();
            Modules = new List<Module>();
            Assignments = new List<Assignment>();
            AssignmentGroups = new List<AssignmentGroup>();
        }

        // Deep copy constructor - copies all content except roster and submissions (Sprint 2)
        public Course(Course source) : this()
        {
            Code = source.Code;
            Name = source.Name + " (Copy)";
            Description = source.Description;
            Semester = source.Semester;
            Section = source.Section;

            foreach (var m in source.Modules)
                Modules.Add(new Module(m));

            foreach (var a in source.Assignments)
                Assignments.Add(new Assignment(a));

            foreach (var g in source.AssignmentGroups)
                AssignmentGroups.Add(new AssignmentGroup(g));
        }
    }
}
