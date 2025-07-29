using UnityEngine;

namespace MyUnityPackage.Controller
{
    public enum EPlayerState
    {
        Idle,
        Walk,
        Run,
        Sprint,
        Jump,
        Fall,
        Climb,
    }
    public class PlayerState : MonoBehaviour
    {
        private EPlayerState playerState;

        public EPlayerState GetPlayerState() => playerState;
        public void SetPlayerState(EPlayerState _playerState) => playerState = _playerState;
        public bool IsGrounded() => IsGrounded(playerState);
        public bool IsGrounded(EPlayerState _playerState)
        {
            return _playerState == EPlayerState.Idle
                || _playerState == EPlayerState.Walk
                || _playerState == EPlayerState.Run
                || _playerState == EPlayerState.Sprint;

        }
    }

}
