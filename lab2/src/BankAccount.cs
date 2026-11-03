namespace AtmLab2;

public sealed class BankAccount
{
    public string AccountNumber { get; }
    public decimal Balance { get; private set; }
    public string Currency { get; }

    public BankAccount(string accountNumber, decimal balance, string currency = "RUB")
    {
        AccountNumber = accountNumber;
        Balance = balance;
        Currency = currency;
    }

    public bool CanDebit(decimal amount) => amount > 0 && Balance >= amount;

    public bool Debit(decimal amount)
    {
        if (!CanDebit(amount)) return false;
        Balance -= amount;
        return true;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }
}
