using CLI.LMS.Helpers;

namespace CLI.LMS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Learning Management System.");

            var userChoice = string.Empty;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please select a user type.");
                Console.WriteLine("1. Student");
                Console.WriteLine("2. Teacher");
                Console.WriteLine("3. Quit");
                Console.Write("Choice: ");

                userChoice = Console.ReadLine();

                if (userChoice == "1")
                {
                    var studentHelper = new StudentMenuHelper();
                    studentHelper.EnterMainMenu();
                }
                else if (userChoice == "2")
                {
                    var teacherHelper = new TeacherMenuHelper();
                    teacherHelper.EnterMainMenu();
                }

            } while (userChoice != "3");
        }
    }
}
