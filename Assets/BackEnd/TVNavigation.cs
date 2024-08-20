using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEditor; // Include this if you need to handle RawImage
/*
public class TVNavigation : MonoBehaviour
{
    private List<GameObject> tvObjects = new List<GameObject>();
    private int currentIndex = 0;
    private GameObject tvGameObject;
    private VideoPlayer nextVideoPlayer = null;
    private int nextIndex = 0;
    private int finalIndex = 0;
    private Material transparentMaterial;
    private Material tvScreenMaterial;
    private RawImage rawImage; // RawImage component to display video/textures
    private int lastIndex;
    private void Start()
    {
        // Load materials
        transparentMaterial = Resources.Load<Material>("Transparent");
        tvScreenMaterial = Resources.Load<Material>("TVScreen");

        if (transparentMaterial == null)
        {
            Debug.LogError("Transparent material is Null");
        }

        if (tvScreenMaterial == null)
        {
            Debug.LogError("TVScreen material is Null");
        }

        // Find the TV game object and its RawImage
        tvGameObject = FindGameObject("AddModelHere/Media/TV");
        if (tvGameObject != null)
        {
            rawImage = tvGameObject.GetComponentInChildren<RawImage>();

            if (rawImage == null)
            {
                Debug.LogError("RawImage component not found in TV GameObject.");
                return;
            }

            // Set the existing RenderTexture
            RenderTexture videoRenderTexture = Resources.Load<RenderTexture>("VideoPlayer"); // Load the existing RenderTexture
            if (videoRenderTexture == null)
            {
                Debug.LogError("VideoPlayer RenderTexture not found.");
            }
            else
            {
                rawImage.texture = videoRenderTexture; // Set RenderTexture to RawImage
            }

            // Populate the list of TV objects
            foreach (Transform child in tvGameObject.transform)
            {
                tvObjects.Add(child.gameObject);
            }

            // Ensure only the first object is active at the start
            for (int i = 0; i < tvObjects.Count; i++)
            {
                tvObjects[i].SetActive(i == 0);
            }

            // Set the initial material if the first GameObject is a Renderer
            SetInitialMaterial();

            // Preload the next video if available
            PreloadNextVideo();
            Debug.Log(tvObjects.Count);
        }
        else
        {
            Debug.LogError("TV GameObject not found.");
        }
    }

    private void SetInitialMaterial()
    {
        if (tvObjects.Count > 0)
        {
            GameObject firstObject = tvObjects[0];
            Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();

            // Check if the first GameObject is a Renderer
            if (firstObject.GetComponent<Renderer>() != null)
            {
                Material firstMaterial = firstObject.GetComponent<Renderer>().material;
                if (tvRenderer != null && firstMaterial != null)
                {
                    tvRenderer.material = firstMaterial;
                    tvRenderer.enabled = true;
                }
                else
                {
                    Debug.LogWarning("TV Renderer or First Material is null. Cannot set initial material.");
                }
            }
        }
    }

    public void ToggleNextGameObject()
    {
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }

        // Hide the current object
        tvObjects[currentIndex].SetActive(false);

        // Increment index and loop around if necessary
        currentIndex = (currentIndex + 1) % tvObjects.Count;
        nextIndex = currentIndex;
        
        // Show the next object
        GameObject nextObject = tvObjects[currentIndex];
        VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // Use the preloaded video player if available
            if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
            {
                nextObject.SetActive(true);
                StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                Debug.Log("Playing preloaded video.");
            }
            else
            {
                // Prepare the video player
                StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
            }
            CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
            // Set the RawImage texture to the custom RenderTexture
            if (rawImage != null)
            {
                if (videoRenderTexture != null)
                {
                    rawImage.texture = videoRenderTexture;
                }
                else
                {
                    Debug.LogError("Video RenderTexture not assigned.");
                }
            }
        }
        else
        {
            // Handle non-video objects
            HandleNonVideoObject(nextObject);

            // Set the RawImage texture to the material's main texture
            Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
            if (rawImage != null && nextMaterial != null)
            {
                rawImage.texture = nextMaterial.mainTexture; // Use the texture of the material
            }
        }

        if (nextIndex == 0)
        {
            Debug.Log("Completed a full loop through TV objects.");
            GameObject info = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Title");
            GameObject Des = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Description");
            if (info != null)
            {
                Debug.Log("Panel GameObject found, activating it.");
                info.SetActive(true);
            }
            if (Des != null)
            {
                Debug.Log("Des found");
                Des.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Panel GameObject not found.");
            }
        }
        Debug.Log("Index: " + nextIndex);
    }

    public void ToggleBackGameObject()
    {
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }

        // Hide the current object
        tvObjects[currentIndex].SetActive(false);

        // Decrement index and loop around if necessary
        currentIndex = (currentIndex - 1 + tvObjects.Count) % tvObjects.Count;
        nextIndex = currentIndex;

        // Show the previous object
        GameObject nextObject = tvObjects[currentIndex];
        VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // Use the preloaded video player if available
            if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
            {
                nextObject.SetActive(true);
                StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                Debug.Log("Playing preloaded video.");
            }
            else
            {
                // Prepare the video player
                StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
            }
            CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
            // Set the RawImage texture to the custom RenderTexture
            if (rawImage != null)
            {
                if (videoRenderTexture != null)
                {
                    rawImage.texture = videoRenderTexture;
                }
                else
                {
                    Debug.LogError("Video RenderTexture not assigned.");
                }
            }
        }
        else
        {
            // Handle non-video objects
            HandleNonVideoObject(nextObject);

            // Set the RawImage texture to the material's main texture
            Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
            if (rawImage != null && nextMaterial != null)
            {
                rawImage.texture = nextMaterial.mainTexture; // Use the texture of the material
            }
        }

         if (nextIndex == 0)
        {
            Debug.Log("Completed a full loop through TV objects.");
            GameObject info = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Title");
            GameObject Des = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Description");
            if (info != null)
            {
                Debug.Log("Panel GameObject found, activating it.");
                info.SetActive(true);
            }
            if (Des != null)
            {
                Debug.Log("Des found");
                Des.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Panel GameObject not found.");
            }
        }

        // Preload the next video
        PreloadNextVideo();
    }

    private CustomRenderTexture FindCustomRenderTexture(string textureName)
    {
        // Load the CustomRenderTexture from the Resources folder
        CustomRenderTexture customRenderTexture = Resources.Load<CustomRenderTexture>(textureName);
        if (customRenderTexture == null)
        {
            Debug.LogWarning($"CustomRenderTexture '{textureName}' not found in Resources.");
        }
        return customRenderTexture;
    }

    private IEnumerator PlayWhenReady(VideoPlayer videoPlayer, GameObject nextObject)
    {
        Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();
        CustomRenderTexture customRenderTexture = FindCustomRenderTexture("VideoPlayer");

        Debug.Log("VideoPlayer status: " + (videoPlayer.isPrepared ? "Prepared" : "Not Prepared"));
        Debug.Log("VideoPlayer frame: " + videoPlayer.frame);

        // Wait until the video player is prepared and the first frame is ready
        while (!videoPlayer.isPrepared || videoPlayer.frame < 1)
        {
            yield return null;
        }

        // Set the transparent material
        if (tvRenderer != null)
        {
            tvRenderer.material = transparentMaterial;
            tvRenderer.enabled = true;
        }
        else
        {
            Debug.LogWarning("TV Renderer is null.");
        }

        // Set the TVScreen material
        if (tvRenderer != null)
        {
            tvRenderer.material = tvScreenMaterial;
            tvRenderer.enabled = true;
        }
        else
        {
            Debug.LogWarning("TV Renderer is null.");
        }

        // Set the video player target texture
        if (customRenderTexture != null)
        {
            videoPlayer.targetTexture = customRenderTexture;
        }
        else
        {
            Debug.LogError("CustomRenderTexture not found.");
        }

        nextObject.SetActive(true);
        videoPlayer.Play();
        Debug.Log("Video playing.");
    }

    private IEnumerator PrepareAndPlayVideo(GameObject nextObject, VideoPlayer videoPlayer)
    {
        Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();
        CustomRenderTexture customRenderTexture = FindCustomRenderTexture("VideoPlayer");

        // Set the transparent material first
        if (tvRenderer != null)
        {
            tvRenderer.material = transparentMaterial;
            tvRenderer.enabled = true;
        }

        // Prepare the video player
        Debug.Log("Preparing video...");
        if (customRenderTexture != null)
        {
            videoPlayer.targetTexture = customRenderTexture;
        }
        else
        {
            Debug.LogError("CustomRenderTexture not found.");
        }
        videoPlayer.targetMaterialRenderer = tvGameObject.GetComponent<Renderer>();
        bool isPrepared = false;

        // Event to signal when the video is prepared
        videoPlayer.prepareCompleted += (vp) => isPrepared = true;
        videoPlayer.Prepare();

        // Wait until the video is prepared
        while (!isPrepared)
        {
            tvRenderer.enabled = false;
            nextObject.SetActive(false);
            yield return null;
        }

        // Wait until the first frame is ready
        while (videoPlayer.frame < 1)
        {
            yield return null;
        }

        // Set the TVScreen material
        if (tvRenderer != null)
        {
            tvRenderer.material = tvScreenMaterial;
            tvRenderer.enabled = true;
        }

        // Show the next object and play the video
        tvRenderer.enabled = true;
        nextObject.SetActive(true);
        videoPlayer.Play();
        Debug.Log("Video prepared and first frame ready. Video playing.");
    }


    private void HandleNonVideoObject(GameObject nextObject)
    {
        // If there's an active video player, stop it
        VideoPlayer tvVideoPlayer = tvGameObject.GetComponent<VideoPlayer>();
        Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();

        if (tvVideoPlayer != null && tvVideoPlayer.isPlaying)
        {
            tvVideoPlayer.Stop();
            tvVideoPlayer.enabled = false; // Disable the video player when not needed
        }

        if (nextObject.GetComponent<VideoPlayer>() != null)
        {
            // Set the RawImage texture to the custom RenderTexture
            if (rawImage != null)
            {
                RenderTexture videoRenderTexture = Resources.Load<RenderTexture>("VideoPlayer"); // Ensure the RenderTexture is loaded correctly
                if (videoRenderTexture != null)
                {
                    rawImage.texture = videoRenderTexture;
                }
                else
                {
                    Debug.LogError("VideoPlayer RenderTexture not found.");
                }
            }

            // Set the TVScreen material if the nextObject has a VideoPlayer component
            if (tvRenderer != null && tvScreenMaterial != null)
            {
                tvRenderer.material = tvScreenMaterial;
                tvRenderer.enabled = true;
            }
        }
        else
        {
            // For non-video objects, set the material of the TV based on nextObject's material
            Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
            if (tvRenderer != null && nextMaterial != null)
            {
                tvRenderer.material = nextMaterial;
                tvRenderer.enabled = true;
            }

            // Set the RawImage texture to the material's main texture
            if (rawImage != null)
            {
                rawImage.texture = nextMaterial?.mainTexture; // Use the texture of the material
            }
        }

        // Show the next object
        nextObject.SetActive(true);
    }

    private void PreloadNextVideo()
    {
        nextIndex = (currentIndex + 1) % tvObjects.Count;
        GameObject nextObject = tvObjects[nextIndex];
        VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // Temporarily enable the VideoPlayer GameObject
            bool wasActive = nextObject.activeSelf;
            nextObject.SetActive(true);

            bool wasEnabled = videoPlayer.enabled;
            videoPlayer.enabled = true;

            Debug.Log("Preloading next video...");
            videoPlayer.targetTexture = rawImage.texture as RenderTexture; // Set the target texture to the custom RenderTexture
            videoPlayer.prepareCompleted += OnPreloadCompleted;
            videoPlayer.Prepare();

            // Restore the original enabled state and active state
            videoPlayer.enabled = wasEnabled;
            nextObject.SetActive(wasActive);

            nextVideoPlayer = videoPlayer;
        }
        else
        {
            nextVideoPlayer = null;
        }
    }



    private void OnPreloadCompleted(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPreloadCompleted; // Unsubscribe from the event to prevent multiple calls
        Debug.Log("Next video preloaded.");
    }

    private GameObject FindGameObject(string path)
    {
        // Split the path by '/' and find each part
        string[] parts = path.Split('/');
        GameObject currentObject = null;

        foreach (string part in parts)
        {
            if (currentObject == null)
            {
                currentObject = GameObject.Find(part);
            }
            else
            {
                Transform childTransform = currentObject.transform.Find(part);
                if (childTransform != null)
                {
                    currentObject = childTransform.gameObject;
                }
                else
                {
                    // Check if the object is inactive
                    GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                    currentObject = null;
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.name == part && obj.transform.IsChildOf(currentObject.transform))
                        {
                            currentObject = obj;
                            break;
                        }
                    }
                    if (currentObject == null) return null;
                }
            }
        }

        return currentObject;
    }
}
*/

public class TVNavigation : MonoBehaviour
{
    private List<GameObject> tvObjects = new List<GameObject>();
    private int currentIndex = 0;
    private GameObject tvGameObject;
    private VideoPlayer nextVideoPlayer = null;
    private int nextIndex = 0;
    private Material transparentMaterial;
    private Material tvScreenMaterial;
    private RawImage rawImage;
    private int lastIndex;
    private bool isLoopCompleted = false; // Add this boolean
    private bool is2LoopCompleted = false;
    private int loopCount = 0;
    private void Start()
    {
        // Load materials
        transparentMaterial = Resources.Load<Material>("Transparent");
        tvScreenMaterial = Resources.Load<Material>("TVScreen");

        if (transparentMaterial == null)
        {
            Debug.LogError("Transparent material is Null");
        }

        if (tvScreenMaterial == null)
        {
            Debug.LogError("TVScreen material is Null");
        }

        // Find the TV game object and its RawImage
        tvGameObject = FindGameObject("AddModelHere/Media/TV");
        if (tvGameObject != null)
        {
            rawImage = tvGameObject.GetComponentInChildren<RawImage>();

            if (rawImage == null)
            {
                Debug.LogError("RawImage component not found in TV GameObject.");
                return;
            }

            // Set the existing RenderTexture
            RenderTexture videoRenderTexture = Resources.Load<RenderTexture>("VideoPlayer");
            if (videoRenderTexture == null)
            {
                Debug.LogError("VideoPlayer RenderTexture not found.");
            }
            else
            {
                rawImage.texture = videoRenderTexture;
            }

            // Populate the list of TV objects
            foreach (Transform child in tvGameObject.transform)
            {
                tvObjects.Add(child.gameObject);
            }

            // Ensure only the first object is active at the start
            for (int i = 0; i < tvObjects.Count; i++)
            {
                tvObjects[i].SetActive(i == 0);
            }

            // Set the initial material if the first GameObject is a Renderer
            SetInitialMaterial();
            Debug.Log(tvObjects.Count);
            // Preload the next video if available
            PreloadNextVideo();
            SetInfoAndDescriptionActive(true);
        }
        else
        {
            Debug.LogError("TV GameObject not found.");
        }
    }

    private void SetInitialMaterial()
    {
        if (tvObjects.Count > 0 && currentIndex != -1)
        {
            GameObject firstObject = tvObjects[currentIndex];
            Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();

            if (firstObject.GetComponent<Renderer>() != null)
            {
                Material firstMaterial = firstObject.GetComponent<Renderer>().material;
                if (tvRenderer != null && firstMaterial != null)
                {
                    tvRenderer.material = firstMaterial;
                    tvRenderer.enabled = true;
                }
                else
                {
                    Debug.LogWarning("TV Renderer or First Material is null. Cannot set initial material.");
                }
            }
            SetMediaObjects(false);
        }
    }

    public void ToggleNextGameObject()
    {
        
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }
        // Check if the loop is complete
        if (currentIndex == 0 && lastIndex == tvObjects.Count - 1)
        {
            loopCount++;
            Debug.Log("LoopCount: "+loopCount);
            if (loopCount >= 1)
            {
                Debug.Log("Two loops completed.");
                currentIndex = -1; // Set index to -1 after two loops
                lastIndex = 0;
                loopCount = 0;
            }
        }
        else
        {
            is2LoopCompleted = false;
        }

        if (currentIndex != -1)
        {
            SetMediaObjects(true);
            // Disable the info and description UI elements when navigating
            SetInfoAndDescriptionActive(false);

           
           
            // Hide the current object
            tvObjects[currentIndex].SetActive(false);
            // Increment index and loop around if necessary
            currentIndex = (currentIndex + 1) % tvObjects.Count;
            
            
            Debug.Log($"Next Index: {currentIndex}");

            lastIndex = (currentIndex + tvObjects.Count - 1) % tvObjects.Count;

            // Show the next object
            GameObject nextObject = (currentIndex != -1) ? tvObjects[currentIndex] : null;
            if (nextObject != null)
            {
                VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

                if (videoPlayer != null)
                {
                    if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
                    {
                        nextObject.SetActive(true);
                        StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                        Debug.Log("Playing preloaded video.");
                    }
                    else
                    {
                        StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
                    }

                    CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
                    if (rawImage != null)
                    {
                        if (videoRenderTexture != null)
                        {
                            rawImage.texture = videoRenderTexture;
                        }
                        else
                        {
                            Debug.LogError("Video RenderTexture not assigned.");
                        }
                    }
                }
                else
                {
                    HandleNonVideoObject(nextObject);
                    
                    Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
                    if (rawImage != null && nextMaterial != null)
                    {
                        rawImage.texture = nextMaterial.mainTexture;
                    }
                }
            }
            
        }
        else
        {
            SetMediaObjects(false);
            // Loop completed, show info and description
            SetInfoAndDescriptionActive(true);
            Debug.Log("Yes");
            currentIndex = 0;
            loopCount = 0;
        }
        Debug.Log("Index after next: " + currentIndex);
    }

    public void ToggleBackGameObject()
    {
        
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }
        // Check if the loop is complete
        if (currentIndex == 0 && lastIndex == tvObjects.Count - 1)
        {
            loopCount++;
            Debug.Log("LoopCount: " + loopCount);
            if (loopCount >= 1)
            {
                Debug.Log("Two loops completed.");
                currentIndex = -1; // Set index to -1 after two loops
                lastIndex = 0;
                loopCount = 0;
            }
        }
        else
        {
            is2LoopCompleted = false;
        }

        if (currentIndex != -1)
        {
            SetMediaObjects(true);
            // Disable the info and description UI elements when navigating
            SetInfoAndDescriptionActive(false);

            // Hide the current object
            tvObjects[currentIndex].SetActive(false);

            // Decrement index and loop around if necessary
            currentIndex = (currentIndex - 1 + tvObjects.Count) % tvObjects.Count;
            Debug.Log($"Current Index: {currentIndex}");      
           
            lastIndex = (currentIndex + tvObjects.Count) % tvObjects.Count;

            // Show the previous object
            GameObject nextObject = (currentIndex != -1) ? tvObjects[currentIndex] : null;
            if (nextObject != null)
            {
                VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

                if (videoPlayer != null)
                {
                    if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
                    {
                        nextObject.SetActive(true);
                        StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                        Debug.Log("Playing preloaded video.");
                    }
                    else
                    {
                        StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
                    }

                    CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
                    if (rawImage != null)
                    {
                        if (videoRenderTexture != null)
                        {
                            rawImage.texture = videoRenderTexture;
                        }
                        else
                        {
                            Debug.LogError("Video RenderTexture not assigned.");
                        }
                    }
                }
                else
                {
                    HandleNonVideoObject(nextObject);

                    Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
                    if (rawImage != null && nextMaterial != null)
                    {
                        rawImage.texture = nextMaterial.mainTexture;
                    }
                }
            }
            
        }
        else
        {
            SetMediaObjects(false);
            // Loop completed, show info and description
            SetInfoAndDescriptionActive(true);
            Debug.Log("Yes");
            currentIndex = 0;
            loopCount = 0;
        }

        PreloadNextVideo();
        Debug.Log("Index after back: " + currentIndex);
    }

    private void SetInfoAndDescriptionActive(bool isActive)
    {
        GameObject info = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Title");
        GameObject Des = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Description");

        if (info != null)
        {
            info.SetActive(isActive);
        }

        if (Des != null)
        {
            Des.SetActive(isActive);
        }
        Debug.Log($"info status: {isActive}");
    }

    private void SetMediaObjects(bool isActive)
    {
        GameObject media = GameObject.Find("AddModelHere/Media/TV");
        if (media != null)
        {
            RawImage rawImage = media.GetComponentInChildren<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = isActive;
                Debug.Log($"RawImage component status: {rawImage.enabled}");
            }
            else
            {
                Debug.LogWarning("RawImage component not found in Media GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Media GameObject not found.");
        }
        Debug.Log($"Requested RawImage status: {isActive}");
    }


    private CustomRenderTexture FindCustomRenderTexture(string textureName)
    {
        CustomRenderTexture customRenderTexture = Resources.Load<CustomRenderTexture>(textureName);
        if (customRenderTexture == null)
        {
            Debug.LogWarning($"CustomRenderTexture '{textureName}' not found.");
        }
        return customRenderTexture;
    }

    private GameObject FindGameObject(string path)
    {
        GameObject gameObject = GameObject.Find(path);
        if (gameObject == null)
        {
            Debug.LogError($"GameObject at path '{path}' not found.");
        }
        return gameObject;
    }

    private IEnumerator PrepareAndPlayVideo(GameObject videoObject, VideoPlayer videoPlayer)
    {
        videoObject.SetActive(true);

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();
    }

    private IEnumerator PlayWhenReady(VideoPlayer videoPlayer, GameObject videoObject)
    {
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoObject.SetActive(true);
        videoPlayer.Play();
    }

    private void HandleNonVideoObject(GameObject nextObject)
    {
        // Retrieve components from the TV game object
        VideoPlayer tvVideoPlayer = tvGameObject.GetComponent<VideoPlayer>();
        Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();

        // Stop and disable the video player if it's active
        if (tvVideoPlayer != null && tvVideoPlayer.isPlaying)
        {
            tvVideoPlayer.Stop();
            tvVideoPlayer.enabled = false; // Disable the video player when not needed
        }

        if (nextObject.GetComponent<VideoPlayer>() != null)
        {
            // Handle video object
            if (rawImage != null)
            {
                RenderTexture videoRenderTexture = Resources.Load<RenderTexture>("VideoPlayer"); // Ensure the RenderTexture is loaded correctly
                if (videoRenderTexture != null)
                {
                    rawImage.texture = videoRenderTexture;
                }
                else
                {
                    Debug.LogError("VideoPlayer RenderTexture not found.");
                }
            }

            // Set the TVRenderer material if nextObject has a VideoPlayer component
            if (tvRenderer != null && tvScreenMaterial != null)
            {
                tvRenderer.material = tvScreenMaterial;
                tvRenderer.enabled = true;
            }
        }
        else
        {
            // Handle non-video object
            Renderer nextRenderer = nextObject.GetComponent<Renderer>();
            if (nextRenderer != null && nextRenderer.material != null)
            {
                Material nextMaterial = nextRenderer.material;

                // Set the TVRenderer material based on the nextObject's material
                if (tvRenderer != null)
                {
                    tvRenderer.material = nextMaterial;
                    tvRenderer.enabled = true;
                }

                // Set the RawImage texture to the material's main texture
                if (rawImage != null)
                {
                    rawImage.texture = nextMaterial.mainTexture; // Use the texture of the material
                }
            }
            else
            {
                Debug.Log("Next object does not have a Renderer or its material is null.");
                return; // Skip this object if there's no valid material
            }
        }

        // Show the next object
        nextObject.SetActive(true);
    }

    private void PreloadNextVideo()
    {
        if (tvObjects.Count > 0)
        {
            nextIndex = (currentIndex + 1) % tvObjects.Count;
            GameObject nextObject = tvObjects[nextIndex];
            VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();
            if (videoPlayer != null)
            {
                nextVideoPlayer = videoPlayer;
            }
        }
    }
    [System.Serializable]
    public class VideoSettings
    {
        public float volume;
        public float speed;
        public string videoUrl; // Add this line for video URL
    }
}


/*
public class TVNavigation : MonoBehaviour
{
    private List<GameObject> tvObjects = new List<GameObject>();
    private int currentIndex = 0;
    private GameObject tvGameObject;
    private VideoPlayer nextVideoPlayer = null;
    private int nextIndex = 0;
    private Material transparentMaterial;
    private Material tvScreenMaterial;
    private RawImage rawImage;
    private int lastIndex;
    private bool isLoopCompleted = false;
    private bool is2LoopCompleted = false;
    private int loopCount = 0;
    private VideoSettings videoSettings;

    private void Start()
    {
        // Load materials
        transparentMaterial = Resources.Load<Material>("Transparent");
        tvScreenMaterial = Resources.Load<Material>("TVScreen");

        if (transparentMaterial == null)
        {
            Debug.LogError("Transparent material is Null");
        }

        if (tvScreenMaterial == null)
        {
            Debug.LogError("TVScreen material is Null");
        }

        // Load video settings
        LoadVideoSettings();

        // Find the TV game object and its RawImage
        tvGameObject = FindGameObject("AddModelHere/Media/TV");
        if (tvGameObject != null)
        {
            rawImage = tvGameObject.GetComponentInChildren<RawImage>();

            if (rawImage == null)
            {
                Debug.LogError("RawImage component not found in TV GameObject.");
                return;
            }

            // Populate the list of TV objects
            foreach (Transform child in tvGameObject.transform)
            {
                tvObjects.Add(child.gameObject);
            }

            // Ensure only the first object is active at the start
            for (int i = 0; i < tvObjects.Count; i++)
            {
                tvObjects[i].SetActive(i == 0);
            }

            // Set the initial material if the first GameObject is a Renderer
            SetInitialMaterial();
            Debug.Log(tvObjects.Count);

            // Preload the next video if available
            PreloadNextVideo();
            SetInfoAndDescriptionActive(true);
        }
        else
        {
            Debug.LogError("TV GameObject not found.");
        }
    }

    private void LoadVideoSettings()
    {
        // Load video settings from StreamingAssets folder or another source
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "videoSettings.json");
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            videoSettings = JsonUtility.FromJson<VideoSettings>(json);
        }
        else
        {
            Debug.LogWarning("Video settings file not found.");
            videoSettings = new VideoSettings();
        }
    }

    private void SetInitialMaterial()
    {
        if (tvObjects.Count > 0 && currentIndex != -1)
        {
            GameObject firstObject = tvObjects[currentIndex];
            Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();

            if (firstObject.GetComponent<Renderer>() != null)
            {
                Material firstMaterial = firstObject.GetComponent<Renderer>().material;
                if (tvRenderer != null && firstMaterial != null)
                {
                    tvRenderer.material = firstMaterial;
                    tvRenderer.enabled = true;
                }
                else
                {
                    Debug.LogWarning("TV Renderer or First Material is null. Cannot set initial material.");
                }
            }
            SetMediaObjects(false);
        }
    }

    public void ToggleNextGameObject()
    {
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }

        if (currentIndex == 0 && lastIndex == tvObjects.Count - 1)
        {
            loopCount++;
            Debug.Log("LoopCount: " + loopCount);
            if (loopCount >= 1)
            {
                Debug.Log("Two loops completed.");
                currentIndex = -1;
                lastIndex = 0;
                loopCount = 0;
            }
        }
        else
        {
            is2LoopCompleted = false;
        }

        if (currentIndex != -1)
        {
            SetMediaObjects(true);
            SetInfoAndDescriptionActive(false);

            tvObjects[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % tvObjects.Count;

            Debug.Log($"Next Index: {currentIndex}");

            lastIndex = (currentIndex + tvObjects.Count - 1) % tvObjects.Count;

            GameObject nextObject = (currentIndex != -1) ? tvObjects[currentIndex] : null;
            if (nextObject != null)
            {
                VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

                if (videoPlayer != null)
                {
                    videoPlayer.url = videoSettings.videoUrl; // Use URL from VideoSettings
                    if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
                    {
                        nextObject.SetActive(true);
                        StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                        Debug.Log("Playing preloaded video.");
                    }
                    else
                    {
                        StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
                    }

                    CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
                    if (rawImage != null)
                    {
                        if (videoRenderTexture != null)
                        {
                            rawImage.texture = videoRenderTexture;
                        }
                        else
                        {
                            Debug.LogError("Video RenderTexture not assigned.");
                        }
                    }
                }
                else
                {
                    HandleNonVideoObject(nextObject);

                    Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
                    if (rawImage != null && nextMaterial != null)
                    {
                        rawImage.texture = nextMaterial.mainTexture;
                    }
                }
            }
        }
        else
        {
            SetMediaObjects(false);
            SetInfoAndDescriptionActive(true);
            Debug.Log("Yes");
            currentIndex = 0;
            loopCount = 0;
        }
        Debug.Log("Index after next: " + currentIndex);
    }

    public void ToggleBackGameObject()
    {
        if (tvObjects.Count == 0)
        {
            Debug.LogWarning("No TV objects found.");
            return;
        }

        if (currentIndex == 0 && lastIndex == tvObjects.Count - 1)
        {
            loopCount++;
            Debug.Log("LoopCount: " + loopCount);
            if (loopCount >= 1)
            {
                Debug.Log("Two loops completed.");
                currentIndex = -1;
                lastIndex = 0;
                loopCount = 0;
            }
        }
        else
        {
            is2LoopCompleted = false;
        }

        if (currentIndex != -1)
        {
            SetMediaObjects(true);
            SetInfoAndDescriptionActive(false);

            tvObjects[currentIndex].SetActive(false);
            currentIndex = (currentIndex - 1 + tvObjects.Count) % tvObjects.Count;
            Debug.Log($"Current Index: {currentIndex}");

            lastIndex = (currentIndex + tvObjects.Count) % tvObjects.Count;

            GameObject nextObject = (currentIndex != -1) ? tvObjects[currentIndex] : null;
            if (nextObject != null)
            {
                VideoPlayer videoPlayer = nextObject.GetComponent<VideoPlayer>();

                if (videoPlayer != null)
                {
                    videoPlayer.url = videoSettings.videoUrl; // Use URL from VideoSettings
                    if (nextVideoPlayer != null && nextVideoPlayer == videoPlayer)
                    {
                        nextObject.SetActive(true);
                        StartCoroutine(PlayWhenReady(nextVideoPlayer, nextObject));
                        Debug.Log("Playing preloaded video.");
                    }
                    else
                    {
                        StartCoroutine(PrepareAndPlayVideo(nextObject, videoPlayer));
                    }

                    CustomRenderTexture videoRenderTexture = FindCustomRenderTexture("VideoPlayer");
                    if (rawImage != null)
                    {
                        if (videoRenderTexture != null)
                        {
                            rawImage.texture = videoRenderTexture;
                        }
                        else
                        {
                            Debug.LogError("Video RenderTexture not assigned.");
                        }
                    }
                }
                else
                {
                    HandleNonVideoObject(nextObject);

                    Material nextMaterial = nextObject.GetComponent<Renderer>()?.material;
                    if (rawImage != null && nextMaterial != null)
                    {
                        rawImage.texture = nextMaterial.mainTexture;
                    }
                }
            }
        }
        else
        {
            SetMediaObjects(false);
            SetInfoAndDescriptionActive(true);
            Debug.Log("Yes");
            currentIndex = 0;
            loopCount = 0;
        }

        PreloadNextVideo();
        Debug.Log("Index after back: " + currentIndex);
    }

    private void SetInfoAndDescriptionActive(bool isActive)
    {
        GameObject info = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Title");
        GameObject des = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel/Description");
        if (info != null)
        {
            info.SetActive(isActive);
        }
        if (des != null)
        {
            des.SetActive(isActive);
        }
        else
        {
            Debug.LogError("Info GameObject not found.");
        }
    }

    private void HandleNonVideoObject(GameObject obj)
    {
        // Set materials for non-video objects
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = transparentMaterial;
        }
    }

    private void PreloadNextVideo()
    {
        if (tvObjects.Count > 0)
        {
            nextIndex = (currentIndex + 1) % tvObjects.Count;
            GameObject nextObject = tvObjects[nextIndex];

            if (nextObject != null)
            {
                nextVideoPlayer = nextObject.GetComponent<VideoPlayer>();

                if (nextVideoPlayer != null)
                {
                    nextVideoPlayer.Prepare();
                    Debug.Log("Preloading video for next index.");
                }
            }
        }
    }

    private IEnumerator PrepareAndPlayVideo(GameObject nextObject, VideoPlayer videoPlayer)
    {
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        if (videoSettings != null)
        {
            videoPlayer.SetDirectAudioVolume(0, videoSettings.volume);
            videoPlayer.playbackSpeed = videoSettings.speed;
        }

        videoPlayer.Play();
        nextObject.SetActive(true);

        Debug.Log("Playing video: " + videoPlayer.url);
    }

    private IEnumerator PlayWhenReady(VideoPlayer videoPlayer, GameObject nextObject)
    {
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();
        nextObject.SetActive(true);
        Debug.Log("Playing preloaded video.");
    }

    private void SetMediaObjects(bool isActive)
    {
        foreach (var obj in tvObjects)
        {
            obj.SetActive(isActive);
        }
    }

    private CustomRenderTexture FindCustomRenderTexture(string name)
    {
        return GameObject.Find(name)?.GetComponent<CustomRenderTexture>();
    }

    private GameObject FindGameObject(string path)
    {
        return GameObject.Find(path);
    }
    [System.Serializable]
    public class VideoSettings
    {
        public float volume;
        public float speed;
        public string videoUrl; // Add this line for video URL
    }

}
*/