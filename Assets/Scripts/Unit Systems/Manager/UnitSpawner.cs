using UnityEngine;
using Utilitys;
using Prototype.Grid;

namespace Unit
{

    public class UnitSpawner : SingletonMonoBehaviour<UnitSpawner>
    {
        [Header("유닛 생성기 설정")]
        public UnitBase unitPrefab;
        
        /// <summary>
        /// 유닛을 그리드에 생성 시킵니다.
        /// </summary>
        public UnitBase SpawnUnit(UnitDataSO unitData, HexTile targetTile, TeamType team, int star = 0)
        {
            if(unitPrefab == null) return null;

            UnitBase target = Instantiate(unitPrefab, targetTile.transform.position, Quaternion.identity);
            target.Init(unitData, team, star);
            target.PlaceUnit(targetTile);
            UnitManager.Instance.RegisterUnit(target);
            UnitUIManager.Instance.Create(target);
            target.RecalculateUnitStats();

            return target;
        }

        // TODO : 임시 유닛 (라운드 동안만 유지되는 유닛) 배치 기능 추가


        // TODO : 카드 쪽에서 합성 가능 여부 확인하는 기능 추가
        // public bool CanCombine(Unit unit)
        // {
            
        // }


        /// <summary>
        /// 유닛 합성 시도
        /// </summary>
        public bool TryCombine(UnitBase unit)           // 메인 유닛 + 재료 유닛 2개 방식으로 변경
        {
            UnitManager.Instance.GetSameUnits(unit);
            
            // TODO : 합성 추가중
            return true;
        }
    }   
}
