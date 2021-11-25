using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ブロックに追加するコンポーネント
/// ブロックを破棄する
/// </summary>
public class DestructableBlockController : MonoBehaviour
{
    /// <summary>ネットワークオブジェクトを破棄するための PhotonView の参照</summary>
    PhotonView view;
    bool isDead = false;

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        // プラットホームから落ちたら破棄する
        if (this.transform.position.y < -1)
        {
            isDead = true;
        }

        // 破壊フラグが立っていたら破棄する
        if (isDead && view && view.IsMine)
        {
            PhotonNetwork.Destroy(view);
        }
    }

    public void Kill()
    {
        isDead = true;
    }
}
