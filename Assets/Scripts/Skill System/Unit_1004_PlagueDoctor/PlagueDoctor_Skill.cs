using Prototype.Grid;
using System.Collections;
using System.Collections.Generic;
using Unit.Skill;
using UnityEngine;

public class PlagueDoctor_Skill : SkillBase
{
    [Header("스킬 수치")]
    [SerializeField] private float Value_A = 5f;
    [SerializeField] private float Value_B = 0.2f;
    [SerializeField] private float Value_C = 50f;         //보호막

    [Header("딜레이")]
    [SerializeField] private float tickDelay = 1f;

    [Header("이펙트")]
    public ParticleSystem SkillVFX;
    private List<ParticleSystem> skillEffects = new();

    private List<HexTile> gasTiles = new();

    private Coroutine skillRoutine;


    private void Awake()
    {
        // 최대 7칸 미리 생성
        if(skillEffects !=null)
        {
            for (int i = 0; i< 7; i++)
            {
                ParticleSystem vfx = Instantiate(SkillVFX, Vector3.zero, Quaternion.identity, transform);
                vfx.Stop();
                skillEffects.Add(vfx);
            }
        }
    }

    protected override void OnStart()
    {
        Debug.Log("[역병의사] 스킬 시작");

        owner.AddShield(Value_C);
        Debug.Log($"[역병의사] 보호막 획득 {Value_C}");
        CreateGasArea();

        skillRoutine = StartCoroutine(SkillCast());
    }

     IEnumerator SkillCast()
     {
        //1. 이펙트 배치 및 실행
        for (int i = 0; i< gasTiles.Count; i++)
        {
            if (i >= skillEffects.Count || gasTiles[i] == null) continue;

            skillEffects[i].transform.position = gasTiles[i].transform.position + Vector3.up * 0.2f;

            skillEffects[i].Play();
        }

        float timer = 0f;
        Debug.Log("[역병의사] 가스 활성화");

        //2.지속시간 동안 반복
        while (timer < Value_A)
        {
            ApplyDebuff();

            yield return new WaitForSeconds(tickDelay);
            timer += tickDelay;
        }

        Debug.Log("[역병의사] 가스 종료");

        FinishSkill();
     }

    void CreateGasArea()
    {
        gasTiles.Clear();

        Vector3Int centerOffset = owner.offset;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
