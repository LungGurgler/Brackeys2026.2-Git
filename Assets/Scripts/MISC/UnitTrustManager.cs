using System.Collections.Generic;
using UnityEngine;

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


    public void ApplyDebuff(TrustDebuff trustDebuff)
    {
        print("applied " + trustDebuff.debuffName);
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
