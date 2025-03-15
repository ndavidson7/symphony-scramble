namespace SymphonyScramble.Scenes;

public enum SceneType
{
    Menu,
    Options,
    Tutorial,
    Level1,
    Level2
}

internal abstract class Scene
{
    public abstract void LoadContent();
    public abstract void Initialize();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}
