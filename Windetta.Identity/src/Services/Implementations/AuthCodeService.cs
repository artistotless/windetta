using Windetta.Common.Authentication;
using Windetta.Identity.Data.Repositories;

namespace Windetta.Identity.Services;

public class AuthCodeService : IAuthCodeService
{
    private readonly IAuthCodesRepository _repository;

    public AuthCodeService(IAuthCodesRepository repository)
    {
        _repository = repository;
    }

    public Task AddCodeAsync(AuthorizationCode code)
    {
        throw new NotImplementedException();
    }

    public Task<AuthorizationCode> GetCodeAsync(string code)
    {
        throw new NotImplementedException();
    }

    public Task RemoveCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
}
