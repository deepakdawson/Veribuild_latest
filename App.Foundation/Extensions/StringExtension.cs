using SixLabors.ImageSharp;

namespace App.Foundation.Extensions
{
    public static class StringExtension
    {
        public static Stream Base64ImageToBitmap(this string imageData)
        {
            byte[] byteBuffer = Convert.FromBase64String(imageData);
            MemoryStream memoryStream = new(byteBuffer)
            {
                Position = 0
            };
            var image = Image.Load(memoryStream);
            MemoryStream ms = new();
            image.SaveAsJpeg(ms);
            return ms;
        }

    }
}
