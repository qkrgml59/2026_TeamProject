using UnityEngine;
using Utilitys;
using System.Collections.Generic;
using Prototype.Grid;
using Unity.VisualScripting;

namespace Unit
{
    public class UnitManager : SingletonMonoBehaviour<UnitManager>
    {
        private readonly List<UnitBase> _allUnits = new();          // 전체 유닛 리스트
        private readonly List<UnitBase> _allyUnits = new();         // 아군 유닛 리스트
        private readonly List<UnitBase> _enemyUnits = new();        // 적군 유닛 리스트

        /// <summary>
        /// 배틀이 종료 되었는지 (아군 또는 적군이 없는지 확인)
        /// </summary>
        public bool IsBattleEnd() => _allyUnits.Count == 0 || _enemyUnits.Count == 0;

        public void RegisterUnit(UnitBase unit)
        {
            if (unit == null || _allUnits.Contains(unit))
                return;

            _allUnits.Add(unit);

            if (unit.team == TeamType.Ally)
                _allyUnits.Add(unit);
            else if (unit.team == TeamType.Enemy)
                _enemyUnits.Add(unit);

            Debug.Log($"{unit.name} 유닛 {unit.team}팀에 저장");
        }

        public void UnregisterUnit(UnitBase unit)
        {
            if (unit == null)
                return;

            _allUnits.Remove(unit);
            _allyUnits.Remove(unit);
            _enemyUnits.Remove(unit);


            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.CheckBattleEnd();
            }
        }

        public void ClaerEnemy()
        {
            foreach (UnitBase u in _enemyUnits.ToArray())
            {
                u.Die();

                _allUnits.Remove(u);
                _enemyUnits.Remove(u);
            }
        }
    
        #region Get Method
        public List<UnitBase> GetAliveEnemies(TeamType team)
        {
            List<UnitBase> result = new();

            foreach (var unit in _allUnits)
            {
                if (unit == null)
                    continue;

                if (unit.CurFSM == UnitStateType.Dead)
                    continue;

                if (unit.team == TeamType.Null)
                    continue;

                if (unit.team == team)
                    continue;

                result.Add(unit);
            }

            return result;
        }

        public List<UnitBase> GetAliveAllies(TeamType team)
        {
            List<UnitBase> result = new();

            foreach (var unit in _allUnits)
            {
                if (unit == null)
                    continue;

                if (unit.CurFSM == UnitStateType.Dead)
                    continue;

                if (unit.team != team)
                    continue;

                result.Add(unit);
            }

            return result;
        }

        public IReadOnlyList<UnitBase> GetUnits(TeamType team)
        {
            return team == TeamType.Ally ? _allyUnits : _enemyUnits;
        }

        #endregion

        #region Searching Method

        /// <summary>
        /// 가장 가까운 적을 탐색하여 반환합니다.
        /// </summary>
        public UnitBase GetNearestEnemy(UnitBase owner)
        {
            var enemies = GetAliveEnemies(owner.team);

            UnitBase nearest = null;
            int minDistance = int.MaxValue;

            foreach (var enemy in enemies)
            {
                int distance = HexMath.Distance(owner.offset, enemy.offset);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }


        /// <summary>
        /// 지정한 거리 내에서 가장 먼 적을 탐색하여 반환합니다.
        /// </summary>
        public UnitBase FindFarthestEnemy(UnitBase owner, float range = float.PositiveInfinity)
        {
            var enemies = GetAliveEnemies(owner.team);

            UnitBase farthest = null;
            int maxDist = int.MinValue;

            foreach (var e in enemies)
            {
                if (e == null) continue;

                int dist = HexMath.Distance(owner.offset, e.offset);

                if (range < dist) continue;     // 지정한 거리 보다 멀먼 무시

                if (dist > maxDist)
                {
                    maxDist = dist;
                    farthest = e;
                }
            }

            return farthest;

        }

        #endregion
    }
}
