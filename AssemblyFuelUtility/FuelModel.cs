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
        private float _overrideAllAmount = 0;
        public float OverrideAllAmount
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

        public bool OverrideAllRatioClamp { get; set; }

        private Dictionary<FuelType, float> _model;

        public void SetAll(float amount)
        {
            if (_model == null) _model = new Dictionary<FuelType, float>();

            foreach(var fuelType in FuelTypes.All())
            {
                Set(fuelType, amount);
            }
        }

        public void Set(string typeName, float amount)
        {
            if (_model == null) _model = new Dictionary<FuelType, float>();

            var type = FuelTypes.FromString(typeName);

            if (type != FuelType.Unknown)
            {
                Set(type, amount);
            }
        }

        public void Set(FuelType type, float amount)
        {
            if (_model == null) _model = new Dictionary<FuelType, float>();

            if (!_model.ContainsKey(type) || amount != _model[type])
            {
                Changed = true;
            }

            _model[type] = amount;
        }

        public float Get(FuelType type)
        {
            if (_model == null) return -1;

            if (!_model.ContainsKey(type)) return -1;

            return _model[type];
        }

        public float Get(string typeName)
        {
            return Get(FuelTypes.FromString(typeName));
        }

        private bool Changed { get; set; }

        public void Apply(ShipConstruct ship)
        {
            if (Changed)
            {
                foreach (var part in ship.parts)
                {
                    foreach (var resource in part.Resources.list)
                    {
                        foreach (var fuelType in FuelTypes.All())
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
