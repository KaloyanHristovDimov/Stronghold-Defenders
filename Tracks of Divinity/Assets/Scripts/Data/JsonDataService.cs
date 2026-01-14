using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonDataService : IDataService
{
    public bool CheckAndCreateDir(string path)
    {
        path = Application.persistentDataPath + path;
        bool exist = Directory.Exists(path);

        if(!exist) Directory.CreateDirectory(path);
        return exist;
    }

    public bool SaveData<T>(string RelativePath, T Data, bool msg = true)
    {
        string path = Application.persistentDataPath + RelativePath;
        try
        {
            if(File.Exists(path)) File.Delete(path);
            else if(msg) LogUtil.WriteFirstTime(path);
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch(System.Exception e)
        {
            LogUtil.FailToSave(e);
            return false;
        }
    }

    public T LoadData<T>(string path, bool warn = true)
    {
        if(!File.Exists(path))
        {
            if(warn) LogUtil.FailToLoadNonexistent(path);
            throw new FileNotFoundException($"{path} does not exist");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch(System.Exception e)
        {
            if(warn) LogUtil.FailToLoad(e);
            throw e;
        }
    }
}
