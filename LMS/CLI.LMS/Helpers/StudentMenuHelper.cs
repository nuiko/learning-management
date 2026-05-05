using Library.LMS.Models;
using Library.LMS.Services;

namespace CLI.LMS.Helpers
{
    public class StudentMenuHelper
    {
        public void EnterMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== Student Menu ===");

            // Issue #18: select a student to proxy as
            var students = StudentServiceProxy.Current.Students;
            if (!students.Any())
            {
                Console.WriteLine("  No students in the system.");
                return;
            }

            Console.WriteLine("  Select a student:");
            foreach (var s in students)
                Console.WriteLine($"  [{s.Id}] {s.Name} ({s.Code}) - {s.Classification}");

            Console.Write("  Enter student ID: ");
            if (!int.TryParse(Console.ReadLine(), out int studentId)) return;

            var student = students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) { Console.WriteLine("  Student not found."); return; }

            StudentCourseMenu(student);
        }

        // Issue #19: student course main menu
        private void StudentCourseMenu(Student student)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine($"=== Welcome, {student.Name} ===");

                var enrolledCourses = CourseServiceProxy.Current.Courses
                    .Where(c => c.Roster.Any(s => s.Id == student.Id))
                    .ToList();

                Console.WriteLine("  Your Courses:");
                if (!enrolledCourses.Any()) Console.WriteLine("  (not enrolled in any courses)");
                else foreach (var c in enrolledCourses)
                    Console.WriteLine($"  [{c.Id}] {c.Code} - {c.Name} ({c.Semester})");

                Console.WriteLine();
                Console.WriteLine("  1. Select a course");
                Console.WriteLine("  2. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")
                    SelectCourse(student, enrolledCourses);

            } while (choice != "2");
        }

        private void SelectCourse(Student student, List<Course> enrolledCourses)
        {
            if (!enrolledCourses.Any()) { Console.WriteLine("  Not enrolled in any courses."); return; }
            Console.Write("  Enter course ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var course = enrolledCourses.FirstOrDefault(c => c.Id == id);
            if (course == null) { Console.WriteLine("  Course not found."); return; }
            CourseDetailMenu(student, course);
        }

        // Issue #19: full course detail menu for students
        private void CourseDetailMenu(Student student, Course course)
        {
            var choice = string.Empty;
            do
            {
                // Sprint 2/4: show letter grade prominently
                double pct = CourseServiceProxy.Current.CalculateStudentGrade(course.Id, student.Id);
                string letter = GradeSettingsServiceProxy.Current.GetLetterGrade(course.Id, pct);

                Console.WriteLine();
                Console.WriteLine($"=== {course.Code} - {course.Name} ===");
                Console.WriteLine($"  Current Grade: {letter} ({pct:F1}%)");
                Console.WriteLine();
                Console.WriteLine("  1. View modules");
                Console.WriteLine("  2. View assignments & submit");
                Console.WriteLine("  3. View other students");
                Console.WriteLine("  4. Course schedule");
                Console.WriteLine("  5. View my grades");
                Console.WriteLine("  6. Unenroll from this course");
                Console.WriteLine("  7. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")      ViewModules(course);
                else if (choice == "2") ViewAndSubmitAssignments(student, course);
                else if (choice == "3") ViewOtherStudents(student, course);
                else if (choice == "4") ViewSchedule(course);
                else if (choice == "5") ViewMyGrades(student, course);
                else if (choice == "6")
                {
                    CourseServiceProxy.Current.UnenrollStudent(course.Id, student.Id);
                    Console.WriteLine("  You have been unenrolled.");
                    return;
                }

            } while (choice != "7");
        }

        // Issue #19: see all modules and module content
        private void ViewModules(Course course)
        {
            Console.WriteLine();
            Console.WriteLine("  -- Modules --");
            if (!course.Modules.Any()) { Console.WriteLine("  (none)"); return; }
            foreach (var m in course.Modules)
            {
                Console.WriteLine($"  [{m.Id}] {m.Name}");
                if (!m.Content.Any()) Console.WriteLine("    (no content)");
                else foreach (var item in m.Content)
                    Console.WriteLine($"    • {item}");
            }
        }

        // Issues #6, #19: view assignments and submit
        private void ViewAndSubmitAssignments(Student student, Course course)
        {
            Console.WriteLine();
            Console.WriteLine("  -- Assignments --");
            if (!course.Assignments.Any()) { Console.WriteLine("  (none)"); return; }

            foreach (var a in course.Assignments.OrderBy(a => a.DueDate))
            {
                var existing = a.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                string status = existing != null ? $"[Submitted {existing.SubmissionDate:MM/dd}]" : "[Not submitted]";
                string type = a is Quiz ? "[Quiz]" : "[Assign]";
                Console.WriteLine($"  [{a.Id}] {type} {a.Name} | {a.AvailablePoints}pts | Due: {a.DueDate:MM/dd/yyyy} {status}");
            }

            Console.WriteLine();
            Console.Write("  Enter assignment ID to submit (or 0 to go back): ");
            if (!int.TryParse(Console.ReadLine(), out int aId) || aId == 0) return;

            var assignment = course.Assignments.FirstOrDefault(a => a.Id == aId);
            if (assignment == null) { Console.WriteLine("  Not found."); return; }

            if (assignment is Quiz quiz)
                Console.WriteLine($"  Question: {quiz.Question}");

            Console.Write("  Your response: ");
            var content = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(content)) return;

            var submission = new Submission
            {
                StudentId = student.Id,
                AssignmentId = aId,
                Content = content
            };
            CourseServiceProxy.Current.AddSubmission(course.Id, aId, submission);
            Console.WriteLine("  Submission recorded.");
        }

        // Issue #19: see other students
        private void ViewOtherStudents(Student self, Course course)
        {
            Console.WriteLine();
            Console.WriteLine("  -- Students in Course --");
            foreach (var s in course.Roster)
            {
                string tag = s.Id == self.Id ? " (you)" : "";
                Console.WriteLine($"  [{s.Id}] {s.Name} ({s.Code}){tag}");
            }
        }

        // Issue #19: course schedule = assignments + due dates
        private void ViewSchedule(Course course)
        {
            Console.WriteLine();
            Console.WriteLine("  -- Course Schedule --");
            if (!course.Assignments.Any()) { Console.WriteLine("  (no assignments)"); return; }
            foreach (var a in course.Assignments.OrderBy(a => a.DueDate))
                Console.WriteLine($"  {a.DueDate:MM/dd/yyyy}  {a.Name}  ({a.AvailablePoints} pts)");
        }

        // Issue #28: student sees grades by assignment + overall weighted average
        private void ViewMyGrades(Student student, Course course)
        {
            Console.WriteLine();
            Console.WriteLine("  -- My Grades --");

            foreach (var a in course.Assignments)
            {
                var sub = a.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                if (sub == null)
                    Console.WriteLine($"  {a.Name}: Not submitted");
                else if (sub.Grade == null)
                    Console.WriteLine($"  {a.Name}: Submitted, not graded yet");
                else
                {
                    double pct = (sub.Grade.Value / a.AvailablePoints) * 100;
                    string letter = GradeSettingsServiceProxy.Current.GetLetterGrade(course.Id, pct);
                    Console.WriteLine($"  {a.Name}: {sub.Grade:F1}/{a.AvailablePoints} ({pct:F1}%) {letter}");
                    if (!string.IsNullOrWhiteSpace(sub.Comment))
                        Console.WriteLine($"    Feedback: {sub.Comment}");
                }
            }

            double overall = CourseServiceProxy.Current.CalculateStudentGrade(course.Id, student.Id);
            string overallLetter = GradeSettingsServiceProxy.Current.GetLetterGrade(course.Id, overall);
            Console.WriteLine();
            Console.WriteLine($"  Overall: {overall:F1}% ({overallLetter})");
        }
    }
}
