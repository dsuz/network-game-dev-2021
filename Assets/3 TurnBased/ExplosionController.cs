using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 爆発・ブロック破壊機能を追加するコンポーネント
/// </summary>
public class ExplosionController : MonoBehaviour
{
    void Start()
    {
        // ParticleSystem の再生が終わったら破棄する
        ParticleSystem ps = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, ps.main.duration);
    }

    /// <summary>
    /// ブロックに Trigger が当たったら破棄する
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        var block = other.gameObject.GetComponent<DestructableBlockController>();
        block.Kill();
    }
}
