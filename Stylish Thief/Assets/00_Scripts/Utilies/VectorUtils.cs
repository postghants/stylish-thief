using UnityEngine;

public static class VectorUtils
{
    public static Vector3 HorizontalizeV3(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public static Vector2 HorizontalizeV2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
