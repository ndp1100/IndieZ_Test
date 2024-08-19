using Cysharp.Text;
using Game.Domain;
using Game.Level.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Hud
{
    public sealed class GamePlayHudView : BaseHudWithModel<SpawnModel>
    {
        [SerializeField] private Button _switchGunBtn;
        [SerializeField] private Button _mineBtn;
        [SerializeField] private TextMeshProUGUI _mineNumberTxt;
        [SerializeField] private TextMeshProUGUI _timerTxt;
        [SerializeField] private TextMeshProUGUI _waveNameTxt;


        [Header("Developer")] [SerializeField] private TMP_Text _FpsText;
        [SerializeField] private Button _restartButton;
        private bool _isDeveloperMode;
        private double dt;
        private double fps;

        private int frameCount;
        [SerializeField] private readonly float updateRate = 0.5f;

        public Button RestartButton => _restartButton;
        public Button SwitchGunBtn => _switchGunBtn;
        public Button MineBtn => _mineBtn;
        public TextMeshProUGUI MineNumberTxt => _mineNumberTxt;

        private void Awake()
        {
        }

        protected override void OnEnable()
        {
#if UNITY_DEVELOPMENT || UNITY_EDITOR
            _FpsText.gameObject.SetActive(true);
            _isDeveloperMode = true;
#else
            if (Debug.isDebugBuild)
            {
                _FpsText.gameObject.SetActive(true);
                _isDeveloperMode = true;
            }
            else
            {
                _FpsText.gameObject.SetActive(false);
                _isDeveloperMode = false;
            }
#endif
        }


        protected override void OnDisable()
        {
        }

        protected override void OnModelChanged(SpawnModel model)
        {
            if (model == null) return;

            if (_timerTxt)
            {
                _timerTxt.SetText(ConvertSecondToMinuteFormat((int)model.Time));
            }

            if (_waveNameTxt)
            {
                if (model.TimeToSpawnNextWave < 0)
                {
                    _waveNameTxt.SetText(model.WaveName);
                }
                else
                {
                    float timeToSpawnNextWave = model.TimeToSpawnNextWave - model.Time;
                    _waveNameTxt.SetText(ZString.Format("{0} Spawn in {1}", model.WaveName,
                        ConvertSecondToMinuteFormat((int)timeToSpawnNextWave)));
                }
            }
        }

        //convert time in second to string format 00:00 (minute:second)
        private string ConvertSecondToMinuteFormat(int time)
        {
            var minute = time / 60;
            var second = time % 60;

            return ZString.Concat($"{minute:D2}:{second:D2}");
        }

        private void Update()
        {
            if (_isDeveloperMode == false) return;

            frameCount++;
            dt += Time.deltaTime;
            if (dt > 1.0 / updateRate)
            {
                fps = frameCount / dt;
                frameCount = 0;
                dt -= 1.0 / updateRate;

                _FpsText.text = ZString.Format("FPS: {0}", fps.ToString("##.0"));
            }
        }
    }
}