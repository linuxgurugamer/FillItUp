using System.Collections.Generic;


namespace FillItUp
{
    public class FuelModel
    {
        private bool Changed = false;
        private FuelTypes.StageResDef fuelTypes;     
        private Dictionary<Tuple<string, int>, float> model;
        Tuple<string, int> key;

        public FuelModel()
        {
            model = new Dictionary<Tuple<string, int>, float>();
            fuelTypes = new FuelTypes.StageResDef();
        }

        public void SetFuelTypes(FuelTypes.StageResDef setfuelTypes)
        {
            this.fuelTypes = setfuelTypes;
        }

        public void SetAll(int stage, float amount)
        {
            foreach(var fuelType in fuelTypes.resources)
            {
                bool ignored = FillItUp.Instance._config.RuntimeLockedResources.ContainsKey(StageRes.Key2(stage, fuelType.First));
                if (!ignored)
                    Set(stage, fuelType.Second, amount);
            }
        }

        public void Set(int stage, string type, float amount)
        {

            if (model == null)
            {
                model = new Dictionary<Tuple<string, int>, float>();
            }
            key = new Tuple<string, int>(type, stage);
            float f;

            bool b = model.TryGetValue(key, out f) ? (amount != f) : true;
            if (b)
            {
                Changed = true;               
                model[key] = amount; 
            }
        }

        public float Get(int stage, string type)
        {
            if (model == null) return 1;
            key = new Tuple<string, int>(type, stage);
            float f;
            if (!model.TryGetValue(key, out f))
                f = 1;
            
            return f;
        }

        public bool AreTanksFull(ShipConstruct ship, SortedDictionary<int, FuelTypes.StageResDef> stages)
        {
            foreach (var s in stages)
            {
                if (!AreTanksFull(ship, s.Key, s.Value))
                    return false;
            }
            return true;
        }

        public bool AreTanksFull(ShipConstruct ship, int stage, FuelTypes.StageResDef stages)
        {
            foreach (var part in stages.parts)
            {
                foreach (var resource in part.Resources)
                {
                    foreach (var fuelType in stages.resources)
                    {
                        key = new Tuple<string, int>(fuelType.Second, stage);

                        if (resource.resourceName == fuelType.Second)
                        {
                            if (!FillItUp.Instance.ignoreLockedTanks || resource.flowState)
                            {
                                float f;
                                if (model.TryGetValue(key, out f))
                                {
                                    if (f < 1)
                                        return false;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return true;
        }

        public void Apply(ShipConstruct ship, SortedDictionary<int, FuelTypes.StageResDef> stages)
        {
            
            if (Changed)
            {
                foreach (var s in stages)
                {
                    Apply(ship, s.Key, s.Value, true);
                }
            }
        }

        public void Apply(ShipConstruct ship, int stage, FuelTypes.StageResDef stages, bool allStages = false)
        {
            if (ship == null || stages == null)
                return;
            if (Changed)
            {
                foreach (var part in stages.parts)
                {
                    foreach (var resource in part.Resources)
                    {
                        foreach (var fuelType in stages.resources)
                        {
                            if (resource.resourceName == fuelType.Second)
                            {
                                if (!FillItUp.Instance.ignoreLockedTanks || resource.flowState)
                                {
                                    float f;

                                    key = new Tuple<string, int>(fuelType.Second, stage);
                                    if (model.TryGetValue(key, out f))
                                    {
                                        resource.amount = resource.maxAmount * f;
                                    }
       
                                }
                                break;
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
                if (!allStages)
                    Changed = false;
            }
            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
