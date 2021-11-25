using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

/// <summary>
/// 三目並べとしてのシステムを提供するコンポーネント
/// 
/// </summary>
[RequireComponent(typeof(PunTurnManager))]
public class TicTacToeManager : MonoBehaviour, IPunTurnManagerCallbacks
{
    /// <summary>ゲーム内で使っている GridController を格納する配列</summary>
    [SerializeField] GridController[] _grids = default;
    /// <summary>ターン終了ボタン。自分のターンの時のみ interactable になる。</summary>
    [SerializeField] Button _endTurnButton = default;
    /// <summary>勝敗を表示するための Text</summary>
    [SerializeField] Text _messageText = default;
    PunTurnManager _punTurnManager = default;

    void Start()
    {
        _punTurnManager = GetComponent<PunTurnManager>();
    }

    /// <summary>
    /// マス目に〇/×を置き、その情報を各クライアントに送信する
    /// この処理が終わった時点ではマス目は確定しない
    /// </summary>
    /// <param name="gridController">操作したマス目</param>
    public void Move(GridController gridController)
    {
        if (!_endTurnButton.interactable) return;   // 自分のターンではない時は処理をしない
        // 操作した内容として送信するメッセージを組み立てて送る
        string message = gridController.name + ", " + (PhotonNetwork.IsMasterClient ? "O" : "X");
        _punTurnManager.SendMove(message, false);   // メッセージは自分を含む全てのクライアントに送られる
    }

    /// <summary>
    /// マス目の操作として送られてきたメッセージを処理する
    /// この処理が終わった時点ではマス目は確定しない
    /// </summary>
    /// <param name="move"></param>
    void OnPlayerMove(object move)
    {
        // 未確定のマス目をクリアする
        _grids.Where(g => !g.Fixed).ToList().ForEach(g => g.Clear());
        // 送られてきたメッセージを処理する
        var moveMessage = move.ToString().Split(',');
        string gridName = moveMessage[0];   // 操作されたマス目のオブジェクト名
        string mark = moveMessage[1];       // 〇 or ×
        _grids.Where(g => g.name == gridName).FirstOrDefault().Set(mark);   // 〇 or × をマス目にセットする
    }

    /// <summary>
    /// プレイヤーがターンを終了した時の処理
    /// マス目を確定させ、勝敗判定をする
    /// </summary>
    /// <returns>勝敗が決していたら true</returns>
    bool OnPlayerFinished()
    {
        // 未確定のマス目を確定させる
        _grids.Where(g => !g.Fixed && g.Mark != "").ToList().ForEach(g => g.Fix());
        return Judge();
    }

    /// <summary>
    /// 勝敗を判定する
    /// </summary>
    /// <returns>勝敗が決していたら true</returns>
    bool Judge()
    {
        // この勝敗判定は正しくなく、真ん中の行が揃った時のみ勝敗が確定する
        if (_grids[3].Mark == _grids[4].Mark && _grids[4].Mark == _grids[5].Mark &&
            _grids[3].Fixed && _grids[4].Fixed && _grids[5].Fixed)
        {
            Endgame(_grids[3].Mark);
            return true;
        }

        return false;
    }

    /// <summary>
    /// ゲームの終了処理をする
    /// </summary>
    /// <param name="winner">勝者</param>
    void Endgame(string winner)
    {
        _messageText.text = winner + " WINS!";
        _endTurnButton.interactable = false;
    }

    /// <summary>
    /// 自分のターンを開始する時に呼ぶ
    /// </summary>
    void BeginTurn()
    {
        _endTurnButton.interactable = true;
    }

    /// <summary>
    /// 自分のターンを終了する時に呼ぶ
    /// ターン終了ボタンをクリックした時に呼ばれることを想定している
    /// </summary>
    public void EndTurn()
    {
        _punTurnManager.SendMove(null, true);   // ターン終了を知らせる
        _endTurnButton.interactable = false;
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
            this.BeginTurn();
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
        Debug.LogFormat($"OnPlayerFinished from Player: {player.ActorNumber}, move: {move}, for turn: {turn}");

        if (!this.OnPlayerFinished())
        {
            // 自分が MasterClient ではなくて、一つ前の ActorNumber の人が行動終了した時に
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber + 1)
            {
                // 自分のターンとみなす
                this.BeginTurn();
            }
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
        Debug.LogFormat($"OnPlayerMove received from Player: {player.ActorNumber}, move: {move.ToString()} for turn {turn}");
        this.OnPlayerMove(move);
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
