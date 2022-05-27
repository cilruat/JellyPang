using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameBoard
{
    /// <summary>
    /// 보드를 터치
    /// </summary>
    /// <param name="touchWorldPos"></param>
    public void OnTouchBoard(Vector3 touchWorldPos)
    {
        //Debug.Log(WorldPosToGridPos(touchWorldPos));

        GridPosition touchPos = WorldPosToGridPos(touchWorldPos);
        if (!IsValidGrid(touchPos))
            return;

        BoardGrid touchGrid = GetGrid(touchPos);

        StartCoroutine(TouchGridProc(touchGrid));
    }
}
