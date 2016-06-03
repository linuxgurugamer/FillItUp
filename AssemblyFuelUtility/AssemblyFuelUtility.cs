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
        private string _jEngineSource = "";
        private JEngine _jEngine;
        private AssemblyFuelConfigNode _config;
        private Rect _windowPosition;
        private FuelModel _fuel;

        private void Awake()
        {
            Debug.Log("AssemblyFuelUtility Awake");

            _config = AssemblyFuelConfigNode.LoadOrCreate(GetEnsuredConfigPath());

            _windowId = WindowHelper.NextWindowId("AssemblyFuelUtility");
            _windowPosition = new Rect(_config.WindowX, _config.WindowY, 0, 0);
            _fuel = _config.FuelModel;

            _jEngine = CreateJintEngine();
            _jEngineSource = System.IO.File.ReadAllText(IOUtils.GetFilePathFor(typeof(AssemblyFuelUtility), "afu_scripts.js"));
            _jEngine.Execute(_jEngineSource);

            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        private void Update()
        {
            if (_fuel != null)
            {
                _fuel.Apply(EditorLogic.fetch.ship);

                _config.FuelModel = _fuel;
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
                _jEngine.SetValue("_ship", ship);
                _jEngine.SetValue("_toggleOn", _toggleOn);

                _jEngineSource = System.IO.File.ReadAllText(IOUtils.GetFilePathFor(typeof(AssemblyFuelUtility), "afu_scripts.js"));
                _jEngine.Execute(_jEngineSource);

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

        private bool ShipHasAnyPartsContaining(ShipConstruct ship, string fuelName)
        {
            foreach (var part in ship.parts)
            {
                if (part.Resources.list.Any(r => r.resourceName == fuelName)) return true;
            }

            return false;
        }

        private bool ShipHasAnyFuelParts(ShipConstruct ship)
        {
            foreach (var part in ship.parts)
            {
                if (part.Resources.list.Any(r => FuelTypes.AllNames().Contains(r.resourceName))) return true;
            }

            return false;
        }

        private JEngine CreateJintEngine()
        {
            var engine = new JEngine(cfg => cfg.
                AllowClr().
                AllowClr(typeof(GUILayout).Assembly).
                AllowClr(typeof(AssemblyFuelUtility).Assembly));
            
            engine.SetValue("ShipHasAnyFuelParts", new Func<ShipConstruct, bool>(ShipHasAnyFuelParts));
            engine.SetValue("ShipHasAnyPartsContaining", new Func<ShipConstruct, string, bool>(ShipHasAnyPartsContaining));

            return engine;
        }

        #endregion

        #region Persistance

        private void Save()
        {
            _config.WindowX = _windowPosition.x;
            _config.WindowY = _windowPosition.y;

            _config.Save(GetEnsuredConfigPath());
        }

        private string GetEnsuredConfigPath()
        {
            string path = IOUtils.GetFilePathFor(typeof(AssemblyFuelUtility), "afu_settings.cfg");
            string directory = System.IO.Path.GetDirectoryName(path);

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            return path;
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
