namespace BioGamaEcuador.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entity, string entityId, string? oldValue, string? newValue, string userId, string? ipAddress = null);
    Task LogLoginAsync(string userId, bool success, string? ipAddress = null);
    Task LogMfaChangeAsync(string userId, bool enabled, string? ipAddress = null);
    Task LogRoleChangeAsync(string userId, string oldRole, string newRole, string changedBy);
}
