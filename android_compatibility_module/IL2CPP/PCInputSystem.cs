using HarmonyLib;
using NeoModLoader.constants;
using NeoModLoader.services;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using static NeoModLoader.AndroidCompatibilityModule.Converter;
using Vector2 = UnityEngine.Vector2;
using Vector = System.Numerics.Vector2;
namespace NeoModLoader.AndroidCompatibilityModule.PCInputSystem;

static class PCInputPatches
{
    [HarmonyPatch(typeof(Input), nameof(Input.GetKey), new[] { typeof(KeyCode) })]
    [HarmonyPrefix]
    public static bool GetButton(KeyCode key, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(key))
        {
            return true;
        }

        __result = PCInputSystem.GetState(key) == KeyState.Hold;
        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(KeyCode) })]
    [HarmonyPrefix]
    public static bool GetButtonDown(KeyCode key, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(key))
        {
            return true;
        }

        __result = PCInputSystem.GetState(key) == KeyState.Pressed;
        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), new[] { typeof(KeyCode) })]
    [HarmonyPrefix]
    public static bool GetButtonUp(KeyCode key, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(key))
        {
            return true;
        }

        __result = PCInputSystem.GetState(key) == KeyState.LetGo;
        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKey), new[] { typeof(string) })]
    [HarmonyPrefix]
    public static bool GetButton2(string name, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(name))
        {
            return true;
        }

        __result = PCInputSystem.GetState(name) == KeyState.Hold;
        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(string) })]
    [HarmonyPrefix]
    public static bool GetButtonDown2(string name, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(name))
        {
            return true;
        }

        __result = PCInputSystem.GetState(name) == KeyState.Pressed;
        return false;
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), new[] { typeof(string) })]
    [HarmonyPrefix]
    public static bool GetButtonUp2(string name, ref bool __result)
    {
        if (!PCInputSystem.ContainsInput(name))
        {
            return true;
        }

        __result = PCInputSystem.GetState(name) == KeyState.LetGo;
        return false;
    }

    [HarmonyPatch(typeof(MoveCamera), nameof(MoveCamera.updateMobileCamera))]
    [HarmonyPrefix]
    public static bool CanMoveCamera()
    {
        return !PCInputSystem.Editing;
    }
}

public enum KeyState
{
    None,
    Hold,
    LetGo,
    Pressed
}
public class PCInput
{
    public Rect ButtonRect;
    public string Name;

    private bool isHeld;
    private bool pressedThisFrame;
    private bool releasedThisFrame;
    
    public void Press()
    {
        if (!isHeld)
            pressedThisFrame = true;

        isHeld = true;
    }
    public void Release()
    {
        if (isHeld)
            releasedThisFrame = true;

        isHeld = false;
    }

    public KeyState State
    {
        get
        {
            if (pressedThisFrame)
                return KeyState.Pressed;
            if (releasedThisFrame)
                return KeyState.LetGo;
            if (isHeld)
                return KeyState.Hold;
            return KeyState.None;
        }
    }

    public void EndFrame()
    {
        pressedThisFrame = false;
        releasedThisFrame = false;
    }

    public void SetPos(Vector2 pos)
    {
        ButtonRect = new Rect(pos.x, pos.y, ButtonRect.width, ButtonRect.height);
    }

    public void SetSize(Vector2 size)
    {
        ButtonRect = new Rect(ButtonRect.x, ButtonRect.y, size.x, size.y);
    }
}

public class PCInputConfig
{
    public Dictionary<KeyCode, PCInput> Inputs = new();
}
public class PCButtonSettings
{
    public class PCButton
    {
        public KeyCode Code;
        public string Name;
        public Vector Position;
        public Vector Size;
        public PCInput FromButton()
        {
            PCInput input = new PCInput();
            input.Name = Name;
            input.ButtonRect = new Rect(Position.X, Position.Y, Size.X, Size.Y);
            return input;
        }
        public static PCButton ToButton(PCInput input, KeyCode code)
        {
            PCButton pcButton = new PCButton();
            pcButton.Name = input.Name;
            pcButton.Size =  new Vector(input.ButtonRect.width, input.ButtonRect.height);
            pcButton.Position = new Vector(input.ButtonRect.x, input.ButtonRect.y);
            pcButton.Code = code;
            return pcButton;
        }
    }
    public List<PCButton> Buttons = new List<PCButton>();
    public static PCButtonSettings LoadFromPath(string Path)
    {
        try
        {
            string settings = File.ReadAllText(Path);
            return JsonConvert.DeserializeObject<PCButtonSettings>(settings);
        }
        catch
        {
            return new PCButtonSettings();
        }
    }
    public void SaveToPath(string Path)
    {
        try
        {
            string settings = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path, settings);
        }
        catch (Exception e)
        {
            LogService.LogError($"Failed to write PC Input Settings due to {e}");
        }
    }

    public PCInputConfig FromSettings()
    {
        var Config = new PCInputConfig();
        foreach (var button in Buttons)
        {
            Config.Inputs.Add(button.Code, button.FromButton());
        }
        return Config;
    }

    public static PCButtonSettings ToSettings(PCInputConfig config)
    {
        PCButtonSettings settings = new PCButtonSettings();
        foreach (var pair in config.Inputs)
        {
            settings.Buttons.Add(PCButton.ToButton(pair.Value, pair.Key));
        }
        return settings;
    }
}
public class Helper
{
    public static KeyCode GetKeyFromString(string name)
    {
        if (Enum.TryParse<KeyCode>(name, true, out var key))
            return key;
        return KeyCode.None;
    }
}
public class PCInputSystem : WrappedBehaviour
{
    public static KeyState GetState(KeyCode Code)
    {
        return Config.Inputs[Code].State;
    }
    public static KeyState GetState(string name)
    {
        return Config.Inputs[Helper.GetKeyFromString(name)].State;
    }

    public static bool ContainsInput(KeyCode Code)
    {
        return Config.Inputs.ContainsKey(Code);
    }
    public static bool ContainsInput(string name)
    {
        return Config.Inputs.ContainsKey(Helper.GetKeyFromString(name));
    }

    public static KeyCode GetKey(PCInput input)
    {
        return Config.Inputs.FirstOrDefault(x => x.Value == input).Key;
    }
    public static PCInputSystem Instance { get; private set; }
    public static bool Editing { get; private set; }
    private static Rect MainButton;
    private static Rect MainWindow;
    private static GUI.WindowFunction MainWindowFunction;
    private static PCInputConfig Config;
    public static void Init()
    {
        Harmony.CreateAndPatchAll(typeof(PCInputPatches), Others.harmony_id);
        Config = PCButtonSettings.LoadFromPath(Paths.PCInputConfigPath).FromSettings();
        Instance = WorldBoxMod.Transform.gameObject.AddComponent<PCInputSystem>();
        InitGUI();
    }

    static void InitGUI()
    {
        MainWindowFunction = C<GUI.WindowFunction>(ManagePCInputs);
        MainButton = new Rect(Screen.width-100, 0,100, 100);
        MainWindow = new Rect(0, 0, Screen.width/2.5f, Screen.height/5);
    }

    public static void SaveConfig(string Path)
    {
        PCButtonSettings.ToSettings(Config).SaveToPath(Path);
    }

    void BeginEditing()
    {
        global::Config.paused = true;
    }

    void StopEditing()
    {
        global::Config.paused = false;
        global::Config.ui_main_hidden = false;
    }
    private void OnGUI()
    {
        foreach (var pair in Config.Inputs)
        {
            var button = pair.Value;
            GUI.Box(button.ButtonRect, button.Name);
        }
        if (GUI.Button(MainButton, "PCInput"))
        {
            Editing = !Editing;
            if (Editing)
            {
                BeginEditing();
            }
            else
            {
                StopEditing();
            }
        }
        if (Editing)
        {
            global::Config.ui_main_hidden = true;
            GUI.Window(67, MainWindow, MainWindowFunction, "PCInputManager");
            MoveInputs();
        }
        else
        {
            CheckInputs();
        }
    }
    static PCInput SelectedInput;
    void CheckInputs()
    {
        foreach (var pair in Config.Inputs)
        {
            var button = pair.Value;

            bool inside = false;

            for (int i = 0; i < Input.touchCount; i++)
            {
                var touchPos = Input.GetTouch(i).position;
                
                touchPos.y = Screen.height - touchPos.y;

                if (button.ButtonRect.Contains(touchPos))
                {
                    inside = true;
                    break;
                }
            }

            if (inside)
                button.Press();
            else
                button.Release();
        }
    }
    void LateUpdate()
    {
        foreach (var input in Config.Inputs.Values)
        {
            input.EndFrame();
        }
    }

    static bool keySelectorOpen = false;
    private static Vector2 scrollPos;

    private static KeyCode pendingKey = KeyCode.None;

    private static void DrawKeySelector()
    {
        if (GUILayout.Button($"Selected Key: {pendingKey}"))
        {
            keySelectorOpen = !keySelectorOpen;
        }

        if (!keySelectorOpen)
            return;

        GUILayout.Label("Choose Key:");

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (GUILayout.Button(key.ToString()))
            {
                pendingKey = key;
                keySelectorOpen = false;
                break;
            }
        }

        GUILayout.EndScrollView();
    }

    enum TouchState
    {
        NoTouch,
        InWindow,
        Touch,
        MissTouch
    }
    static TouchState CheckTouch()
    {
        if (Input.touchCount == 0) return TouchState.NoTouch;
        
        var touch = Input.GetTouch(0);
        var pos = touch.position;
        pos.y = Screen.height - pos.y;
        if (MainWindow.Contains(pos))
        {
            return TouchState.InWindow;
        }
        foreach (var pair in Config.Inputs)
        {
            var button = pair.Value;

            if (button.ButtonRect.Contains(pos))
            {
                SelectedInput = button;
                return TouchState.Touch;
            }
        }
        return TouchState.MissTouch;
    }

    private TouchState PrevState;
    void MoveInputs()
    {
        var state = CheckTouch();
        if (state != TouchState.Touch)
        {
            switch (state)
            {
                case TouchState.MissTouch when PrevState == TouchState.NoTouch:
                    SelectedInput = null;
                    PrevState = state;
                    return;
                case TouchState.InWindow:
                case TouchState.NoTouch:
                    PrevState = state;
                    return;
            }
        }
        if (SelectedInput == null) return;
        var touch = Input.GetTouch(0);
        var pos = touch.position;
        pos.y = Screen.height - pos.y;
        SelectedInput.SetPos(pos - SelectedInput.ButtonRect.size/2);
    }
    public static PCInput CreateNewButton(string Name, KeyCode Code, Rect rect = default)
    {
        if (ContainsInput(Code))
        {
            return Config.Inputs[Code];
        }
        PCInput input = new PCInput { Name = Name, ButtonRect = rect };
        Config.Inputs.Add(Code, input);
        return input;
    }

    private static float currentX = 50;
    private static float currentY = 50;
    static void ManagePCInputs(int windowid)
    {
        if (GUILayout.Button("Save Settings"))
        {
            SaveConfig(Paths.PCInputConfigPath);
        }
        DrawKeySelector();
        GUILayout.Label("Button Size X");
        currentX = GUILayout.DoHorizontalSlider(
            currentX,
            0,
            300,
            GUI.skin.horizontalSlider,
            GUI.skin.horizontalSliderThumb,
            new[] { GUILayout.Width(200), GUILayout.Height(20) }
        );
        GUILayout.Label("Button Size Y");
        currentY = GUILayout.DoHorizontalSlider(
            currentY,
            0,
            300,
            GUI.skin.horizontalSlider,
            GUI.skin.horizontalSliderThumb,
            new[] { GUILayout.Width(200), GUILayout.Height(20) }
        );
        if (SelectedInput != null)
        {
            SelectedInput.SetSize(new Vector2(currentX, currentY));
          //  SelectedInput.Name = GUILayout.TextField(SelectedInput.Name, TextStyle); text field currently not supported
            if (pendingKey == KeyCode.None) return;
            var old = GetKey(SelectedInput);
            if (old == pendingKey || old == default) return;
            Config.Inputs.Remove(old);
            Config.Inputs[pendingKey] = SelectedInput;
            SelectedInput.Name = pendingKey.ToString();
            pendingKey = KeyCode.None;
        }
        else
        {
          // string newname = GUILayout.TextField("New Button Name", TextStyle); text field currently not supported
           if (GUILayout.Button("Create New Button") && pendingKey != KeyCode.None)
           {
               SelectedInput = CreateNewButton(pendingKey.ToString(), pendingKey, new Rect(Screen.width-(currentX+200), 200+currentY, currentX, currentY));
               pendingKey = KeyCode.None;
           }
        }
    }
}