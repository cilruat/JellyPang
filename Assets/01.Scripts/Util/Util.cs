using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Util
{
    public static string StringAppend(params string[] strArr)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        for (int i = 0; i < strArr.Length; ++i)
        {
            if (strArr[i] == "" ||
                strArr[i] == string.Empty ||
                strArr[i] == null)
            {
                continue;
            }

            builder.Append(strArr[i]);
        }

        return builder.ToString();
    }

    public static void DrawHandlesRect(Vector2 center, Vector2 size, Color color)
    {
#if UNITY_EDITOR
        Vector2 extents = size * 0.5f;

        Vector3 topLeft = new Vector3(center.x - extents.x, center.y + extents.y);
        Vector3 topRight = new Vector3(center.x + extents.x, center.y + extents.y);
        Vector3 bottomLeft = new Vector3(center.x - extents.x, center.y - extents.y);
        Vector3 bottomRight = new Vector3(center.x + extents.x, center.y - extents.y);

        Handles.color = color;

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topLeft, bottomLeft);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomLeft, bottomRight);
#endif
    }

    public static void DrawDebugRect(Rect rect)
    {
        Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0f);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0f);
        Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0f);
        Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0f);

        Debug.DrawLine(topLeft, topRight);
        Debug.DrawLine(bottomLeft, bottomRight);
        Debug.DrawLine(topLeft, bottomLeft);
        Debug.DrawLine(topRight, bottomRight);
    }


    public static bool PercentageCheck(int percentage)
    {
        if (percentage == 100)
            return true;

        percentage = 100 - percentage;

        int rand = UnityEngine.Random.Range(0, 100);

        if (percentage <= rand)
            return true;
        else
            return false;
    }

    public static T GetRandomListItem<T>(List<T> list)
    {
        int rand = UnityEngine.Random.Range(0, list.Count);
        return list[rand];
    }

    public static T StringToEnum<T>(string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static void ShuffleList<T>(List<T> list)
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < list.Count; ++index)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            tmp = list[random1];
            list[random1] = list[random2];
            list[random2] = tmp;
        }
    }
    public static T RandomEnumValue<T>(bool hasMinusEnum = false, bool hasTotal = false)
    {
        var arr = Enum.GetValues(typeof(T));
        int lastIndex = arr.Length;

        if (hasMinusEnum)
            --lastIndex;

        if (hasTotal)
            --lastIndex;

        return (T)arr.GetValue(UnityEngine.Random.Range(0, lastIndex));
    }

    public static T RandomListValue<T>(List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T RandomArrValue<T>(T[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    public static T CreateGameObject<T>() where T : MonoBehaviour
    {
        return CreateGameObject<T>(null, Vector3.zero, Quaternion.identity);
    }

    public static T CreateGameObject<T>(Transform parent) where T : MonoBehaviour
    {
        return CreateGameObject<T>(parent, Vector3.zero, Quaternion.identity);
    }

    public static T CreateGameObject<T>(Transform parent, Vector3 pos) where T : MonoBehaviour
    {
        return CreateGameObject<T>(parent, pos, Quaternion.identity);
    }

    public static T CreateGameObject<T>(Transform parent, Vector3 pos, Quaternion rot) where T : MonoBehaviour
    {
        GameObject obj = new GameObject();

        if (parent != null)
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localRotation = rot;
        }

        obj.name = typeof(T).Name;
        return obj.AddComponent<T>();
    }

}