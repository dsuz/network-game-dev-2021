using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当たってはいけない場所（基地）を制御するコンポーネント
/// </summary>
public class KillzoneController : MonoBehaviour
{
    /// <summary>GameManager への参照</summary>
    GameManager m_gameManager;

    void Start()
    {
        // GameManager を取得しておく
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (!m_gameManager)
        {
            Debug.LogError("Not found GameManager. GameManager must be attached to the GameObject with Tag: GameController");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 隕石がぶつかってきたら
        if (collision.gameObject.tag == "Respawn")
        {
            m_gameManager.Damage(1);    // ライフを 1 減らして
            Destroy(collision.gameObject);  // 隕石は破棄する
        }
    }
}
