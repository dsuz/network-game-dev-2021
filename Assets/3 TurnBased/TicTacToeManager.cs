using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class TicTacToeManager : MonoBehaviour, IPunTurnManagerCallbacks
{
    [SerializeField] GridController[] _grids = default;
    [SerializeField] Button _endTurnButton = default;
    [SerializeField] Text _messageText = default;
    PunTurnManager _punTurnManager = default;

    void Start()
    {
        _punTurnManager = GetComponent<PunTurnManager>();
    }

    public void Move(GridController gridController)
    {
        if (!_endTurnButton.interactable) return;
        string message = gridController.name + ", " + (PhotonNetwork.IsMasterClient ? "O" : "X");
        _punTurnManager.SendMove(message, false);
    }

    void OnPlayerMove(object move)
    {
        _grids.Where(g => !g.Fixed).ToList().ForEach(g => g.Clear());
        var moveMessage = move.ToString().Split(',');
        string gridName = moveMessage[0];
        string mark = moveMessage[1];
        _grids.Where(g => g.name == gridName).FirstOrDefault().Set(mark);
    }

    bool OnPlayerFinished()
    {
        _grids.Where(g => !g.Fixed && g.Mark != "").ToList().ForEach(g => g.Fix());
        return Judge();
    }

    bool Judge()
    {
        if (_grids[3].Mark == _grids[4].Mark && _grids[4].Mark == _grids[5].Mark &&
            _grids[3].Fixed && _grids[4].Fixed && _grids[5].Fixed)
        {
            Endgame(_grids[3].Mark);
            return true;
        }

        return false;
    }

    void Endgame(string winner)
    {
        _messageText.text = winner + " WINS!";
        _endTurnButton.interactable = false;
    }

    void BeginTurn()
    {
        _endTurnButton.interactable = true;
    }

    public void EndTurn()
    {
        _punTurnManager.SendMove(null, true);
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
