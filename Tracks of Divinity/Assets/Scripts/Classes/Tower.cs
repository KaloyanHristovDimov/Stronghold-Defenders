using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjects/TowerData", order = 2)]
public class Tower : ScriptableObject, ICardInfo
{
    public bool damageType;
    public int damage, range, price, aoeRange;
    public float speed;
    public TowerType towerType;
    public Biome biome;
    public GameObject prefab;

    public Tower(TowerType towerType, int price, Biome biome)
    {
        this.towerType = towerType;
        this.price = price;
        this.biome = biome;
    }

    public Tower(bool damageType, int damage, int range, float speed, int price, TowerType towerType, Biome biome, int aoeRange = 0)
    {
        this.damageType = damageType;
        this.damage = damage;
        this.range = range;
        this.speed = speed;
        this.price = price;
        this.towerType = towerType;
        this.biome = biome;
        this.aoeRange = aoeRange;
    }
}
