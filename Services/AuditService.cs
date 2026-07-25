using Microsoft.Extensions.Logging;

namespace BioGamaEcuador.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task LogAsync(string action, string entity, string entityId, string? oldValue, string? newValue, string userId, string? ipAddress = null)
    {
        _logger.LogInformation(
            "[AUDIT] Action={Action}, Entity={Entity}, EntityId={EntityId}, UserId={UserId}, IP={IP}, Old={Old}, New={New}",
            action, entity, entityId, userId, ipAddress ?? "-", oldValue ?? "-", newValue ?? "-");
        return Task.CompletedTask;
    }

    public Task LogLoginAsync(string userId, bool success, string? ipAddress = null)
    {
        _logger.LogInformation(
            "[AUDIT-LOGIN] UserId={UserId}, Success={Success}, IP={IP}",
            userId, success, ipAddress ?? "-");
        return Task.CompletedTask;
    }

    public Task LogMfaChangeAsync(string userId, bool enabled, string? ipAddress = null)
    {
        _logger.LogInformation(
            "[AUDIT-MFA] UserId={UserId}, Enabled={Enabled}, IP={IP}",
            userId, enabled, ipAddress ?? "-");
        return Task.CompletedTask;
    }

    public Task LogRoleChangeAsync(string userId, string oldRole, string newRole, string changedBy)
    {
        _logger.LogInformation(
            "[AUDIT-ROLE] UserId={UserId}, OldRole={OldRole}, NewRole={NewRole}, ChangedBy={ChangedBy}",
            userId, oldRole, newRole, changedBy);
        return Task.CompletedTask;
    }
}
