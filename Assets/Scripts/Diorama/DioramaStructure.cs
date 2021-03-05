using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[Serializable]
public class DioramaDocument
{
    [JsonProperty]
    public DioramaChannel channel;
    [JsonProperty]
    public DioramaProject tutorialPresentation;
    [JsonProperty]
    public DioramaCategory[] categories;
}

[Serializable]
public class DioramaChannels
{
    [JsonProperty]
    public DioramaChannel[] channels;
}

[Serializable]
public class DioramaChannel
{
    [JsonProperty]
    public int id;
    [JsonProperty]
    public string name;
    [JsonProperty]
    public string status;
    [JsonProperty]
    public string description;
    [JsonProperty]
    public string color;
    [JsonProperty]
    public string textColor;
    [JsonProperty]
    public string buttonGraphic;
}

[Serializable]
public class DioramaCategory
{
    [JsonProperty]
    public string name;
    [JsonProperty]
    public DioramaProject[] presentations;
}

[Serializable]
public class DioramaProject
{
    [JsonProperty]
    public string presentationId;
    [JsonProperty]
    public string title;
    [JsonProperty]
    public string subtitle;
    [JsonProperty]
    public string trackedImage;
    [JsonProperty]
    public bool approved;
    [JsonProperty]
    public DioramaItem[] items;
}

[Serializable]
public class DioramaItem
{
    [JsonProperty]
    public string itemName;
    [JsonProperty]
    public string itemType;
    [JsonProperty]
    public float itemRotationX;
    [JsonProperty]
    public float itemRotationY;
    [JsonProperty]
    public float itemRotationZ;
    [JsonProperty]
    public float itemScaleX;
    [JsonProperty]
    public float itemScaleY;
    [JsonProperty]
    public float itemScaleZ;
    [JsonProperty]
    public float itemPositionX;
    [JsonProperty]
    public float itemPositionY;
    [JsonProperty]
    public float itemPositionZ;
    [JsonProperty]
    public string itemColor;
    [JsonProperty]
    public DioramaTextData[] textData;
    [JsonProperty]
    public string[] feedbackEmail;
    [JsonProperty]
    public string imageUrl;
    [JsonProperty]
    public DioramaModelPiece[] modelUrl;
    [JsonProperty]
    public string videoUrl;
    [JsonProperty]
    public string webLinkUrl;
    [JsonProperty]
    public string[] multiImage;
    [JsonProperty]
    public DioramaTextData[] titleData;
    [JsonProperty]
    public string modelUrlStatus;
    [JsonProperty]
    public bool video360;
    [JsonProperty]
    public bool transparentVideo;
    [JsonProperty]
    public bool autoplayVideo;
    [JsonProperty]
    public bool loopVideo;
}

[Serializable]
public class DioramaTextData
{
    [JsonProperty]
    public string label;
    public string text;
}

[Serializable]
public class DioramaModelPiece
{
    [JsonProperty]
    public string type;
    [JsonProperty]
    public string path;
    [JsonProperty]
    public DioramaModelPiece[] children;
}

public class DioramaStructure : MonoBehaviour
{
}
