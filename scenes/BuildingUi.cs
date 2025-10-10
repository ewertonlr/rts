using Godot;
using System;

public partial class BuildingUi : Control
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            Log.Info("Input event received in BuildingUi: " + @event);

        }
        // {
        // if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
    }
}
