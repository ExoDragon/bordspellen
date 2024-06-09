namespace Core.Infrastructure.Seeder.Util;

public static class BoardGameImageConverter
{
    public static byte[] ConvertImageToBytes(string imageLocation)
    {
        return File.ReadAllBytes(imageLocation);
    }
}