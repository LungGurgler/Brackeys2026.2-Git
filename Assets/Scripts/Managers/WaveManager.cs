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

    private Dictionary<UnitType, int> unitMinWave = new()
    {
        [UnitType.Farmer] = 1,
        [UnitType.Archer] = 2,
        [UnitType.Knight] = 3,
        [UnitType.Golem] = 4,
        [UnitType.GoldKnight] = 5,
        [UnitType.Wizard] = 6,
      

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
        print(currentWave); 
        if (currentWave == 1)
        {
            UnitController.Instance.SpawnPlayerUnits(5, UnitType.Farmer);
        }

        int availablePoints = Mathf.FloorToInt(garrisonValue * Mathf.Pow(1.01f,currentWave - 1));
        while (availablePoints > 0)
        {
            List<UnitType> validUnits = new List<UnitType>();

            foreach (var item in unitGarrisonValue)
            {
                if(item.Key == UnitType.Farmer && currentWave >= 10) 
                {
                    continue; 
                }

                if (unitMinWave[item.Key] <= currentWave - 1)
                {
                    if (unitGarrisonValue[item.Key] <= availablePoints)
                    {
                        validUnits.Add(item.Key);
                    }
                }
            }

            UnitType unit = validUnits[Random.Range(0, validUnits.Count)];
            UnitController.Instance.SpawnEnemies(unit,1);
            availablePoints -= unitGarrisonValue[unit]; 
        }
            
        UnitController.Instance.SpawnTraitors();
        
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
