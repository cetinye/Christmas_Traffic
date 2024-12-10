using UnityEngine;

namespace Christmas_Traffic
{
    public class LaneEntrance : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Santa santa))
            {
                santa.SetLandable(true);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Santa santa))
            {
                santa.SetLandable(false);
            }
        }
    }
}