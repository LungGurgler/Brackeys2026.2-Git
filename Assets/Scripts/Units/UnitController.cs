using NUnit.Framework;
using System.Collections;
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

    private Dictionary<UnitType, int> TraitorsToSpawn = new() //stores the additional units to be spawned next round 
    {
        [UnitType.Farmer] = 0,
        [UnitType.Knight] = 0, 
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,

    };



    [SerializeField]
    private Transform[] enemySpawnPoints = new Transform[8];
    private Vector2[] originalEnemySpawnPos = new Vector2[8]; 

    public int playerUnitsDead { get; private set; } = 0;  
   

    [SerializeField]
    private Transform enemyParent;
    private Camera cam;

 
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

        cam = Camera.main;
        for(int i = 0; i < originalEnemySpawnPos.Length; i++)
        {
            originalEnemySpawnPos[i] = enemySpawnPoints[i].position;
        }
    }

    private void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetPlayerUnitPos();
        }

        if (activePlayerUnits.Count < 80)
        {
            StartCoroutine(scaleCamera(5f));
            setSpawnPositions(1);
        } 
        else if(activePlayerUnits.Count >= 80 && activePlayerUnits.Count < 180)
        {
            StartCoroutine(scaleCamera(10f));
            setSpawnPositions(2);

        } else if(activePlayerUnits.Count >= 180 && activePlayerUnits.Count < 250)
        {
            StartCoroutine(scaleCamera(15f));
            setSpawnPositions(3);
        } 
        else
        {
            StartCoroutine(scaleCamera(20f));
            setSpawnPositions(4);
        }
    }

    private void setSpawnPositions(float multiplier)
    {
        for(int i = 0; i < enemySpawnPoints.Length; i++)
        {
            enemySpawnPoints[i].position = originalEnemySpawnPos[i] * multiplier; 
        }
    }
    
    public void RemovePlayerUnit(Unit unit)
    {
        TraitorsToSpawn[unit.unitType]++;
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
        foreach (var item in TraitorsToSpawn)
        {
            GameObject spawnUnit = UnitLib.getUnit(item.Key);
            for(int i = 0; i < TraitorsToSpawn[item.Key]; i++)
            {
                Unit unit = Instantiate(spawnUnit, getRandomSpawnPoint(), Quaternion.identity, enemyParent).GetComponent<Unit>();
                unit.transform.tag = "EnemyUnit";
                unit.isAlly = false;
                unit.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                unit.SetTraitor();
                activeEnemyUnits.Add(unit);
            }

        }

        ClearTraitors();
    }

    public void DestroyAllAllies()
    {
        for(int i = 0; i < activePlayerUnits.Count; i++)
        {
            Destroy(activePlayerUnits[i].gameObject);
        
        }

        activePlayerUnits.Clear();
    }



    public void ResetPlayerUnitPos()
    {
       for(int i = 0; i < activePlayerUnits.Count; i++)
        {
            activePlayerUnits[i].PlacePlayerUnit(i);
        }
    }
    

    private void ClearTraitors()
    {
        //Holy hardcode
        TraitorsToSpawn[UnitType.Farmer] = 0;
        TraitorsToSpawn[UnitType.Knight] = 0;
        TraitorsToSpawn[UnitType.GoldKnight] = 0;
        TraitorsToSpawn[UnitType.Golem] = 0;
        TraitorsToSpawn[UnitType.Archer] = 0;
        TraitorsToSpawn[UnitType.Wizard] = 0;
    }
   
    
    public Vector2 getRandomSpawnPoint()
    {
       
        return (Vector2) enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position + (Random.insideUnitCircle * Random.Range(1,1.5f));
    }

    public int getAllyCount()
    {
        return activePlayerUnits.Count; 
    }


    private IEnumerator scaleCamera(float targetSize)
    {
        float oldSize = cam.orthographicSize; 
        while (cam.orthographicSize != targetSize)
        {

            cam.orthographicSize = Mathf.Lerp(oldSize,targetSize,Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        
    }
}
