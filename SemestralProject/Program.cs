using Microsoft.EntityFrameworkCore;
using SemestralProject.Entities;

namespace SemestralProject
{
    internal class Program
    {
        private static ProjectDbContext dbContext;
        private static User User;
        static void Main(string[] args)
        {
            InitializeDatabase();
            bool loggedIn = false;

            // Loop until the user logs in successfully
            while (!loggedIn)
            {
                Console.WriteLine("Send 1 to login or 2 if you want to register");
                string input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        loggedIn = UserLogin();
                        break;
                    case 2:
                        loggedIn = UserRegister();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        break;
                }
            }
            CreateTask();
        }

        static void InitializeDatabase()
        {
            dbContext = new ProjectDbContext();

            dbContext.Database.EnsureCreated();
        }

        static bool UserLogin()
        {
            Console.WriteLine("Enter a username");
            string username = Console.ReadLine();
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if(user == null)
            {
                Console.WriteLine("User doesn't exist. Press any key to return to the main menu...");
                Console.ReadKey();
                return false;
            }
            User = user;

            if(CheckUserPassword())
            {
                return true;   
            }
            return true;
        }

        static bool CheckUserPassword()
        {
            while (true)
            {
                Console.WriteLine("Enter your password:");
                string enteredPassword = Console.ReadLine();

                if (User.Password != enteredPassword)
                {
                    Console.WriteLine("Wrong password. Please try again.");
                }
                else
                {
                    return true; // Password is correct, return true and exit the loop
                }
            }
        }

        static bool UserRegister()
        {
            Console.WriteLine("Enter a username");
            string username = Console.ReadLine();
            Console.WriteLine("Enter a password");
            string password = Console.ReadLine();

            User user = new User
            {
                UserName = username,
                Password = password
            };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            User = user;
            return true;
        }
        static void CreateTask()
        {
            while (true)
            {
                Console.WriteLine("Send 1 to list all the tasks or 2 if you want to create one");
                string input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                    continue; // Restart the loop to prompt the user again
                }

                if (choice == 1)
                {
                    if (User != null && User.Tasks != null && User.Tasks.Any())
                    {
                        foreach (var task in User.Tasks)
                        {
                            Console.WriteLine($"Task Title: {task.Title}, Task Description: {task.Description}");
                        }
                    }
                    else
                    {
                        // Either User is null, User.Tasks is null, or User has no tasks
                        Console.WriteLine("You don't have any tasks.");
                    }
                    continue; // Exit the loop since a valid choice was made
                }
                else if (choice == 2)
                {
                    Console.WriteLine("Title of the task");
                    string title = Console.ReadLine();
                    Console.WriteLine("Description of the task");
                    string description = Console.ReadLine();

                    UserTask newTask = new UserTask
                    {
                        Title = title,
                        Description = description,
                        UserId = User.Id // Assuming UserId needs to be set for the relationship
                    };

                    if (User.Tasks == null)
                    {
                        User.Tasks = new List<UserTask>();
                    }

                    User.Tasks.Add(newTask);

                    // Save changes to the database
                    dbContext.SaveChanges();

                    continue; // Exit the loop since a valid choice was made
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    // The loop will continue and prompt the user again
                }
            }
        }
    }
}