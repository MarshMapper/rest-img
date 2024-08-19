namespace RestImg
{
    public class CorsHostOptions
    {
        public const string ProjectSection = "RestImg";
        public const string Cors = $"{ProjectSection}:Cors";
        public string[] AllowedHosts { get; set; }
    }
}
