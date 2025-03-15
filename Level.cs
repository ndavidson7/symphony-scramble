using DotTiled;
using DotTiled.Serialization;
using SymphonyScramble.Models;
using SymphonyScramble.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Object = DotTiled.Object;

namespace SymphonyScramble;

internal class Level : Scene
{
    private static readonly Loader _loader = Loader.Default();

    private readonly Game1 _game;
    private readonly string _mapName;

    // Tiled-related fields
    private Map _map;
    private Dictionary<uint, Tileset> _tilesets;
    private readonly Dictionary<uint, Texture2D> _tilesetTextures = [];

    // Level objects
    private Actor? _player;
    private BoundingRectangle? _levelEnd;
    private List<Tile>? _tiles;
    private List<Note>? _notes;
    private List<FollowingEnemy>? _enemies;
    private List<InvisibleBarrier>? _invisibleBarriers;
    private Vector2? _levelBounds;

    public Level(Game1 game, string mapName)
    {
        _game = game;
        _mapName = mapName;
    }

    public override void LoadContent()
    {
        _map = _loader.LoadMap(Path.Combine(Globals.Content.RootDirectory, "Maps", $"{_mapName}.tmx"));
        _tilesets = _map.Tilesets.ToDictionary(tileset => tileset.FirstGID.Value);

        foreach (var kvp in _tilesets)
        {
            var tileImageSource = Path.GetFileNameWithoutExtension(kvp.Value.Image.Value.Source);
            _tilesetTextures[kvp.Key] = Globals.Content.Load<Texture2D>($"Tilesets\\{tileImageSource}");
        }

        _levelBounds = new(_map.Width * _map.TileWidth, _map.Height * _map.TileHeight); // dimensions of the map in pixels
    }

    public override void Initialize()
    {
        ObjectLayer playerLayer = (ObjectLayer)_map.Layers.First(x => x.Name.Equals("player", StringComparison.OrdinalIgnoreCase));

        Object playerStartObject = playerLayer.Objects.First(x => x.Name.Equals("start", StringComparison.OrdinalIgnoreCase));
        Vector2 playerStartPosition = new(playerStartObject.X, playerStartObject.Y);
        _player = new Player(playerStartPosition);

        Object levelEndObject = playerLayer.Objects.First(x => x.Name.Equals("end", StringComparison.OrdinalIgnoreCase));
        _levelEnd = new(levelEndObject.X, levelEndObject.Y, levelEndObject.Width, levelEndObject.Height);

        // Instantiate objects for each tile in collisions layer
        TileLayer collisionsLayer = (TileLayer)_map.Layers.First(x => x.Name.Equals("collisions", StringComparison.OrdinalIgnoreCase));
        LoadAndConjoinAdjacentTiles(collisionsLayer);

        // TODO: instantiate all objects that have a state that can change over the course of a level (notes, enemies, etc.)
        ObjectLayer invisibleBarrierLayer = (ObjectLayer)_map.Layers.First(x => x.Name.Equals("invisibleBarriers", StringComparison.OrdinalIgnoreCase));
        foreach (Object barrier in invisibleBarrierLayer.Objects)
        {
            Vector2 barrierPos = new(barrier.X, barrier.Y);
            _invisibleBarriers.Add(new InvisibleBarrier(barrierPos, 1f, true));
        }
    }

    private void LoadAndConjoinAdjacentTiles(TileLayer tiledLayer)
    {
        TiledLayerType layerType = tiledLayer.;

        if (layerType == TiledLayerType.TileLayer)
        {
            // Check previously constructed tiles in order to create a single instance of Tile for adjacent tiles
            Tile?[] traversedTiles = new Tile?[tiledLayer.height * tiledLayer.width];

            for (int y = 0; y < tiledLayer.height; y++)
            {
                for (int x = 0; x < tiledLayer.width; x++)
                {
                    int index = (y * tiledLayer.width) + x; // Assuming the default render order is used which is from right to bottom
                    int gid = tiledLayer.data[index]; // The tileset tile index
                    int tileX = x * _map.TileWidth;
                    int tileY = y * _map.TileHeight;

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                        continue;

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                    TiledMapTileset mapTileset = _map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                    TiledTileset tileset = _tilesets[mapTileset.firstgid];

                    // Use the connection object as well as the tileset to figure out the tile
                    TiledTile tile = _map.GetTiledTile(mapTileset, tileset, gid);

                    // Get tile type
                    string tileClass = $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace}.{tile.type}";
                    Type tileType = Type.GetType(tileClass) ?? throw new ArgumentException($"Tile type, {tileClass}, is not a valid Tile type");

                    // Get index of left and top tiles
                    int leftTileIndex = index - 1;
                    int topTileIndex = index - tiledLayer.width;

                    // If either exists and is the same type as the current tile, extend with current tile
                    // Check left first so that we prioritize ground as opposed to walls
                    Tile? currentTile;
                    if (x > 0 && ((currentTile = traversedTiles[leftTileIndex]) is not null) && currentTile.GetType() == tileType && !currentTile.IsWall)
                    {
                        // Extend left tile with current tile
                        int tileWidth = GetTileWidth(tileType.Name);

                        currentTile.ExtendRight(tileWidth);

                        traversedTiles[index] = currentTile;
                    }
                    else if (y > 0 && ((currentTile = traversedTiles[topTileIndex]) is not null) && currentTile.GetType() == tileType && !currentTile.IsGround)
                    {
                        // Extend top tile with current tile
                        int tileHeight = GetTileHeight(tileType.Name);

                        currentTile.ExtendDown(tileHeight);

                        traversedTiles[index] = currentTile;
                    }
                    else
                    {
                        // Get tile parameters and construct tile
                        Vector2 tilePos = new(tileX, tileY);
                        Tile newTile = (Tile)(Activator.CreateInstance(tileType, tilePos, 1f) ?? throw new ArgumentException($"Tile type, {tileType}, could not be constructed"));

                        // Add tile
                        traversedTiles[index] = newTile;
                        _tiles?.Add(newTile);
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
                    _tiles?.Add(newTile);
                }
            }
        }
    }

    public static int GetTileHeight(Type tileType)
    {
        //return tileType switch
        //{
        //    "ImpassableTile" => ImpassableTile.UnscaledHeight,
        //    "SolidPlatform" => SolidPlatform.UnscaledHeight,
        //    "SemiSolidPlatform" => SemiSolidPlatform.UnscaledHeight,
        //    "BouncyPlatform" => BouncyPlatform.UnscaledHeight,
        //    "TimedPlatform" => TimedPlatform.UnscaledHeight,
        //    "MovingPlatform" => MovingPlatform.UnscaledHeight,
        //    _ => 0,
        //};
        return (int) tileType.GetMember("UnscaledHeight").FirstOrDefault()
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

    public override void Update(GameTime gameTime)
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


    public override void Draw()
    {
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _game.GetCamera().Transform); // moved from level
        
        foreach (TiledLayer layer in _map.Layers.Where(x => x.type == TiledLayerType.TileLayer))
        {
            for (int y = 0; y < layer.height; y++)
            {
                for (int x = 0; x < layer.width; x++)
                {
                    int index = (y * layer.width) + x;
                    int gid = layer.data[index]; // The tileset tile index
                    int tileX = x * _map.TileWidth;
                    int tileY = y * _map.TileHeight;

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                        continue;

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                    TiledMapTileset mapTileset = _map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                    TiledTileset tileset = _tilesets?[mapTileset.firstgid] ?? throw new Exception("Tilesets were not initialized correctly");
                    Texture2D tilesetTexture = _tilesetTextures[mapTileset.firstgid];

                    // Use the connection object as well as the tileset to figure out the source rectangle
                    TiledSourceRect rect = _map.GetSourceRect(mapTileset, tileset, gid);

                    // Create destination and source rectangles
                    Rectangle source = new(rect.x, rect.y, rect.width, rect.height);
                    Rectangle destination = new(tileX, tileY, _map.TileWidth, _map.TileHeight);

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
