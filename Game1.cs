using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using TiledCS;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security;
using static System.Formats.Asn1.AsnWriter;
using System.Collections;
using Microsoft.Xna.Framework.Media;

namespace SymphonyScramble;

/*
 * REFERENCES
 * Title: Identify if a string is a number
 * Author: 'mqp' (on stackoverflow) and edited by 'Vadim Ovchinnikov' (on stackoverflow)
 * Date: 4/25/2023
 * URL: https://stackoverflow.com/questions/894263/identify-if-a-string-is-a-number
 * 
 * Title: Easiest way to read from and write to files
 * Author: 'vc 74' (on stackoverflow)
 * Date: 4/25/23
 * URL: https://stackoverflow.com/questions/7569904/easiest-way-to-read-from-and-write-to-files
 */
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Camera _camera;

    // contains all data and manages updating and drawing logic
    public Level _level1;
    public Level _level2;
    public Level _tutorial;
    public List<Level> _levels;
    public int _level_num;
    public bool _level_started;

    private Menu _menu;
    private OptionsMenu _options;

    private string _difficulty;
    private int _highScore;

    public enum ScreenState
    {
        Menu,
        Options,
        Tutorial,
        Level_1,
        Level_2 // etc.
    }
    private ScreenState _screenState;

    public Actor _player;

    public float _scaleFactor;

    public Song _level1_music;
    public Song _level2_music;

    public Song _menu_music;
    public Song _tutorial_music;

    public List<List<String>> _tutorialStrings;
    public List<Vector2> _tutorialStringPositions;

    public int _level1_score;

    public bool paused;

    public string _winningMessage;
    public Vector2 _winningMessagePosition;

    public void SetState(ScreenState state)
    {
        _screenState = state;
    }

    // switches to next level or back to the main menu (for when you get to the end of a level)
    public void RotateState()
    {
      
        switch_levels();
        switch_songs();
    }

    public void SetDifficulty(string diff)
    {
        _difficulty = diff;
    }

    public string GetDifficulty()
    {
        return _difficulty;
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    public void RotateDifficulty()
    {
        _difficulty = _difficulty switch
        {
            "EASY" => "REGULAR",
            "REGULAR" => "HARD",
            _ => "EASY",
        };
    }

    public int GetHighScore()
    {
        string filename = "High_Score.txt";
        if (File.Exists(filename))
        {
            // https://stackoverflow.com/questions/894263/identify-if-a-string-is-a-number
            string filetext = File.ReadAllText(filename);
            var isNumeric = int.TryParse(filetext, out int n);
            if (isNumeric)
                return n;
        }

        return _highScore;

    }

    public void UpdateHighScore(int score)
    {
        string filename = "High_Score.txt";
        if (File.Exists(filename))
        {
            File.WriteAllText(filename, $"{score}");
        }

        _highScore = score;
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void switch_levels()
    {
        switch (_screenState)
        {
            case (Game1.ScreenState.Menu):
                SetState(Game1.ScreenState.Level_1);
                Globals.CurrentLevel = _level1;
                _level_num = 1;
                break;
            case (Game1.ScreenState.Level_1):
                SetState(Game1.ScreenState.Level_2);
                Globals.CurrentLevel = _level2;
                _level_num = 2;
                break;
            case (Game1.ScreenState.Level_2):
                SetState(Game1.ScreenState.Menu);
                break;
            case (Game1.ScreenState.Tutorial):
                SetState(Game1.ScreenState.Menu);
                break;
            default:
                SetState(Game1.ScreenState.Menu);
                Globals.CurrentLevel = _level1;
                break;
        }
        _level_started = true;
        Globals.ResetTime();
        if (Globals.CurrentLevel._firstLaunch) Globals.CurrentLevel.LoadContent();
        else Globals.CurrentLevel.Reset();
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Config.WindowSize.X;
        _graphics.PreferredBackBufferHeight = Config.WindowSize.Y;
        _graphics.ApplyChanges();

        Globals.Game = this;
        Globals.Content = Content;
        Globals.GraphicsDevice = GraphicsDevice;

        _screenState = ScreenState.Menu;

        _difficulty = "EASY";
        _highScore = GetHighScore();

        _level_num = 1;
        _menu = new Menu(this);
        _options = new OptionsMenu(this);


        string playerClass = "SymphonyScramble.King";
        Type? playerType = Type.GetType(playerClass) ?? throw new ArgumentException($"Player layer class, {playerClass}, is not a valid Actor type");


        TiledMap _map = new TiledMap(Path.Combine(Globals.Content.RootDirectory, "drum.tmx"));
        float _scale = Math.Min((float)Config.WindowSize.X / (_map.Width * _map.TileWidth), (float)Config.WindowSize.Y / (_map.Height * _map.TileHeight));
        _player = (Actor)Activator.CreateInstance(playerType, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding, null, new object[] { new Vector2(0, 0), _scale }, null);

        _level1 = new(this, "trumpet");

        _level2 = new(this, "drum");

        _tutorial = new(this, "tutorial");

        _level1_score = 0;

        Globals.CurrentLevel = _level1;

        _levels = new List<Level>() { _level1, _level2 };

        _tutorialStrings = new List<List<String>>();
        _tutorialStringPositions = new List<Vector2>();

        loadTutorialStrings();

        loadMusic();
        MediaPlayer.Play(_menu_music);

        _winningMessage = "";

        base.Initialize();
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


    public void switch_songs()
    {
        if (MediaPlayer.State != MediaState.Stopped ) MediaPlayer.Stop();
        switch (_screenState)
        {
            case (ScreenState.Menu):
                MediaPlayer.Play(_menu_music);
                break;
            case (ScreenState.Options):
                MediaPlayer.Play(_menu_music);
                break;
            case (ScreenState.Level_1):
                MediaPlayer.Play(_level1_music);
                break;
            case (ScreenState.Level_2):
               MediaPlayer.Play(_level2_music);
                break;
            case (ScreenState.Tutorial):
                MediaPlayer.Play(_tutorial_music);
                break;
            default:
                MediaPlayer.Play(_menu_music);
                break;
        }
        
    }

    protected override void LoadContent()
    {

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;

        _menu.LoadContent();
       // _options.LoadContent();
        _camera = new Camera();

        _level_started = false;

        loadMusic();
    }

    protected override void Update(GameTime gameTime)
    {
        // only poll input once, then access it using a global variable
        KeyboardState kb = Keyboard.GetState();

        if (!paused)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
            {

                SetState(Game1.ScreenState.Menu);
                Globals.CurrentLevel = _level1;
                _level_num = 1;
                _level1_score = 0;
                Globals.ResetTime();

                switch_songs();
            }

            Globals.Update(gameTime, kb);
            // control which screen is being updated
            switch (_screenState)
            {
                case ScreenState.Menu:
                    _menu.Update();
                    break;
                case ScreenState.Options:
                    _options.Update();
                    break;
                case ScreenState.Tutorial:
                    _tutorial.Update();
                    break;
                case ScreenState.Level_1:
                    _level1.Update();
                    break;
                case ScreenState.Level_2:
                    _level2.Update();
                    break;
                default: break;
            }

            if (_level_started)
                _camera.Follow(Globals.CurrentLevel.Player);

            base.Update(gameTime);
        }

        else if (kb.IsKeyDown(Keys.Enter))
        {
            paused = false;
            Globals.CurrentLevel.RotateState();
           
        }
    } 

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Honeydew);

        switch (_screenState) {
            case ScreenState.Menu:
                _menu.Draw();
                break;
            case ScreenState.Options:
                _options.Draw();
                break;
            case ScreenState.Tutorial:
                _tutorial.Draw();
                break;
            case ScreenState.Level_1: 
                _level1.Draw();
                break;
            case ScreenState.Level_2:
                _level2.Draw();
                break;
            default: break;
        }

        base.Draw(gameTime);
    }
}
