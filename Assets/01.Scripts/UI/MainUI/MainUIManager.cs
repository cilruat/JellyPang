using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : ObjectSingleton<MainUIManager>
{
    void Start()
    {
        //서버 연결 초기화
        NetworkManager.Instance.Init();
    }

}
