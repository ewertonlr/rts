using Godot;
using System.Collections.Generic;

public partial class Building : StaticBody2D
{
    private Timer productionTimer;

    public PackedScene unitScene = GD.Load<PackedScene>("uid://b42ek7rmo23nq");

    public Queue<PackedScene> productionQueue = new Queue<PackedScene>();
    public PackedScene currentProduction = null;

    private float productionInterval = 1.0f;
    private int productionQueueLimit = 5;

    public override void _Ready()
    {
        productionTimer = GetNode<Timer>("ProductionTimer");
        productionTimer.Timeout += OnProductionTimerTimeout;
        productionTimer.WaitTime = productionInterval;

        // TODO: Testing Purposes
        AddToProductionQueue(unitScene);
        AddToProductionQueue(unitScene);
        AddToProductionQueue(unitScene);
        AddToProductionQueue(unitScene);
        AddToProductionQueue(unitScene);
        AddToProductionQueue(unitScene);
        // TryStartProduction(); // TEMP FOR TESTING
    }

    public void AddToProductionQueue(PackedScene unitScene)
    {
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
    public void OnProductionTimerTimeout()
    {
        Log.Info("Unidade produzida!");
        FinishProductionAndCheckQueue();
    }
}
