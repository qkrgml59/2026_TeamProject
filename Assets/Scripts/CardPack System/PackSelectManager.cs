using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PackSelectManager : MonoBehaviour
{
    [Header("카드팩 UI 목록")]
    public List<CardPackView> packViews = new();

    [Header("선택된 카드팩")]
    public List<CardPackView> selectedPacks = new();

    [Header("선택 제한")]
    public int selectLimit = 3;

    private void Start()
    {
        
    }

    /// <summary>
    /// 카드팩 생성
    /// </summary>
    public void GenratePacks()
    {
        for (int i = 0; i< packViews.Count; i++)
        {
            var cards = StageManager.Instance.GetPackPreviewCards(10);

            packViews[i].Init(
                cards,
                OnSelectPack,
                OnOpenDetail
                );
        }
    }

    ///<summary>
    ///카드팩 선택
    /// </summary>
    private void OnSelectPack(CardPackView pack)
    {
        if (selectedPacks.Contains(pack))
            return;

        if (selectedPacks.Count >= selectLimit)
            return;

        selectedPacks.Add(pack);

        Debug.Log($"팩 선택 됨 ({selectedPacks.Count}/{selectLimit})");
    }

    ///<summary>
    ///카드팩 상세 보기 (우클릭)
    /// </summary>
    private void OnOpenDetail(CardPackView pack)
    {
        PackDetailUI.Instance.Show(pack.GetCards());
    }

    ///<summary>
    ///선택 완료 -> 다음 씬
    /// </summary>
    public void Confirm()
    {
        if(selectedPacks.Count < selectLimit)
        {
            Debug.Log("3개의 팩을 선택하세요");
            return;
        }

        SceneManager.LoadScene("");
    }
}
