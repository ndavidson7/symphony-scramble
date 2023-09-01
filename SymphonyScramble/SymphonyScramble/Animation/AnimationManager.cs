// Adapted from https://github.com/LubiiiCZ/DevQuickie/tree/master/Quickie004-SpritesheetAnimation

using System.Collections.Generic;
using System;
using SymphonyScramble.StateMachine.States.OnGroundStates;
using SymphonyScramble.StateMachine.States.InAirStates;
using Microsoft.Xna.Framework.Input;

namespace SymphonyScramble;

/// <summary>
/// An AnimationManager holds a collection of Animations for an individual animated Sprite
/// and handles the logic of playing each Animation.
/// Each Animation is mapped to by the State that Animation will be played during.
/// </summary>
public class AnimationManager
{
    #region Fields
    private Dictionary<PlayerState, Animation> _anims;
    private Dictionary<EnemyState, Animation> _enemyAnims;
    private PlayerState? _lastKey;
    private EnemyState? _enemyLastKey;

    private EnemyWalkState? eWalkState;
    private EnemyIdleState? eIdleState;
    private EnemyLandState? eLandState;
    private EnemyFallState? eFallState;
    private EnemyJumpState? eJumpState;
    private EnemyFlyState? eFlyState;

    private readonly bool _isEnemy;
    #endregion

    public AnimationManager()
    {
        _anims = new();
    }

    public AnimationManager(bool isEnemy)
    {
        _isEnemy = isEnemy;
        _enemyAnims = new();
    }

    public void AddAnimation(PlayerState key, Animation animation)
    {
        _anims.Add(key, animation);
        _lastKey ??= key;
    }

    public void AddAnimation(EnemyState key, Animation animation)
    {
        _enemyAnims.Add(key, animation);

        // This code is horrendous I'm so sorry.
        // Will make it cleaner if we need to, but this works rn.

        if (key is EnemyWalkState) eWalkState = key as EnemyWalkState;
        else if (key is EnemyIdleState) eIdleState = key as EnemyIdleState;
        else if (key is EnemyLandState) eLandState = key as EnemyLandState;
        else if (key is EnemyFallState) eFallState = key as EnemyFallState;
        else if (key is EnemyJumpState) eJumpState = key as EnemyJumpState;
        else if (key is EnemyFlyState) eFlyState = key as EnemyFlyState;


        _enemyLastKey ??= key;
    }

    public void RescaleAnimation(PlayerState key, float newScale)
    {
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Rescale(newScale);
        }
    }

    // This block is never called, but it will fail if it is. 
    public void RescaleAnimation(EnemyState key, float newScale)
    {
        if (_enemyAnims.TryGetValue(key, out Animation value))
        {
            value.Rescale(newScale);
        }
    }

    public EnemyState MatchNewStateToBaseState(EnemyState newState)
    {

        // Again, this code is horrendous.
        // When we get a new enemyState, match it by type to the base state, and use that instead. 
        if (newState is EnemyWalkState) return eWalkState;
        if (newState is EnemyIdleState) return eIdleState;
        if (newState is EnemyLandState) return eLandState;
        if (newState is EnemyFallState) return eFallState;
        if (newState is EnemyJumpState) return eJumpState;
        if (newState is EnemyFlyState)
        {
            return eFlyState;
        }
        return null;
    }

    public void Update(PlayerState key)
    {
        
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Start();
            _anims[key].Update();
            _lastKey = key;
        }
        else
        {
            _anims[_lastKey].Stop();
            _anims[_lastKey].Reset();
        }
    }

    public void Update(EnemyState newState)
    {
        
        EnemyState key = MatchNewStateToBaseState(newState); // Find the matching head state.
        if (_enemyAnims.TryGetValue(key, out Animation value))
        {
            _enemyAnims[key].Start();
            _enemyAnims[key].Update();
            _enemyLastKey = key;
        }
        else
        {
            _enemyAnims[_enemyLastKey].Stop();
            _enemyAnims[_enemyLastKey].Reset();
        }
    }

    public void Draw(Vector2 position, bool flip)
    {
        if (!_isEnemy)
        {
            if (Globals.CurrentLevel.Player.isHurt)
            {
                _anims[_lastKey].Draw(position, flip, true);
            }
            else _anims[_lastKey].Draw(position, flip);
        }
        else _enemyAnims[_enemyLastKey].Draw(position, flip);
    }

    public bool IsAnimFinished()
    {
        if (!_isEnemy) return _anims[_lastKey].IsFinished;
        return _enemyAnims[_enemyLastKey].IsFinished;
    }

    public void ResetAnimation()
    {
        if (!_isEnemy) _anims[_lastKey].Reset();
        else _enemyAnims[_enemyLastKey].Reset();
    }
}