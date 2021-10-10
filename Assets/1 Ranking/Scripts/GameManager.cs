using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームを管理するマネージャーコンポーネント
/// 管理オブジェクトにアタッチして使う
/// </summary>
[RequireComponent(typeof(CometGenerator))]
public class GameManager : MonoBehaviour
{
    /// <summary>ミスしてもいい回数</summary>
    [SerializeField] int m_maxLife = 5;
    /// <summary>スコア表示</summary>
    [SerializeField] Text m_scoreText;
    /// <summary>ライフ表示</summary>
    [SerializeField] Text m_lifeText;
    /// <summary>ゲーム開始ボタン</summary>
    [SerializeField] Button m_startButton;
    /// <summary>ランキングシステムのプレハブ</summary>
    [SerializeField] GameObject m_rankingPrefab;
    /// <summary>CometGenerator のオブジェクト</summary>
    CometGenerator m_cometGenerator;
    /// <summary>スコア</summary>
    int m_score;
    /// <summary>ミスしてもいい回数</summary>
    int m_life;

    void Start()
    {
        m_cometGenerator = GetComponent<CometGenerator>();
    }

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    public void StartGame()
    {
        m_startButton.gameObject.SetActive(false);  // スタートボタンを消す
        m_score = 0;    // スコアをリセットする
        m_life = m_maxLife; // ライフをリセットする
        AddScore(0);    // 表示をリセットする
        Damage(0);  // 表示をリセットする
        m_cometGenerator.StartGenerate();   // 隕石の生成開始
    }

    /// <summary>
    /// 点数を追加する
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        m_score += score;
        m_scoreText.text = m_score.ToString();
    }

    /// <summary>
    /// ライフを減らす
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        m_life -= damage;
        m_lifeText.text = m_life.ToString();

        if (m_life < 1)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    void GameOver()
    {
        m_startButton.gameObject.SetActive(true);   // スタートボタンを表示する
        m_cometGenerator.StopGenerate();    // 隕石の生成を止める

        // ランキングシステムを発動させる
        var ranking = Instantiate(m_rankingPrefab);
        ranking.GetComponent<RankingManager>().SetScoreOfCurrentPlay(m_score);
    }
}
