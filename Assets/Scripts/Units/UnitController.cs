using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{


    public static UnitController Instance; 


    private List<Unit> activePlayerUnits = new List<Unit>();
    private List<Unit> activeEnemyUnits = new List<Unit>();
    public List<Unit> ActiveEnemyUnits { get { return activeEnemyUnits; } }
    private List<Unit> UnitsToSpawnNext = new List<Unit>();

    private Dictionary<UnitType, int> UnitsToSpawn = new() //stores the additional units to be spawned next round 
    {
        [UnitType.Farmer] = 0,
        [UnitType.Knight] = 0, 
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,

    };



    [SerializeField]
    private Transform[] enemySpawnPoints;


    public int playerUnitsDead { get; private set; } = 0;  
   

    [SerializeField]
    private Transform enemyParent;

 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject unit = UnitLib.getUnit(UnitType.Archer);
            spawnPlayerUnit(unit);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnPlayerUnits(5,UnitType.Archer);
        }
        

        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetPlayerUnitPos();
        }


    }


    
    public void RemovePlayerUnit(Unit unit)
    {
        UnitsToSpawn[unit.unitType]++;
        playerUnitsDead++; 
        activePlayerUnits.Remove(unit); 
    }

    public void RemoveEnemyUnit(Unit unit)
    {
        activeEnemyUnits.Remove(unit);
    }
    
    public void SpawnPlayerUnits(int count, UnitType unitType)
    {
        GameObject spawnUnit = UnitLib.getUnit(unitType); 
        for (int i = 0; i < count; i++)
        {
            spawnPlayerUnit(spawnUnit); 
        }
    }

    public void spawnPlayerUnit(GameObject spawnUnit)
    {
       
        Unit unit = Instantiate(spawnUnit,KingController.Instance.transform).GetComponent<Unit>();
        unit.transform.tag = "PlayerUnit"; 
        unit.isAlly = true;
        unit.PlacePlayerUnit(activePlayerUnits.Count);
        activePlayerUnits.Add(unit);
    }


  


    public void SpawnEnemies(UnitType unitType, float count)
    {
        GameObject spawnUnit = UnitLib.getUnit(unitType); 
        
        for(int i = 0; i < count; i++)
        {

            Unit unit = Instantiate(spawnUnit, getRandomSpawnPoint(), Quaternion.identity, enemyParent).GetComponent<Unit>();
            unit.transform.tag = "EnemyUnit";
            unit.isAlly = false;
            unit.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            activeEnemyUnits.Add(unit);
        }
    }

    public void SpawnTraitors()
    {
        foreach (var item in UnitsToSpawn)
        {
            GameObject spawnUnit = UnitLib.getUnit(item.Key);
            for(int i = 0; i < UnitsToSpawn[item.Key]; i++)
            {
                Unit unit = Instantiate(spawnUnit, getRandomSpawnPoint(), Quaternion.identity, enemyParent).GetComponent<Unit>();
                unit.transform.tag = "EnemyUnit";
                unit.isAlly = false;
                unit.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                unit.SetTraitor();
                activeEnemyUnits.Add(unit);
            }

        }

    }

    public void ResetPlayerUnitPos()
    {
       for(int i = 0; i < activePlayerUnits.Count; i++)
        {
            activePlayerUnits[i].PlacePlayerUnit(i);
        }
    }
    
   
    
    public Vector2 getRandomSpawnPoint()
    {
       
        return (Vector2) enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position + (Random.insideUnitCircle * Random.Range(1,1.5f));
    }

    public int getAllyCount()
    {
        return activePlayerUnits.Count; 
    }
}
