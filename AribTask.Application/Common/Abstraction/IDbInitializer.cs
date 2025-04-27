namespace AribTask.Application.Common.Abstraction
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}