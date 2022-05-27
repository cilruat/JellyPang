using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : Slider
{
    private Text _text = null;

    public void SetText(string value)
    {
        if (_text == null)
        {
            _text = GetComponentInChildren<Text>();
        }

        _text.text = value;
    }
}
