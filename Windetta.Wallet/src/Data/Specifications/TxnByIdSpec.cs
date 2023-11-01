using LinqSpecs;
using System.Linq.Expressions;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Data.Specifications;

public class WalletByUserIdSpec : Specification<UserWallet>
{
    private readonly Guid _userId;

    public WalletByUserIdSpec(Guid userId)
    {
        _userId = userId;
    }

    public override Expression<Func<UserWallet, bool>> ToExpression()
        => x => x.UserId == _userId;
}
