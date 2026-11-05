namespace DeadlockLab;

public class TransferDemo
{
    public static async Task RunSafeAsync()
    {
        Console.WriteLine("=== SAFE TRANSFER DEMO ===");

        BankAccount account1 = new(10000);
        BankAccount account2 = new(10000);
        BankAccount account3 = new(10000);

        BankTransferService service = new();

        const int operations = 10000;
        const decimal amount = 1;

        decimal initialTotal = account1.Balance + account2.Balance + account3.Balance;
        Console.WriteLine($"Начальный общий баланс: {initialTotal}");

        Task[] tasks = new Task[4];

        tasks[0] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe
                (
                    account1,
                    account2,
                    amount
                );
            }
        });

        tasks[1] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe
                (
                    account2,
                    account1,
                    amount
                );
            }
        });

        tasks[2] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe
                (
                    account1,
                    account3,
                    amount
                );
            }
        });

        tasks[3] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe
                (
                    account3,
                    account1,
                    amount
                );
            }
        });

        await Task.WhenAll(tasks);

        Console.WriteLine($"Баланс A: {account1.Balance}");
        Console.WriteLine($"Баланс B: {account2.Balance}");
        Console.WriteLine($"Баланс C: {account3.Balance}");

        var finalTotal = account1.Balance + account2.Balance + account3.Balance;
        Console.WriteLine($"Общий баланс: {finalTotal}");


        Console.WriteLine("\n--- Проверка на отсутствие потерь ---");
        Console.WriteLine(initialTotal == finalTotal ? "PASS : Общий баланс сохранился!" : "ERROR : Баланс изменился!");

        Console.WriteLine("\n--- Проверка недостатка средств ---");
        try
        {
            service.TransferSafe(account1, account2, 99999m);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("PASS: Перевод отклонён (недостаточно средств). Балансы не изменились.");
        }

        Console.WriteLine("\n--- Проверка перевода самому себе ---");
        try
        {
            service.TransferSafe(account1, account1, 100m);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("PASS: Перевод отклонён (нельзя перевести на тот же счёт).");
        }
    }
}