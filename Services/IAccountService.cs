namespace BioGamaEcuador.Services;

public interface IAccountService
{
    Task<AccountResult> RegisterAsync(string email, string password, string? role = null);
    Task<AccountResult> ConfirmEmailAsync(string userId, string token);
    Task<AccountResult> ForgotPasswordAsync(string email);
    Task<AccountResult> ResetPasswordAsync(string email, string token, string newPassword);
    Task<AccountResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<AccountResult> EnableMfaAsync(string userId);
    Task<AccountResult> DisableMfaAsync(string userId);
    Task<AccountResult> VerifyMfaTokenAsync(string userId, string code);
    Task<IReadOnlyList<string>> GenerateRecoveryCodesAsync(string userId);
}

public class AccountResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UserId { get; set; }
}
