using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class SelectionManager : Control
{
    [Signal]
    public delegate void BuildingSelectedEventHandler(Building selectedBuilding);
    [Signal]
    public delegate void BuildingUnselectedEventHandler();
    private BuildingManagerUi buildingManagerUi;
    private bool isDragging = false;
    private Vector2 startDraggingPosition;
    private ColorRect selectionRect;

    // TODO: Temporary test for unit selection
    public List<Unit> PlacedUnits { get; private set; } = new List<Unit>();
    public List<Building> PlacedBuildings { get; private set; } = new List<Building>();
    public Building ghostBuilding = null;

    public override void _Ready()
    {
        buildingManagerUi = GetNode<BuildingManagerUi>("%BuildingManagerUI");

        if (buildingManagerUi != null)
        {
            BuildingSelected += buildingManagerUi.OnBuildingSelected;
            BuildingUnselected += buildingManagerUi.OnBuildingUnselected;
        }

        Log.Info($"SelectionManager is Ready with {PlacedUnits.Count} units placed.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (ghostBuilding != null)
        {
            ghostBuilding.GlobalPosition = GetGlobalMousePosition();
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && !keyEvent.Pressed && keyEvent.AsTextPhysicalKeycode() == "B")
        {
            // Godot.Vector2 mousePosition = GetGlobalMousePosition();
            Log.Warn($"Place Building!");
            // Log.Warn($"Mouse at => {mousePosition}");

            // GameManager.Instance.SetGhostBuilding(mousePosition);
            ghostBuilding = GameManager.Instance.GetGhostBuilding();
            AddChild(ghostBuilding);

        }

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                if (ghostBuilding != null)
                    return;

                startDraggingPosition = GetGlobalMousePosition();
                isDragging = true;
                selectionRect = new ColorRect();
                selectionRect.Color = new Color(Colors.Blue, 0.4f);
                AddChild(selectionRect);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
            {
                if (ghostBuilding != null)
                {
                    ghostBuilding = null;
                    return;
                }

                Control hoveredControl = GetViewport().GuiGetHoveredControl();

                if (hoveredControl == null)
                {
                    GameManager.Instance.RemoveAllUnits();
                    EmitSignal(SignalName.BuildingUnselected);
                }

                Vector2 finalPosition = GetGlobalMousePosition();
                Vector2 size = finalPosition - startDraggingPosition;
                Rect2 finalRect = new Rect2(startDraggingPosition, size).Abs();

                PlacedUnits = GetAllUnitsFromGroup();

                PlacedUnits.ForEach(unit =>
                {
                    Rect2 unitGlobalRect = unit.GetGlobalBounds();

                    if (finalRect.Intersects(unitGlobalRect))
                    {
                        GameManager.Instance.AddUnit(unit);
                        unit.SetSelected(true);
                    }
                });

                if (GameManager.Instance.AllUnits.Count == 0)
                {
                    PlacedBuildings = GetAllBuildingsFromGroup();

                    PlacedBuildings.ForEach(building =>
                    {
                        Rect2 buildingGlobalRect = building.GetGlobalBounds();

                        if (finalRect.Intersects(buildingGlobalRect))
                        {
                            GameManager.Instance.SelectBuilding(building);
                            building.SetSelected(true);
                            EmitSignal(SignalName.BuildingSelected, building);
                        }
                    });
                }

                isDragging = false;
                selectionRect?.QueueFree();
                selectionRect = null;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
            {
                Vector2 targetPosition = GetGlobalMousePosition();
                float spacing = 75.0f;

                for (int i = 0; i < GameManager.Instance.AllUnits.Count; i++)
                {
                    Unit unit = GameManager.Instance.AllUnits[i];

                    // Assuming 5 units per row
                    int row = i / 5;
                    int col = i % 5;
                    Vector2 gridOffset = new Vector2(col * spacing, row * spacing);

                    Vector2 moveToPosition = targetPosition + gridOffset;

                    unit.MoveTo(moveToPosition);
                    unit.SetBehaviorState(Unit.BehaviorState.Moving);
                }
            }
        }
        else if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (isDragging)
            {
                Vector2 currentMousePosition = GetGlobalMousePosition();

                Vector2 minPos = startDraggingPosition.Min(currentMousePosition);

                selectionRect.GlobalPosition = minPos;
                selectionRect.Size = (currentMousePosition - startDraggingPosition).Abs();
            }
        }
    }

    public List<Unit> GetAllUnitsFromGroup(string groupName = "units")
    {
        var nodesInGroup = GetTree().GetNodesInGroup(groupName);

        List<Unit> allUnits = nodesInGroup.OfType<Unit>().ToList();

        return allUnits;
    }

    public List<Building> GetAllBuildingsFromGroup(string groupName = "buildings")
    {
        var nodesInGroup = GetTree().GetNodesInGroup(groupName);

        List<Building> allBuildings = nodesInGroup.OfType<Building>().ToList();

        return allBuildings;
    }

}
