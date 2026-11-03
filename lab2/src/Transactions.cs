namespace AtmLab2;

public interface ITransaction
{
    Guid Id { get; }
    decimal Amount { get; }
    DateTime CreatedAt { get; }
    TransactionStatus Status { get; }
    bool Execute();
}

public sealed class WithdrawalTransaction : ITransaction
{
    private readonly BankAccount _account;
    public Guid Id { get; } = Guid.NewGuid();
    public decimal Amount { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public TransactionStatus Status { get; private set; } = TransactionStatus.Created;

    public WithdrawalTransaction(BankAccount account, decimal amount)
    {
        _account = account;
        Amount = amount;
    }

    public bool Execute()
    {
        var success = _account.Debit(Amount);
        Status = success ? TransactionStatus.Completed : TransactionStatus.Rejected;
        return success;
    }
}

public sealed class DepositTransaction : ITransaction
{
    private readonly BankAccount _account;
    public Guid Id { get; } = Guid.NewGuid();
    public decimal Amount { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public TransactionStatus Status { get; private set; } = TransactionStatus.Created;

    public DepositTransaction(BankAccount account, decimal amount)
    {
        _account = account;
        Amount = amount;
    }

    public bool Execute()
    {
        if (Amount <= 0)
        {
            Status = TransactionStatus.Rejected;
            return false;
        }
        _account.Credit(Amount);
        Status = TransactionStatus.Completed;
        return true;
    }
}

public sealed class TransactionJournal
{
    private readonly List<ITransaction> _items = [];
    public IReadOnlyList<ITransaction> Items => _items;

    public void Register(ITransaction transaction) => _items.Add(transaction);
    public ITransaction? Find(Guid id) => _items.SingleOrDefault(x => x.Id == id);
}

public sealed class TransactionService
{
    private readonly TransactionJournal _journal;
    public TransactionService(TransactionJournal journal) => _journal = journal;

    public ITransaction CreateTransaction(TransactionKind kind, BankAccount account, decimal amount)
    {
        ITransaction transaction = kind switch
        {
            TransactionKind.Withdrawal => new WithdrawalTransaction(account, amount),
            TransactionKind.Deposit => new DepositTransaction(account, amount),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        _journal.Register(transaction);
        return transaction;
    }
}
