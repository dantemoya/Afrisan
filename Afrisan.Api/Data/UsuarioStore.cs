using Afrisan.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Afrisan.Api.Data;

// Identity conserva el hashing, bloqueo y validación de cuentas. Solo se adapta
// su relación de roles: RolId es la única fuente de pertenencia, sin tabla puente.
public sealed class UsuarioStore(AfrisanDbContext context, IdentityErrorDescriber describer)
    : UserStore<Usuario, IdentityRole, AfrisanDbContext, string>(context, describer)
{
    public override async Task AddToRoleAsync(Usuario user, string normalizedRoleName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var role = await Context.Roles.SingleOrDefaultAsync(
            x => x.NormalizedName == normalizedRoleName, cancellationToken)
            ?? throw new InvalidOperationException("El rol no existe.");
        if (user.RolId is not null && user.RolId != role.Id)
            throw new InvalidOperationException("La cuenta ya tiene un rol. Retira el anterior antes de asignar otro.");
        user.RolId = role.Id;
    }

    public override async Task RemoveFromRoleAsync(Usuario user, string normalizedRoleName,
        CancellationToken cancellationToken = default)
    {
        if (await IsInRoleAsync(user, normalizedRoleName, cancellationToken)) user.RolId = null;
    }

    public override async Task<IList<string>> GetRolesAsync(Usuario user,
        CancellationToken cancellationToken = default)
    {
        return await Context.Roles.Where(x => x.Id == user.RolId && x.Name != null)
            .Select(x => x.Name!).ToListAsync(cancellationToken);
    }

    public override Task<bool> IsInRoleAsync(Usuario user, string normalizedRoleName,
        CancellationToken cancellationToken = default)
        => Context.Roles.AnyAsync(x => x.Id == user.RolId && x.NormalizedName == normalizedRoleName,
            cancellationToken);

    public override async Task<IList<Usuario>> GetUsersInRoleAsync(string normalizedRoleName,
        CancellationToken cancellationToken = default)
    {
        return await Context.Users.Where(x => Context.Roles.Any(
            role => role.Id == x.RolId && role.NormalizedName == normalizedRoleName))
            .ToListAsync(cancellationToken);
    }
}
