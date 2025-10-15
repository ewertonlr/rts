using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export]
    public string Id { get; set; }

    [Export]
    public string Name { get; set; } = "New Unit";

    [Export]
    public int Cost { get; set; } = 50;

    [Export]
    public float DefaultAttackInterval { get; set; } = 1.0f;

    [Export]
    public int Health { get; set; } = 100;

    [Export]
    public int AttackDamage { get; set; } = 10;

    [Export]
    public float MovementSpeed { get; set; } = 100f;

    [Export]
    public Texture2D DefaultIcon { get; set; }

    [Export]
    public PackedScene UnitScene { get; set; }
}