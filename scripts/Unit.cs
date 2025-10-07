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
    private Unit targetUnit;

    public enum BehaviorState
    {
        Idle,
        Moving,
        Attacking
    }
    public BehaviorState behaviorState = BehaviorState.Idle;


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
        if (behaviorState == BehaviorState.Idle)
            return;

        if (GlobalPosition.DistanceTo(navigationAgent.TargetPosition) < 10f || navigationAgent.IsNavigationFinished())
        {
            if (behaviorState == BehaviorState.Moving)
            {
                SetBehaviorState(BehaviorState.Idle);
                Log.Info($"Unit {this} from TeamID {this.teamID} has reached the threshold distance and/or finished its navigation!");
                Log.Info($"Unit {this} from TeamID {this.teamID} entering in Idle state!");
                return;
            }
            else if (behaviorState == BehaviorState.Attacking)
            {
                MoveTo(targetUnit.GlobalPosition);
            }
        }

        ProcessMovement();

    }

    private void ProcessMovement()
    {

        Vector2 nextPathPosition = navigationAgent.GetNextPathPosition();

        Vector2 currentPosition = GlobalPosition;

        if (currentPosition.DistanceTo(nextPathPosition) < 1f)
            return; // Already at the next path position

        // Log.Info($"Unit {this} nextPathPosition {nextPathPosition}");

        Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();
        float speed = 100f; // You can adjust the speed as needed
        Velocity = direction * speed;

        MoveAndSlide();
    }
    public void SetBehaviorState(BehaviorState newState)
    {
        behaviorState = newState;

        if (behaviorState == BehaviorState.Idle)
        {
            Velocity = Vector2.Zero;
            targetUnit = null;
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        navigationAgent.TargetPosition = targetPosition;
        // Log.Info($"Unit {this} navigation position {targetPosition}");
    }

    public void SetTargetUnit(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
        // MoveTo(targetUnit.GlobalPosition);
        SetBehaviorState(BehaviorState.Attacking);

        Log.Info($"Unit {this} set target to {targetUnit}");
    }
    private void OnAttackRangeBodyEntered(Node2D body)
    {
        if (body is not Unit unit || unit.teamID == this.teamID)
            return;

        SetTargetUnit(unit);

        Log.Info($"Unit {this} from Team {this.teamID} detected body {unit} from Team {unit.teamID} in attack range.");
    }
}

