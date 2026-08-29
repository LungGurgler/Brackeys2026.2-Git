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
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("PlayerUnitAttackSpeedMult");
            foreach (var item in playerUnitAttackSpeedMult)
            {
                print(item);
            }
            print("playerUnitDamageDebuff");
            foreach (var item in playerUnitDamageDebuff)
            {
                print(item);
            }
            print("playerUnitHealthDebuff");
            foreach (var item in playerUnitHealthDebuff)
            {
                print(item);
            }
            print("playerUnitMoveSpeedMult");
            foreach (var item in playerUnitMoveSpeedMult)
            {
                print(item);
            }
            print("EnemyUnitAttackDamage");
            foreach (var item in EnemyUnitDamageBuff)
            {
                print(item);
            }


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
        // TrustParent.gameObject.SetActive(false);
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
                
                playerUnitHealthDebuff[UnitType.Farmer] += trustDebuff.value;
                playerUnitHealthDebuff[UnitType.Knight] += trustDebuff.value;
                playerUnitHealthDebuff[UnitType.GoldKnight] += trustDebuff.value;
                playerUnitHealthDebuff[UnitType.Archer] += trustDebuff.value;
                playerUnitHealthDebuff[UnitType.Wizard] += trustDebuff.value;
                playerUnitHealthDebuff[UnitType.Golem] += trustDebuff.value;
                
                break;
            case Values.MoveSpeed:
                    playerUnitMoveSpeedMult[UnitType.Farmer] *= 1 - trustDebuff.value / 100;
                    playerUnitMoveSpeedMult[UnitType.Knight] *= 1 - trustDebuff.value / 100;
                    playerUnitMoveSpeedMult[UnitType.GoldKnight] *= 1 - trustDebuff.value / 100;
                    playerUnitMoveSpeedMult[UnitType.Golem] *= 1 - trustDebuff.value / 100;
                    playerUnitMoveSpeedMult[UnitType.Archer] *= 1 - trustDebuff.value / 100;
                    playerUnitMoveSpeedMult[UnitType.Wizard] *= 1 - trustDebuff.value / 100;
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
