using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GemObject_AddTime : GemObjectBase
{
    public override TYPE Type => TYPE.ADD_TIME;

    public override void Explode(GameBoard board)
    {
        transform.DOMove(new Vector3(0f, -5.5f), 0.5f)
            .OnComplete(() => 
            {
                board.AddTime();

                //삭제
                Owner.ReleaseGemObj();
                board.ReturnGemObject();
            });
    }
}
