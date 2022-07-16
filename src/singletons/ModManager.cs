using Godot;
using System;
using System.Collections.Generic;

public class ModManager : Singleton<ModManager>
{
    public enum ModTypes
    {
        Number,
        Bullet,
        Laser,
        Health,
        Coin,
        Freeze,
        Explode,
        COUNT,
    }

    [Export] private Color _numberColour;
    [Export] private Color _bulletColour;
    [Export] private Color _laserColour;
    [Export] private Color _healthColour;
    [Export] private Color _coinColour;
    [Export] private Color _freezeColour;
    [Export] private Color _explodeColour;
    
    [Export] private PackedScene _bulletScene;
    [Export] private PackedScene _laserScene;
    [Export] private PackedScene _healthScene;
    [Export] private PackedScene _coinScene;
    [Export] private PackedScene _freezeScene;
    [Export] private PackedScene _explodeScene;

    [Export] public PackedScene BulletParticles;

    public Dictionary<ModTypes, Color> ModColours = new Dictionary<ModTypes, Color>();
    public Dictionary<ModTypes, PackedScene> ModPickupScenes = new Dictionary<ModTypes, PackedScene>();

    public override void _Ready()
    {
        base._Ready();

        ModColours[ModTypes.Number] = _numberColour;
        ModColours[ModTypes.Bullet] = _bulletColour;
        ModColours[ModTypes.Laser] = _laserColour;
        ModColours[ModTypes.Health] = _healthColour;
        ModColours[ModTypes.Coin] = _coinColour;
        ModColours[ModTypes.Freeze] = _freezeColour;
        ModColours[ModTypes.Explode] = _explodeColour;
        
        ModPickupScenes[ModTypes.Bullet] = _bulletScene;
        ModPickupScenes[ModTypes.Laser] = _laserScene;
        ModPickupScenes[ModTypes.Health] = _healthScene;
        ModPickupScenes[ModTypes.Coin] = _coinScene;
        ModPickupScenes[ModTypes.Freeze] = _freezeScene;
        ModPickupScenes[ModTypes.Explode] = _explodeScene;
    }
}
