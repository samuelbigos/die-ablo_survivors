using Godot;
using System;
using GodotOnReady.Attributes;

public partial class OneUp : Spatial
{
    [Export] private PackedScene _floatingTextScene;
    
    [OnReadyGet] private MeshInstance _mesh1;
    [OnReadyGet] private MeshInstance _mesh2;

    public int Cost;

    private Vector2 _gridPos;
    private FloatingText _floatingText;

    public void Init(Vector2 gridPos, int cost)
    {
        _gridPos = gridPos;
        GlobalTransform = new Transform(GlobalTransform.basis, _gridPos.To3D());
        
        ShaderMaterial shader1 = (_mesh1.MaterialOverride as ShaderMaterial).Duplicate() as ShaderMaterial;
        shader1.SetShaderParam("u_col", ModManager.Instance.ModColoursSecondary[ModManager.ModTypes.Coin]);
        _mesh1.MaterialOverride = shader1;
        
        SpatialMaterial shader2 = (_mesh2.MaterialOverride as SpatialMaterial).Duplicate() as SpatialMaterial;
        shader2.AlbedoColor = ModManager.Instance.ModColoursSecondary[ModManager.ModTypes.Number];
        _mesh2.MaterialOverride = shader2;
        
        Game.DoTurn += DoTurn;

        _floatingText = _floatingTextScene.Instance<FloatingText>();
        _floatingText.Init(Game.Instance.Camera, this, $"{cost}", ModManager.Instance.ModColours[ModManager.ModTypes.Coin], true);
        Game.Instance.AddChild(_floatingText);

        Cost = cost;
    }

    private void DoTurn()
    {
        if (_gridPos == Dice.Instance.GridPos)
        {
            if (Dice.Instance.OnOneUp(this))
                QueueFree();
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        Game.DoTurn -= DoTurn;
        _floatingText.QueueFree();
    }
}
