using Godot;
using System;

public class SDFTextSprite : MeshInstance
{

    Label label;
    Viewport port;

    private string _Text = "";
    [Export]
    public String Text
    {
        get => _Text;
        set
        {
            _Text = value; 
            if(!(label is null))
                label.Text = value;
            
        }
    }
    [Export]
    float pixelDensity = 100f;
    private Vector2 _Size = new Vector2(1.5f,0.3f);
    [Export]
    public Vector2 Size
    {
        get => _Size;
        set
        {
            _Size = value;
            Scale = new Vector3(_Size.x, _Size.y, Scale.z);
            if(!(port is null))
                port.Size = value * pixelDensity;
        }
    }

    public override void _Ready()
    {

        label = GetNode<Label>("Port/Label");
        label.Text = Text;
        port = GetNode<Viewport>("Port");
        port.Size = Size;

        //It's a bit ridiculous that we have to set this in code
        //instead of assigning in the editor, but it should be fixed
        //in Godot 3.2.4 when it releases so this should be fine for now.
        var texture = port.GetTexture();
        texture.Flags = (uint) Texture.FlagsEnum.Filter;
        
        ShaderMaterial shaderMat = new ShaderMaterial();
        shaderMat.Shader = (Shader) GD.Load<Shader>("res://BasicScenes/GUI/2.5D UI/Text3D/SDF.shader").Duplicate();
        shaderMat.Shader.SetDefaultTextureParam("sdf_texture", port.GetTexture());
        GD.PrintErr(shaderMat.Shader.GetDefaultTextureParam("sdf_texture"));
        MaterialOverride = shaderMat;
        
    }

}
