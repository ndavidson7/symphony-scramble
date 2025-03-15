namespace SymphonyScramble.Models;

public class Note : SolidPlatform

{
    private static Rectangle SOURCE_RECTANGLE = new Rectangle(0, 0, 8, 8);
    private const float DEFAULT_SCALE = 1f;

    private Texture2D platformSprite;

    private bool _hasBeenTouched = false;

    public Note(Vector2 position, float scale = DEFAULT_SCALE) : base(position, scale)
    {
    }

    public void Load()
    {
        platformSprite = Globals.Content.Load<Texture2D>("Sprites/Interactables/Notes/note");
    }


    public void SetTouched()
    {
        {
            _hasBeenTouched = true;
        }
    }

    public override void Update()
    {
        if (_hasBeenTouched)
        {
            Globals.CurrentLevel.toRemove = this;
        }
    }


    public override void Draw()
    {
        Globals.SpriteBatch.Draw(platformSprite, new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height), Color.White);

    }
}







