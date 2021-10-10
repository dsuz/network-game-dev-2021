using UnityEngine;

/// <summary>
/// 隕石を生成するコンポーネント
/// 管理オブジェクトにアタッチする
/// </summary>
[RequireComponent(typeof(GameManager))]
public class CometGenerator : MonoBehaviour
{
    /// <summary>隕石を生成する場所</summary>
    [SerializeField] Transform[] m_spawnPoints;
    /// <summary>隕石として生成するプレハブ</summary>
    [SerializeField] GameObject m_cometPrefab;
    /// <summary>生成した隕石はこのオブジェクトの子オブジェクトとする</summary>
    [SerializeField] Transform m_prefabGenerationRoot;
    /// <summary>ゲーム中かどうか</summary>
    bool m_isInGame;
    /// <summary>隕石の生成間隔を管理するタイマー</summary>
    float m_timer;
    /// <summary>次の隕石が生成されるまでの間隔</summary>
    float m_interval;

    void Update()
    {
        if (!m_isInGame) return;    // ゲーム中でない場合は何もしない
        m_timer += Time.deltaTime;  // タイマー加算

        if (m_timer > m_interval)   // 間隔を越えたら
        {
            m_timer = 0f;   // タイマーをリセット
            int i = Random.Range(0, m_spawnPoints.Length);  // どの場所に隕石を生成するかランダムに決める
            var go = Instantiate(m_cometPrefab, m_spawnPoints[i].position, Quaternion.identity);    // 隕石を生成する
            go.transform.SetParent(m_prefabGenerationRoot); // ルートの子オブジェクトに設定する
            m_interval = Random.Range(0.5f, 1f);    // 次の隕石生成までの間隔をランダムに決める
        }
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGenerate()
    {
        m_isInGame = true;
    }

    /// <summary>
    /// ゲーム終了
    /// 隕石の生成を止めて、シーン内の隕石を全て消す
    /// </summary>
    public void StopGenerate()
    {
        m_isInGame = false;

        // 隕石のルートオブジェクトの子オブジェクトを全て消す
        foreach (var t in m_prefabGenerationRoot.GetComponentsInChildren<Transform>())
        {
            if (t != m_prefabGenerationRoot)    // ルートオブジェクトは消さないように
            {
                Destroy(t.gameObject);
            }
        }
    }
}
