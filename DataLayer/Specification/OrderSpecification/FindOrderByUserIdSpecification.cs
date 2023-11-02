using System.Linq.Expressions;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.OrderSpecification;

public class FindOrderByUserIdSpecification : BaseSpecification<Orders>
{
    public FindOrderByUserIdSpecification(Guid userId) : base(o => o.UserId == userId)
    {
        AddInclude(e => e.Villa);
        AddInclude(e => e.Status);
        AddInclude(e => e.Villa.VillaDetails);
        AddInclude(e => e.Villa.Status);
        AddInclude(e => e.User);
        AddInclude(e => e.User.Role);
    }
}