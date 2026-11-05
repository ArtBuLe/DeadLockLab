namespace DeadlockLab;


public class BankTransferService
{
    public void TransferUnsafe(BankAccount from, BankAccount to, decimal amount)
    {
        lock (from.SyncRoot)
        {
            lock (to.SyncRoot)
            {
                if (from.GetBalanceUnsafe() < amount)
                    throw new InvalidOperationException("Недостаточно средств.");

                from.SetBalanceUnsafe(from.GetBalanceUnsafe() - amount);
                to.SetBalanceUnsafe(to.GetBalanceUnsafe() + amount);
            }
        }
    }
    public void TransferSafe(BankAccount from, BankAccount to, decimal amount)
    {
        if (from.Id == to.Id)
        {
            throw new ArgumentException("Нельзя перевести деньги на тот же счёт.");
        }

        object lock1, lock2;    

        if (from.Id < to.Id)
        {
            lock1 = from.SyncRoot;
            lock2 = to.SyncRoot;
        }
        else 
        {
            lock1 = to.SyncRoot;
            lock2 = from.SyncRoot;
        }

        lock (lock1)
        {
            lock (lock2)
            {
                if (from.GetBalanceUnsafe() < amount)
                    throw new InvalidOperationException("Недостаточно средств.");

                from.SetBalanceUnsafe(from.GetBalanceUnsafe() - amount);
                to.SetBalanceUnsafe(to.GetBalanceUnsafe() + amount);
            }
        }
    }
}