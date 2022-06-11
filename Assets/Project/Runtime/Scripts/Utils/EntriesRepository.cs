using System;
using System.Collections.Generic;
using UnityEngine;

public class EntriesRepository : ScriptableObject
{
    public List<JournalEntry> colossusEntries;
    public List<JournalEntry> explorerEntries;
    public List<JournalEntry> whaleEntries;
    public List<JournalEntry> kingdomEntries;
    public List<JournalEntry> archivistEntries;
    public List<JournalEntry> poetEntries;

    public EntriesRepository()
    {
        colossusEntries = new List<JournalEntry>();
        explorerEntries = new List<JournalEntry>();
        whaleEntries = new List<JournalEntry>();
        kingdomEntries = new List<JournalEntry>();
        archivistEntries = new List<JournalEntry>();
        poetEntries = new List<JournalEntry>();
    }

    public void AddEntry(JournalEntry je)
    {
        switch (je.type)
        {
            case JournalEntry.EntryType.Archivist:
            {
                archivistEntries.Add(je);
                archivistEntries.Sort((a, b) => a.number - b.number);
                break;
            }
            case JournalEntry.EntryType.Colossus:
            {
                colossusEntries.Add(je);
                colossusEntries.Sort((a, b) => a.number - b.number);
                break;
            }
            case JournalEntry.EntryType.Explorer:
            {
                explorerEntries.Add(je);
                explorerEntries.Sort((a, b) => a.number - b.number);
                break;
            }
            case JournalEntry.EntryType.Kingdom:
            {
                kingdomEntries.Add(je);
                kingdomEntries.Sort((a, b) => a.number - b.number);
                break;
            }
            case JournalEntry.EntryType.Poet:
            {
                poetEntries.Add(je);
                poetEntries.Sort((a, b) => a.number - b.number);
                break;
            }
            case JournalEntry.EntryType.Whale:
            {
                whaleEntries.Add(je);
                whaleEntries.Sort((a, b) => a.number - b.number);
                break;
            }
        }
    }

    public JournalEntry GetEntry(JournalEntry.EntryType type, int number)
    {
        switch (type)
        {
            case JournalEntry.EntryType.Archivist:
            {
                return archivistEntries[number];
            }
            case JournalEntry.EntryType.Colossus:
            {
                return colossusEntries[number];
            }
            case JournalEntry.EntryType.Explorer:
            {
                return explorerEntries[number];
            }
            case JournalEntry.EntryType.Kingdom:
            {
                return kingdomEntries[number];
            }
            case JournalEntry.EntryType.Poet:
            {
                return poetEntries[number];
            }
            case JournalEntry.EntryType.Whale:
            {
                return whaleEntries[number];
            }
            default:
            {
                return null;
            }
        }
    }
}

[Serializable]
public class JournalEntry
{
    public EntryType type;
    public int number;
    public string date;
    public string content;

    public enum EntryType
    {
        Kingdom,
        Explorer,
        Archivist,
        Whale,
        Poet,
        Colossus
    }

    public JournalEntry(string content)
    {
        this.content = content;
    }
}