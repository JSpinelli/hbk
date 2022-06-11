using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    public EntriesRepository entriesRepository;
    public JournalController journalController;
    
    public List<JournalEntry> testEntries = new List<JournalEntry>();

    public readonly Dictionary<JournalEntry.EntryType, List<JournalEntry>> CurrentEntries =
        new Dictionary<JournalEntry.EntryType, List<JournalEntry>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Should not be another class");
            Destroy(this);
        }
    }

    private void Start()
    {
        EventManager.Instance.Register<ObjectScanned>(OnObjectScanned);
    }

    private void OnObjectScanned(HBKEvent e)
    {
        ObjectScanned of = (ObjectScanned) e;
        AddEntry(of.ScannedEntry);
    }

    private void AddEntry(JournalEntry je)
    {
        if (CurrentEntries.TryGetValue(je.type, out var journalEntries))
        {
            journalEntries.Add(je);
            // Sort Entries
            
        }
        else
        {
            journalEntries = new List<JournalEntry> {je};
            CurrentEntries.Add(je.type, journalEntries);
        }
        
        journalController.AddEntry(je);
    }

    public void TestEntries()
    {
        foreach (var entry in testEntries)
        {
            AddEntry(entry);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(JournalManager))]
public class DrawJournalManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        JournalManager controller = (JournalManager) target;
        if (GUILayout.Button("Test Entries"))
        {
            controller.TestEntries();
        }
    }
}
#endif