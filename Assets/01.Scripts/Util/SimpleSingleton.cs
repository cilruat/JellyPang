using System;
using System.Reflection;

/// <summary>
/// 게임오브젝트와 연결할 필요가 없는 싱글톤에 사용, 생성자를 Private로 할것
/// </summary>
/// <typeparam name="T">구상 클래스</typeparam>
public class SimpleSingleton<T> where T : SimpleSingleton<T>
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type t = typeof(T);

                //ConstructorInfo[] ctors = t.GetConstructors();
                //if (ctors.Length > 0)
                //{
                //    throw new InvalidOperationException(String.Format("{0}의 생성자가 존재하지 않습니다. (private 생성자 필요)", t.Name));
                //}

                // private 생성자로 생성
                _instance = (T)Activator.CreateInstance(t, true);
            }

            return _instance;
        }
    }

    public static void ReleaseInstance()
    {
        if (_instance != null)
            _instance = null;
    }
}
