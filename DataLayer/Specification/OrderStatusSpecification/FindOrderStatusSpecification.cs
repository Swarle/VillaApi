using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.OrderStatusSpecification;

public class FindOrderStatusSpecification : BaseSpecification<OrderStatus>
{
    public FindOrderStatusSpecification(string status) : base(s => s.Status == status){}
    public FindOrderStatusSpecification(Guid id) : base(s => s.Id == id){}
}