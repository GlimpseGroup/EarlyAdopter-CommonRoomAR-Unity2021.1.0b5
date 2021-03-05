using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RenderHeads.Media.AVProVideo;
using Dummiesman;
using TMPro;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
//using Observer;

[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARTrackedImageManager))]
public class DioramaManager : MonoBehaviour
{
    public static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    public Camera MainCamera;
    ARSessionOrigin m_SessionOrigin;
    public static ARRaycastManager m_Raycaster;

    public static readonly float DIORAMASCALE = 4.0f;

    public static DioramaManager instance;

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        instance = this;
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_Raycaster = GetComponent<ARRaycastManager>();
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(AdminController.Instance.DioramaTOKEN))
            RequestToken();
        else
        {
            TOKEN = AdminController.Instance.DioramaTOKEN;
            RequestDioramaByCategory();
        }

        if (AdminController.Instance != null)
        {
            if (AdminController.Instance.DioramaWebinar)
                StartCoroutine(co_WebinarStart());
        }
    }
    IEnumerator co_WebinarStart()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (GameObject g in DisableOnWebinarStart)
            g.SetActive(false);
        SelectNewPresentationBlocker.SetActive(true);
        SwitchWebinarMode();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInstantiateDiorama();
        TransformReferencePointUpdate();

        if (DioramaReferencePoint)
        {
            if (!t_DioramaReferenceVisual)
            {
                t_DioramaReferenceVisual = Instantiate(DioramaReferenceVisual);
                t_DioramaReferenceVisual.gameObject.SetActive(true);
            }
            t_DioramaReferenceVisual.position = DioramaReferencePoint.transform.position;
            t_DioramaReferenceVisual.rotation = DioramaReferencePoint.transform.rotation;
        }
        else
            if (t_DioramaReferenceVisual)
                Destroy(t_DioramaReferenceVisual.gameObject);

        if (DioramaBeacon.instance != null && WebinarImageTracking)
        {
            if (DioramaReferencePoint == null)
            {
                DioramaReferencePoint = new GameObject();
                DioramaReferencePoint.name = "Diorama Reference Point";
                DioramaReferencePoint.transform.position = DioramaBeacon.instance.transform.position;
                DioramaReferencePoint.transform.rotation = Quaternion.identity;

                t_DioramaReferenceVisual = Instantiate(DioramaReferenceVisual);
                t_DioramaReferenceVisual.gameObject.SetActive(false);
                LoadingVisual.SetActive(false);

                PlaceReferencePointPhase = false;
                ConfirmPlaceReferencePoint = false;
            }
            DioramaReferencePoint.gameObject.SetActive(true);
            t_DioramaReferenceVisual.gameObject.SetActive(false);
            DioramaReferencePoint.transform.position = DioramaBeacon.instance.transform.position;
            DioramaReferencePoint.transform.rotation = Quaternion.identity;

        }
        else if (WebinarImageTracking)
        {
            if (DioramaReferencePoint)
            {
                DioramaReferencePoint.gameObject.SetActive(false);
                t_DioramaReferenceVisual.gameObject.SetActive(false);
            }
        }

        if (PlaceReferencePointPhase)
        {
            if (ConfirmPlaceReferencePoint)
            {
#if UNITY_EDITOR
                DioramaReferencePoint = new GameObject();
                DioramaReferencePoint.name = "Diorama Reference Point";
                DioramaReferencePoint.transform.position = new Vector3(0,-0.5f,2f);
                DioramaReferencePoint.transform.rotation = Quaternion.identity;

                t_DioramaReferenceVisual = Instantiate(DioramaReferenceVisual);
                t_DioramaReferenceVisual.gameObject.SetActive(true);
                LoadingVisual.SetActive(false);

                PlaceReferencePointPhase = false;
                ConfirmPlaceReferencePoint = false;
#endif
                Vector2 PlaceReferencePointPos = new Vector2(Screen.width / 2, Screen.height / 2);
                if (m_Raycaster.Raycast(PlaceReferencePointPos, s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    DioramaReferencePoint = new GameObject();
                    DioramaReferencePoint.name = "Diorama Reference Point";
                    DioramaReferencePoint.transform.position = hitPose.position;
                    DioramaReferencePoint.transform.rotation = Quaternion.identity;

                    t_DioramaReferenceVisual = Instantiate(DioramaReferenceVisual);
                    t_DioramaReferenceVisual.gameObject.SetActive(true);
                    LoadingVisual.SetActive(false);

                    PlaceReferencePointPhase = false;
                    ConfirmPlaceReferencePoint = false;
                }
            }

            if (t_DioramaReferencePreVisual == null)
            {
                t_DioramaReferencePreVisual = Instantiate(DioramaReferencePreVisual);
                t_DioramaReferencePreVisual.gameObject.SetActive(true);
            }
            Vector2 pos = new Vector2(Screen.width / 2, Screen.height / 2);
            if (m_Raycaster.Raycast(pos, s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                t_DioramaReferencePreVisual.gameObject.SetActive(true);
                t_DioramaReferencePreVisual.position = hitPose.position;
            }
            else
            {
                t_DioramaReferencePreVisual.gameObject.SetActive(false);
            }    
        } else
        {
            if (t_DioramaReferencePreVisual)
            {
                Destroy(t_DioramaReferencePreVisual.gameObject);
                t_DioramaReferencePreVisual = null;
            }
        }

        NetworkIssue.alpha = NetworkIssueAlpha;
        NetworkIssueAlpha -= Time.deltaTime;
        NetworkIssueAlpha = NetworkIssueAlpha > 0 ? NetworkIssueAlpha : 0;

        DioramaReferencePointREADONLY = DioramaReferencePoint;
        SelectionScreenOpen = LoadingScreen.activeInHierarchy || CategoryBGFullscreen.activeInHierarchy || CategoryBGWindowed.activeInHierarchy || StudentBGFullscreen.activeInHierarchy || StudentBGWindowed.activeInHierarchy;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    ARTrackedImageManager m_TrackedImageManager;

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            //trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            WebinarImagePrefabSearch(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            WebinarImagePrefabSearch(trackedImage);
        }
    }
    
    void WebinarImagePrefabSearch(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        //Debug.Log("WEBINAR : " + imageName);
        for (int i = 0; i < LoadedDocument.categories.Length; i++)
        {
            DioramaCategory category = LoadedDocument.categories[i];
            for (int j = 0; j < category.presentations.Length; j++)
            {
                string comparison = LoadedDocument.categories[i].presentations[j].trackedImage;
                if (string.IsNullOrEmpty(comparison))
                    continue;
                if (comparison.LastIndexOf("/") != -1)
                    comparison = comparison.Substring(comparison.LastIndexOf("/") + 1);
                if (comparison.IndexOf('.') != -1)
                    comparison = comparison.Substring(0, comparison.IndexOf('.'));
                //Debug.Log("WEBINAR COMPARE : " + imageName + " :: " + comparison);
                if (imageName == comparison)
                    LoadProjectIfNotOpened(i, j);
            }
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Diorama Selection")]
    public bool SelectionScreenOpen = true;

    public DioramaDocument LoadedDocument;
    public Transform LoadedProject;

    public Transform ProjectAssets;

    public int[] LastProject = { 0, 0 };

    [Header("Diorama Selection (UI)")]
    public TMP_FontAsset GlobalTMPFont;
    public Texture2D Error;
    public CanvasGroup NetworkIssue;
    private float NetworkIssueAlpha = 0;

    public Transform CategoryContent;
    public Transform CategoryButton;

    public Text StudentUIText;
    public Transform StudentContent;
    public Transform StudentButton;

    public GameObject SelectNewDioramaButton;
    public GameObject LoadingScreen;
    public GameObject CategoryBGFullscreen;
    public GameObject CategoryBGWindowed;
    public GameObject StudentBGFullscreen;
    public GameObject StudentBGWindowed;

    public Text Nickname;
    public Text Nickname2;

    [Header("Webinar")]
    public ARTrackedImageManager ImageManager;
    public TrackedImageInfoManager ImageInfo;
    public RuntimeReferenceImageLibrary ImageLibrary;
    public bool WebinarImageTracking = false;
    public Text WebinarButtonText;
    public GameObject SelectNewPresentationBlocker;

    public GameObject[] DisableOnWebinarStart;

    public void SwitchWebinarMode()
    {
        if (t_DioramaReferenceVisual)
            Destroy(t_DioramaReferenceVisual.gameObject);
        if (!WebinarImageTracking)
        {
            WebinarImageTracking = true;
            WebinarButtonText.text = "World Placement Mode";
            if (DioramaReferencePoint != null)
            {
                Destroy(DioramaReferencePoint.gameObject);
                DioramaReferencePoint = null;
            }
        }
        else
        {
            if (DioramaBeacon.instance)
                Destroy(DioramaBeacon.instance.gameObject);
            if (t_DioramaReferenceVisual)
                Destroy(t_DioramaReferenceVisual.gameObject);
            WebinarImageTracking = false;
            WebinarButtonText.text = "Webinar Mode";
            if (DioramaReferencePoint != null)
            {
                Destroy(DioramaReferencePoint.gameObject);
                DioramaReferencePoint = null;
            }
            StartPlaceReferencePoint();
            ConfirmPlaceReferencePoint = true;
        }
    }

    void DownloadWebinarImageTargets()
    {
        StartCoroutine(cr_DownloadWebinarImageTargets());
    }
    IEnumerator cr_DownloadWebinarImageTargets()
    {
        List<Texture2D> images = new List<Texture2D>();
        List<string> imageNames = new List<string>();
        for (int i = 0; i < LoadedDocument.categories.Length; i++)
        {
            DioramaCategory category = LoadedDocument.categories[i];
            for (int j = 0; j < category.presentations.Length; j++)
            {
                DioramaProject project = category.presentations[j];
                if (!string.IsNullOrEmpty(project.trackedImage))
                {
                    string link = project.trackedImage;
                    if (string.IsNullOrEmpty(link) || i < 0 || j < 0)
                        yield break;

                    string url = link;
                    string fileName = link.Substring(link.LastIndexOf('/') + 1);
                    string IMAGENAME = fileName;
                    fileName = LoadedDocument.categories[i].name + "/" +
                                             LoadedDocument.categories[i].presentations[j].title + "/" +
                        fileName;
                    Debug.Log(fileName + " ::: " + url);
                    string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

                    if (!link.Substring(link.Length - 5).Contains("."))
                    {
                        fileName += ".png";
                        savePath += ".png";
                    }
                    Texture2D image = new Texture2D(1, 1);
                    if (!File.Exists(savePath))
                    {
                        using (UnityWebRequest www = UnityWebRequest.Get(url))
                        {
                            www.redirectLimit = 10;
                            www.chunkedTransfer = false;
                            www.SendWebRequest();
                            float totalTime = 0;
                            while ((!www.isDone || !www.downloadHandler.isDone) && totalTime < 12)
                            {
                                yield return new WaitForSeconds(1f);
                                totalTime++;
                            }
                            //yield return www.SendWebRequest();
                            if (!string.IsNullOrEmpty(www.error))
                            {
                                Debug.Log(www.error);
                                image = Error;
                            }
                            else if (totalTime < 12)
                            {
                                Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
                                File.WriteAllBytes(savePath, www.downloadHandler.data);
                                byte[] bytes = File.ReadAllBytes(savePath);
                                image.LoadImage(bytes);
                            }
                            if (www != null && !www.isDone)
                            {
                                NetworkIssueAlpha = 2;
                                www.Abort();
                            }
                        }
                    }
                    else
                    {
                        byte[] bytes = File.ReadAllBytes(savePath);
                        image.LoadImage(bytes);
                    }
                    images.Add(image);
                    if (IMAGENAME.IndexOf('.') != -1)
                        IMAGENAME = IMAGENAME.Substring(0, IMAGENAME.IndexOf('.'));
                    imageNames.Add(IMAGENAME);
                }
            }
        }
        ImageLibrary = null;
        ImageLibrary = ImageManager.CreateRuntimeLibrary();
        if (ImageLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            // add images to mutableLibrary
            //foreach(Texture2D image in images)
            for (int i = 0; i < images.Count; i++)
            {
                Texture2D image = images[i];
                string name = imageNames[i];
                MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob(mutableLibrary, image, name, image.width / 300 * 0.0254f);
            }
        }
        ImageManager.referenceLibrary = ImageLibrary;
        ImageManager.enabled = true;
    }
    public void LoadCategoryList()
    {
        if (LoadedDocument == null)
            return;

        if (CategoryContent.childCount > 1)
            foreach (Transform child in CategoryContent)
                if (child.name != "Category-Template")
                    Destroy(child.gameObject);

        bool ADMIN = false;
        if (AdminController.GetInstance())
        {
            if (AdminController.GetInstance().isAdmin)
                ADMIN = true;
        }
        else
            ADMIN = true;

        // Check for amount of approved projects in each category
        for (int i = 0; i < LoadedDocument.categories.Length; i++)
        {
            DioramaCategory category = LoadedDocument.categories[i];
            int validProjects = 0;
            foreach (DioramaProject project in category.presentations)
                validProjects += (project.approved || ADMIN) ? 1 : 0;
            if (validProjects <= 0)
                continue;
            
            Transform button = Instantiate(CategoryButton);
            button.gameObject.SetActive(true);
            button.SetParent(CategoryContent);
            button.localScale = Vector3.one;
            button.localRotation = Quaternion.identity;
            button.GetComponentInChildren<Text>().text = category.name;
            int z = new int(); z = i;
            button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                StudentUIText.text = "Select from " + category.name;
                LoadStudentList(z); 
                //Observer_Events.CustomEvent("TOUCH")
                //    .AddParameter("assetName", "Category-Load-" + category.name)
                //    .AddParameter("startTime", Observer_Functions.GetCurrentTimeString())
                //    .EndParameters();
            });
        }
    }
    public void LoadStudentList(int value)
    {
        if (StudentContent.childCount > 1)
            foreach (Transform child in StudentContent)
                if (child.name != "Student-Template")
                    Destroy(child.gameObject);

        bool ADMIN = false;
        if (AdminController.GetInstance())
        {
            if (AdminController.GetInstance().isAdmin)
                ADMIN = true;
        } else
            ADMIN = true;
        
        // Check for approved projects
        for (int i = 0; i < LoadedDocument.categories[value].presentations.Length; i++)
        {
            DioramaProject project = LoadedDocument.categories[value].presentations[i];
            if (!project.approved && !ADMIN)
                continue;

            Transform button = Instantiate(StudentButton);
            button.gameObject.SetActive(true);
            button.SetParent(StudentContent);
            button.localScale = Vector3.one;
            button.localRotation = Quaternion.identity;
            button.GetComponentInChildren<Text>().text = project.title;
            int y = new int(); y = value; int z = new int(); z = i;
            button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                LoadProject(y, z);
                //Observer_Events.CustomEvent("TOUCH")
                //    .AddParameter("assetName", "Project-Load-" + project.title)
                //    .AddParameter("startTime", Observer_Functions.GetCurrentTimeString())
                //    .EndParameters();
            });
        }
    }

    public void LoadLastOpenedProject()
    {
        if (LoadedDocument == null || string.IsNullOrEmpty(TOKEN))
        {
            Debug.Log("NO LOADED TOKEN/DOCUMENT ERROR: NO/BAD CONNECTION?");
            return;
        }
        LoadProject(LastProject[0], LastProject[1]);
    }
    public void LoadProjectIfNotOpened(int Category, int Student)
    {
        if (LastProject[0] == Category && LastProject[1] == Student)
            return;
        ConfirmFirstDioramaSelection();
        SelectNewDioramaButton.SetActive(true);

        if (DioramaReferencePoint)
            foreach (Transform t in DioramaReferencePoint.transform)
                Destroy(t.gameObject);

        StartCoroutine(co_LoadProject(Category, Student));
    }
    public void LoadProject(int Category, int Student)
    {
        ConfirmFirstDioramaSelection();
        SelectNewDioramaButton.SetActive(true);

        if (DioramaReferencePoint)
            foreach (Transform t in DioramaReferencePoint.transform)
                Destroy(t.gameObject);

        StartCoroutine(co_LoadProject(Category, Student));
    }
    bool b_LoadProject = false;
    IEnumerator co_LoadProject(int Category, int Student)
    {
        if (b_LoadProject)
            yield break;
        b_LoadProject = true;
        LoadingScreen.SetActive(true);
        LoadedProject = new GameObject().transform;
        DioramaProject selectedProject = LoadedDocument.categories[Category].presentations[Student];
        LoadedProject.name = selectedProject.presentationId;
        LoadedProject.gameObject.SetActive(false);
        selectedProject.title = selectedProject.title.Replace(":", "");
        selectedProject.title = selectedProject.title.Replace("|", "");

        if (Category == 0 && Student == 0 && AgoraInterface.instance != null)
        {
            AgoraInterface.instance.AgoraPassword = selectedProject.presentationId;
            if (AdminController.Instance)
                if (!string.IsNullOrEmpty(AdminController.Instance.SSOAuthTOKEN))
                    AgoraInterface.instance.AgoraPasswordInputted = AgoraInterface.instance.AgoraPassword;
        }

        yield return StartCoroutine(CleanUp());

        Transform Analytics = Instantiate(ProjectAssets.Find("analytics"));
        Analytics.SetParent(LoadedProject);
        Analytics.gameObject.SetActive(true);
        Analytics.GetComponent<DioramaAnalytics>().ProjectName = selectedProject.title;

        // IF ANY ASSETS ARE NEW, DELETE OLD
        bool newAssets = false;
        for (int i = 0; i < selectedProject.items.Length; i++)
        {
            DioramaItem item = LoadedDocument.categories[Category].presentations[Student].items[i];
            if (!string.IsNullOrEmpty(item.imageUrl))
            {
                string fileName = item.imageUrl.Substring(item.imageUrl.LastIndexOf('/') + 1);
                fileName = LoadedDocument.categories[Category].name + "/" +
                    LoadedDocument.categories[Category].presentations[Student].title + "/" +
                    fileName;
                string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                newAssets = newAssets || !File.Exists(savePath);
            }
            if (!string.IsNullOrEmpty(item.videoUrl))
            {
                string fileName = item.videoUrl.Substring(item.videoUrl.LastIndexOf('/') + 1);
                fileName = LoadedDocument.categories[Category].name + "/" +
                    LoadedDocument.categories[Category].presentations[Student].title + "/" +
                    fileName;
                string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                newAssets = newAssets || !File.Exists(savePath);
            }
            if (item.multiImage != null)
            {
                foreach (string image in item.multiImage)
                {
                    if (!string.IsNullOrEmpty(image))
                    {
                        string fileName = image.Substring(image.LastIndexOf('/') + 1);
                        fileName = LoadedDocument.categories[Category].name + "/" +
                            LoadedDocument.categories[Category].presentations[Student].title + "/" +
                            fileName;
                        string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                        newAssets = newAssets || !File.Exists(savePath);
                    }
                }
            }
            if (item.modelUrl != null)
            {
                DioramaModelPiece[] models = item.modelUrl;
                if (models.Length > 0)
                {
                    string fileName = item.itemName;
                    fileName = LoadedDocument.categories[Category].name + "/" +
                        LoadedDocument.categories[Category].presentations[Student].title + "/" +
                        fileName;
                    string modelDir = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                    foreach (DioramaModelPiece piece in models)
                        newAssets = newAssets || CheckExistDioramaModelPiece(piece, modelDir);

                }
            }
            if (newAssets)
                break;
        }
        if (newAssets)
        {
            string directory = Application.persistentDataPath + "/" + LoadedDocument.categories[Category].name + "/" +
                        LoadedDocument.categories[Category].presentations[Student].title;
            if (Directory.Exists(directory))
                DeleteDirectory(directory);
            Debug.Log("DELETED: " + directory);
        }

        //ImageLibrary = null; // Maybe we can destroy the previous markers?
        //ImageLibrary = ImageManager.CreateRuntimeLibrary();
        //if (ImageLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        //{
        //    // add images to mutableLibrary
        //    MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob(mutableLibrary, image, selectedProject.title, 0.3f);
        //}

        for (int i = 0; i < selectedProject.items.Length; i++)
        {

            DioramaItem item = LoadedDocument.categories[Category].presentations[Student].items[i];
            if (item.itemType == "video" && item.video360)
                item.itemType = "360video";

            if (item.itemType == "quad" || item.itemType == "frame" || item.itemType == "shadowbox" || item.itemType == "360video" || item.itemType == "video" || item.itemType == "single_image" || item.itemType == "multi_images" || item.itemType == "text" || item.itemType == "email_submission")
            {
                Transform prefab = Instantiate(ProjectAssets.Find(item.itemType));
                prefab.SetParent(LoadedProject);
                prefab.gameObject.SetActive(true);

                prefab.localPosition = new Vector3(item.itemPositionX, item.itemPositionY, item.itemPositionZ);
                prefab.localRotation = Quaternion.Euler(item.itemRotationX, item.itemRotationY, item.itemRotationZ);
                prefab.localScale = new Vector3(item.itemScaleX, item.itemScaleY, item.itemScaleZ);
                if (item.itemType == "360video")
                    prefab.localScale = new Vector3(item.itemScaleX, item.itemScaleX, item.itemScaleX);

                if (item.itemType == "frame" || item.itemType == "quad")
                {
                    Color c = new Color(); ColorUtility.TryParseHtmlString(item.itemColor, out c);
                    if (prefab.GetComponentInChildren<MeshRenderer>() != null)
                    {
                        prefab.GetComponentInChildren<MeshRenderer>().material.color = c;
                    }
                }
                if (item.imageUrl != null)
                {
                    string url = item.imageUrl;
                    string fileName = item.imageUrl.Substring(item.imageUrl.LastIndexOf('/') + 1);
                    prefab.name = fileName;
                    fileName = LoadedDocument.categories[Category].name + "/" +
                                             LoadedDocument.categories[Category].presentations[Student].title + "/" +
                        fileName;
                    Debug.Log(fileName + " ::: " + url);
                    string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

                    if (!item.imageUrl.Substring(item.imageUrl.Length - 5).Contains("."))
                    {
                        fileName += ".png";
                        savePath += ".png";
                    }

                    Texture2D image = new Texture2D(1, 1);
                    float aspectRatio = 1.0f;
                    if (!File.Exists(savePath))
                    {
                        using (UnityWebRequest www = UnityWebRequest.Get(url))
                        {
                            www.redirectLimit = 10;
                            www.chunkedTransfer = false;
                            www.SendWebRequest();
                            float totalTime = 0;
                            while ((!www.isDone || !www.downloadHandler.isDone) && totalTime < 12)
                            {
                                yield return new WaitForSeconds(1f);
                                totalTime++;
                            }
                            //yield return www.SendWebRequest();
                            if (!string.IsNullOrEmpty(www.error))
                            {
                                Debug.Log(www.error);
                                image = Error;
                            }
                            else if (totalTime < 12)
                            {
                                Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
                                File.WriteAllBytes(savePath, www.downloadHandler.data);
                                byte[] bytes = File.ReadAllBytes(savePath);
                                image.LoadImage(bytes);
                            }
                            if (www != null && !www.isDone)
                            {
                                NetworkIssueAlpha = 2;
                                www.Abort();
                            }
                        }
                    }
                    else
                    {
                        byte[] bytes = File.ReadAllBytes(savePath);
                        image.LoadImage(bytes);

                    }
                    if (item.videoUrl != null)
                        prefab.GetComponent<ApplyToMaterial>()._defaultTexture = image;
                    prefab.GetComponent<MeshRenderer>().material.mainTexture = image;
                    aspectRatio = (float)image.width / (float)image.height;
                    prefab.localScale = new Vector3(item.itemScaleY * aspectRatio, item.itemScaleY, item.itemScaleZ);
                }
                if (item.videoUrl != null)
                {
                    prefab.Find("PlayOrStop").GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector3(100 * 1 / prefab.localScale.x / DIORAMASCALE, 100 * 1 / prefab.localScale.y / DIORAMASCALE, 1);

                    string url = item.videoUrl;
                    string fileName = item.videoUrl.Substring(item.videoUrl.LastIndexOf('/') + 1);
                    prefab.name = fileName;
                    fileName = LoadedDocument.categories[Category].name + "/" +
                        LoadedDocument.categories[Category].presentations[Student].title + "/" +
                        fileName;
                    Debug.Log(fileName + " ::: " + url);
                    string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

                    if (!item.videoUrl.Substring(item.videoUrl.Length - 5).Contains("."))
                    {
                        fileName += ".mp4";
                        savePath += ".mp4";
                    }

                    Texture2D image = new Texture2D(1, 1);
                    if (!System.IO.File.Exists(savePath))
                    {
                        using (UnityWebRequest www = UnityWebRequest.Get(url))
                        {
                            www.redirectLimit = 10;
                            www.chunkedTransfer = false;
                            www.SendWebRequest();
                            float totalTime = 0;
                            while ((!www.isDone || !www.downloadHandler.isDone) && totalTime < 50)
                            {
                                yield return new WaitForSeconds(1f);
                                totalTime++;
                            }
                            //yield return www.SendWebRequest();
                            if (!string.IsNullOrEmpty(www.error))
                            {
                                Debug.Log(www.error);
                            }
                            else if (totalTime < 50)
                            {
                                Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
                                File.WriteAllBytes(savePath, www.downloadHandler.data);
                            }
                            if (www != null && !www.isDone)
                            {
                                NetworkIssueAlpha = 2;
                                www.Abort();
                            }
                        }
                    }
                    // AUTO OPENER FOR PRECACHING
                    Transform videoLoader = Instantiate(ProjectAssets.Find("videoloader"));
                    videoLoader.GetComponent<MediaPlayer>().m_VideoPath = fileName;
                    videoLoader.GetComponent<DestroyWith>().With = LoadedProject.gameObject;
                    videoLoader.gameObject.SetActive(true);

                    prefab.GetComponent<MediaPlayer>().m_VideoPath = fileName;
                    if (item.autoplayVideo && prefab.GetComponentInChildren<DioramaVideo>())
                        prefab.GetComponentInChildren<DioramaVideo>().autoplay = true;
                    if (item.autoplayVideo && prefab.GetComponentInChildren<Diorama360Video>())
                        prefab.GetComponentInChildren<Diorama360Video>().autoplay = true;
                    if (item.loopVideo)
                        prefab.GetComponent<MediaPlayer>().m_Loop = true;
                    if (item.transparentVideo)
                        prefab.GetComponent<MediaPlayer>().m_AlphaPacking = AlphaPacking.LeftRight;

                    if (prefab.GetComponent<ApplyToMaterial>())
                        prefab.GetComponent<ApplyToMaterial>()._material = prefab.GetComponent<MeshRenderer>().material;
                    prefab.GetChild(0).gameObject.SetActive(true);
                }
                if (item.multiImage != null)
                {

                    prefab.name = "Multi_Image_Object";

                    foreach (string multiImageUrl in item.multiImage)
                    {
                        Transform newImagePrefab = Instantiate(ProjectAssets.Find("single_image"), prefab);
                        string imageName = multiImageUrl.Substring(multiImageUrl.LastIndexOf('/') + 1);
                        newImagePrefab.name = imageName;
                        imageName = LoadedDocument.categories[Category].name + "/" +
                                                        LoadedDocument.categories[Category].presentations[Student].title + "/" + imageName;
                        Debug.Log(imageName + " ::: " + multiImageUrl);
                        string multiImageSavePath = string.Format("{0}/{1}", Application.persistentDataPath, imageName);

                        if (!multiImageUrl.Substring(multiImageUrl.Length - 5).Contains("."))
                        {
                            imageName += ".png";
                            multiImageSavePath += ".png";
                        }

                        Texture2D newMultiImage = new Texture2D(1, 1);
                        float aspectRatio = 1.0f;
                        if (!System.IO.File.Exists(multiImageSavePath))
                        {

                            using (UnityWebRequest www = UnityWebRequest.Get(multiImageUrl))
                            {
                                www.redirectLimit = 10;
                                www.chunkedTransfer = false;
                                www.SendWebRequest();
                                float totalTime = 0;
                                while ((!www.isDone || !www.downloadHandler.isDone) && totalTime < 12)
                                {
                                    yield return new WaitForSeconds(1f);
                                    totalTime++;
                                }
                                //yield return www.SendWebRequest();
                                if (!string.IsNullOrEmpty(www.error))
                                {
                                    Debug.Log(www.error);
                                    newMultiImage = Error;
                                }
                                else if (totalTime < 12)
                                {
                                    Directory.CreateDirectory(multiImageSavePath.Substring(0, multiImageSavePath.LastIndexOf('/')));
                                    File.WriteAllBytes(multiImageSavePath, www.downloadHandler.data);
                                    byte[] bytes = File.ReadAllBytes(multiImageSavePath);
                                    newMultiImage.LoadImage(bytes);
                                }
                                if (www != null && !www.isDone)
                                {
                                    NetworkIssueAlpha = 2;
                                    www.Abort();
                                }
                            }
                        }
                        else
                        {
                            byte[] bytes = File.ReadAllBytes(multiImageSavePath);
                            newMultiImage.LoadImage(bytes);
                        }
                        newImagePrefab.GetComponent<MeshRenderer>().material.mainTexture = newMultiImage;
                        newImagePrefab.gameObject.SetActive(true);
                        prefab.localScale = new Vector3(1, 1, 1);

                        //Calculate aspectr ratio depending on portrait or landscape
                        if ((float)newMultiImage.height > (float)newMultiImage.width)
                        {
                            aspectRatio = (float)newMultiImage.width / (float)newMultiImage.height;
                            newImagePrefab.localScale = new Vector3(item.itemScaleY * aspectRatio, item.itemScaleY, item.itemScaleZ);
                        }
                        else{
                            aspectRatio = (float)newMultiImage.height / (float)newMultiImage.width;
                            newImagePrefab.localScale = new Vector3(item.itemScaleX, item.itemScaleX * aspectRatio, item.itemScaleZ);
                        }
                         
                    }
                    prefab.GetComponent<DioramaMultiImages>().GoToFirstPicture();

                }
                if (item.textData != null && item.titleData != null)
                {
                    float xts = 1, yts = 1;
                    if (item.itemScaleX > item.itemScaleY)
                        yts = item.itemScaleX / item.itemScaleY;
                    else
                        xts = item.itemScaleY / item.itemScaleX;
                    foreach (DioramaTextData title in item.titleData)
                    {
                        GameObject go = Instantiate(prefab.GetChild(0).gameObject); //new GameObject(title.label);
                        go.SetActive(true);
                        go.name = title.label;
                        go.transform.SetParent(prefab);
                        go.transform.localPosition = new Vector3(0, 0, 0);
                        go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        go.transform.localScale = new Vector3(xts, yts, 1);
                        TextMeshPro tm = go.GetComponent<TextMeshPro>();
                        tm.fontSize = 0.45f;
                        //tm.fontStyle = FontStyles.Bold;
                        tm.text = title.text;
                        tm.color = Color.black;
                        tm.alignment = TextAlignmentOptions.Left;
                        tm.enableWordWrapping = true;
                    }

                    foreach (DioramaTextData text in item.textData)
                    {
                        Debug.Log(string.Format("Label: {0}, Name: {1}", text.label, text.text));
                        GameObject go = Instantiate(prefab.GetChild(0).gameObject); //new GameObject(title.label);
                        go.SetActive(true);
                        go.name = text.label;//GameObject go = new GameObject(text.label);
                        go.transform.SetParent(prefab);
                        go.transform.localPosition = new Vector3(0, 0, 0);
                        go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        go.transform.localScale = new Vector3(xts, yts, 1);
                        TextMeshPro tm = go.GetComponent<TextMeshPro>();
                        tm.fontSize = 0.3f;
                        if (text.label == "Team Club Participants" || text.label.ToLower().Contains("participant"))
                            tm.fontStyle = FontStyles.Bold;
                        tm.text = text.text;
                        tm.color = Color.black;
                        tm.alignment = TextAlignmentOptions.TopLeft;
                        tm.enableWordWrapping = true;
                    }
                }
                if (item.feedbackEmail != null)
                {
                    prefab.GetComponent<MailController>().SendToAddress = item.feedbackEmail[0];
                }
                if (item.webLinkUrl != null && item.videoUrl == null)
                {
                    Transform weblink = prefab.GetChild(0);
                    weblink.gameObject.SetActive(true);
                    weblink.name = item.webLinkUrl;
                    //float smaller = prefab.localScale.x > prefab.localScale.y ? prefab.localScale.y : prefab.localScale.x;
                    weblink.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector3(50 * 1 / prefab.localScale.x / DIORAMASCALE, 50 * 1 / prefab.localScale.y / DIORAMASCALE, 1);
                }
            }
            else if (item.itemType == "3dobject" && item.modelUrl != null)
            {
                DioramaModelPiece[] models = item.modelUrl;
                if (models.Length > 0)
                {
                    string fileName = item.itemName;
                    //prefab.name = fileName;
                    fileName = LoadedDocument.categories[Category].name + "/" +
                        LoadedDocument.categories[Category].presentations[Student].title + "/" +
                        fileName;
                    string modelDir = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
                    foreach (DioramaModelPiece piece in models)
                        yield return DownloadDioramaModelPiece(piece, modelDir);

                    foreach (string file in Directory.GetFiles(modelDir))
                    {
                        if (file.Contains(".obj"))
                        {
                            Transform prefab = new OBJLoader().Load(file).transform;
                            prefab.SetParent(LoadedProject);
                            prefab.gameObject.SetActive(true);
                            prefab.localPosition = new Vector3(item.itemPositionX, item.itemPositionY, item.itemPositionZ);
                            prefab.localRotation = Quaternion.Euler(item.itemRotationX, item.itemRotationY, item.itemRotationZ);
                            prefab.localScale = new Vector3(item.itemScaleX, item.itemScaleY, item.itemScaleZ);
                        }
                    }
                }
            }
        }
        LoadingScreen.SetActive(false);
        LastProject[0] = Category; LastProject[1] = Student;
        b_LoadProject = false;

        System.Random random = new System.Random();
        string username = "AVE" + random.Next(1000, 9999);
        if (!string.IsNullOrEmpty(Nickname.text) || !string.IsNullOrEmpty(Nickname2.text))
            username = Nickname.text + Nickname2.text;
        AgoraInterface.JoinRoomID(LoadedDocument.categories[Category].presentations[Student].presentationId, username);
    }

    private IEnumerator CleanUp()
    {
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    bool CheckExistDioramaModelPiece(DioramaModelPiece piece, string directory)
    {
        bool add = false;
        if (Directory.Exists(directory))
            return add;
        if (piece.children != null)
            if (piece.children.Length > 0)
                foreach (DioramaModelPiece p in piece.children)
                    add = CheckExistDioramaModelPiece(p, directory + piece.path.Substring(piece.path.LastIndexOf('/')));

        string savePath = string.Format("{0}/{1}", directory, piece.path.Substring(piece.path.LastIndexOf("/") + 1));
        return add || !File.Exists(savePath);
    }

    IEnumerator DownloadDioramaModelPiece(DioramaModelPiece piece, string directory)
    {
        Directory.CreateDirectory(directory);
        if (piece.children != null)
            if (piece.children.Length > 0)
                foreach (DioramaModelPiece p in piece.children)
                    yield return DownloadDioramaModelPiece(p, directory + piece.path.Substring(piece.path.LastIndexOf('/')));

        Debug.Log("3DMP: " + piece.path.Substring(piece.path.LastIndexOf("/") + 1) + " ::: " + piece.path);
        string savePath = string.Format("{0}/{1}", directory, piece.path.Substring(piece.path.LastIndexOf("/") + 1));
        if (!File.Exists(savePath))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(piece.path))
            {
                www.redirectLimit = 10;
                www.chunkedTransfer = false;
                www.SendWebRequest();
                float totalTime = 0;
                while ((!www.isDone || !www.downloadHandler.isDone) && totalTime < 30)
                {
                    yield return new WaitForSeconds(1f);
                    totalTime++;
                }
                //yield return www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                    Debug.Log(www.error);
                else if (totalTime < 30)
                    File.WriteAllBytes(savePath, www.downloadHandler.data);
                if (www != null && !www.isDone)
                {
                    NetworkIssueAlpha = 2;
                    www.Abort();
                }
            }
        }
    }
    IEnumerator ChangeTMPFont(TextMeshPro text)
    {
        while (!text.gameObject.activeInHierarchy)
        {
            Debug.Log("TMP ::: Waiting for " + text.gameObject.name + "'s activation...");
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("TMP ::: " + text.gameObject.name + " activated! Changing font..");
        text.font = GlobalTMPFont;
        text.fontSharedMaterial = GlobalTMPFont.material;
    }

    public void CheckInstantiateDiorama()
    {
        if (SelectionScreenOpen)
            return;
        if (!DioramaReferencePoint)
            return;
        if (LoadedProject == null)
            return;
        if (DioramaReferencePoint.transform.childCount == 0)
        {
            Transform t = Instantiate(LoadedProject);
            t.name = LoadedProject.name;
            t.gameObject.SetActive(true);
            t.SetParent(DioramaReferencePoint.transform);

            t.localPosition = Vector3.zero;
            t.LookAt(MainCamera.transform);
            t.localRotation = Quaternion.Euler(0, t.localRotation.eulerAngles.y + 180, 0);
            t.localScale = Vector3.one * DIORAMASCALE;
        }
    }

    public void SelectNewDiorama()
    {
        if (LoadedDocument == null || string.IsNullOrEmpty(TOKEN))
        {
            Debug.Log("NO LOADED TOKEN/DOCUMENT ERROR: NO/BAD CONNECTION?");
            return;
        }
        //AgoraInterface.LeaveRoom();
        if (DioramaReferencePoint)
            if (DioramaReferencePoint.transform.childCount > 0)
                Destroy(DioramaReferencePoint.transform.GetChild(0).gameObject);
        Destroy(LoadedProject.gameObject);
        LoadedProject = null;
    }

    public void ConfirmFirstDioramaSelection()
    {
        CategoryBGFullscreen.SetActive(false);
        CategoryBGWindowed.SetActive(true);
        StudentBGFullscreen.SetActive(false);
        StudentBGWindowed.SetActive(true);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Diorama Reference Point")]
    public GameObject DioramaReferencePointREADONLY;
    public static GameObject DioramaReferencePoint;

    public bool PlaceReferencePointPhase = false;

    public Transform DioramaReferenceVisual;
    private Transform t_DioramaReferenceVisual;
    public Transform DioramaReferencePreVisual;
    private Transform t_DioramaReferencePreVisual;
    public GameObject LoadingVisual;

    bool ConfirmPlaceReferencePoint = false;
    public void StartPlaceReferencePoint()
    {
        if (DioramaReferencePoint != null)
            return;
        PlaceReferencePointPhase = true;
        ConfirmPlaceReferencePoint = false;
        t_DioramaReferencePreVisual = Instantiate(DioramaReferencePreVisual);
        t_DioramaReferencePreVisual.gameObject.SetActive(false);
    }
    
    public void ValuePlaceReferencePoint(bool value)
    {
        ConfirmPlaceReferencePoint = value;
        if (!value)
        {
            PlaceReferencePointPhase = false;
            Destroy(t_DioramaReferencePreVisual.gameObject);
            t_DioramaReferencePreVisual = null;
        } else
        {

        }
    }
    
    public LayerMask TransformReferencePointLayerMask;
    public int TransformReferencePointMode = -1;
    // Mode
    // 0 - Translate
    // 1 - Rotate
    // 2 - Scale

    int TransformReferencePointAxis = -1;
    // Axes
    // 0 - X
    // 1 - Y
    // 2 - Z
    public void TranslateReferencePointAlongAxis(int Axis)
    {
        TransformReferencePointMode = 0;
        TransformReferencePointAxis = Axis;
    }
    public void RotateReferencePointAlongAxis(int Axis)
    {
        TransformReferencePointMode = 1;
        TransformReferencePointAxis = Axis;
    }
    public void ScaleReferencePointAlongAxis(int Axis)
    {
        TransformReferencePointMode = 2;
        TransformReferencePointAxis = Axis;
    }

    Vector2 prevMousePos;
    void TransformReferencePointUpdate()
    {
        if (DioramaReferencePoint == null)
            return;

        Touch touch;
        Vector2 mousePos = Vector2.zero;

        bool FirstTouch = false;
        bool touching = false;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touching = true;
            mousePos = touch.position;
            FirstTouch = touch.phase == TouchPhase.Began;
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            touching = true;
            mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            FirstTouch = Input.GetMouseButtonDown(0);
        }
        if (!touching)
        {
            TransformReferencePointMode = -1;
            TransformReferencePointAxis = -1;
            prevMousePos = Vector2.zero;
        }

        if (FirstTouch)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 20, TransformReferencePointLayerMask);
            if (!hit.collider)
                return;
            if (!hit.collider.GetComponent<Button>())
                return;
            Button button = hit.collider.GetComponent<Button>();

            button.onClick.Invoke();
            //Debug.Log(button.name);
        }

        float diffX = mousePos.x - prevMousePos.x;
        float diffY = mousePos.y - prevMousePos.y;
        double angle = MainCamera.transform.rotation.eulerAngles.y * Math.PI / 180.0;
        float horiz = Convert.ToSingle(Math.Cos(angle) * diffX + Math.Sin(angle) * diffY);
        float vert = Convert.ToSingle(Math.Cos(angle) * diffY + Math.Sin(angle) * -diffX);

        if (touching && prevMousePos != null && prevMousePos != Vector2.zero
            && TransformReferencePointAxis >= 0 && TransformReferencePointMode == 0)
        {
            if (TransformReferencePointAxis == 0)
                DioramaReferencePoint.transform.position += new Vector3(0.0005f * horiz, 0, 0);
            if (TransformReferencePointAxis == 1)
                DioramaReferencePoint.transform.position += new Vector3(0, 0.0005f * diffY, 0);
            if (TransformReferencePointAxis == 2)
                DioramaReferencePoint.transform.position += new Vector3(0, 0, 0.0005f * vert);
        }
        if (touching && prevMousePos != null && prevMousePos != Vector2.zero
            && TransformReferencePointAxis >= 0 && TransformReferencePointMode == 1)
        {
            if (TransformReferencePointAxis == 0)
                DioramaReferencePoint.transform.Rotate(new Vector3(vert, 0, 0), Space.Self);
            if (TransformReferencePointAxis == 1)
                DioramaReferencePoint.transform.Rotate(new Vector3(0, horiz, 0), Space.Self);
            if (TransformReferencePointAxis == 2)
                DioramaReferencePoint.transform.Rotate(new Vector3(0, 0, vert), Space.Self);
        }
        if (touching && prevMousePos != null && prevMousePos != Vector2.zero
            && TransformReferencePointAxis >= 0 && TransformReferencePointMode == 2)
        {
            DioramaReferencePoint.transform.localScale *= 1 + (0.0005f * horiz);
        }
        if (mousePos != Vector2.zero)
            prevMousePos = mousePos;
    }


    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [Header("Channels")]
    public Text HeaderChannelName;

    void ChannelsDioramaUpdate()
    {
        if (AdminController.Instance != null)
        {
            if (AdminController.Instance.CHANNELID > 0)
            {
                HeaderChannelName.text = AdminController.Instance.CURRENTCHANNEL.name;
            } else
            {
                HeaderChannelName.text = "";
            }
        } else
        {
            HeaderChannelName.text = "";
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void TakeSnapshot()
    {
        var culture = new CultureInfo("en-US");
        string name = DateTime.Now.ToString(culture).Replace("/", "-").Replace(":", "-");

        string file = Path.Combine(Application.persistentDataPath, "Diorama-Shot-" + name + ".png");
#if !UNITY_EDITOR
        ScreenCapture.CaptureScreenshot("Diorama-Shot-" + name + ".png");
#else
        ScreenCapture.CaptureScreenshot(file);
#endif
        StartCoroutine(co_TakeSnapshot(file));
    }
    IEnumerator co_TakeSnapshot(string file)
    {
        yield return new WaitForSeconds(1f);
        NativeGallery.SaveImageToGallery(file,
            "Avenues", "Diorama-Shot-" + name + ".png");
        new NativeShare().AddFile(file).SetSubject("Avenues Screenshot").Share();
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Diorama Backend")]
    public string TOKEN = "";

    [Serializable]
    class DioramaAuthentication
    {
        [JsonProperty]
        public string username;
        [JsonProperty]
        public string password;
        [JsonProperty]
        public bool issueJWT;
    }

    [Serializable]
    public class DioramaAuthenticationTokenResponse
    {
        [JsonProperty]
        public string token;
        [JsonProperty]
        public string user_email;
        [JsonProperty]
        public string user_nicename;
        [JsonProperty]
        public string user_display_name;
    }

    public void RequestToken()
    {
        StartCoroutine(co_RequestToken());
    }
    IEnumerator co_RequestToken()
    {
        int TOKEN_attempts = 0;
        string uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/login";
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        DioramaAuthentication auth = new DioramaAuthentication();
        //auth.username = "arapp";
        auth.username = "testfaculty";
        //auth.password = "sXAuW$zWAc4W9euu1n";
        auth.password = "##82vB3Y4W##";
        auth.issueJWT = true;

        string jsonInput = JsonConvert.SerializeObject(auth, settings);

        byte[] body = Encoding.UTF8.GetBytes(jsonInput);

        for (int i = 0; i < 3; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");

            request.redirectLimit = 10;
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();

            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 15)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                TOKEN_attempts++;
                Debug.Log("REQUESTTOKEN NETWORK ERROR : " + request.error);
            }
            else if (totalTime < 15)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTTOKEN : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    TOKEN_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM AUTHENTICATION TOKEN attempt#" + TOKEN_attempts + " : " + response);
                    Debug.Log("EMPTY RESPONSE: RESTARTING SCENE");
                    //SceneManager.LoadScene("Diorama");
                    yield break;
                }

                DioramaAuthenticationTokenResponse tokenResponse = JsonConvert.DeserializeObject<DioramaAuthenticationTokenResponse>(response);
                TOKEN = tokenResponse.token;
                Debug.Log("Request Token is: " + TOKEN);
                break;
            }

            TOKEN_attempts++;
            if (i == 2)
            {
                Debug.Log("CONNECTION FAILED WHILE OBTAINING TOKEN? RESTARTING SCENE attempt#" + TOKEN_attempts);
                //SceneManager.LoadScene("Diorama");

            } else
            {
                //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM AUTHENTICATION TOKEN attempt#" + TOKEN_attempts);
            }
        }
        RequestDioramaByCategory();
    }

    // -1 = Not Running
    // 0 = Evaluating
    // 1 = Error
    // 100 = Requested Successfully
    private int RequestDioramaByCategoryReturnCode = -1;
    public void RequestDioramaByCategory()
    {
        RequestDioramaByCategoryReturnCode = -1;
        if (TOKEN == "")
            RequestToken();
        StartCoroutine(co_RequestDioramaByCategory());
    }
    IEnumerator co_RequestDioramaByCategory()
    {
        int CATEGORY_attempts = 0;
        RequestDioramaByCategoryReturnCode = 0;

        string uri = "";

        if (AdminController.Instance.CHANNELID < 0)
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/presentations-by-category";
        else
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channel/id/" + AdminController.Instance.CHANNELID;

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = true,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        for (int i = 0; i < 3; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri);

            request.redirectLimit = 10;
            request.SetRequestHeader("Authentication", "Bearer " + TOKEN);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();

            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 15)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                CATEGORY_attempts++;
                Debug.Log("REQUESTESSION NETWORK ERROR : " + request.error);
                RequestDioramaByCategoryReturnCode = 1;
            }
            else if (totalTime < 15)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTESSION : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    CATEGORY_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CATEGORIES attempt#" + CATEGORY_attempts + " : " + response);
                    Debug.Log("EMPTY RESPONSE: RESTARTING SCENE");
                    //SceneManager.LoadScene("Diorama");
                    yield break;
                }
                RequestDioramaByCategoryReturnCode = 100;

                LoadedDocument = JsonConvert.DeserializeObject<DioramaDocument>(response, settings);
                LoadingScreen.SetActive(false);

                DownloadWebinarImageTargets();
                LoadCategoryList();
                LoadProject(0, 0);
                break;

                //DrawlightSession save = JsonConvert.DeserializeObject<DrawlightSession>(response, settings);
                //Debug.Log(save.drawlight_session_id + ":" + Encoding.UTF8.GetString(save.drawlight_session));

                //DrawMenuManager.DrawSavedata(DrawlightSession.SessionToSavedata(save));
            }
            if (i == 2)
            {
                CATEGORY_attempts++;
                Debug.Log("CONNECTION FAILED WHILE OBTAINING CATEGORIES? RESTARTING SCENE attempt#" + CATEGORY_attempts);
                //SceneManager.LoadScene("Diorama");
            }
            else
            {
                CATEGORY_attempts++;
                //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CATEGORIES attempt#" + CATEGORY_attempts);
            }
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        DioramaReferencePointREADONLY = DioramaReferencePoint;
    }

    public static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }

    public void JoinAgoraRoomProjectId()
    {
        if (string.IsNullOrEmpty(LoadedProject.name))
            return;

        System.Random random = new System.Random();
        string username = "AVE" + random.Next(1000, 9999);
        if (!string.IsNullOrEmpty(Nickname.text) || !string.IsNullOrEmpty(Nickname2.text))
            username = Nickname.text + Nickname2.text;
        AgoraInterface.JoinRoomID(LoadedProject.name, username, false);
    }
    
    public void SearchAndStop360Videos()
    {
        Diorama360Video[] videos = FindObjectsOfType<Diorama360Video>();

        foreach (Diorama360Video v in videos)
        {
            if (v.Media.Control.IsPlaying())
            {
                v.UnpauseAfterExit360();
                v.PlayOrStop();
            }
        }
    }
    public void SearchAndStopAllVideosExcept(Diorama360Video video)
    {
        Diorama360Video[] v360s = FindObjectsOfType<Diorama360Video>();
        foreach (Diorama360Video v in v360s)
        {
            if (v.Media.Control.IsPlaying() && v != video)
                v.PlayOrStop();
        }
        DioramaVideo[] vs = FindObjectsOfType<DioramaVideo>();
        foreach (DioramaVideo v in vs)
        {
            if (v.Media.Control.IsPlaying())
                v.PlayOrStop();
        }
    }
    public void SearchAndStopAllVideosExcept(DioramaVideo video)
    {
        Diorama360Video[] v360s = FindObjectsOfType<Diorama360Video>();
        foreach (Diorama360Video v in v360s)
        {
            if (v.Media.Control.IsPlaying())
                v.PlayOrStop();
        }
        DioramaVideo[] vs = FindObjectsOfType<DioramaVideo>();
        foreach (DioramaVideo v in vs)
        {
            if (v.Media.Control.IsPlaying() && v != video)
                v.PlayOrStop();
        }
    }
}
