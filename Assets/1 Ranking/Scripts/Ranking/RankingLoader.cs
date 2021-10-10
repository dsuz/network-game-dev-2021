using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ランキングシステムをテストするためのテスト用コード
/// </summary>
public class RankingLoader : MonoBehaviour
{
    [SerializeField] GameObject m_rankingPrefab;
    [SerializeField] Button m_setScoreButton;

    public void LoadRanking()
    {
        var go = Instantiate(m_rankingPrefab);
        m_setScoreButton.onClick.AddListener(delegate { go.GetComponent<RankingManager>().SetScoreOfCurrentPlay(10); });    // これは期待したように動かなかった
    }
}
