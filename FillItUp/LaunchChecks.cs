using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace FillItUp
{
#if false
    public static class bb
    {
        public static int GetListenerNumber(this UnityEventBase unityEvent)
        {
            var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            var invokeCallList = field.GetValue(unityEvent);
            Log.Info("invokeCallList.GetType(): " + invokeCallList.GetType().ToString());


            var property = invokeCallList.GetType().GetProperty("Count");
            return (int)property.GetValue(invokeCallList);
        }
    }
#endif

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class LaunchChecks : MonoBehaviour
    {
        UnityAction launchDelegate;
        UnityAction defaultLaunchDelegate;

        const int WIDTH = 300;
        const int HEIGHT = 200;

        int btnId;
        internal void Start()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<FIU>().warnAtLaunch)
                return;
            Log.Info("FillItUp.Start 1");

            ButtonManager.BtnManager.InitializeListener(EditorLogic.fetch.launchBtn, EditorLogic.fetch.launchVessel, "Fill-It-Up");

            btnId = ButtonManager.BtnManager.AddListener(EditorLogic.fetch.launchBtn, OnLaunchButtonInput, "Fill-It-Up", "Fill-It-Up");
            Log.Info("FillItUp.Start, btnId: " + btnId);
            //launchDelegate = new UnityAction(OnLaunchButtonInput);
            //defaultLaunchDelegate = new UnityAction(EditorLogic.fetch.launchVessel);
            //Log.Info("FillItUp.defaultLaunchDelegate.name: " + defaultLaunchDelegate.Method.Name);


            //EditorLogic.fetch.launchBtn.onClick.RemoveListener(defaultLaunchDelegate);
            //EditorLogic.fetch.launchBtn.onClick.RemoveAllListeners();
            //EditorLogic.fetch.launchBtn.onClick.AddListener(launchDelegate);


            //Log.Info("GetListenerNumber: " + EditorLogic.fetch.launchBtn.onClick.GetListenerNumber());



            Log.Info("FillItUp.Start 2");

        }


        public void OnLaunchButtonInput()
        {
            if (!FuelLevelsFull())
            {
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                   new Vector2(0.5f, 0.5f),
                   new MultiOptionDialog("Fill It Up",
                       "The tanks are not full due to adjustments done with the Fill It Up mod\n\n" +
                       "Plese select your option from the choices below",
                       "Tanks Not Full",
                       HighLogic.UISkin,
                       new Rect(0.5f, 0.5f, WIDTH, HEIGHT),
                       new DialogGUIFlexibleSpace(),
                       new DialogGUIVerticalLayout(
                           new DialogGUIFlexibleSpace(),

                           new DialogGUIHorizontalLayout(
                               new DialogGUIFlexibleSpace(),
                               new DialogGUIButton("OK to launch",
                                   delegate
                                   {
                                       //ResetDelegates();
                                       Log.Info("FillItUp.OnLaunchButtonInput 1");
                                       //defaultLaunchDelegate();
                                       ButtonManager.BtnManager.InvokeNextDelegate(btnId, "Fill-It-Up-next");

                                   }, 240.0f, 30.0f, true),
                                new DialogGUIFlexibleSpace()
                            ),

                           new DialogGUIFlexibleSpace(),
                           new DialogGUIHorizontalLayout(
                               new DialogGUIFlexibleSpace(),
                               new DialogGUIButton("Fill all tanks and launch",
                                   delegate
                                   {
                                       ResetToFull();
                                       //ResetDelegates();
                                       Log.Info("FillItUp.OnLaunchButtonInput 2");
                                       //defaultLaunchDelegate();
                                       ButtonManager.BtnManager.InvokeNextDelegate(btnId, "Fill-It-Up-next");

                                   }, 240.0f, 30.0f, true),
                                new DialogGUIFlexibleSpace()
                            ),

                            new DialogGUIFlexibleSpace(),

                            new DialogGUIHorizontalLayout(
                               new DialogGUIFlexibleSpace(),
                               new DialogGUIButton("Cancel", () => { }, 240.0f, 30.0f, true),
                               new DialogGUIFlexibleSpace()
                               )
                           )
                       ),
                        false,
                        HighLogic.UISkin);
            }
            else
            {
                //ResetDelegates();
                Log.Info("FillItUp.OnLaunchButtonInput 3");
                //defaultLaunchDelegate();
                ButtonManager.BtnManager.InvokeNextDelegate(btnId, "Fill-It-Up-next");
            }
        }

        void OnExitButtonInput()
        {
            ResetDelegates();
        }

        internal void ResetDelegates()
        {
            EditorLogic.fetch.launchBtn.onClick.RemoveListener(launchDelegate);
            EditorLogic.fetch.launchBtn.onClick.AddListener(defaultLaunchDelegate);
        }


        bool FuelLevelsFull()
        {
            return FillItUp.Instance.TanksFull();
        }
        void ResetToFull()
        {
            FillItUp.Instance.SetToggleOff();
            FillItUp.Instance.ResetToFull();
        }
    }
}
