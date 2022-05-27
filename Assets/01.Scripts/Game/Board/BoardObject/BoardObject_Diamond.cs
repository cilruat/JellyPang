using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DG.Tweening;

public class BoardObject_Diamond : BoardObjectBase
{
    public override TYPE Type => TYPE.DIAMOND;
    public override bool IsExplodable => true;
    public override bool IsSplashable => false;

    public override bool IsFallable => true;
    public override bool IsSpecialObject => true;

    public override void Set(COLOR color)
    {
        base.Set(COLOR.NONE);
    }

    public override void Explode(GameBoard board)
    {
        StartCoroutine(this.ExplodeProc(board));
    }

    IEnumerator ExplodeProc(GameBoard board)
    {
        SetExplodingState(true, board);

        GridPosition pos = Owner.Position;

        float removeInterval = 0.04f;        
        float fxMoveDuration_Vertical = (pos.row + 1) * removeInterval;
        float fxMoveDuration_Left = (pos.col + 1) * removeInterval;
        float fxMoveDuration_Right = (Define.BOARD_SIZE.col - pos.col) * removeInterval;

        //위에서 떨어지는 이펙트
        BoardFx fx_Vertical = board.GetBoardFx(BoardFx.TYPE.DIAMOND_PROJECTILE, false);
        fx_Vertical.Play(fxMoveDuration_Vertical);
        fx_Vertical.transform.position = board.GridPosToWorldPos(new GridPosition(-1, pos.col));
        fx_Vertical.transform.DOLocalMoveY(Owner.transform.localPosition.y, fxMoveDuration_Vertical);
                
        //위에서 아래로 삭제
        for(int row = 0; row < pos.row; ++row)
        {
            yield return new WaitForSeconds(removeInterval);

            BoardGrid grid = board.GetGrid(new GridPosition(row, pos.col));
            if (grid == null)
                continue;

            if (grid.IsEmpty || !grid.BoardObject.IsExplodable || grid.BoardObject.IsExploding)
                continue;

            grid.BoardObject.Explode(board);
        }

        //사라지기
        _sprite.enabled = false;

        //폭발 이펙트
        BoardFx fxExplosion = board.GetBoardFx(BoardFx.TYPE.DIAMOND_EXPLOSION);
        fxExplosion.transform.position = Owner.transform.position;

        //효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_COLORBOMB);

        //좌로 가는 이펙트
        BoardFx fx_Left = board.GetBoardFx(BoardFx.TYPE.DIAMOND_PROJECTILE, false);
        fx_Left.Play(fxMoveDuration_Left);
        fx_Left.transform.position = Owner.transform.position;
        fx_Left.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        fx_Left.transform.DOLocalMoveX(board.GridPosToWorldPos(new GridPosition(pos.row, -1)).x, fxMoveDuration_Left);

        //우로 가는 이펙트
        BoardFx fx_Right = board.GetBoardFx(BoardFx.TYPE.DIAMOND_PROJECTILE, false);
        fx_Right.Play(fxMoveDuration_Right);
        fx_Right.transform.position = Owner.transform.position;
        fx_Left.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        fx_Right.transform.DOLocalMoveX(board.GridPosToWorldPos(new GridPosition(pos.row, Define.BOARD_SIZE.col)).x, fxMoveDuration_Right);

        //가로로 삭제
        int cnt = 1;
        while(true)
        {
            yield return new WaitForSeconds(removeInterval);

            BoardGrid leftGrid = board.GetGrid(new GridPosition(pos.row, pos.col - cnt));
            BoardGrid rightGrid = board.GetGrid(new GridPosition(pos.row, pos.col + cnt));

            if (leftGrid == null && rightGrid == null)
                break;

            if (leftGrid != null && !leftGrid.IsEmpty
                && leftGrid.BoardObject.IsExplodable && !leftGrid.BoardObject.IsExploding)
            {
                leftGrid.BoardObject.Explode(board);
            }

            if (rightGrid != null && !rightGrid.IsEmpty
                && rightGrid.BoardObject.IsExplodable && !rightGrid.BoardObject.IsExploding)
            {
                rightGrid.BoardObject.Explode(board);
            }

            ++cnt;
        }
                
        //삭제
        SetExplodingState(false, board);
        board.ReturnBoardObject(this);
        Owner.ReleaseBoardObj();

        //사라지기 복원
        _sprite.enabled = true;
    }
}
