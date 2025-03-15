using Microsoft.Xna.Framework.Media;
using SymphonyScramble.Scenes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymphonyScramble;

public class Game1 : Game
{
    private const string HighScoreFilePath = "high_score.txt";

    private readonly GraphicsDeviceManager _graphics;
    private readonly SceneManager _sceneManager;

    private readonly Camera _camera = new();

    public bool paused;

    public Game1()
    {
        _graphics = new(this)
        {
            PreferredBackBufferWidth = 1920,
            PreferredBackBufferHeight = 1080
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.Title = "Symphony Scramble";

        _sceneManager = new(this);

        if (File.Exists(HighScoreFilePath))
            HighScore = int.TryParse(File.ReadAllText(HighScoreFilePath), out int highScore) ? highScore : 0;
    }

    public int HighScore { get; set; }

    protected override void Initialize()
    {
        Globals.Game = this;
        Globals.Content = Content;
        Globals.GraphicsDevice = GraphicsDevice;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);

        MediaPlayer.IsRepeating = true;

        _sceneManager.SwitchScene(SceneType.Menu);
    }

    protected override void Update(GameTime gameTime)
    {
        // only poll input once, then access it using a global variable
        KeyboardState kb = Keyboard.GetState();

        Globals.Update(gameTime, kb);
        
        // control which screen is being updated
        _sceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Honeydew);

        _sceneManager.Draw();

        base.Draw(gameTime);
    }

    public enum DifficultyLevel
    {
        Easy,
        Regular,
        Hard
    }

    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Regular;

    public void RotateDifficulty()
    {
        Difficulty = (DifficultyLevel) (((int)Difficulty + 1) % Enum.GetValues<DifficultyLevel>().Length);
    }

    protected void loadMusic()
    {
        /* Music downloaded from Mario game mp3s: https://downloads.khinsider.com/mario
         * 
         *   Menu: Mario Kart Wii - N64 Sherbet Land
         *   Tutorial: Mario Kart Wii - GBA Shy Guy Beach
         *   Level 1: Super Mario 3D - Bowser Land Theme
         *   Level 2: Mario Kart Wii - N64 DK's Jungle Parkway
         */
        _level1_music = Globals.Content.Load<Song>("Music/trumpet2");
        _level2_music = Globals.Content.Load<Song>("Music/tutorial2"); 
        _tutorial_music = Globals.Content.Load<Song>("Music/Beach");
        _menu_music = Globals.Content.Load<Song>("Music/menu");
        MediaPlayer.IsRepeating = true;
    }

    public void setWinningMessage()
    {
        _winningMessage = Globals.CurrentLevel == _level1 ? "You beat Dr.Evil's Brass!\nWhen you're ready, press\nthe Enter key to continue\non your musical journey" :
            "You've escaped\nDr. Music's\nclutches! Your\nscore is\n" + Globals.CurrentLevel._score + "!!!\nWay to go!\n\n" + "Think you\ncan score\nhigher?\nPress Enter\nto return\nto the\nmain menu.";
        _winningMessagePosition = Globals.CurrentLevel == _level1 ? new Vector2(1880, 60) : new Vector2(200, -20);
    }

    protected void loadTutorialStrings()
    {
        _tutorialStrings.Add(new List<String>(){"Welcome to Sympthony Scramble! Against your best wishes,\n" +
                             "you've been captured and shrunken down by the evil Dr. Music.", "Black"});
        
        _tutorialStringPositions.Add(new Vector2(-50, 170));

        _tutorialStrings.Add(new List<String>(){"It's up to you to make your way through any melodic obstacle \n" +
                             "he throws in your way, and perhaps you'll gain a\nbetter appreciation for music along the way!", "Black" });

        _tutorialStringPositions.Add(new Vector2(-50, 190));

        _tutorialStrings.Add(new List<String>(){"(Press A and D to move left and right)", "Red" });

        _tutorialStringPositions.Add(new Vector2(-50, 270));

        _tutorialStrings.Add(new List<String>() { "(Press W to jump on platforms!)", "Red" });

        _tutorialStringPositions.Add(new Vector2(235, 170));

        _tutorialStrings.Add(new List<String>() { "Throughout your\n" +
            "journey you'll\n" +
            "encounter different\n"+
            "types of platforms.\n" +
            "These platforms\n" +
            "can be entered\n"+
            "from below!", "Black" });

        _tutorialStringPositions.Add(new Vector2(430, 120));

        _tutorialStrings.Add(new List<String>() { "You'll encounter enemies as you progress. These blue\n" +
            "enemies will move back and forth on platforms.\n"+
            "Avoid them to save your health, or jump on top\n" +
            "to defeat them!", "Black" });

        _tutorialStringPositions.Add(new Vector2(520, 0));

        _tutorialStrings.Add(new List<String>() { "These timed platforms\n" +
            "are destroyed a few seconds\n"+
            "after you touch them!", "Black" });

        _tutorialStringPositions.Add(new Vector2(750, 180));

        _tutorialStrings.Add(new List<String>() { "Green enemies are able to follow you\n" +
            "and jump over barriers. Look out!", "Green"});

        _tutorialStringPositions.Add(new Vector2(910, 160));

        _tutorialStrings.Add(new List<String>() { "The top black bar shows your Song Health.\n You need to keep the song alive, as well\n" +
            "as yourself. Collect music\nnotes to boost your score,\nand keep the song alive!", "Black"});

        _tutorialStringPositions.Add(new Vector2(1200, 160));

        _tutorialStrings.Add(new List<String>() { "You're all ready to go, good luck!\n" + "(Also look out for flying enemies on your way)", "Black"});

        _tutorialStringPositions.Add(new Vector2(1700, 160));

        _tutorialStrings.Add(new List<String>() { "You can also press S to fast-fall!", "Red" });

        _tutorialStringPositions.Add(new Vector2(1700, 270));
    }
}
