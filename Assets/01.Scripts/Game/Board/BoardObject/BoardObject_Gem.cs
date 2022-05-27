using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class BoardObject_Gem : BoardObjectBase
{
    public Sprite[] GemSprites;

    private GemObjectBase _gemObj = null;

    public override TYPE Type => TYPE.GEM;
    public override bool IsExplodable => true;
    public override bool IsSplashable => true;

    public override bool IsFallable => true;
    public override bool IsSpecialObject => false;

    public override void Set(COLOR color)
    {
        base.Set(color);

        _sprite.sprite = GemSprites[UnsafeUtility.EnumToInt(color)];
    }

    public void ChangeColor(COLOR color)
    {
        if (_color == color)
            return;

        _color = color;
        _sprite.sprite = GemSprites[UnsafeUtility.EnumToInt(color)];
    }

    public override void Explode(GameBoard board)
    {
        //젬오브젝트
        if (_gemObj != null)
        {
            _gemObj.transform.SetParent(null);
            _gemObj.Explode(board);
        }            

        //이펙트
        BoardFx fx = null;
        if (board.IsFever)
            fx = board.GetBoardFx(BoardFx.TYPE.FEVER_EXPLOSION);
        else
            fx = board.GetBoardFx(BoardFx.TYPE.GEM_EXPLOSION);

        fx.transform.position = transform.position;

        //점수 추가
        board.AddScore();

        //삭제
        board.ReturnBoardObject(this);
        Owner.ReleaseBoardObj();
    }

    public void SetGemObj(GemObjectBase obj)
    {
        _gemObj = obj;        
        _gemObj.Owner = this;

        _gemObj.transform.SetParent(transform);
        _gemObj.transform.position = transform.position;
    }

    public void ReleaseGemObj()
    {
        _gemObj.Owner = null;
        _gemObj = null;
    }
}
