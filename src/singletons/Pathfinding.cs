using Godot;
using System;
using System.Collections.Generic;

public class Pathfinding : Singleton<Pathfinding>
{
    private AStar2D _astar = new AStar2D();

    public override void _Ready()
    {
        base._Ready();

        for (int x = 0; x < Game.Instance.GridSize.x; x++)
        {
            for (int y = 0; y < Game.Instance.GridSize.y; y++)
            {
                int id = GridToID(x, y);
                _astar.AddPoint(id, new Vector2(x, y));
                
                if (x > 0)
                    _astar.ConnectPoints(id, GridToID(x - 1, y));
                if (y > 0)
                    _astar.ConnectPoints(id, GridToID(x, y - 1));
            }
        }
    }

    public Vector2[] Evaluate(Vector2 from, Vector2 to)
    {
        return _astar.GetPointPath(GridToID((int)from.x, (int)from.y), GridToID((int)to.x, (int)to.y));
    }

    private int GridToID(int x, int y)
    {
        return (int) (x + Game.Instance.GridSize.y * y);
    }
}
