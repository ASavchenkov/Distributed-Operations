using Godot;
using System;

//Size and position derived from external "Control" nodes.
//This lets us make 3D UIs that fit.

//Can be applied to any Spatial (theoretically)
public interface ControlledSpatial
{
    ControlTracker tracker {get;set;}
    void Resize(Vector3 topRight, Vector3 bottomLeft);
    #region NODE STUFF
    T GetNode<T>(NodePath path) where T : class;
    Viewport GetViewport();
    #endregion
}

public class ControlTracker : Godot.Object
{
    ControlledSpatial parent;
    Control layoutControl;
    public Camera camera;
    public Vector3 TopRight;
    public Vector3 BottomLeft;
    public Vector3 Size {get => TopRight-BottomLeft;}
    
    public float depth;

    public ControlTracker( ControlledSpatial parent, NodePath ControlPath, float depth)
    {
        this.parent = parent;
        this.layoutControl = parent.GetNode<Control>(ControlPath);
        this.camera = parent.GetViewport().GetCamera();
        this.depth = depth;
        layoutControl.Connect("item_rect_changed", this, nameof(OnLayoutChange));

        OnLayoutChange();
    }
    public void OnLayoutChange()
    {
        Rect2 newRect = layoutControl.GetGlobalRect();

        //Swapping corners makes the math much cleaner on the receiving end.
        
        //Relative to the camera. Makes sizing easier on the other end.
        //Going to need to convert to global for placement.
        TopRight = camera.ToLocal(camera.ProjectRayOrigin(new Vector2(newRect.Position.x + newRect.Size.x, newRect.Position.y)));
        TopRight += new Vector3(0,0,-depth);
        BottomLeft = camera.ToLocal(camera.ProjectRayOrigin(new Vector2(newRect.Position.x, newRect.Position.y + newRect.Size.y )));
        BottomLeft += new Vector3(0,0,-depth);
        
        #region PERSPECTIVE_HIDDEN
        // TopRight = camera.ProjectLocalRayNormal(new Vector2(newRect.Position.x + newRect.Size.x, newRect.Position.y ));
        // TopRight *= (depth/Math.Abs(TopRight.z));
        // BottomLeft = camera.ProjectLocalRayNormal(new Vector2(newRect.Position.x, newRect.Position.y + newRect.Size.y ));
        // BottomLeft *= (depth/Math.Abs(BottomLeft.z));
        #endregion

        parent.Resize(TopRight,BottomLeft);
        
    }
}
public class ControlledBoxArea : Area, ControlledSpatial
{

    public ControlTracker tracker {get;set;}
    
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
        tracker = new ControlTracker(this, ControlPath, depth);
    }

    public void Resize(Vector3 topRight, Vector3 bottomLeft)
    {
        //identical to just setting Translation if child of Camera
        GlobalTransform = GetViewport().GetCamera().Transform.Translated(bottomLeft);
        collider.Translation = (topRight-bottomLeft)/2 + new Vector3(0,0,collider.Translation.z);
        var shape = (BoxShape) collider.Shape;
        shape.Extents =  (topRight-bottomLeft)/2 + new Vector3(0,0,thiccness);
    }

}
