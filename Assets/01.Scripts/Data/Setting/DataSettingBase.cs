using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 데이터 세팅파일 베이스 클래스
/// !! 세팅파일 이름은 무조건 클래스 이름과 똑같이 할 것 !!
/// </summary>
/// <typeparam name="T"></typeparam>
/// 
public abstract class DataSettingBase<T> : ScriptableObject where T : DataSettingBase<T>
{
    private static T _defaultSetting;

    public static T DefaultSetting
    {
        get
        {
            if (_defaultSetting == null)
            {
                _defaultSetting = Resources.Load<T>(Util.StringAppend(Define.DATA_SETTING_DIR_PATH, typeof(T).Name));

                if (_defaultSetting == null)
                {
                    Debug.LogError("DataSetting is null");
                }
            }

            return _defaultSetting;
        }
    }
}