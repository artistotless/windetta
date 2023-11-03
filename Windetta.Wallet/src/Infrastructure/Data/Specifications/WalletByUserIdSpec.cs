using LinqSpecs;
using System.Linq.Expressions;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Infrastructure.Data.Specifications;

public class TxnByIdSpec : Specification<Transaction>
{
    private readonly Guid _id;

    public TxnByIdSpec(Guid id)
    {
        _id = id;
    }

    public override Expression<Func<Transaction, bool>> ToExpression()
        => x => x.Id == _id;
}
