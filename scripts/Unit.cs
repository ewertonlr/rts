using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    private const string AllyTextureUID = "uid://dhuy3j2atec8i";
    private const string EnemyTextureUID = "uid://dp01wsppv1ocb";


    private Texture2D AllyTexture = ResourceLoader.Load<Texture2D>(AllyTextureUID);
    private Texture2D EnemyTexture = ResourceLoader.Load<Texture2D>(EnemyTextureUID);


    private NavigationAgent2D navigationAgent;
    private Sprite2D sprite;
    private Area2D attackRange;


    [Export(PropertyHint.Range, "1,2")]
    public int teamID = 1;


    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = teamID == 1 ? AllyTexture : EnemyTexture;

        attackRange = GetNode<Area2D>("AttackRange");
        attackRange.BodyEntered += OnAttackRangeBodyEntered;

        Log.Info($"Unit {this} created at position {GlobalPosition}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GlobalPosition.DistanceTo(navigationAgent.TargetPosition) < 10f) // Threshold to consider reaching the point
        {
            Velocity = Vector2.Zero;
            return;
        }

        if (navigationAgent.IsNavigationFinished())
            return;

        Vector2 nextPathPosition = navigationAgent.GetNextPathPosition();

        Vector2 currentPosition = GlobalPosition;

        if (currentPosition.DistanceTo(nextPathPosition) < 1f)
            return; // Already at the next path position

        // Log.Info($"Unit {this} nextPathPosition {nextPathPosition}");

        Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
        float speed = 100f; // You can adjust the speed as needed
        Velocity = direction * speed;

        MoveAndSlide();

        // Log.Info($"Unit {this} moving to {direction} at position {GlobalPosition}");


    }

    public void MoveTo(Vector2 targetPosition)
    {
        navigationAgent.TargetPosition = targetPosition;
        // Log.Info($"Unit {this} navigation position {targetPosition}");
    }

    private void OnAttackRangeBodyEntered(Node2D body)
    {
        // Log.Info($"Unit {this} from Team {this.teamID} detected body {body} in attack range.");
        if (body is not Unit unit || unit.teamID == this.teamID)
            return;

        Log.Info($"Unit {this} from Team {this.teamID} detected body {unit} from Team {unit.teamID} in attack range.");
    }
}

