using UnityEngine;

public abstract class Loader<T> : MonoBehaviour where T : class, new()
{
    public static T obj;
    
    protected abstract T PersisterObj { get; set; }
    private static readonly string savePath = $"/Save Data/{typeof(T).Name}.json";
    private static IDataService dataService;

    protected abstract void CustomNew();

    private void Awake()
    {
        dataService ??= new JsonDataService();

        if(PersisterObj != null) //Persister carries obj after initial main menu load
        {
            obj = PersisterObj;
            return;
        }

        dataService.CheckAndCreateDir("/Save Data");
        obj = new();
        try{ obj = dataService.LoadData<T>(Application.persistentDataPath + savePath); }
        catch(System.Exception){ CustomNew(); }
    }

    public static void SaveObj() => dataService.SaveData(savePath, obj);
    
    private void OnDestroy()
    {
        PersisterObj = obj;
        SaveObj();
    }
}
