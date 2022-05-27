using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using MoreMountains.NiceVibrations;

public class GemObject_EnergyBomb : GemObjectBase
{
    public override TYPE Type => TYPE.ENERGY_BOMB;

    public override void Explode(GameBoard board)
    {
        board.AfterExplodeFunc = this.ExplodeProc(board);
    }

    IEnumerator ExplodeProc(GameBoard board)
    {
        SetExplodingState(true, board);

        float animationDuration = 0.5f;

        transform.position = board.transform.position;
        transform.DOScale(2.5f, animationDuration);
        transform.DOShakePosition(animationDuration, 0.35f, 100)
            .OnComplete(()=> 
            {
                //사라지기 
                _sprite.enabled = false;
                transform.localScale = Vector3.one;
            });

        //효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_ENERGYBOMB);

        yield return new WaitForSeconds(animationDuration);

        //이펙트
        BoardFx fxExplosion = board.GetBoardFx(BoardFx.TYPE.ENERGYBOMB_EXPLOSION);
        fxExplosion.transform.position = board.transform.position;

        //효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_COLORBOMB);

        //진동
        MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        board.transform.DOShakePosition(0.2f, 0.125f, 30);

        //색 변경
        int[,] pic1 = {
            {1, 1, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 1},
            {1, 2, 3, 3, 3, 2, 1},
            {1, 2, 3, 4, 3, 2, 1},
            {1, 2, 3, 4, 3, 2, 1},
            {1, 2, 3, 3, 3, 2, 1},
            {1, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 1, 1}
        };

        int[,] pic2 = {
            {3, 3, 3, 2, 3, 3, 3},
            {3, 3, 2, 2, 2, 3, 3},
            {3, 2, 2, 2, 2, 2, 3},
            {2, 2, 2, 2, 2, 2, 2},
            {2, 2, 2, 1, 2, 2, 2},
            {2, 2, 1, 1, 1, 2, 2},
            {2, 1, 1, 1, 1, 1, 2},
            {1, 1, 1, 1, 1, 1, 1}
        };

        int[,] pic3 = {
            {2, 2, 2, 1, 2, 2, 2},
            {2, 2, 1, 2, 1, 2, 2},
            {2, 2, 1, 2, 1, 2, 2},
            {2, 2, 1, 2, 1, 2, 2},
            {2, 2, 2, 1, 2, 2, 2},
            {2, 2, 1, 1, 2, 2, 2},
            {2, 2, 2, 1, 2, 2, 2},
            {2, 2, 1, 1, 2, 2, 2}
        };

        List<int[,]> picList = new List<int[,]>();
        picList.Add(pic1);
        picList.Add(pic2);
        picList.Add(pic3);

        List<BoardObjectBase.COLOR> colorList = new List<BoardObjectBase.COLOR>();
        int colorTotalCnt = UnsafeUtility.EnumToInt(BoardObjectBase.COLOR.TOTAL);
        for (int idx = 0; idx < colorTotalCnt; ++idx)
            colorList.Add((BoardObjectBase.COLOR)idx);
        Util.ShuffleList(colorList);

        int[,] pic = Util.RandomListValue(picList);

        for(int row = 0; row < Define.BOARD_SIZE.row; ++row)
        {
            for(int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                BoardGrid grid = board.GetGrid(row, col);
                
                if (grid.IsEmpty || grid.BoardObject.Type != BoardObjectBase.TYPE.GEM)
                    continue;

                BoardObject_Gem gem = grid.BoardObject as BoardObject_Gem;

                gem.ChangeColor(colorList[pic[row,col]]);
            }
        }

        SetExplodingState(false, board);

        //삭제
        Owner.ReleaseGemObj();
        board.ReturnGemObject();

        //사라지기 복원
        _sprite.enabled = true;
    }
}
