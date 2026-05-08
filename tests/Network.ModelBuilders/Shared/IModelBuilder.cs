namespace Network.ModelBuilders.Shared;

/// <summary>
/// Contract for all test model builders.
/// </summary>
public interface IModelBuilder<T>
{
    /// <summary>
    /// Build and return the constructed instance.
    /// </summary>
    T Build();
}
