using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private IRepository<Villa> _villas;
        private IRepository<Orders> _orders;
        private IRepository<OrderStatus> _orderStatus;
        private IRepository<Role> _role;
        private IRepository<Users> _users;
        private IRepository<VillaDetails> _villaDetails;
        private IRepository<VillaStatus> _villaStatus;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
        }

        public IRepository<Villa> Villas
        {
            get
            {
                if (_villas == null)
                {
                    _villas = new Repository<Villa>(_context);
                }

                return _villas;
            }

        }
        public IRepository<Orders> Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new OrderRepository(_context);
                }

                return _orders;
            }
        }

        public IRepository<OrderStatus> OrderStatus
        {
            get
            {
                if (_orderStatus == null)
                {
                    _orderStatus = new Repository<OrderStatus>(_context);
                }

                return _orderStatus;
            }
        }

        public IRepository<Role> Role
        {
            get
            {
                if (_role == null)
                {
                    _role = new Repository<Role>(_context);
                }

                return _role;
            }
        }

        public IRepository<Users> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UserRepository(_context);
                }

                return _users;
            }
        }

        public IRepository<VillaDetails> VillaDetails
        {
            get
            {
                if (_villaDetails == null)
                {
                    _villaDetails = new VillaDetailsRepository(_context);
                }

                return _villaDetails;
            }
        }

        public IRepository<VillaStatus> VillaStatus
        {
            get
            {
                if (_villaStatus == null)
                {
                    _villaStatus = new Repository<VillaStatus>(_context);
                }

                return _villaStatus;
            }
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
        }


    }
}
