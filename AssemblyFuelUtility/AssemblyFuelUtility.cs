using AssemblyFuelUtility.Settings;
using JEngine = Jint.Engine;
using KSP.IO;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssemblyFuelUtility.Util;

namespace AssemblyFuelUtility
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AssemblyFuelUtility : MonoBehaviour
    {
        private int _windowId;
        private bool _toggleOn = false;
        private string[] _fuelTypes;
        private string _jEngineSource = "";
        private float _timeSinceLastRebuild = 0;
        private JEngine _jEngine;
        private AssemblyFuelConfigNode _config;
        private Rect _windowPosition;
        private FuelModel _fuel;

        private void Awake()
        {
            Debug.Log("AssemblyFuelUtility Awake");

            _config = AssemblyFuelConfigNode.LoadOrCreate();
            _windowId = WindowHelper.NextWindowId("AssemblyFuelUtility");
            _windowPosition = new Rect(_config.WindowX, _config.WindowY, 0, 0);
            _fuel = new FuelModel();

            _jEngine = CreateJintEngine();
            _jEngineSource = System.IO.File.ReadAllText(IOUtils.GetFilePathFor(typeof(AssemblyFuelUtility), "afu_scripts.js"));
            _jEngine.Execute(_jEngineSource);

            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        private void Update()
        {
            _timeSinceLastRebuild += Time.deltaTime;

            if (_timeSinceLastRebuild >= 0.5)
            {
                _timeSinceLastRebuild = 0;

                RebuildModel();
            }

            if (_fuel != null)
            {
                _fuel.Apply(EditorLogic.fetch.ship);
            }
        }

        private void OnDestroy()
        {
            Save();

            //Clean up
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);

            if (_afuButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_afuButton);
            }
        }

        private void OnGUI()
        {
            if (_toggleOn)
            {
                if (Event.current.type == EventType.Layout)
                {
                    _windowPosition.height = 0;
                    _windowPosition.width = 0;
                }

                _windowPosition = GUILayout.Window(_windowId, _windowPosition, RenderWindowContent, "Assembly Fuel Utility");
            }
        }

        private void RenderWindowContent(int windowId)
        {
            try
            {
                var ship = EditorLogic.fetch.ship;

                //Variables
                _jEngine.SetValue("_fuel", _fuel);
                _jEngine.SetValue("_fuelTypes", _fuelTypes);
                _jEngine.SetValue("_ship", ship);
                _jEngine.SetValue("_toggleOn", _toggleOn);

                #if DEBUG

                _jEngineSource = System.IO.File.ReadAllText(IOUtils.GetFilePathFor(typeof(AssemblyFuelUtility), "afu_scripts.js"));
                _jEngine.Execute(_jEngineSource);

                #endif

                var state = (object[])_jEngine.Execute("renderMainGui();").GetCompletionValue().ToObject();

                _toggleOn = (bool)state[0];
                _fuel = (FuelModel)state[1];
            }
            catch (Exception ex)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(400));
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("Unable to render GUI. Check for mistakes in afu_scripts.js. Exception details are below.");
                        GUILayout.Label(ex.Message);
                        GUILayout.Label(ex.StackTrace);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

            GUI.DragWindow();
        }

        #region Utility

        private void RebuildModel()
        {
            _fuelTypes = FuelTypes.Discover(EditorLogic.fetch.ship);

            _fuel.SetFuelTypes(_fuelTypes);
        }

        private JEngine CreateJintEngine()
        {
            var engine = new JEngine(cfg => cfg.
                AllowClr().
                AllowClr(typeof(GUILayout).Assembly).
                AllowClr(typeof(AssemblyFuelUtility).Assembly));

            return engine;
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

        ApplicationLauncherButton _afuButton = null;

        private void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready)
            {
                _afuButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAFUToggle,
                    OnAFUToggle,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
                    (Texture)GameDatabase.Instance.GetTexture("AssemblyFuelUtility/AFUIcon", false));
            }
        }

        private void OnAFUToggle()
        {
            _toggleOn = !_toggleOn;
        }

        #endregion
    }
}
