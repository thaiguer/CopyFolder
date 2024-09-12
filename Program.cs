Console.WriteLine("Hello!");

string sourcePath = @"C:\proj";
string targetPath = @"C:\projBKP";

var sourceFiles = GetFilesAndSubFiles(sourcePath);
Console.WriteLine($"{sourceFiles.Count} files found.");
Console.WriteLine($"Copying files...");

await CopyFilesAsync(sourceFiles, targetPath);
Console.WriteLine("End of the copy.");
Console.ReadKey();

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
    files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList());    
    return files;
}