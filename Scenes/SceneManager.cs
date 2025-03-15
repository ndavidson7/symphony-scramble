using SymphonyScramble.GUI;

namespace SymphonyScramble.Scenes;

internal class SceneManager
{
    private readonly Scene[] _scenes;

    private Scene? _activeScene;

    public SceneManager(Game1 game)
    {
        _scenes =
        [
            new Menu(game),
            new OptionsMenu(game),
            new Level(game, "tutorial"),
            new Level(game, "trumpet"),
            new Level(game, "drum"),
        ];
    }

    public void Update(GameTime gameTime)
    {
        _activeScene?.Update(gameTime);
    }

    public void Draw()
    {
        _activeScene?.Draw();
    }

    public void SwitchScene(SceneType type)
    {
        _activeScene = _scenes[(int)type];
        _activeScene.LoadContent();
    }
}
