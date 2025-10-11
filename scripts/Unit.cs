using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    [Signal]
    public delegate void DiedEventHandler(Unit deadUnit);

    private const string AllyTextureUID = "uid://dhuy3j2atec8i";
    private const string EnemyTextureUID = "uid://dp01wsppv1ocb";


    private Texture2D AllyTexture = ResourceLoader.Load<Texture2D>(AllyTextureUID);
    private Texture2D EnemyTexture = ResourceLoader.Load<Texture2D>(EnemyTextureUID);


    private NavigationAgent2D navigationAgent;
    private Sprite2D sprite;
    private Sprite2D selectionIndicator;
    private Area2D attackRange;
    private CollisionShape2D attackRangecollisionShape;
    private Timer attackInvervalTimer;


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
    private float attackRangeRadius = 0f;
    private float defaultAttackInterval = 1.0f;

    private int health = 100;
    private int attackDamage = 10;

    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = teamID == 1 ? AllyTexture : EnemyTexture;

        selectionIndicator = GetNode<Sprite2D>("SelectionIndicator");

        attackRange = GetNode<Area2D>("AttackRange");
        attackRange.BodyEntered += OnAttackRangeBodyEntered;

        attackRangecollisionShape = attackRange.GetNode<CollisionShape2D>("CollisionShape2D");
        CircleShape2D circleShape = attackRangecollisionShape.Shape as CircleShape2D;
        this.attackRangeRadius = circleShape.Radius;

        attackInvervalTimer = GetNode<Timer>("AttackIntervalTimer");
        attackInvervalTimer.WaitTime = defaultAttackInterval;
        attackInvervalTimer.Timeout += OnAttackIntervalTimeout;

        if (GameManager.Instance != null)
            Died += GameManager.Instance.OnUnitDied;

        // Log.Info($"Unit {this} created at position {GlobalPosition}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (behaviorState == BehaviorState.Idle)
            return;

        if (behaviorState == BehaviorState.Attacking && targetUnit != null)
        {
            if (!IsInstanceValid(targetUnit))
            {
                this.targetUnit = null;
                SetBehaviorState(BehaviorState.Idle);
                return;
            }

            if (GlobalPosition.DistanceTo(targetUnit.GlobalPosition) <= attackRangeRadius)
            {
                Velocity = Vector2.Zero;
                TryAttack();
                return;
            }
            MoveTo(targetUnit.GlobalPosition);
        }

        if (behaviorState == BehaviorState.Moving)
        {
            if (GlobalPosition.DistanceTo(navigationAgent.TargetPosition) < 1f || navigationAgent.IsNavigationFinished())
            {
                SetBehaviorState(BehaviorState.Idle);
                // Log.Info($"Unit {this} from TeamID {this.teamID} entering in Idle state!");
                return;
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

        //Log.Info($"Unit {this} nextPathPosition {nextPathPosition}");

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
        //Log.Info($"Unit {this} navigation position {targetPosition}");
    }
    public void SetTargetUnit(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
        MoveTo(targetUnit.GlobalPosition);
        SetBehaviorState(BehaviorState.Attacking);

        // Log.Info($"Unit {this} set target to {targetUnit}");
    }

    public void SetSelected(bool isSelected)
    {
        selectionIndicator.Visible = isSelected;
    }
    public void TryAttack()
    {
        if (IsInstanceValid(targetUnit))
        {
            targetUnit.ReceiveDamage(attackDamage);
            // Log.Info($"Unit {this} from Team {this.teamID} attacking Unit {targetUnit} from Team {targetUnit.teamID}!");
            attackInvervalTimer.Start();
        }
        else
        {
            this.targetUnit = null;
            SetBehaviorState(BehaviorState.Idle);
        }
    }
    public void ReceiveDamage(int damage)
    {
        health -= damage;
        // Log.Info($"Unit {this} received {damage} damage, health now {health}");
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        // Log.Info($"Unit {this} died.");
        EmitSignal(SignalName.Died, this);
        QueueFree();
    }
    private void OnAttackIntervalTimeout()
    {
        TryAttack();
    }
    private void OnAttackRangeBodyEntered(Node2D body)
    {
        if (body is not Unit unit || unit.teamID == this.teamID)
            return;

        SetTargetUnit(unit);

        // Log.Info($"Unit {this} from Team {this.teamID} detected body {unit} from Team {unit.teamID} in attack range.");
    }

    public Rect2 GetGlobalBounds()
    {
        return GlobalTransform * sprite.GetRect();
    }
}

