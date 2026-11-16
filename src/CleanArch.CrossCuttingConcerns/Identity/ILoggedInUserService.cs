namespace CleanArch.CrossCuttingConcerns.Identity;

public interface ILoggedInUserService
{
    bool IsAuthenticated { get; }
    string UserId { get; }
}