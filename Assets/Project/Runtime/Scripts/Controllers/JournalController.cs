using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class JournalController : MonoBehaviour
{
    // Properties
    public float scrollSensitivity = 0.1f;
    public Color selectedIcon;
    public Color deselectedIcon;
    public List<JournalEntry.EntryType> entriesOrder;
    public List<JournalEntry.EntryType> unlockedEntries;

    // References
    public List<GameObject> objectEntries;
    public GameObject display;
    public GameObject entriesContent;
    public GameObject entryPrefab;
    public ScrollRect icons;
    public ScrollRect entries;
    public GameObject scrollBarHandle;
    public GameObject scrollBarTop;
    public GameObject scrollBarBottom;

    // State
    private int _currentSelectedIcon = -1;
    private bool _onCategories = true;
    private readonly Dictionary<JournalEntry.EntryType, List<GameObject>> _currentObjectEntries =
        new Dictionary<JournalEntry.EntryType, List<GameObject>>();

    void Start()
    {
        InputManager.Instance.Register(OnVertical, "Vertical");
        InputManager.Instance.Register(OnHorizontal, "Horizontal");
        
        EventManager.Instance.Register<CameraSwitch>(OnCameraSwitch);

        display.SetActive(false);

        foreach (var obj in objectEntries)
        {
            obj.SetActive(false);
        }
    }

    private void OnCameraSwitch(HBKEvent e)
    {
        CameraSwitch cw = (CameraSwitch) e;

        if (cw.NewCam == "Core")
        {
            display.SetActive(true);
            _onCategories = true;
            RefreshEntries( 0);
        }
        else
        {
            display.SetActive(false);
        }
    }

    public void AddEntry(JournalEntry je)
    {
        GameObject newEntry = Instantiate(entryPrefab, entriesContent.transform, false);
        newEntry.name = je.type.ToString() + je.number;
        newEntry.GetComponent<TextMeshProUGUI>().text = je.content;
        newEntry.GetComponent<ShapesScaler>().Scale();

        if (_currentObjectEntries.TryGetValue(je.type, out var currentList))
        {
            currentList.Add(newEntry);
        }
        else
        {
            currentList = new List<GameObject> {newEntry};
            _currentObjectEntries.Add(je.type, currentList);
            objectEntries[entriesOrder.FindIndex((e) => e == je.type)].SetActive(true);
            unlockedEntries = new List<JournalEntry.EntryType>();
            for (int i = 0; i < entriesOrder.Count; i++)
            {
                if (objectEntries[i].activeSelf)
                {
                    unlockedEntries.Add(entriesOrder[i]);
                }
            }
            _currentSelectedIcon = 0;
        }
    }

    private void RefreshEntries(int newIconIndex)
    {
        if (_currentSelectedIcon == -1) return; //No entries active
        List<GameObject> currentList;
        foreach (var entry in JournalManager.Instance.CurrentEntries)
        {
            objectEntries[entriesOrder.FindIndex((e) => e == entry.Key)].GetComponent<ShapeRenderer>().Color =
                deselectedIcon;
            if (!_currentObjectEntries.TryGetValue(entry.Key, out currentList)) continue;
            foreach (var obj in currentList)
            {
                obj.SetActive(false);
            }
        }
        if (_currentObjectEntries.TryGetValue(unlockedEntries[newIconIndex], out currentList))
        {
            objectEntries[entriesOrder.FindIndex((e) => e == unlockedEntries[newIconIndex])]
                .GetComponent<ShapeRenderer>().Color = selectedIcon;
            foreach (var entry in currentList)
            {
                entry.SetActive(true);
                entry.GetComponent<ShapesScaler>().Scale();
            }
        }

        _currentSelectedIcon = newIconIndex;
    }

    private void OnVertical(InputAction.CallbackContext value)
    {
        float val = value.ReadValue<float>();
        if (val == 0) return;
        if (val > 0) // UP
        {
            if (_onCategories)
            {
                icons.normalizedPosition = new Vector2(0, icons.normalizedPosition.y + scrollSensitivity);
                if (_currentSelectedIcon == 0) return;
                entries.verticalNormalizedPosition = 1;
                RefreshEntries(_currentSelectedIcon - 1);
            }
            else
            {
                entries.verticalNormalizedPosition += scrollSensitivity;
                entries.verticalNormalizedPosition = Mathf.Clamp(entries.verticalNormalizedPosition, 0, 1);
            }
        }
        else //Down
        {
            if (_onCategories)
            {
                icons.normalizedPosition = new Vector2(0, icons.normalizedPosition.y - scrollSensitivity);
                if (_currentSelectedIcon == JournalManager.Instance.CurrentEntries.Count - 1) return;
                entries.verticalNormalizedPosition = 1;
                RefreshEntries(_currentSelectedIcon + 1);
            }
            else
            {
                entries.verticalNormalizedPosition -= scrollSensitivity;
                entries.verticalNormalizedPosition = Mathf.Clamp(entries.verticalNormalizedPosition, 0, 1);
            }
        }
    }

    private void OnHorizontal(InputAction.CallbackContext value)
    {
        float val = value.ReadValue<float>();
        if (val == 0) return;
        if (val > 0) //Right
        {
            // Swap to Entries Column
            _onCategories = false;
            scrollBarBottom.GetComponent<Image>().color = selectedIcon;
            scrollBarHandle.GetComponent<Image>().color = selectedIcon;
            scrollBarTop.GetComponent<Image>().color = selectedIcon;
        }
        else //Left
        {
            // Swap to Icons Column
            _onCategories = true;
            scrollBarBottom.GetComponent<Image>().color = deselectedIcon;
            scrollBarHandle.GetComponent<Image>().color = deselectedIcon;
            scrollBarTop.GetComponent<Image>().color = deselectedIcon;
        }
    }
}