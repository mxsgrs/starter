namespace Sales.WebApi.Persistence;

public class User
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Register an existing Network user in the Sales database for FK constraint purposes
    /// </summary>
    public static User Register(Guid id) => new() { Id = id };

    private User() { }
}
