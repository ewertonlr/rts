using Godot;
using System;

public partial class SelectionManager : Control
{
    private bool isDragging = false;
    private Vector2 startDraggingPosition;
    private Rect2 selectionRect;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                startDraggingPosition = GetGlobalMousePosition();
                isDragging = true;

                Log.Debug("Left mouse button clicked at position: " + startDraggingPosition);

            }
            else if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
            {
                isDragging = false;

                Log.Debug("Left mouse button released at position: " + GetGlobalMousePosition());

            }
        }
        else if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (isDragging)
            {
                Vector2 currentMousePosition = GetGlobalMousePosition();
                selectionRect = new Rect2(startDraggingPosition, currentMousePosition - startDraggingPosition);

                Log.Debug("Dragging... Current selection rectangle: " + selectionRect);
                // Update selection rectangle visual here
            }
        }
    }
}
