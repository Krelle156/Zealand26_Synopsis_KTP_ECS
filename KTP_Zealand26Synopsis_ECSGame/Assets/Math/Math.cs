using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MyMath
{
    //This class exist solely because I do not know all built in math methods and I believe without proof that there are a few formulas missing.
    public class Math
    {
        public static float2 GetNormal(float2 a, float2 b)
        {
            float2 dir = math.normalize(b - a);
            return new float2(-dir.y, dir.x);
        }

        public static float2 GetProjectionLengthMinMax(FixedList4096Bytes<float2> points, float2 axis)
        {
            float min = float.MaxValue;
            float max = float.MinValue;

                for (int i = 0; i < points.Length; i++)
                {
                float projectionLength = math.dot(points[i], axis) / math.length(axis);
                    if (projectionLength < min)
                    {
                        min = projectionLength;
                    }
                    if (projectionLength > max)
                    {
                        max = projectionLength;
                    }
                }
                return new float2(min, max);
        }

        public static float VectorLength(float2 a)
        {
            return math.sqrt(a.x * a.x + a.y * a.y);
        }

    }

}
