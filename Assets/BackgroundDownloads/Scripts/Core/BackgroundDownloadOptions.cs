using System.IO;
using UnityEngine;

/// <summary>
/// Options for customizing the background download process.
/// </summary>
public class BackgroundDownloadOptions
{
#if UNITY_EDITOR
    public static readonly string DEFAULT_DOWNLOAD_PATH = Application.dataPath+"/download/";//Application.persistentDataPath;
#else
        public static readonly string DEFAULT_DOWNLOAD_PATH = Path.Combine(Application.persistentDataPath , "Avenues");
#endif
    private const string DEFAULT_DESCRIPTION = "Downloading...";

    /// <summary>
    /// The URL for the data that should be downloaded.
    /// </summary>
    public string URL { get; private set; }

    private string destinationPath;

    /// <summary>
    /// The path to save the downloaded data in, once the download completes.
    /// </summary>
	public string DestinationPath
	{
		get { return destinationPath ?? GetDefaultDestinationPath(); }
	}

	private string GetDefaultDestinationPath()
	{
        string vidSavePath = "";
#if UNITY_EDITOR
        vidSavePath = Path.Combine(Application.dataPath, "download");


#else
          vidSavePath = Path.Combine(Application.persistentDataPath,"Avenues");
#endif
        vidSavePath = Path.Combine(vidSavePath, ".mp4");

        return vidSavePath;

//        Debug.Log(Path.Combine(DEFAULT_DOWNLOAD_PATH, DownloadController.GetInstance().mCurrentVideo + ".mp4"));
  //      return Path.Combine(DEFAULT_DOWNLOAD_PATH, DownloadController.GetInstance().mCurrentVideo + ".mp4");// Path.GetFileName(URL));	
	}

	private string title;

    /// <summary>
    /// The title to display for the ongoing download in the notification area.
    /// </summary>
	public string Title
	{
		get { return title ?? GetDefaultTitle(); }
	}

	private string GetDefaultTitle()
	{
		return Application.productName;
	}

	private string description;
    
    /// <summary>
    /// The description to display for the ongoing download in the notification area.
    /// </summary>
    public string Description
	{
		get { return description ?? DEFAULT_DESCRIPTION; }
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundDownloadOptions"/> class.
	/// </summary>
	public BackgroundDownloadOptions(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new System.ArgumentException("url should not be null or empty!");
		}

        System.Uri uri;

        // Verify that the passed string can be parsed as a URL
        if (!System.Uri.TryCreate(url, System.UriKind.Absolute, out uri))
        {
            throw new System.ArgumentException("url is not a valid URL address!");
        }

        this.URL = url;
	}

	/// <summary>
	/// Sets the destination path (folder) for storing the downloaded data.
	/// If this is not called, data is stored by the result of GetDefaultDestinationPath().
    /// NOTE: By default, data is stored under <see cref="Application.persistentDataPath"/>
	/// </summary>
	public BackgroundDownloadOptions SetDestinationPath(string destinationPath)
	{
		if (!string.IsNullOrEmpty(destinationPath))
		{
            Debug.Log("destinationPath " + destinationPath);

            Debug.Log("Path.GetFileName(URL)  "+ Path.GetFileName(URL));
            string combination = Path.Combine(destinationPath, Path.GetFileName(URL));
            this.destinationPath = combination.Replace("\\", "/");

        }

		return this;
	}

    public BackgroundDownloadOptions SetDestinationPath(string destinationPath, string videoName)
    {
        if (!string.IsNullOrEmpty(destinationPath))
        {
            Debug.Log("destinationPath " + destinationPath);

            Debug.Log("Path.GetFileName(URL)  " + videoName);
            string combination = Path.Combine(destinationPath,videoName);
            this.destinationPath = combination.Replace("\\", "/");

        }

        return this;
    }

    /// <summary>
    /// Sets the notification title text that will be shown while the download is active.
    /// </summary>
    public BackgroundDownloadOptions SetTitle(string title)
	{
		if (title != null)
		{
			this.title = title;
		}

		return this;
	}

	/// <summary>
	/// Sets the notification description text that will be shown while the download is active.
    /// NOTE: This may not be applicable on every platform!
	/// </summary>
	public BackgroundDownloadOptions SetDescription(string description)
	{
		if (description != null)
		{
			this.description = description;
		}

		return this;	
	}
}