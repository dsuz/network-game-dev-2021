using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 迎撃ミサイルを制御するコンポーネント
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ShotController : MonoBehaviour
{
    ParticleSystem m_particle;
    GameManager m_gameManager;

    void Start()
    {
        // 使用するオブジェクト/コンポーネントの参照を取っておく
        m_particle = GetComponent<ParticleSystem>();
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (!m_gameManager)
        {
            Debug.LogError("Not found GameManager. GameManager must be attached to the GameObject with Tag: GameController");
        }
    }

    void Update()
    {
        // パーティクルが再生中じゃない時にクリックしたら弾を発射する
        if (m_particle.isStopped && Input.GetButtonDown("Fire1"))
        {
            // クリックした場所に移動して
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos += Vector3.forward;
            this.transform.position = pos;
            // パーティクルを再生する
            m_particle.Play();
        }
    }

    /// <summary>
    /// パーティクルのコライダーに衝突があった時に呼ばれる関数
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        // 衝突相手が隕石だったら
        if (other.gameObject.tag == "Respawn")
        {
            Destroy(other.gameObject);  // 隕石を破壊し
            m_gameManager.AddScore(1);  // スコアを加算する
        }
    }
}
