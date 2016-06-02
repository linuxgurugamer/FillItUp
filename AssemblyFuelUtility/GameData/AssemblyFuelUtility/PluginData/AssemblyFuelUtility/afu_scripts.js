var UnityEngine = importNamespace('UnityEngine');
var GUILayout = UnityEngine.GUILayout;
var GUI = UnityEngine.GUI;
var Rect = UnityEngine.Rect;
var _buttonHeight = 25;
var _smallButtonDimension = [17, 17];

function renderMainGui() {
    setSmallButtonSkin();

    if (GUI.Button(new Rect(421, 2, 17, 17), "x")) {
        _toggleOn = false;
    }

    GUILayout.BeginHorizontal(GUILayout.Width(420));
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                if (ShipHasAnyFuelParts(_ship)) {
                    GUILayout.Label('Use the sliders or [E]mpty and [F]ull buttons to adjust fuel in all tanks.');
                }
                else {
                    GUILayout.Label('The ship has no parts which contain fuel of any kind.');
                }
            }
            GUILayout.EndHorizontal();

            if (ShipHasAnyFuelParts(_ship)) {

                GUILayout.Space(2);

                if (ShipHasAnyPartsContaining(_ship, 'LiquidFuel')) {
                    _fuel.LiquidFuel = renderFuelControlGroup(_fuel.LiquidFuel, 'Liquid Fuel');
                }

                if (ShipHasAnyPartsContaining(_ship, 'Oxidizer')) {
                    _fuel.Oxidizer = renderFuelControlGroup(_fuel.Oxidizer, 'Oxidizer');
                }

                if (ShipHasAnyPartsContaining(_ship, 'SolidFuel')) {
                    _fuel.SolidFuel = renderFuelControlGroup(_fuel.SolidFuel, 'Solid Fuel');
                }

                if (ShipHasAnyPartsContaining(_ship, 'MonoPropellant')) {
                    _fuel.Monoprop = renderFuelControlGroup(_fuel.Monoprop, 'Monoprop');
                }

                GUILayout.BeginHorizontal();
                {
                    setLargeButtonSkin();

                    if (GUILayout.Button("Empty All Tanks", GUILayout.Height(_buttonHeight))) {
                        _fuel.LiquidFuel = 0;
                        _fuel.Oxidizer = 0;
                        _fuel.SolidFuel = 0;
                        _fuel.Monoprop = 0;
                    }
                    if (GUILayout.Button("Fill All Tanks", GUILayout.Height(_buttonHeight))) {
                        _fuel.LiquidFuel = 1;
                        _fuel.Oxidizer = 1;
                        _fuel.SolidFuel = 1;
                        _fuel.Monoprop = 1;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }
    GUILayout.EndHorizontal();

    return [_toggleOn, _fuel];
}
function renderFuelControlGroup(currentAmount, label) {
    var amount = currentAmount;
    var quickAmount = -1;

    GUILayout.BeginHorizontal();
    {
        if (label != null && label != '') {
            GUILayout.Label(label, GUILayout.Width(65));
        }

        if (GUILayout.Button("E", GUILayout.Width(_smallButtonDimension[0]), GUILayout.Height(_smallButtonDimension[1]))) { quickAmount = 0; }

        GUI.skin.horizontalScrollbar.margin.top = 5;
        amount = GUILayout.HorizontalScrollbar(amount, 0.1, 0, 1.1);

        if (GUILayout.Button("F", GUILayout.Width(_smallButtonDimension[0]), GUILayout.Height(_smallButtonDimension[1]))) { quickAmount = 1; }

        amount = quickAmount > -1 ? quickAmount : amount;

        GUILayout.Label((amount * 100).toFixed(0) + '%', GUILayout.Width(33));
    }
    GUILayout.EndHorizontal();

    return amount;
}
function setSmallButtonSkin() {
    GUI.skin.button.alignment = 1;
    GUI.skin.button.padding.top = 0;
    GUI.skin.button.padding.bottom = 0;
    GUI.skin.button.padding.right = 0;
    GUI.skin.button.padding.left = 0;
}
function setLargeButtonSkin() {
    GUI.skin.button.alignment = 4;
}