using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameBoard
{
    /// <summary>
    /// 그리드 가져오기
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public BoardGrid GetGrid(int row, int col)
    {
        if (!IsValidGrid(row, col))
            return null;

        return _boardGrids[row, col];
    }
    public BoardGrid GetGrid(GridPosition pos)
    {
        return GetGrid(pos.row, pos.col);
    }

    /// <summary>
    /// 유효한 그리드인지 검사
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    /// 
    public bool IsValidGrid(int row, int col)
    {
        if (row < 0 || col < 0)
            return false;

        if (row >= Define.BOARD_SIZE.row || col >= Define.BOARD_SIZE.col)
            return false;

        return true;
    }
    public bool IsValidGrid(GridPosition pos)
    {
        return IsValidGrid(pos.row, pos.col);
    }

    /// <summary>
    /// 월드좌표 위치의 그리드 정보 제공
    /// </summary>
    /// <param name="touchPos"></param>
    /// <returns></returns>
    public GridPosition WorldPosToGridPos(Vector3 touchPos)
    {
        int row = Mathf.FloorToInt((((((Define.BOARD_SIZE.row * 0.5f) * Define.GRID_WORLD_WIDTH) - touchPos.y) + transform.position.y) / Define.GRID_WORLD_WIDTH));
        int col = Mathf.FloorToInt((((touchPos.x - ((Define.BOARD_SIZE.col * 0.5f) * -Define.GRID_WORLD_WIDTH)) + transform.position.x) / Define.GRID_WORLD_WIDTH));

        return new GridPosition(row, col);
    }

    /// <summary>
    /// 그리드의 World Position값 가져오기
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 GridPosToWorldPos(GridPosition pos, bool isLocalPos = true)
    {
        float posX = -(((Define.BOARD_SIZE.col * 0.5f) - 0.5f) * Define.GRID_WORLD_WIDTH) + (Define.GRID_WORLD_WIDTH * pos.col);
        float posY = (((Define.BOARD_SIZE.row * 0.5f) - 0.5f) * Define.GRID_WORLD_WIDTH) + (-Define.GRID_WORLD_WIDTH * pos.row);

        Vector3 result = Vector3.zero;

        if(isLocalPos)
            result = new Vector3(posX, posY, 0f);
        else
            result = new Vector3(posX + transform.position.x, posY + transform.position.y, 0f);

        return result;
    }

    /// <summary>
    /// 근처에 현재 위치와 같은 색상 젬검색
    /// </summary>
    /// <param name="list"></param>
    private void FindFiils(GridPosition pos, BoardObjectBase.COLOR targetColor, List<GridPosition> list)
    {
        if (!IsValidGrid(pos))
            return;

        if (list.Contains(pos))
            return;

        BoardGrid grid = GetGrid(pos);
        if (grid.IsEmpty)
            return;

        switch(grid.BoardObject.Type)
        {
            case BoardObjectBase.TYPE.GEM:
                {
                    if (grid.BoardObject.Color != targetColor)
                        return;

                    list.Add(pos);

                    FindFiils(pos.Up, targetColor, list);
                    FindFiils(pos.Down, targetColor, list);
                    FindFiils(pos.Left, targetColor, list);
                    FindFiils(pos.Right, targetColor, list);
                }
                break;
            default:
                return;
        }
    }

    private List<GridPosition> FindConnectedFever(List<GridPosition> list)
    {
        List<GridPosition> result = new List<GridPosition>();

        for(int idx = 0; idx < list.Count; ++idx)
        {
            GridPosition pos = list[idx];

            GridPosition[] arr = new GridPosition[] { pos.Up, pos.Down, pos.Left, pos.Right };
            for(int arrIdx = 0; arrIdx < arr.Length; ++arrIdx)
            {
                if (list.Contains(arr[arrIdx]))
                    continue;

                if (result.Contains(arr[arrIdx]))
                    continue;

                BoardGrid targetGrid = GetGrid(arr[arrIdx]);
                if (targetGrid == null)
                    continue;

                if (targetGrid.IsEmpty || targetGrid.BoardObject.Color == BoardObjectBase.COLOR.NONE || !targetGrid.BoardObject.IsSplashable)
                    continue;

                result.Add(arr[arrIdx]);
            }
        }

        return result;
    }

    /// <summary>
    /// 해당 위치의 오브젝트가 몇 칸 까지 내려가야하는지 체크
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetEmptyGridBelow(GridPosition pos)
    {
        int result = 0;

        if (pos.row >= Define.BOARD_SIZE.row)
            return result;

        for(int row = pos.row + 1; row < Define.BOARD_SIZE.row; ++row)
        {
            BoardGrid grid = GetGrid(row, pos.col);

            if (!grid.IsEmpty)
                break;

            ++result;
        }

        return result;
    }

    /// <summary>
    /// 두 Grid의 보드오브젝트 스왑
    /// </summary>
    /// <param name="grid1"></param>
    /// <param name="grid2"></param>
    public void SwapBoardObject(BoardGrid grid1, BoardGrid grid2)
    {
        BoardObjectBase tempObj1 = grid1.BoardObject;
        BoardObjectBase tempObj2 = grid2.BoardObject;

        grid1.ReleaseBoardObj();
        grid2.ReleaseBoardObj();
        
        grid1.SetBoardObj(tempObj2);
        grid2.SetBoardObj(tempObj1);
    }

    /// <summary>
    /// 보드오브젝트 낙하시 Grid변경
    /// </summary>
    /// <param name="fromGrid"></param>
    /// <param name="toGrid"></param>
    public void FallBoardObject(BoardGrid fromGrid, BoardGrid toGrid)
    {
        if (fromGrid.IsEmpty)
        {
            Debug.LogError("GameBoard - FallBoardObject - Invalid fromGrid");
            return;
        }

        if (!toGrid.IsEmpty)
        {
            Debug.LogError("GameBoard - FallBoardObject - Invalid toGrid");
            return;
        }

        BoardObjectBase boardObj = fromGrid.BoardObject;

        fromGrid.ReleaseBoardObj();
        toGrid.SetBoardObj(boardObj);
    }

    /// <summary>
    /// 주위 Grid 중 보드오브젝트의 색상이 다른 그리드 전달
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<BoardGrid> GetAroundDiffrentColorGrid(BoardGrid grid)
    {
        List<BoardGrid> result = new List<BoardGrid>();

        GridPosition[] posArr = { grid.Position.Up, grid.Position.Down, grid.Position.Left, grid.Position.Right };
        for(int idx = 0; idx < posArr.Length; ++idx)
        {
            GridPosition pos = posArr[idx];

            BoardGrid targetGrid = GetGrid(pos);
            if (targetGrid == null || targetGrid.IsEmpty)
                continue;

            if (grid.BoardObject.Color != targetGrid.BoardObject.Color)
                result.Add(targetGrid);
        }

        return result;
    }

    /// <summary>
    /// 보드 이펙트 전달
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public BoardFx GetBoardFx(BoardFx.TYPE type, bool isAutoPlay = true)
    {
        BoardFx fx = _boardFxPool.Get(type);
        if (isAutoPlay)
            fx.Play();

        return fx;
    }
}
