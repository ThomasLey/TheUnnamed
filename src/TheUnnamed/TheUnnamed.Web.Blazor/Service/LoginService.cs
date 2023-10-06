using Microsoft.AspNetCore.Components.Authorization;
using TheUnnamed.Core.Database.Repository;

namespace TheUnnamed.Web.Blazor.Service;

public class LoginService
{
    private readonly IDocumentRepository _repository;
    private readonly AuthenticationStateProvider _authentication;

    public LoginService(IDocumentRepository repository, AuthenticationStateProvider authentication)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
    }

    public void EnsureUser()
    {
        _repository.EnsureUserAsync(new WriteUserEntity()
        {
            DisplayName = _authentication.GetAuthenticationStateAsync().Result.User.Identity?.Name,
            UniqueName = _authentication.GetAuthenticationStateAsync().Result.User.Identity?.Name
        });
    }
}