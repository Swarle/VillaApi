using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;

namespace DataLayer.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        IRepository<Villa> Villas { get; }
        IRepository<Orders> Orders { get; }
        IRepository<OrderStatus> OrderStatus { get; }
        IRepository<Role> Role { get; }
        IRepository<Users> Users { get; }
        IRepository<VillaDetails> VillaDetails { get; }
        IRepository<VillaStatus> VillaStatus { get; }
    }
}
