namespace BioGamaEcuador.Settings;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderName { get; set; } = "BioGama";
    public string SenderEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
