using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    private NavigationAgent2D navigationAgent;

    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }
}
