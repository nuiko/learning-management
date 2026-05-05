using Library.LMS.Models;

namespace Library.LMS.Services
{
    public class CourseServiceProxy
    {
        private static CourseServiceProxy? _instance;
        private static object _instanceLock = new object();

        private List<Course> _courses;
        public List<Course> Courses => _courses;

        private CourseServiceProxy()
        {
            _courses = new List<Course>();
        }

        public static CourseServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new CourseServiceProxy();
                }
                return _instance;
            }
        }

        // ── Course CRUD ───────────────────────────────────────────────

        public Course? Add(Course? course)
        {
            if (course == null) return null;
            if (course.Id == 0)
            {
                int lastKey = Courses.Any() ? Courses.Select(c => c.Id).Max() : 0;
                course.Id = lastKey + 1;
            }
            _courses.Add(course);
            return course;
        }

        public Course? Delete(int courseId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            _courses.Remove(course);
            return course;
        }

        public Course? GetById(int courseId) =>
            _courses.FirstOrDefault(c => c.Id == courseId);

        // Sprint 2: deep copy a course (no roster, no submissions)
        public Course? Copy(int courseId)
        {
            var source = GetById(courseId);
            if (source == null) return null;
            var copy = new Course(source);
            return Add(copy);
        }

        // ── Assignment CRUD ───────────────────────────────────────────

        public Assignment? AddAssignment(int courseId, Assignment assignment)
        {
            var course = GetById(courseId);
            if (course == null) return null;
            int lastKey = course.Assignments.Any() ? course.Assignments.Select(a => a.Id).Max() : 0;
            assignment.Id = lastKey + 1;
            course.Assignments.Add(assignment);
            return assignment;
        }

        public bool DeleteAssignment(int courseId, int assignmentId)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return false;
            course.Assignments.Remove(assignment);
            return true;
        }

        // Sprint 3: copy assignment from one course to another (no submissions)
        public Assignment? CopyAssignment(int sourceCourseId, int assignmentId, int targetCourseId)
        {
            var source = GetById(sourceCourseId);
            var target = GetById(targetCourseId);
            if (source == null || target == null) return null;
            var original = source.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (original == null) return null;
            return AddAssignment(targetCourseId, new Assignment(original));
        }

        // ── Module CRUD ───────────────────────────────────────────────

        public Module? AddModule(int courseId, Module module)
        {
            var course = GetById(courseId);
            if (course == null) return null;
            int lastKey = course.Modules.Any() ? course.Modules.Select(m => m.Id).Max() : 0;
            module.Id = lastKey + 1;
            course.Modules.Add(module);
            return module;
        }

        public bool DeleteModule(int courseId, int moduleId)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var module = course.Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return false;
            course.Modules.Remove(module);
            return true;
        }

        // ── Roster management ─────────────────────────────────────────

        public bool EnrollStudent(int courseId, Student student)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            if (course.Roster.Any(s => s.Id == student.Id)) return false;
            course.Roster.Add(student);
            return true;
        }

        public bool UnenrollStudent(int courseId, int studentId)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var student = course.Roster.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return false;
            course.Roster.Remove(student);
            // Also remove their submissions
            foreach (var a in course.Assignments)
                a.Submissions.RemoveAll(s => s.StudentId == studentId);
            return true;
        }

        // Sprint 3: export roster to CSV string
        public string ExportRoster(int courseId)
        {
            var course = GetById(courseId);
            if (course == null) return string.Empty;
            var lines = new List<string> { "Id,Name,Code,Classification" };
            foreach (var s in course.Roster)
                lines.Add($"{s.Id},{s.Name},{s.Code},{s.Classification}");
            return string.Join(Environment.NewLine, lines);
        }

        // Sprint 3: import roster from CSV lines (idempotent)
        public void ImportRoster(int courseId, string csvContent)
        {
            var course = GetById(courseId);
            if (course == null) return;
            var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1)) // skip header
            {
                var parts = line.Split(',');
                if (parts.Length < 3) continue;
                if (!int.TryParse(parts[0], out int id)) continue;
                // Only add if student exists in system and not already enrolled
                var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
                if (student != null && !course.Roster.Any(s => s.Id == id))
                    course.Roster.Add(student);
            }
        }

        // ── Submission management ─────────────────────────────────────

        public Submission? AddSubmission(int courseId, int assignmentId, Submission submission)
        {
            var course = GetById(courseId);
            if (course == null) return null;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return null;
            // Remove existing submission by same student if present
            assignment.Submissions.RemoveAll(s => s.StudentId == submission.StudentId);
            int lastKey = assignment.Submissions.Any() ? assignment.Submissions.Select(s => s.Id).Max() : 0;
            submission.Id = lastKey + 1;
            submission.SubmissionDate = DateTime.Now;
            assignment.Submissions.Add(submission);
            return submission;
        }

        public bool GradeSubmission(int courseId, int assignmentId, int submissionId, double grade, string comment)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return false;
            var submission = assignment.Submissions.FirstOrDefault(s => s.Id == submissionId);
            if (submission == null) return false;
            submission.Grade = grade;
            submission.Comment = comment;
            return true;
        }

        // ── Assignment Groups (Sprint 2) ──────────────────────────────

        public AssignmentGroup? AddGroup(int courseId, AssignmentGroup group)
        {
            var course = GetById(courseId);
            if (course == null) return null;
            int lastKey = course.AssignmentGroups.Any() ? course.AssignmentGroups.Select(g => g.Id).Max() : 0;
            group.Id = lastKey + 1;
            course.AssignmentGroups.Add(group);
            return group;
        }

        public bool DeleteGroup(int courseId, int groupId)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups.FirstOrDefault(g => g.Id == groupId);
            if (group == null) return false;
            course.AssignmentGroups.Remove(group);
            return true;
        }

        public bool AddAssignmentToGroup(int courseId, int groupId, int assignmentId)
        {
            var course = GetById(courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups.FirstOrDefault(g => g.Id == groupId);
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (group == null || assignment == null) return false;
            if (!group.Assignments.Any(a => a.Id == assignmentId))
                group.Assignments.Add(assignment);
            return true;
        }

        // ── Grade calculation (Sprint 2) ──────────────────────────────

        public double CalculateStudentGrade(int courseId, int studentId)
        {
            var course = GetById(courseId);
            if (course == null) return 0;

            bool hasGroups = course.AssignmentGroups.Any();

            if (hasGroups)
            {
                double total = 0;
                double totalWeight = 0;
                foreach (var group in course.AssignmentGroups)
                {
                    if (!group.Assignments.Any()) continue;
                    double groupEarned = 0, groupPossible = 0;
                    foreach (var a in group.Assignments)
                    {
                        var sub = a.Submissions.FirstOrDefault(s => s.StudentId == studentId);
                        if (sub?.Grade != null)
                        {
                            groupEarned += sub.Grade.Value;
                            groupPossible += a.AvailablePoints;
                        }
                    }
                    if (groupPossible > 0)
                    {
                        total += (groupEarned / groupPossible) * group.Weight;
                        totalWeight += group.Weight;
                    }
                }
                return totalWeight > 0 ? (total / totalWeight) * 100 : 0;
            }
            else
            {
                double earned = 0, possible = 0;
                foreach (var a in course.Assignments)
                {
                    var sub = a.Submissions.FirstOrDefault(s => s.StudentId == studentId);
                    if (sub?.Grade != null)
                    {
                        earned += sub.Grade.Value;
                        possible += a.AvailablePoints;
                    }
                }
                return possible > 0 ? (earned / possible) * 100 : 0;
            }
        }

        // Sprint 4: export gradebook as CSV
        public string ExportGradebook(int courseId)
        {
            var course = GetById(courseId);
            if (course == null) return string.Empty;

            var header = "Student," + string.Join(",", course.Assignments.Select(a => a.Name));
            var lines = new List<string> { header };

            foreach (var student in course.Roster)
            {
                var grades = course.Assignments.Select(a =>
                {
                    var sub = a.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                    return sub?.Grade?.ToString("F1") ?? "-";
                });
                lines.Add($"{student.Name},{string.Join(",", grades)}");
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
