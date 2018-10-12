using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Research
{
    public class VectorTransform
    {
        public static Vector3 MapVector2Transform(Transform t, Vector3 v)
        {
            return t.position + (v.x * t.right) + (v.y * t.up) + (v.z * t.forward);
        }
    }
}