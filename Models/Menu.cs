using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SymphonyScramble.Game1;
using Microsoft.Xna.Framework.Media;

namespace SymphonyScramble;

/*
 * REFERENCES
 * Title: MonoGame_Tutorials/Tutorial013
 * Author: Niall Lewin (Oyyou on Github)
 * Date: 4/24/2023
 * URL: https://github.com/Oyyou/MonoGame_Tutorials/tree/master/MonoGame_Tutorials/Tutorial013
 */
public class Menu
{
    #region Fields
    private List<Button> _buttons;
    private Game1 _game;
    private SpriteFont _titleFont;
    private SpriteFont _altFont;
    #endregion

    public Menu(Game1 game) 
    {
        // not sure if i need something here
        ContentManager _content = Globals.Content;
        _game = game;
        var startButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-green_L_btn");
        var optionsButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-orange_M_btn");
        var tutorialButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-blue_M_btn");
        var quitButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-red_M_btn");
        var buttonFont = _content.Load<SpriteFont>("Fonts/font");
        _titleFont = _content.Load<SpriteFont>("Fonts/TitleFont");
        _altFont = _content.Load<SpriteFont>("Fonts/AlternateFont");

        Vector2 pos = new Vector2(Config.WindowSize.X / 2 - (startButtonTexture.Width / 2), Config.WindowSize.Y *3/8);

        var startButton = new Button(startButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "START"
        };
        startButton.Click += StartButton_Click;


        pos.Y += startButtonTexture.Height * 7 / 6;
        pos.X += startButtonTexture.Width / 4;

        var tutorialButton = new Button(tutorialButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "TUTORIAL"
        };

        tutorialButton.Click += TutorialButton_Click;

        pos.X -= tutorialButtonTexture.Width + Config.WindowSize.X / 50;
        var optionsButton = new Button(optionsButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "OPTIONS"
        };
        optionsButton.Click += OptionsButton_Click;


        pos.X += startButtonTexture.Width + Config.WindowSize.X/40;
        var quitButton = new Button(quitButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "QUIT"
        };
        quitButton.Click += QuitButton_Click;

     

        _buttons = new List<Button>() 
        {
            startButton, optionsButton, tutorialButton, quitButton
        };
    }

    private void OptionsButton_Click(object sender, EventArgs e)
    {
        Debug.WriteLine("Optionz!");
        _game.SetState(Game1.ScreenState.Options);
    }

    // executed when quit is clicked

    private void TutorialButton_Click(object sender, EventArgs e)
    {

        _game._level_started = true;
        _game.SetState(Game1.ScreenState.Tutorial);
        Globals.ResetTime();
        Globals.CurrentLevel = _game._tutorial;
        Globals.CurrentLevel.LoadContent();


        _game.switch_songs();
    }

        private void QuitButton_Click(object sender, EventArgs e)
    {
        _game.Exit();
    }

    // executed when start is clicked

    
    private void StartButton_Click(object sender, EventArgs e)
    {


        _game.switch_levels();

        _game.switch_songs();

    }

    public void LoadContent()
    {

    }

    public void Update()
    {
        foreach (var button in _buttons)
        {
            button.Update();
        }
    }

    public void Draw()
    {

        Globals.SpriteBatch.Begin();

        var text = "Symphony Scramble!";
        var x = (Config.WindowSize.X / 2) - (_titleFont.MeasureString(text).X / 2);
        var y = (Config.WindowSize.Y / 7);

        Globals.SpriteBatch.DrawString(_titleFont, text, new Vector2(x, y), Color.ForestGreen);

        foreach (var button in _buttons)
        {
            button.Draw();
        }

        String highscoretext = $"High Score: {_game.GetHighScore()}";
        x = (Config.WindowSize.X / 2) - (_altFont.MeasureString(highscoretext).X / 2);
        y = Config.WindowSize.Y * 17 / 20;
        Globals.SpriteBatch.DrawString(_altFont, highscoretext, new Vector2(x, y), Color.Crimson);

        Globals.SpriteBatch.End();
    }
}

