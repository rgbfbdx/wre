using System;
using System.Data.SqlClient;

class Program
{
    static string connectionString =
        "Server=localhost;Database=Movies;Trusted_Connection=True;";

    static int? currentUserId = null;

    static void Main()
    {
        while (true)
        {
            if (currentUserId == null)
                ShowAuthMenu();
            else
                ShowUserMenu();
        }
    }

    static void ShowAuthMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Реєстрація");
        Console.WriteLine("2. Авторизація");
        Console.WriteLine("0. Вихід");
        Console.Write("Вибір: ");

        switch (Console.ReadLine())
        {
            case "1": Register(); break;
            case "2": Login(); break;
            case "0": Environment.Exit(0); break;
        }
    }

    static void ShowUserMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Додати фільм");
        Console.WriteLine("2. Видалити мій фільм");
        Console.WriteLine("3. Редагувати профіль");
        Console.WriteLine("4. Вийти з акаунту");
        Console.Write("Вибір: ");

        switch (Console.ReadLine())
        {
            case "1": AddMovie(); break;
            case "2": DeleteMovie(); break;
            case "3": EditProfile(); break;
            case "4": currentUserId = null; break;
        }
    }

    static void Register()
    {
        Console.Clear();
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Users (Username, Email, Password) VALUES (@u,@e,@p)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", password);

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Реєстрація успішна!");
            }
            catch
            {
                Console.WriteLine("Помилка. Email вже існує.");
            }
        }
        Pause();
    }

    static void Login()
    {
        Console.Clear();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT Id FROM Users WHERE Email=@e AND Password=@p";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", password);

            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                currentUserId = (int)result;
                Console.WriteLine("Вхід виконано!");
            }
            else
                Console.WriteLine("Невірні дані.");
        }
        Pause();
    }

    static void AddMovie()
    {
        Console.Clear();
        Console.Write("Назва: ");
        string title = Console.ReadLine();

        Console.Write("Рік: ");
        int year = int.Parse(Console.ReadLine());

        Console.Write("Опис: ");
        string desc = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Movies (Title,ReleaseYear,Description,UserId) VALUES (@t,@y,@d,@u)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@y", year);
            cmd.Parameters.AddWithValue("@d", desc);
            cmd.Parameters.AddWithValue("@u", currentUserId);
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Фільм додано!");
        Pause();
    }

    static void DeleteMovie()
    {
        Console.Clear();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string showQuery = "SELECT Id, Title FROM Movies WHERE UserId=@u";
            SqlCommand showCmd = new SqlCommand(showQuery, connection);
            showCmd.Parameters.AddWithValue("@u", currentUserId);

            SqlDataReader reader = showCmd.ExecuteReader();
            while (reader.Read())
                Console.WriteLine($"{reader["Id"]} - {reader["Title"]}");
            reader.Close();

            Console.Write("ID фільму для видалення: ");
            int id = int.Parse(Console.ReadLine());

            string deleteQuery = "DELETE FROM Movies WHERE Id=@id AND UserId=@u";
            SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@id", id);
            deleteCmd.Parameters.AddWithValue("@u", currentUserId);

            deleteCmd.ExecuteNonQuery();
        }

        Console.WriteLine("Фільм видалено.");
        Pause();
    }

    static void EditProfile()
    {
        Console.Clear();
        Console.Write("Новий username: ");
        string username = Console.ReadLine();

        Console.Write("Новий email: ");
        string email = Console.ReadLine();

        Console.Write("Новий password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET Username=@u, Email=@e, Password=@p WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", password);
            cmd.Parameters.AddWithValue("@id", currentUserId);
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Профіль оновлено.");
        Pause();
    }

    static void Pause()
    {
        Console.WriteLine("Натисніть будь-яку клавішу...");
        Console.ReadKey();
    }
}
