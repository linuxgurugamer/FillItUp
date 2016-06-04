using KSP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssemblyFuelUtility
{
    public class FuelModel
    {
        private bool Changed { get; set; }
        private float _overrideAllAmount = 0;
        private string[] _fuelTypes = new string[0];
        private Dictionary<string, float> _model;

        public void SetFuelTypes(string[] fuelTypes)
        {
            _fuelTypes = fuelTypes;
        }

        public void SetAll(float amount)
        {
            if (_model == null) _model = new Dictionary<string, float>();

            foreach(var fuelType in _fuelTypes)
            {
                Set(fuelType, amount);
            }
        }

        public void Set(string type, float amount)
        {
            if (_model == null) _model = new Dictionary<string, float>();

            if (!_model.ContainsKey(type) || amount != _model[type])
            {
                Changed = true;
            }

            _model[type] = amount;
        }

        public float Get(string type)
        {
            if (_model == null) return -1;

            if (!_model.ContainsKey(type)) return -1;

            return _model[type];
        }
        
        public float OverrideAll
        {
            get
            {
                return _overrideAllAmount;
            }
            set
            {
                if (value != _overrideAllAmount)
                {
                    SetAll(value);

                    _overrideAllAmount = value;
                }
            }
        }

        public void Apply(ShipConstruct ship)
        {
            if (Changed)
            {
                foreach (var part in ship.parts)
                {
                    foreach (var resource in part.Resources.list)
                    {
                        foreach (var fuelType in _fuelTypes)
                        {
                            if (resource.resourceName == fuelType.ToString() && _model.ContainsKey(fuelType))
                            {
                                resource.amount = resource.maxAmount * _model[fuelType];
                            }
                        }
                    }
                }

                var resourceEditors = EditorLogic.FindObjectsOfType<UIPartActionResourceEditor>();

                foreach (var ed in resourceEditors)
                {
                    ed.resourceAmnt.text = ed.Resource.amount.ToString("F1");
                    ed.resourceMax.text = ed.Resource.maxAmount.ToString("F1");
                    ed.slider.value = (float)(ed.Resource.amount / ed.Resource.maxAmount);
                }
                
                GameEvents.onEditorShipModified.Fire(ship);

                Changed = false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
