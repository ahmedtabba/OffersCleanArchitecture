using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Offers.CleanArchitecture.Application.Common.Exceptions;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Domain.Common;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public sealed class UnitOfWorkAsync : IUnitOfWorkAsync
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    private readonly IMediator _mediator;
    public UnitOfWorkAsync(AppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null || _transaction.GetDbTransaction().Connection?.State!=ConnectionState.Open)
        {
            // No active transaction, so no changes are saved
            throw new NoActiveTransactionException();
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null && _transaction.GetDbTransaction().Connection?.State == ConnectionState.Open)
        {
           await DispatchDomainEvents(_context);
           await _transaction.CommitAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null && _transaction.GetDbTransaction().Connection?.State == ConnectionState.Open)
        {
            await _transaction.RollbackAsync();
        }
    }

    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }
    public void Dispose()
    {
        if (_transaction != null && _transaction.GetDbTransaction().Connection?.State == ConnectionState.Open)
        {
            _transaction.Dispose();
        }
    }
}
