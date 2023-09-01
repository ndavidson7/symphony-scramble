namespace SymphonyScramble;

public class Box : Interactable
{
    private static Rectangle SOURCE_RECTANGLE = new Rectangle(0, 0, 8, 8);
	private const float DEFAULT_SCALE = 1f;
	private const float DEFAULT_GRAVITY = 8f;
    private const float DEFAULT_PUSH_DRAG = 0.6f;
    private const float DEFAULT_HIT_DRAG = 0.95f;

    public Box(Vector2 position, float scale = DEFAULT_SCALE, float gravity = DEFAULT_GRAVITY, float pushDrag = DEFAULT_PUSH_DRAG, float hitDrag = DEFAULT_HIT_DRAG) : base(position, scale, SOURCE_RECTANGLE, gravity, pushDrag, hitDrag)
	{
	}

    public override void LoadContent()
    {
        _texture = Globals.Content.Load<Texture2D>("Sprites/Interactables/Box/Idle");
    }
}

