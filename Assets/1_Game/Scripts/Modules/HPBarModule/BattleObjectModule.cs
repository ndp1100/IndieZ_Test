using System;
using Cysharp.Threading.Tasks;
using Game.Level.Mine;
using Game.Level.Unit;
using Injection;
using UnityEngine;

namespace Game.Level.Modules
{
    public class BattleObjectModule : Module<BattleObjectModuleView>
    {
        [Inject] private Context _context;
        [Inject] private Injector _injector;

        public BattleObjectModule(BattleObjectModuleView view) : base(view)
        {
        }

        public override void Initialize()
        {
        }

        public HPBarView CreateHPBar(UnitController unit)
        {
            var hpBar = _view.HPBarPool.Get<HPBarView>();
            _injector.Inject(hpBar);

            hpBar.Show(unit);
            return hpBar;
        }

        public void RemoveHPBar(HPBarView hpBar)
        {
            hpBar.Dispose();
            _view.HPBarPool.Release(hpBar);
        }

        public void CreateDmgText(UnitController unit, float dmg, Color color)
        {
            var dmgText = _view.DmgTextPool.Get<DmgTextView>();
            _injector.Inject(dmgText);

            if (dmg > 0)
                dmgText.ShowDmgText(unit.View.UIPivotTransform, dmg.ToString("N0"), color);
            else
                dmgText.ShowDmgText(unit.View.UIPivotTransform, "MISS", color);
        }

        public void ReleaseDmgText(DmgTextView dmgText)
        {
            _view.DmgTextPool.Release(dmgText);
        }

        public MineController CreateMine(Vector3 position, MineModel model)
        {
            var mine = _view.MinePool.Get<MineView>();
            if (mine != null)
            {
                MineController mineController = new MineController(mine, model, position, _context);
                _injector.Inject(mineController);
                return mineController;
            }

            return null;
        }

        public void ReleaseMine(MineController mineController)
        {
            mineController.Dispose();

            UniTask.Delay(TimeSpan.FromSeconds(0.25f)).ContinueWith(() =>
            {
                _view.MinePool.Release(mineController.View);
            }).Forget();
            
        }

        public override void Dispose()
        {
            _view.Dispose();
        }
    }
}