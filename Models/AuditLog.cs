namespace BioGamaEcuador.Models;

public sealed class AuditLog
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
