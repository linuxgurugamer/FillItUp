
using UnityEngine;
using UnityEngine.Events;



namespace FillItUp
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class LaunchChecks : MonoBehaviour
    {
        UnityAction launchDelegate;
        UnityAction defaultLaunchDelegate;

        const int WIDTH = 300;
        const int HEIGHT = 200;

        internal void Start()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<FIU>().warnAtLaunch)
                return;

            launchDelegate = new UnityAction(OnLaunchButtonInput);
            defaultLaunchDelegate = new UnityAction(EditorLogic.fetch.launchVessel);

            EditorLogic.fetch.launchBtn.onClick.RemoveListener(defaultLaunchDelegate);
            EditorLogic.fetch.launchBtn.onClick.AddListener(launchDelegate);

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
                                       ResetDelegates();
                                       defaultLaunchDelegate();
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
                                       ResetDelegates();
                                       defaultLaunchDelegate();
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
                ResetDelegates();
                defaultLaunchDelegate();
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
