using System;
using System.Collections.Generic;

// a) Record model
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// b) Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c) Concrete processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction t)
        => Console.WriteLine($"[BankTransfer] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction t)
        => Console.WriteLine($"[MobileMoney] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction t)
        => Console.WriteLine($"[CryptoWallet] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}

// d) Base Account
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account {AccountNumber}] New balance: {Balance:C}");
    }
}

// e) Sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }
        Balance -= transaction.Amount;
        Console.WriteLine($"[Savings {AccountNumber}] Deducted {transaction.Amount:C}. Balance: {Balance:C}");
    }
}

// f) App
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var acct = new SavingsAccount("ACC-1001", 1000m);

        var t1 = new Transaction(1, DateTime.Today, 120m, "Groceries");
        var t2 = new Transaction(2, DateTime.Today, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Today, 150m, "Entertainment");

        ITransactionProcessor p1 = new MobileMoneyProcessor();  // t1
        ITransactionProcessor p2 = new BankTransferProcessor(); // t2
        ITransactionProcessor p3 = new CryptoWalletProcessor(); // t3

        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        acct.ApplyTransaction(t1);
        acct.ApplyTransaction(t2);
        acct.ApplyTransaction(t3);

        _transactions.AddRange(new[] { t1, t2, t3 });

        Console.WriteLine("\nAll transactions stored.");
    }
}

public class Program
{
    public static void Main()
    {
        new FinanceApp().Run();
    }
}

