using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardFxPool : MonoBehaviour
{
    private Dictionary<BoardFx.TYPE, ObjectPool<BoardFx>> _poolDic = new Dictionary<BoardFx.TYPE, ObjectPool<BoardFx>>();

    private readonly int _instantiateCnt = 1;
    private readonly int _overAllocateCnt = 1;

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="boardSize"></param>
    public void Init()
    {
        Transform tr = transform;

        foreach (var pair in GameDataSetting.DefaultSetting.BoardFxPrefabDic)
        {
            BoardFx.TYPE type = pair.Key;
            BoardFx prefab = pair.Value;

            ObjectPool<BoardFx> pool = new ObjectPool<BoardFx>(prefab, Util.StringAppend(type.ToString(), "_"), tr, _instantiateCnt, _overAllocateCnt);

            _poolDic.Add(type, pool);
        }
            }

    public BoardFx Get(BoardFx.TYPE type)
    {
        if (!_poolDic.ContainsKey(type))
        {
            Debug.LogError("BoardFxPool - Get - InvalidType");
            return null;
        }

        BoardFx obj = _poolDic[type].Pop();
        obj.Pool = this;

        return obj;
    }

    public void Return(BoardFx obj)
    {
        if (!_poolDic.ContainsKey(obj.Type))
        {
            Debug.LogError("BoardFxPool - Return - InvalidType");
            return;
        }

        _poolDic[obj.Type].Push(obj);
    }
}
