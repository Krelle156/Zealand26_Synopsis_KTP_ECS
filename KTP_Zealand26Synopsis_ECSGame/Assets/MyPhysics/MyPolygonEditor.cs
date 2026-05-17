using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

[CustomEditor(typeof(MyPolygonColliderAuthoring))]
public class MyPolygonEditor : Editor
{
    public float handleSize = 0.04f;
    public Vector3 snap = Vector3.one * 0.5f;


    private void OnSceneGUI()
    {
        //Handles are awesome!
        //https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Handles.FreeMoveHandle.html
        MyPolygonColliderAuthoring polygonCollider = target as MyPolygonColliderAuthoring;


        if(polygonCollider.points == null || polygonCollider.points.Length < 3)
        {
            polygonCollider.points = new float2[3];
            polygonCollider.points[0] = new float2(0, 0.5f);
            polygonCollider.points[1] = new float2(-0.5f, -0.5f);
            polygonCollider.points[2] = new float2(0.5f, -0.5f);
        }

        Vector2 offSet = polygonCollider.transform.position;

        Handles.color = Color.red;
        for (int i = 0; i < polygonCollider.points.Length; i++)
        {
            Vector2 currentPoint2D = polygonCollider.points[i];

            EditorGUI.BeginChangeCheck();
            Vector2 new2DPoint = (Vector2)Handles.FreeMoveHandle(currentPoint2D + offSet, handleSize, snap, Handles.DotHandleCap) - offSet;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(polygonCollider, "Move Point");
                polygonCollider.points[i] = new2DPoint;
            }
        }

        for (int i = 0; i < polygonCollider.points.Length; i++)
        {
            Vector2 currentPoint2D = polygonCollider.points[i];
            Vector2 nextPoint2D = polygonCollider.points[0];
            if (!(i + 1 == polygonCollider.points.Length)) nextPoint2D = polygonCollider.points[i + 1];



            Handles.DrawLine(currentPoint2D + offSet, nextPoint2D + offSet);
        }
    }
}
