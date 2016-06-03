//AFU
var AssemblyFuelUtility = importNamespace('AssemblyFuelUtility');
var FuelTypes = AssemblyFuelUtility.FuelTypes;

//Unity
var UnityEngine = importNamespace('UnityEngine');
var GUILayout = UnityEngine.GUILayout;
var GUIStyle = UnityEngine.GUIStyle;
var GUI = UnityEngine.GUI;
var Rect = UnityEngine.Rect;
var Color = UnityEngine.Color;
//GUI Settings
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

            if (ShipHasAnyFuelParts(_ship)) {
                renderHeadingLabel('Fuel Mixer');

                GUILayout.Label('Use the sliders or [E]mpty and [F]ull buttons to adjust fuel in all tanks.');
            }
            else {
                GUILayout.Label('The ship has no parts which contain fuel of any kind.');
            }

            if (ShipHasAnyFuelParts(_ship)) {

                GUILayout.Space(2);

                var fuelTypes = FuelTypes.AllNames();

                for (var i = 0; i < fuelTypes.length; i++) {
                    var fuelType = fuelTypes[i];

                    if (ShipHasAnyPartsContaining(_ship, fuelType)) {
                        _fuel.Set(fuelType, renderFuelControlGroup(_fuel.Get(fuelType), fuelType));
                    }
                }

                GUILayout.Space(15);

                _fuel.OverrideAllAmount = renderFuelControlGroup(_fuel.OverrideAllAmount, 'All Fuel Types');
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
    var setAmount = -1;

    GUILayout.BeginHorizontal();
    {
        if (label != null && label != '') {
            GUILayout.Label(label, GUILayout.Width(90));
        }

        if (GUILayout.Button("E", GUILayout.Width(_smallButtonDimension[0]), GUILayout.Height(_smallButtonDimension[1]))) { quickAmount = 0; }

        GUI.skin.horizontalScrollbar.margin.top = 5;
        amount = GUILayout.HorizontalScrollbar(amount, 0.1, 0, 1.1);

        if (GUILayout.Button("F", GUILayout.Width(_smallButtonDimension[0]), GUILayout.Height(_smallButtonDimension[1]))) { quickAmount = 1; }

        amount = quickAmount > -1 ? quickAmount : amount;

        setAmount = parseInt(GUILayout.TextField((amount * 100).toFixed(0), GUILayout.Width(30), GUILayout.Height(_smallButtonDimension[1])));

        if (setAmount != NaN && setAmount > -1) {
            var setAmountDecimal = (setAmount / 100);

            if (setAmountDecimal != currentAmount.toFixed(2)) {
                amount = setAmountDecimal;
            }
        }

        GUILayout.Label('%', GUILayout.Width(15));
    }
    GUILayout.EndHorizontal();

    return amount;
}
function setSmallButtonSkin() {
    GUI.skin.button.alignment = 1;
    GUI.skin.button.padding.top = 2;
    GUI.skin.button.padding.bottom = 0;
    GUI.skin.button.padding.right = 0;
    GUI.skin.button.padding.left = 0;
}
function setLargeButtonSkin() {
    GUI.skin.button.alignment = 4;
}
function renderHeadingLabel(text) {
    GUI.skin.label.fontSize = 15;
    GUILayout.Label(text, GUILayout.Height(25));
    setStandardGui();
}
function setStandardGui() {
    GUI.skin.label.fontSize = 12;
    GUI.skin.toggle.fontSize = 12;
    GUI.skin.button.fontSize = 12;
    GUI.skin.textField.fontSize = 11;
    GUI.contentColor = Color.white;
}