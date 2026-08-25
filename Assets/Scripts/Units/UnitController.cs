using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{


    public static UnitController Instance; 


    private List<Unit> activePlayerUnits = new List<Unit>();
    private List<Unit> activeEnemyUnits = new List<Unit>();
    private List<Unit> UnitsToSpawnNext = new List<Unit>();

    private Dictionary<UnitType, int> UnitsToSpawn = new() //stores the additional units to be spawned next round 
    {
        [UnitType.Farmer] = 0,
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,
    };

    [SerializeField]
    private GameObject[] unitObjects;

    [SerializeField]
    private Transform[] enemySpawnPoints; 



   

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
            GameObject unit = UnitLib.getUnit(UnitType.Farmer);
            spawnPlayerUnit(unit);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnPlayerUnits(5,UnitType.Farmer);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnEnemies(3);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetPlayerUnitPos();
        }
        
       
    }

    public void RemovePlayerUnit(Unit unit)
    {
        UnitsToSpawn[unit.unitType]++;
        UnitsToSpawn[unit.unitType] = (int) Mathf.Clamp(UnitsToSpawn[unit.unitType], 0, Mathf.Infinity);
        activePlayerUnits.Remove(unit); 
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


    public void AddEnemyUnit(Unit unit)
    {
        activeEnemyUnits.Remove(unit);
        if (activeEnemyUnits.Count <= 0)
        { 
            //end wave here!
        }
    }


    public void SpawnEnemies(int count)
    {
        int num = Random.Range(0, unitObjects.Length);

        for(int i = 0; i < count; i++)
        {

            Unit unit = Instantiate(unitObjects[num], getRandomSpawnPoint(), Quaternion.identity, enemyParent).GetComponent<Unit>();
            unit.transform.tag = "EnemyUnit";
            unit.isAlly = false;
            activeEnemyUnits.Add(unit);
        }
    }

    private void ResetPlayerUnitPos()
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
