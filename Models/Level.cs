using System.Collections.Generic;
using TiledCS;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Reflection.Metadata;
using System.Security;
using static System.Formats.Asn1.AsnWriter;
using System.Collections;
using static System.Net.WebRequestMethods;
using Microsoft.Xna.Framework.Media;

namespace SymphonyScramble;

public class Level
{
    #region Fields
    private readonly Game1 _game;
    // Tiled-related fields
    private readonly string _mapName;
    private TiledMap _map;
    private Dictionary<int, TiledTileset> _tilesets;
    //private Dictionary<int, Dictionary<int, Texture2D>> _tilesetTextures;
    private Dictionary<int, Texture2D> _tilesetTextures;
    private float _scaleFactor;
    private Matrix _transformMatrix;

    // Level objects
    private Actor _player;
    private LevelEnd _levelEnd;
    private readonly List<Tile> _tiles;
    private readonly List<Tile> _nonstatictiles;
    private readonly List<Nonphysical> _nonPhysicals;
    private readonly List<Note> _notes;
    private readonly List<FollowingEnemy> _enemies;
    private readonly List<InvisibleBarrier> _invisibleBarriers;
    private Vector2 _levelBounds;
    private Vector2 _playerStartPosition;
    #endregion

    #region Properties
    public Actor Player => _player;
    public LevelEnd End => _levelEnd;
    public List<Nonphysical> NonPhysicals => _nonPhysicals;
    public List<Tile> Tiles => _tiles;
    public List<Tile> NonstaticTiles => _nonstatictiles;
    public List<Note> Notes => _notes;

    public List<FollowingEnemy> Enemies => _enemies;

    public List<InvisibleBarrier> InvisibleBarriers => _invisibleBarriers;
    #endregion

    public ICollidable toRemove;
    public Nonphysical toRemoveEnemy;
    public SpriteFont _scoreFont;
    public SpriteFont _healthFont;
    public string _scoreString;
    public int _score;
    public int _totalNotes;

    private string _healthString;
    private int _healthInt = 100;
    private Rectangle _healthBar;
    private Texture2D healthBarTexture;

    private readonly int _hurtSeverity;

    public bool _firstLaunch;

    public string _difficulty;

    private string _timer;

    public int _songHealth;

    private Song music;

    private double _songDecrement;

    private bool _flyingEnemyExists;

    private float _songHealthDifficultyDecrement;

    private Vector2 zeroVector;

    private  List<string> levelNames;

    public Level(Game1 game, string mapName)
    {
        _game = game;
        _mapName = mapName;

        _player = _game._player;
        _notes = new List<Note>();
        _tiles = new List<Tile>();
        _nonstatictiles = new List<Tile>();

        _nonPhysicals = new List<Nonphysical>();

        _invisibleBarriers = new List<InvisibleBarrier>();

        _score = _game._level1_score;
        _totalNotes = 0;
        _scoreString = "Score: ";

        _healthString = "Health: " + _healthInt;

        _hurtSeverity = 10;

        _firstLaunch = true;

        _difficulty = _game.GetDifficulty();

        _timer = "0:00";

        _songHealth = 100;

        _songDecrement = 0;

        _songHealthDifficultyDecrement = .2f;

        zeroVector = new Vector2(0, 0);

        levelNames = new List<string> { "Level 1: Brass Ensenemble", "Level 2: Out of the Djimbe" };

    }

    public string handleTime()
    {
        int seconds = (int)Globals.TotalSeconds;
        int minutes = seconds / 60;

        string secString = seconds < (minutes * 60) + 10 ? "0" : "";

        return minutes + ":" + secString + (seconds - (minutes * 60));
    }

    public Color getColorFromString(string inputColor) {
        switch (inputColor)
        {
            case "Black":
                return Color.Black;
            case "Blue":
                return Color.Blue;
            case "Red":
                return Color.Red;
            case "Green":
                return Color.Green;
            default:
                return Color.Black;
        }
    }

    public float songHealthDifficulty()
    {
        switch (Globals.CurrentLevel._difficulty)
        {
            case ("EASY"):
                return .6f;
            case ("HARD"):
                return .15f;
            default:
                return .3f;
        }
    }

    public int healthDifficulty()
    {
        switch (Globals.CurrentLevel._difficulty)
        {
            case ("EASY"):
                return 200;
            case ("HARD"):
                return 66;
            default:
                return 100;
        }
    }

    public bool isGamePaused()
    {
        return _game.paused;
    }

    public void handleSongHealth()
    {
        _songDecrement += Globals.ElapsedSeconds;
        if (_songDecrement > _songHealthDifficultyDecrement)
        {
            _songDecrement = 0;
            if (_songHealth > 0)
            {
                if (Globals.CurrentLevel != _game._tutorial) _songHealth--;
                else if (_player.Position.X > 1200) _songHealth--;
            }
            else
            {
                Reset();
            }
        }
        MediaPlayer.Volume = handleSongVolume();
    }

    public float handleSongVolume()
    {
        if (_songHealth < 50)
        {
            return (float)_songHealth / 50;
        }
        return 1f;
    }

    public void UpdateHighScore()
    {
        if (Globals.CurrentLevel == _game._level2)
        {
            _game._level1_score = 0;
            if (_score > _game.GetHighScore())  _game.UpdateHighScore(_score);
        }
        if (Globals.CurrentLevel == _game._level1) _game._level1_score = _score;
    }

    public void UpdateHealth()
    {
        if (_healthInt <= _hurtSeverity)
        {
            Reset();
        }
        _healthInt -= _hurtSeverity;
        _healthString = "Health: " + _healthInt;
    }

    public void EndSequence()
    {
        if (this != _game._tutorial)
        {
            _game.setWinningMessage();
            _game.paused = true;
        }
    }

    public void RotateState()
    {
        _game.RotateState();
    }

    public void LoadContent()
    {

        Globals.CurrentLevel = this;

        healthBarTexture = Globals.Content.Load<Texture2D>("Sprites/Actors/King/HealthBar");

        // Set the "Copy to Output Directory" property of these two files to `Copy if newer`
        // by clicking them in the solution explorer.
        _map = new TiledMap(Path.Combine(Globals.Content.RootDirectory, $"{_mapName}.tmx"));
        _tilesets = _map.GetTiledTilesets(Globals.Content.RootDirectory + "/"); // DO NOT forget the / at the end

        _levelBounds = new(_map.Width * _map.TileWidth, _map.Height * _map.TileHeight); // dimensions of the map in pixels

        _tilesetTextures = new();
        foreach (var item in _tilesets)
        {
            //foreach (var tile in item.Value.Tiles)
            //{
            //    if (!_tilesetTextures.ContainsKey(item.Key)) _tilesetTextures[item.Key] = new();

            //    // If the Tiled Tileset type is "Based on Tileset Image," then each tile will point to the same Texture2D.
            //    // Otherwise, each tile has its own Texture2D.
            //    var tileImage = item.Value.Image != null ? Path.GetFileNameWithoutExtension(item.Value.Image.source) : Path.GetFileNameWithoutExtension(tile.image.source);
            //    var tilesetDictionary = _tilesetTextures[item.Key];
            //    tilesetDictionary[tile.id] = Globals.Content.Load<Texture2D>($"Tilesets\\SmallTiles\\{tileImage}");
            //}
            var tileImage = Path.GetFileNameWithoutExtension(item.Value.Image.source);
            _tilesetTextures[item.Key] = Globals.Content.Load<Texture2D>($"Tilesets/{tileImage}");
        }

        _scoreFont = Globals.Content.Load<SpriteFont>("Fonts/File");
        _healthFont = Globals.Content.Load<SpriteFont>("Fonts/HealthFont");

        // Scale map to fit window size
        _scaleFactor = Math.Min((float)Config.WindowSize.X / (_map.Width * _map.TileWidth), (float)Config.WindowSize.Y / (_map.Height * _map.TileHeight));
        _transformMatrix = Matrix.CreateScale(_scaleFactor, _scaleFactor, 1f);

        var playerLayer = _map.Layers.First(x => x.name == "player");

        //string playerClass = $"{MethodBase.GetCurrentMethod().DeclaringType.Namespace}.{playerLayer.@class}";
        //Type? playerType = Type.GetType(playerClass) ?? throw new ArgumentException($"Player layer class, {playerClass}, is not a valid Actor type");
        var playerStartObject = playerLayer.objects.First(x => x.name == "start");

        _playerStartPosition = new(playerStartObject.x, playerStartObject.y);

        setPlayerPosition(_playerStartPosition);
        //_player = (Actor)Activator.CreateInstance(playerType, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding, null, new object[] { _playerStartPosition, _scaleFactor }, null);


        var playerEndObject = playerLayer.objects.First(x => x.name == "end");
        Vector2 playerEndPos = new(playerEndObject.x, playerEndObject.y);
        Vector2 playerEndDimensions = new(playerEndObject.width, playerEndObject.height);
        _levelEnd = new LevelEnd(playerEndPos, playerEndDimensions);

        // Instantiate objects for each tile in collisions layer
        var collisionsLayer = _map.Layers.First(x => x.name == "collisions");
        LoadAndConjoinAdjacentTiles(collisionsLayer, _tiles);

        var invisibleBarrierLayer = _map.Layers.First(x => x.name == "invisibleBarriers");
        foreach (var barrier in invisibleBarrierLayer.objects)
        {
            Vector2 barrierPos = new(barrier.x, barrier.y);
            _invisibleBarriers.Add(new InvisibleBarrier(barrierPos, 1f, true));
        }


        _difficulty = _game.GetDifficulty();
        removeAllRemovables();
        loadRemovables();
        resetLevelInfo();


    }

    public void LoadAndConjoinAdjacentTiles(TiledLayer tiledLayer, params List<Tile>[] tileLists)
    {
        TiledLayerType layerType = tiledLayer.type;

        if (layerType == TiledLayerType.TileLayer)
        {
            // Check previously constructed tiles in order to create a single instance of Tile for adjacent tiles
            Dictionary<(int, int), Tile> traversedTiles = new();

            for (var y = 0; y < tiledLayer.height; y++)
            {
                for (var x = 0; x < tiledLayer.width; x++)
                {
                    var index = (y * tiledLayer.width) + x; // Assuming the default render order is used which is from right to bottom
                    var gid = tiledLayer.data[index]; // The tileset tile index
                    var tileX = x * _map.TileWidth;
                    var tileY = y * _map.TileHeight;

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                    {
                        continue;
                    }

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                    var mapTileset = _map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                    var tileset = _tilesets[mapTileset.firstgid];

                    // Use the connection object as well as the tileset to figure out the tile
                    var tile = _map.GetTiledTile(mapTileset, tileset, gid);

                    // Get tile type
                    string tileClass = $"{MethodBase.GetCurrentMethod().DeclaringType.Namespace}.{tile.type}";
                    Type? tileType = Type.GetType(tileClass) ?? throw new ArgumentException($"Tile type, {tileClass}, is not a valid Tile type");

                    // Get index of left and top tiles
                    // (don't care about bounds checking because out of bounds indices won't be in dictionary to begin with)
                    var leftTileIndex = (x - 1, y);
                    var topTileIndex = (x, y - 1);

                    // If either exists and is the same type as the current tile, extend with current tile
                    // Check left first so that we prioritize ground as opposed to walls
                    if (traversedTiles.TryGetValue(leftTileIndex, out Tile? leftTile) && leftTile.GetType() == tileType && !leftTile.IsWall)
                    {
                        // Extend left tile with current tile
                        int tileWidth = GetTileWidth(tileType.Name);

                        leftTile.ExtendRight(tileWidth);

                        traversedTiles[(x, y)] = leftTile;
                    }
                    else if (traversedTiles.TryGetValue(topTileIndex, out Tile? topTile) && topTile.GetType() == tileType && !topTile.IsGround)
                    {
                        // Extend top tile with current tile
                        int tileHeight = GetTileHeight(tileType.Name);

                        topTile.ExtendDown(tileHeight);

                        traversedTiles[(x, y)] = topTile;
                    }
                    else
                    {
                        // Get tile parameters and construct tile
                        Vector2 tilePos = new(tileX, tileY);
                        Tile newTile = (Tile)Activator.CreateInstance(tileType, tilePos, 1f);

                        // Add tile to dictionary and list
                        traversedTiles[(x, y)] = newTile;
                        foreach (var tileList in tileLists) tileList.Add(newTile);
                    }
                }
            }
        }
        else
        {
            // Check previously constructed tiles in order to create a single instance of Tile for adjacent tiles
            Dictionary<(float, float), Tile> traversedTiles = new();

            // Get object type
            string objClass = $"{MethodBase.GetCurrentMethod().DeclaringType.Namespace}.{tiledLayer.@class}";
            Type? objType = Type.GetType(objClass) ?? throw new ArgumentException($"Tile type, {objClass}, is not a valid Object type");

            foreach (var obj in tiledLayer.objects)
            {
                // Get object coordinates
                float x = obj.x;
                float y = obj.y;

                // Get coordinates of surrounding tiles
                var leftTilePos = (x - 8, y);
                var rightTilePos = (x + 8, y);
                var topTilePos = (x, y - 8);
                var bottomTilePos = (x, y + 8);

                // If either exists and is the same type as the current tile, extend with current tile
                // Check left first so that we prioritize ground as opposed to walls
                if (traversedTiles.TryGetValue(leftTilePos, out Tile? leftTile) && leftTile.GetType() == objType && !leftTile.IsWall)
                {
                    // Extend left tile with current tile
                    int tileWidth = GetTileWidth(objType.Name);

                    leftTile.ExtendRight(tileWidth);

                    traversedTiles[(x, y)] = leftTile;
                }
                else if (traversedTiles.TryGetValue(rightTilePos, out Tile? rightTile) && rightTile.GetType() == objType && !rightTile.IsWall)
                {
                    // Extend left tile with current tile
                    int tileWidth = GetTileWidth(objType.Name);

                    rightTile.ExtendLeft(tileWidth);

                    traversedTiles[(x, y)] = rightTile;
                }
                else if (traversedTiles.TryGetValue(topTilePos, out Tile? topTile) && topTile.GetType() == objType && !topTile.IsGround)
                {
                    // Extend top tile with current tile
                    int tileHeight = GetTileHeight(objType.Name);

                    topTile.ExtendDown(tileHeight);

                    traversedTiles[(x, y)] = topTile;
                }
                else if (traversedTiles.TryGetValue(bottomTilePos, out Tile? bottomTile) && bottomTile.GetType() == objType && !bottomTile.IsGround)
                {
                    // Extend left tile with current tile
                    int tileHeight = GetTileHeight(objType.Name);

                    bottomTile.ExtendUp(tileHeight);

                    traversedTiles[(x, y)] = bottomTile;
                }
                else
                {
                    // Get object parameters and construct object
                    Vector2 objPos = new(x, y);
                    Tile newTile = (Tile)Activator.CreateInstance(objType, objPos, 1f);

                    // Add object to dictionary and list(s)
                    traversedTiles[(x, y)] = newTile;
                    foreach (var tileList in tileLists) tileList.Add(newTile);
                }
            }
        }
    }

    public void removeAllRemovables()
    {

        List<SolidPlatform> tiledRemove = _nonstatictiles.OfType<SolidPlatform>().ToList(); ;
        foreach (var item in tiledRemove)
        {
            _tiles.Remove(item);
            _nonstatictiles.Remove(item);
        }

        _notes.Clear();
        _nonPhysicals.Clear();
    }


    public void loadRemovables()
    {

        var movingplatformLayer = _map.Layers.First(x => x.name == "movingplatforms");
        LoadAndConjoinAdjacentTiles(movingplatformLayer, _tiles, _nonstatictiles);

        var timedplatformLayer = _map.Layers.First(x => x.name == "timedplatforms");
        foreach (var o in timedplatformLayer.objects)
        {
            Vector2 nonstaticPos = new(o.x, o.y);
            TimedPlatform t = new TimedPlatform(nonstaticPos);
            _tiles.Add(t);
            _nonstatictiles.Add(t);
        }

        var notesLayer = _map.Layers.First(x => x.name == "notes");
        foreach (var note in notesLayer.objects)
        {
            Vector2 notePos = new(note.x, note.y);
            _notes.Add(new Note(notePos));
            _totalNotes++;
        }

        var platformEnemiesLayer = _map.Layers.First(x => x.name == "platformEnemies");
        foreach (var enemy in platformEnemiesLayer.objects)
        {
            Vector2 enemyPos = new(enemy.x, enemy.y);
            _nonPhysicals.Add(new PlatformEnemy(enemyPos));
        }

        var followingEnemiesLayer = _map.Layers.First(x => x.name == "followingEnemies");
        foreach (var enemy in followingEnemiesLayer.objects)
        {
            Vector2 enemyPos = new(enemy.x, enemy.y);
            _nonPhysicals.Add(new FollowingEnemy(enemyPos));
        }

        var flyingEnemiesLayer = _map.Layers.First(x => x.name == "flyingEnemies");
        foreach (var enemy in flyingEnemiesLayer.objects)
        {
            Vector2 enemyPos = new(enemy.x, enemy.y);
            _nonPhysicals.Add(new FlyingEnemy(enemyPos));
        }

        foreach (var o in _nonstatictiles)
        {
            if (o is MovingPlatform)
            {
                ((MovingPlatform)o).Load();
            }
            else if (o is TimedPlatform)
            {
                ((TimedPlatform)o).Load();
            }
        }

        foreach (var o in Notes)
        {
            o.Load();
        }
    }

    public static int GetTileHeight(string tileType)
    {
        return tileType switch
        {
            "ImpassableTile" => ImpassableTile.UnscaledHeight,
            "SolidPlatform" => SolidPlatform.UnscaledHeight,
            "SemiSolidPlatform" => SemiSolidPlatform.UnscaledHeight,
            "BouncyPlatform" => BouncyPlatform.UnscaledHeight,
            "TimedPlatform" => TimedPlatform.UnscaledHeight,
            "MovingPlatform" => MovingPlatform.UnscaledHeight,
            _ => 0,
        };
    }

    public static int GetTileWidth(string tileType)
    {
        return tileType switch
        {
            "ImpassableTile" => ImpassableTile.UnscaledWidth,
            "SolidPlatform" => SolidPlatform.UnscaledWidth,
            "SemiSolidPlatform" => SemiSolidPlatform.UnscaledWidth,
            "BouncyPlatform" => BouncyPlatform.UnscaledWidth,
            "TimedPlatform" => TimedPlatform.UnscaledWidth,
            "MovingPlatform" => MovingPlatform.UnscaledWidth,
            _ => 0,
        };
    }

    public void setPlayerPosition(Vector2 pos)
    {
        _player.Position = pos;
    }

    public void resetLevelInfo()
    {
        _songHealth = 100;
        _healthInt = healthDifficulty();
        _songHealthDifficultyDecrement = songHealthDifficulty();
        _score = _game._level1_score;
        _scoreString = "Score: " + _score;
        _player.isHurt = false;
        setPlayerPosition(Globals.CurrentLevel._playerStartPosition);
        _flyingEnemyExists = false;
    }

    public void Reset()
    {
        Globals.CurrentLevel = this;
        Globals.CurrentLevel._firstLaunch = false;
        _difficulty = _game.GetDifficulty();
        removeAllRemovables();
        loadRemovables();
        resetLevelInfo();
    }

    public void Update()
    {

        _player.Update();

        if (!_player.InBounds(Globals.CurrentLevel._levelBounds))
        {
            Reset();
            return;
        }

        foreach (Note o in _notes)
        {
            o.Update();
        }
        foreach (Tile o in _tiles)
        {
            o.Update();
        }

        foreach (Nonphysical n in _nonPhysicals)
        {
            n.Update();
        }

        if (toRemove != null)
        {
            if (toRemove is Tile tile)
            {
                _tiles.Remove(tile);
                _nonstatictiles.Remove(tile);
            }
            if (toRemove is Note note)
            {
                _score += 200;
                _songHealth += 10;
                _notes.Remove(note);
                _scoreString = "Score: " + _score;
            }

        }
        toRemove = null;

        if (toRemoveEnemy != null)
        {
            _nonPhysicals.Remove(toRemoveEnemy);
        }
        toRemoveEnemy = null;

        _timer = handleTime();

        handleSongHealth();

        if (Globals.CurrentLevel == _game._tutorial && _player.Position.X > 1500 && !_flyingEnemyExists)
        {
            _nonPhysicals.Add(new FlyingEnemy(new Vector2(1800, 100)));
            _nonPhysicals.Add(new FlyingEnemy(new Vector2(1700, 100)));
            _nonPhysicals.Add(new FlyingEnemy(new Vector2(1750, 120)));
            _flyingEnemyExists = true;
        }
    }


    public void Draw()
    {
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _game.GetCamera().Transform); // moved from level
        var tileLayers = _map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

        foreach (var layer in tileLayers)
        {
            for (var y = 0; y < layer.height; y++)
            {
                for (var x = 0; x < layer.width; x++)
                {
                    var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                    var gid = layer.data[index]; // The tileset tile index
                    var tileX = x * _map.TileWidth;
                    var tileY = y * _map.TileHeight;

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                    {
                        continue;
                    }

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                    var mapTileset = _map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                    var tileset = _tilesets[mapTileset.firstgid];
                    var tilesetTexture = _tilesetTextures[mapTileset.firstgid];
                    //var tile = _map.GetTiledTile(mapTileset, tileset, gid);
                    //var tilesetTexture = _tilesetTextures[mapTileset.firstgid][tile.id];

                    // Use the connection object as well as the tileset to figure out the source rectangle
                    var rect = _map.GetSourceRect(mapTileset, tileset, gid);

                    // Create destination and source rectangles
                    var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                    var destination = new Rectangle(tileX, tileY, _map.TileWidth, _map.TileHeight);

                    Globals.SpriteBatch.Draw(tilesetTexture, destination, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                }
            }
        }

        foreach (Tile t in _nonstatictiles)
        {
            t.Draw();
        }

        //if (Globals.KeyboardState.IsKeyDown(Keys.P))
        //{
        //    foreach (Tile t in _tiles)
        //    {
        //        t.Draw();
        //    }
        //}

        foreach (Note i in _notes)
        {
            i.Draw();
        }

        foreach (Nonphysical n in _nonPhysicals)
        {
            n.Draw();
        }

        _player.Draw();


        // Health bar


        Globals.SpriteBatch.Draw(healthBarTexture, new Vector2(_player.Position.X - 70, _player.Position.Y - 89), new Rectangle(0, 0, _songHealth, 5), Color.Black);
        Globals.SpriteBatch.DrawString(_healthFont, "Song Health", _player.Position + new Vector2(-120, -90), Color.Black, 0, new Vector2(0, 0), 0.15f, SpriteEffects.None, 0);


        Globals.SpriteBatch.Draw(healthBarTexture, new Vector2(_player.Position.X - 70, _player.Position.Y - 82), new Rectangle(0, 0, _healthInt / 3, 5), Color.White);
        Globals.SpriteBatch.DrawString(_healthFont, "Player Health", _player.Position + new Vector2(-120, -83), Color.Red, 0, zeroVector, 0.15f, SpriteEffects.None, 0);

      


        Globals.SpriteBatch.Draw(healthBarTexture, _player.Position + new Vector2(88, -82), new Rectangle(0, 0, (int)_healthFont.MeasureString(_scoreString).X, (int)_healthFont.MeasureString(_scoreString).Y), Color.Black, 0, new Vector2(0,0), 0.15f, SpriteEffects.None, 0);


        Globals.SpriteBatch.DrawString(_healthFont, _scoreString, _player.Position + new Vector2(88, -82), Color.Yellow, 0, zeroVector, 0.15f, SpriteEffects.None, 0);

        Globals.SpriteBatch.DrawString(_healthFont, _timer, _player.Position + new Vector2(88, 82), Color.Black, 0, zeroVector, 0.25f, SpriteEffects.None, 0);

        if (this == _game._level1)
        {
            Globals.SpriteBatch.DrawString(_healthFont, levelNames[0], _player.Position + new Vector2(Config.WindowSize.X / -10, 82), Color.Black, 0, zeroVector, 0.25f, SpriteEffects.None, 0);
        }
        else if (this == _game._level2)
        {
            Globals.SpriteBatch.DrawString(_healthFont, levelNames[1], _player.Position + new Vector2(Config.WindowSize.X / -10, 82), Color.Black, 0, zeroVector, 0.25f, SpriteEffects.None, 0);
        }
        else if (this == _game._tutorial)
        {
            for (int i = 0; i < _game._tutorialStrings.Count; i++)
            {
                Globals.SpriteBatch.DrawString(_healthFont, _game._tutorialStrings[i][0], _game._tutorialStringPositions[i], getColorFromString(_game._tutorialStrings[i][1]), 0, zeroVector, 0.15f, SpriteEffects.None,0);
            }
        }

        
        if (_game.paused) Globals.SpriteBatch.DrawString(_healthFont, _game._winningMessage, _game._winningMessagePosition, Color.Black, 0, Vector2.Zero, 0.15f, SpriteEffects.None, 0);


        Globals.SpriteBatch.End();
    }
}
