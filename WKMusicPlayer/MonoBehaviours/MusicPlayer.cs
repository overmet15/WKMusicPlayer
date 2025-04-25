using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WKMusicPlayer.MonoBehaviours
{
    public class MusicPlayer : MonoBehaviour
    {
        readonly Color itemNormalColor = new Color32(96, 96, 96, 255);
        readonly Color itemCurrentColor = new Color32(106, 1, 1, 255);

        const float itemTransitionSpeed = 1f;

        MusicPlayerBridge bridge;

        public List<Image> clipImages = new List<Image>();

        AudioSource mySource;

        bool open = true; //open by default

        int curIndex = -1;
        bool loop;
        bool movingTime;
        bool paused;

        RectTransform myRect;
        void Start()
        {
            myRect = GetComponent<RectTransform>();
            mySource = gameObject.AddComponent<AudioSource>();
            mySource.bypassEffects = true;

            mySource.ignoreListenerPause = true;
            mySource.ignoreListenerVolume = true;

            mySource.reverbZoneMix = 0;

            bridge = gameObject.AddComponent<MusicPlayerBridge>();
            bridge.Init();

            bridge.playButton.onClick.AddListener(PauseToggle);
            bridge.loopButton.onClick.AddListener(LoopToggle);

            bridge.nextButton.onClick.AddListener(() => { Move(true); });
            bridge.backButton.onClick.AddListener(() => { Move(false); });

            bridge.volSlider.onValueChanged.AddListener((v) => { mySource.volume = v; });

            bridge.volSlider.value = 0.25f;

            bridge.sliderKnobRect.onStateChange.AddListener((b) =>
            {
                if (b) mySource.Stop();
                else
                {
                    mySource.Play();
                    mySource.time = bridge.slider.value;
                }

                movingTime = b;
            });

            SongDatabase.onDatabaseRebuild.AddListener(OnDatabaseRebuild);
            _ = SongDatabase.Init();

            ToggleMenu();
            PauseToggle();
            loop = true;
            LoopToggle();
        }

        void Update()
        {
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F9))
            {
                if (BepInEx.UnityInput.Current.GetKey(KeyCode.LeftShift)) _ = SongDatabase.Init();
                else ToggleMenu();
            }

            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F10)) Move(false);
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F11)) Move(transform);
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F12)) PauseToggle();

            if (SongDatabase.clips == null) return;

            if (mySource.isPlaying)
            {
                bridge.slider.value = mySource.time;
            }
            else if (!paused)
            {
                if (loop || movingTime) return;

                Move(true);
            }
        }

        void ToggleMenu()
        {
            open = !open;

            if (open) myRect.DOAnchorPosX(50, 0.35f).SetEase(Ease.OutCubic).SetUpdate(true);
            else myRect.DOAnchorPosX(-750, 0.2f).SetEase(Ease.InCubic).SetUpdate(true);
        }

        void PauseToggle()
        {
            paused = !paused;

            if (paused)
            {
                bridge.playButtonImage.sprite = bridge.playSprite;
                mySource.Pause();
            }
            else
            {
                bridge.playButtonImage.sprite = bridge.pauseSprite;
                mySource.UnPause();
            }
        }

        void PauseToggle(bool pause)
        {
            paused = pause;

            if (paused)
            {
                bridge.playButtonImage.sprite = bridge.playSprite;
                mySource.Pause();
            }
            else
            {
                bridge.playButtonImage.sprite = bridge.pauseSprite;
                mySource.UnPause();
            }
        }

        void LoopToggle()
        {
            loop = !loop;

            mySource.loop = loop;

            if (loop) bridge.loopButtonImage.sprite = bridge.loopOffSprite;
            else bridge.loopButtonImage.sprite = bridge.loopSprite;
        }

        public void Move(bool next)
        {
            if (SongDatabase.clips == null || SongDatabase.clips.Count == 0) return;

            int index = curIndex + (next ? 1 : -1);

            if (index >= SongDatabase.clips.Count) index = 0;
            else if (index < 0) index = SongDatabase.clips.Count - 1;

            curIndex = index;
            ChangeSong(index);
        }


        void OnDatabaseRebuild()
        {
            for (int childIndex = 0; childIndex < bridge.musicListRect.childCount; childIndex++)
            {
                Destroy(bridge.musicListRect.GetChild(childIndex).gameObject);
            }

            clipImages.Clear();
            PauseToggle(true);
            curIndex = -1;
            bridge.curPlayingText.text = "Select the song.";
            bridge.slider.value = 0;

            for (int i = 0; i < SongDatabase.clips.Count; i++)
            {
                var obj = Instantiate(bridge.musicListPrefab, bridge.musicListRect);
                obj.SetActive(true);
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SongDatabase.clips[i].name;
                Button b = obj.GetComponent<Button>();

                clipImages.Add(b.targetGraphic as Image);

                clipImages[i].DOColor(itemNormalColor, itemTransitionSpeed).SetEase(Ease.OutCubic).SetUpdate(true);

                // storing as local variable because when using i directly, it results it always being latest index
                int toUse = i;

                b.onClick.AddListener(() =>
                {
                    ChangeSong(toUse);
                });
            }
        }

        public void ChangeSong(int index)
        {
            curIndex = index;

            mySource.clip = SongDatabase.clips[index];

            bridge.slider.maxValue = SongDatabase.clips[index].length;

            bridge.curPlayingText.text = $"Currently Playing:\n{SongDatabase.clips[index].name}";

            for (int j = 0; j < SongDatabase.clips.Count; j++)
            {
                if (j == index) clipImages[j].DOColor(itemCurrentColor, itemTransitionSpeed).SetEase(Ease.OutCubic).SetUpdate(true);
                else clipImages[j].DOColor(itemNormalColor, itemTransitionSpeed).SetEase(Ease.OutCubic).SetUpdate(true);
            }
            mySource.Play();
        }
    }
}
