using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOverIcon : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOShakePosition(1f, 0.3f, 100);
    }
}
