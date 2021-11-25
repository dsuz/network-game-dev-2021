using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

/// <summary>
/// レーザー砲台を制御するコンポーネント
/// 以下の機能を持つ
/// 1. ターン管理
/// 2. 自分の番の時は、マウスで Block をポイントするとレーザーの終点となる Transform をそこへ動かる
/// 3. Block をポイントした状態で Fire すると爆発のプレハブを ray の hit point に生成する
/// </summary>
public class LaserCannonController : MonoBehaviour, IPunTurnManagerCallbacks
{
    /// <summary>爆発のプレハブの名前</summary>
    [SerializeField] string m_explosionPrefabName;
    /// <summary>銃口</summary>
    [SerializeField] Transform m_muzzle;
    /// <summary>レーザーの当たる場所</summary>
    [SerializeField] Transform m_hitPoint;
    /// <summary>TurnManager</summary>
    PunTurnManager m_turnManager;
    /// <summary>自分の番の時に砲台を赤くするための Animator</summary>
    Animator m_anim;
    bool m_isMyTurn = false;

    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 自分の番ではない時は何もしない
        if (!m_isMyTurn)
        {
            return;
        }

        // カメラの位置 → マウスでクリックした場所に Ray を飛ばすように設定する
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // out パラメータで Ray の衝突情報を受け取るための変数
                        // Ray を飛ばして、コライダーに当たったかどうかを戻り値で受け取る
        bool isHit = Physics.Raycast(ray, out hit);

        // Ray が当たったかどうかで異なる処理をする
        if (isHit)
        {
            // Ray が当たっている時は線を引くために hitpoint にアンカーを移動する
            m_hitPoint.position = hit.point;
            // Ray が当たっている状態で Fire したら
            if (Input.GetButtonUp("Fire1"))
            {
                // 自分のターンが終わったことを通知する
                PunTurnManager turnManager = GameObject.FindObjectOfType<PunTurnManager>();
                turnManager.SendMove(null, true);   // 第一引数でメッセージを送れるが、今回は何もしない / 第二引数に true を渡すことで「自分のターンを終了する」ことを通知する
                FinishMyTurn(); // ローカルでの自ターン管理をする
                PhotonNetwork.Instantiate(m_explosionPrefabName, hit.point, Quaternion.identity);   // 爆発エフェクトを生成する
            }
        }
        else
        {
            // Ray が当たっていない時は、レーザーを消すために銃口とレーザーの終点を同じ位置にする
            m_hitPoint.position = m_muzzle.position;
        }
    }

    /// <summary>
    /// 自分のターン開始時に呼び出す
    /// </summary>
    void StartMyTurn()
    {
        m_anim.SetBool("IsMyTurn", true);
        m_isMyTurn = true;
    }

    /// <summary>
    /// 自分のターン終了時に呼び出す
    /// </summary>
    void FinishMyTurn()
    {
        m_anim.SetBool("IsMyTurn", false);
        m_hitPoint.position = m_muzzle.position;    // レーザーを消す
        m_isMyTurn = false;
    }

    #region IPunTurnManagerCallbacks の実装

    /// <summary>
    /// ターン開始時に呼び出される
    /// </summary>
    /// <param name="turn">ターン番号 (1, 2, 3, ...)</param>
    void IPunTurnManagerCallbacks.OnTurnBegins(int turn)
    {
        Debug.LogFormat("OnTurnBegins {0}", turn);

        // Master クライアントが先手とする
        if (PhotonNetwork.IsMasterClient)
        {
            StartMyTurn();
        }
    }

    /// <summary>
    /// プレイヤーが行動終了したら呼び出される
    /// </summary>
    /// <param name="player">行動終了したプレイヤーの情報</param>
    /// <param name="turn"></param>
    /// <param name="move">プレイヤーが送ったメッセージ</param>
    void IPunTurnManagerCallbacks.OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        Debug.LogFormat("OnPlayerFinished {0}", turn);
        
        // 自分が MasterClient ではなくて、一つ前の ActorNumber の人が行動終了した時に
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.ActorNumber ==  player.ActorNumber + 1)
        {
            // 自分のターンとみなす
            StartMyTurn();
        }
    }

    /// <summary>
    /// プレイヤーが PunTurnManager.SendMove を呼び出したが、行動を終了していない時
    /// </summary>
    /// <param name="player">SendMove を呼び出したプレイヤーの情報</param>
    /// <param name="turn"></param>
    /// <param name="move">プレイヤーが送ったメッセージ</param>
    void IPunTurnManagerCallbacks.OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        Debug.LogFormat("OnPlayerMove {0}", turn);
    }

    /// <summary>
    /// 参加しているプレイヤー全員が行動を終了した時に呼び出される
    /// </summary>
    /// <param name="turn">ターン番号</param>
    void IPunTurnManagerCallbacks.OnTurnCompleted(int turn)
    {
        Debug.LogFormat("OnTurnCompleted {0}", turn);
        // 新たなターンを開始する
        PunTurnManager turnManager = GameObject.FindObjectOfType<PunTurnManager>();
        turnManager.BeginTurn();
    }

    /// <summary>
    /// ターンが時間切れになった時に呼び出される
    /// </summary>
    /// <param name="turn"></param>
    void IPunTurnManagerCallbacks.OnTurnTimeEnds(int turn)
    {
        Debug.LogFormat("OnTurnTimeEnds {0}", turn);
        // 新たなターンを開始する
        PunTurnManager turnManager = GameObject.FindObjectOfType<PunTurnManager>();
        turnManager.BeginTurn();
    }

    #endregion
}
