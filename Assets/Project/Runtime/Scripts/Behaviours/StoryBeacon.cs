using UnityEngine;

public class StoryBeacon : MonoBehaviour
{
    public JournalEntry.EntryType type;
    public int number;

    public JournalEntry GetEntry()
    {
        return JournalManager.Instance.entriesRepository.GetEntry(type,number);
    }
}
