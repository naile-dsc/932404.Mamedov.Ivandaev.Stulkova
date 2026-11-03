namespace AtmLab2;

public enum TransactionKind { Withdrawal, Deposit }
public enum TransactionStatus { Created, Completed, Rejected }

public sealed record OperationResult(bool Success, string Message)
{
    public static OperationResult Ok(string message) => new(true, message);
    public static OperationResult Fail(string message) => new(false, message);
}

public sealed record CashPlan(IReadOnlyDictionary<int, int> Banknotes)
{
    public decimal Total => Banknotes.Sum(x => (decimal)x.Key * x.Value);
}
