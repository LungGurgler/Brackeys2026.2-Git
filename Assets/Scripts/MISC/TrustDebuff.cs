using UnityEngine;

public enum Values
{
    MoveSpeed,
    AttackSpeed,
    Damage, 
    Health, 
    EnemyAttack
};

[CreateAssetMenu(fileName = "TrustDebuff", menuName = "Scriptable Objects/TrustDebuff")]
public class TrustDebuff : ScriptableObject
{



    public string debuffName; 
    public UnitType targetUnit;
    public Values targetValue; 
    public float value;
    public Sprite debuffSprite; 


}
