using Godot;
using System;
using System.Diagnostics;

public class Resources : Singleton<Resources>
{
    [Export] public Material FlashMaterial;
    [Export] public Material FreezeMaterial;
    [Export] public PackedScene LightningVFX;
}
