using Lean.Touch;
using UnityEngine;

namespace Christmas_Traffic
{
    public class TapListener : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        void Update()
        {
            if (levelManager.State != LevelManager.GameState.Playing) return;

            if (LeanTouch.Fingers.Count > 0 && LeanTouch.Fingers[0].Down)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(levelManager.MainCamera.ScreenToWorldPoint(LeanTouch.Fingers[0].ScreenPosition), Vector2.zero);
                if (hitInfo && hitInfo.collider.TryGetComponent(out Santa santa) && santa.SantaState != Santa.SantaStates.Landing && santa.SantaState != Santa.SantaStates.Dead)
                {
                    AudioManager.Instance.PlayOneShot(AudioManager.SoundType.Tap);
                    santa.ClearPoints();
                    santa.pathDrawable = true;
                    santa.SantaState = Santa.SantaStates.FollowingPath;
                    Taptic.Success();
                }
            }
        }
    }
}