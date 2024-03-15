namespace DataCrafter.Reflection.OrderBy;

public class OrderBy<TToOrder, TBy> : IOrderBy
{
    private readonly Func<TToOrder, TBy> _expression;

    public OrderBy(Func<TToOrder, TBy> expression)
    {
        _expression = expression;
    }

    public dynamic Expression => _expression;
}
