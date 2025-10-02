using Godot;
using System;

public partial class Main : Node2D
{
    private bool _debug = true;

    public override void _Ready()
    {
        if (_debug) Log.Debug("Log Test.");
    }
}