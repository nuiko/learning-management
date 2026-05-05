using Library.LMS.Models;
using Library.LMS.Services;

namespace CLI.LMS.Helpers
{
    public class TeacherMenuHelper
    {
        public void EnterMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== Teacher Main Menu ===");

            var choice = string.Empty;

            do
            {
                Console.WriteLine();
                Console.WriteLine("1. Manage Students");
                Console.WriteLine("2. Manage Courses");
                Console.WriteLine("3. Back");
                Console.Write("Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")
                    ManageStudents();
                else if (choice == "2")
                    ManageCourses();

            } while (choice != "3");
        }

        // ══════════════════════════════════════════════════════════════
        //  STUDENT MANAGEMENT  (Sprint 1 + Sprint 3)
        // ══════════════════════════════════════════════════════════════

        private void ManageStudents()
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("--- Student Management ---");
                Console.WriteLine("1. Enroll a new student");
                Console.WriteLine("2. Edit a student");
                Console.WriteLine("3. Remove a student from the system");
                Console.WriteLine("4. List all students");
                Console.WriteLine("5. Back");
                Console.Write("Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")
                {
                    var s = CreateStudentRecord();
                    StudentServiceProxy.Current.Add(s);
                    Console.WriteLine($"  Student '{s.Name}' enrolled (ID: {s.Id}).");
                }
                else if (choice == "2")
                    EditStudent();
                else if (choice == "3")
                    RemoveStudent();
                else if (choice == "4")
                    ListStudents();

            } while (choice != "5");
        }

        private Student CreateStudentRecord()
        {
            var newStudent = new Student();

            Console.WriteLine();
            Console.Write("Name: ");
            newStudent.Name = Console.ReadLine().Trim();

            Console.Write("Code (FSUID): ");
            newStudent.Code = Console.ReadLine().Trim();

            Console.WriteLine("Classification:  F-Freshman  S-Sophomore  J-Junior  R-Senior  U-Unknown");
            Console.Write("Choice: ");
            var classChoice = Console.ReadLine().Trim();

            if (classChoice.Equals("F", StringComparison.OrdinalIgnoreCase))
                newStudent.Classification = Classification.Freshman;
            else if (classChoice.Equals("S", StringComparison.OrdinalIgnoreCase))
                newStudent.Classification = Classification.Sophomore;
            else if (classChoice.Equals("J", StringComparison.OrdinalIgnoreCase))
                newStudent.Classification = Classification.Junior;
            else if (classChoice.Equals("R", StringComparison.OrdinalIgnoreCase))
                newStudent.Classification = Classification.Senior;
            else
                newStudent.Classification = Classification.Unknown;

            return newStudent;
        }

        private void EditStudent()
        {
            ListStudents();
            Console.Write("Enter student ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) { Console.WriteLine("  Student not found."); return; }

            Console.Write($"Name [{student.Name}]: ");
            var name = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(name)) student.Name = name;

            Console.Write($"Code [{student.Code}]: ");
            var code = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(code)) student.Code = code;

            Console.WriteLine($"Classification [{student.Classification}]:  F S J R U");
            Console.Write("Choice (Enter to skip): ");
            var c = Console.ReadLine().Trim();
            if (c.Equals("F", StringComparison.OrdinalIgnoreCase)) student.Classification = Classification.Freshman;
            else if (c.Equals("S", StringComparison.OrdinalIgnoreCase)) student.Classification = Classification.Sophomore;
            else if (c.Equals("J", StringComparison.OrdinalIgnoreCase)) student.Classification = Classification.Junior;
            else if (c.Equals("R", StringComparison.OrdinalIgnoreCase)) student.Classification = Classification.Senior;
            else if (c.Equals("U", StringComparison.OrdinalIgnoreCase)) student.Classification = Classification.Unknown;

            Console.WriteLine("  Student updated.");
        }

        private void RemoveStudent()
        {
            ListStudents();
            Console.Write("Enter student ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var removed = StudentServiceProxy.Current.Delete(id);
            if (removed == null) Console.WriteLine("  Student not found.");
            else Console.WriteLine($"  '{removed.Name}' removed from the system.");
        }

        private void ListStudents()
        {
            var students = StudentServiceProxy.Current.Students;
            if (!students.Any()) { Console.WriteLine("  No students enrolled."); return; }
            Console.WriteLine();
            foreach (var s in students)
                Console.WriteLine($"  [{s.Id}] {s.Name} ({s.Code}) - {s.Classification}");
        }

        // ══════════════════════════════════════════════════════════════
        //  COURSE MANAGEMENT  (Sprints 1-4)
        // ══════════════════════════════════════════════════════════════

        private void ManageCourses()
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("--- Course Management ---");
                ListCoursesBySemester();
                Console.WriteLine();
                Console.WriteLine("1. Add a new course");
                Console.WriteLine("2. Select a course");
                Console.WriteLine("3. Copy a course");
                Console.WriteLine("4. Delete a course");
                Console.WriteLine("5. Back");
                Console.Write("Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")
                {
                    var c = CreateCourseRecord();
                    CourseServiceProxy.Current.Add(c);
                    Console.WriteLine($"  Course '{c.Name}' added (ID: {c.Id}).");
                }
                else if (choice == "2")
                    SelectCourse();
                else if (choice == "3")
                    CopyCourse();
                else if (choice == "4")
                    DeleteCourse();

            } while (choice != "5");
        }

        private Course CreateCourseRecord()
        {
            var course = new Course();
            Console.WriteLine();
            Console.Write("Name: ");
            course.Name = Console.ReadLine().Trim();
            Console.Write("Code: ");
            course.Code = Console.ReadLine().Trim();
            Console.Write("Description: ");
            course.Description = Console.ReadLine().Trim();
            Console.Write("Semester (e.g. Fall 2025): ");
            course.Semester = Console.ReadLine().Trim();
            Console.Write("Section (e.g. 001): ");
            course.Section = Console.ReadLine().Trim();
            return course;
        }

        private void ListCoursesBySemester()
        {
            var courses = CourseServiceProxy.Current.Courses;
            if (!courses.Any()) { Console.WriteLine("  No courses yet."); return; }
            var bySemester = courses.GroupBy(c => c.Semester ?? "(No Semester)")
                                    .OrderBy(g => g.Key);
            foreach (var group in bySemester)
            {
                Console.WriteLine($"  [{group.Key}]");
                foreach (var c in group.OrderBy(c => c.Name))
                    Console.WriteLine($"    [{c.Id}] {c.Code} - {c.Name} (Section {c.Section})");
            }
        }

        private void SelectCourse()
        {
            Console.Write("Enter course ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var course = CourseServiceProxy.Current.GetById(id);
            if (course == null) { Console.WriteLine("  Course not found."); return; }
            CourseSubMenu(course);
        }

        private void CopyCourse()
        {
            Console.Write("Enter course ID to copy: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var copy = CourseServiceProxy.Current.Copy(id);
            if (copy == null) Console.WriteLine("  Course not found.");
            else Console.WriteLine($"  Course copied as '{copy.Name}' (ID: {copy.Id}).");
        }

        private void DeleteCourse()
        {
            Console.Write("Enter course ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var deleted = CourseServiceProxy.Current.Delete(id);
            if (deleted == null) Console.WriteLine("  Course not found.");
            else Console.WriteLine($"  Course '{deleted.Name}' deleted. Enrolled students were not removed.");
        }

        // ══════════════════════════════════════════════════════════════
        //  COURSE SUB-MENU  (Issue #17 + all teacher course features)
        // ══════════════════════════════════════════════════════════════

        private void CourseSubMenu(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine($"=== {course.Code} - {course.Name} | {course.Semester} Sec {course.Section} ===");
                Console.WriteLine($"  Description: {course.Description}");
                Console.WriteLine();
                Console.WriteLine("1. Manage Roster");
                Console.WriteLine("2. Manage Assignments");
                Console.WriteLine("3. Manage Modules");
                Console.WriteLine("4. Manage Assignment Groups");
                Console.WriteLine("5. Grade Submissions");
                Console.WriteLine("6. Course Settings (grade ranges)");
                Console.WriteLine("7. Update course description");
                Console.WriteLine("8. Export Roster (CSV)");
                Console.WriteLine("9. Import Roster (CSV)");
                Console.WriteLine("10. Export Gradebook (CSV)");
                Console.WriteLine("11. Back");
                Console.Write("Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")       ManageRoster(course);
                else if (choice == "2")  ManageAssignments(course);
                else if (choice == "3")  ManageModules(course);
                else if (choice == "4")  ManageAssignmentGroups(course);
                else if (choice == "5")  GradeSubmissions(course);
                else if (choice == "6")  ManageGradeSettings(course);
                else if (choice == "7")  UpdateCourseDescription(course);
                else if (choice == "8")  ExportRoster(course);
                else if (choice == "9")  ImportRoster(course);
                else if (choice == "10") ExportGradebook(course);

            } while (choice != "11");
        }

        private void UpdateCourseDescription(Course course)
        {
            Console.Write($"Description [{course.Description}]: ");
            var desc = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(desc)) course.Description = desc;
            Console.WriteLine("  Description updated.");
        }

        // ── Roster ───────────────────────────────────────────────────

        private void ManageRoster(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("  -- Roster --");
                if (!course.Roster.Any()) Console.WriteLine("  (empty)");
                else foreach (var s in course.Roster)
                    Console.WriteLine($"  [{s.Id}] {s.Name} ({s.Code}) - {s.Classification}");

                Console.WriteLine();
                Console.WriteLine("  1. Enroll a student");
                Console.WriteLine("  2. Unenroll a student");
                Console.WriteLine("  3. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")      EnrollStudentInCourse(course);
                else if (choice == "2") UnenrollStudentFromCourse(course);
            } while (choice != "3");
        }

        private void EnrollStudentInCourse(Course course)
        {
            ListStudents();
            Console.Write("Enter student ID to enroll: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) { Console.WriteLine("  Student not found."); return; }
            bool ok = CourseServiceProxy.Current.EnrollStudent(course.Id, student);
            Console.WriteLine(ok ? $"  {student.Name} enrolled." : "  Already enrolled.");
        }

        private void UnenrollStudentFromCourse(Course course)
        {
            Console.Write("Enter student ID to unenroll: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            bool ok = CourseServiceProxy.Current.UnenrollStudent(course.Id, id);
            Console.WriteLine(ok ? "  Student unenrolled." : "  Student not found in roster.");
        }

        private void ExportRoster(Course course)
        {
            var csv = CourseServiceProxy.Current.ExportRoster(course.Id);
            Console.WriteLine();
            Console.WriteLine("  --- Roster CSV ---");
            Console.WriteLine(csv);
            Console.WriteLine("  ------------------");
        }

        private void ImportRoster(Course course)
        {
            Console.WriteLine("  Paste CSV content (Id,Name,Code,Classification), then enter a blank line:");
            var lines = new List<string>();
            string line;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
                lines.Add(line);
            CourseServiceProxy.Current.ImportRoster(course.Id, string.Join(Environment.NewLine, lines));
            Console.WriteLine("  Roster imported.");
        }

        // ── Assignments ───────────────────────────────────────────────

        private void ManageAssignments(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("  -- Assignments --");
                PrintAssignments(course);
                Console.WriteLine();
                Console.WriteLine("  1. Add assignment");
                Console.WriteLine("  2. Edit assignment");
                Console.WriteLine("  3. Delete assignment");
                Console.WriteLine("  4. Copy assignment from another course");
                Console.WriteLine("  5. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")      AddAssignment(course);
                else if (choice == "2") EditAssignment(course);
                else if (choice == "3") DeleteAssignment(course);
                else if (choice == "4") CopyAssignmentFromCourse(course);
            } while (choice != "5");
        }

        private void PrintAssignments(Course course)
        {
            if (!course.Assignments.Any()) { Console.WriteLine("  (none)"); return; }
            foreach (var a in course.Assignments.OrderBy(a => a.DueDate))
            {
                string type = a is Quiz ? "[Quiz]" : "[Assign]";
                Console.WriteLine($"  [{a.Id}] {type} {a.Name} | {a.AvailablePoints}pts | Due: {a.DueDate:MM/dd/yyyy}");
            }
        }

        private void AddAssignment(Course course)
        {
            Console.WriteLine("  Type: 1-Regular Assignment  2-Quiz");
            Console.Write("  Choice: ");
            var typeChoice = Console.ReadLine().Trim();

            Assignment assignment;
            if (typeChoice == "2")
            {
                var quiz = new Quiz();
                Console.Write("  Question: ");
                quiz.Question = Console.ReadLine().Trim();
                assignment = quiz;
            }
            else
            {
                assignment = new Assignment();
            }

            Console.Write("  Name: ");
            assignment.Name = Console.ReadLine().Trim();
            Console.Write("  Description: ");
            assignment.Description = Console.ReadLine().Trim();
            Console.Write("  Available Points: ");
            double.TryParse(Console.ReadLine(), out double pts);
            assignment.AvailablePoints = pts;
            Console.Write("  Due Date (MM/DD/YYYY): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime due);
            assignment.DueDate = due;

            CourseServiceProxy.Current.AddAssignment(course.Id, assignment);
            Console.WriteLine($"  Assignment '{assignment.Name}' added.");
        }

        private void EditAssignment(Course course)
        {
            Console.Write("  Enter assignment ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var a = course.Assignments.FirstOrDefault(x => x.Id == id);
            if (a == null) { Console.WriteLine("  Not found."); return; }

            Console.Write($"  Name [{a.Name}]: ");
            var name = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(name)) a.Name = name;

            Console.Write($"  Description [{a.Description}]: ");
            var desc = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(desc)) a.Description = desc;

            Console.Write($"  Available Points [{a.AvailablePoints}]: ");
            var ptsStr = Console.ReadLine().Trim();
            if (double.TryParse(ptsStr, out double pts)) a.AvailablePoints = pts;

            Console.Write($"  Due Date [{a.DueDate:MM/dd/yyyy}] (Enter to skip): ");
            var dateStr = Console.ReadLine().Trim();
            if (DateTime.TryParse(dateStr, out DateTime date)) a.DueDate = date;

            if (a is Quiz quiz)
            {
                Console.Write($"  Question [{quiz.Question}]: ");
                var q = Console.ReadLine().Trim();
                if (!string.IsNullOrWhiteSpace(q)) quiz.Question = q;
            }

            Console.WriteLine("  Assignment updated.");
        }

        private void DeleteAssignment(Course course)
        {
            Console.Write("  Enter assignment ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            bool ok = CourseServiceProxy.Current.DeleteAssignment(course.Id, id);
            Console.WriteLine(ok ? "  Assignment and submissions deleted." : "  Not found.");
        }

        private void CopyAssignmentFromCourse(Course course)
        {
            Console.Write("  Enter source course ID: ");
            if (!int.TryParse(Console.ReadLine(), out int srcId)) return;
            var src = CourseServiceProxy.Current.GetById(srcId);
            if (src == null) { Console.WriteLine("  Course not found."); return; }
            Console.WriteLine($"  Assignments in '{src.Name}':");
            PrintAssignments(src);
            Console.Write("  Enter assignment ID to copy: ");
            if (!int.TryParse(Console.ReadLine(), out int aId)) return;
            var copied = CourseServiceProxy.Current.CopyAssignment(srcId, aId, course.Id);
            Console.WriteLine(copied == null ? "  Not found." : $"  Assignment '{copied.Name}' copied.");
        }

        // ── Modules ───────────────────────────────────────────────────

        private void ManageModules(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("  -- Modules --");
                if (!course.Modules.Any()) Console.WriteLine("  (none)");
                else foreach (var m in course.Modules)
                    Console.WriteLine($"  [{m.Id}] {m.Name} ({m.Content.Count} items)");
                Console.WriteLine();
                Console.WriteLine("  1. Add module");
                Console.WriteLine("  2. Edit module content");
                Console.WriteLine("  3. Delete module");
                Console.WriteLine("  4. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")      AddModule(course);
                else if (choice == "2") EditModuleContent(course);
                else if (choice == "3") DeleteModule(course);
            } while (choice != "4");
        }

        private void AddModule(Course course)
        {
            Console.Write("  Module name: ");
            var name = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(name)) return;
            CourseServiceProxy.Current.AddModule(course.Id, new Module { Name = name });
            Console.WriteLine("  Module added.");
        }

        private void EditModuleContent(Course course)
        {
            Console.Write("  Enter module ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var module = course.Modules.FirstOrDefault(m => m.Id == id);
            if (module == null) { Console.WriteLine("  Not found."); return; }

            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine($"  Module: {module.Name}");
                if (!module.Content.Any()) Console.WriteLine("  (no content)");
                else for (int i = 0; i < module.Content.Count; i++)
                    Console.WriteLine($"  [{i + 1}] {module.Content[i]}");
                Console.WriteLine();
                Console.WriteLine("  1. Add content item");
                Console.WriteLine("  2. Edit content item");
                Console.WriteLine("  3. Remove content item");
                Console.WriteLine("  4. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("  Content: ");
                    var item = Console.ReadLine().Trim();
                    if (!string.IsNullOrWhiteSpace(item)) module.Content.Add(item);
                }
                else if (choice == "2")
                {
                    Console.Write("  Item number to edit: ");
                    if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= module.Content.Count)
                    {
                        Console.Write($"  New value [{module.Content[idx - 1]}]: ");
                        var val = Console.ReadLine().Trim();
                        if (!string.IsNullOrWhiteSpace(val)) module.Content[idx - 1] = val;
                    }
                }
                else if (choice == "3")
                {
                    Console.Write("  Item number to remove: ");
                    if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= module.Content.Count)
                        module.Content.RemoveAt(idx - 1);
                }
            } while (choice != "4");
        }

        private void DeleteModule(Course course)
        {
            Console.Write("  Enter module ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            bool ok = CourseServiceProxy.Current.DeleteModule(course.Id, id);
            Console.WriteLine(ok ? "  Module deleted." : "  Not found.");
        }

        // ── Assignment Groups (Sprint 2) ──────────────────────────────

        private void ManageAssignmentGroups(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine();
                Console.WriteLine("  -- Assignment Groups --");
                if (!course.AssignmentGroups.Any()) Console.WriteLine("  (none)");
                else foreach (var g in course.AssignmentGroups)
                    Console.WriteLine($"  [{g.Id}] {g.Name} | Weight: {g.Weight * 100:F0}%");
                Console.WriteLine();
                Console.WriteLine("  1. Add group");
                Console.WriteLine("  2. Edit group");
                Console.WriteLine("  3. Delete group");
                Console.WriteLine("  4. Add assignment to group");
                Console.WriteLine("  5. Back");
                Console.Write("  Choice: ");
                choice = Console.ReadLine();

                if (choice == "1")      AddAssignmentGroup(course);
                else if (choice == "2") EditAssignmentGroup(course);
                else if (choice == "3") DeleteAssignmentGroup(course);
                else if (choice == "4") AddAssignmentToGroup(course);
            } while (choice != "5");
        }

        private void AddAssignmentGroup(Course course)
        {
            Console.Write("  Group name: ");
            var name = Console.ReadLine().Trim();
            Console.Write("  Weight % (e.g. 30 for 30%): ");
            double.TryParse(Console.ReadLine(), out double w);
            CourseServiceProxy.Current.AddGroup(course.Id, new AssignmentGroup { Name = name, Weight = w / 100.0 });
            Console.WriteLine("  Group added.");
        }

        private void EditAssignmentGroup(Course course)
        {
            Console.Write("  Enter group ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var group = course.AssignmentGroups.FirstOrDefault(g => g.Id == id);
            if (group == null) { Console.WriteLine("  Not found."); return; }
            Console.Write($"  Name [{group.Name}]: ");
            var name = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(name)) group.Name = name;
            Console.Write($"  Weight % [{group.Weight * 100:F0}]: ");
            if (double.TryParse(Console.ReadLine(), out double w)) group.Weight = w / 100.0;
            Console.WriteLine("  Group updated.");
        }

        private void DeleteAssignmentGroup(Course course)
        {
            Console.Write("  Enter group ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            bool ok = CourseServiceProxy.Current.DeleteGroup(course.Id, id);
            Console.WriteLine(ok ? "  Group deleted." : "  Not found.");
        }

        private void AddAssignmentToGroup(Course course)
        {
            Console.Write("  Enter group ID: ");
            if (!int.TryParse(Console.ReadLine(), out int gId)) return;
            PrintAssignments(course);
            Console.Write("  Enter assignment ID: ");
            if (!int.TryParse(Console.ReadLine(), out int aId)) return;
            bool ok = CourseServiceProxy.Current.AddAssignmentToGroup(course.Id, gId, aId);
            Console.WriteLine(ok ? "  Assignment added to group." : "  Group or assignment not found.");
        }

        // ── Grading (Sprint 2) ────────────────────────────────────────

        private void GradeSubmissions(Course course)
        {
            Console.WriteLine();
            PrintAssignments(course);
            Console.Write("  Enter assignment ID to grade: ");
            if (!int.TryParse(Console.ReadLine(), out int aId)) return;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == aId);
            if (assignment == null) { Console.WriteLine("  Not found."); return; }

            if (!assignment.Submissions.Any()) { Console.WriteLine("  No submissions."); return; }

            foreach (var sub in assignment.Submissions)
            {
                var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == sub.StudentId);
                Console.WriteLine();
                Console.WriteLine($"  Student: {student?.Name ?? "Unknown"}");
                Console.WriteLine($"  Submitted: {sub.SubmissionDate:MM/dd/yyyy HH:mm}");
                Console.WriteLine($"  Content: {sub.Content}");
                Console.WriteLine($"  Available Points: {assignment.AvailablePoints}");
                Console.WriteLine("  Grade as: 1-Points  2-Percentage  3-Skip");
                Console.Write("  Choice: ");
                var c = Console.ReadLine().Trim();

                double grade = -1;
                if (c == "1")
                {
                    Console.Write("  Points earned: ");
                    double.TryParse(Console.ReadLine(), out grade);
                }
                else if (c == "2")
                {
                    Console.Write("  Percentage (0-100): ");
                    if (double.TryParse(Console.ReadLine(), out double pct))
                        grade = (pct / 100.0) * assignment.AvailablePoints;
                }

                if (grade >= 0)
                {
                    Console.Write("  Feedback comment (Enter to skip): ");
                    var comment = Console.ReadLine().Trim();
                    CourseServiceProxy.Current.GradeSubmission(course.Id, aId, sub.Id, grade, comment);
                    Console.WriteLine("  Graded.");
                }
            }
        }

        // ── Grade Settings (Sprint 4) ─────────────────────────────────

        private void ManageGradeSettings(Course course)
        {
            var ranges = GradeSettingsServiceProxy.Current.GetRanges(course.Id);
            Console.WriteLine();
            Console.WriteLine("  -- Grade Ranges --");
            foreach (var r in ranges.OrderByDescending(r => r.MinPercent))
                Console.WriteLine($"  {r.Letter}: {r.MinPercent:F1}% - {r.MaxPercent:F1}%  Color: {r.HexColor}");
            Console.WriteLine();
            Console.WriteLine("  1. Edit a grade range");
            Console.WriteLine("  2. Back");
            Console.Write("  Choice: ");
            if (Console.ReadLine().Trim() != "1") return;

            Console.Write("  Letter grade to edit (A/B/C/D/F): ");
            var letter = Console.ReadLine().Trim().ToUpper();
            var range = ranges.FirstOrDefault(r => r.Letter == letter);
            if (range == null) { Console.WriteLine("  Not found."); return; }

            Console.Write($"  Min % [{range.MinPercent}]: ");
            if (double.TryParse(Console.ReadLine(), out double min)) range.MinPercent = min;
            Console.Write($"  Max % [{range.MaxPercent}]: ");
            if (double.TryParse(Console.ReadLine(), out double max)) range.MaxPercent = max;
            Console.Write($"  Hex color [{range.HexColor}] (e.g. #4CAF50): ");
            var hex = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(hex)) range.HexColor = hex;
            Console.WriteLine("  Grade range updated.");
        }

        // ── Gradebook Export (Sprint 4) ───────────────────────────────

        private void ExportGradebook(Course course)
        {
            var csv = CourseServiceProxy.Current.ExportGradebook(course.Id);
            Console.WriteLine();
            Console.WriteLine("  --- Gradebook CSV ---");
            Console.WriteLine(csv);
            Console.WriteLine("  ---------------------");
        }
    }
}
