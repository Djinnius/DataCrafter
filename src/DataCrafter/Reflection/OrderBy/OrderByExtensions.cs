namespace DataCrafter.Reflection.OrderBy;
internal static class OrderByExtensions
{
    public static IOrderedEnumerable<TToOrder> OrderBy<TToOrder>(this IEnumerable<TToOrder> source, IOrderBy orderBy)
        => Enumerable.OrderBy(source, orderBy.Expression);

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IOrderBy orderBy)
        => Queryable.OrderBy(source, orderBy.Expression);

    public static IOrderedEnumerable<TToOrder> OrderByDescending<TToOrder>(this IEnumerable<TToOrder> source, IOrderBy orderBy)
        => Enumerable.OrderByDescending(source, orderBy.Expression);

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, IOrderBy orderBy)
        => Queryable.OrderByDescending(source, orderBy.Expression);

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, IOrderBy orderBy)
        => Queryable.ThenBy(source, orderBy.Expression);

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, IOrderBy orderBy)
        => Queryable.ThenByDescending(source, orderBy.Expression);
}
