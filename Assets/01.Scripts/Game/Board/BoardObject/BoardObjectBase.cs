using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardObjectBase : MonoBehaviour
{
    public enum TYPE
    {
        GEM,
        DIAMOND,
    };

    public enum COLOR
    {
        NONE = -1,

        RED,
        YELLOW,
        BLUE,
        GREEN,
        PURPLE,

        TOTAL            
    };

    protected SpriteRenderer _sprite;
    protected COLOR _color = 0;
    protected bool _isExploding = false;

    public abstract TYPE Type { get; }
    public COLOR Color => _color;
    public bool IsExploding => _isExploding;
    public BoardGrid Owner { get; set; }
    public abstract bool IsExplodable { get; }          //터지는 오브젝트인가
    public abstract bool IsSplashable { get; }          //스플래시에 영향을 받는 오브젝트인가
    public abstract bool IsFallable { get; }            //내려가는 오브젝트인가
    public abstract bool IsSpecialObject { get; }       //매치를 통해 제작되는 특수 오브젝트인가 

    new Transform transform;

    protected virtual void Awake()
    {
        transform = GetComponent<Transform>();

        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Set(COLOR color)
    {
        _color = color;
    }

    public abstract void Explode(GameBoard board);

    /// <summary>
    /// 폭발에 일정 시간이 걸리는 오브젝트 상태변환
    /// </summary>
    /// <param name="value"></param>
    /// <param name="board"></param>
    protected void SetExplodingState(bool value, GameBoard board)
    {
        _isExploding = value;

        if (_isExploding)
            ++board.CurrentExplodingCnt;
        else
            --board.CurrentExplodingCnt;
    }
}
