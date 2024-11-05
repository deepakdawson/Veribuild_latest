namespace App.Entity.Config
{
    public class AppConfig
    {
        public const string Key = "AppSetting";

        public string Contract { get; set; } = string.Empty;
        public string PropertiesVideos { get; set; } = string.Empty;
        public string FloorImages { get; set; } = string.Empty;
        public string GalleryImages { get; set; } = string.Empty;
        public string UserCredential { get; set; } = string.Empty;
        public string PropertyDocument { get; set; } = string.Empty;
        public string FloorPlanDoc { get; set; } = string.Empty;
        public string CustomEventDoc { get; set; } = string.Empty;
    }
}
