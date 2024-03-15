using Microsoft.Extensions.Options;

namespace DataCrafter.Options.WritableOptions;
public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
}
