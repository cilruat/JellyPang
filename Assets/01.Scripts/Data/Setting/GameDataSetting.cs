using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Studio4Fun/Setting/GameDataSetting", order = 1)]
public class GameDataSetting : DataSettingBase<GameDataSetting>
{
    [SerializeField]
    private BoardGrid _gridPrefab = null;

    [SerializeField]
    private BoardObjectBase[] _boardObjectPrefab = null;
    [SerializeField]
    private GemObjectBase[] _gemObjectPrefab = null;
    [SerializeField]
    private BoardFx[] _boardFxPrefab = null;

    private Dictionary<BoardObjectBase.TYPE, BoardObjectBase> _boardObjPrefabDic;
    private Dictionary<GemObjectBase.TYPE, GemObjectBase> _gemObjPrefabDic;    
    private Dictionary<BoardFx.TYPE, BoardFx> _boardFxPrefabDic;

    public BoardGrid GridPrefab => _gridPrefab;

    public Dictionary<BoardObjectBase.TYPE, BoardObjectBase> BoardObjPrefabDic 
    { 
        get 
        {
            if (_boardObjPrefabDic == null)
            {
                _boardObjPrefabDic = new Dictionary<BoardObjectBase.TYPE, BoardObjectBase>();

                for (int idx = 0; idx < _boardObjectPrefab.Length; ++idx)
                {
                    BoardObjectBase obj = _boardObjectPrefab[idx];
                    _boardObjPrefabDic.Add(obj.Type, obj);
                }
            }

            return _boardObjPrefabDic;
        } 
    }
    public Dictionary<GemObjectBase.TYPE, GemObjectBase> GemObjPrefabDic
    {
        get
        {
            if (_gemObjPrefabDic == null)
            {
                _gemObjPrefabDic = new Dictionary<GemObjectBase.TYPE, GemObjectBase>();

                for (int idx = 0; idx < _gemObjectPrefab.Length; ++idx)
                {
                    GemObjectBase obj = _gemObjectPrefab[idx];
                    _gemObjPrefabDic.Add(obj.Type, obj);
                }
            }

            return _gemObjPrefabDic;
        }
    }
    public Dictionary<BoardFx.TYPE, BoardFx> BoardFxPrefabDic
    {
        get
        {
            if (_boardFxPrefabDic == null)
            {
                _boardFxPrefabDic = new Dictionary<BoardFx.TYPE, BoardFx>();

                for (int idx = 0; idx < _boardFxPrefab.Length; ++idx)
                {
                    BoardFx obj = _boardFxPrefab[idx];
                    _boardFxPrefabDic.Add(obj.Type, obj);
                }
            }

            return _boardFxPrefabDic;
        }
    }

    public BoardObjectBase GetBoardObjectPrefab(BoardObjectBase.TYPE type)
    {
        return BoardObjPrefabDic[type];
    }

    public BoardFx GetBoardFxPrefab(BoardFx.TYPE type)
    {
        return BoardFxPrefabDic[type];
    }


}
