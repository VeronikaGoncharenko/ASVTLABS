using System.Numerics;
using Lab5.Network.Common;
using Lab5.Network.Common.UserApi;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var serverAdress = new Uri("tcp://127.0.0.1:5555");
        var client = new NetTcpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");
        await client.ConnectAsync();

        var userApi = new UserApiClient(client);
        await ManageUsers(userApi);
        client.Dispose();
    }

    private static async Task ManageUsers(IUserApi userApi)
    {
        PrintMenu();

        while(true) {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1) 
            {
                var users = await userApi.GetAllAsync();
                Console.WriteLine($"| Id    |      Имя       |     Количество сданных лаб по ассемблеру     |");
                foreach (var user in users)
                {
                    Console.WriteLine($"| {user.Id,5} | {user.name,10} | {user.kolvo,10} |");
                }
            }

            if (key.Key == ConsoleKey.D2) 
            {
                Console.Write("Введите айди студента: ");
                var userIdString = Console.ReadLine();
                int.TryParse(userIdString, out var userId);
                var user = await userApi.GetAsync(userId);
                Console.WriteLine($"| {user.Id,5} | {user.name,10} | {user.kolvo,10} |");
            }

            if (key.Key == ConsoleKey.D3) 
            {

                Console.Write("Напишите имя студента: ");
                var addMan = Console.ReadLine() ?? "empty";
                Console.Write("Напишите количество сданных лабораторных работ: ");
                var newKolVo = Console.ReadLine() ?? "нет";
                var addUser = new User(Id: 0,
                    name: addMan,
                    kolvo: newKolVo
                );
                var addResult = await userApi.AddAsync(addUser);
                Console.WriteLine(addResult ? "Ok" : "Error");
                
            }
            if (key.Key == ConsoleKey.D4) // Обновление только статуса пользователя
            {
                Console.Write("Введите айди студента: ");
                var updateIdString = Console.ReadLine();
                int.TryParse(updateIdString, out var updateId);

                // Получаем текущие данные пользователя
                var existingUser = await userApi.GetAsync(updateId);
                if (existingUser == null)
                {
                    Console.WriteLine("Не найдено.");
                    continue;
                }
                Console.Write("Напишите новое количество сданных лабораторных работ: ");
                var addDescription = Console.ReadLine() ?? "empty";

                // Создаем новый объект пользователя с измененным только статусом
                var updatedUser = new User(
                    Id: existingUser.Id,
                    name: existingUser.name,
                    kolvo: addDescription
                );

                var updateResult = await userApi.UpdateAsync(updateId, updatedUser);

                Console.WriteLine(updateResult ? "Обновлено" : "Ошибка при обновлении");
            }
            if (key.Key == ConsoleKey.D5) 
            {
                Console.Write("Введите айди студента: ");
                var deleteIdString = Console.ReadLine();
                int.TryParse(deleteIdString, out var deleteId);

                var deleteResult = await userApi.DeleteAsync(deleteId);

                Console.WriteLine(deleteResult ? "Удалено" : "Ошибка при удалении");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
        //while (Console.Read)
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Вывести всех студентов");
            Console.WriteLine("2 - Показать студента по id");
            Console.WriteLine("3 - Добавить студента");
            Console.WriteLine("4 - Изменить количество сданных лабораторных работ");
            Console.WriteLine("5 - Удалить студента");
            Console.WriteLine("-------");
        }
    }
    
    

}
