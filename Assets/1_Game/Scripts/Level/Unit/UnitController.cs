using System;
using Core;
using Game.Level.Modules;
using Injection;
using UnityEngine;

namespace Game.Level.Unit
{
    public enum UnitType
    {
        Player,
        Enemy
    }


    public class UnitModel : Observable
    {
        public float CurrentHP;
        public float MaxHP;
        public float RotateSpeed;
        public UnitType UnitType;
        public float WalkSpeed;

        public UnitModel()
        {
        }

        public UnitModel(UnitType unitType, float maxHp, float walkSpeed, float rotateSpeed)
        {
            UnitType = unitType;
            MaxHP = maxHp;
            CurrentHP = maxHp;
            WalkSpeed = walkSpeed;
            RotateSpeed = rotateSpeed;
        }
    }


    public class UnitController
    {
        private readonly BattleObjectModule _battleObjectModule;
        private HPBarView _hpBarView;

        public readonly Context SubContext;
        public readonly Injector SubInjector;
        public readonly UnitModel UnitModel;
        public readonly UnitView View;

        public UnitController(UnitView view, UnitModel model, Context context)
        {
            View = view;
            _battleObjectModule = context.Get<BattleObjectModule>();

            SubContext = new Context(context);
            SubContext.InstallByType(this, typeof(UnitController));

            SubInjector = new Injector(SubContext);
            SubContext.Install(SubInjector);

            SubInjector.Inject(View);

            UnitModel = model;

            var hpBarModule = context.Get<BattleObjectModule>();
            _hpBarView = hpBarModule.CreateHPBar(this);
            UnitModel.SetChanged();
        }

        public bool IsDead()
        {
            return UnitModel.CurrentHP <= 0;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead()) return;

            if(damage < 0) //miss
            {
                _battleObjectModule.CreateDmgText(this, damage, Color.magenta);
                return;
            }

            UnitModel.CurrentHP -= damage;
            UnitModel.SetChanged();

            _battleObjectModule.CreateDmgText(this, damage,
                UnitModel.UnitType == UnitType.Enemy ? Color.yellow : Color.red);

            YOLogger.LogTemporaryChannel("UnitController", $"Unit {View.transform.name} take {damage}");

            if (IsDead()) OnDead();
        }

        public virtual void OnDead()
        {
            YOLogger.LogTemporaryChannel("UnitController", $"Unit {View.transform.name} is dead");
            View.SetDeath();

            _battleObjectModule.RemoveHPBar(_hpBarView);
        }
    }
}