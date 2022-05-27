using UnityEngine;
using System.Collections;

public class BoardGrid : MonoBehaviour
{
    private GridPosition _position;
    private BoardObjectBase _boardObj;

    public GridPosition Position => _position;
    public bool IsEmpty => _boardObj == null;
    public BoardObjectBase BoardObject => _boardObj;

    public void Init(GridPosition pos)
    {
        _position = pos;
    }

    public void SetBoardObj(BoardObjectBase obj)
    {
        _boardObj = obj;
        _boardObj.Owner = this;
    }

    public void ReleaseBoardObj()
    {
        _boardObj.Owner = null;
        _boardObj = null;
    }
}
