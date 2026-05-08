using FluentResults;

namespace Network.ModelBuilders.Shared;

/// <summary>
/// Contract for builders that wrap entity factory methods returning a Result.
/// </summary>
public interface IEntityModelBuilder<T> : IModelBuilder<T>
{
    /// <summary>
    /// Build and return the Result produced by the entity factory method.
    /// </summary>
    Result<T> BuildResult();
}
