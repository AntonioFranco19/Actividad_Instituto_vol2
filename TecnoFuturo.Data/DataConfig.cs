namespace TecnoFuturo.Data;

public static class DataConfig
{
    public static string GetSecurepath()
    {
        string? path = Environment.GetEnvironmentVariable("SECURE_PATH");

        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "data");
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static string GetFilePath(string path)
    {
        return Path.Combine(GetSecurepath(), path);
    }
}