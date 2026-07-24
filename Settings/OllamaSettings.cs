namespace BioGamaEcuador.Settings
{
    public class OllamaSettings
    {
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "gemma2:2b";
        public int TimeoutSeconds { get; set; } = 90;
    }
}
