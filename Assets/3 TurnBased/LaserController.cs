using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 銃口から出るレーザーを Line Renderer で制御するコンポーネント
/// </summary>
public class LaserController : MonoBehaviour
{
    /// <summary>銃口の位置を示す Transform</summary>
    [SerializeField] Transform m_muzzle;
    /// <summary>レーザーが当たる位置を示す Transform</summary>
    [SerializeField] Transform m_hitPoint;
    LineRenderer m_line;

    void Start()
    {
        m_line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // muzzle -> hit point に Line を引く
        m_line.SetPosition(0, m_muzzle.position);
        m_line.SetPosition(1, m_hitPoint.position);
    }
}
