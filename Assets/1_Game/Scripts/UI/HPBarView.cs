using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Level.Unit;
using Injection;
using UnityEngine;
using UnityEngine.UI;

public class HPBarView : MonoBehaviour, IObserver, IDisposable
{
    [Inject] private LevelView _levelView;

    [SerializeField] private Slider _hpBar;

    private UnitController _unit;
    private bool _isPlayer;

    void Awake()
    {
        if (_hpBar == null)
            _hpBar = GetComponent<Slider>();

        if (_hpBar != null)
            _hpBar.value = 1;
    }

    void LateUpdate()
    {
        if (_unit == null || _levelView == null)
            return;

        Vector3 screenPos =
            _levelView.MainCamera.WorldToScreenPoint(_unit.View.UIPivotTransform.transform.position);

        transform.position = _isPlayer ? Vector3.Lerp(transform.position, screenPos, 100 * Time.deltaTime) : screenPos;
    }

    public void Show(UnitController unit)
    {
        _unit = unit;
        _unit.UnitModel.AddObserver(this);
        _isPlayer = _unit.UnitModel.UnitType == UnitType.Player;

        _hpBar.value = unit.UnitModel.CurrentHP / unit.UnitModel.MaxHP;
        Debug.Log("HPBarView.Show: " + _hpBar.value);
    }

    public void OnObjectChanged(Observable observable)
    {
        if (observable is UnitModel unitModel)
        {
            _hpBar.value = unitModel.CurrentHP / unitModel.MaxHP;
        }
    }

    public void Dispose()
    {
        _unit.UnitModel.RemoveObserver(this);
        _unit = null;
    }
}