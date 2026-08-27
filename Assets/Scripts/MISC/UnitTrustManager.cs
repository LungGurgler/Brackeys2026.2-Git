using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTrustManager : MonoBehaviour
{
    public static UnitTrustManager Instance; 


   
    private Dictionary<UnitType, float> playerUnitMoveSpeedMult = new() //
    {
        [UnitType.Farmer] = 1,
        [UnitType.Knight] = 1,
        [UnitType.GoldKnight] = 1,
        [UnitType.Archer] = 1,
        [UnitType.Wizard] = 1,
        [UnitType.Golem] = 1,
    };
    private Dictionary<UnitType, float> playerUnitAttackSpeedMult = new() //
    {
        [UnitType.Farmer] = 1,
        [UnitType.Knight] = 1,
        [UnitType.GoldKnight] = 1,
        [UnitType.Archer] = 1,
        [UnitType.Wizard] = 1,
        [UnitType.Golem] = 1,
    };

    private Dictionary<UnitType, float> playerUnitDamageDebuff = new() //
    {
        [UnitType.Farmer] = 0,
        [UnitType.Knight] = 0,
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,
    };

    private Dictionary<UnitType, float> playerUnitHealthDebuff = new() //
    {
        [UnitType.Farmer] = 0,
        [UnitType.Knight] = 0,
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,
    };

    private Dictionary<UnitType, float> EnemyUnitDamageBuff = new() //
    {
        [UnitType.Farmer] = 0,
        [UnitType.Knight] = 0,
        [UnitType.GoldKnight] = 0,
        [UnitType.Archer] = 0,
        [UnitType.Wizard] = 0,
        [UnitType.Golem] = 0,
    };


    [SerializeField]
    private List<TrustDebuff> debuffs = new List<TrustDebuff>();


    [SerializeField]
    private Transform TrustParent; 

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            ApplyDebuff(debuffs[0]);
        }  
    }


    public void DisplayDebuffs()
    {
        TrustParent.gameObject.SetActive(true); 
        shuffleDebuffs();
        buttonA.GetComponent<Image>().sprite = debuffs[0].debuffSprite;
        buttonB.GetComponent<Image>().sprite = debuffs[1].debuffSprite;

        buttonA.GetChild(0).GetComponent<TextMeshProUGUI>().text = debuffs[0].name;
        buttonB.GetChild(0).GetComponent<TextMeshProUGUI>().text = debuffs[1].name;

        buttonA.GetChild(1).GetComponent<TextMeshProUGUI>().text = debuffs[0].description;
        buttonB.GetChild(1).GetComponent<TextMeshProUGUI>().text = debuffs[1].description;
    }

    public void ApplyDebuffButton(int num)
    {
        ApplyDebuff(debuffs[num]);
        TrustParent.gameObject.SetActive(false);
        WaveManager.Instance.startWave();
    } 

    private void ApplyDebuff(TrustDebuff trustDebuff)
    {
        if (trustDebuff.targetUnit == UnitType.ALL)
        {
            ApplyToAllUnits(trustDebuff); 
        }
        else
        {  
            switch (trustDebuff.targetValue)
            {
                case Values.MoveSpeed:
                        playerUnitAttackSpeedMult[trustDebuff.targetUnit] *= (1 - trustDebuff.value / 100);
                    break;

                case Values.AttackSpeed:
                        playerUnitAttackSpeedMult[trustDebuff.targetUnit] *= (1 + trustDebuff.value / 100);
                    break;

                case Values.Damage:
                        playerUnitDamageDebuff[trustDebuff.targetUnit] += trustDebuff.value;
                    break;
                case Values.Health:
                        playerUnitHealthDebuff[trustDebuff.targetUnit] += trustDebuff.value;
                    break;

                case Values.EnemyAttack:
                    EnemyUnitDamageBuff[trustDebuff.targetUnit] += trustDebuff.value;
                    break; 
            }
        }
    }

    private void ApplyToAllUnits(TrustDebuff trustDebuff)
    {
        switch (trustDebuff.targetValue)
        {
            case Values.Health:
                foreach (var item in playerUnitHealthDebuff)
                {
                    playerUnitHealthDebuff[item.Key] += trustDebuff.value; 
                }
                break;
            case Values.MoveSpeed:
                foreach (var item in playerUnitMoveSpeedMult)
                {
                    playerUnitMoveSpeedMult[item.Key] *= 1 - trustDebuff.value / 100;
                }
                break;

        }
    }
    private void shuffleDebuffs()
    {
        for (int i = debuffs.Count - 1; i >= 0; i--)
        {
            int j = Random.Range(0, i);
            TrustDebuff temp = debuffs[i];
            debuffs[i] = debuffs[j];
            debuffs[j] = temp;
        }

   
    }

    public float GetPlayerMoveSpeed(UnitType unitType)
    {
        return playerUnitMoveSpeedMult[unitType];
    }

    public float GetPlayerAttackSpeed(UnitType unitType)
    {
        return playerUnitAttackSpeedMult[unitType];
    }

    public float GetPlayerDamage(UnitType unitType)
    {
        return playerUnitDamageDebuff[unitType];
    }
    
    public float GetPlayerHealth(UnitType unitType)
    {
        return playerUnitHealthDebuff[unitType];
    }

    public float GetEnemyUnitDamage(UnitType unitType)
    {
        return EnemyUnitDamageBuff[unitType];
    }
}
