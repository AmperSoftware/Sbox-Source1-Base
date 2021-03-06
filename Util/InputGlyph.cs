using Sandbox;
using Sandbox.UI;

namespace Amper.Source1;

public class InputGlyph : Image
{
	public InputButton Button { get; set; }
	public InputGlyphSize Size { get; set; }
	GlyphStyle GlyphStyle { get; set; } = default;

	static Texture UnboundTexture = Texture.Load( FileSystem.Mounted, "/ui/unbound.png" );

	public Texture GetGlyphTexture()
	{ 
		// this key is unbound.
		if ( string.IsNullOrEmpty( Input.GetButtonOrigin( Button ) ) )
			return UnboundTexture;

		// texture doesnt exist, or can't be generated
		var texture = Input.GetGlyph( Button, Size, GlyphStyle );
		if ( texture == null )
			return UnboundTexture;

		return texture;
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "large", Size == InputGlyphSize.Large );
		SetClass( "medium", Size == InputGlyphSize.Medium );
		SetClass( "small", Size == InputGlyphSize.Small );

		var texture = GetGlyphTexture();

		Texture = texture;

		float width = texture.Width;
		float height = texture.Height;
		var aspectRatio = width / height;

		Style.AspectRatio = aspectRatio;

		if ( Input.Pressed( Button ) )
			SetClass( "pressed", true );

		if ( Input.Released( Button ) )
			SetClass( "pressed", false );
	}
}
