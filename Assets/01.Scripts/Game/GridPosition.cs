using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

[System.Serializable]
public struct GridPosition
{
    public enum DIRECTION
    {
        NONE = -1,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public static GridPosition[] DirectionPoint =
    {
        new GridPosition(-1,0),     //위
        new GridPosition(1,0),    //아래
        new GridPosition(0,-1),    //좌
        new GridPosition(0,1)      //우
    };

    public GridPosition(int r, int c)
    {
        row = r;
        col = c;
    }

    public int row;
    public int col;
    
    public GridPosition Up { get { return this + DirectionPoint[UnsafeUtility.EnumToInt(DIRECTION.UP)]; } }
    public GridPosition Down { get { return this + DirectionPoint[UnsafeUtility.EnumToInt(DIRECTION.DOWN)]; } }
    public GridPosition Left { get { return this + DirectionPoint[UnsafeUtility.EnumToInt(DIRECTION.LEFT)]; } }
    public GridPosition Right { get { return this + DirectionPoint[UnsafeUtility.EnumToInt(DIRECTION.RIGHT)]; } }

    public static GridPosition One { get { return new GridPosition(1, 1); } }

    public override bool Equals(object obj)
    {
        if (!(obj is GridPosition))
        {
            return false;
        }
        
        var point = (GridPosition)obj;
        return row == point.row &&
               col == point.col;
    }
    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + row.GetHashCode();
        hashCode = hashCode * -1521134295 + col.GetHashCode();
        return hashCode;
    }
    public static bool operator ==(GridPosition left, GridPosition right)
    {
        if (left.row != right.row || left.col != right.col)
            return false;

        return true;
    }
    public static bool operator !=(GridPosition left, GridPosition right)
    {
        if (left.row != right.row || left.col != right.col)
            return true;

        return false;
    }
    public static GridPosition operator +(GridPosition left, GridPosition right)
    {
        GridPosition result;
        result.row = left.row + right.row;
        result.col = left.col + right.col;

        return result;
    }
    public static GridPosition operator -(GridPosition left, GridPosition right)
    {
        GridPosition result;
        result.row = left.row - right.row;
        result.col = left.col - right.col;

        return result;
    }
    public DIRECTION ToDirection(GridPosition To)
    {
        GridPosition dirPoint = To - this;
        for (int idx = 0; idx < DirectionPoint.Length; ++idx)
        {
            if (dirPoint == DirectionPoint[idx])
                return (DIRECTION)(idx);
        }

        return DIRECTION.NONE;
    }
    public GridPosition DirectionToPos(DIRECTION dir)
    {
        return this + DirectionPoint[(int)dir];
    }
    public override string ToString()
    {
        return string.Format("(Grid - {0} / {1}", row.ToString("00"), col.ToString("00"));
    }
}