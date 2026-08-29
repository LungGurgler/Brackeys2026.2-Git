using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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



    [SerializeField]
    private TextMeshProUGUI waveText;
    [SerializeField]
    private TextMeshProUGUI enemiesRemainingText;


    [SerializeField]
    private Image deathScreenBG;
    [SerializeField]
    private TextMeshProUGUI dsHeader;
    [SerializeField]
    private TextMeshProUGUI dsBodyText;


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

    private void Start()
    {
        startWave();
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
            enemiesRemainingText.text = UnitController.Instance.ActiveEnemyUnits.Count.ToString();
            if (UnitController.Instance.ActiveEnemyUnits.Count <= 0)
            {

                endWave();
            }
        }
    }

    public void startWave()
    {
        waveActive = true;
        waveText.text = "Wave " + (currentWave - 1);
        if (currentWave == 1)
        {
            UnitController.Instance.SpawnPlayerUnits(5, UnitType.Farmer);
        }
        if (currentWave - 1 == 1)
        {
            UnitController.Instance.SpawnEnemies(UnitType.Farmer, 5);
        }
        else
        {
            int availablePoints = Mathf.FloorToInt(garrisonValue * Mathf.Pow(1.01f, currentWave - 1));
            while (availablePoints > 0)
            {
                List<UnitType> validUnits = new List<UnitType>();

                foreach (var item in unitGarrisonValue)
                {
                    if (item.Key == UnitType.Farmer && currentWave >= 10)
                    {
                        continue;
                    }

                    if(item.Key == UnitType.Golem)
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
                UnitController.Instance.SpawnEnemies(unit, 1);
                availablePoints -= unitGarrisonValue[unit];
            }
        }
        UnitController.Instance.SpawnTraitors();

    }


    public void endWave()
    {
        waveActive = false;
        currentWave++;
        SoundManager.Instance.PlaySFX(SFXKeys.WaveOver, 1.0f);
        BeginGarrison();
        UnitController.Instance.ResetPlayerUnitPos();
    }


    public void endGame()
    {
        SoundManager.Instance.PlaySFX(SFXKeys.LoseGame);
        deathScreenBG.transform.parent.gameObject.SetActive(true);
        waveActive = false;
        Time.timeScale = 0f;
        StartCoroutine(startDeathScreen());
        UnitController.Instance.DestroyAllAllies();

    }

    private IEnumerator startDeathScreen()
    {
        Color32 targetColour = deathScreenBG.color;
        Color32 startColour = deathScreenBG.color;
        startColour.a = 0;
        deathScreenBG.color = startColour;
        targetColour.a = 155;
        float elapsedTime = 0f;
        while (elapsedTime < 2f)
        {

            elapsedTime += Time.unscaledDeltaTime;
            deathScreenBG.color = Color32.Lerp(startColour, targetColour, elapsedTime / 2f);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(startTextDisplay());
    }

    private IEnumerator startTextDisplay()
    {
        Color32 targetColour = dsHeader.color;
        Color32 startColour = dsHeader.color;
        startColour.a = 0;
        dsHeader.color = startColour;
        targetColour.a = 255;
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {

            elapsedTime += Time.unscaledDeltaTime;
            dsHeader.color = Color32.Lerp(startColour,targetColour, elapsedTime / 0.5f);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(StartBodyDisplay());
    }

    private IEnumerator StartBodyDisplay()
    {
        Color targetColour = dsBodyText.color;
        Color startColour = dsBodyText.color;
        startColour.a = 0f;
        targetColour.a = 1f;
        float elpasedTime = 0f;
        while (elpasedTime < 0.25f)
        {
            elpasedTime += Time.unscaledDeltaTime;
            dsBodyText.color = Color.Lerp(startColour, targetColour, elpasedTime / 0.25f);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(StartDelay(1.5f));

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

   private IEnumerator StartDelay(float duration)
    {
        print("waiting!");
        yield return new WaitForSecondsRealtime(duration);
        StartCoroutine(loadScene(0));
    }

    private IEnumerator loadScene(int index)
    {

        Time.timeScale = 1f;
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(index);
        while (!loadScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }



    }

}
