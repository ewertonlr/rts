using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    private NavigationAgent2D navigationAgent;

    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (navigationAgent.IsNavigationFinished())
            return;

        Vector2 nextPathPosition = navigationAgent.GetNextPathPosition();
        Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
        float speed = 100f; // You can adjust the speed as needed
        Velocity = direction * speed;

        MoveAndSlide();

        if (GlobalPosition.DistanceTo(nextPathPosition) < 5f) // Threshold to consider reaching the point
        {
            navigationAgent.Advance();
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        navigationAgent.TargetPosition = targetPosition;
    }
}
