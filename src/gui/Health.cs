using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GodotOnReady.Attributes;

public class Health : HBoxContainer
{
    private List<Control> _pips = new();
    
    public override void _Ready()
    {
        base._Ready();

        for (int i = 0; i < GetChildCount(); i++)
        {
            _pips.Add(GetNode<Control>($"{i}"));
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        Debug.Assert(_pips.Count >= Dice.Instance.MaxHealth, "_pips.Count >= Dice.Instance.MaxHealth");
        for (int i = 0; i < Dice.Instance.MaxHealth; i++)
        {
            _pips[i].Visible = i < Dice.Instance.Health;
        }
    }
}
