using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace BioGamaEcuador.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailService _email;

    public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService email)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _email = email;
    }

    public async Task<AccountResult> RegisterAsync(string email, string password, string? role = null)
    {
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return Fail(result.Errors.FirstOrDefault()?.Description ?? "Error al registrar usuario.");

        if (!string.IsNullOrEmpty(role))
            await _userManager.AddToRoleAsync(user, role);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

        // Note: The calling controller should build the full confirmation link and call SendConfirmationLinkAsync
        return new AccountResult { Success = true, UserId = user.Id };
    }

    public async Task<AccountResult> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Fail("Usuario no encontrado.");

        var code = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded
            ? new AccountResult { Success = true, UserId = user.Id }
            : Fail("Error al confirmar el correo.");
    }

    public async Task<AccountResult> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            return new AccountResult { Success = true }; // No revelar si el usuario existe

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

        return new AccountResult { Success = true, UserId = user.Id };
    }

    public async Task<AccountResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Fail("Usuario no encontrado.");

        var code = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ResetPasswordAsync(user, code, newPassword);
        return result.Succeeded
            ? new AccountResult { Success = true }
            : Fail(result.Errors.FirstOrDefault()?.Description ?? "Error al restablecer la contrasena.");
    }

    public async Task<AccountResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Fail("Usuario no encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded
            ? new AccountResult { Success = true }
            : Fail(result.Errors.FirstOrDefault()?.Description ?? "Error al cambiar la contrasena.");
    }

    public async Task<AccountResult> EnableMfaAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Fail("Usuario no encontrado.");

        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        return result.Succeeded
            ? new AccountResult { Success = true }
            : Fail("Error al habilitar MFA.");
    }

    public async Task<AccountResult> DisableMfaAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Fail("Usuario no encontrado.");

        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        return result.Succeeded
            ? new AccountResult { Success = true }
            : Fail("Error al deshabilitar MFA.");
    }

    public async Task<AccountResult> VerifyMfaTokenAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Fail("Usuario no encontrado.");

        var valid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
        return valid
            ? new AccountResult { Success = true }
            : Fail("Codigo de verificacion invalido.");
    }

    public async Task<IReadOnlyList<string>> GenerateRecoveryCodesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Array.Empty<string>();

        var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        return codes?.ToList() ?? new List<string>();
    }

    private static AccountResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
