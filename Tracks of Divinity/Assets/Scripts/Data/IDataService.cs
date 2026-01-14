public interface IDataService
{
    bool CheckAndCreateDir(string path);
    bool SaveData<T>(string RelativePath, T Data, bool msg = true);
    T LoadData<T>(string RelativePath, bool warn = true);
}