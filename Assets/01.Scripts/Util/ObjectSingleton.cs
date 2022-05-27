using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// 게임오브젝트와 연결할 싱글톤에 사용
/// 이클래스를 상속한 클래스를 다시상속하지 말것
/// </summary>
/// <typeparam name="T">구상 클래스</typeparam>
public class ObjectSingleton<T> : MonoBehaviour where T : ObjectSingleton<T>
{
    private static T _instance = null;
    private static bool _attachedGameObject;    // 존재하는 게임오브젝트가 연결되어있는가

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if (_instance != null)
                {
                    _attachedGameObject = true;
                }

                if (_instance == null && _attachedGameObject == false)
                {
                    Type t = typeof(T);

                    GameObject container = new GameObject();
                    container.name = t.Name;
                    _instance = container.AddComponent(t) as T;
                }
            }

            return _instance;
        }
    }

    public static bool IsInstanceCreated
    {
        get
        {
            if (_instance == null)
                return false;
            return true;
        }
    }

    /// <summary>
    /// 오브젝트를 생성하기위한 빈함수
    /// </summary>
    public void Create()
    {

    }

    public static void ReleaseInstance()
    {
        if (_instance == null)
            return;

        GameObject.Destroy(_instance.gameObject);
        _instance = null;
    }
}
