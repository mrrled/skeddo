using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace newUI.Services;

public class DragDropService
{
    private Control? draggedControl;
    private Point dragStartPoint;
    
    public void StartDrag(Control control, Point startPoint)
    {
        draggedControl = control;
        dragStartPoint = startPoint;
    }
    
    public bool CanDrop(Control target, Point dropPoint)
    {
        return draggedControl != null && 
               draggedControl != target &&
               IsValidDropTarget(target);
    }
    
    public void Drop(Control target, Point dropPoint)
    {
        if (draggedControl != null && CanDrop(target, dropPoint))
        {
            OnDrop?.Invoke(draggedControl, target);
            draggedControl = null;
        }
    }
    
    public void CancelDrag()
    {
        draggedControl = null;
    }
    
    public event Action<Control, Control>? OnDrop;
    
    private bool IsValidDropTarget(Control target)
    {
        return true;
    }
}