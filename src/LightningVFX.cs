using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class LightningVFX : MeshInstance
{
    public GridObject P1;
    public GridObject P2;
    
    private Vector3[] _vertList = new Vector3[1000];
    private Color[] _colList = new Color[1000];
    private int[] _indexList = new int[2000];

    private float _timer = 0.5f;
    private Vector3 _p1Cached;
    private Vector3 _p2Cached;
    private OpenSimplexNoise _noise = new OpenSimplexNoise();

    public override void _Ready()
    {
        base._Ready();

        _noise.Seed = DateTime.Now.Millisecond;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        ArrayMesh outMesh = new();
        
        int v = 0;
        int i = 0;

        if (IsInstanceValid(P1))
            _p1Cached = P1.GlobalTransform.origin;
        if (IsInstanceValid(P2))
            _p2Cached = P2.GlobalTransform.origin;

        Vector2 p1 = _p1Cached.To2D();
        Vector2 p2 = _p2Cached.To2D();

        Vector2 p1p2 = (p2 - p1).Normalized();
        Vector2 tangent = new Vector2(p1p2.y, -p1p2.x).Normalized();

        float length = (p2 - p1).Length();
        float segLen = 0.5f;
        int pointNum = Mathf.Max(2, (int)(length / segLen));
        float intensity = 1.0f - Mathf.Clamp(length / 150.0f, 0.0f, 1.0f);

        for (int line = 0; line < 5; line++)
        {
            List<Vector2> points = new();
            for (int j = 0; j < pointNum + 1; j++)
            {
                Vector2 point = p1 + (p2 - p1) * j / pointNum;
                float t = (float)j / pointNum + line * pointNum;
                float noise = _noise.GetNoise2d(_timer * 50.0f, t * 50.0f) * 1.5f;
                point += tangent * noise * Mathf.Sin(_timer + t * Mathf.Pi * 20.0f * (line / 10.0f)) * segLen * intensity * 3.0f;
                points.Add(point);
            }

            for (int p = 0; p < points.Count - 1; p++)
            {
                Vector2 point = points[p];
                Line(points[p].To3D(), points[p + 1].To3D(), Colors.White, ref v, ref i, _vertList, _colList, _indexList);
            }
        }

        Debug.Assert(v < _vertList.Length, "v < _vertList.Length");
        Debug.Assert(v < _colList.Length, "v < _colList.Length");
        Debug.Assert(i < _indexList.Length, "i < _indexList.Length");

        Span<Vector3> verts = _vertList.AsSpan(0, v);
        Span<Color> colours = _colList.AsSpan(0, v);
        Span<int> indices = _indexList.AsSpan(0, i);

        Godot.Collections.Array arrays = new();
        arrays.Resize((int) ArrayMesh.ArrayType.Max);
        arrays[(int) ArrayMesh.ArrayType.Vertex] = verts.ToArray();
        arrays[(int) ArrayMesh.ArrayType.Color] = colours.ToArray();
        arrays[(int) ArrayMesh.ArrayType.Index] = indices.ToArray();

        outMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

        Mesh = outMesh;

        _timer -= delta;
        if (_timer < 0.0f)
            QueueFree();
    }

    public static void Line(Vector3 p1, Vector3 p2, Color col, ref int v, ref int i, Vector3[] verts, Color[] cols, int[] indices)
    {
        cols[v] = col;
        verts[v] = p1;
        indices[i++] = v++;
        cols[v] = col;
        verts[v] = p2;
        indices[i++] = v++;
    }
}
