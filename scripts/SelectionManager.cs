using Godot;
using System;

public partial class SelectionManager : Control
{
    private bool isDragging = false;
    private Vector2 startDraggingPosition;
    private ColorRect selectionRect;

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

                Log.Debug("Left mouse button clicked at position: " + startDraggingPosition);

            }
            else if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
            {
                isDragging = false;
                selectionRect?.QueueFree();
                selectionRect = null;

                Log.Debug("Left mouse button released at position: " + GetGlobalMousePosition());

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

                Log.Debug("Dragging... Current selection rectangle: " + selectionRect);
                // Update selection rectangle visual here
            }
        }
    }
}
