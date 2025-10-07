using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class SelectionManager : Control
{
    private bool isDragging = false;
    private Vector2 startDraggingPosition;
    private ColorRect selectionRect;

    // TODO: Temporary test for unit selection
    public List<Unit> PlacedUnits { get; private set; } = new List<Unit>();

    public override void _Ready()
    {
        // PlacedUnits.Add(new Unit(new Vector2(100, 50)));
        // PlacedUnits.Add(new Unit(new Vector2(300, 150)));
        // PlacedUnits.Add(new Unit(new Vector2(500, 250)));
        // PlacedUnits.Add(new Unit(new Vector2(700, 350)));
        // PlacedUnits.Add(new Unit(new Vector2(900, 450)));

        Log.Info($"SelectionManager is Ready with {PlacedUnits.Count} units placed.");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                startDraggingPosition = GetGlobalMousePosition();
                isDragging = true;
                selectionRect = new ColorRect();
                selectionRect.Color = new Color(Colors.Blue, 0.4f);
                AddChild(selectionRect);

                // Log.Debug("Left mouse button clicked at position: " + startDraggingPosition);

            }
            else if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
            {
                GameManager.Instance.RemoveAllUnits();

                Vector2 finalPosition = GetGlobalMousePosition();
                Vector2 size = finalPosition - startDraggingPosition;
                Rect2 finalRect = new Rect2(startDraggingPosition, size).Abs();

                PlacedUnits = GetAllUnitsFromGroup("units");

                PlacedUnits.ForEach(unit =>
                {

                    if (finalRect.HasPoint(unit.GlobalPosition))
                    {
                        GameManager.Instance.AddUnit(unit);
                        // Log.Info($"Unit {unit} is within the selection rectangle, adding to the selection list.");
                    }
                });

                // Log.Info($"Unit List has {GameManager.Instance.AllUnits.Count} units after the selection.");

                isDragging = false;
                selectionRect?.QueueFree();
                selectionRect = null;

                // Log.Debug("Left mouse button released at position: " + GetGlobalMousePosition());
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
            {
                Vector2 targetPosition = GetGlobalMousePosition();
                float spacing = 75.0f;

                for (int i = 0; i < GameManager.Instance.AllUnits.Count; i++)
                {
                    Unit unit = GameManager.Instance.AllUnits[i];

                    int row = i / 5; // Assuming 5 units per row
                    int col = i % 5;
                    Vector2 gridOffset = new Vector2(col * spacing, row * spacing);

                    Vector2 moveToPosition = targetPosition + gridOffset;

                    unit.MoveTo(moveToPosition);
                    unit.SetBehaviorState(Unit.BehaviorState.Moving);

                    // Log.Info($"Moving unit {unit} to position {moveToPosition}");
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

                // Log.Debug("Dragging... Current selection rectangle: " + selectionRect);

            }
        }
    }

    public List<Unit> GetAllUnitsFromGroup(string groupName = "units")
    {
        var nodesInGroup = GetTree().GetNodesInGroup(groupName);

        List<Unit> allUnits = nodesInGroup.OfType<Unit>().ToList();

        return allUnits;
    }
}
