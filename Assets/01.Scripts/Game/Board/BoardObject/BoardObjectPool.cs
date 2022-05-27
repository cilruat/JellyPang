using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardObjectPool : MonoBehaviour
{
    private Dictionary<BoardObjectBase.TYPE, ObjectPool<BoardObjectBase>> _poolDic = new Dictionary<BoardObjectBase.TYPE, ObjectPool<BoardObjectBase>>();

    private readonly int _instantiateCnt = 1;
    private readonly int _overAllocateCnt = 1;

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="boardSize"></param>
    public void Init()
    {
        Transform tr = transform;

        foreach(var pair in GameDataSetting.DefaultSetting.BoardObjPrefabDic)
        {
            BoardObjectBase.TYPE type = pair.Key;
            BoardObjectBase prefab = pair.Value;

            ObjectPool<BoardObjectBase> pool = new ObjectPool<BoardObjectBase>(prefab, Util.StringAppend(type.ToString(), "_"), tr, _instantiateCnt, _overAllocateCnt);

            _poolDic.Add(type, pool);
        }
    }

    public T Get<T>() where T : BoardObjectBase
    {
        BoardObjectBase.TYPE type;

        switch (typeof(T).Name)
        {
            case "BoardObject_Gem":
                type = BoardObjectBase.TYPE.GEM;
                break;
            case "BoardObject_Diamond":
                type = BoardObjectBase.TYPE.DIAMOND;
                break;
            default:
                Debug.LogError("BoardObjectPool - Get - InvalidType");
                return null;
        }

        if (!_poolDic.ContainsKey(type))
        {
            Debug.LogError("BoardObjectPool - Get - InvalidType");
            return null;
        }

        T obj = _poolDic[type].Pop() as T;
        return obj;
    }

    public void Return(BoardObjectBase obj)
    {
        if (!_poolDic.ContainsKey(obj.Type))
        {
            Debug.LogError("BoardObjectPool - Get - InvalidType");
            return;
        }

        _poolDic[obj.Type].Push(obj);
    }
}
