using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _queue;
    private int _overAllocateCount;
    private T _prefab;
    private Transform _parentObj;
    private string _objName;
    private int _objCnt;

    public ObjectPool(T prefabObj, string objName, Transform parentObj, int count, int overAllocateCount)
    {
        _overAllocateCount = overAllocateCount;
        _objCnt = 0;
        _prefab = prefabObj;
        _parentObj = parentObj;
        _objName = objName;
        _queue = new Queue<T>(count);
        Allocate(count);
    }

    public void Allocate(int alloCount)
    {
        for (int i = 0; i < alloCount; ++i)
        {
            T obj = GameObject.Instantiate<T>(_prefab);
            obj.transform.SetParent(_parentObj);
            obj.name = _objName + _objCnt++.ToString();
            obj.gameObject.SetActive(false);
            _queue.Enqueue(obj);
        }
    }

    public T Pop(bool setActive = true)
    {
        if (_queue.Count <= 0)
        {
            //Debug.Log(typeof(T).ToString() + " ObjPool : Over Count Object Pop");
            Allocate(_overAllocateCount);
        }

        T retObj = _queue.Dequeue();
        retObj.gameObject.SetActive(setActive);
        return retObj;
    }

    public void Push(T obj)
    {
        obj.transform.SetParent(_parentObj);
        obj.gameObject.SetActive(false);
        _queue.Enqueue(obj);
    }
}