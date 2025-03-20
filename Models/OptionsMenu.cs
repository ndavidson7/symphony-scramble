using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SymphonyScramble;

public class OptionsMenu
{
    #region Fields
    private List<Button> _buttons;
    private Game1 _game;
    private SpriteFont _titleFont;
    #endregion

    private List<string> _credits;
    public SpriteFont _creditsFont;

    public OptionsMenu (Game1 game)
    {
        _game = game;
        ContentManager _content = Globals.Content;

        var difficultyButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-grey_L_btn");
        var backButtonTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-red_M_btn");
        var clearScoreTexture = _content.Load<Texture2D>("Sprites/Buttons/CGB02-blue_L_btn");
        var buttonFont = _content.Load<SpriteFont>("Fonts/font");
        _titleFont = _content.Load<SpriteFont>("Fonts/TitleFont");


        var pos = new Vector2(Config.WindowSize.X / 2 - (difficultyButtonTexture.Width / 2), Config.WindowSize.Y * 4 / 16); // * 2 / 5);
        var difficultyButton = new Button(difficultyButtonTexture, buttonFont)
        {
            Position = pos,
            Text = $"{_game.GetDifficulty()} MODE"
        };
        difficultyButton.Click += DifficultyButton_Click;


        pos.Y += difficultyButtonTexture.Height * 13 / 12;
        var clearScoreButton = new Button(clearScoreTexture, buttonFont)
        {
            Position = pos,
            Text = "CLEAR HIGH SCORE"
        };
        clearScoreButton.Click += ClearHighScore_Click;


        pos.Y += difficultyButtonTexture.Height * 13 / 12;
        pos.X = Config.WindowSize.X/4 + difficultyButtonTexture.Width / 4;
        var backButton = new Button(backButtonTexture, buttonFont)
        {
            Position = pos,
            Text = "BACK"
        };
        backButton.Click += BackButton_Click;

       

        _buttons = new List<Button>() 
        {
            backButton, difficultyButton, clearScoreButton
        };

        LoadCredits();

        _creditsFont = Globals.Content.Load<SpriteFont>("Fonts/HealthFont");
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
        _game.SetState(Game1.ScreenState.Menu);
    }

    private void ClearHighScore_Click(object sender, EventArgs e)
    {
        _game.UpdateHighScore(0);
    }

    private void DifficultyButton_Click(object sender, EventArgs e)
    {
        _game.RotateDifficulty();
        _buttons[1].Text = $"{_game.GetDifficulty()} MODE";
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

    public void LoadCredits()
    {
        _credits = new List<string>();
        _credits.Add("Developers:\nJohn Fitzgerald, jhf8fjb\nNicholas Davidson, nid3dhu\nCyrus Rody-Ramazani, clr3dw\nJosh Mehr, jmm3vn\nSam Crown, ssc5jz");
        _credits.Add("Music:\nMenu: Mario Kart Wii - N64 Sherbet Land\n" +
            "Tutorial: Mario Kart Wii - GBA Shy Guy Beach\n" +
            "Level 1: Super Mario 3D - Bowser Land Theme\n" +
            "Level 2: Mario Kart Wii - N64 DK's Jungle Parkway");
    }

    public void Draw()
    {
        Globals.SpriteBatch.Begin();

        var text = "Options";
        var x = (Config.WindowSize.X / 2) - (_titleFont.MeasureString(text).X / 2);
        var y = (Config.WindowSize.Y / 10);

        Globals.SpriteBatch.DrawString(_titleFont, text, new Vector2(x, y), Color.ForestGreen);

        foreach (var button in _buttons)
        {
           button.Draw();
        }

        y = 27* (Config.WindowSize.Y / 32);
        x = (_titleFont.MeasureString(text).X / 3);

        foreach (var cred in _credits){
            
            Globals.SpriteBatch.DrawString(_creditsFont, cred, new Vector2(x, y), Color.Black, 0, new Vector2(0,0), 0.35f, SpriteEffects.None, 0);
            x += (Config.WindowSize.X / 3);
        }




        Globals.SpriteBatch.End();
    }
}
