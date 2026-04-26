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
    [HarmonyPatch(typeof(Input), nameof(Input.GetKey), new []{typeof(KeyCode)})]
    [HarmonyPrefix]
    public static bool GetButton(KeyCode key, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(key))
        {
            return true;
        }
        __result = PCInputSystem.GetState(key) == KeyState.Hold;
        return false;
    }
    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown),  new []{typeof(KeyCode)})]
    [HarmonyPrefix]
    public static bool GetButtonDown(KeyCode key, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(key))
        {
            return true;
        }
        __result = PCInputSystem.GetState(key) == KeyState.Pressed;
        return false;
    }
    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp),  new []{typeof(KeyCode)})]
    [HarmonyPrefix]
    public static bool GetButtonUp(KeyCode key, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(key))
        {
            return true;
        }
        __result = PCInputSystem.GetState(key) == KeyState.LetGo;
        return false;
    }
    [HarmonyPatch(typeof(Input), nameof(Input.GetKey), new []{typeof(string)})]
    [HarmonyPrefix]
    public static bool GetButton2(string name, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(name))
        {
            return true;
        }
        __result = PCInputSystem.GetState(name) == KeyState.Hold;
        return false;
    }
    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown),  new []{typeof(string)})]
    [HarmonyPrefix]
    public static bool GetButtonDown2(string name, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(name))
        {
            return true;
        }
        __result = PCInputSystem.GetState(name) == KeyState.Pressed;
        return false;
    }
    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp),  new []{typeof(string)})]
    [HarmonyPrefix]
    public static bool GetButtonUp2(string name, ref bool __result)
    {
        if(!PCInputSystem.ContainsInput(name))
        {
            return true;
        }
        __result = PCInputSystem.GetState(name) == KeyState.LetGo;
        return false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.isTouchOverUI))]
    [HarmonyPostfix]
    public static void isTouchOverUI(ref bool __result)
    {
        if (PCInputSystem.Editing)
        {
            __result = true;
        }
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
    private static GUIStyle TextStyle;
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
        TextStyle = new GUIStyle();
        MainButton = new Rect(Screen.width-50, Screen.height-50,50, 50);
        MainWindow = new Rect(0, 0, Screen.width/10, Screen.height);
    }

    public static void SaveConfig(string Path)
    {
        PCButtonSettings.ToSettings(Config).SaveToPath(Path);
    }

    void BeginEditing()
    {
        global::Config.paused = true;
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.enabled = false;
        }
    }

    void StopEditing()
    {
        global::Config.paused = false;
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.enabled = true;
        }
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
        if (GUILayout.Button("Select Key"))
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

    static bool CheckTouch()
    {
        if (Input.touchCount == 0) return true;
        
        var touch = Input.GetTouch(0);
        var pos = touch.position;
        pos.y = Screen.height - pos.y;
        if (MainWindow.Contains(pos))
        {
            return true;
        }
        foreach (var pair in Config.Inputs)
        {
            var button = pair.Value;

            if (button.ButtonRect.Contains(pos))
            {
                SelectedInput = button;
                return true;
            }
        }
        return false;
    }
    void MoveInputs()
    {
        if (!CheckTouch())
        {
            SelectedInput = null;
        }
        if (SelectedInput == null) return;
        var touch = Input.GetTouch(0);
        var pos = touch.position;
        pos.y = Screen.height - pos.y;
        SelectedInput.SetPos(pos);
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
    static void ManagePCInputs(string windowid)
    {
        if (GUILayout.Button("Save Settings"))
        {
            SaveConfig(Paths.PCInputConfigPath);
        }
        DrawKeySelector();
        if (SelectedInput != null)
        {
            SelectedInput.Name = GUILayout.TextField(SelectedInput.Name, TextStyle);
            if (pendingKey != KeyCode.None)
            {
                var old = GetKey(SelectedInput);
                if (!old.Equals(default(KeyCode)))
                    Config.Inputs.Remove(old);
                        
                Config.Inputs[pendingKey] = SelectedInput;
            }
        }
        else
        {
           string newname = GUILayout.TextField("New Button Name", TextStyle);
           if (GUILayout.Button("Create New Button") && pendingKey != KeyCode.None)
           {
               CreateNewButton(newname, pendingKey);
           }
        }
    }
}