using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ブロックをネットワークオブジェクトとして生成するコンポーネント
/// </summary>
public class BlockGenerator : MonoBehaviour
{
    /// <summary>生成するブロックのプレハブ名</summary>
    [SerializeField] string m_destructableBlockPrefabName = "Prefab";
    /// <summary>生成するブロック数</summary>
    [SerializeField] int m_blockCount = 50;
    bool m_isInitialized = false;

    void Update()
    {
        // MasterClient として部屋に入った時、一回だけコルーチンとしてブロックを生成する
        if (!m_isInitialized)
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    m_isInitialized = true;
                    Debug.Log("Start Generating Blocks...");
                    StartCoroutine(GenerateBlocksImpl(m_blockCount));
                }
                else
                {
                    m_isInitialized = true;
                }
            }
        }
    }

    /// <summary>
    /// Implementation for generating blocks.
    /// </summary>
    /// <param name="blockCount"></param>
    /// <returns></returns>
    IEnumerator GenerateBlocksImpl(int blockCount)
    {
        for (int i = 0; i < blockCount; i++)
        {
            // ランダムな座標にネットワークオブジェクトとしてプレハブを生成する
            Vector3 pos = new Vector3(Random.Range(-2, 2), Random.Range(1, 20), Random.Range(-2, 2));
            PhotonNetwork.Instantiate(m_destructableBlockPrefabName, pos, Quaternion.identity);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
