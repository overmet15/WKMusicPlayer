using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WKMusicPlayer.MonoBehaviours
{
    public class MusicPlayerBridge : MonoBehaviour
    {
        public Transform musicListRect { get; private set; }
        public GameObject musicListPrefab { get; private set; }

        public TextMeshProUGUI curPlayingText { get; private set; }

        public Button playButton { get; private set; }
        public Image playButtonImage { get; private set; }

        public Button backButton { get; private set; }
        public Button nextButton { get; private set; }
        public Button loopButton { get; private set; }

        public Image loopButtonImage { get; private set; }
        public Slider slider { get; private set; }
        public Slider volSlider { get; private set; }

        public Knob sliderKnobRect { get; private set; }

        public Sprite pauseSprite { get; private set; }
        public Sprite playSprite { get; private set; }
        public Sprite loopSprite { get; private set; }
        public Sprite loopOffSprite { get; private set; }

        public bool ready { get; private set; }

        public void Init()
        {
            // misc objects
            musicListRect = transform.Find("MusicList/ScrollRect/Content");
            musicListPrefab = transform.Find("MusicList/ItemPrefab").gameObject;
            curPlayingText = transform.Find("UI/Top/CurPlaying").GetComponent<TextMeshProUGUI>();
            volSlider = transform.Find("UI/Top/Vol").GetComponent<Slider>();

            // buttons and slider
            Transform actions = transform.Find("UI/Actions");

            playButton = actions.Find("PausePlay").GetComponent<Button>();
            playButtonImage = playButton.targetGraphic as Image;

            backButton = actions.Find("Back").GetComponent<Button>();
            nextButton = actions.Find("Next").GetComponent<Button>();

            loopButton = actions.Find("Loop").GetComponent<Button>();
            loopButtonImage = loopButton.targetGraphic as Image;

            slider = transform.Find("UI/Slider").GetComponent<Slider>();
            //sliderKnob = slider.handleRect.transform.GetChild(0).gameObject.AddComponent<Knob>();
            sliderKnobRect = slider.gameObject.AddComponent<Knob>();

            pauseSprite = Plugin.bundle.LoadAsset<Sprite>("Pause");
            playSprite = Plugin.bundle.LoadAsset<Sprite>("Play");
            loopSprite = Plugin.bundle.LoadAsset<Sprite>("Loop");
            loopOffSprite = Plugin.bundle.LoadAsset<Sprite>("LoopOff");

            ready = true;
        }
    }
}
