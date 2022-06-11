using System;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    private SphereCollider sc;
    
    // Properties
    public Color fogColor;
    public JournalEntry.EntryType bubbleType;
    
    // State
    private bool _colliderTriggered;

    private void Start()
    {
        sc = GetComponent<SphereCollider>();
        GameManager.Instance.AddBubbleTrigger(sc);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_colliderTriggered) return;
        if (other.CompareTag("Player"))
        {
            _colliderTriggered = true;
            GameManager.Instance.FogChange(fogColor);
            EventManager.Instance.Fire(new BubbleCollision(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_colliderTriggered) return;
        if (other.CompareTag("Player"))
        {
            _colliderTriggered = false;
            GameManager.Instance.FogRevert();
            EventManager.Instance.Fire(new BubbleCollision(false));
        }
    }
}
