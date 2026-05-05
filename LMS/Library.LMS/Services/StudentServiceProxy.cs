using Library.LMS.Models;

namespace Library.LMS.Services
{
    public class StudentServiceProxy
    {
        private static StudentServiceProxy? _instance;
        private static object _instanceLock = new object();

        private List<Student> _students;
        public List<Student> Students => _students;

        private StudentServiceProxy()
        {
            _students = new List<Student>();
        }

        public static StudentServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new StudentServiceProxy();
                }
                return _instance;
            }
        }

        public Student? Add(Student? student)
        {
            if (student == null) return null;
            if (student.Id == 0)
            {
                int lastKey = Students.Any() ? Students.Select(s => s.Id).Max() : 0;
                student.Id = lastKey + 1;
            }
            _students.Add(student);
            return student;
        }

        // Sprint 3: remove student from system, cascade unenroll from all courses + delete submissions
        public Student? Delete(int studentId)
        {
            var student = _students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return null;

            // Cascade: remove from all course rosters and delete their submissions
            foreach (var course in CourseServiceProxy.Current.Courses)
            {
                course.Roster.RemoveAll(s => s.Id == studentId);
                foreach (var a in course.Assignments)
                    a.Submissions.RemoveAll(s => s.StudentId == studentId);
            }

            _students.Remove(student);
            return student;
        }
    }
}
