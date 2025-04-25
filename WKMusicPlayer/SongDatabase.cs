using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace WKMusicPlayer
{
    public static class SongDatabase
    {
        public static List<AudioClip> clips;

        public static UnityEvent onDatabaseRebuild = new UnityEvent();

        static bool loading;

        public static async Task Init()
        {
            if (loading) return;

            loading = true;

            List<AudioClip> toDestroy = clips;

            Plugin.Check();

            clips = new List<AudioClip>();

            List<string> files = await Task.Run(() =>
            {
                List<string> tempFiles = new List<string>(Directory.GetFiles(Plugin.musicDirectory));

                if (File.Exists(Plugin.externalMusicFile))
                {
                    string externalFile = File.ReadAllText(Plugin.externalMusicFile);
                    tempFiles.AddRange(externalFile.Split('\n'));
                }

                return tempFiles;
            });

            foreach (string source in files)
            {
                if (source == Plugin.externalMusicFile || string.IsNullOrWhiteSpace(source)) continue;

                Plugin.log.LogInfo($"Loading {source}");

                AudioClip clip = await LoadAudioClipAsync(source);

                if (clip == null) continue;
                
                clip.name = Path.GetFileNameWithoutExtension(source);
                clips.Add(clip);
            }

            onDatabaseRebuild.Invoke();

            if (toDestroy != null)
                for (int i = 0; i < toDestroy.Count; i++) Object.Destroy(toDestroy[i]);

            loading = false;
        }

        private static async Task<AudioClip> LoadAudioClipAsync(string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN))
                    {
                        request.SendWebRequest();

                        while (!request.isDone)
                        {
                            Task.Delay(1).Wait();
                        }

                        if (request.result != UnityWebRequest.Result.Success)
                        {
                            Plugin.log.LogError($"Error loading song: {path}");
                            return null;
                        }

                        return DownloadHandlerAudioClip.GetContent(request);
                    }
                }
                catch
                {
                    Plugin.log.LogError($"Error parsing song: {path}");
                    return null;
                }
            });
        }

    }
}
