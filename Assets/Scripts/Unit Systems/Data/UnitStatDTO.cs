using System;
using UnityEngine;

[Serializable]
public class UnitStatDTO
{
    public string ID;
    public string Name_KR;
    public string Name_EN;
    public string Race;
    public int Cost;
    public string Race_Synergy_A;
    public string Race_Synergy_B;
    public string Synergy_A;
    public string Synergy_B;
    public string Synergy_C;

    // 성급별 스탯
    public float MaxHp_1; public float AttackDamage_1;
    public float MaxHp_2; public float AttackDamage_2;
    public float MaxHp_3; public float AttackDamage_3;

    public float Defense;
    public float MagicResistance;
    public float AbilityPower;
    public float AttackSpeed;
    public int Attack_Rage;
    public float Move_Speed;
    public float MaxManaPoint;
    public float StartManaPoint;
    public float ManaRegeneration;

    public string NormalAttack_Type;
    public int Skill_ID;
}
