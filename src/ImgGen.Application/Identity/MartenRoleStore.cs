// Copyright (c) SC Strategic Solutions. All rights reserved.

using Domain.Identity.Models;
using JasperFx;
using Marten;
using Microsoft.AspNetCore.Identity;
using SecurityClaim = System.Security.Claims.Claim;

namespace ImgGen.Application.Identity;

public class MartenRoleStore<TRole> :
    IQueryableRoleStore<TRole>,
    IRoleClaimStore<TRole>
    where TRole : Role
{
    private bool _disposed;

    private readonly IDocumentStore _documentStore;


    private IQuerySession QuerySession => _documentStore.QuerySession();

    protected readonly IdentityErrorDescriber ErrorDescriber;

    public IQueryable<TRole> Roles
    {
        get
        {
            ThrowIfDisposed();
            return _documentStore.QuerySession().Query<TRole>();
        }
    }

    public MartenRoleStore(IDocumentStore documentStore, IdentityErrorDescriber? describer = null)
    {
        _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        ErrorDescriber = describer ?? new IdentityErrorDescriber();
    }

    public void Dispose()
    {
        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected Guid ConvertIdFromString(string id) => Guid.Parse(id);

    protected virtual string ConvertIdToString(Guid id) => id.ToString();

    public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await using var session = _documentStore.LightweightSession();

        session.Insert(role);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <summary>
    ///     Updates a role in a store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to update in the store.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
    public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await using var session = _documentStore.LightweightSession();
        session.Update(role);
        try
        {
            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (ConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    ///     Deletes a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to delete from the store.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
    public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await using var session = _documentStore.LightweightSession();
        session.Delete(role);
        try
        {
            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (ConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    ///     Gets the ID for a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose ID should be returned.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the ID of the role.</returns>
    public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return Task.FromResult(role.Id.ToString());
    }

    /// <summary>
    ///     Gets the name of a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be returned.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
    public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return Task.FromResult(role.Name);
    }

    /// <summary>
    ///     Sets the name of a role in the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be set.</param>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.Name = roleName;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Get a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
    public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return Task.FromResult(role.NormalizedName);
    }

    /// <summary>
    ///     Set a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be set.</param>
    /// <param name="normalizedName">The normalized name to set</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetNormalizedRoleNameAsync(
        TRole role,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Finds the role who has the specified ID as an asynchronous operation.
    /// </summary>
    /// <param name="roleId">The role ID to look for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
    public virtual async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await QuerySession
            .Query<TRole>()
            .SingleOrDefaultAsync(r => r.Id.ToString() == roleId, token: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Finds the role who has the specified normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="normalizedRoleName">The normalized role name to look for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
    public virtual async Task<TRole?> FindByNameAsync(
        string normalizedRoleName,
        CancellationToken cancellationToken)
    {
        var role = await QuerySession
            .Query<TRole>()
            .SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, token: cancellationToken)
            .ConfigureAwait(false);

        return role;
    }

    /// <summary>
    ///     Get the claims associated with the specified <paramref name="role" /> as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose claims should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the claims granted to a role.</returns>
    public virtual Task<IList<SecurityClaim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new())
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        var claims = role.Claims.Select(rc => rc.ToClaim()).ToList();
        return Task.FromResult<IList<SecurityClaim>>(claims);
    }

    /// <summary>
    ///     Adds the <paramref name="claim" /> given to the specified <paramref name="role" />.
    /// </summary>
    /// <param name="role">The role to add the claim to.</param>
    /// <param name="claim">The claim to add to the role.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task AddClaimAsync(TRole role, SecurityClaim claim, CancellationToken cancellationToken = new())
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        var martenIdentityClaim = claim.ToModelClaim();
        if (!role.Claims.Contains(martenIdentityClaim))
        {
            role.Claims.Add(martenIdentityClaim);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Removes the <paramref name="claim" /> given from the specified <paramref name="role" />.
    /// </summary>
    /// <param name="role">The role to remove the claim from.</param>
    /// <param name="claim">The claim to remove from the role.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task RemoveClaimAsync(TRole role, SecurityClaim claim, CancellationToken cancellationToken = new())
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        var claimsToRemove = role.Claims.Where(
                rc =>
                    string.Equals(rc.Type, claim.Type, StringComparison.Ordinal)
                    && string.Equals(rc.Value, claim.Value, StringComparison.Ordinal)
            )
            .ToArray();

        foreach (var martenIdentityClaim in claimsToRemove)
        {
            role.Claims.Remove(martenIdentityClaim);
        }

        return Task.CompletedTask;
    }
}
