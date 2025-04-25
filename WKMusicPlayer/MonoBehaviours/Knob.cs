using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WKMusicPlayer.MonoBehaviours
{
    public class Knob : MonoBehaviour, IPointerDownHandler, IPointerUpHandler // Yes
    {
        public UnityEvent<bool> onStateChange = new UnityEvent<bool>();

        public void OnPointerDown(PointerEventData eventData) => onStateChange.Invoke(true);


        public void OnPointerUp(PointerEventData eventData) => onStateChange.Invoke(false);
    }
}
