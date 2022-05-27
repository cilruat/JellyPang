using UnityEngine;
using System.Collections;

public class LoadingUI : ObjectSingleton<LoadingUI>
{
    public void SetLoading(bool show)
    {
        gameObject.SetActive(show);
    }
}
