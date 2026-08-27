using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance; 
   
    [SerializeField]
    private int currentWave = 1;
    [SerializeField]
    private bool waveActive = false; 


    private List<UnitType> unitsAvailable = new List<UnitType>(); 

    private Dictionary<UnitType, Vector2Int> unitSpawn = new()
    {
        [UnitType.Farmer] = new Vector2Int(1, 10),
        [UnitType.Archer] = new Vector2Int(2, Mathf.RoundToInt(Mathf.Infinity)),
        [UnitType.Knight] = new Vector2Int(3, Mathf.RoundToInt(Mathf.Infinity)),
        [UnitType.Golem] = new Vector2Int(4, Mathf.RoundToInt(Mathf.Infinity)),
        [UnitType.GoldKnight] = new Vector2Int(6, Mathf.RoundToInt(Mathf.Infinity)),
        [UnitType.Wizard] = new Vector2Int(7, Mathf.RoundToInt(Mathf.Infinity)),
      

    };

    private Dictionary<UnitType, int> unitsToSpawn = new()
    {
        [UnitType.Farmer] = 3,
        [UnitType.Archer] = 6,
        [UnitType.Knight] = 6,
        [UnitType.Golem] = 4,
        [UnitType.GoldKnight] = 5,
        [UnitType.Wizard] = 4,

    };

    //======================
    //  GARRISON MANAGEMENT
    //======================
    private readonly Dictionary<UnitType, int> unitGarrisonValue = new()
    {
        [UnitType.Farmer] = 1,
        [UnitType.Knight] = 2,
        [UnitType.GoldKnight] = 4,
        [UnitType.Golem] = 20,
        [UnitType.Archer] = 3,
        [UnitType.Wizard] = 8,
    };

    private UnitType[] garrisonUnits = new UnitType[2]; 

    private int garrisonValue { get { return (currentWave - 1) * 10; } }

    [SerializeField]
    private Transform GarrisonParent;

    [SerializeField]
    private Transform buttonA;
    [SerializeField]
    private Transform buttonB;

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
        if (!waveActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                startWave();
            }
        }
        if (waveActive)
        {
            if (UnitController.Instance.ActiveEnemyUnits.Count <= 0)
            {
                endWave();
            }
        }
    }

    public void startWave()
    {
        waveActive = true;
        CheckAvaialbleUnits();
        if (currentWave == 1)
        {
            UnitController.Instance.SpawnPlayerUnits(30, UnitType.Farmer);
        }
       
        
            for (int i = 0; i < 3 * currentWave; i++)
            {
                UnitType unit = unitsAvailable[Random.Range(0, unitsAvailable.Count)];
                UnitController.Instance.SpawnEnemies(unit, unitsToSpawn[unit]);
            }
        UnitController.Instance.SpawnTraitors();
        
    }

    private void CheckAvaialbleUnits()
    {
        foreach (var item in unitSpawn)
        {
            if (unitSpawn[item.Key].x <= currentWave && unitSpawn[item.Key].y >= currentWave)
            {
                unitsAvailable.Add(item.Key);
            }
            else
            {
                if (unitsAvailable.Contains(item.Key))
                {
                    unitsAvailable.Remove(item.Key);
                }
            }
        }
    }

    public void endWave()
    {
        waveActive = false;
        currentWave++;
        BeginGarrison();
        UnitController.Instance.ResetPlayerUnitPos();
    }

    private void BeginGarrison()
    {
        GarrisonParent.gameObject.SetActive(true); 
        List<UnitType> validUnits = new List<UnitType>();
        foreach (var item in unitGarrisonValue)
        {
            if (unitGarrisonValue[item.Key] <= garrisonValue)
            {
                validUnits.Add(item.Key);
            }
        }
        validUnits = shuffle(validUnits); 

        garrisonUnits[0] = validUnits[0];
        garrisonUnits[1] = validUnits[1];
        setGarrisonButtons();
    }

    private void setGarrisonButtons()
    {
        buttonA.GetChild(1).GetComponent<Image>().sprite = SpriteLib.getUnitSprite(garrisonUnits[0]);
        buttonB.GetChild(1).GetComponent<Image>().sprite = SpriteLib.getUnitSprite(garrisonUnits[1]);
        buttonA.GetComponentInChildren<TextMeshProUGUI>().text = "HIRE " + Mathf.FloorToInt(garrisonValue / unitGarrisonValue[garrisonUnits[0]]) + " " + garrisonUnits[0].ToString() + "s";
        buttonB.GetComponentInChildren<TextMeshProUGUI>().text = "HIRE " + Mathf.FloorToInt(garrisonValue / unitGarrisonValue[garrisonUnits[1]]) + " " + garrisonUnits[1].ToString() +"s";

    }

    public void SelectGarrison(int num)
    {
       
        UnitController.Instance.SpawnPlayerUnits(Mathf.FloorToInt(garrisonValue / unitGarrisonValue[garrisonUnits[num]]), garrisonUnits[num]);
        //Insert debuffs here!
        UnitTrustManager.Instance.DisplayDebuffs();
    }


    //     List<int> list = Enumerable.Range(0, validUnits.Count).OrderBy(x => rand.Next()).Take(2).ToList();

    public List<UnitType> shuffle(List<UnitType> validUnits)
    {
        for(int i = validUnits.Count - 1; i >= 0; i--)
        {
            int j = Random.Range(0, i);
            UnitType temp = validUnits[i];
            validUnits[i] = validUnits[j];
            validUnits[j] = temp; 
        }

        return validUnits; 
    }


}
