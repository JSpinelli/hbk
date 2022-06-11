using UnityEngine;

public class BlackHoleTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.StartBlackHoleTransition();
    }
}
