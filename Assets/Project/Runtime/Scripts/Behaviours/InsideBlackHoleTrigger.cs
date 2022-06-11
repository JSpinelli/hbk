using UnityEngine;

public class InsideBlackHoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.StopBlackHoleTransition();
    }
}
