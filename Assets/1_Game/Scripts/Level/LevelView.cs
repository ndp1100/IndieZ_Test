using System.Collections.Generic;
using UnityEngine;

public sealed class LevelView : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private PlayerView _playerView;

    [SerializeField] private List<EnemyView> _enemies;

    public Camera MainCamera => _mainCamera;
    public GameObject PlayerCamera => _playerCamera;
    public PlayerView PlayerView => _playerView;
    public List<EnemyView> Enemies => _enemies;
}