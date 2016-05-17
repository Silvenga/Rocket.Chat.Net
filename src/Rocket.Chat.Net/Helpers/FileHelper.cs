namespace Rocket.Chat.Net.Helpers
{
    using System.IO;
    using System.Security.Cryptography;

    public class FileHelper
    {
        public static string ConvertToBase64(Stream stream)
        {
            // TODO Check performace

            using (var workspace = new MemoryStream())
            using (var transform = new ToBase64Transform())
            using (var destination = new CryptoStream(workspace, transform, CryptoStreamMode.Write))
            using (var reader = new StreamReader(workspace))
            {
                stream.CopyTo(destination);
                destination.FlushFinalBlock();

                workspace.Position = 0;
                return reader.ReadToEnd();
            }
        }
    }
}