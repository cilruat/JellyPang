using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemObjectPool : MonoBehaviour
{
    private Dictionary<GemObjectBase.TYPE, ObjectPool<GemObjectBase>> _poolDic = new Dictionary<GemObjectBase.TYPE, ObjectPool<GemObjectBase>>();

    private readonly int _instantiateCnt = 1;
    private readonly int _overAllocateCnt = 1;

    /// <summary>
    /// 초기화
    /// </summary>
    public void Init()
    {
        Transform tr = transform;

        foreach (var pair in GameDataSetting.DefaultSetting.GemObjPrefabDic)
        {
            GemObjectBase.TYPE type = pair.Key;
            GemObjectBase prefab = pair.Value;

            ObjectPool<GemObjectBase> pool = new ObjectPool<GemObjectBase>(prefab, Util.StringAppend(type.ToString(), "_"), tr, _instantiateCnt, _overAllocateCnt);

            _poolDic.Add(type, pool);
        }
    }

    public GemObjectBase Get(GemObjectBase.TYPE type)
    {
        if (!_poolDic.ContainsKey(type))
        {
            Debug.LogError("GemObjectPool - Get - InvalidType");
            return null;
        }

        GemObjectBase obj = _poolDic[type].Pop();
        return obj;
    }

    public void Return(GemObjectBase obj)
    {
        if (!_poolDic.ContainsKey(obj.Type))
        {
            Debug.LogError("GemObjectPool - Get - InvalidType");
            return;
        }

        _poolDic[obj.Type].Push(obj);
    }
}
