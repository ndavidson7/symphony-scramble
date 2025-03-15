using Microsoft.Xna.Framework.Content;
using SymphonyScramble.Scenes;
using System;
using System.Collections.Generic;

namespace SymphonyScramble.GUI;

/*
 * REFERENCES
 * Title: MonoGame_Tutorials/Tutorial013
 * Author: Niall Lewin (Oyyou on Github)
 * Date: 4/24/2023
 * URL: https://github.com/Oyyou/MonoGame_Tutorials/tree/master/MonoGame_Tutorials/Tutorial013
 */
internal class Menu : Scene
{
    private readonly List<Button> _buttons;
    private readonly Game1 _game;
    private readonly SpriteFont _titleFont;
    private readonly SpriteFont _altFont;

    public Menu(Game1 game)
    {
        ContentManager content = Globals.Content;
        _game = game;
        var startButtonTexture = content.Load<Texture2D>("Sprites/Buttons/CGB02-green_L_btn");
        var optionsButtonTexture = content.Load<Texture2D>("Sprites/Buttons/CGB02-orange_M_btn");
        var tutorialButtonTexture = content.Load<Texture2D>("Sprites/Buttons/CGB02-blue_M_btn");
        var quitButtonTexture = content.Load<Texture2D>("Sprites/Buttons/CGB02-red_M_btn");
        var buttonFont = content.Load<SpriteFont>("Fonts/font");
        _titleFont = content.Load<SpriteFont>("Fonts/TitleFont");
        _altFont = content.Load<SpriteFont>("Fonts/AlternateFont");

        Vector2 pos = new(Config.WindowSize.X / 2 - startButtonTexture.Width / 2, Config.WindowSize.Y * 3 / 8);

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


        pos.X += startButtonTexture.Width + Config.WindowSize.X / 40;
        var quitButton = new Button(quitButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "QUIT"
        };
        quitButton.Click += QuitButton_Click;

        _buttons = [startButton, optionsButton, tutorialButton, quitButton];
    }

    private void OptionsButton_Click(object sender, EventArgs e)
    {
        Debug.WriteLine("Optionz!");
        _game.SetState(Game1.ScreenState.Options);
    }

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

    private void StartButton_Click(object sender, EventArgs e)
    {
        _game.switch_levels();

        _game.switch_songs();
    }

    public void LoadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
        foreach (var button in _buttons)
        {
            button.Update();
        }
    }

    public override void Draw()
    {
        Globals.SpriteBatch.Begin();

        var text = "Symphony Scramble!";
        var x = Config.WindowSize.X / 2 - _titleFont.MeasureString(text).X / 2;
        var y = Config.WindowSize.Y / 7;

        Globals.SpriteBatch.DrawString(_titleFont, text, new Vector2(x, y), Color.ForestGreen);

        foreach (var button in _buttons)
        {
            button.Draw();
        }

        string highscoreText = $"High Score: {_game.GetHighScore()}";
        x = Config.WindowSize.X / 2 - _altFont.MeasureString(highscoreText).X / 2;
        y = Config.WindowSize.Y * 17 / 20;
        Globals.SpriteBatch.DrawString(_altFont, highscoreText, new Vector2(x, y), Color.Crimson);

        Globals.SpriteBatch.End();
    }
}

