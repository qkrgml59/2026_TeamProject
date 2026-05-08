using Prototype.Grid;
using System.Collections;
using System.Collections.Generic;
using Unit.Skill;
using UnityEngine;
using UnityEngine.UI;
using StatSystem;

namespace Unit.Skill
{
    public class PlagueDoctor_Skill : SkillBase
    {
        [Header("스킬 수치")]
        [SerializeField] private float baseDamage = 300;    // 기본 쉴드량
        [SerializeField] private float Value_A = 5f;   // 지속시간
        [SerializeField] private float Value_B = 0.2f; // 20% 감소

        [Header("스킬 효과")]
        [SerializeField] private ParticleSystem shieldParticle;
        private ParticleSystem _shield;

        [SerializeField] private ParticleSystem fogParticle;
        private ParticleSystem[] _fogs;

        private Coroutine skillRoutine;

        // 디버프용 타일 저장
        List<HexTile> tiles = new List<HexTile>();

        public override void Init(UnitBase _owner)
        {
            base.Init(_owner);

            // 연기 파티클 사전 생성
            if (fogParticle != null)
            {
                _fogs = new ParticleSystem[6];
                for (int i = 0; i < 6; i++)
                {
                    _fogs[i] = Instantiate(fogParticle, Vector3.zero, Quaternion.identity);
                    _fogs[i].gameObject.SetActive(false);
                }
            }
        }

        protected override void OnStart()
        {
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            //1.공격 제한
            owner.SetTargetUnit(null);
            //공격 불가 TODO : 나중에... 상태 넣어야 함

            ShieldEntry shield = new ShieldEntry(this, baseDamage, Value_A);
            owner.ApplyShield(shield);

            // 보호막 효과 생성
            ShieldSetActive(true);
            owner.shield.OnShieldRemoved += ShieldRemoved; 

            yield return null;


            // FIX : 중심 타일은 디버프가 필요 없어 보여서 제외

            //2. 중심 타일 구하기 
            Vector3Int centerCube = HexMath.OffsetToCube(owner.offset);
            //HexTile centerTile = GridManager.Instance.GetTile(centerCube);


            //3. 주변 타일 하나씩 직접 가져오기
            tiles.Clear();

            //if (centerTile != null)
            //    tiles.Add(centerTile);
            //주변 6칸
            foreach (var dir in HexMath.CubeDirections)
            {
                HexTile tile = GridManager.Instance.GetTile(centerCube + dir);
                if (tile != null)
                    tiles.Add(tile);
            }


            //4. 디버프 적용
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;

                // TODO: 실제 디버프 효과 넣어야 함

                // 연기 효과 생성
                SetFogOnTile(i, tiles[i]);
                Debug.Log($"타일 {tiles[i].name} 영향 받음");
            }

            //5. 지속 시간
            yield return new WaitForSeconds(Value_A);

            //6. 종료
            Debug.Log("역병의사 스킬 종료");


            FinishSkill();
        }

        protected override void OnCancel()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }

            owner.shield.OnShieldRemoved -= ShieldRemoved;
            ShieldSetActive(false);
            FogsSetActive(false);
        }

        protected override void OnFinish()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }

            owner.shield.OnShieldRemoved -= ShieldRemoved;
            ShieldSetActive(false);
            FogsSetActive(false);
        }

        void ShieldRemoved(Object source)
        {
            if (source != this) return;

            ShieldSetActive(false);
        }

        void ShieldSetActive(bool active)
        {
            if (active)
            {
                if (_shield != null)
                    _shield.gameObject.SetActive(true);
                else if (shieldParticle != null)
                    _shield = Instantiate(shieldParticle, owner.transform);
            }
            else
            {
                if (_shield != null) _shield.gameObject.SetActive(false);
            }
        }

        void FogsSetActive(bool active)
        {
            for (int i = 0; i < _fogs.Length; i++)
            {
                if (_fogs[i] != null) _fogs[i].gameObject.SetActive(active);
            }
        }

        void SetFogOnTile(int index, HexTile tile)
        {
            if (index >= _fogs.Length) return;

            _fogs[index].transform.position = tile.transform.position;
            _fogs[index].gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            // 연기 효과는 유닛의 자식이 아니므로 따로 제거
            for (int i = 0; i < _fogs.Length; i++)
            {
                if (_fogs[i] != null) Destroy(_fogs[i]);
            }
        }

        public override string GetDescription(StatSet statSet)
        {
            return $"고정 상태가 되며 보호막을 {baseDamage} 얻습니다. 그후 {Value_A}초 동안 주변 칸에 가스를 살포합니다." +
                $"\n가스는 내부에 있는 적을 중독 시켜 방어력과 마법저항력을 {(Value_B * 100).ToString("F0")}% 감소시킵니다." +
                $"\n" +
                $"\n<color=#575757>고정 - 기본 공격과 이동이 불가능합니다.</color>";
        }
    }
}

