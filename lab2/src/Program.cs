using AtmLab2;

var account = new BankAccount("40817810000000001234", 24_500m);
var card = new BankCard("2200000000007812", "1234", account);
var client = new Client("Иван Петров");
client.AddCard(card);

var dispenser = new CashDispenser([
    new Cassette(5000, 10),
    new Cassette(1000, 100),
    new Cassette(500, 100),
    new Cassette(100, 200)
]);
var journal = new TransactionJournal();
var service = new TransactionService(journal);
var atm = new ATM("ATM-17", dispenser, service);
var bank = new Bank("Учебный банк");
bank.RegisterAtm(atm);

var withdrawal = atm.WithdrawCash(card, "1234", 5_000m);
Console.WriteLine($"Снятие: {withdrawal.Success}. {withdrawal.Message}");
Console.WriteLine($"Баланс после снятия: {account.Balance:0.00} RUB");

var wrongPin = atm.WithdrawCash(card, "0000", 1_000m);
Console.WriteLine($"Неверный PIN: {wrongPin.Success}. {wrongPin.Message}");

var deposit = atm.DepositCash(card, 1_500m);
Console.WriteLine($"Пополнение: {deposit.Success}. {deposit.Message}");
Console.WriteLine($"Баланс после пополнения: {account.Balance:0.00} RUB");
Console.WriteLine($"Транзакций в журнале: {journal.Items.Count}");
