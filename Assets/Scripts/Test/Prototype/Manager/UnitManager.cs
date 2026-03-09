using UnityEngine;
using Utilitys;
using System.Collections.Generic;

namespace Prototype.Unit
{
    public class UnitManager : SingletonMonoBehaviour<UnitManager>
    {
        private readonly List<UnitBase> _allUnits = new();          // 전체 유닛 리스트
        private readonly List<UnitBase> _allyUnits = new();         // 아군 유닛 리스트
        private readonly List<UnitBase> _enemyUnits = new();        // 적군 유닛 리스트

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
        }

        #region Get Mathod
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
    }
}
