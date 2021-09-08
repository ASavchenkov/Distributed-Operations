using Godot;
using System;

//Size and position derived from external "Control" nodes.
//This lets us make 3D UIs that fit.

//Can be applied to any Spatial (theoretically)
public interface ControlledSpatial
{
    ControlTracker tracker {get;set;}
    void Resize(Vector3 topLeft, Vector3 bottomRight);
    #region NODE STUFF
    T GetNode<T>(NodePath path) where T : class;
    #endregion
}

public class ControlTracker : Godot.Object
{
    ControlledSpatial parent;
    Control layoutControl;
    Camera camera;
    public Vector3 TopRight;
    public Vector3 BottomLeft;
    public Vector3 Size {get => TopRight-BottomLeft;}
    
    public float depth;

    public ControlTracker( ControlledSpatial parent, NodePath camPath, NodePath ControlPath, float depth)
    {
        this.parent = parent;
        this.layoutControl = parent.GetNode<Control>(ControlPath);
        this.camera = parent.GetNode<Camera>(camPath);
        this.depth = depth;
        layoutControl.Connect("resized", this, nameof(OnLayoutResized));
        OnLayoutResized();
    }
    public void OnLayoutResized()
    {
        Rect2 newRect = layoutControl.GetGlobalRect();
        //Swapping corners makes the math much cleaner on the receiving end.
        TopRight = camera.ProjectLocalRayNormal(new Vector2(newRect.Position.x + newRect.Size.x, newRect.Position.y ));
        TopRight *= (depth/Math.Abs(TopRight.z));
        BottomLeft = camera.ProjectLocalRayNormal(new Vector2(newRect.Position.x, newRect.Position.y + newRect.Size.y ));
        BottomLeft *= (depth/Math.Abs(BottomLeft.z));

        parent.Resize(TopRight,BottomLeft);
        
    }
}
public class ControlledBoxArea : Area, ControlledSpatial
{

    public ControlTracker tracker {get;set;}
    
    public NodePath camPath = "..";
    [Export]
    public NodePath ControlPath;
    [Export]
    float depth = 1;
    [Export]
    float thiccness = 0.01f;


    CollisionShape collider;
    public override void _Ready()
    {
        collider = GetNode<CollisionShape>("CollisionShape");
        tracker = new ControlTracker(this,camPath, ControlPath, depth);
    }

    public void Resize(Vector3 topRight, Vector3 bottomLeft)
    {
        Translation = bottomLeft;
        collider.Translation = (topRight-bottomLeft)/2 + new Vector3(0,0,collider.Translation.z);
        var shape = (BoxShape) collider.Shape;
        shape.Extents =  (topRight-bottomLeft)/2 + new Vector3(0,0,thiccness);
    }

}
