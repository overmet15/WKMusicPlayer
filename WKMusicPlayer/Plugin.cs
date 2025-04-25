using BepInEx;
using BepInEx.Logging;
using System.IO;
using UnityEngine;
using WKMusicPlayer.MonoBehaviours;

namespace WKMusicPlayer
{
    [BepInPlugin(guid, pluginName, versionString)]
    public class Plugin : BaseUnityPlugin
    {
        public const string guid = "com.overmet15.WKMusicPlayer";
        public const string pluginName = "WKMusicPlayer";
        public const string versionString = "1.0.0";

        public static readonly string musicDirectory = Path.Combine(Application.dataPath, "..", "Music");
        public static readonly string externalMusicFile = Path.Combine(Application.dataPath, "..", "Music", "external.txt");
        public static AssetBundle bundle;

        public static ManualLogSource log;

        void Awake()
        {
            Logger.LogInfo($"{pluginName} v{versionString} loading...");

            Check();

            string bundlePath =
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "WKMusicPlayer");

            if (!File.Exists(Path.Combine(bundlePath)))
            {
                Logger.LogError("Couldn't find the asset bundle.");
                return;
            }

            bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
            {
                Logger.LogError("Couldn't load the asset bundle.");
                return;
            }

            log = Logger;

            Logger.LogInfo($"{pluginName} v{versionString} loaded.");
        }

        void Start()
        {
            if (bundle != null) InitializeMusicPlayer();
        }

        public static void Check()
        {
            if (!Directory.Exists(musicDirectory)) Directory.CreateDirectory(musicDirectory);

            if (!File.Exists(externalMusicFile)) File.Create(externalMusicFile);
        }

        public static void InitializeMusicPlayer()
        {
            GameObject canvas = Instantiate(bundle.LoadAsset<GameObject>("MusicPlayerCanvas"));

            DontDestroyOnLoad(canvas);

            canvas.transform.GetChild(0).gameObject.AddComponent<MusicPlayer>();
        }
    }
}
