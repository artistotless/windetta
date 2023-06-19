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
        => _repository.AddAsync(code);

    public Task<AuthorizationCode> GetCodeAsync(string code)
        => _repository.GetAsync(code);

    public Task RemoveCodeAsync(string code)
        => _repository.RemoveAsync(code);
}
