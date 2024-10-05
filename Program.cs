using CopyFolder;
using System.Text.Json;

var config = GetConfigFromFile();
if (config.TargetPath =="" || config.SourcePath == "")
{
    Console.WriteLine("Unnable to read the config file.");
    Console.WriteLine("The file must be in the same folder as the .exe file.");
    Console.WriteLine("And have the following aspect:");
    Console.WriteLine(@"{""SourcePath"":""C:\\proj"",""TargetPath"":""C:\\projBKP""}");
    Console.ReadKey(true);
    return;
}

var sourceFiles = GetFilesAndSubFiles(config.SourcePath);

if(sourceFiles.Count < 1)
{
    Console.WriteLine($"There are no files in the informed directory: {config.SourcePath}");
    Console.ReadKey(true);
    return;
}

Console.WriteLine($"{sourceFiles.Count} files found.");
Console.WriteLine($"Copying files...");

//DirectoryInfo info = new (config.TargetPath);
//if(!info.Exists) info.Create();
//await CopyFilesAsync(sourceFiles, config.TargetPath);

Copy(config.SourcePath, config.TargetPath);

Console.WriteLine("End of the copy.");
Console.ReadKey(true);

Config GetConfigFromFile()
{
    try
    {
        var currentApp = Environment.ProcessPath;
        var currentDirectory = new FileInfo(currentApp ?? "").DirectoryName;
        
        if (currentDirectory == "") return new Config();
        
        var jsonContent = File.ReadAllText($@"{currentDirectory}/config.json");
        var classFromJson = JsonSerializer.Deserialize<Config>(jsonContent);
        
        if(classFromJson == null) return new Config();
        
        return classFromJson;
    }
    catch
    {
        return new Config();
    }
}

async Task CopyFilesAsync(List<string> sourceFiles, string targetPath)
{
    var tasks = new List<Task>();
    foreach (var i in sourceFiles)
    {
        FileInfo fileInfo = new(i);
        tasks.Add(CopyFileAsync(i, $@"{targetPath}\{fileInfo.Name}"));
    }
    await Task.WhenAll(tasks);
}

async Task CopyFileAsync(string sourceFile, string targetFile)
{
    try
    {
        using (Stream source = File.Open(sourceFile, FileMode.Open, FileAccess.ReadWrite))
        {
            using (Stream destination = File.Create(targetFile))
            {
                await source.CopyToAsync(destination);
            }
        }
        Console.WriteLine($"{targetFile}: copied.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{targetFile}: {ex.Message}.");
    }
}

List<string> GetFilesAndSubFiles(string path)
{
    var files = new List<string>();

    DirectoryInfo info = new(path);
    if (!info.Exists) return files;

    files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList());    
    return files;
}

void Copy(string sourceDirectory, string targetDirectory)
{
    DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
    DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

    CopyAll(diSource, diTarget);
}

void CopyAll(DirectoryInfo source, DirectoryInfo target)
{
    Directory.CreateDirectory(target.FullName);

    // Copy each file into the new directory.
    foreach (FileInfo fi in source.GetFiles())
    {
        Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
        fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
    }

    // Copy each subdirectory using recursion.
    foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
    {
        DirectoryInfo nextTargetSubDir =
            target.CreateSubdirectory(diSourceSubDir.Name);
        CopyAll(diSourceSubDir, nextTargetSubDir);
    }
}