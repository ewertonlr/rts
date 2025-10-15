using Godot;
using Godot.Collections;
using System.Collections.Generic;
// using System.Numerics;
using System.Runtime.Intrinsics.Wasm;

public partial class Building : StaticBody2D
{
    private Timer productionTimer;

    public PackedScene unitScene = GD.Load<PackedScene>("uid://b42ek7rmo23nq");

    [Export]
    public int[] ProduceableUnitIDs = new int[] { 1 }; // IDs of units this building can produce

    [Export]
    public Array<UnitData> AvailableUnits { get; set; } = new Array<UnitData>();

    public Queue<UnitData> productionQueue = new Queue<UnitData>();
    public UnitData currentProduction = null;
    private Sprite2D sprite;
    private Sprite2D selectionIndicator;

    private float productionInterval = 1.0f;
    private int productionQueueLimit = 5;
    private Vector2 rallyPoint = Vector2.Zero;

    public override void _Ready()
    {
        selectionIndicator = GetNode<Sprite2D>("SelectionIndicator");
        sprite = GetNode<Sprite2D>("Sprite2D");

        productionTimer = GetNode<Timer>("ProductionTimer");
        productionTimer.Timeout += OnProductionTimerTimeout;
        productionTimer.WaitTime = productionInterval;

        rallyPoint = GlobalPosition + new Vector2(0, 200);
    }

    public void AddToProductionQueue(UnitData unitData)
    {
        if (productionQueue.Count >= productionQueueLimit)
        {
            Log.Info("Fila de produção cheia.");
            return;
        }
        productionQueue.Enqueue(unitData);
        Log.Info($"Unidade adicionada à fila de produção. Total na fila: {productionQueue.Count}");

        TryStartProduction();
    }

    public void TryStartProduction()
    {
        if (productionQueue.Count > 0 && productionTimer.IsStopped())
        {
            currentProduction = productionQueue.Peek();
            Log.Info($"Iniciando produção de: {currentProduction.ResourcePath}");
            productionTimer.Start();
        }
    }

    public async void ProduceUnit()
    {
        if (productionQueue.Count == 0)
        {
            currentProduction = null;
            Log.Info("UnitToProduce não está definido.");
            return;
        }

        UnitData unitData = productionQueue.Dequeue();
        currentProduction = null;

        Unit newUnit = unitData.UnitScene.Instantiate<Unit>();
        GetParent().AddChild(newUnit);
        newUnit.GlobalPosition = this.GlobalPosition;
        newUnit.Initialize(unitData, 1, rallyPoint + new Vector2(-200, 0));

        // TODO Additional unit from other team for testing
        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

        Unit newEnemyUnit = unitData.UnitScene.Instantiate<Unit>();
        GetParent().AddChild(newEnemyUnit);
        newEnemyUnit.GlobalPosition = this.GlobalPosition;
        newEnemyUnit.Initialize(unitData, 2, rallyPoint + new Vector2(200, 0));

        // newUnit.Initialize(GameDataCatalog.Instance.GetUnitData(1), 1);

        TryStartProduction();
    }

    public void FinishProductionAndCheckQueue()
    {
        if (productionQueue.Count > 0)
        {
            ProduceUnit();
        }
    }

    public void SetSelected(bool isSelected)
    {
        selectionIndicator.Visible = isSelected;
    }
    public Rect2 GetGlobalBounds()
    {
        return GlobalTransform * sprite.GetRect();
    }
    public void OnProductionTimerTimeout()
    {
        Log.Info("Unidade produzida!");
        FinishProductionAndCheckQueue();
    }
}
