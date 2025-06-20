using UnityEngine;
using UnityEngine.UI;

// This is a helper script to automatically create the basic UI structure
public class GameEntryUISetup : MonoBehaviour
{
    [Header("Auto-Create UI")]
    public bool createUIOnStart = true;

    void Start()
    {
        if (createUIOnStart)
        {
            CreateBasicUI();
        }
    }

    [ContextMenu("Create Basic UI")]
    void CreateBasicUI()
    {
        // Create Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create Main Menu Panel
        GameObject mainMenuPanel = CreatePanel(canvas.transform, "MainMenuPanel");
        CreateButton(mainMenuPanel.transform, "New Game Button", new Vector2(0, 50));
        CreateButton(mainMenuPanel.transform, "Quit Button", new Vector2(0, -50));

        // Create Save Slot Panel
        GameObject saveSlotPanel = CreatePanel(canvas.transform, "SaveSlotPanel");
        saveSlotPanel.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            GameObject slotButton = CreateButton(saveSlotPanel.transform, $"Save Slot {i + 1}",
                new Vector2(0, 100 - (i * 80)));
            // Make it bigger for save slot info
            slotButton.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 60);
        }

        // Add level slider
        GameObject levelSlider = CreateSlider(saveSlotPanel.transform, "Level Slider", new Vector2(0, -150));
        GameObject levelText = CreateText(saveSlotPanel.transform, "Level: 1", new Vector2(0, -120));

        CreateButton(saveSlotPanel.transform, "Back Button", new Vector2(-150, -200));

        // Create Character Selection Panel
        GameObject charPanel = CreatePanel(canvas.transform, "CharacterSelectionPanel");
        charPanel.SetActive(false);

        CreateText(charPanel.transform, "Select Character", new Vector2(0, 150));

        // Character button container
        GameObject charContainer = new GameObject("Character Container");
        charContainer.transform.SetParent(charPanel.transform);
        RectTransform containerRect = charContainer.AddComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 50);
        containerRect.sizeDelta = new Vector2(600, 200);

        // Add Grid Layout for character buttons
        GridLayoutGroup grid = charContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(120, 60);
        grid.spacing = new Vector2(10, 10);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // Create a sample character button prefab
        GameObject charButtonPrefab = CreateButton(charContainer.transform, "Character Button", Vector2.zero);
        charButtonPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 60);

        CreateText(charPanel.transform, "Selected: None", new Vector2(0, -50));
        CreateButton(charPanel.transform, "Confirm Button", new Vector2(0, -100));
        CreateButton(charPanel.transform, "Back Button", new Vector2(-150, -150));

        Debug.Log("Basic UI structure created! Now assign references in GameEntryManager.");
    }

    GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        return panel;
    }

    GameObject CreateButton(Transform parent, string name, Vector2 position)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent);

        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        rect.anchoredPosition = position;

        Image img = button.AddComponent<Image>();
        img.color = Color.gray;

        Button btn = button.AddComponent<Button>();

        // Add text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = name;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 14;

        return button;
    }

    GameObject CreateText(Transform parent, string content, Vector2 position)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 30);
        rect.anchoredPosition = position;

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 16;

        return textObj;
    }

    GameObject CreateSlider(Transform parent, string name, Vector2 position)
    {
        GameObject slider = new GameObject(name);
        slider.transform.SetParent(parent);

        RectTransform rect = slider.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 20);
        rect.anchoredPosition = position;

        Slider sliderComp = slider.AddComponent<Slider>();

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slider.transform);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        Image bgImg = background.AddComponent<Image>();
        bgImg.color = Color.gray;
        sliderComp.targetGraphic = bgImg;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(slider.transform);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.anchoredPosition = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = Color.blue;
        sliderComp.fillRect = fillRect;

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(slider.transform);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = Vector2.zero;
        handleAreaRect.anchoredPosition = Vector2.zero;

        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        sliderComp.handleRect = handleRect;

        return slider;
    }
}