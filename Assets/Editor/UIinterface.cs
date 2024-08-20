using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static AnnotationScript1;
using static TVNavigation;

public class CombinedEditorWindow : EditorWindow
{
    private CombinedEditorWindow window;
    //Import Var.
    private string modelFilePath = string.Empty;
    private string modelPath = string.Empty;
    private Vector2 scrollPosition4;
    private string imagename = null;
    private string mediaFilePath = null;
    private string mediapath = null;
    private GameObject addModelHere;
    private GameObject addMediaHere;
    private GameObject tvObject;
    private const string TV_TAG = "TV";
    private float volume = 1.0f;
    private float speed = 1.0f;
    private string filePath;
    private string videoUrl = "";
    private TVNavigation.VideoSettings videoSettings;
    //Descrp. Var.
    private string newTitle = "";
    private string newDescription = "";
    private Text titleTextObject;
    private Text descriptionTextObject;
    private GameObject popupObject1;
    private const string TitlePlayerPrefsKey = "NewTitle";
    private const string DescriptionPlayerPrefsKey = "NewDescription";
    private bool waitingForPopup = true;
    private bool applyButtonPressed = true;
    private bool applyButtonReleased = true;
    private TitleDescriptionData data = new TitleDescriptionData();
    public string fileName;
    private DataManager dataManager;

    //Annotation Var.
    private GameObject textPrefab;
    private AnnotationDatasLists annotationDatasLists;
    public Renderer targetRenderer;
    private string annotationText;
    private Vector3 offset = Vector3.up * 1;
    private Vector3 rotation = new Vector3(0, 90, 0); // Initial Y rotation set to 90 degrees
    private Material lineMaterial;
    private Canvas targetCanvas;
    private List<ImportedFileInfo> importedFiles = new List<ImportedFileInfo>();
    private string folderPath = "Assets/Colors";
    private List<Material> materialsInFolder = new List<Material>();
    private string filepaths;
    //Anchor Var.
    private List<HotspotData> dots = new List<HotspotData>();
    private HotspotData selectedHotspot;
    //Annotation Var.
    private List<AnnotationDatas> annotations = new List<AnnotationDatas>();
    private AnnotationDatas selectedAnnotation;
    private LineRenderer previewLine;
    private GameObject previewText;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;
    private Vector2 scrollPosition2;
    private Vector2 scrollPosition3;
    private Vector2 scrollPosition5;
    private const string AnnotationsKey = "AnnotationsData";
    //Animation Var.
    private GameObject parentObject; // Reference to the parent GameObject
    private List<GameObject> gameObjectsList = new List<GameObject>();
    private AnimationScript1 animationScript;
    private SerializedObject serializedObject;
    private List<GameObject> allModels;
    private List<GameObject> filteredModels;
    public GameObject selectedModel;
    private List<AnimationClip> animationsList = new List<AnimationClip>();
    private SerializedProperty animationDataArray;
    private int selectedAnimationIndex = -1;
    private int selectedModelIndex = -1;
    private GameObject modelToImportAnimation;
    private List<GameObject> selectedGameObjects = new List<GameObject>();
    private List<AnimationImportHistoryEntry> importHistory = new List<AnimationImportHistoryEntry>();
    private const string animatorPath = "Assets/Resources/default.controller";
    //Material Var.
    [HideInInspector]
    private Vector2 scrollPosition7;
    private List<ChangeMaterial1> changeMaterialScripts = new List<ChangeMaterial1>();
    private Dictionary<ChangeMaterial1, bool> scriptFoldouts = new Dictionary<ChangeMaterial1, bool>();
    private Renderer targetRenderer1;
    private List<Material> materials = new List<Material>();
    private UnityEngine.UI.Button changeMaterialButton; // The button to change the material
    private bool isFourthFunctionEnabled = false;
    private GameObject imageObject;
    private int currentDesignIndex = 0;
    private Texture2D previewTexture;
    private List<Renderer> validRenderers;
    [SerializeField]
    private GameObject dragOptionsGameObject;
    private string jsonFilePath; // Path to save the JSON file
    bool imagesFoldout = true;
    int groupCounter = 1;
    // Track groups of designs
    private List<DesignGroup> designGroups = new List<DesignGroup>();
    //Chatbot Var.
    [SerializeField] private OpenAIController openAIController;
    private string additionalSystemInfo = "";
    private Vector2 scrollPosition6;
    //Boolean Var.
    private bool modelFileFoldout = true;
    private bool mediaFileFoldout = true;
    private bool annotationFoldout = true;
    private bool hotspotFoldout = true; // Added foldout for Hotspot Editor
    private bool descriptionFoldout = true;
    private bool animationFoldout = true;
    private bool materialFoldout = true;
    private bool normalAnimations = true;
    private bool chatbotFoldout = true;

    private bool modelFileEnabled = true;
    private bool mediaFileEnabled = true;
    private bool annotationEnabled = true;
    private bool hotspotEnabled = true; // Added boolean for Hotspot Editor
    private bool descriptionEnabled = true;
    private bool animationEnabled = true;
    private bool materialEnabled = true;
    private bool chatbotEnabled = true;

    private bool reset = false;
    private bool messageShown = false; // Add this flag at the class level
    private bool pinged = false;
    [MenuItem("Tools/Combined Editor")]
    public static void ShowWindow()
    {
        CombinedEditorWindow window = GetWindow<CombinedEditorWindow>("Combined Editor");

        // Initialize title text
        window.titleTextObject = GameObject.Find("Title")?.GetComponent<Text>();
        if (window.titleTextObject == null)
        {
            Debug.LogWarning("Title GameObject or Text component not found.");
        }

        // Initialize description text
        window.descriptionTextObject = GameObject.Find("Description")?.GetComponent<Text>();
        if (window.descriptionTextObject == null)
        {
            Debug.LogWarning("Description GameObject or Text component not found.");
        }

        // Initialize canvas
        Canvas canvas = GameObject.Find("ModelCanvas")?.GetComponent<Canvas>();
        if (canvas != null)
        {
            window.targetCanvas = canvas;
            TextMeshProUGUI textPrefab = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textPrefab != null)
            {
                window.textPrefab = textPrefab.gameObject;
                Debug.Log($"Found TextMeshProUGUI in {canvas.name}");
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI object not found as a child of the ModelCanvas.");
            }
        }
        else
        {
            Debug.LogWarning("ModelCanvas GameObject not found. Ensure it is present in the scene.");
        }

        // Load saved text and annotations
        window.LoadSavedText();
    }

    private void OnGUI()
    {
        GUILayout.Label("Creator UI Functions", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Load All History", "Load all previously added data")))
        {
            LoadAnimations();
            LoadAnnotations();
            LoadDesignData();

        }
        GUILayout.EndHorizontal();
        // Model File Import Section
        GUILayout.BeginHorizontal();
        modelFileEnabled = EditorGUILayout.ToggleLeft("", modelFileEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = modelFileEnabled;
        modelFileFoldout = EditorGUILayout.Foldout(modelFileFoldout, new GUIContent("Model File Import", "Expand to import model file"));
        GUILayout.EndHorizontal();

        if (modelFileFoldout && modelFileEnabled)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Model File Path:", "Path to the model file"), GUILayout.Width(110));
            if (GUILayout.Button(new GUIContent("Browse", "Browse for a FBX model file"), GUILayout.Width(70)))
            {
                modelFilePath = EditorUtility.OpenFilePanel("Select FBX Model File to Import", "", "fbx");
                modelPath = ShortenFileName(modelFilePath);
            }
            GUILayout.TextField(modelPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Import FBX Model", "Import selected FBX model")))
            {
                ImportFBXModel();
            }
            Show3DModels();
        }
        GUI.enabled = true;


        // Title/Description Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        descriptionEnabled = EditorGUILayout.ToggleLeft("", descriptionEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = descriptionEnabled;
        descriptionFoldout = EditorGUILayout.Foldout(descriptionFoldout, new GUIContent("Add Information", "Expand to add title and description"));
        GUILayout.EndHorizontal();

        if (descriptionFoldout && descriptionEnabled)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Title:", "Enter the title of the Model"), EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Show Preview", "Preview the title and description."), GUILayout.Width(110)))
            {
                
                applyButtonPressed = false;
                applyButtonReleased = false;
                // Find and focus on Popup GameObject
                FocusOnPopup();
            }
            GUILayout.EndHorizontal();



            newTitle = EditorGUILayout.TextField(newTitle);
            data.title = newTitle;
            GUILayout.BeginHorizontal();
            // Use TextArea for multiline text input
            GUILayout.Label(new GUIContent("Description", "Enter the Description of the Model"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            // Create a style with word wrap enabled
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            scrollPosition1 = EditorGUILayout.BeginScrollView(scrollPosition1, GUILayout.Height(100)); // Adjust the height as needed
            newDescription = EditorGUILayout.TextArea(newDescription, textAreaStyle, GUILayout.ExpandHeight(true)); // Apply the custom style
            data.description = newDescription;
            EditorGUILayout.EndScrollView(); // End scroll view
            // Clear text fields if reset is true
            if (reset)
            {
                newTitle = "";
                newDescription = "";
                reset = false; // Reset the flag to prevent repeated clearing
                Debug.Log(newTitle);
                Debug.Log(newDescription);
            }

            // Apply changes in real-time
            ApplyChanges();
            if (GUILayout.Button(new GUIContent("Save", "Save the title and description changes")))
            {
                ApplyChanges();
                if (popupObject1 != null)
                {
                    applyButtonPressed = true;
                    popupObject1.SetActive(false);
                    Debug.Log("Popup Disabled");
                }
            }


        }
        GUI.enabled = true;

        // Media File Import Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        mediaFileEnabled = EditorGUILayout.ToggleLeft("", mediaFileEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = mediaFileEnabled;
        mediaFileFoldout = EditorGUILayout.Foldout(mediaFileFoldout, new GUIContent("Media Import", "Expand to import media files"));
        GUILayout.EndHorizontal();
        if (mediaFileFoldout && mediaFileEnabled)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Media File Path:", GUILayout.Width(100));
            if (GUILayout.Button(new GUIContent("Browse", "Select Media File to Import, supports only JPG, JPEG, and PNG formats"), GUILayout.Width(70)))
            {
                mediaFilePath = EditorUtility.OpenFilePanel("Select Media File to Import", "", "mp4,mov,jpg,png,jpeg");
            }
            mediapath = mediaFilePath;
            mediapath = GUILayout.TextField(ShortenFileName(mediapath));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Import Media File", "Import the selected media file")))
            {
                ImportMediaFile();
            }
            GUILayout.Space(10);
            /*
            GUILayout.Label("Video Settings", EditorStyles.boldLabel);
            
            volume = EditorGUILayout.Slider("Volume", volume, 0.0f, 1.0f);
            speed = EditorGUILayout.Slider("Playback Speed", speed, 0.5f, 2.0f);
            videoUrl = EditorGUILayout.TextField("Video URL", videoUrl);

            if (GUILayout.Button("Save Settings"))
            {
                SaveSettings();
            }

            if (GUILayout.Button("Load Settings"))
            {
                LoadSettings();
            }
            GUILayout.Space(10);
            */
            GUILayout.Label(new GUIContent("Imported Files", "List of imported files"), EditorStyles.boldLabel);
            ShowImportedFiles();
        }
        GUI.enabled = true;

        // Hotspot Editor Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        hotspotEnabled = EditorGUILayout.ToggleLeft("", hotspotEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = hotspotEnabled;
        hotspotFoldout = EditorGUILayout.Foldout(hotspotFoldout, new GUIContent("Annotation Anchor Editor", "Expand to edit annotation anchors"));
        GUILayout.EndHorizontal();
        if (hotspotFoldout && hotspotEnabled)
        {
            GUILayout.Space(10);

            GUILayout.Label(new GUIContent("All Anchors", "List of all annotation anchors"), EditorStyles.boldLabel);

            List<HotspotData> dotsCopy = new List<HotspotData>(dots);
            foreach (HotspotData dotData in dotsCopy)
            {
                if (dotData != null)
                {
                    try
                    {
                        GUILayout.BeginHorizontal();
                        try
                        {
                            if (GUILayout.Button(new GUIContent(dotData.name, "Edit this hotspot")))
                            {
                                OpenHotspotEditor(dotData);
                            }

                            if (dotData.dot != null)
                            {
                                GUILayout.Label($"ID: {dotData.dot.GetInstanceID()}");
                            }

                            if (GUILayout.Button(new GUIContent("Remove", "Remove this hotspot")))
                            {
                                RemoveHotspot(dotData);
                                break;
                            }
                        }
                        finally
                        {
                            GUILayout.EndHorizontal();
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        dots.Remove(dotData);
                    }
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("Add Annotation Anchor", "Add a new annotation anchor")))
            {
                SpawnHotspot();
            }
        }
        GUI.enabled = true;

        // Annotation Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        annotationEnabled = EditorGUILayout.ToggleLeft("", annotationEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = annotationEnabled;
        annotationFoldout = EditorGUILayout.Foldout(annotationFoldout, new GUIContent("Add Annotation", "Expand to add an annotation"));
        GUILayout.EndHorizontal();
        FindValidAnchorRenderers("AddModelHere", "Media");
        if (annotationFoldout && annotationEnabled)
        {
            GUILayout.Space(10);
            bool missingFields = annotationText == "" || targetRenderer == null || lineMaterial == null;

            if (missingFields)
            {
                EditorGUILayout.HelpBox("Please fill in all required fields.", MessageType.Warning);
            }

            //targetRenderer = EditorGUILayout.ObjectField(new GUIContent("Component", "Select the component for the annotation"), targetRenderer, typeof(Renderer), true) as Renderer;
            targetRenderer = CustomAnchorRendererDropdown("Component", targetRenderer);
            if (materialsInFolder.Any())
            {
                // Display a dropdown of materials
                string[] materialNames = materialsInFolder.Select(m => m.name).ToArray();
                int selectedIndex = materialsInFolder.IndexOf(lineMaterial);
                int newSelectedIndex = EditorGUILayout.Popup("Select Material", selectedIndex, materialNames);

                if (newSelectedIndex != selectedIndex)
                {
                    lineMaterial = materialsInFolder[newSelectedIndex];
                }
            }
            annotationText = EditorGUILayout.TextField(new GUIContent("Annotation Text", "Enter the annotation text"), annotationText);

            GUILayout.Label(new GUIContent("Offset", "Set the offset for the annotation"), EditorStyles.boldLabel);
            offset.x = EditorGUILayout.Slider(new GUIContent("X", "Adjust the X offnset of the annotation"), Mathf.Round(offset.x * 10f) / 10f, -10f, 10f);
            offset.y = EditorGUILayout.Slider(new GUIContent("Y", "Adjust the Y offset of the annotation"), Mathf.Round(offset.y * 10f) / 10f, -10f, 10f);
            offset.z = EditorGUILayout.Slider(new GUIContent("Z", "Adjust the Z offset of the annotation"), Mathf.Round(offset.z * 10f) / 10f, -10f, 10f);

            GUILayout.Label(new GUIContent("Rotation", "Set the rotation for the annotation"), EditorStyles.boldLabel);
            rotation.x = EditorGUILayout.Slider(new GUIContent("X", "Adjust the X rotation of the annotation"), Mathf.Round(rotation.x * 10f) / 10f, -180f, 180f);
            rotation.y = EditorGUILayout.Slider(new GUIContent("Y", "Adjust the Y rotation of the annotation"), Mathf.Round(rotation.y * 10f) / 10f, -180f, 180f);
            rotation.z = EditorGUILayout.Slider(new GUIContent("Z", "Adjust the Z rotation of the annotation"), Mathf.Round(rotation.z * 10f) / 10f, -180f, 180f);

            if (GUILayout.Button(new GUIContent("Save", "Save all annotations")) && !missingFields)
            {
                if (targetRenderer is MeshRenderer || targetRenderer is SkinnedMeshRenderer)
                {
                    Debug.Log("Save Successful");

                    // Create a new annotation
                    AnnotationDatas newAnnotation = new AnnotationDatas()
                    {
                        ownText = annotationText,
                        offset = offset,
                        rotation = rotation,
                        newMat = lineMaterial
                    };

                    // Set the target renderer
                    newAnnotation.SetTargetRenderer(targetRenderer);

                    // Add the annotation to the list
                    annotations.Add(newAnnotation);

                    // Create an AnnotationDatasLists object
                    AnnotationDatasLists annotationDatasLists = new AnnotationDatasLists()
                    {
                        annotations = annotations
                    };

                    // Save the annotations to the JSON file
                    SaveAnnotations(annotationDatasLists);
                    AddAnnotation(newAnnotation, offset, rotation, targetCanvas);
                    lineMaterial = null;
                    ResetFields();
                    DestroyPreview();
                }
                else
                {
                    Debug.LogWarning("Target Renderer must be either a MeshRenderer or a SkinnedMeshRenderer.");
                }
            }

            if (GUILayout.Button(new GUIContent("Cancel", "Cancel the Annotation Set-up")))
            {
                ResetFields();
                DestroyPreview();
            }
            GUILayout.Label(new GUIContent("Annotations List", "List of all Annotations"), EditorStyles.boldLabel);

            if (annotations.Count == 0)
            {
                GUILayout.Label("No annotation entries yet.", EditorStyles.label);
            }
            else
            {
                List<AnnotationDatas> annotationsToRemove = new List<AnnotationDatas>();

                foreach (AnnotationDatas annotation in annotations)
                {
                    if (annotation == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"ID: {annotation.rendererInstanceID}, Text: {annotation.ownText}", EditorStyles.label);

                    if (GUILayout.Button("Edit"))
                    {
                        // Load the selected annotation's data into the editor fields
                        selectedAnnotation = annotation;
                        annotationText = annotation.ownText;
                        offset = annotation.offset;
                        rotation = annotation.rotation;
                        lineMaterial = annotation.newMat;
                        targetRenderer = FindRendererByInstanceID(annotation.rendererInstanceID);
                        ShowEditWindow();
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        annotationsToRemove.Add(annotation); // Collect the annotation to remove later
                    }

                    EditorGUILayout.EndHorizontal();
                }

                // Remove the annotations after iterating through the list
                foreach (var annotationToRemove in annotationsToRemove)
                {
                    RemoveAnnotation(annotationToRemove);
                }
            }

            if (GUI.changed && !missingFields)
            {
                DrawAnnotationPreview(offset, rotation, annotationText);
            }
        }
        GUI.enabled = true;


        //Animation Section

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        animationEnabled = EditorGUILayout.ToggleLeft("", animationEnabled, GUILayout.Width(20));
        GUI.enabled = animationEnabled;
        animationFoldout = EditorGUILayout.Foldout(animationFoldout, new GUIContent("Add Animation", "Expand to add animations"));
        GUILayout.EndHorizontal();

        if (animationFoldout && animationEnabled)
        {
            GUILayout.BeginVertical();
            normalAnimations = EditorGUILayout.Foldout(normalAnimations, new GUIContent("Add Animations from Folder", "Expand to add animations from a folder"));
            if (normalAnimations)
            {
                // Animation selection dropdown
                GUILayout.Label(new GUIContent("Select Animation:", "Select an animation to import"), GUILayout.Width(101));
                // Filter animations that can be previewed
                List<AnimationClip> previewableAnimations = animationsList.FindAll(anim => CanPreviewAnimation(anim));
                if (previewableAnimations.Count > 0)
                {
                    string[] animationNames = previewableAnimations.ConvertAll(anim => anim.name).ToArray();
                    selectedAnimationIndex = EditorGUILayout.Popup(selectedAnimationIndex, animationNames);
                }
                else
                {
                    GUILayout.Label(new GUIContent("No animations found.", "No animations available for preview"), EditorStyles.label);
                    selectedAnimationIndex = -1;
                }

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Select Model to Import:", "Select a model to import the animation into"), GUILayout.Width(150));

                // Dropdown list for selecting models excluding "Media"
                List<string> modelNamesList = new List<string>();
                PopulateGameObjectsList();
                foreach (var obj in gameObjectsList)
                {
                    if (obj.name != "Media")
                    {
                        modelNamesList.Add(obj.name);
                    }
                }
                string[] modelNames = modelNamesList.ToArray();

                selectedModelIndex = EditorGUILayout.Popup(selectedModelIndex, modelNames);

                GUILayout.EndHorizontal();

                // Update selectedGameObjects based on dropdown selection
                selectedGameObjects.Clear();
                if (selectedModelIndex >= 0 && selectedModelIndex < gameObjectsList.Count)
                {
                    selectedGameObjects.Add(gameObjectsList[selectedModelIndex]);
                }

                GUILayout.Space(10);

                // Import button
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Import Animation to Model", "Import the selected animation to the selected model"), GUILayout.Width(200)))
                {
                    if (selectedAnimationIndex != -1 && selectedGameObjects.Count > 0)
                    {
                        AnimationClip selectedAnimation = previewableAnimations[selectedAnimationIndex];
                        List<GameObject> successfullyImportedModels = new List<GameObject>();

                        foreach (var model in selectedGameObjects)
                        {
                            // Check if model has an avatar
                            Animator animator = model.GetComponent<Animator>();
                            if (animator == null)
                            {
                                EditorUtility.DisplayDialog("Import Error", "The selected model does not have an avatar. Import aborted.", "OK");
                                continue;
                            }
                            ImportAnimationToModel(selectedAnimation, model);
                            successfullyImportedModels.Add(model);
                        }

                        // Add to history
                        foreach (var model in successfullyImportedModels)
                        {
                            AnimationImportHistoryEntry entry = new AnimationImportHistoryEntry
                            {
                                model = model,
                                clip = selectedAnimation
                            };
                            importHistory.Add(entry);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Please select an animation and a model to import into.");
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                // Display history
                GUILayout.Label(new GUIContent("Import History", "History of imported animations"), EditorStyles.boldLabel);
                if (importHistory.Count > 0)
                {
                    for (int i = 0; i < importHistory.Count; i++)
                    {
                        if (importHistory[i].model != null && importHistory[i].clip != null)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(new GUIContent("Entry " + (i + 1) + ":", "Animation import entry number"), GUILayout.Width(80));
                            GUILayout.Label(new GUIContent("Model: " + importHistory[i].model.name, "Name of the model"));
                            GUILayout.Label(new GUIContent("Clip: " + importHistory[i].clip.name, "Name of the animation clip"));

                            if (GUILayout.Button(new GUIContent("Delete", "Delete this import history entry"), GUILayout.Width(60)))
                            {
                                DeleteHistoryEntry(i);
                            }

                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            // Optionally remove the null entry from the list if needed
                            importHistory.RemoveAt(i);
                            i--; // Adjust index after removal
                        }
                    }
                }
                else
                {
                    GUILayout.Label(new GUIContent("No animation entries yet.", "No animation import history available"), EditorStyles.label);
                }

            }

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        GUI.enabled = true;


        // Material Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        materialEnabled = EditorGUILayout.ToggleLeft("", materialEnabled, GUILayout.Width(20)); // Adjust width as needed
        GUI.enabled = materialEnabled;
        materialFoldout = EditorGUILayout.Foldout(materialFoldout, new GUIContent("Add Design", "Expand to add designs"));
        GUILayout.EndHorizontal();
        FindValidRenderers("AddModelHere", "Media");

        if (dragOptionsGameObject == null)
        {
            FindAndActivateDragOptions();
        }
        UpdateDragOptionsActiveState();

        if (materialFoldout && materialEnabled)
        {
            // Buttons at the top
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add Design", "Add a new design group")))
            {
                designGroups.Add(new DesignGroup($"Design Group {groupCounter++}"));
            }
            if (GUILayout.Button(new GUIContent("Add Compartment", "Add a new compartment to the current design group")))
            {
                if (designGroups.Count > 0)
                {
                    DesignGroup lastGroup = designGroups[designGroups.Count - 1];
                    ChangeMaterial1 newScript = dragOptionsGameObject.AddComponent<ChangeMaterial1>();
                    lastGroup.compartments.Add(newScript);
                    scriptFoldouts[newScript] = false; // Initialize foldout state
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please add a design group first.", "OK");
                }
            }
            GUILayout.EndHorizontal();

            // Iterating through design groups
            for (int i = 0; i < designGroups.Count; i++)
            {
                var group = designGroups[i];
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                group.foldout = EditorGUILayout.Foldout(group.foldout, group.groupName);
                if (GUILayout.Button(new GUIContent("Duplicate Group", "Duplicate this design group"), GUILayout.Width(150)))
                {
                    DuplicateGroup(group);
                }
                if (GUILayout.Button(new GUIContent("Remove Group", "Remove this design group"), GUILayout.Width(150)))
                {
                    RemoveGroup(i);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return; // Exit loop after removal
                }
                GUILayout.EndHorizontal();

                // Display compartments if the group is unfolded
                if (group.foldout)
                {
                    for (int j = 0; j < group.compartments.Count; j++)
                    {
                        var script = group.compartments[j];

                        // Check if script is null
                        if (script == null)
                        {
                            Debug.LogWarning($"Encountered null script at index {j}. Removing from compartments.");
                            group.compartments.RemoveAt(j);
                            j--; // Adjust index after removal
                            continue;
                        }

                        GUILayout.BeginVertical(EditorStyles.helpBox);

                        EditorGUILayout.BeginHorizontal();
                        // Check if targetRenderer is null
                        string labelName = script.targetRenderer != null ? script.targetRenderer.name : "Material";
                        GUILayout.Label(new GUIContent(labelName, "Name of the material or target renderer"), GUILayout.Width(100));

                        if (GUILayout.Button(new GUIContent("Remove", "Remove this compartment"), GUILayout.Width(120)))
                        {
                            // Safely remove the script and compartment
                            group.compartments.RemoveAt(j);

                            // Remove the script from dragOptionsGameObject
                            if (dragOptionsGameObject != null && script != null)
                            {
                                DestroyImmediate(script);
                            }

                            EditorGUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            break; // Break to avoid modifying the collection during enumeration
                        }

                        if (GUILayout.Button(new GUIContent("Assign Material", "Assign a material to this compartment"), GUILayout.Width(120)))
                        {
                            ToggleFoldout(script);
                        }

                        EditorGUILayout.EndHorizontal();

                        // Display material settings if foldout is open
                        if (scriptFoldouts.TryGetValue(script, out bool isFoldout) && isFoldout)
                        {
                            EditorGUI.indentLevel++;
                            RenderMaterialScript(script);
                            EditorGUI.indentLevel--;
                        }

                        GUILayout.EndVertical();
                    }
                }

                GUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Save Design", "Save the current design")))
            {
                SaveDesign();
                SaveDesignData();
            }

            // Add the Picture Import Section
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Import Thumbnail", "Import a Thumbnail for the design"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Media File Path:", "Path to the media file"), GUILayout.Width(100));

            if (GUILayout.Button(new GUIContent("Browse", "Browse for a Thumbnail, Supports JPG, PNG, JPEG"), GUILayout.Width(70)))
            {
                mediaFilePath = EditorUtility.OpenFilePanel("Select Thumbnail to Import", "", "jpg,png,jpeg");
                if (!string.IsNullOrEmpty(mediaFilePath))
                {
                    LoadPreviewImage();
                }
            }
            mediapath = mediaFilePath;
            mediapath = GUILayout.TextField(ShortenFileName(mediapath));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (previewTexture != null)
            {
                GUILayout.Label(new GUIContent(previewTexture, "Preview of the Thumbnail"), GUILayout.Width(100), GUILayout.Height(100));
            }

            if (GUILayout.Button(new GUIContent("Import Thumbnail File", "Import the selected thumbnail file for design visual")))
            {
                ImportImageFile();
            }

            GUILayout.Space(5);
            GUILayout.Label(new GUIContent("Imported Thumbnails", "List of imported thumbnails"), EditorStyles.boldLabel);
            imagesFoldout = EditorGUILayout.Foldout(imagesFoldout, new GUIContent("Show Imported Thumbnails", "Toggle to show or hide imported thumbnails"));
            if (imagesFoldout)
            {
                ShowImportedImages();
            }
        }
        GUI.enabled = true;


        // Chatbot Section
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        chatbotEnabled = EditorGUILayout.ToggleLeft(new GUIContent("", "Enable or disable the AI chatbot section"), chatbotEnabled, GUILayout.Width(20));
        GUI.enabled = chatbotEnabled;
        chatbotFoldout = EditorGUILayout.Foldout(chatbotFoldout, new GUIContent("AI Chatbot Configuration", "Expand to configure the AI chatbot"));
        GUILayout.EndHorizontal();
        if (chatbotFoldout && chatbotEnabled)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Additional Product Info:", "Add additional information for the AI chatbot"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            scrollPosition6 = EditorGUILayout.BeginScrollView(scrollPosition6, GUILayout.Height(100));
            additionalSystemInfo = EditorGUILayout.TextArea(additionalSystemInfo, textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button(new GUIContent("Update AI Chatbot", "Update the AI chatbot with the provided information")))
            {
                UpdateSystemMessage();
            }
        }
        GUI.enabled = true;

        GUILayout.EndScrollView(); // End ScrollView
    }

    private void CopyComponent(ChangeMaterial1 source, ChangeMaterial1 destination)
    {
        // Copy targetRenderer and targetRendererPath
        destination.targetRenderer = source.targetRenderer;
        destination.targetRendererPath = source.targetRendererPath;

        // Initialize new materials array
        destination.materials = new Material[source.materials.Length];

        // Ensure source.materials is not null
        if (source.materials == null)
        {
            Debug.LogError("Source materials array is null.");
            return;
        }

        // Copy materials from source to destination
        for (int i = 0; i < source.materials.Length; i++)
        {
            Debug.Log($"Ori Mat {i}: ", source.materials[i]);
            if (source.materials[i] != null)
            {
                // Create a new instance of the material
                Material originalMat = source.materials[i];
                Material newMat = new Material(originalMat);
                newMat.CopyPropertiesFromMaterial(originalMat);

                // Assign new material to destination
                destination.materials[i] = newMat;
            }
            else
            {
                // Handle case where source.materials[i] is null
                destination.materials[i] = null;
                Debug.LogWarning($"Source material at index {i} is null.");
            }
        }

        // Copy other properties
        destination.currentMaterialIndex = source.currentMaterialIndex;
        destination.materialsAutofilled = source.materialsAutofilled;
    }


    private void DuplicateGroup(DesignGroup originalGroup)
    {
        var newGroup = new DesignGroup($"Design Group {groupCounter++}");
        foreach (var compartment in originalGroup.compartments)
        {
            if (compartment != null)
            {
                var newCompartment = dragOptionsGameObject.AddComponent<ChangeMaterial1>();
                CopyComponent(compartment, newCompartment);

                // Ensure selected material is copied
                if (compartment.selectedMaterial != null)
                {
                    newCompartment.selectedMaterial = newCompartment.materials
                        .FirstOrDefault(m => m.name == compartment.selectedMaterial.name);
                }

                newGroup.compartments.Add(newCompartment);
                scriptFoldouts[newCompartment] = false;
            }
            else
            {
                Debug.LogWarning("Encountered null compartment during duplication.");
            }
        }
        designGroups.Add(newGroup);
    }

    [System.Serializable]
    public class DesignGroup
    {
        public string groupName;
        public List<CompartmentData> compartment = new List<CompartmentData>();
        [System.NonSerialized] public List<ChangeMaterial1> compartments = new List<ChangeMaterial1>();
        public bool foldout = true;
        public DesignGroup(string name)
        {
            groupName = name;
        }
        public DesignGroup()
        {


        }
    }

    [System.Serializable]
    public class CompartmentData
    {
        public string targetRendererPath;  // Serialize the path instead of the renderer
        public List<string> materialPaths = new List<string>(); // Serialize paths of materials
    }
    [System.Serializable]
    public class DesignData
    {
        public List<DesignGroup> designGroups = new List<DesignGroup>();
    }


    private void SaveDesignData()
    {
        // Create a new DesignData object to hold all design groups
        DesignData data = new DesignData();

        foreach (var group in designGroups)
        {
            DesignGroup groupData = new DesignGroup
            {
                groupName = group.groupName,
                foldout = group.foldout
            };

            foreach (var compartment in group.compartments)
            {
                CompartmentData compartmentData = new CompartmentData();
                compartment.UpdateTargetRendererPath();
                compartmentData.targetRendererPath = compartment.targetRendererPath;
                compartmentData.materialPaths = compartment.GetMaterialPaths();

                groupData.compartment.Add(compartmentData);
            }

            data.designGroups.Add(groupData);
        }

        // Serialize the DesignData object to JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Design data saved to JSON file at " + jsonFilePath);
    }



    private void RemoveGroup(int index)
    {
        if (index < 0 || index >= designGroups.Count)
        {
            Debug.LogError($"Invalid index {index} for removing group. List Count: {designGroups.Count}");
            return;
        }

        // Get the group to be removed
        var group = designGroups[index];
        if (group == null)
        {
            Debug.LogError($"No group found at index {index}. List Count: {designGroups.Count}");
            return;
        }

        Debug.Log($"Attempting to remove all scripts from group at index {index} with {group.compartments.Count} compartments.");

        // Remove all scripts/components from the group
        foreach (var script in group.compartments.ToList()) // Use ToList to avoid modifying the collection while iterating
        {
            if (script != null)
            {
                RemoveComponent(script); // Assuming RemoveComponent handles the necessary cleanup for individual components
            }
        }

        // Remove the group from designGroups
        designGroups.RemoveAt(index);

        // Update group names and reset the counter if needed
        for (int i = 0; i < designGroups.Count; i++)
        {
            designGroups[i].groupName = $"Design Group {i + 1}";
        }

        groupCounter = designGroups.Count + 1;

        // Optionally, you may want to update JSON data after modification
        SaveDesignData();
    }

    private void RemoveAllScriptsFromDragOptionsGameObject()
    {
        // Get all components attached to dragOptionsGameObject
        Component[] components = dragOptionsGameObject.GetComponents<Component>();

        // Iterate over each component and remove it
        foreach (var component in components)
        {
            if (component != null && !(component is Transform)) // Avoid removing the Transform component
            {
                DestroyImmediate(component);
            }
        }

        Debug.Log("Removed all scripts from dragOptionsGameObject.");
    }


    private void LoadDesignData()
    {
        // Remove existing ChangeMaterial1 components from dragOptionsGameObject
        var existingCompartments = dragOptionsGameObject.GetComponents<ChangeMaterial1>();
        foreach (var compartment in existingCompartments)
        {
            DestroyImmediate(compartment); // Use DestroyImmediate for Editor scripts
        }

        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            DesignData data = JsonUtility.FromJson<DesignData>(json);

            designGroups.Clear(); // Clear current design groups

            foreach (var groupData in data.designGroups)
            {
                var newGroup = new DesignGroup
                {
                    groupName = groupData.groupName,
                    foldout = groupData.foldout
                };

                foreach (var compartmentData in groupData.compartment)
                {
                    ChangeMaterial1 compartment = dragOptionsGameObject.AddComponent<ChangeMaterial1>();
                    compartment.targetRenderer = FindRendererByPath(compartmentData.targetRendererPath);

                    if (compartment.targetRenderer == null)
                    {
                        Debug.LogWarning($"Renderer not found for path: {compartmentData.targetRendererPath}");
                        continue;
                    }

                    List<Material> materials = new List<Material>();
                    foreach (string materialPath in compartmentData.materialPaths)
                    {
                        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                        if (mat != null)
                        {
                            materials.Add(mat);
                        }
                        else
                        {
                            Debug.LogWarning($"Material not found at path: {materialPath}");
                        }
                    }

                    compartment.materials = materials.ToArray();
                    compartment.materialsAutofilled = true; // Mark materials as autofilled

                    // Set the selected material (if there are materials loaded)
                    if (materials.Count > 0)
                    {
                        // Example: use the first material from the list as the selected one
                        compartment.selectedMaterial = materials[0];
                    }
                    else
                    {
                        compartment.selectedMaterial = null;
                    }

                    newGroup.compartments.Add(compartment);
                }

                designGroups.Add(newGroup);
            }

            // Refresh the editor window UI with the loaded data
            Repaint();

            Debug.Log("Design data loaded and UI refreshed.");
        }
        else
        {
            Debug.LogWarning("JSON file not found. No design data loaded."); 
        }
    }

    private Renderer FindRendererByPath(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            Debug.Log("object: ", obj);
            return obj.GetComponent<Renderer>();

        }
        return null;
    }

    private void SaveDesign()
    {
        GameObject dragOptionsHere = GameObject.Find("DragOptionsHere");

        if (dragOptionsHere == null)
        {
            Debug.LogWarning("DragOptionsHere not found.");
            return;
        }

        // Clear existing ChangeMaterial1 scripts from dragOptionsHere GameObject
        ChangeMaterial1[] existingScripts = dragOptionsHere.GetComponents<ChangeMaterial1>();

        foreach (var existingScript in existingScripts)
        {
            DestroyImmediate(existingScript);
        }
        /*
        ChangeMaterial1[] moreScripts = dragOptionsGameObject.GetComponents<ChangeMaterial1>();
        foreach (var moreScript in moreScripts)
        {
            DestroyImmediate(moreScript);
        }
        */
        // Dictionary to store scripts by targetRenderer
        Dictionary<Renderer, ChangeMaterial1> rendererToScriptMap = new Dictionary<Renderer, ChangeMaterial1>();

        // Iterate through design groups and compartments
        foreach (var group in designGroups)
        {
            foreach (var script in group.compartments)
            {
                if (script == null || script.targetRenderer == null)
                {
                    continue;
                }

                if (!rendererToScriptMap.TryGetValue(script.targetRenderer, out ChangeMaterial1 existingScript))
                {
                    existingScript = dragOptionsHere.AddComponent<ChangeMaterial1>();
                    existingScript.targetRenderer = script.targetRenderer;
                    existingScript.materials = new Material[0];
                    existingScript.currentMaterialIndex = script.currentMaterialIndex;

                    rendererToScriptMap.Add(script.targetRenderer, existingScript);
                }

                // Combine materials from different design groups
                List<Material> materialsList = new List<Material>(existingScript.materials);
                materialsList.AddRange(script.materials);
                existingScript.materials = materialsList.ToArray();

                Debug.Log($"Added materials for {group.groupName}, Target Renderer: {script.targetRenderer.name}");
            }
        }

        // Save designs and materials
        foreach (var kvp in rendererToScriptMap)
        {
            ChangeMaterial1 script = kvp.Value;

            // Save current material index (assuming it's the same for all materials)
            PlayerPrefs.SetInt($"{script.targetRenderer.name}_MaterialIndex", script.currentMaterialIndex);

            // Save material references
            for (int k = 0; k < script.materials.Length; k++)
            {
                if (script.materials[k] != null)
                {
                    string materialKey = $"{script.targetRenderer.name}_Material{k + 1}"; // Adjusted indices to start from 1
                    PlayerPrefs.SetString(materialKey, AssetDatabase.GetAssetPath(script.materials[k]));
                    Debug.Log($"Saved material reference for {script.targetRenderer.name}, Material {k + 1}: {AssetDatabase.GetAssetPath(script.materials[k])}");
                }
            }
        }

        PlayerPrefs.SetInt("DesignCount", rendererToScriptMap.Count); // Use renderer count as design count

        PlayerPrefs.Save();
        Debug.Log("DesignGroup:" + rendererToScriptMap.Count);
        Debug.Log("Design saved successfully.");
    }




    private void RemoveComponent(ChangeMaterial1 script)
    {
        scriptFoldouts.Remove(script); // Remove the corresponding foldout state
        DestroyImmediate(script);
        changeMaterialScripts.Remove(script);

    }

    /*
    private void RenderMaterialScript(ChangeMaterial1 script)
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        try
        {
            Renderer previousRenderer = script.targetRenderer;
            //script.targetRenderer = (Renderer)EditorGUILayout.ObjectField("Target Renderer", script.targetRenderer, typeof(Renderer), true);
            script.targetRenderer = CustomRendererDropdown("Component", script.targetRenderer);
            if (script.targetRenderer != previousRenderer || !script.materialsAutofilled)
            {
                if (script.targetRenderer != null)
                {
                    Material[] rendererMaterials = script.targetRenderer.sharedMaterials;
                    script.materials = new Material[rendererMaterials.Length];

                    for (int i = 0; i < rendererMaterials.Length; i++)
                    {
                        script.materials[i] = rendererMaterials[i];
                    }
                    script.materialsAutofilled = true; // Mark materials as autofilled
                }
                else
                {
                    script.materials = null;
                }
            }

            if (script.materials != null)
            {
                EditorGUILayout.LabelField($"Materials Count: {script.materials.Length}");
                for (int i = 0; i < script.materials.Length; i++)
                {
                    script.materials[i] = (Material)EditorGUILayout.ObjectField($"Material {i + 1}", script.materials[i], typeof(Material), false);
                }
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(script);
        }
    }
    */
    private void RenderMaterialScript(ChangeMaterial1 script)
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        try
        {
            Renderer previousRenderer = script.targetRenderer;
            script.targetRenderer = CustomRendererDropdown("Component", script.targetRenderer);

            if (script.targetRenderer != previousRenderer || !script.materialsAutofilled)
            {
                if (script.targetRenderer != null)
                {
                    // Autofill materials from the target renderer
                    Material[] rendererMaterials = script.targetRenderer.sharedMaterials;
                    if (rendererMaterials != null)
                    {
                        script.materials = new Material[rendererMaterials.Length];

                        for (int i = 0; i < rendererMaterials.Length; i++)
                        {
                            script.materials[i] = rendererMaterials[i];
                        }

                        // Autofill default material if no material is selected
                        if (script.selectedMaterial == null && script.materials.Length > 0)
                        {
                            script.selectedMaterial = script.materials[0];
                        }

                        script.materialsAutofilled = true; // Mark materials as autofilled
                    }
                    else
                    {
                        script.materials = new Material[0];
                        script.selectedMaterial = null;
                    }
                }
                else
                {
                    script.materials = new Material[0];
                    script.selectedMaterial = null;
                }
            }

            // Find and display materials from the folder
            if (script.targetRenderer != null)
            {
                GameObject addModelHere = GameObject.Find("AddModelHere");
                if (addModelHere != null)
                {
                    string textureFolderName = string.Empty;
                    foreach (Transform child in addModelHere.transform)
                    {
                        if (child.name != "Media")
                        {
                            textureFolderName = child.name.Replace("(Clone)", "");
                            textureFolderName = child.name + "_Textures";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(textureFolderName))
                    {
                        string folderPath = $"Assets/{textureFolderName}";

                        // Ensure the folder exists
                        if (!AssetDatabase.IsValidFolder(folderPath))
                        {
                            AssetDatabase.CreateFolder("Assets", textureFolderName);
                        }

                        if (script.targetRenderer != null)
                        {
                            // Retrieve all materials from the target renderer
                            Material[] originalMaterials = script.targetRenderer.sharedMaterials;

                            // Create a list to store the new materials
                            List<Material> newMaterials = new List<Material>();

                            // Iterate through each material in the target renderer
                            foreach (Material originalMat in originalMaterials)
                            {
                                if (originalMat != null)
                                {
                                    // Define the path for the new material
                                    string materialPath = $"{folderPath}/{originalMat.name}.mat";

                                    // Check if the material already exists at the path
                                    Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                                    if (existingMaterial == null)
                                    {
                                        // Create a new material if one doesn't exist
                                        Material newMat = new Material(originalMat.shader);
                                        newMat.CopyPropertiesFromMaterial(originalMat);
                                        AssetDatabase.CreateAsset(newMat, materialPath);
                                        AssetDatabase.SaveAssets(); // Ensure the asset is saved
                                        existingMaterial = newMat;
                                    }

                                    // Add the material (new or existing) to the list of materials to use
                                    newMaterials.Add(existingMaterial);
                                }
                            }

                            // Update the targetRenderer's materials with the newly created or updated materials
                            script.targetRenderer.sharedMaterials = newMaterials.ToArray();
                        }

                        // Instantiate materials from Assets/Colors
                        string colorFolderPath = "Assets/Colors";
                        if (AssetDatabase.IsValidFolder(colorFolderPath))
                        {
                            string[] colorMaterialGuids = AssetDatabase.FindAssets("t:Material", new[] { colorFolderPath });
                            Material[] colorMaterialArray = colorMaterialGuids
                                .Select(guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)))
                                .Where(m => m != null)
                                .ToArray();

                            foreach (Material colorMat in colorMaterialArray)
                            {
                                if (colorMat != null)
                                {
                                    string materialPath = $"{folderPath}/{colorMat.name}.mat";
                                    if (!AssetDatabase.LoadAssetAtPath<Material>(materialPath))
                                    {
                                        Material newMat = new Material(colorMat.shader);
                                        newMat.CopyPropertiesFromMaterial(colorMat);
                                        AssetDatabase.CreateAsset(newMat, materialPath);
                                    }
                                }
                            }
                        }

                        // Display materials for selection
                        string[] materialNames = AssetDatabase.FindAssets("t:Material", new[] { folderPath })
                            .Select(guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)))
                            .Where(m => m != null)
                            .Select(m => m.name)
                            .ToArray();

                        int selectedIndex = Array.IndexOf(materialNames, script.selectedMaterial != null ? script.selectedMaterial.name : string.Empty);
                        selectedIndex = Mathf.Clamp(selectedIndex, 0, materialNames.Length - 1);

                        int newSelectedIndex = EditorGUILayout.Popup("Select Material", selectedIndex, materialNames);

                        if (newSelectedIndex >= 0 && newSelectedIndex < materialNames.Length)
                        {
                            script.selectedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{folderPath}/{materialNames[newSelectedIndex]}.mat");

                            // Update script.materials to contain only the selected material
                            if (script.selectedMaterial != null && !script.materials.Contains(script.selectedMaterial))
                            {
                                script.materials = new Material[] { script.selectedMaterial };
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No valid child found under AddModelHere.");
                    }
                }
                else
                {
                    Debug.LogWarning("AddModelHere GameObject not found.");
                }
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(script);
        }
    }

    private void AutofillMaterials(ChangeMaterial1 script)
    {
        if (script.targetRenderer != null)
        {
            Material[] rendererMaterials = script.targetRenderer.sharedMaterials;
            if (rendererMaterials != null)
            {
                script.materials = rendererMaterials.ToArray();
                Debug.Log($"Autofilled {rendererMaterials.Length} materials from target renderer.");

                if (script.selectedMaterial == null && script.materials.Length > 0)
                {
                    script.selectedMaterial = script.materials[0];
                }

                script.materialsAutofilled = true;
            }
            else
            {
                script.materials = new Material[0];
                script.selectedMaterial = null;
                Debug.LogWarning("Target renderer has no materials.");
            }
        }
        else
        {
            script.materials = new Material[0];
            script.selectedMaterial = null;
            Debug.LogWarning("Target Renderer is null.");
        }
    }



    private void ToggleFoldout(ChangeMaterial1 script)
    {
        if (scriptFoldouts.TryGetValue(script, out bool isFoldout))
        {
            scriptFoldouts[script] = !isFoldout; // Toggle the foldout state
        }
        else
        {
            scriptFoldouts[script] = true; // Initialize foldout state if not present
        }
    }


    private void DuplicateDesign(ChangeMaterial1 original)
    {
        // Create a new instance of ChangeMaterial1 script
        ChangeMaterial1 newDesign = dragOptionsGameObject.AddComponent<ChangeMaterial1>();
        newDesign.targetRenderer = original.targetRenderer;
        newDesign.materials = original.materials != null ? original.materials.ToArray() : new Material[0]; // Copy materials array

        // Add to list and dictionary
        changeMaterialScripts.Add(newDesign);
        scriptFoldouts[newDesign] = false;
    }

    private void RenameFoldout(ChangeMaterial1 design, string newName)
    {
        if (scriptFoldouts.ContainsKey(design))
        {
            scriptFoldouts[design] = true;
        }
    }

    private void LoadPreviewImage()
    {
        byte[] fileData = System.IO.File.ReadAllBytes(mediaFilePath);
        previewTexture = new Texture2D(2, 2);
        previewTexture.LoadImage(fileData);
    }
    

    private void LoadAnimations()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets" });
        animationsList.Clear();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);

            if (clip != null)
            {
                animationsList.Add(clip);
            }
        }
    }

    private List<GameObject> GetChildGameObjects(GameObject parent)
    {
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            childObjects.Add(child.gameObject);
        }
        return childObjects;
    }
    private void PopulateGameObjectsList()
    {
        gameObjectsList.Clear();

        // Find the parent GameObject named "AddModelHere"
        GameObject parentObject = GameObject.Find("AddModelHere");

        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                // Exclude the Media GameObject
                if (child.gameObject.name != "Media")
                {
                    gameObjectsList.Add(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogWarning("Parent object 'AddModelHere' not found.");
        }

    }
    private bool CanPreviewAnimation(AnimationClip animation)
    {
        foreach (var model in gameObjectsList)
        {
            if (model != null)
            {
                Animator animator = model.GetComponent<Animator>();
                if (animator != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ImportAnimationToModel(AnimationClip animation, GameObject model)
    {
        if (model != null)
        {
            // Ensure the model has an Animator component
            Animator animator = model.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the model: " + model.name);
                return;
            }

            // Load the default AnimatorController from the specified path
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);
            if (controller == null)
            {
                Debug.LogError("Default AnimatorController not found at path: " + animatorPath);
                return;
            }

            // Ensure the model's Animator uses the loaded AnimatorController
            animator.runtimeAnimatorController = controller;

            // Access the base layer of the AnimatorController
            AnimatorControllerLayer layer = controller.layers[0];
            AnimatorStateMachine stateMachine = layer.stateMachine;

            // Find the "Other" state
            AnimatorState otherState = stateMachine.states.FirstOrDefault(state => state.state.name == "Other").state;
            if (otherState == null)
            {
                Debug.LogError("'Other' state not found in the default AnimatorController.");
                return;
            }

            // Override the motion in the "Other" state with the new animation clip
            otherState.motion = animation;

            Debug.Log("Imported Animation: " + animation.name + " to the 'Other' state in " + model.name + " using the default AnimatorController.");
        }
    }

    private void DeleteHistoryEntry(int index)
    {
        AnimationImportHistoryEntry entry = importHistory[index];

        // Access the Animator component on the model
        Animator animator = entry.model.GetComponent<Animator>();
        if (animator != null)
        {
            // Get the AnimatorController
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                // Access the base layer of the AnimatorController
                AnimatorControllerLayer layer = controller.layers[0];
                AnimatorStateMachine stateMachine = layer.stateMachine;

                // Find the "Other" state
                AnimatorState otherState = stateMachine.states.FirstOrDefault(state => state.state.name == "Other").state;
                if (otherState != null && otherState.motion == entry.clip)
                {
                    // Remove the animation clip by setting the motion to null
                    otherState.motion = null;
                    Debug.Log("Removed animation clip from the 'Other' state in " + entry.model.name);
                }
                else
                {
                    Debug.LogWarning("Animation clip not found in the 'Other' state for " + entry.model.name);
                }
            }
        }

        // Remove the entry from the history
        importHistory.RemoveAt(index);
        Debug.Log("Deleted Entry " + (index + 1));
    }


    private void ImportMediaFile()
    {
        ImportFile1(mediaFilePath, "Media", "AddMediaHere", new string[] { ".mp4", ".jpg", ".png", ".mov", ".jpeg" });
        Debug.Log(mediaFilePath);
        mediaFilePath = string.Empty; // Clear the file path
    }

    private void ImportImageFile()
    {
        ImportFile2(mediaFilePath, "Image", "AddMediaHere", new string[] { ".jpg", ".png", ".jpeg" });
        Debug.Log(mediaFilePath);
        mediaFilePath = string.Empty; // Clear the file path
    }

    public class ImportedFileInfo
    {
        public string FilePath { get; set; }
        public UnityEngine.Object Asset { get; set; }
        public string ParentObjectName { get; set; }
    }

    private void ImportFile1(string modelFilePath, string fileType, string parentObjectName, string[] validExtensions)
    {
        string fileName = Path.GetFileName(modelFilePath);
        string destPath = $"Assets/{fileName}";

        // Check if the file already exists in the project
        if (File.Exists(destPath))
        {
            // Inform the user that the file already exists
            if (EditorUtility.DisplayDialog("File Exists", $"The {fileType} file already exists in the project.", "Ping", "Cancel"))
            {
                // Ping the existing asset
                UnityEngine.Object existingAsset = AssetDatabase.LoadMainAssetAtPath(destPath);
                EditorGUIUtility.PingObject(existingAsset);
            }
            return;
        }

        // Check if the file extension is valid
        string extension = Path.GetExtension(modelFilePath).ToLower();
        if (Array.Exists(validExtensions, ext => ext == extension))
        {
            File.Copy(modelFilePath, destPath);
            AssetDatabase.Refresh();
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(destPath, typeof(UnityEngine.Object));

            if (asset != null)
            {
                // Handle different file types
                GameObject tvObject = FindGameObject("AddModelHere/Media/TV");
                if (tvObject != null)
                {
                    // Display dialog to confirm import
                    if (EditorUtility.DisplayDialog("Confirm Import?", $"Do you want to add this {fileType} file to the Media?", "Yes", "Cancel"))
                    {
                        // Import the latest file
                        ImportedFileInfo latestImportedFile = new ImportedFileInfo
                        {
                            FilePath = destPath,
                            Asset = asset,
                            ParentObjectName = parentObjectName
                        };
                        importedFiles.Add(latestImportedFile);

                        if (latestImportedFile.Asset is VideoClip videoClip)
                        {
                            DisplayVideoOnTV(tvObject, videoClip, fileName);
                        }
                        else if (latestImportedFile.Asset is Texture2D texture)
                        {
                            DisplayImageOnTV(tvObject, texture, fileName);
                        }
                        else
                        {
                            Debug.LogWarning($"Unsupported asset type: {latestImportedFile.Asset.GetType()}. Only VideoClip and Texture2D are supported.");
                        }
                    }
                    else
                    {
                        // If the import is canceled, remove the imported file from the asset folder
                        FileUtil.DeleteFileOrDirectory(destPath);
                        AssetDatabase.Refresh();
                        Debug.Log("File will not be added to the asset folder.");
                    }
                }
                else
                {
                    Debug.LogWarning("TV GameObject not found.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to load the imported {fileType} asset.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid {fileType} file format. Please select a {fileType} file with a valid extension ({string.Join(", ", validExtensions)}).");
        }
    }

    private void ImportFile2(string modelFilePath, string fileType, string parentObjectName, string[] validExtensions)
    {
        string fileName = Path.GetFileName(modelFilePath);
        string destPath = "Assets/" + fileName;

        // Check if the file already exists in the project
        if (File.Exists(destPath))
        {
            // Inform the user that the file already exists
            if (EditorUtility.DisplayDialog("File Exists", $"The {fileType} file already exists in the project.", "Ping", "Cancel"))
            {
                // Ping the existing asset
                UnityEngine.Object existingAsset = AssetDatabase.LoadMainAssetAtPath(destPath);
                EditorGUIUtility.PingObject(existingAsset);
            }
            return;
        }

        // Check if the file extension is valid
        string extension = Path.GetExtension(modelFilePath).ToLower();
        if (Array.Exists(validExtensions, ext => ext == extension))
        {
            File.Copy(modelFilePath, destPath);
            AssetDatabase.Refresh();
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(destPath, typeof(UnityEngine.Object));
            if (asset != null)
            {
                importedFiles.Add(new ImportedFileInfo
                {
                    FilePath = destPath,
                    Asset = asset,
                    ParentObjectName = parentObjectName
                });

                // Handle different file types
                GameObject imageObject = FindGameObject("UI/Functions/FourthFunction/Image/Placeholder");
                if (imageObject != null)
                {
                    // Display dialog to confirm import for the latest file only
                    if (EditorUtility.DisplayDialog("Confirm Import?", $"Do you want to add this image?", "Yes", "Cancel"))
                    {
                        // Import the latest file
                        ImportedFileInfo latestImportedFile = importedFiles[importedFiles.Count - 1];
                        if (latestImportedFile.Asset is Texture2D texture)
                        {
                            DisplayDesignImage(imageObject, texture, fileName);
                        }
                        else
                        {
                            Debug.LogWarning($"Unsupported asset type: {latestImportedFile.Asset.GetType()}. Only Texture2D is supported.");
                        }
                    }
                    else
                    {
                        // If the import is canceled, remove the imported file from the asset folder
                        AssetDatabase.Refresh();
                        Debug.Log("Image will not be added to the asset folder.");
                        FileUtil.DeleteFileOrDirectory(destPath);
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    Debug.LogWarning($"Image GameObject not found.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to load the imported {fileType} asset.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid {fileType} file format. Please select a {fileType} file with a valid extension ({string.Join(", ", validExtensions)}).");
        }
    }

    // Function to display video on TV
    private void DisplayVideoOnTV(GameObject tvObject, VideoClip videoClip, string fileName)
    {
        // Create a new GameObject for the video
        GameObject videoObject = new GameObject(fileName);
        videoObject.transform.parent = tvObject.transform;
        videoObject.transform.localPosition = Vector3.zero; // Set local position if needed

        // Add a VideoPlayer component to the new GameObject
        VideoPlayer videoPlayer = videoObject.AddComponent<VideoPlayer>();

        // Find the CustomRenderTexture asset
        CustomRenderTexture customRenderTexture = FindCustomRenderTexture("VideoPlayer");
        if (customRenderTexture == null)
        {
            Debug.LogWarning("Custom RenderTexture 'VideoPlayer' not found.");
            return;
        }

        // Set the RenderTexture for the VideoPlayer
        videoPlayer.targetTexture = customRenderTexture;

        // Set the VideoClip to the VideoPlayer
        videoPlayer.clip = videoClip;

        Debug.Log($"Video '{fileName}' displayed on new GameObject with VideoPlayer component.");
    }


    private CustomRenderTexture FindCustomRenderTexture(string textureName)
    {
        string[] assetPaths = AssetDatabase.FindAssets($"{textureName} t:CustomRenderTexture");
        if (assetPaths.Length == 0)
        {
            Debug.LogWarning($"No CustomRenderTexture found with the name '{textureName}'.");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(assetPaths[0]);
        CustomRenderTexture customRenderTexture = AssetDatabase.LoadAssetAtPath<CustomRenderTexture>(path);
        if (customRenderTexture == null)
        {
            Debug.LogWarning($"Failed to load CustomRenderTexture at path: {path}");
        }
        return customRenderTexture;
    }


    private void DisplayImageOnTV(GameObject tvObject, Texture2D texture, string fileName)
    {
        // Create a new GameObject for the image
        GameObject imageObject = new GameObject(fileName);
        imageObject.transform.parent = tvObject.transform;
        imageObject.transform.localPosition = Vector3.zero; // Set local position if needed

        // Create a MeshRenderer with a Quad mesh
        MeshFilter meshFilter = imageObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = imageObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
        new Vector3(-1, -1, 0),
        new Vector3(1, -1, 0),
        new Vector3(1, 1, 0),
        new Vector3(-1, 1, 0)
        };
        mesh.uv = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };
        mesh.triangles = new int[]
        {
        0, 1, 2,
        2, 3, 0
        };

        meshFilter.mesh = mesh;

        // Assign material with the texture
        Material material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = texture;
        meshRenderer.material = material;

        Debug.Log($"Image file '{fileName}' imported and displayed on TV.");
    }



    // Function to display design image
    private void DisplayDesignImage(GameObject imageObject, Texture2D texture, string fileName)
    {
        // Create a new GameObject for the image
        GameObject designObject = new GameObject(fileName);
        designObject.transform.SetParent(imageObject.transform);

        // Ensure the Image component exists
        UnityEngine.UI.Image uiImage = designObject.AddComponent<UnityEngine.UI.Image>();

        // Create a new material instance based on the placeholder's material
        Material material = new Material(imageObject.GetComponent<UnityEngine.UI.Image>().material);

        // Set the main texture of the material to the provided image texture
        material.mainTexture = texture;

        // Assign the material to the Image component
        uiImage.material = material;

        // Get the RectTransform of the imageObject and designObject
        RectTransform imageRectTransform = imageObject.GetComponent<RectTransform>();
        RectTransform designRectTransform = designObject.GetComponent<RectTransform>();

        // Set the designObject to fill the entire space of the imageObject
        designRectTransform.anchorMin = new Vector2(0, 0);
        designRectTransform.anchorMax = new Vector2(1, 1);
        designRectTransform.offsetMin = new Vector2(0, 0);
        designRectTransform.offsetMax = new Vector2(0, 0);

        // Match pivot and other properties 
        designRectTransform.pivot = imageRectTransform.pivot;
        designRectTransform.localScale = Vector3.one;  // Reset scale to default
        designRectTransform.localRotation = Quaternion.identity;  // Reset rotation to default

        Debug.Log($"Image file '{fileName}' imported and displayed on the placeholder.");
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

    private void ShowImportedFiles()
    {
        GameObject tvGameObject = FindGameObject("AddModelHere/Media/TV");
        if (tvGameObject == null)
        {
            Debug.LogWarning("TV GameObject not found.");
            return;
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        // Begin the scroll view
        scrollPosition3 = EditorGUILayout.BeginScrollView(scrollPosition3, GUILayout.Height(100)); // Adjust height as needed

        foreach (Transform child in tvGameObject.transform)
        {
            if (child.name != "Empty")
            {
                GameObject tvObject = child.gameObject;
                GUILayout.BeginHorizontal();
                GUILayout.Label(ShortenFileName(tvObject.name), EditorStyles.boldLabel);

                // Check if the GameObject has a VideoPlayer component
                VideoPlayer videoPlayer = tvObject.GetComponent<VideoPlayer>();
                if (tvGameObject.transform.childCount == 0)
                {
                    GUILayout.Label("No Media was Added");
                }
                if (videoPlayer != null)
                {
                    GUILayout.Label("Type: Video");
                    // Add button to delete the video
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        DestroyImmediate(tvObject);
                    }
                }
                else
                {
                    // Check if the GameObject has a Renderer component
                    Renderer renderer = tvObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        GUILayout.Label("Type: Photo");
                        // Add button to delete GameObject
                        if (GUILayout.Button("Delete", GUILayout.Width(50)))
                        {
                            DestroyImmediate(tvObject);
                        }
                    }
                    else
                    {
                        // If it doesn't have VideoPlayer or Renderer, display as Unknown
                        GUILayout.Label("Type: Unknown");
                    }
                }
                GUILayout.EndHorizontal();
            }
           
        }

        // End the scroll view
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    private void ShowImportedImages()
    {
        GameObject imageGameObject = GameObject.Find("UI/Functions/FourthFunction/Image/Placeholder");
        if (imageGameObject == null)
        {
            Debug.LogWarning("Image GameObject not found.");
            return;
        }
        GUILayout.BeginVertical(EditorStyles.helpBox);
        // Begin the scroll view
        scrollPosition5 = EditorGUILayout.BeginScrollView(scrollPosition5, GUILayout.Height(300)); // Adjust height as needed


        foreach (Transform child in imageGameObject.transform)
        {
            GUILayout.BeginHorizontal();
            GameObject imageObject = child.gameObject;

            string imagename = imageObject.name;
            GUILayout.Label(imagename, EditorStyles.boldLabel);

            // Check if the GameObject has an Image component
            UnityEngine.UI.Image uiImage = imageObject.GetComponent<UnityEngine.UI.Image>();
            if (uiImage != null)
            {
                GUILayout.Label("Type: Photo");
                // Display the image in the editor
                Texture2D texture = null;
                if (uiImage.material != null && uiImage.material.mainTexture != null)
                {
                    texture = uiImage.material.mainTexture as Texture2D;
                }

                if (texture != null)
                {
                    // Display the texture in the editor
                    GUILayout.Label(texture, GUILayout.Width(150), GUILayout.Height(150));
                }
                else
                {
                    Debug.LogWarning("Texture is null for image: " + imageObject.name);
                }
                // Add button to delete GameObject
                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    DestroyImmediate(imageObject);
                }
            }
            else
            {
                // If it doesn't have a Renderer, display as Unknown
                GUILayout.Label("Type: Unknown");
            }

            GUILayout.EndHorizontal();
        }


        // End the scroll view
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }


    private void Show3DModels()
    {
        GameObject addModelHere = GameObject.Find("AddModelHere");
        if (addModelHere == null)
        {
            Debug.LogWarning("AddModelHere GameObject not found.");
            return;
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        // Begin the scroll view
        scrollPosition4 = EditorGUILayout.BeginScrollView(scrollPosition4, GUILayout.Height(100)); // Adjust height as needed

        foreach (Transform child in addModelHere.transform)
        {
            if (child.name != "Media")
            {
                GameObject modelObject = child.gameObject;
                GUILayout.BeginHorizontal();
                GUILayout.Label(ShortenFileName(modelObject.name), EditorStyles.boldLabel);

                // Check if the GameObject has a MeshFilter component
                MeshFilter meshFilter = modelObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                    string fileType = GetFileType(assetPath);
                    GUILayout.Label($"Type: 3D Model ({fileType})");

                    // Add button to delete the 3D model
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        DestroyImmediate(modelObject);
                    }
                }
                else
                {
                    GUILayout.Label("Type: 3D Model");
                    // Add button to delete the 3D model
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        DestroyImmediate(modelObject);
                    }
                }

                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private string GetFileType(string assetPath)
    {
        if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
        {
            return "FBX";
        }
        else if (assetPath.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase))
        {
            return "OBJ";
        }
        else
        {
            return "Unknown";
        }
    }

    private void ImportFBXModel()
    {
        if (string.IsNullOrEmpty(modelFilePath) || !File.Exists(modelFilePath))
        {
            Debug.LogWarning("Please select a valid file to import.");
            return;
        }

        FBXImporter.ImportAndSetupFBX(modelFilePath); // Pass the file path to the importer
    }
    /*
    private void ImportModelFolder()
    {
        if (string.IsNullOrEmpty(modelFolderPath) || !Directory.Exists(modelFolderPath))
        {
            Debug.LogWarning("Please select a valid folder to import.");
            return;
        }

        string[] fbxFiles = Directory.GetFiles(modelFolderPath, "*.fbx", SearchOption.TopDirectoryOnly);
        foreach (string fbxFile in fbxFiles)
        {
            string destPath = "Assets/" + Path.GetFileName(fbxFile);
            UnityEngine.Object existingAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(destPath);

            if (File.Exists(destPath))
            {
                if (!pinged)
                {
                    if (EditorUtility.DisplayDialog("File Exists", "The file already exists in the project.", "OK"))
                    {
                        EditorGUIUtility.PingObject(existingAsset);
                        pinged = true;
                    }
                }

                if (pinged)
                {
                    if (IsFileAlreadyAdded(destPath))
                    {
                        PingExistingFile(destPath);
                        EditorUtility.DisplayDialog("File Already Added", "The file is already added to the Scene.", "OK");
                        pinged = false;
                        continue;
                    }

                    if (EditorUtility.DisplayDialog("Add Existing File", "Do you want to add the existing file to the Scene?", "Yes", "No"))
                    {
                        AddFileToGameObject(destPath);
                        Debug.Log("File Added");
                    }
                    else if (ConfirmDeletion("Do you want to delete the existing file? Note: If this file is on Scene, the model and all its Attributes will also be removed! This Action cannot be undone."))
                    {
                        DeleteFileAndDependencies(destPath);
                        AddFileToGameObject(destPath);
                        Debug.Log("File Added");
                    }
                    continue;
                }
            }

            if (EditorUtility.DisplayDialog("Add File to GameObject", "Do you want to add the file to the Scene?", "Yes", "No"))
            {
                if (fbxFile.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (EditorUtility.DisplayDialog("Remove Previous Models?", "Do you want to Disable all Previous Models and Remove their Attributes? Take note, once you approve, this action cannot be undone.", "Yes", "No"))
                    {
                        RemovePreviousModelsAndData();
                    }

                    CopyAndAddFileToGameObject(fbxFile, destPath);
                    ImportTexturesForModel(destPath);
                }
                else
                {
                    Debug.LogWarning($"Unsupported file format: {Path.GetExtension(fbxFile)}");
                }
            }
            else
            {
                CleanupUnintendedFile(destPath);
            }
        }
    }

    private void ImportTexturesForModel(string modelPath)
    {
        if (string.IsNullOrEmpty(texturesFolderPath) || !Directory.Exists(texturesFolderPath))
        {
            Debug.LogWarning($"Textures folder not found for model folder: {modelFolderPath}");
            return;
        }

        string[] textureFiles = Directory.GetFiles(texturesFolderPath, "*.*", SearchOption.AllDirectories);
        foreach (string textureFile in textureFiles)
        {
            string destPath = "Assets/Textures/" + Path.GetFileName(textureFile);
            FileUtil.CopyFileOrDirectory(textureFile, destPath);
            AssetDatabase.Refresh();
            Debug.Log($"Texture added: {destPath}");
        }
    }
    */
    private void PingExistingFile(string destPath)
    {
        GameObject existingFile = GetExistingFileGameObject(destPath);
        if (existingFile != null)
        {
            EditorGUIUtility.PingObject(existingFile);
        }
        else
        {
            Debug.LogWarning("Failed to ping the existing file. GameObject not found.");
        }
    }

    private bool ConfirmDeletion(string message)
    {
        return EditorUtility.DisplayDialog("Delete File", message, "Yes", "No");
    }

    private void DeleteFileAndDependencies(string destPath)
    {
        RemoveAllClonesInCanvas("Text (TMP)");
        reset = true;
        messageShown = false;
        RemoveChildModels();
        ClearSerializedData();
        ClearScriptsFromDragOptionsHere();
        RemoveChildGameObjectsFromTV();
        UpdateTitleAndDescription();
        FileUtil.DeleteFileOrDirectory(destPath);
        AssetDatabase.Refresh();
        Debug.Log("File Deleted");
        pinged = false;
    }

    private void RemovePreviousModelsAndData()
    {
        reset = true;
        messageShown = false;
        RemoveChildModels();
        ClearSerializedData();
        ClearScriptsFromDragOptionsHere();
        RemoveChildGameObjectsFromTV();
        UpdateTitleAndDescription();
    }

    private void CopyAndAddFileToGameObject(string modelFilePath, string destPath)
    {
        FileUtil.CopyFileOrDirectory(modelFilePath, destPath);
        AssetDatabase.Refresh();
        Debug.Log("File added to the asset folder.");
        AddFileToGameObject(destPath);
        Debug.Log("Remove Completed");
        ResetUI();
    }

    private void CopyFileToAssets(string modelFilePath, string destPath)
    {
        FileUtil.CopyFileOrDirectory(modelFilePath, destPath);
        AssetDatabase.Refresh();
        Debug.Log("File added to the asset folder.");
    }

    private void CleanupUnintendedFile(string destPath)
    {
        AssetDatabase.Refresh();
        Debug.Log("File will not be added to the asset folder.");
        FileUtil.DeleteFileOrDirectory(destPath);
        AssetDatabase.Refresh();
    }

    private void RemoveChildGameObjectsFromTV()
    {
        GameObject tvObject = FindGameObject("AddModelHere/Media/TV");
        Debug.Log("TV Active");
        if (tvObject != null)
        {
            tvObject.SetActive(true);
            int initialChildCount = tvObject.transform.childCount;
            bool wasActive = tvObject.activeSelf;

            for (int i = tvObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = tvObject.transform.GetChild(i);
                if (child.gameObject != tvObject && tvObject.transform.childCount != 0)
                {
                    DestroyImmediate(child.gameObject);
                    string assetPath = AssetDatabase.GetAssetPath(child.gameObject);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
                if (tvObject.transform.childCount == 0)
                {
                    tvObject.SetActive(false);
                }
            }

            if (wasActive)
            {
                tvObject.SetActive(false);
                Debug.Log("Delete Media Complete, TV deactivated Commenced");
            }

            int finalChildCount = tvObject.transform.childCount;
            Debug.Log("Initial child count: " + initialChildCount);
            Debug.Log("Final child count: " + finalChildCount);
        }
        else
        {
            Debug.LogWarning("TV GameObject not found.");
            FindInActiveObjectByTag("TV");
        }
    }

    GameObject FindInActiveObjectByTag(string tag)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].CompareTag(tag))
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    private string ShortenFileName(string filePath, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return string.Empty; 
        }

        if (filePath.Length <= maxLength)
        {
            return filePath;
        }

        string fileName = Path.GetFileName(filePath);
        string directory = Path.GetDirectoryName(filePath);

        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(directory))
        {
            return filePath;
        }

        if (fileName.Length > maxLength)
        {
            return "..." + fileName.Substring(fileName.Length - maxLength + 3);
        }

        int directoryMaxLength = maxLength - fileName.Length - 3;
        if (directory.Length > directoryMaxLength)
        {
            return directory.Substring(0, directoryMaxLength) + "..." + fileName;
        }

        return directory + Path.DirectorySeparatorChar + fileName;
    }

    private void AddFileToGameObject(string destPath)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(destPath);
        if (model != null)
        {
            GameObject instantiatedModel = Instantiate(model);
            instantiatedModel.name = model.name;
            instantiatedModel.transform.SetParent(GameObject.Find("AddModelHere").transform, false);
            EditorGUIUtility.PingObject(instantiatedModel);
        }
        else
        {
            Debug.LogError("Failed to load the model from the specified path.");
        }
    }

    // Function to check if the file is already added to the GameObject AddModelHere
    private bool IsFileAlreadyAdded(string filePath)
    {
        GameObject addModelHere = GameObject.Find("AddModelHere");
        if (addModelHere != null)
        {
            Transform[] children = addModelHere.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == Path.GetFileNameWithoutExtension(filePath))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Function to get the GameObject of the existing file
    private GameObject GetExistingFileGameObject(string filePath)
    {
        GameObject addModelHere = GameObject.Find("AddModelHere");
        if (addModelHere != null)
        {
            return addModelHere.transform.Find(Path.GetFileNameWithoutExtension(filePath))?.gameObject;
        }
        return null;
    }
    public void LoadMaterials()
    {
        materialsInFolder.Clear();

        if (Directory.Exists(folderPath))
        {
            // Find all material assets
            var materialGuids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
            foreach (var guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith(folderPath))
                {
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material != null)
                    {
                        materialsInFolder.Add(material);
                    }
                }
            }

            if (materialsInFolder.Count == 0)
            {
                Debug.LogWarning($"No materials found in the folder {folderPath}.");
            }
        }
        else
        {
            Debug.LogWarning($"The folder path {folderPath} does not exist.");
        }
    }
    private Renderer FindRendererByInstanceID(int instanceID)
    {
        // Use FindObjectsOfType<Renderer>() to get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.GetInstanceID() == instanceID)
            {
                return renderer;
            }
        }
        // Log if the renderer is not found
        Debug.LogWarning($"Renderer with InstanceID {instanceID} not found.");
        return null;
    }

    private void InitializeEditorWithData()
    {
        if (annotations.Count > 0)
        {
            // Iterate over all annotations to find and initialize the targetRenderer
            foreach (var annotation in annotations)
            {
                if (!string.IsNullOrEmpty(annotation.rendererPath))
                {
                    GameObject rendererObj = GameObject.Find(annotation.rendererPath);
                    if (rendererObj != null)
                    {
                        Renderer renderer = rendererObj.GetComponent<Renderer>();
                        annotation.targetRenderer = renderer; // Set the renderer in annotation data
                    }
                    else
                    {
                        Debug.LogWarning($"Renderer at path {annotation.rendererPath} could not be found during initialization.");
                    }
                }
                else
                {
                    Debug.LogWarning("Renderer path is null or empty for annotation.");
                }
            }

            // Optionally set the first annotation as selected
            selectedAnnotation = annotations.FirstOrDefault();
            if (selectedAnnotation != null)
            {
                annotationText = selectedAnnotation.ownText;
                offset = selectedAnnotation.offset;
                rotation = selectedAnnotation.rotation;
                lineMaterial = selectedAnnotation.newMat;
                targetRenderer = selectedAnnotation.targetRenderer;
            }
        }
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
    public void SaveAnnotations(AnnotationDatasLists annotationDatasLists)
    {
        SetAnnotationIndices(annotationDatasLists.annotations);
        foreach (var annotation in annotationDatasLists.annotations)
        {
            // Convert the renderer's transform hierarchy to a path
            if (annotation.targetRenderer != null)
            {
                annotation.rendererPath = GetGameObjectPath(annotation.targetRenderer.gameObject);
            }
        }
        Debug.Log("Saving annotations:\n" + JsonUtility.ToJson(annotationDatasLists, true));
        string json = JsonUtility.ToJson(annotationDatasLists, true);
        File.WriteAllText(filepaths, json);
        Debug.Log("Annotations saved to " + filepaths);
    }
    public void SetAnnotationIndices(List<AnnotationDatas> annotations)
    {
        for (int i = 0; i < annotations.Count; i++)
        {
            annotations[i].index = i; // Set the index based on position in the list
        }
    }
    public void InitializeAnnotationDatas()
    {
        annotationDatasLists = new AnnotationDatasLists();
        // Optionally set up any default values or configurations if needed
    }

    public AnnotationDatasLists LoadAnnotations()
    {
        if (!File.Exists(filepaths))
        {
            Debug.LogWarning("Annotations file not found.");
            return new AnnotationDatasLists();
        }

        // Deserialize JSON
        string json = File.ReadAllText(filepaths);
        AnnotationDatasLists annotationDatasLists = JsonUtility.FromJson<AnnotationDatasLists>(json);

        // Clear the existing list before adding new annotations
        annotations.Clear();

        // Find the renderer using the stored path
        foreach (var annotation in annotationDatasLists.annotations)
        {
            if (!string.IsNullOrEmpty(annotation.rendererPath))
            {
                GameObject obj = GameObject.Find(annotation.rendererPath);
                if (obj != null)
                {
                    annotation.targetRenderer = obj.GetComponent<Renderer>();
                }
                else
                {
                    Debug.LogWarning("Renderer not found for path: " + annotation.rendererPath);
                }
            }

            // Check if offset is being loaded correctly
            Debug.Log($"Loaded Offset: {annotation.offset}");

            // Add to the main annotations list
            annotations.Add(annotation);
        }

        return annotationDatasLists;
    }

    private void LogAnnotationDataLists(AnnotationDatasLists dataLists)
    {
        Debug.Log("Loaded AnnotationDatasLists:");

        if (dataLists.annotations != null)
        {
            foreach (var annotation in dataLists.annotations)
            {
                Debug.Log($"Annotation ID: {annotation.uniqueID}");
                Debug.Log($"Text: {annotation.ownText}");
                Debug.Log($"Position: {annotation.position}");
                Debug.Log($"Rotation: {annotation.rotation}");
                Debug.Log($"Material: {annotation.newMat}");
                Debug.Log($"Renderer Instance ID: {annotation.rendererInstanceID}");

                // Optionally log more details depending on the complexity of AnnotationDatas
            }
        }
        else
        {
            Debug.Log("No annotations found in loaded data.");
        }
    }


    public List<Material> GetMaterialsFromFolder(string folderPath)
    {
        List<Material> materials = new List<Material>();

        // Load all assets at the specified path
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Material", new[] { folderPath });

        foreach (string guid in assetGUIDs)
        {
            // Convert GUID to asset path
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the material from the asset path
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (material != null)
            {
                materials.Add(material);
            }
        }

        return materials;
    }
    private void ShowEditWindow()
    {
        if (selectedAnnotation == null)
        {
            Debug.LogWarning("No annotation selected.");
            return;
        }

        AnnotationEditWindows editWindow = ScriptableObject.CreateInstance<AnnotationEditWindows>();

        // Initialize the edit window with the current data
        editWindow.Init(this, selectedAnnotation, lineMaterial, textPrefab, targetCanvas);

        editWindow.ShowUtility();
    }
    public void UpdateAnnotation(AnnotationDatas annotation, string newText, Vector3 newPosition, Vector3 newRotation, Material newMat)
    {
        Debug.Log($"Updating annotation with ID: {annotation.uniqueID}");

        // Ensure the annotation text is valid
        if (annotation.text == null)
        {
            Debug.LogWarning("Annotation text is null. Attempting to find or create text.");
            FindOrCreateAnnotationText(annotation);
        }

        if (annotation.text == null)
        {
            Debug.LogWarning("Annotation text object could not be found after trying to create it.");
            return;
        }

        Debug.Log($"Annotation text object found: {annotation.text.name}");


        // Remove the original LinePreview if it exists
        Transform existingLinePreview = annotation.text.transform.Find("LinePreview");
        if (existingLinePreview != null)
        {
            DestroyImmediate(existingLinePreview.gameObject);
        }

        Bounds bounds;

        if (annotation.targetRenderer is MeshRenderer meshRenderer)
        {
            bounds = meshRenderer.bounds;
        }
        else if (annotation.targetRenderer is SkinnedMeshRenderer skinnedMeshRenderer)
        {
            bounds = skinnedMeshRenderer.bounds;
        }
        else
        {
            // Calculate bounds manually if renderer type is unknown
            bounds = CalculateBounds(annotation.targetRenderer);
            if (bounds.size == Vector3.zero)
            {
                Debug.LogWarning("Failed to calculate bounds for unknown renderer type.");
                return;
            }
        }
        Debug.Log($"Updating annotation: {annotation.uniqueID}");
        // Update annotation text, position, and rotation
        Vector3 annotationPosition = bounds.center + newPosition;
        annotation.ownText = newText;
        annotation.text.text = newText;
        annotation.offset = newPosition;
        annotation.rotation = newRotation;
        annotation.newMat = newMat;

        RectTransform rectTransform = annotation.text.rectTransform;
        if (rectTransform != null)
        {
            rectTransform.position = annotationPosition;
            rectTransform.rotation = Quaternion.Euler(newRotation);
        }
        else
        {
            Debug.LogWarning("RectTransform component not found on annotation.text.");
        }

        // Handle the LineRenderer
        LineRenderer lineRenderer = annotation.text.transform.Find("Line")?.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.material = annotation.newMat; // Use the material from AnnotationDatas
            lineRenderer.SetPosition(1, annotationPosition);
        }
        else
        {
            // Create a new LineRenderer if it does not exist
            GameObject lineObj = new GameObject("LinePreview");
            lineObj.transform.SetParent(annotation.text.transform, false);
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = annotation.newMat ?? lineMaterial; // Use the stored material or default
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.SetPosition(0, bounds.center);
            lineRenderer.SetPosition(1, annotationPosition);
        }
        SaveAnnotations(new AnnotationDatasLists { annotations = annotations });
    }

    private Bounds CalculateBounds(Renderer renderer)
    {
        // Ensure the renderer has a valid mesh and is active in the scene
        if (renderer == null || !renderer.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Renderer is null or inactive. Cannot calculate bounds.");
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        // Create a new Bounds instance to calculate
        Bounds bounds = new Bounds(renderer.transform.position, Vector3.zero);

        // Use the MeshFilter if available
        MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            bounds = meshFilter.sharedMesh.bounds;
            bounds.center = renderer.transform.TransformPoint(bounds.center);
            bounds.size = Vector3.Scale(bounds.size, renderer.transform.lossyScale);
        }
        else
        {
            // If no MeshFilter, assume default bounds
            bounds = new Bounds(renderer.transform.position, Vector3.one);
        }

        return bounds;
    }
    private TextMeshProUGUI FindOrCreateAnnotationText(AnnotationDatas annotation)
    {
        if (annotation == null)
        {
            Debug.LogWarning("Annotation data is null.");
            return null;
        }

        // Find the "ModelCanvas" GameObject
        GameObject modelCanvas = GameObject.Find("ModelCanvas");
        if (modelCanvas == null)
        {
            Debug.LogWarning("ModelCanvas GameObject not found.");
            return null;
        }

        // Find all TextMeshProUGUI components under "ModelCanvas"
        TextMeshProUGUI[] texts = modelCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);

        // Filter TextMeshProUGUI components that end with "(Clone)"
        var cloneTexts = texts.Where(t => t.gameObject.name.EndsWith("(Clone)")).ToList();

        // Ensure there are enough clones
        if (annotation.index < 0 || annotation.index >= cloneTexts.Count)
        {
            Debug.LogWarning($"No matching TextMeshProUGUI found for index {annotation.index}. Available clones: {cloneTexts.Count}.");
            return null;
        }

        // Get the correct TextMeshProUGUI component based on annotation index
        TextMeshProUGUI selectedText = cloneTexts[annotation.index];
        annotation.text = selectedText;

        Debug.Log($"Annotation text assigned: {selectedText.gameObject.name}");
        return selectedText;
    }



    /*
    private void AddAnnotation(AnnotationDatas annotationData, Vector3 offset, Vector3 rotation, Canvas canvas)
    {
        EnsureLineMaterial(); // Ensure the material is set before using it

        Debug.Log("Adding annotation for: " + targetRenderer.gameObject.name);
        Bounds bounds;

        if (targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)targetRenderer).bounds;
        }
        else if (targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot add annotation.");
            return;
        }

        Vector3 annotationPosition = bounds.center + offset;

        if (textPrefab != null)
        {
            GameObject textInstance = Instantiate(textPrefab, canvas.transform); // Clone the prefab
            TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                annotationData.text = textComponent;
                annotationData.text.rectTransform.position = annotationPosition;
                annotationData.text.rectTransform.rotation = Quaternion.Euler(rotation);
                annotationData.text.text = annotationData.ownText;
                annotationData.SetTargetRenderer(targetRenderer); // Store the targetRenderer in annotationData
                Debug.Log("Annotation text set to: " + annotationData.ownText);               
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the instantiated prefab. Cannot add annotation.");
                DestroyImmediate(textInstance); // Clean up the instantiated object
                return;
            }

            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(annotationData.text.transform, false);
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.SetPosition(0, bounds.center);
            lineRenderer.SetPosition(1, annotationPosition);
            annotationData.newMat = lineMaterial;
            annotationData.position = annotationPosition;
            annotationData.rotation = rotation;
            DestroyPreview();
        }
        else
        {
            Debug.LogWarning("TextPrefab is null. Cannot add annotation.");
            return;
        }
    }
    */
    private void AddAnnotation(AnnotationDatas annotationData, Vector3 offset, Vector3 rotation, Canvas canvas)
    {
        EnsureLineMaterial(); // Ensure the material is set before using it

        Debug.Log("Adding annotation for: " + targetRenderer.gameObject.name);
        Bounds bounds;

        if (targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)targetRenderer).bounds;
        }
        else if (targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot add annotation.");
            return;
        }

        Vector3 annotationPosition = bounds.center + offset;

        if (textPrefab != null)
        {
            GameObject textInstance = Instantiate(textPrefab, canvas.transform); // Clone the prefab
            TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                annotationData.text = textComponent;
                annotationData.text.rectTransform.position = annotationPosition;
                annotationData.text.rectTransform.rotation = Quaternion.Euler(rotation);
                annotationData.text.text = annotationData.ownText;
                annotationData.SetTargetRenderer(targetRenderer); // Store the targetRenderer in annotationData
                Debug.Log("Annotation text set to: " + annotationData.ownText);

                // Attach the FaceCamera script to the textInstance to make it face the camera during runtime
                textInstance.AddComponent<FaceCamera>();
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the instantiated prefab. Cannot add annotation.");
                DestroyImmediate(textInstance); // Clean up the instantiated object
                return;
            }

            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(annotationData.text.transform, false);
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.SetPosition(0, bounds.center);
            lineRenderer.SetPosition(1, annotationPosition);
            annotationData.newMat = lineMaterial;
            annotationData.position = annotationPosition;
            annotationData.rotation = rotation;
            DestroyPreview();
        }
        else
        {
            Debug.LogWarning("TextPrefab is null. Cannot add annotation.");
            return;
        }
    }


    public void DrawAnnotationPreview(Vector3 position, Vector3 rotation, string text)
    {
        EnsureLineMaterial(); // Ensure the material is set before using it

        if (targetCanvas == null || targetCanvas.transform == null)
        {
            // Activate or initialize the targetCanvas if it's null
            ActivateOrInitializeCanvas();
        }

        if (targetCanvas != null && targetCanvas.transform != null)
        {
            if (previewText == null)
            {
                previewText = Instantiate(textPrefab, targetCanvas.transform);
                previewText.name = "PreviewText";

            }

            Bounds bounds;

            if (targetRenderer is MeshRenderer)
            {
                bounds = ((MeshRenderer)targetRenderer).bounds;
            }
            else if (targetRenderer is SkinnedMeshRenderer)
            {
                bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
            }
            else
            {
                return;
            }

            Vector3 annotationPosition = bounds.center + position;
            previewText.GetComponent<TextMeshProUGUI>().text = text;
            previewText.GetComponent<RectTransform>().position = annotationPosition;
            previewText.GetComponent<RectTransform>().rotation = Quaternion.Euler(rotation);
            
            if (previewLine == null)
            {
                GameObject lineObj = new GameObject("LinePreview");
                lineObj.transform.SetParent(textPrefab.transform, false);
                previewLine = lineObj.AddComponent<LineRenderer>();
                previewLine.material = lineMaterial;
                previewLine.startWidth = 0.02f; // Reduce line width
                previewLine.endWidth = 0.02f; // Reduce line width
            }
            
            previewLine.SetPosition(0, bounds.center);
            previewLine.SetPosition(1, annotationPosition);
        }
    }

    private void ActivateOrInitializeCanvas()
    {
        // Custom logic to activate or initialize the targetCanvas
        // This could be as simple as setting it active or instantiating it if needed

        if (targetCanvas == null)
        {
            Canvas canvas = GameObject.Find("ModelCanvas")?.GetComponent<Canvas>();
            targetCanvas = canvas;
            // Example initialization logic, adjust as necessary
            Debug.Log("Canvas Was Null, Fixing");
            targetCanvas.gameObject.SetActive(true);
        }
        else if (!targetCanvas.gameObject.activeSelf)
        {
            targetCanvas.gameObject.SetActive(true);
        }
    }


    public void ResetFields()
    {
        targetRenderer = null;
        lineMaterial = null;
        annotationText = "";
        offset = new Vector3(0, 1, 0);
        rotation = new Vector3(0, 90, 0); // Reset rotation to initial value     
    }
    /*
    public void DestroyPreview()
    {
        if (previewText != null)
        {
            DestroyImmediate(previewText.gameObject);
        }
        else
        {
            Debug.Log("Preview Text is not destroyed");
        }
        
        if (previewLine != null)
        {
            DestroyImmediate(previewLine.gameObject);

        }
        else
        {
            Debug.Log("Preview Line is not destroyed");
        }
    }*/
    public void DestroyPreview()
    {
        // Find the GameObject named "ModelCanvas"
        GameObject canvas = GameObject.Find("ModelCanvas");
        if (canvas != null)
        {
            // Loop through all children of "ModelCanvas"
            foreach (Transform child in canvas.transform)
            {
                // Check if the child's name is "PreviewText"
                if (child.name == "PreviewText")
                {
                    // Destroy the child
                    DestroyImmediate(child.gameObject);
                    Debug.Log("Preview Text has been destroyed.");
                }
            }
        }
        else
        {
            Debug.Log("Preview Text is not destroyed");
        }

        // Find all objects of type TextMeshProUGUI
        TextMeshProUGUI[] allTextObjects = GameObject.FindObjectsOfType<TextMeshProUGUI>();

        foreach (TextMeshProUGUI textObject in allTextObjects)
        {
            // Check if the object's name is "Text (TMP)(Clone)"
            if (textObject.name == "Text (TMP)(Clone)")
            {
                // Loop through all children of each "Text (TMP)(Clone)"
                foreach (Transform child in textObject.transform)
                {
                    // Check if the child's name is "LinePreview"
                    if (child.name == "LinePreview")
                    {
                        // Destroy the child
                        DestroyImmediate(child.gameObject);
                        Debug.Log("Preview Line has been destroyed.");
                    }
                }
            }
        }

        // Find the GameObject named "Text (TMP)"
        GameObject text = GameObject.Find("Text (TMP)");

        if (text != null)
        {
            // Loop through all children of "Text (TMP)"
            foreach (Transform child in text.transform)
            {
                // Check if the child's name is "LinePreview"
                if (child.name == "LinePreview")
                {
                    // Destroy the child
                    DestroyImmediate(child.gameObject);
                    Debug.Log("Main Preview Line has been destroyed.");
                }
            }
        }
        else
        {
            Debug.Log("Text (TMP) object not found.");
        }
    }


    private void EnsureLineMaterial()
    {
        if (lineMaterial == null)
        {
            // Find the material named "Default"
            string defaultMaterialPath = "Assets/Default.mat"; // Adjust the path as per your project structure
            lineMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultMaterialPath);

        }
    }

    private void RemoveAnnotation(AnnotationDatas annotation)
    {
        // Check if the annotation has a text object and destroy it
        if (annotation.text != null)
        {
            DestroyImmediate(annotation.text.gameObject);
        }

        // Check if the annotation has a line object and destroy it
        if (annotation.text != null)
        {
            Transform lineTransform = annotation.text.transform.Find("Line");
            if (lineTransform != null)
            {
                DestroyImmediate(lineTransform.gameObject);
            }
        }

        // Remove the annotation from the list
        annotations.Remove(annotation);

        // Save the updated annotations list
        AnnotationDatasLists annotationDatasLists = new AnnotationDatasLists()
        {
            annotations = annotations
        };
        SaveAnnotations(annotationDatasLists);

        // Destroy the preview if applicable
        DestroyPreview();
    }


    private void RemoveAllClonesInCanvas(string objectName)
    {
        // Find the ModelCanvas in the scene
        Canvas modelCanvas = FindObjectOfType<Canvas>();
        if (modelCanvas != null)
        {
            // Search for all child objects with the specified name
            Transform[] children = modelCanvas.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                // Check if the object name contains the specified name and is a clone (not the original prefab)
                if (child.name.Contains(objectName) && child.name.Contains("(Clone)"))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }


    private void SpawnHotspot()
    {
        GameObject newHotspot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newHotspot.transform.position = Vector3.zero; // Default position, can be changed in HotspotEditWindow
        newHotspot.transform.localScale = Vector3.one * 0.02f;

        // Create a new material instance for the hotspot
        Material hotspotMaterial = new Material(Shader.Find("Standard"));
        hotspotMaterial.color = Color.red; // Default color, can be changed in HotspotEditWindow
        newHotspot.GetComponent<Renderer>().sharedMaterial = hotspotMaterial;

        newHotspot.name = "Anchor"; // Default name, can be changed in HotspotEditWindow

        HotspotData dotData = new HotspotData(newHotspot, newHotspot.transform.position, hotspotMaterial, "Anchor", null);
        dots.Add(dotData);

        // Ensure the dot is marked as dirty so it gets saved with the scene
        EditorUtility.SetDirty(newHotspot);
        SaveHotspots();
    }

    private void RemoveHotspot(HotspotData dotData)
    {
        if (dotData.dot != null)
        {
            DestroyImmediate(dotData.dot);
        }
        dots.Remove(dotData);
        SaveHotspots();

        if (selectedHotspot == dotData)
        {
            selectedHotspot = null;
        }
    }

    private void OpenHotspotEditor(HotspotData dotData)
    {

        HotspotEditWindow.ShowWindow(dotData);
        selectedHotspot = dotData;
        Selection.activeGameObject = dotData.dot;
        Tools.current = Tool.Move;
    }
    private void SaveSettings()
    {
        VideoSettings settings = new VideoSettings
        {
            volume = volume,
            speed = speed,
            videoUrl = videoUrl // Save the URL
        };

        string json = JsonUtility.ToJson(settings, true);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Video settings saved.");
    }

    private void LoadSettings()
    {
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            VideoSettings settings = JsonUtility.FromJson<VideoSettings>(json);
            volume = settings.volume;
            speed = settings.speed;
            videoUrl = settings.videoUrl; // Load the URL
            Debug.Log("Video settings loaded.");
        }
        else
        {
            Debug.LogWarning("Settings file not found.");
        }
    }
    
    private void OnEnable()
    {
        try
        {
            // Get or create the editor window instance
            CombinedEditorWindow window = GetWindow<CombinedEditorWindow>("Combined Editor");

            // Find and initialize canvas
            Canvas canvas = GameObject.Find("ModelCanvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                targetCanvas = canvas;
                TextMeshProUGUI textPrefab = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
                if (textPrefab != null)
                {
                    window.textPrefab = textPrefab.gameObject;
                    Debug.Log("TextObject Reenabled");
                    Debug.Log("Canvas found: " + canvas.name);
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found under ModelCanvas.");
                }
            }
            else
            {
                Debug.LogError("ModelCanvas not found in the scene.");
            }

            // Find and initialize AnimationScript1
            animationScript = FindObjectOfType<AnimationScript1>();
            if (animationScript != null)
            {
                serializedObject = new SerializedObject(animationScript);
                animationDataArray = serializedObject.FindProperty("animationDataArray");
            }
            else
            {
                Debug.LogError("AnimationScript1 not found in the scene.");
            }

            // Find and initialize other components and scripts
            tvObject = FindInActiveObjectByTag("TV");
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;

            LoadMaterials();
            LoadHotspots();
            FindAnimationScript();
            LoadAnimations();
           
            dataManager = FindObjectOfType<DataManager>(); // Ensure a DataManager is in the scene

            // Use Application.streamingAssetsPath for WebGL
            string streamingAssetsPath = Application.streamingAssetsPath;
            fileName = Path.Combine(streamingAssetsPath, "TitleDescriptionData.json");
            filepaths = Path.Combine(Application.streamingAssetsPath, "AnnotationData.json");
            EditorApplication.delayCall += () =>
            {
                annotationDatasLists = LoadAnnotations();
                Debug.Log("Annotations loaded: " + annotationDatasLists.annotations.Count);
                // Check if annotations are correctly linked to renderers
                foreach (var annotation in annotationDatasLists.annotations)
                {
                    if (annotation.targetRenderer == null)
                    {
                        Debug.LogWarning("Failed to link annotation: " + annotation.ownText);
                    }
                }
            };
            InitializeEditorWithData();
            dataManager.LoadData(fileName);
            data = dataManager.data;
            Debug.Log("DataManager found: File " + fileName);

            LoadSavedText();
            openAIController = Utils.FindObjectOfType<OpenAIController>(true);
            if (openAIController == null)
            {
                Debug.LogError("OpenAI Controller not found in the scene.");
            }

            FindAndActivateDragOptions();

            // Use Application.dataPath for Editor builds
            jsonFilePath = Path.Combine(streamingAssetsPath, "DesignData.json");

            LoadDesignData();
            Debug.Log($"JSON file path set to: {jsonFilePath}");

            // Load additional info from PlayerPrefs
            additionalSystemInfo = PlayerPrefs.GetString("AdditionalInfo", "");
            // Set the file path for StreamingAssets
            filePath = Path.Combine(streamingAssetsPath, "videoSettings.json");
            LoadSettings();
        }
        catch (Exception ex)
        {
            Debug.Log($"Error in OnEnable: {ex.Message}");
        }
    }

    private void FindAndActivateDragOptions()
    {
        dragOptionsGameObject = GameObject.Find("DragOptionsComponent");
        if (dragOptionsGameObject != null)
        {
            dragOptionsGameObject.SetActive(true);
        }
        else
        {
            EditorGUILayout.HelpBox("DragOptionsComponent GameObject not found!", MessageType.Error);
        }
    }

    private void UpdateDragOptionsActiveState()
    {
        if (dragOptionsGameObject != null)
        {
            if (EditorApplication.isPlaying)
            {
                dragOptionsGameObject.SetActive(false);
            }
            else
            {
                dragOptionsGameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        SaveHotspots();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        UpdateHotspotBounds();

        if (selectedHotspot != null)
        {
            if (selectedHotspot.dot == null)
            {
                selectedHotspot = null;
                return;
            }

            if (selectedHotspot.targetRenderer != null)
            {
                // Draw handles for the selected hotspot
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.PositionHandle(selectedHotspot.dot.transform.position, Quaternion.identity);
                Undo.RecordObject(selectedHotspot.dot.transform, "Move Hotspot");
                newPosition = ClampPositionToRendererBounds(newPosition, selectedHotspot.targetRenderer);
                selectedHotspot.dot.transform.position = newPosition;
                selectedHotspot.position = newPosition;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedHotspot.dot);
                }
            }
        }
    }

    private Vector3 ClampPositionToRendererBounds(Vector3 position, Renderer renderer)
    {
        Bounds bounds = renderer.bounds;
        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);
        return position;

    }

    private void UpdateHotspotBounds()
    {
        foreach (HotspotData dotData in dots)
        {
            if (dotData.targetRenderer != null)
            {
                dotData.bounds = dotData.targetRenderer.bounds;
            }
        }
    }

    private void SaveHotspots()
    {
        HotspotEditorSaveData saveData = new HotspotEditorSaveData { dots = dots };
        string json = JsonUtility.ToJson(saveData);
        EditorPrefs.SetString("HotspotEditorWindow_Hotspots", json);
    }

    private void LoadHotspots()
    {
        string json = EditorPrefs.GetString("HotspotEditorWindow_Hotspots", "");
        if (!string.IsNullOrEmpty(json))
        {
            HotspotEditorSaveData saveData = JsonUtility.FromJson<HotspotEditorSaveData>(json);
            dots = saveData.dots;

            foreach (HotspotData dotData in dots)
            {
                dotData.dot = GameObject.Find(dotData.name);
                if (dotData.dot != null)
                {
                    dotData.instanceID = dotData.dot.GetInstanceID();
                }
            }
        }
    }

    private void LoadSavedText()
    {
        if (dataManager != null)
        {
            dataManager.LoadData(fileName);
            data = dataManager.data;
        }
        newTitle = data.title;
        newDescription = data.description;
        Debug.Log(newTitle + "," + newDescription);

        if (titleTextObject != null && descriptionTextObject != null)
        {
            titleTextObject.text = newTitle;
            descriptionTextObject.text = newDescription;
        }

    }

    private void ApplyChanges()
    {
        if (popupObject1 == null)
        {
            FocusOnPopup();
            if (popupObject1 == null && waitingForPopup)
            {
                waitingForPopup = false;
                return;
            }
            return;
        }
        dataManager = FindObjectOfType<DataManager>(); // Ensure a DataManager is in the scene
        if (dataManager != null)
        {
            dataManager.data = data;
            dataManager.SaveData(fileName);
        }
        else
        {
            Debug.LogError("DataManager is not assigned. Cannot save data to JSON file.");
        }
        if (popupObject1.activeInHierarchy)
        {
            ApplyChangesToTextObjects();
        }
        else
        {
            waitingForPopup = true;
            EditorApplication.update += WaitForPopupAndApplyChanges;
        }
       
    }

    private void WaitForPopupAndApplyChanges()
    {
        if (popupObject1 != null && popupObject1.activeInHierarchy)
        {
            waitingForPopup = false;
            EditorApplication.update -= WaitForPopupAndApplyChanges;
            ApplyChangesToTextObjects();
        }
    }

    private void ApplyChangesToTextObjects()
    {
        if (titleTextObject == null || descriptionTextObject == null)
        {
            AssignTextObjects();
        }



        if (titleTextObject != null && descriptionTextObject != null)
        {
            if (reset)
            {
                UpdateTitleAndDescription();
                reset = false;
            }

            if (!string.IsNullOrEmpty(newTitle.Trim()) && !string.IsNullOrEmpty(newDescription.Trim()))
            {

                titleTextObject.text = newTitle;
                descriptionTextObject.text = newDescription;

                // Save changes to PlayerPrefs
                PlayerPrefs.SetString("newTitle", newTitle);
                PlayerPrefs.SetString("newDescription", newDescription);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Debug.LogWarning("Title or Description Text Object not selected.");
        }
    }


    private void UpdateTitleAndDescription()
    {
        if ((string.IsNullOrEmpty(newTitle?.Trim()) || string.IsNullOrEmpty(newDescription?.Trim())) && reset)
        {
            if (!messageShown)
            {
                Debug.LogWarning("New title or description is empty due to reset.");
                messageShown = true; // Set the flag to true after showing the message
            }

            // Set title and description to empty strings
            newTitle = "";
            newDescription = "";

            // Check if titleTextObject is assigned
            if (titleTextObject != null)
            {

                titleTextObject.text = newTitle;
            }
            else
            {
                Debug.LogError("titleTextObject is not assigned.");
            }

            // Check if descriptionTextObject is assigned
            if (descriptionTextObject != null)
            {

                descriptionTextObject.text = newDescription;
            }
            else
            {
                Debug.LogError("descriptionTextObject is not assigned.");
            }

            // Save the changes
            PlayerPrefs.SetString("newTitle", newTitle);
            PlayerPrefs.SetString("newDescription", newDescription);
            PlayerPrefs.Save();

            reset = false;
        }
    }


    private void AssignTextObjects()
    {
        // Directly find the Title and Description within popupObject1
        if (popupObject1 != null)
        {
            Transform titleTransform = popupObject1.transform.Find("Title");
            Transform descriptionTransform = popupObject1.transform.Find("Description");

            if (titleTransform != null)
            {
                titleTextObject = titleTransform.GetComponent<Text>();
            }

            if (descriptionTransform != null)
            {
                descriptionTextObject = descriptionTransform.GetComponent<Text>();
            }
        }
    }

    private void FocusOnPopup()
    {
        // Find the SecondFunction GameObject first
        GameObject secondFunctionObject = FindGameObject("Functions/SecondFunction");
        GameObject AddModelhere = FindGameObject("AddModelHere");
        if (secondFunctionObject != null)
        {
            // Ensure SecondFunction is active
            if (!secondFunctionObject.activeSelf)
            {
                secondFunctionObject.SetActive(true);
            }

            // Find the Popup GameObject within the active SecondFunction hierarchy
            GameObject popupObject = FindGameObject("Functions/SecondFunction/Popup/Panel");

            if (popupObject != null)
            {
                // Enable the Popup GameObject if it is disabled and not already active
                if (!popupObject.activeSelf && !applyButtonPressed)
                {
                    popupObject.SetActive(true);
                    Debug.Log("PopUp Enabled");
                }

                // Check if the "Show Preview" button was pressed
                if (!applyButtonPressed)
                {
                    // Focus on the Popup GameObject in the scene view
                    popupObject1 = popupObject;
                    Selection.activeGameObject = popupObject;

                    // Adjust the scene view camera position and zoom level
                    SceneView sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null)
                    {
                        sceneView.LookAt(popupObject.transform.position, Quaternion.identity);
                        sceneView.size = 500; // Adjust the zoom level as needed
                    }
                }
                else
                {
                    // Focus on the AddModelhere GameObject after pressing the "Apply" button
                    if (!applyButtonReleased)
                    {
                        // Get the target object's position
                        Vector3 targetPosition = AddModelhere.transform.position;

                        // Set the rotation of the scene view camera to look at the target object from the side (-90 degrees around the Z-axis)
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView != null)
                        {
                            sceneView.LookAt(targetPosition, Quaternion.Euler(0, 90, 0));

                            // Adjust the zoom level as needed
                            sceneView.size = 1;

                            // Set applyButtonReleased to true to ensure this block only runs once
                            applyButtonReleased = true;
                        }
                    }
                    else
                    {
                        // Unlock the scene view after pressing the "Apply" button
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView != null)
                        {
                            sceneView.orthographic = false;
                            sceneView.isRotationLocked = false;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Popup GameObject not found in the Functions/SecondFunction/Popup hierarchy.");
            }
        }
        else
        {
            Debug.LogWarning("SecondFunction GameObject not found in the Functions hierarchy.");
        }
    }

    private void FindAnimationScript()
    {
        animationScript = FindObjectOfType<AnimationScript1>();
        if (animationScript != null)
        {
            serializedObject = new SerializedObject(animationScript);
            animationDataArray = serializedObject.FindProperty("animationDataArray");
        }
    }

    private void OnHierarchyChanged()
    {
        GameObject fourthFunction = GameObject.Find("FourthFunction");
        isFourthFunctionEnabled = fourthFunction != null && fourthFunction.activeInHierarchy;
        Repaint(); // Repaint the window to update the GUI
    }


    private void ClearSerializedData()
    {
        // Call remove functions for all annotations
        while (annotations.Count > 0)
        {
            RemoveAnnotation(annotations[0]);
        }

        // Call remove functions for all hotspots
        while (dots.Count > 0)
        {
            RemoveHotspot(dots[0]);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure the changes are saved to disk immediately

        PlayerPrefs.Save();

        // You might also want to clear lists if they are serialized as JSON or other formats
        ClearSerializedLists();
        if (annotations.Count == 0 && dots.Count == 0)
        {
            if (!messageShown)
            {
                Debug.Log("Dots Cleared");
            }
        }
    }

    private void ClearSerializedLists()
    {
        // Assuming the serialized data is stored in JSON format in PlayerPrefs or other storage
        // Clear AnimationData
        var animationDataList = new List<AnimationData>();
        SaveSerializedData("AnimationData", animationDataList);

        // Clear AnnotationDatasLists
        var annotationDatasLists = new AnnotationDatasLists { annotations = new List<AnnotationDatas>() };
        SaveSerializedData("AnnotationDatasLists", annotationDatasLists);

        // Clear HotspotEditorSaveData
        var hotspotEditorSaveData = new HotspotEditorSaveData { dots = new List<HotspotData>() };
        SaveSerializedData("HotspotEditorSaveData", hotspotEditorSaveData);
    }

    private void SaveSerializedData<T>(string key, T data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private void RemoveChildModels()
    {
        // Find the GameObject named "AddModelHere"
        GameObject addModelHere = GameObject.Find("AddModelHere");

        // Check if the GameObject is found
        if (addModelHere != null)
        {
            // Loop through each child of the "AddModelHere" GameObject
            foreach (Transform child in addModelHere.transform)
            {
                // Check if the child is not the "AddModelHere" GameObject itself
                if (child.gameObject != addModelHere)
                {
                    // Check if the child is not the TV GameObject
                    if (child.gameObject.name != "Media")
                    {
                        // Destroy the child GameObject
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }
        else
        {
            // Log a warning if the GameObject is not found
            Debug.LogWarning("GameObject 'AddModelHere' not found.");
        }
    }

    private void UpdateSystemMessage()
    {
        if (openAIController != null)
        {
            Debug.Log("Updating AI Chatbot.");
            openAIController.SetAdditionalInfo(additionalSystemInfo);
        }
        else
        {
            Debug.LogError("OpenAIController is null. Cannot update AI Chatbot.");
        }
    }


    public static class Utils
    {
        public static T FindObjectOfType<T>(bool includeInactive) where T : UnityEngine.Object
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            foreach (T obj in objects)
            {
                if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (includeInactive || ((obj as Component)?.gameObject.activeInHierarchy ?? true))
                    return obj;
            }
            return null;
        }
    }

    private void ClearScriptsFromDragOptionsHere()
    {
        GameObject dragOptionsHere = GameObject.Find("DragOptionsHere");
        if (dragOptionsHere != null)
        {
            var components = dragOptionsHere.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component.GetType().Name != "Hidden1")
                {
                    DestroyImmediate(component);
                }
            }
        }
    }

    private void FindValidRenderers(string parentName, string excludedChildName)
    {
        validRenderers = new List<Renderer>();
        GameObject parentObject = GameObject.Find(parentName);

        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                if (child.name != excludedChildName)
                {
                    Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer renderer in renderers)
                    {
                        if (!IsExcludedChild(renderer.gameObject, parentName, excludedChildName))
                        {
                            validRenderers.Add(renderer);
                        }
                    }
                }
            }
        }
    }

    private bool IsExcludedChild(GameObject gameObject, string parentName, string excludedChildName)
    {
        Transform current = gameObject.transform;

        while (current != null)
        {
            if (current.name == excludedChildName || current.name == parentName)
            {
                return false;
            }
            current = current.parent;
        }

        return true;
    }

    private Renderer CustomRendererDropdown(string label, Renderer selectedRenderer)
    {
        if (validRenderers == null || validRenderers.Count == 0)
        {
            EditorGUILayout.HelpBox("No valid renderers found.", MessageType.Warning);
            return null;
        }

        string[] options = new string[validRenderers.Count];
        int currentIndex = -1;

        for (int i = 0; i < validRenderers.Count; i++)
        {
            options[i] = validRenderers[i].gameObject.name;
            if (validRenderers[i] == selectedRenderer)
            {
                currentIndex = i;
            }
        }

        int newIndex = EditorGUILayout.Popup(label, currentIndex, options);

        if (newIndex >= 0 && newIndex < validRenderers.Count)
        {
            return validRenderers[newIndex];
        }

        return null;
    }

    private void FindValidAnchorRenderers(string parentName, string excludedChildName)
    {
        validRenderers = new List<Renderer>();
        GameObject parentObject = GameObject.Find(parentName);

        if (parentObject != null)
        {
            // Iterate through children of the parent object
            foreach (Transform child in parentObject.transform)
            {
                if (child.name != excludedChildName)
                {
                    Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer renderer in renderers)
                    {
                        if (!IsExcludedChild(renderer.gameObject, parentName, excludedChildName))
                        {
                            validRenderers.Add(renderer);
                        }
                    }
                }
            }
        }

        // Find all GameObjects with a Sphere mesh filter in the scene
        MeshFilter[] allMeshFilters = GameObject.FindObjectsOfType<MeshFilter>();
        foreach (MeshFilter meshFilter in allMeshFilters)
        {
            if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.name == "Sphere")
            {
                Renderer renderer = meshFilter.GetComponent<Renderer>();
                if (renderer != null && !IsChildOfParent(renderer.gameObject, parentName))
                {
                    validRenderers.Add(renderer);
                }
            }
        }
    }



    private bool IsChildOfParent(GameObject gameObject, string parentName)
    {
        Transform parentTransform = gameObject.transform.parent;
        while (parentTransform != null)
        {
            if (parentTransform.name == parentName)
            {
                return true;
            }
            parentTransform = parentTransform.parent;
        }
        return false;
    }


    private Renderer CustomAnchorRendererDropdown(string label, Renderer selectedRenderer)
    {
        if (validRenderers == null || validRenderers.Count == 0)
        {
            EditorGUILayout.HelpBox("No valid renderers found.", MessageType.Warning);
            return null;
        }

        string[] options = new string[validRenderers.Count];
        int currentIndex = -1;

        for (int i = 0; i < validRenderers.Count; i++)
        {
            options[i] = validRenderers[i].gameObject.name;
            if (validRenderers[i] == selectedRenderer)
            {
                currentIndex = i;
            }
        }

        int newIndex = EditorGUILayout.Popup(label, currentIndex, options);

        if (newIndex >= 0 && newIndex < validRenderers.Count)
        {
            return validRenderers[newIndex];
        }

        return null;
    }


    private void ResetUI()
    {
        modelFileFoldout = false;
        descriptionFoldout = false;
        mediaFileFoldout = false;
        hotspotFoldout = false;
        annotationFoldout = false;
        animationFoldout = false;
        materialFoldout = false;
        chatbotFoldout = false;

        modelFileEnabled = false;
        descriptionEnabled = false;
        mediaFileEnabled = false;
        hotspotEnabled = false;
        annotationEnabled = false;
        animationEnabled = false;
        materialEnabled = false;
        chatbotEnabled = false;
    }
}

[System.Serializable]
public class AnimationImportHistoryEntry
{
    public GameObject model;
    public AnimationClip clip;
}

[System.Serializable]
public class AnimationData
{
    public Transform targetTransform; // Use Transform instead of MeshRenderer to manipulate position and rotation
    public Vector3 disassemblyOffset; // Offset for disassembly
    public float disassemblyDuration; // Duration of disassembly animation
    [HideInInspector]
    public Vector3 finalPosition; // Store final position for disassembly
    [HideInInspector]
    public Quaternion finalRotation; // Store final rotation for disassembly
    [HideInInspector]
    public Vector3 initialPosition; // Store initial position for reassembly
    [HideInInspector]
    public Quaternion initialRotation; // Store initial rotation for reassembly
}

[Serializable]
public class AnnotationDatas
{
    public int index;
    public string ownText;
    public Vector3 offset;
    public Vector3 rotation;
    public int rendererInstanceID;
    public string rendererPath;
    public Renderer targetRenderer;
    public string uniqueID;
    public Material newMat;

    // Added properties
    public TextMeshProUGUI text;
    public Vector3 position;

    // Constructor
    public AnnotationDatas()
    {
        uniqueID = System.Guid.NewGuid().ToString();
        ownText = "";
        offset = Vector3.zero;
        rotation = Vector3.zero;
        rendererInstanceID = 0;
        targetRenderer = null;
        text = null;
        newMat = null;
        position = Vector3.zero;
        index = 1;
    }

    public void SetTargetRenderer(Renderer renderer)
    {
        targetRenderer = renderer;
        rendererInstanceID = renderer.GetInstanceID();
    }
    public Renderer GetTargetRenderer()
    {
        // Check if the targetRenderer is already assigned
        if (targetRenderer != null)
        {
            return targetRenderer;
        }

        // If rendererInstanceID is not set, return null
        if (rendererInstanceID == 0)
        {
            return null;
        }

        // Use a more efficient method to find the target Renderer
        Renderer renderer = EditorUtility.InstanceIDToObject(rendererInstanceID) as Renderer;

        // Assign to targetRenderer if found
        targetRenderer = renderer;

        return targetRenderer;
    }

    public bool IsValid()
    {
        return targetRenderer != null && rendererInstanceID == targetRenderer.GetInstanceID();
    }
}

[System.Serializable]
public class AnnotationDatasLists
{
    public List<AnnotationDatas> annotations = new List<AnnotationDatas>();
}

[System.Serializable]
public class HotspotData
{
    public GameObject dot;
    public Vector3 position;
    public string name;
    public Renderer targetRenderer;
    public Bounds bounds;
    public int instanceID;

    // Use Material instead of Color for hotspot color
    public Material material;

    public HotspotData(GameObject dot, Vector3 position, Material material, string name, Renderer targetRenderer)
    {
        this.dot = dot;
        this.position = position;
        this.material = material;
        this.name = name;
        this.targetRenderer = targetRenderer;
        this.instanceID = dot.GetInstanceID();
        if (targetRenderer != null)
        {
            this.bounds = targetRenderer.bounds;
        }
    }
}

[System.Serializable]
public class HotspotEditorSaveData
{
    public List<HotspotData> dots;
}

public static class HotspotDataExtensions
{
    public static void UpdateHotspot(this HotspotData dotData)
    {
        if (dotData.dot != null)
        {
            dotData.dot.transform.position = dotData.position;
            dotData.dot.GetComponent<Renderer>().sharedMaterial = dotData.material; // Assign the material
            dotData.dot.name = dotData.name;
            EditorUtility.SetDirty(dotData.dot);
        }
    }
}

public class AnnotationEditWindows : EditorWindow
{
    private CombinedEditorWindow parentWindow;
    private AnnotationDatas annotationData;
    private Material lineMaterial;
    private Material originalLineMaterial; // Field to store the original material
    private LineRenderer previewLine;
    private GameObject previewText;
    private GameObject textPrefab;
    private Canvas targetCanvas;
    private string folderPath = "Assets/Colors";
    private List<Material> materialsInFolder = new List<Material>();

    private string originalText;
    private Vector3 originalPosition;
    private Vector3 originalRotation;

    private string annotationText;
    private Vector3 offset;
    private Vector3 rotation;

    public void Init(CombinedEditorWindow parent, AnnotationDatas data, Material material, GameObject prefab, Canvas canvas)
    {
        parentWindow = parent;
        annotationData = data;
        textPrefab = prefab;
        targetCanvas = canvas;

        // Ensure targetRenderer is set
        annotationData.GetTargetRenderer();

        // Store the original values for reverting changes if needed
        originalText = data.ownText;
        originalPosition = data.offset;
        originalRotation = data.rotation;

        // Initialize fields with current annotation data
        annotationText = originalText;
        offset = originalPosition;
        rotation = originalRotation;

        // Set the originalLineMaterial if previewLine exists
        if (previewLine != null)
        {
            originalLineMaterial = previewLine.material;
        }
        else
        {
            originalLineMaterial = material; // Use the provided material if no preview line exists yet
        }

        // Initialize lineMaterial with the original material
        lineMaterial = originalLineMaterial;
        parentWindow.LoadMaterials();
    }


    private void OnGUI()
    {
        GUILayout.Label("Edit Annotation", EditorStyles.boldLabel);

        // Input field for annotation text
        annotationText = EditorGUILayout.TextField("Annotation Text", annotationText);

        // Dropdown for selecting a material from the available options
        if (materialsInFolder.Any())
        {
            string[] materialNames = materialsInFolder.Select(m => m.name).ToArray();
            int selectedIndex = materialsInFolder.IndexOf(lineMaterial);
            int newSelectedIndex = EditorGUILayout.Popup("Select Material", selectedIndex, materialNames);

            if (newSelectedIndex != selectedIndex)
            {
                lineMaterial = materialsInFolder[newSelectedIndex];
            }
        }

        // Sliders for editing the offset
        GUILayout.Label("Edit Offset", EditorStyles.boldLabel);
        offset.x = EditorGUILayout.Slider("X", offset.x, -10f, 10f);
        offset.y = EditorGUILayout.Slider("Y", offset.y, -10f, 10f);
        offset.z = EditorGUILayout.Slider("Z", offset.z, -10f, 10f);

        // Rotation can be uncommented if you need to adjust it
        
        GUILayout.Label("Edit Rotation", EditorStyles.boldLabel);
        rotation.x = EditorGUILayout.Slider("X", rotation.x, -180f, 180f);
        rotation.y = EditorGUILayout.Slider("Y", rotation.y, -180f, 180f);
        rotation.z = EditorGUILayout.Slider("Z", rotation.z, -180f, 180f);
        

        // Draw a preview of the annotation in the scene
        if (Event.current.type == EventType.Repaint)
        {
            DrawPreview(offset, rotation, annotationText);
        }

        // Buttons to confirm or cancel changes
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            // Update the parent window's annotation with the changes
            parentWindow.UpdateAnnotation(annotationData, annotationText, offset, rotation, lineMaterial);
            parentWindow.DestroyPreview();
            lineMaterial = null;
            parentWindow.ResetFields();
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            // Revert any changes and close the window
            parentWindow.DrawAnnotationPreview(originalPosition, originalRotation, originalText);
            parentWindow.DestroyPreview();
            lineMaterial = null;
            parentWindow.ResetFields();
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }


    private void DrawPreview(Vector3 position, Vector3 rotation, string text)
    {
        Renderer renderer = annotationData.GetTargetRenderer();
        if (renderer == null)
        {
            Debug.LogWarning("Target renderer is null or has been destroyed. Cannot draw preview.");
            return;
        }

        if (!renderer.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Renderer is inactive.");
            return;
        }

        Bounds bounds;
        if (renderer is MeshRenderer meshRenderer)
        {
            bounds = meshRenderer.bounds;
        }
        else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
        {
            bounds = skinnedMeshRenderer.bounds;
        }
        else
        {
            bounds = CalculateBounds(renderer.transform);
        }

        // Handle preview text
        if (previewText == null)
        {
            previewText = Instantiate(textPrefab, targetCanvas.transform);
            previewText.name = annotationData.uniqueID;
        }

        // Handle preview line
        if (previewLine == null)
        {
            GameObject lineObj = new GameObject("LinePreview");
            lineObj.transform.SetParent(targetCanvas.transform, false);
            previewLine = lineObj.AddComponent<LineRenderer>();
        }

        // Set the line material
        if (lineMaterial != null)
        {
            previewLine.material = lineMaterial;
        }
        else if (originalLineMaterial != null)
        {
            previewLine.material = originalLineMaterial;
        }
        else
        {
            Debug.LogWarning("No valid material is set for the LineRenderer.");
        }

        previewLine.startWidth = 0.02f;
        previewLine.endWidth = 0.02f;

        Vector3 annotationPosition = bounds.center + position;
        TextMeshProUGUI textComponent = previewText.GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = previewText.GetComponent<RectTransform>();

        if (textComponent != null)
        {
            textComponent.text = text;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component not found on previewText.");
        }

        if (rectTransform != null)
        {
            rectTransform.position = annotationPosition;
            rectTransform.rotation = Quaternion.Euler(rotation);
        }
        else
        {
            Debug.LogWarning("RectTransform component not found on previewText.");
        }

        if (previewLine != null)
        {
            previewLine.SetPosition(0, bounds.center);
            previewLine.SetPosition(1, annotationPosition);
        }
    }

    private Bounds CalculateBounds(Transform transform)
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        else
        {
            bounds.center = transform.position;
            bounds.size = transform.lossyScale;
        }

        return bounds;
    }

    private void OnDestroy()
    {
        if (previewText != null)
        {
            DestroyImmediate(previewText.gameObject);
        }

        if (previewLine != null)
        {
            DestroyImmediate(previewLine.gameObject);
        }
    }
}


public class HotspotEditWindow : EditorWindow
{
    private HotspotData dotData;
    private Material initialMaterial;
    private List<Renderer> validRenderers;

    // Modify the ShowWindow method to pass the list of HotspotData objects
    public static void ShowWindow(HotspotData dotData)
    {
        HotspotEditWindow window = GetWindow<HotspotEditWindow>("Edit Hotspot");
        window.dotData = dotData;
        window.initialMaterial = dotData.material; // Save the initial material
        window.FindValidRenderers("AddModelHere", "Media");
        window.Show();
    }

    private void OnGUI()
    {
        if (dotData == null)
        {
            EditorGUILayout.HelpBox("No Hotspot selected.", MessageType.Warning);
            return;
        }

        GUILayout.Label("Edit Hotspot", EditorStyles.boldLabel);

        // Material Field for Hotspot Material with preview
        GUILayout.Label("Material", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        dotData.material = EditorGUILayout.ObjectField("Anchor Material", dotData.material, typeof(Material), false) as Material;
        if (EditorGUI.EndChangeCheck())
        {
            UpdateHotspotMaterialPreview(dotData.material);
        }

        // Text Field for Hotspot Name
        GUILayout.Label("Name", EditorStyles.boldLabel);
        dotData.name = EditorGUILayout.TextField("Anchor Name", dotData.name);

        // Field for selecting the target renderer
        GUILayout.Label("Select Component", EditorStyles.boldLabel);
        dotData.targetRenderer = CustomRendererDropdown("Component", dotData.targetRenderer);


        // Display current position (not editable here, updated in SceneView)
        GUILayout.Label("Position", EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Anchor Position", dotData.dot.transform.position);
        if (dotData.targetRenderer == null)
        {
            EditorGUILayout.HelpBox("Please assign a component.", MessageType.Error);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Anchor"))
        {
            if (dotData.targetRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target renderer before updating the dot.", "OK");
                return;
            }

            // Ensure properties are updated
            dotData.position = dotData.dot.transform.position; // Ensure dotData position is up-to-date
            dotData.UpdateHotspot();
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            // Revert to the initial material if the user cancels
            dotData.material = initialMaterial;
            dotData.UpdateHotspot();
            Close();
        }

        GUILayout.EndHorizontal();
    }

    // Update the hotspot material preview
    private void UpdateHotspotMaterialPreview(Material material)
    {
        dotData.dot.GetComponent<Renderer>().sharedMaterial = material;
        EditorUtility.SetDirty(dotData.dot);
    }

    private void FindValidRenderers(string parentName, string excludedChildName)
    {
        validRenderers = new List<Renderer>();
        GameObject parentObject = GameObject.Find(parentName);

        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                if (child.name != excludedChildName)
                {
                    Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer renderer in renderers)
                    {
                        if (!IsExcludedChild(renderer.gameObject, parentName, excludedChildName))
                        {
                            validRenderers.Add(renderer);
                        }
                    }
                }
            }
        }
    }

    private bool IsExcludedChild(GameObject gameObject, string parentName, string excludedChildName)
    {
        Transform current = gameObject.transform;

        while (current != null)
        {
            if (current.name == excludedChildName || current.name == parentName)
            {
                return false;
            }
            current = current.parent;
        }

        return true;
    }

    private Renderer CustomRendererDropdown(string label, Renderer selectedRenderer)
    {
        if (validRenderers == null || validRenderers.Count == 0)
        {
            EditorGUILayout.HelpBox("No valid renderers found.", MessageType.Warning);
            return null;
        }

        string[] options = new string[validRenderers.Count];
        int currentIndex = -1;

        for (int i = 0; i < validRenderers.Count; i++)
        {
            options[i] = validRenderers[i].gameObject.name;
            if (validRenderers[i] == selectedRenderer)
            {
                currentIndex = i;
            }
        }

        int newIndex = EditorGUILayout.Popup(label, currentIndex, options);

        if (newIndex >= 0 && newIndex < validRenderers.Count)
        {
            return validRenderers[newIndex];
        }

        return null;
    }
}