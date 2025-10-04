using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    private NavigationAgent2D navigationAgent;

    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
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

        Log.Info($"Unit {this} nextPathPosition {nextPathPosition}");

        Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
        float speed = 100f; // You can adjust the speed as needed
        Velocity = direction * speed;

        MoveAndSlide();

        Log.Info($"Unit {this} moving to {direction} at position {GlobalPosition}");


    }

    // public Unit()
    // {

    // }

    // public Unit(Vector2 initialPosition)
    // {
    //     GlobalPosition = initialPosition;
    // }

    public void MoveTo(Vector2 targetPosition)
    {
        navigationAgent.TargetPosition = targetPosition;
        // Log.Info($"Unit {this} navigation position {targetPosition}");
    }
}
