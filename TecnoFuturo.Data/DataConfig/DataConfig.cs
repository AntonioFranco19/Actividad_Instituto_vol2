using Microsoft.Extensions.Options;
using TecnoFuturo.Core.Options;

namespace TecnoFuturo.Data.DataConfig;

public class DataConfig
{
    private readonly IOptions<DirectoryOption> _directories;

    public DataConfig(IOptions<DirectoryOption> directories)
    {
        _directories = directories;
    }
    public string GetSecurepath()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), _directories.Value.Data);

        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TecnoFuturo_data");
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public string GetSecureFilePath(string path)
    {
        return Path.Combine(GetSecurepath(), path);
    }

    public string GetBackupPath()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), _directories.Value.Backup);
        
        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TecnoFuturo_backup");
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        return path;
    }

    public string GetBackUpFilePath(string path)
    {
        return Path.Combine(GetBackupPath(), path);
    }
}