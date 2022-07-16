using Godot;
using System;

public static class Utils
{
    public static void GlobalPosition(this Spatial spatial, Vector3 position)
    {
        spatial.GlobalTransform = new Transform(spatial.GlobalTransform.basis, position);
    } 
    
    public static Vector2 To2D(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    
    public static Vector3 To3D(this Vector2 vec)
    {
        return new Vector3(vec.x, 0.0f, vec.y);
    }
    
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    
    public static float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
