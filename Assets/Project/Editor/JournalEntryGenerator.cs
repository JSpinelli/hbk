using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Principal;
using Debug = UnityEngine.Debug;

public class JournalEntryGenerator
{
    private static string _path = "/Project/Editor/journalEntries.tsv";
    
    [MenuItem("Tools/Generate Journal Entries")]
    public static void GenerateJournalEntries()
    {
        string[] allEntries = File.ReadAllLines(Application.dataPath + _path);
        EntriesRepository repository = ScriptableObject.CreateInstance<EntriesRepository>();
        int index = 0;
        foreach (var entry in allEntries)
        {
            if (index == 0)
            {
                index++;
            }
            else
            {
                string[] splitData = entry.Split('\t');
                if (splitData.Length == 4)
                {
                    JournalEntry journalEntry = new JournalEntry(splitData[3]);
                    switch (splitData[0])
                    {
                        case "Kingdom":
                        {
                            journalEntry.type = JournalEntry.EntryType.Kingdom;
                            break;
                        }
                        case "Explorer":
                        {
                            journalEntry.type = JournalEntry.EntryType.Explorer;
                            break;
                        }
                        case "Archivist":
                        {
                            journalEntry.type = JournalEntry.EntryType.Archivist;
                            break;
                        }
                        case "Whale":
                        {
                            journalEntry.type = JournalEntry.EntryType.Whale;
                            break;
                        }
                        case "Poet":
                        {
                            journalEntry.type = JournalEntry.EntryType.Poet;
                            break;
                        }
                        case "Colossus":
                        {
                            journalEntry.type = JournalEntry.EntryType.Colossus;
                            break;
                        }
                    }

                    journalEntry.number = int.Parse(splitData[1]);
                    journalEntry.date = splitData[2];
                    repository.AddEntry(journalEntry);
                }
            }
        }
        AssetDatabase.CreateAsset(repository,
            $"Assets/Project/Runtime/Journal Entries/repository.asset");
        AssetDatabase.SaveAssets();
    }
}
