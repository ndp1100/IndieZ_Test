using System;
using System.IO;
using System.Text;
using Core;
using Game.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Domain
{
    [Serializable]
    public sealed class GameModel : Observable
    {
        private static string _gameSaveFilename = "/xoi_supermarket_vr.json";
        private static string _saveGameOverride;

        public GameData GameData { get; private set; }

        public static GameModel Load(GameConfig config, string saveGameOverride)
        {
            var gameModel = new GameModel();
            _saveGameOverride = saveGameOverride;

            gameModel.Prepare(config);
            return gameModel;
        }

        private void Prepare(GameConfig config)
        {
            LoadGameData(config);
        }

        public static string GetGameSavePath()
        {
            if (!string.IsNullOrEmpty(_saveGameOverride))
                return $"{Application.persistentDataPath}/{_saveGameOverride}";

            return $"{Application.persistentDataPath}{_gameSaveFilename}";
        }

        private void SaveGame()
        {
            var gameSavePath = GetGameSavePath();
            YOLogger.LogTemporaryChannel("SaveGame", $"Save Game Success : {gameSavePath}");
            var content = JsonConvert.SerializeObject(GameData, Formatting.Indented);

            File.WriteAllText(gameSavePath, content);
        }

        public void SaveGameData()
        {
            SaveGame();
        }

        private GameData LoadGameData(GameConfig gameConfig)
        {
            try
            {
                if (GameData == null)
                {
                    var gameSavePath = GetGameSavePath();

                    if (File.Exists(gameSavePath))
                    {
                        YOLogger.Log($"Loading game save : {gameSavePath}");

                        var bytes = File.ReadAllBytes(gameSavePath);
                        var text = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                        try
                        {
                            GameData = JsonConvert.DeserializeObject<GameData>(text);
                        }
                        catch (Exception e)
                        {
                            YOLogger.Error($">>> Parse GameSave error: {e.Message}");
                        }
                    }
                    else
                    {
                        YOLogger.Log("Game save not found, starting a new game!");
                    }
                }
            }
            catch (Exception ex)
            {
                YOLogger.Error($"Failed to load saved game due to: {ex}");
            }

            if (GameData == null /*|| AlwaysStartNewGame*/)
            {
                GameData = new GameData();

                SaveGame();
            }

            return GameData;
        }


        #region Function Level Experience GameData

        public void AddExperience(int amount)
        {
            GameData.Experience += amount;
            SetChanged();
        }

        public void LevelUp()
        {
            GameData.Level++;
            SetChanged();
        }

        #endregion
    }
}