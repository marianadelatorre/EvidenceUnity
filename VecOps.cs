using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VecOps {
    public static float DotProduct(Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public static Vector3 CrossProduct(Vector3 a, Vector3 b) {
        float x = a.y * b.z - a.z * b.y;
        float y = a.z * b.x - a.x * b.z;
        float z = a.x * b.y - a.y * b.x;
        return new Vector3(x, y, z);
    }

    public static float Magnitude(Vector3 v) {
        return Mathf.Sqrt((v.x * v.x) + (v.y * v.y) + (v.z * v.z));
    }

    public static Vector3 Normalize(Vector3 v) {
        float magnitude = Magnitude(v);
        return new Vector3(v.x / magnitude, v.y / magnitude, v.z / magnitude);
    }

    public static float Angle(Vector3 a, Vector3 b) {

        float dotProduct = DotProduct(a, b);
        float magnitudeA = Magnitude(a);
        float magnitudeB = Magnitude(b);
        float angle = Mathf.Acos(dotProduct / (magnitudeA * magnitudeB));
        return angle;
    }

    public static Matrix4x4 TranslateM(Vector3 t){

        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = t.x;
        m[1, 3] = t.y;
        m[2, 3] = t.z;
        return m;
    }
    public static Matrix4x4 ScaleM(Vector3 s){

        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = s.x;
        m[1, 1] = s.y;
        m[2, 2] = s.z;
        return m;

    }

    public static Matrix4x4 RotateXM(float degrees){   //float rad = degrees * Mathf.Deg2Rad
         float rad = degrees * Mathf.Deg2Rad;
         float sin = Mathf.Sin(rad);
         float cos = Mathf.Cos(rad);
         Matrix4x4 m = Matrix4x4.identity;
         m[1, 1] = cos;
         m[1, 2] = -sin;
         m[2, 1] = sin;
         m[2, 2] = cos;
         return m;
    }

    public static Matrix4x4 RotateYM(float degrees){
         float rad = degrees * Mathf.Deg2Rad;
         float sin = Mathf.Sin(rad);
         float cos = Mathf.Cos(rad);
         Matrix4x4 m = Matrix4x4.identity;
         m[0, 0] = cos;
         m[0, 2] = sin;
         m[2, 0] = -sin;
         m[2, 2] = cos;
         return m;
    }

    public static Matrix4x4 RotateZM(float degrees){
         float rad = degrees * Mathf.Deg2Rad;
         float sin = Mathf.Sin(rad);
         float cos = Mathf.Cos(rad);
         Matrix4x4 m = Matrix4x4.identity;
         m[0, 0] = cos;
         m[0, 1] = -sin;
         m[1, 0] = sin;
         m[1, 1] = cos;
         return m;
    }

    //ApplyTransform
    //Receives: original vertices and transformation matrix
    //Returns: the list of transformed vertices

    public static List<Vector3> ApplyTransform(List<Vector3> originals, Matrix4x4 m){
        
        List<Vector3> result = new List<Vector3>();
        foreach(Vector3 v in originals){
            Vector4 temp = new Vector4(v.x, v.y, v.z, 1);
            result.Add(m * temp);
        }
        return result;
    }
}
