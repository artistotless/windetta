using LinqSpecs;
using System.Linq.Expressions;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Data.Specifications;

public class TxnByIdSpec : Specification<Transaction>
{
    private readonly string _id;

    public TxnByIdSpec(string id)
    {
        _id = id;
    }

    public override Expression<Func<Transaction, bool>> ToExpression()
        => x => x.Id == _id;
}
