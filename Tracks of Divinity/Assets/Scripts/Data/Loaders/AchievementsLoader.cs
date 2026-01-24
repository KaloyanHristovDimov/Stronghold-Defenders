using System.Collections.Generic;
using UnityEngine;

public class AchievementsLoader : Loader<Achievements>
{
    [SerializeField] List<Achievement> achievementsSource = new();

    private static AchievementsLoader inst;
    public static List<Achievement> Source => inst.achievementsSource;

    protected override Achievements PersisterObj{
        get => Persister.achievementsObj;
        set => Persister.achievementsObj = value;
    }

    protected override void Awake()
    {
        inst = this;
        base.Awake();
    }

    protected override void CustomNew()
    {
        obj = new();
        obj.InitialSet();
    }
}
