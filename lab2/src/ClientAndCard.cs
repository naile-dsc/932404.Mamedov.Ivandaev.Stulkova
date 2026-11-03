using System.Security.Cryptography;
using System.Text;

namespace AtmLab2;

public sealed class Client
{
    private readonly List<BankCard> _cards = [];
    public string FullName { get; }
    public IReadOnlyList<BankCard> Cards => _cards;

    public Client(string fullName) => FullName = fullName;

    public void AddCard(BankCard card)
    {
        ArgumentNullException.ThrowIfNull(card);
        if (!_cards.Contains(card)) _cards.Add(card);
    }
}

public sealed class BankCard
{
    private readonly string _pinHash;
    public string Number { get; }
    public BankAccount Account { get; }

    public BankCard(string number, string pin, BankAccount account)
    {
        Number = number;
        Account = account;
        _pinHash = Hash(pin);
    }

    public bool ValidatePin(string pin) => _pinHash == Hash(pin);

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
