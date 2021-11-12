using Godot;
using System;

public class SpatialLabel : SpatialControl
{
    SDFTextSprite sprite;

    //Just redirect to the SDFTextSprite.
    public String _Text = "";
    [Export]
    public String Text
    {
        get => _Text;
        set
        {
            _Text = value;
            if(!(sprite is null))
                sprite.Text = value;
        }
    }

    public override void _Ready()
    {
        sprite = GetNode<SDFTextSprite>("TextSprite");
        Size = Size;
        Text = Text;
        base._Ready();
    }

    //Set the sprite size to take up our entire space.
    public void OnOwnSizeChanged()
    {
        if(!(sprite is null))
        {
            sprite.Translation = new Vector3(Size.x/2, -Size.y/2, 0);
            sprite.Size = Size / sprite.PixelSize;
            GD.PrintErr(sprite.Translation, sprite.Size);
        }
        
    }
}