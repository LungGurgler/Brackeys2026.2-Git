using UnityEngine;

[CreateAssetMenu(fileName = "GarrisonUpgrades", menuName = "Scriptable Objects/GarrisonUpgrades")]
public class GarrisonUpgrades : ScriptableObject
{

    public UnitType unitToSpawn;
    public int spawnCount;
    public float value; 
    
}
