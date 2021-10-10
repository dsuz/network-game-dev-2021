using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;         // NCMB を使うため

/// <summary>
/// ランキングシステムを管理するクラス
/// </summary>
public class RankingManager : MonoBehaviour
{
    /// <summary>ランキングを表示する Text</summary>
    [SerializeField] Text m_rankingText;
    /// <summary>名前を入力するフィールド</summary>
    [SerializeField] InputField m_nameInput;
    /// <summary>名前の登録を行うためのオブジェクトが配置されたパネル</summary>
    [SerializeField] RectTransform m_entryPanel;
    /// <summary>最初にrankingを閉じさせない秒数</summary>
    [SerializeField] float m_gracePeriod = 10f;
    /// <summary>ランキング情報の配列</summary>
    List<NCMBObject> m_ranking;
    /// <summary>今回のスコア</summary>
    int m_score;
    float m_timer;
    /// <summary>画面を閉じてもよいか</summary>
    bool m_closable = false;

    void Update()
    {
        if (!m_closable)
        {
            m_timer += Time.deltaTime;

            if (m_timer > m_gracePeriod)
            {
                m_closable = true;
            }
        }
    }

    /// <summary>
    /// ランキングシステムを閉じる
    /// </summary>
    public void CloseRanking()
    {
        if (m_closable && !m_entryPanel.gameObject.activeSelf)    // エントリーが表示されている間は閉じさせない
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ランキングを取得する
    /// ランキングをサーバーから取ってきて、10 位以内に入っていたら名前を入力する画面を表示する
    /// </summary>
    /// <param name="score">今回のスコア。0 の場合はランキング入力画面は出ない</param>
    public void GetRanking(int score)
    {
        m_score = score;

        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");
        query.OrderByDescending("Score");
        query.Limit = 10;

        // 検索する https://mbaas.nifcloud.com/assets/sdk_doc/unity/Help/classNCMB_1_1NCMBQuery.html#a6210c11562957ea3fb6a8a417939c7b5
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError(e.ToString());
            }
            else
            {
                // 結果を保存する
                m_ranking = objList;
                // ランキングを表示する
                MakeRankingText();

                // ランキングの一番下より点数が大きい場合は
                if ((score > 0 && m_ranking.Count < 10) || score > int.Parse(m_ranking[m_ranking.Count - 1]["Score"].ToString()) || m_ranking.Count == 0)
                {
                    m_entryPanel.gameObject.SetActive(true);    // エントリーパネルを表示する
                }
            }
        });
    }

    /// <summary>
    /// ランキング情報の配列から、ランキング情報のテキストを作って表示する
    /// </summary>
    void MakeRankingText()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < m_ranking.Count; i++)
        {
            builder.Append((i + 1).ToString());
            builder.Append(" : ");
            builder.Append(m_ranking[i]["Name"].ToString());
            builder.Append(" : ");
            builder.AppendLine(m_ranking[i]["Score"].ToString());
        }
        Debug.Log("Ranking Text:\r\n" + builder.ToString());
        m_rankingText.text = builder.ToString();
    }

    /// <summary>
    /// 今回のスコアを登録する。ランキングシステムを呼び出したら、まずこの関数を呼び出す。
    /// これを行うと、ランキングの取得や表示が始まる
    /// </summary>
    /// <param name="score"></param>
    public void SetScoreOfCurrentPlay(int score)
    {
        GetRanking(score);
    }

    /// <summary>
    /// ハイスコアの名前登録を行う
    /// </summary>
    public void Entry()
    {
        // 保存するためのデータを作る
        NCMBObject obj = new NCMBObject("HighScore");
        obj["Name"] = m_nameInput.text;
        obj["Score"] = m_score;
        
        // データを保存する https://mbaas.nifcloud.com/assets/sdk_doc/unity/Help/classNCMB_1_1NCMBObject.html#a2e429d428a70cb7ef9ad6230b0cd2837
        obj.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError(e.ToString());
            }
            else
            {
                // 正常終了したら、エントリー画面を消してランキングをリロードする
                m_entryPanel.gameObject.SetActive(false);   // エントリー画面を消す
                GetRanking(0);  // ランキングをリロードする
            }
        });
    }
}
