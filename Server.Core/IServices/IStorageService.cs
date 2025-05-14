public interface IStorageService
{
    Task UploadFileAsync(string filePath, string objectName);
    Task<bool> DeleteFileAsync(string fileName);
    Task<Stream> DownloadFileAsync(string fileName);
    Task RenameFileAsync(string oldName, string newName);
    Task RenameFolderAsync(string oldPrefix, string newPrefix);
    Task ReplaceFileAsync(string fileName, Stream newFileStream);

}
