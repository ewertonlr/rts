using Godot;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Wasm;

public partial class Building : StaticBody2D
{
    private Timer productionTimer;

    public PackedScene unitScene = GD.Load<PackedScene>("uid://b42ek7rmo23nq");
    // public List<PackedScene> availableUnits = new List<PackedScene>();
    [Export]
    public int[] ProduceableUnitIDs = new int[] { 1 }; // IDs of units this building can produce
    public List<UnitData> AvailableUnits { get; private set; } = new List<UnitData>();

    public Queue<PackedScene> productionQueue = new Queue<PackedScene>();
    public PackedScene currentProduction = null;
    private Sprite2D sprite;
    private Sprite2D selectionIndicator;

    private float productionInterval = 1.0f;
    private int productionQueueLimit = 5;

    public override void _Ready()
    {
        selectionIndicator = GetNode<Sprite2D>("SelectionIndicator");
        sprite = GetNode<Sprite2D>("Sprite2D");

        productionTimer = GetNode<Timer>("ProductionTimer");
        productionTimer.Timeout += OnProductionTimerTimeout;
        productionTimer.WaitTime = productionInterval;

        LoadAvailableUnitData();

        // availableUnits.Add(unitScene);
        // // TODO: Testing Purposes
        // availableUnits.Add(unitScene);
        // availableUnits.Add(unitScene);
        // availableUnits.Add(unitScene);
        // availableUnits.Add(unitScene);
        // availableUnits.Add(unitScene);

        // TODO: Testing Purposes
        // AddToProductionQueue(unitScene);
        // AddToProductionQueue(unitScene);
        // AddToProductionQueue(unitScene);
        // AddToProductionQueue(unitScene);
        // AddToProductionQueue(unitScene);
        // AddToProductionQueue(unitScene);
        // TryStartProduction(); // TEMP FOR TESTING
    }

    private void LoadAvailableUnitData()
    {
        // Limpa a lista antes de preencher
        AvailableUnits.Clear();

        if (GameDataCatalog.Instance == null)
        {
            Log.Error("GameDataCatalog não está pronto!");
            return;
        }

        foreach (int unitId in ProduceableUnitIDs)
        {
            UnitData data = GameDataCatalog.Instance.GetUnitData(unitId);
            if (data != null)
            {
                AvailableUnits.Add(data);
            }
        }
        Log.Info($"Building carregou {AvailableUnits.Count} tipos de unidades para produção.");
    }

    public void AddToProductionQueue(PackedScene unitScene)
    {
        // if (AvailableUnits.Contains(unitScene) == false)
        // {
        //     Log.Info("Unidade não disponível para produção.");
        //     return;
        // }
        if (productionQueue.Count >= productionQueueLimit)
        {
            Log.Info("Fila de produção cheia.");
            return;
        }
        productionQueue.Enqueue(unitScene);
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

    public void ProduceUnit()
    {
        if (productionQueue.Count == 0)
        {
            currentProduction = null;
            Log.Info("UnitToProduce não está definido.");
            return;
        }

        PackedScene unitScene = productionQueue.Dequeue();
        currentProduction = null;

        Unit newUnit = unitScene.Instantiate<Unit>();
        newUnit.Initialize(GameDataCatalog.Instance.GetUnitData(1), 1); // TODO: Set proper teamID and UnitData
        newUnit.GlobalPosition = this.GlobalPosition + new Vector2(50, 0);
        GetParent().AddChild(newUnit);

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
