using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Run once to create the editable scene. Runtime builds do not need this class.
public static class StartMenuBuilder
{
    private const string Art = "Assets/Game/Art/ZombiePack/Start Screen/";
    private const string ScenePath = "Assets/Scenes/StartScreen.unity";
    private static readonly Color Cream = new Color32(246, 231, 194, 255);
    private static readonly Color Orange = new Color32(215, 76, 29, 255);

    public static void Build()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            throw new InvalidOperationException("StartScreen already exists; refusing to overwrite it.");
        ImportArt();
        var sprites = AssetDatabase.LoadAllAssetsAtPath(Art + "Button.png").OfType<Sprite>().ToDictionary(s => s.name);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(22, 18, 17, 255);
        camera.orthographic = true;
        camera.transform.position = new Vector3(0, 0, -10);
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        var canvas = new GameObject("Start Screen Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1672, 941);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var backdrop = Rect("Backdrop", canvas.transform, Vector2.zero, Vector2.zero);
        Stretch(backdrop);
        backdrop.gameObject.AddComponent<Image>().color = new Color32(22, 18, 17, 255);
        var stage = Rect("Artwork and Menu", canvas.transform, Vector2.zero, new Vector2(1672, 941));
        var background = Rect("Background", stage, Vector2.zero, stage.sizeDelta).gameObject.AddComponent<Image>();
        background.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "Screen.png");
        background.raycastTarget = false;
        var menu = Rect("Main Buttons", stage, new Vector2(0, -91), new Vector2(400, 330));
        var play = ArtButton("Mulai", menu, 102, sprites);
        var settings = ArtButton("Pengaturan", menu, 0, sprites);
        var quit = ArtButton("Keluar", menu, -102, sprites);
        var panel = Rect("Settings Panel", stage, new Vector2(0, -91), new Vector2(540, 330));
        panel.gameObject.AddComponent<Image>().color = new Color32(28, 23, 21, 250);
        Label("Heading", panel, "PENGATURAN", new Vector2(0, 122), new Vector2(480, 46), 32);
        Label("Volume Label", panel, "VOLUME", new Vector2(-145, 58), new Vector2(180, 35), 22);
        var slider = Rect("Master Volume", panel, new Vector2(60, 58), new Vector2(260, 30)).gameObject.AddComponent<Slider>();
        var track = Rect("Track", slider.transform, Vector2.zero, new Vector2(260, 10));
        track.gameObject.AddComponent<Image>().color = new Color32(91, 66, 51, 255);
        var handleArea = Rect("Handle Area", slider.transform, Vector2.zero, new Vector2(240, 30));
        var handle = Rect("Handle", handleArea, Vector2.zero, new Vector2(20, 30));
        var handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.color = Orange;
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        slider.value = 1;
        var toggle = Rect("Fullscreen", panel, new Vector2(0, -7), new Vector2(390, 42)).gameObject.AddComponent<Toggle>();
        var box = Rect("Box", toggle.transform, new Vector2(-170, 0), new Vector2(32, 32));
        var boxImage = box.gameObject.AddComponent<Image>();
        boxImage.color = new Color32(91, 66, 51, 255);
        var check = Rect("Check", box, Vector2.zero, new Vector2(20, 20)).gameObject.AddComponent<Image>();
        check.color = Orange;
        toggle.targetGraphic = boxImage;
        toggle.graphic = check;
        Label("Label", toggle.transform, "LAYAR PENUH", new Vector2(25, 0), new Vector2(310, 40), 24);
        var back = Rect("Kembali", panel, new Vector2(0, -105), new Vector2(270, 55)).gameObject.AddComponent<Button>();
        back.targetGraphic = back.gameObject.AddComponent<Image>();
        back.targetGraphic.color = Orange;
        Label("Label", back.transform, "KEMBALI", Vector2.zero, new Vector2(260, 50), 25);
        var status = Label("Status", stage, "", new Vector2(0, -302), new Vector2(1000, 40), 24);
        Label("Controls", stage, "WASD / PANAH: GERAK    •    MOUSE: BIDIK    •    KLIK KIRI: TEMBAK", new Vector2(0, -368), new Vector2(1300, 35), 19);
        var controller = canvas.AddComponent<StartMenu>();
        var serialized = new SerializedObject(controller);
        Set(serialized, "mainButtons", menu.gameObject);
        Set(serialized, "settingsPanel", panel.gameObject);
        Set(serialized, "playButton", play);
        Set(serialized, "settingsButton", settings);
        Set(serialized, "backButton", back);
        Set(serialized, "volumeSlider", slider);
        Set(serialized, "fullscreenToggle", toggle);
        Set(serialized, "statusText", status);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(play.onClick, controller.Play);
        UnityEventTools.AddPersistentListener(settings.onClick, controller.OpenSettings);
        UnityEventTools.AddPersistentListener(quit.onClick, controller.Quit);
        UnityEventTools.AddPersistentListener(back.onClick, controller.CloseSettings);
        UnityEventTools.AddPersistentListener(slider.onValueChanged, controller.SetVolume);
        UnityEventTools.AddPersistentListener(toggle.onValueChanged, controller.SetFullscreen);
        panel.gameObject.SetActive(false);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }
            .Concat(EditorBuildSettings.scenes.Where(s => s.path != ScenePath)).ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("START_MENU_BUILD_OK: scene created, 9 button sprites, 6 UI actions wired.");
    }

    private static void ImportArt()
    {
        AssetDatabase.Refresh();
        foreach (string filename in new[] { "Screen.png", "Button.png" })
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Art + filename);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = filename == "Screen.png" ? SpriteImportMode.Single : SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.SaveAndReimport();
            if (filename != "Button.png") continue;
            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            var rects = new SpriteRect[9];
            string[] names = { "Mulai", "Pengaturan", "Keluar" };
            string[] states = { "Normal", "Hover", "Pressed" };
            // Coordinates use the original artwork's 1772 x 886 layout.
            float sx = width / 1772f, sy = height / 886f;
            for (int row = 0; row < 3; row++)
            for (int col = 0; col < 3; col++)
                rects[row * 3 + col] = new SpriteRect {
                    name = names[row] + states[col], spriteID = GUID.Generate(),
                    rect = new Rect((40 + col * 578) * sx, height - (98 + row * 249 + 194) * sy, 550 * sx, 194 * sy),
                    alignment = SpriteAlignment.Center, pivot = new Vector2(.5f, .5f)
                };
            provider.SetSpriteRects(rects);
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(rects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
        }
    }

    private static Button ArtButton(string name, Transform parent, float y, System.Collections.Generic.Dictionary<string, Sprite> sprites)
    {
        var rect = Rect(name, parent, new Vector2(0, y), new Vector2(330, 116));
        var image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprites[name + "Normal"];
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState { highlightedSprite = sprites[name + "Hover"], selectedSprite = sprites[name + "Hover"], pressedSprite = sprites[name + "Pressed"] };
        return button;
    }

    private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static Text Label(string name, Transform parent, string value, Vector2 position, Vector2 size, int fontSize)
    {
        var text = Rect(name, parent, position, size).gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.color = Cream;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    private static void Set(SerializedObject target, string name, UnityEngine.Object value)
    {
        target.FindProperty(name).objectReferenceValue = value;
    }

    public static void VerifyAndPreview()
    {
        EditorSceneManager.OpenScene(ScenePath);
        var menu = UnityEngine.Object.FindFirstObjectByType<StartMenu>();
        var data = new SerializedObject(menu);
        foreach (string field in new[] { "mainButtons", "settingsPanel", "playButton", "settingsButton", "backButton", "volumeSlider", "fullscreenToggle", "statusText" })
            if (data.FindProperty(field).objectReferenceValue == null)
                throw new InvalidOperationException("Missing menu reference: " + field);
        var buttons = menu.GetComponentsInChildren<Button>(true);
        if (buttons.Length != 4 || buttons.Any(b => b.onClick.GetPersistentEventCount() != 1))
            throw new InvalidOperationException("Button action wiring is incomplete.");
        if (EditorBuildSettings.scenes[0].path != ScenePath ||
            !EditorBuildSettings.scenes.Any(s => s.enabled && s.path == "Assets/Scenes/SampleScene.unity"))
            throw new InvalidOperationException("Menu/game scene build order is incorrect.");
        menu.OpenSettings();
        if (!((GameObject)data.FindProperty("settingsPanel").objectReferenceValue).activeSelf ||
            ((GameObject)data.FindProperty("mainButtons").objectReferenceValue).activeSelf)
            throw new InvalidOperationException("Opening settings failed.");
        Capture("Logs/StartScreen-settings.png");
        menu.CloseSettings();
        if (((GameObject)data.FindProperty("settingsPanel").objectReferenceValue).activeSelf ||
            !((GameObject)data.FindProperty("mainButtons").objectReferenceValue).activeSelf)
            throw new InvalidOperationException("Closing settings failed.");
        Capture("Logs/StartScreen-preview.png");
        Debug.Log("START_MENU_VERIFY_OK: references, button actions, settings navigation, build order, previews.");
    }

    private static void Capture(string path)
    {
        var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        var camera = Camera.main;
        var texture = new RenderTexture(1672, 941, 24);
        texture.Create();
        camera.targetTexture = texture;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1f;
        Canvas.ForceUpdateCanvases();
        camera.Render();
        var previous = RenderTexture.active;
        RenderTexture.active = texture;
        var pixels = new Texture2D(1672, 941, TextureFormat.RGB24, false);
        pixels.ReadPixels(new Rect(0, 0, 1672, 941), 0, 0);
        pixels.Apply();
        System.IO.File.WriteAllBytes(path, pixels.EncodeToPNG());
        RenderTexture.active = previous;
        camera.targetTexture = null;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        UnityEngine.Object.DestroyImmediate(pixels);
        UnityEngine.Object.DestroyImmediate(texture);
    }
}
