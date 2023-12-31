﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Windetta.Common.Types;
using Windetta.Identity.Domain.Entities;

namespace Windetta.Identity.Infrastructure.Data;

public class IdentityDbContext : IdentityDbContext<User, Role, Guid>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get all the entities that inherit from AuditableEntity
        // and have a state of Added or Modified
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity &&
             (e.State == EntityState.Added || e.State == EntityState.Modified));

        // For each entity we will set the Audit properties
        foreach (var entityEntry in entries)
        {
            // If the entity state is Added let's set
            // the  CreatedAt property
            if (entityEntry.State == EntityState.Added)
                ((AuditableEntity)entityEntry.Entity).Created = DateTimeOffset.UtcNow;
            else
                // If the state is Modified then we don't want
                // to modify the CreatedAt and CreatedAt properties
                // so we set their state as IsModified to false
                Entry((AuditableEntity)entityEntry.Entity).Property(p => p.Created).IsModified = false;

            // In any case we always want to set the properties
            // LastModifiedAt
            ((AuditableEntity)entityEntry.Entity).LastModified = DateTimeOffset.UtcNow;
        }

        // After we set all the needed properties
        // we call the base implementation of SaveChangesAsync
        // to actually save our entities in the database
        return await base.SaveChangesAsync(cancellationToken);
    }
}
