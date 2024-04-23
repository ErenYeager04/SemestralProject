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
            // Inicializace databáze
            InitializeDatabase();
            bool loggedIn = false;

            // Smyčka dokud uživatel úspěšně nepřihlásí
            while (!loggedIn)
            {
                Console.WriteLine("Pošlete 1 pro přihlášení nebo 2 pokud chcete registrovat");
                string input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Neplatný vstup. Zadejte prosím 1 nebo 2.");
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
                        Console.WriteLine("Neplatná volba. Zadejte prosím 1 nebo 2.");
                        break;
                }
            }
            CreateTask();
        }

        static void InitializeDatabase()
        {
            // Inicializace kontextu databáze
            dbContext = new ProjectDbContext();

            // Zajištění vytvoření databáze (pokud neexistuje)
            dbContext.Database.EnsureCreated();
        }

        static bool UserLogin()
        {
            Console.WriteLine("Zadejte uživatelské jméno");
            string username = Console.ReadLine();
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                Console.WriteLine("Uživatel neexistuje. Stiskněte libovolnou klávesu pro návrat do hlavního menu...");
                Console.ReadKey();
                return false;
            }
            User = user;

            if (CheckUserPassword())
            {
                return true;
            }
            return true;
        }

        static bool CheckUserPassword()
        {
            while (true)
            {
                Console.WriteLine("Zadejte heslo:");
                string enteredPassword = Console.ReadLine();

                if (User.Password != enteredPassword)
                {
                    Console.WriteLine("Nesprávné heslo. Zkuste to znovu.");
                }
                else
                {
                    return true; // Heslo je správné, vrátí true a ukončí smyčku
                }
            }
        }

        static bool UserRegister()
        {
            Console.WriteLine("Zadejte uživatelské jméno");
            string username = Console.ReadLine();
            Console.WriteLine("Zadejte heslo");
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
                Console.WriteLine("Pošlete 1 pro výpis všech úkolů nebo 2 pokud chcete vytvořit nový");
                string input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Neplatný vstup. Zadejte prosím 1 nebo 2.");
                    continue; // Restartovat smyčku pro znovu vyzvání uživatele
                }

                if (choice == 1)
                {
                    if (User != null && User.Tasks != null && User.Tasks.Any())
                    {
                        foreach (var task in User.Tasks)
                        {
                            Console.WriteLine($"Název úkolu: {task.Title}, Popis úkolu: {task.Description}");
                        }
                    }
                    else
                    {
                        // Buď je User null, User.Tasks je null nebo User nemá žádné úkoly
                        Console.WriteLine("Nemáte žádné úkoly.");
                    }
                    continue; // Ukončit smyčku, protože byla provedena platná volba
                }
                else if (choice == 2)
                {
                    Console.WriteLine("Název úkolu");
                    string title = Console.ReadLine();
                    Console.WriteLine("Popis úkolu");
                    string description = Console.ReadLine();

                    UserTask newTask = new UserTask
                    {
                        Title = title,
                        Description = description,
                        UserId = User.Id // Předpokládá se, že UserId musí být nastaveno pro vztah
                    };

                    if (User.Tasks == null)
                    {
                        User.Tasks = new List<UserTask>();
                    }

                    User.Tasks.Add(newTask);

                    // Uložení změn do databáze
                    dbContext.SaveChanges();

                    continue; // Ukončit smyčku, protože byla provedena platná volba
                }
                else
                {
                    Console.WriteLine("Neplatná volba. Zadejte prosím 1 nebo 2.");
                    // Smyčka pokračuje a znovu vyzve uživatele
                }
            }
        }
    }
}
