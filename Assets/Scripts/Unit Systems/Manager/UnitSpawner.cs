using UnityEngine;
using Utilitys;
using Prototype.Grid;
using System.Collections.Generic;
using Item;

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

            // 비전투 중인 아군의 경우 합성 확인
            if (BattleManager.currentBattleState == BattleState.Prepare && team == TeamType.Ally)
            {
                TryCombine(target);
            }

            return target;
        }

        // TODO : 임시 유닛 (라운드 동안만 유지되는 유닛) 배치 기능 추가



        int _mergeCount = 3;            // 합성에 필요한 유닛 개수
        int _maxStar = 2;               // 최대 성급 확인

        public bool CanCombine(UnitBase unit, out List<UnitBase> materials)
        {
            materials = null;
            if (unit.star >= _maxStar) return false;     // 최대성급이면 합성 불가

            materials = UnitManager.Instance.GetSameUnits(unit);
            if (materials.Count < _mergeCount) return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanCombine(string ID, int star, TeamType team, out List<UnitBase> materials)
        {
            materials = UnitManager.Instance.GetSameUnits(ID, star, team);
            if (materials.Count < _mergeCount) return false;

            return true;
        }


        /// <summary>
        /// 유닛 합성 시도
        /// </summary>
        public bool TryCombine(UnitBase unit)           // 메인 유닛 + 재료 유닛 2개 방식으로 변경
        {
            if(!CanCombine(unit, out List<UnitBase> materials))
                return false;

            // 아이템 (+ 위치) 기준으로 정렬
            materials.Sort((a, b) =>
            {
                // 1. 아이템 개수 (많은 게 앞으로)
                int itemCompare = b.EquippedItems.Count.CompareTo(a.EquippedItems.Count);
                if (itemCompare != 0)
                    return itemCompare;

                // 2. x는 0에 가까울수록 우선
                int xCompare = Mathf.Abs(a.offset.x).CompareTo(Mathf.Abs(b.offset.x));
                if (xCompare != 0)
                    return xCompare;

                // 3. y는 클수록 우선
                return b.offset.y.CompareTo(a.offset.y);
            });

            
            // 베이스 유닛 설정 (가장 앞 유닛)
            if(materials.Count == 0) return false;
            if (materials.Count > _mergeCount) materials.RemoveRange(_mergeCount, materials.Count - _mergeCount);       // 합성에 필요한 개수 만큼만 남김

            UnitBase baseUnit = materials[0];
            materials.RemoveAt(0);


            // 랜덤 아이템 장착
            TryTransferRandomItems(baseUnit, materials);

            // 유닛 정리
            UpgradeUnitGrade(baseUnit);         // 유닛 성급 증가
            RemoveMaterialUnits(materials);     // 재료 유닛 삭제

            // 합성 완료 된 유닛을 기준으로 합성 여부 재확인
            TryCombine(baseUnit);               

            return true;
        }

        bool TryTransferRandomItems(UnitBase baseUnit, List<UnitBase> materials)
        {
            if (baseUnit.EquippedItems.Count >= 3)
                return false;

            List<ItemBase> items = new List<ItemBase>();

            for (int i = 0; i < materials.Count; i++)
            {
                items.AddRange(materials[i].EquippedItems);
                materials[i].UnequipAllItems();     // 재료 유닛의 모든 아이템 제거
            }

            // 전체 아이템 목록중에서 랜덤으로 장착
            while(items.Count > 0)
            {
                int index = Random.Range(0, items.Count - 1);
                ItemBase item = items[index];
                baseUnit.TryEquippedItem(item);
                items.RemoveAt(index);
            }

            return true;
        }

        void UpgradeUnitGrade(UnitBase baseUnit)
        {
            baseUnit.UpgradeUnitGrade();
        }

        void RemoveMaterialUnits(List<UnitBase> materials)
        {
            foreach(var u in materials)
            {
                u.Die();
            }

            materials.Clear();
        }
    }   
}
