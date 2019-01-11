using KSP.IO;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FillItUp.Util;

using ClickThroughFix;
using ToolbarControl_NS;

namespace FillItUp
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class FillItUp : MonoBehaviour
    {
        static public FillItUp Instance;
        private int _windowId;
        private bool _toggleOn = false;
        private FuelTypes.StageResDef allShipResources;
        private SortedDictionary<int, FuelTypes.StageResDef> allPartsResourcesByStage;
        private SortedDictionary<int, FuelTypes.StageResDef> allPartsResourcesShip;

        private FillItUpConfigNode _config;
        private Rect _windowPosition;
        private FuelModel _resourcesByStage;
        private FuelModel _resourcesShip;

        GUIStyle _windowStyle;
        private void Awake()
        {
            Debug.Log("FillItUp Awake");

            _config = FillItUpConfigNode.LoadOrCreate();
            _windowId = WindowHelper.NextWindowId("FillItUp");
            _windowPosition = new Rect(_config.WindowX, _config.WindowY, 0, 0);
            _resourcesByStage = new FuelModel();
            _resourcesShip = new FuelModel();

            Instance = this;
            //GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            OnGUIAppLauncherReady();

            //GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
            GameEvents.onEditorLoad.Add(OnEditorLoad);
            GameEvents.onEditorUndo.Add(OnEditorUndo);
            GameEvents.onEditorShipModified.Add(OnEditorShipModified);

            GameEvents.onEditorLoad.Add(this.OnShipLoad);
            GameEvents.onPartPriorityChanged.Add(this.OnPartPriorityChanged);

            GameEvents.onPartRemove.Add(this.onPartAttachRemove);
            GameEvents.onPartAttach.Add(this.onPartAttachRemove);
            GameEvents.onEditorPartPlaced.Add(this.OnPartPriorityChanged);

            GameEvents.StageManager.OnGUIStageSequenceModified.Add(OnGUIStageSequenceModified);

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
        }

        void OnGUIStageSequenceModified()
        {
            Debug.Log("FillItUp.OnGUIStageSequenceModified");
            RebuildModel();
        }
        private void onPartAttachRemove(GameEvents.HostTargetAction<Part, Part> evt)
        {
            Debug.Log("FillItUp.OnPartPriorityChanged");
            RebuildModel();
        }
        private void OnPartPriorityChanged(Part p)
        {
            Debug.Log("FillItUp.OnPartPriorityChanged");
            RebuildModel();
        }
        private void OnShipLoad(ShipConstruct ship, CraftBrowserDialog.LoadType loadType)
        {
            Debug.Log("FillItUp.OnShipLoad");
            RebuildModel();
        }
        private void OnEditorShipModified(ShipConstruct sc)
        {
            Debug.Log("FillItUp.OnEditorShipModified");
            RebuildModel();
        }
        private void OnEditorUndo(ShipConstruct sc)
        {
            Debug.Log("FillItUp.OnEditorLoad");
            RebuildModel();
        }
        private void OnEditorLoad(ShipConstruct sc, CraftBrowserDialog.LoadType lt)
        {
            Debug.Log("FillItUp.OnEditorLoad");
            RebuildModel();
        }
        private void OnEditorPartEvent(ConstructionEventType data0, Part data1)
        {
            Debug.Log("FillItUp.OnEditorPartEvent");
            RebuildModel();
        }

        private void Update()
        {
            if (allShipResources == null)
                RebuildModel();
            if (_resourcesByStage != null) // implies _resourcesShip is also not null
            {
                if (byStages)
                {
                    _resourcesByStage.Apply(EditorLogic.fetch.ship, allPartsResourcesByStage);
                }
                else
                {
                    _resourcesShip.Apply(EditorLogic.fetch.ship,-1, allShipResources);
                }
            }
        }

        private void OnDestroy()
        {
            Save();

            //Clean up

            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
            GameEvents.onEditorLoad.Remove(OnEditorLoad);
            GameEvents.onEditorUndo.Remove(OnEditorUndo);
            GameEvents.onEditorShipModified.Remove(OnEditorShipModified);

            GameEvents.onEditorLoad.Remove(this.OnShipLoad);
            GameEvents.onPartPriorityChanged.Remove(this.OnPartPriorityChanged);

            GameEvents.onPartRemove.Remove(this.onPartAttachRemove);
            GameEvents.onPartAttach.Remove(this.onPartAttachRemove);
            GameEvents.onEditorPartPlaced.Remove(this.OnPartPriorityChanged);

            GameEvents.StageManager.OnGUIStageSequenceModified.Remove(OnGUIStageSequenceModified);
        }

        GUIStyle boldLabelFont;
        RectOffset horizOffset;
        bool fontInitted = false;
        private void OnGUI()
        {
           // GUI.skin = HighLogic.Skin;
            if (!fontInitted)
            {
                boldLabelFont = new GUIStyle(GUI.skin.label);
                boldLabelFont.fontStyle = FontStyle.Bold;

               horizOffset = GUI.skin.horizontalSlider.padding;
               // horizOffset.top += 5;
                fontInitted = true;
            }
            if (_toggleOn)
            {
                if (Event.current.type == EventType.Layout)
                {
                    _windowPosition.height = 100;
                    _windowPosition.width = 425;
                }

                _windowPosition = ClickThruBlocker.GUILayoutWindow(_windowId, _windowPosition, RenderWindowContent, "Fill It Up", _windowStyle);
            }
        }
        float allFuels = 100;
        internal SortedDictionary <int, float>  allFuelsByStage = new SortedDictionary<int, float>();

        bool byStages = false;
        internal bool ignoreLockedTanks = true;

        int maxLabelSize = 0;

        private void RenderWindowContent(int windowId)
        {
            maxLabelSize = 0;

            foreach (var s in allShipResources.resources)
            {
                Vector2 x = GUI.skin.label.CalcSize(new GUIContent(s.First));
                maxLabelSize = (int)Math.Max(maxLabelSize, x.x);

            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Mixer", boldLabelFont);
            GUILayout.FlexibleSpace();
            ignoreLockedTanks = GUILayout.Toggle(ignoreLockedTanks, "Ignore locked tanks");
            GUILayout.FlexibleSpace();

            byStages = GUILayout.Toggle(byStages, "By stages");
            GUILayout.EndHorizontal();
            if (!byStages)
                DoSingleStage( -1, allShipResources, _resourcesShip, ref allFuels);
            else
                DoByStage();

            GUI.DragWindow();
        }

        void DoSingleStage( int stage, FuelTypes.StageResDef _fuelTypes, FuelModel _resources, ref float allFuels)
        { 
            GUILayout.BeginHorizontal();
            GUILayout.Label("Use the sliders or [E]mpty and [F]ull buttons to adjust fuel in all tanks.");
            GUILayout.EndHorizontal();

            foreach (var ftype in _fuelTypes.resources)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(ftype.First, GUILayout.Width(maxLabelSize + 5));
                if (GUILayout.Button("E", GUILayout.Width(25)))
                {
                    _resources.Set(stage, ftype.Second, 0);
                }
                float f = _resources.Get(stage, ftype.Second) * 100;

                GUIStyle sliderStyle = new GUIStyle("horizontalslider");
                //sliderStyle.padding.top += 5;
                //GUI.skin.horizontalSlider = sliderStyle;
                var newf = GUILayout.HorizontalSlider(f, 0, 100,  GUILayout.Width(150));
                if (newf != f)
                {
                    newf = Math.Max(0, Math.Min(100, newf));
                    _resources.Set(stage, ftype.Second, newf / 100);
                }
                if (GUILayout.Button("F", GUILayout.Width(25)))
                {
                    _resources.Set(stage, ftype.Second, 1);
                }
                f = (float)Math.Round(_resources.Get(stage, ftype.Second) * 100);
                string s = GUILayout.TextField(f.ToString(), GUILayout.Width(35));
                try
                {
                    var f1 = float.Parse(s);
                    if (f1 != f)
                        _resources.Set(stage, ftype.Second, f / 100);
                } catch
                { }

                GUILayout.Label("%");
                GUILayout.EndHorizontal();
                
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("All Fuels", boldLabelFont);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Empty", GUILayout.Width(60)))
            {
                _resources.SetAll(stage, 0);
                allFuels = 0;
            }
            GUILayout.FlexibleSpace();
            var newAllFuels = GUILayout.HorizontalSlider(allFuels, 0, 100, GUILayout.Width(250));
            if (allFuels != newAllFuels)
            {
                allFuels = newAllFuels;
                _resources.SetAll(stage, allFuels / 100);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fill", GUILayout.Width(60)))
            {
                _resources.SetAll(stage, 1);
                allFuels = 100;
            }
            GUILayout.EndHorizontal();
    
        }

        // The variable to control where the scrollview 'looks' into its child elements.
        Vector2 scrollPosition;
        int numResVis = 0;
        void DoByStage()
        {
            if (numResVis > 20)
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(400), GUILayout.MaxHeight(700));
            int newNumResVis = 0;
            foreach (var s in allPartsResourcesByStage)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Stage " + (s.Key + 1).ToString()))
                {
                    s.Value.stageExpanded = !s.Value.stageExpanded;
                }
                GUILayout.EndHorizontal();
                newNumResVis++;                if (s.Value.stageExpanded)
                {  newNumResVis += 4;
                    newNumResVis += s.Value.resources.Count;
                    float f;
                    if (!allFuelsByStage.TryGetValue(s.Key, out f))
                        f = 100;
                    DoSingleStage(s.Key, s.Value, _resourcesByStage, ref f);
                    allFuelsByStage[s.Key] = f;
                }
            }
            // End the scrollview we began above.
            if (numResVis > 20)
                GUILayout.EndScrollView();
            numResVis = newNumResVis;
        }

        #region Utility



        private void RebuildModel()
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;
            Debug.Log("FillItUp.RebuildModel");
            FuelTypes.Discover(EditorLogic.fetch.ship, out allShipResources, out allPartsResourcesByStage, out allPartsResourcesShip);

            _resourcesByStage.SetFuelTypes(allShipResources);
            _resourcesShip.SetFuelTypes(allShipResources);
        }



        #endregion

        #region Persistance

        private void Save()
        {
            _config.WindowX = _windowPosition.x;
            _config.WindowY = _windowPosition.y;

            _config.Save();
        }

        #endregion

        #region App Launcher

        //ApplicationLauncherButton _afuButton = null;
        ToolbarControl toolbarControl = null;

        internal const string MODID = "FillItUp_NS";
        internal const string MODNAME = "Fill It Up";
        private void OnGUIAppLauncherReady()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(OnAFUToggle, OnAFUToggle,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                MODID,
                "fillItUpButton",
                "FillItUp/PluginData/FIUIcon_38",
                "FillItUp/PluginData/FIUIcon_24",
                MODNAME
            );
        }

        private void OnAFUToggle()
        {
            _toggleOn = !_toggleOn;
        }

        #endregion
    }
}
