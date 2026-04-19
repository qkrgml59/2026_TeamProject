using Item;
using Prototype.Card.Item;
using Prototype.Card.Spell;
using Prototype.Card.Unit;
using Spell;
using Unit;
using UnityEngine;
using Utilitys;
namespace Game.UI
{
    public class DetailPanelController : SingletonMonoBehaviour<DetailPanelController>
    {
        [Header("유닛 상세 정보")] public UnitDetailVeiw unitDetailView;
        [Header("유닛 카드 정보")] public UnitCardDetailView unitCardDetailView;
        [Header("아이템 상세 정보")] public ItemDetailView itemDetailView;
        [Header("마법 상세 정보")] public SpellDetailView spellDetailView;

        private void Start()
        {
            HideAll();
        }

        public void HideAll()
        {
            if (unitDetailView != null)
            {
                unitDetailView.UnBind();
                unitDetailView.gameObject.SetActive(false);
            }

            if (unitCardDetailView != null)
            {
                unitCardDetailView.Clear();
                unitCardDetailView.gameObject.SetActive(false);
            }

            if (itemDetailView != null) itemDetailView.Hide();

            if (spellDetailView != null) spellDetailView.Hide();
        }

        /// <summary>
        /// 유닛의 상세 설명을 보여줍니다.
        /// </summary>
        public void ShowUnitDetail(UnitBase unit)
        {
            HideAll();

            if (unitDetailView == null || unit == null) return;

            unitDetailView.Bind(unit);
            unitDetailView.gameObject.SetActive(true);
        }

        /// <summary>
        /// 유닛 '카드'의 상세 설명을 보여줍니다.
        /// </summary>
        public void ShowUnitCardDetail(UnitCardDataSO unitCard)
        {
            HideAll();

            if (unitCardDetailView == null || unitCard == null) return;

            unitCardDetailView.Show(unitCard);
            unitCardDetailView.gameObject.SetActive(true);
        }

        /// <summary>
        /// 아이템의 상세 설명을 보여줍니다.
        /// </summary>
        public void ShowItemDetail(ItemSO item)
        {
            HideAll();

            if(itemDetailView == null || item == null) return;

            itemDetailView.Show(item);
        }

        /// <summary>
        /// 스펠의 상세 설명을 보여줍니다.
        /// </summary>
        public void ShowSpellDetail(SpellCardDataSO spell)
        {
            HideAll();

            if(spellDetailView == null || spell == null) return;

            spellDetailView.Show(spell);
        }
    }
}
