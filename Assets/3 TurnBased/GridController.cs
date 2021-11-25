using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    public bool Fixed { get; set; }
    public string Mark
    {
        get
        {
            return _text.text;
        }
    }
    [SerializeField] Color _fixedColor = Color.white;
    [SerializeField] Color _notFixedColor = Color.red;
    Text _text = default;

    void Start()
    {
        _text = GetComponent<Text>();
    }

    public void Set(string mark)
    {
        if (this.Fixed) return;
        _text.text = mark;
        _text.color = _notFixedColor;
    }

    public void Fix()
    {
        this.Fixed = true;
        _text.color = _fixedColor;
    }

    public void Clear()
    {
        this.Set("");
    }
}
