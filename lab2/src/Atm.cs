namespace AtmLab2;

public sealed class ATM
{
    private readonly CashDispenser _dispenser;
    private readonly TransactionService _service;
    public string TerminalId { get; }

    public ATM(string terminalId, CashDispenser dispenser, TransactionService service)
    {
        TerminalId = terminalId;
        _dispenser = dispenser;
        _service = service;
    }

    public OperationResult WithdrawCash(BankCard card, string pin, decimal amount)
    {
        if (!card.ValidatePin(pin)) return OperationResult.Fail("Неверный PIN-код.");
        var plan = _dispenser.PrepareBanknotes(amount);
        if (plan is null) return OperationResult.Fail("Банкомат не может выдать указанную сумму.");
        if (!card.Account.CanDebit(amount)) return OperationResult.Fail("Недостаточно средств.");

        var transaction = _service.CreateTransaction(TransactionKind.Withdrawal, card.Account, amount);
        if (!transaction.Execute()) return OperationResult.Fail("Списание отклонено.");

        _dispenser.Dispense(plan);
        return OperationResult.Ok($"Выдано {plan.Total:0.00} {card.Account.Currency}.");
    }

    public OperationResult DepositCash(BankCard card, decimal amount)
    {
        var transaction = _service.CreateTransaction(TransactionKind.Deposit, card.Account, amount);
        return transaction.Execute()
            ? OperationResult.Ok($"Зачислено {amount:0.00} {card.Account.Currency}.")
            : OperationResult.Fail("Пополнение отклонено.");
    }
}

public sealed class Bank
{
    private readonly List<ATM> _atms = [];
    public string Name { get; }
    public IReadOnlyList<ATM> Atms => _atms;

    public Bank(string name) => Name = name;
    public void RegisterAtm(ATM atm)
    {
        if (!_atms.Contains(atm)) _atms.Add(atm);
    }
}
