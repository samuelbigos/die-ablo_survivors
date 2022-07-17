using Godot;
using System;
using System.Collections.Generic;

public class ModManager : Singleton<ModManager>
{
    public enum ModTypes
    {
        Number,
        Bullet,
        Heal,
        Lightning,
        Coin,
        Freeze,
        //Explode,
        COUNT,
    }

    [Export] private Color _numberColour;
    [Export] private Color _numberColourSecondary;
    [Export] private Color _bulletColour;
    [Export] private Color _bulletColourSecondary;
    [Export] private Color _healthColour;
    [Export] private Color _healthColourSecondary;
    [Export] private Color _lightningColour;
    [Export] private Color _lightningColourSecondary;
    [Export] private Color _coinColour;
    [Export] private Color _coinColourSecondary;
    [Export] private Color _freezeColour;
    [Export] private Color _freezeColourSecondary;
    // [Export] private Color _explodeColour;
    
    [Export] private PackedScene _bulletScene;
    [Export] private PackedScene _healthScene;
    [Export] private PackedScene _lightningScene;
    [Export] private PackedScene _coinScene;
    [Export] private PackedScene _freezeScene;
    // [Export] private PackedScene _explodeScene;

    [Export] public PackedScene BulletParticles;

    public Dictionary<ModTypes, Color> ModColours = new Dictionary<ModTypes, Color>();
    public Dictionary<ModTypes, Color> ModColoursSecondary = new Dictionary<ModTypes, Color>();
    public Dictionary<ModTypes, PackedScene> ModPickupScenes = new Dictionary<ModTypes, PackedScene>();

    public override void _Ready()
    {
        base._Ready();

        ModColours[ModTypes.Number] = _numberColour;
        ModColoursSecondary[ModTypes.Number] = _numberColourSecondary;
        ModColours[ModTypes.Bullet] = _bulletColour;
        ModColoursSecondary[ModTypes.Bullet] = _bulletColourSecondary;
        ModColours[ModTypes.Heal] = _healthColour;
        ModColoursSecondary[ModTypes.Heal] = _healthColourSecondary;
        ModColours[ModTypes.Lightning] = _lightningColour;
        ModColoursSecondary[ModTypes.Lightning] = _lightningColourSecondary;
        ModColours[ModTypes.Coin] = _coinColour;
        ModColoursSecondary[ModTypes.Coin] = _coinColourSecondary;
        ModColours[ModTypes.Freeze] = _freezeColour;
        ModColoursSecondary[ModTypes.Freeze] = _freezeColourSecondary;
        // ModColours[ModTypes.Explode] = _explodeColour;
        
        ModPickupScenes[ModTypes.Bullet] = _bulletScene;
        ModPickupScenes[ModTypes.Heal] = _healthScene;
        ModPickupScenes[ModTypes.Lightning] = _lightningScene;
        ModPickupScenes[ModTypes.Coin] = _coinScene;
        ModPickupScenes[ModTypes.Freeze] = _freezeScene;
        // ModPickupScenes[ModTypes.Explode] = _explodeScene;
    }
}
