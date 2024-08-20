using UnityEngine;
using UnityEngine.Video;
using static TVNavigation;

public class VideoPlayerManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    [HideInInspector]
    public string settingsFilePath;
    public TVNavigation.VideoSettings videoSettings;
    private void Start()
    {
        settingsFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "videoSettings.json");
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (System.IO.File.Exists(settingsFilePath))
        {
            string json = System.IO.File.ReadAllText(settingsFilePath);
            VideoSettings settings = JsonUtility.FromJson<VideoSettings>(json);

            // Apply settings to VideoPlayer
            if (videoPlayer != null)
            {
                videoPlayer.url = settings.videoUrl;
                videoPlayer.SetDirectAudioVolume(0, settings.volume);
                videoPlayer.playbackSpeed = settings.speed;

                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += (source) => {
                    videoPlayer.Play();
                };
            }
        }
        else
        {
            Debug.LogWarning("Settings file not found.");
        }
    }
}
