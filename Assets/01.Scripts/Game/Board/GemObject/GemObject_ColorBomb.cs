using UnityEngine;
using System.Collections;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class GemObject_ColorBomb : GemObjectBase
{
    public override TYPE Type => TYPE.COLOR_BOMB;

    public override void Explode(GameBoard board)
    {
        board.AfterExplodeFunc = this.ExplodeProc(board);
    }

    IEnumerator ExplodeProc(GameBoard board)
    {
        SetExplodingState(true, board);

        float bombMoveDuration = 0.25f;

        BoardGrid targetGrid = board.GetGrid(Random.Range(0, Define.BOARD_SIZE.row), Random.Range(0, Define.BOARD_SIZE.col));

        transform.DOLocalMove(board.GridPosToWorldPos(targetGrid.Position, false), bombMoveDuration);

        yield return new WaitForSeconds(bombMoveDuration);

        //이펙트
        BoardFx fxExplosion = board.GetBoardFx(BoardFx.TYPE.COLORBOMB_EXPLOSION);
        fxExplosion.transform.position = transform.position;

        //효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_COLORBOMB);

        //진동
        MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        board.transform.DOShakePosition(0.2f, 0.125f, 30);

        //색 변경
        GridPosition[] range = new GridPosition[] { 
            new GridPosition(0, 0), new GridPosition(-2, 0), new GridPosition(-1, -1), new GridPosition(-1, 0), new GridPosition(-1, 1),
            new GridPosition(0, -2), new GridPosition(0, -1), new GridPosition(0, 1), new GridPosition(0, 2), new GridPosition(1, -1),
            new GridPosition(1, 0), new GridPosition(1, 1), new GridPosition(2, 0)};

        for(int idx = 0; idx < range.Length; ++idx)
        {
            BoardGrid grid = board.GetGrid(targetGrid.Position.row + range[idx].row, targetGrid.Position.col + range[idx].col);
            
            if (grid == null)
                continue;

            if (grid.IsEmpty || grid.BoardObject.Type != BoardObjectBase.TYPE.GEM)
                continue;

            BoardObject_Gem gem = grid.BoardObject as BoardObject_Gem;

            gem.ChangeColor(Owner.Color);
        }

        SetExplodingState(false, board);

        //삭제
        Owner.ReleaseGemObj();
        board.ReturnGemObject();
    }
}
