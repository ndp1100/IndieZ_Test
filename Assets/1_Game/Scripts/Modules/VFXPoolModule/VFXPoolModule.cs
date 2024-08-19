using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Config;
using Game.UI.Pool;
using Injection;
using UnityEngine;

namespace Game.Level.Modules
{
    public class VFXPoolModule : Module<VFXPoolModuleView>
    {
        [Inject] private Context _context;

        [Inject] private GameConfig _gameConfig;
        [Inject] private GameManager _gameManager;
        [Inject] private Injector _injector;


        public VFXPoolModule(VFXPoolModuleView view) : base(view)
        {

        }

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public void CreateHitEffect(RaycastHit hit, float time)
        {
            string tag = hit.collider.tag;
            var hitEffect = _view.GetHitEffect(tag);
            hitEffect.position = hit.point;
            hitEffect.rotation = Quaternion.LookRotation(hit.normal);
            hitEffect.gameObject.SetActive(true);

            //delay time and auto release
            UniTask.Delay((int) (time * 1000)).ContinueWith(() =>
            {
                _view.ReleaseHitEffect(tag, hitEffect);
            }).Forget();

        }
    }
}