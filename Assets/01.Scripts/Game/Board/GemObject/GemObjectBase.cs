using UnityEngine;
using System.Collections;

public abstract class GemObjectBase : MonoBehaviour
{
    public enum TYPE
    {
        ADD_TIME,
        COLOR_BOMB,
        ENERGY_BOMB,
    }

    protected SpriteRenderer _sprite;

    public abstract TYPE Type { get; }
    public BoardObject_Gem Owner { get; set; }

    new Transform transform;

    protected virtual void Awake()
    {
        transform = GetComponent<Transform>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public abstract void Explode(GameBoard board);

    /// <summary>
    /// 폭발에 일정 시간이 걸리는 오브젝트 상태변환
    /// </summary>
    /// <param name="value"></param>
    /// <param name="board"></param>
    protected void SetExplodingState(bool value, GameBoard board)
    {
        if (value)
            ++board.CurrentExplodingCnt;
        else
            --board.CurrentExplodingCnt;
    }
}
