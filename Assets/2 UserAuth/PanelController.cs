using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI のスライドイン・アウトする機能を提供するコンポーネント
/// UI オブジェクトに追加して使う
/// </summary>
[RequireComponent(typeof(Animator))]
public class PanelController : MonoBehaviour
{
    Animator m_anim;

    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    /// <summary>
    /// スライドインする
    /// </summary>
    public void SlideIn()
    {
        m_anim.Play("In");
    }

    /// <summary>
    /// スライドアウトする
    /// </summary>
    public void SlideOut()
    {
        m_anim.Play("Out");
    }
}
