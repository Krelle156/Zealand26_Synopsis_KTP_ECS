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

        if (polygonCollider.shapes == null || polygonCollider.shapes[0].points.Length < 3)
        {
            polygonCollider.shapes = new simpleShape[1];
            polygonCollider.shapes[0] = new simpleShape { points = new float2[3] };
            polygonCollider.shapes[0].points[0] = new float2(0, 0.5f);
            polygonCollider.shapes[0].points[1] = new float2(-0.5f, -0.5f);
            polygonCollider.shapes[0].points[2] = new float2(0.5f, -0.5f);
        }

        Vector2 offSet = polygonCollider.transform.position;

        Handles.color = Color.red;
        for(int shapeIndex = 0; shapeIndex < polygonCollider.shapes.Length; shapeIndex++)
        {
            for (int i = 0; i < polygonCollider.shapes[shapeIndex].points.Length; i++)
            {
                Vector2 currentPoint2D = polygonCollider.shapes[shapeIndex].points[i];

                EditorGUI.BeginChangeCheck();
                Vector2 new2DPoint = (Vector2)Handles.FreeMoveHandle(currentPoint2D + offSet, handleSize, snap, Handles.DotHandleCap) - offSet;
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(polygonCollider, "Move Point");
                    polygonCollider.shapes[shapeIndex].points[i] = new2DPoint;
                }
            }
            for (int i = 0; i < polygonCollider.shapes[shapeIndex].points.Length; i++)
            {
                Vector2 currentPoint2D = polygonCollider.shapes[shapeIndex].points[i];
                Vector2 nextPoint2D = polygonCollider.shapes[shapeIndex].points[0];
                if (!(i + 1 == polygonCollider.shapes[shapeIndex].points.Length)) nextPoint2D = polygonCollider.shapes[shapeIndex].points[i + 1];


                Handles.DrawLine(currentPoint2D + offSet, nextPoint2D + offSet);
            }
        } 


    }
}
