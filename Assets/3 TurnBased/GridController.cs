using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 三目並べのマス目を制御する
/// クリックして〇×を表示したり、色を変えたりする機能を持つ
/// </summary>
public class GridController : MonoBehaviour
{
    /// <summary>マス目の内容が確定しているれば true</summary>
    public bool Fixed { get; set; }
    /// <summary>マス目に表示されている〇, ×, (空白)を取得する</summary>
    public string Mark
    {
        get
        {
            return _text.text;
        }
    }
    /// <summary>内容が確定している時の色</summary>
    [SerializeField] Color _fixedColor = Color.white;
    /// <summary>内容が未確定の時の色</summary>
    [SerializeField] Color _notFixedColor = Color.red;
    /// <summary>内容を表示するための Text</summary>
    Text _text = default;

    void Start()
    {
        _text = GetComponent<Text>();
    }

    /// <summary>
    /// 未確定の状態で内容をセットする
    /// </summary>
    /// <param name="mark">内容としてセットする文字列</param>
    public void Set(string mark)
    {
        if (this.Fixed) return; // 確定状態の時は処理をしない
        _text.text = mark;
        _text.color = _notFixedColor;
    }

    /// <summary>
    /// 内容を確定させる
    /// </summary>
    public void Fix()
    {
        this.Fixed = true;
        _text.color = _fixedColor;
    }

    /// <summary>
    /// 内容をクリアする
    /// </summary>
    public void Clear()
    {
        this.Set("");
    }
}
