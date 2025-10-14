using Godot;
using System;
using System.Dynamic;
using System.Collections.Generic;

[GlobalClass]
public partial class Unit : CharacterBody2D
{
    [Signal]
    public delegate void DiedEventHandler(Unit deadUnit);

    private NavigationAgent2D navigationAgent;
    private Sprite2D sprite;
    private Sprite2D selectionIndicator;
    private Area2D attackRange;
    private CollisionShape2D attackRangecollisionShape;
    private Timer attackInvervalTimer;
    private Unit targetUnit;

    public UnitData Data { get; private set; }
    // public Texture2D Icon => Data?.GetIcon(teamID);
    public Texture2D Icon => sprite?.Texture;
    public string Name => Data?.Name ?? "Unknown";
    public int Cost => Data?.Cost ?? 0;

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

    private int health;
    private int attackDamage;

    private const string TEAM_COLOR_SHADER_PATH = "uid://nihmenvkqgr0";
    private static readonly Color TARGET_COLOR = new Color(0.3882f, 0.6078f, 1.0f, 1.0f);
    private static readonly Color TEAM2_REPLACEMENT_COLOR = new Color(1.0f, 0.2f, 0.2f, 1.0f);
    private const float COLOR_TOLERANCE = 0.75f;
    private ShaderMaterial _teamColorMaterial;

    public override void _Ready()
    {
        navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        sprite = GetNode<Sprite2D>("Sprite2D");
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

        // ApplyTeamShader();
        // if (Data != null)
        // {
        //     // Aplica a textura com base no teamID (se já estiver definido)
        //     sprite.Texture = Data.SpriteTextureByTeam.GetValueOrDefault(teamID);
        // }
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

    public void Initialize(UnitData data, int teamID)
    {
        if (data == null)
        {
            Log.Error("Initialize chamado com UnitData nulo.");
            return;
        }
        this.Data = data;
        this.teamID = teamID;
        health = 100;
        attackDamage = 10;

        Log.Info($"Init Unit {this} of Team {teamID}");

        ApplyTeamShader();
    }


    public void ApplyTeamShader()
    {
        if (sprite == null)
            sprite = GetNode<Sprite2D>("Sprite2D");

        if (teamID == 2)
        {
            Log.Info($"Applying team shader for Unit {this} of Team {teamID}");
            // === TIME VERMELHO: APLICA O SHADER ===

            // 1a. Garante que o material base esteja carregado e instanciado.
            if (_teamColorMaterial == null)
            {
                Log.Info("Loading and creating team color shader material.");

                Shader shader = GD.Load<Shader>(TEAM_COLOR_SHADER_PATH);
                // IMPORTANTE: Criar uma nova instância do material para esta unidade
                _teamColorMaterial = new ShaderMaterial();
                _teamColorMaterial.Shader = shader;

                // 1b. Configura os parâmetros (só precisa ser feito uma vez)
                _teamColorMaterial.SetShaderParameter("target_color", TARGET_COLOR);
                _teamColorMaterial.SetShaderParameter("replacement_color", TEAM2_REPLACEMENT_COLOR);
                _teamColorMaterial.SetShaderParameter("color_tolerance", COLOR_TOLERANCE);
            }

            // 2. Aplica o material ao Sprite2D
            sprite.Material = _teamColorMaterial;
        }
        else
        {
            // === TIME AZUL/PADRÃO: REMOVE O SHADER ===

            // Garante que o sprite NÃO tenha material para usar a renderização padrão.
            sprite.Material = null;
        }
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

