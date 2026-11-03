namespace AtmLab2;

public sealed class Cassette
{
    public int Denomination { get; }
    public int Count { get; private set; }

    public Cassette(int denomination, int count)
    {
        if (denomination <= 0 || count < 0) throw new ArgumentOutOfRangeException();
        Denomination = denomination;
        Count = count;
    }

    public bool CanProvide(int count) => count >= 0 && Count >= count;

    public void Take(int count)
    {
        if (!CanProvide(count)) throw new InvalidOperationException("Недостаточно купюр в кассете.");
        Count -= count;
    }
}

public sealed class CashDispenser
{
    private readonly List<Cassette> _cassettes;
    public IReadOnlyList<Cassette> Cassettes => _cassettes;

    public CashDispenser(IEnumerable<Cassette> cassettes) =>
        _cassettes = cassettes.OrderByDescending(x => x.Denomination).ToList();

    public CashPlan? PrepareBanknotes(decimal amount)
    {
        if (amount <= 0 || decimal.Truncate(amount) != amount) return null;
        var remaining = (int)amount;
        var selected = new Dictionary<int, int>();

        foreach (var cassette in _cassettes)
        {
            var wanted = remaining / cassette.Denomination;
            var count = Math.Min(wanted, cassette.Count);
            if (count == 0) continue;
            selected[cassette.Denomination] = count;
            remaining -= count * cassette.Denomination;
        }

        return remaining == 0 ? new CashPlan(selected) : null;
    }

    public void Dispense(CashPlan plan)
    {
        foreach (var item in plan.Banknotes)
        {
            var cassette = _cassettes.Single(x => x.Denomination == item.Key);
            if (!cassette.CanProvide(item.Value))
                throw new InvalidOperationException("Состав кассет изменился после подготовки выдачи.");
        }
        foreach (var item in plan.Banknotes)
            _cassettes.Single(x => x.Denomination == item.Key).Take(item.Value);
    }
}
