using System;
using System.Collections.Generic;
using Game.UI.Hud;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField] public BaseHud[] Huds;

        [Header("Debug Editor")]
        [SerializeField] private bool _isUseDebugSave = false;
        [SerializeField] private string _debugSaveFilename = "/xoi_supermarket_vr_playground.json";

        public bool IsUseDebugSave => _isUseDebugSave;
        public string DebugSaveFilename => _debugSaveFilename;


        void Awake()
        {
            foreach (var hud in Huds)
            {
                hud.gameObject.SetActive(false);
            }
        }

        public IEnumerable<IHud> AllHuds()
        {
            foreach (var hud in Huds)
            {
                yield return hud;
            }
        }
    }
}