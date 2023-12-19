using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using OscJack;
using UnityEngine.Networking;
using System.Security.Policy;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

[HelpURL("https://github.com/FoxComment/XRController/blob/main/README.md")]
public class HeadTracker : MonoBehaviour
{


	#region OSC Related

	[Header("Senders")]


	[Space(5)]


	[SerializeField]
	private Vector3 oscOut_Param_Rotation;
	public Vector3 OSC_OUT_PARAM_ROTATION
	{
		get { return oscOut_Param_Rotation; }
		set { oscOut_Param_Rotation = value; }
	}


	[SerializeField]
	private float oscOut_Param_PositionX;
	public float OSC_OUT_PARAM_POSITIONX
	{
		get { return oscOut_Param_PositionX; }
		set { oscOut_Param_PositionX = value; }
	}


	[SerializeField]
	private float oscOut_Param_PositionY;
	public float OSC_OUT_PARAM_POSITIONY
	{
		get { return oscOut_Param_PositionY; }
		set { oscOut_Param_PositionY = value; }
	}


	[SerializeField]
	private float oscOut_Param_PositionZ;
	public float OSC_OUT_PARAM_POSITIONZ
	{
		get { return oscOut_Param_PositionZ; }
		set { oscOut_Param_PositionZ = value; }
	}


	[SerializeField]
	private float oscOut_Param_Horizontal;
	public float OSC_OUT_PARAM_HORIZONTAL
	{
		get { return oscOut_Param_Horizontal; }
		set { oscOut_Param_Horizontal = value; }
	}


	[SerializeField]
	private float oscOut_Param_Vertical;
	public float OSCOUT_PARAM_VERTICAL
	{
		get { return oscOut_Param_Vertical; }
		set { oscOut_Param_Vertical = value; }
	}


	[SerializeField]
	private int oscOut_Param_Back;
	public int OSCOUT_PARAM_BACK
	{
		get { return oscOut_Param_Back; }
		set { oscOut_Param_Back = value; }
    }


    [SerializeField]
    private int oscOut_Param_Use;
    public int OSCOUT_PARAM_USE
    {
        get { return oscOut_Param_Use; }
        set { oscOut_Param_Use = value; }
    }


    [SerializeField]
    private float oscIn_Param_VibrationPower;
    public float OSCIN_PARAM_VIBRATIONPOWER
    {
        get { return oscIn_Param_VibrationPower; }
        set { oscIn_Param_VibrationPower = value; }
    }


    [SerializeField]
	[TextArea]
	private string oscOut_Param_XRController;
	public string OSCOUT_PARAM_XRCONTROLLER
	{
		get { return oscOut_Param_XRController; }
		set { oscOut_Param_XRController = value; }
	}
	[Space(20)]


	[Header("Addresses")]


	[Space(5)]


	[SerializeField] private List<OSCAddresses> oscModeAddresses = new List<OSCAddresses>();
	[SerializeField] private OscPropertySender oscSender_Back;
	[SerializeField] private OscPropertySender oscSender_Use;
	[SerializeField] private OscPropertySender oscSender_Horizontal;
	[SerializeField] private OscPropertySender oscSender_Vertical;
	[SerializeField] private OscPropertySender oscSender_Rotation;
	[SerializeField] private OscPropertySender oscSender_PositionX;
	[SerializeField] private OscPropertySender oscSender_PositionY;
	[SerializeField] private OscPropertySender oscSender_PositionZ;
	[SerializeField] private OscPropertySender oscSender_XrController;

	[Header("______________________________________________________")]

	[Space(20)]


    #endregion


    #region Vars




    [Space(20)]

	[Header("Configuration")]

	[Space(5)]

	[SerializeField] private RectTransform touchpadTRA;
	[SerializeField] private InputField ipInput;
	[SerializeField] private Toggle disableYAxisTOGGLE;
	[SerializeField] private Slider motionSmoothingSLIDER;
	[SerializeField] private Toggle autoUpdatingTOGGLE;
	[SerializeField] private Toggle potatoModeTOGGLE;
	[SerializeField] private Toggle stayAwakeTOGGLE;
	[SerializeField] private Toggle askPackageTypeOnStartTOGGLE;
    [SerializeField] private Dropdown inputTypeDROPDOWN;
	[SerializeField] private Transform settingsMenuParent;
	[SerializeField] private Text settingsPageTEXT;
    [SerializeField] private List<GameObject> settingsSubMenus;	
    private int settingsCurrentPage;

	[Space(20)]

	[Header("Debug")]

	[Space(5)]

	[SerializeField] private Camera cam;
	private OSCController controller = new OSCController();
	[SerializeField] private Transform debugThreeDCubeTRA;
	[SerializeField] private Transform positionalJointTRA;
	[SerializeField] private RectTransform backgroundDecoStripes;
	[SerializeField] private RectTransform backgroundDecoStripesTwo;
	[SerializeField] private RectTransform backgroundDecoStripesThree;
	[SerializeField] private GameObject debugModeTab;
	//private Slider[] trackerPosSLIDES;
	[SerializeField] private Image statusLED;
	[SerializeField] private Text dataText;
	private Quaternion correctionQuaternion;
	private float controllerotationOffset;
	private Animator uiANIM;
	[SerializeField] private Color[] statusLEDColors; 

	[Space(20)]

	[Header("Notiffications")]

	[Space(5)]

	[SerializeField] private Text notifficationText;
	[SerializeField] private CanvasGroup notifficationCanvas;
	public static List<string> notifficationQue = new List<string>();

	[Space(20)]

	[Header("Notiffications")]

	[Space(5)]

	/// <summary>
	/// Fetch Updates from this place, then parse .json thingy and make a notiffication
	/// </summary>
	private string githubRepoAPIURL = "https://api.github.com/repos/user/reponame/releases";
	/// <summary>
	/// Link opens in a web browser, formed from UN and REPO
	/// </summary>
	private string githubLatestReleaseURL;
	[SerializeField][Tooltip("Your Github username \n(So app can fetch updates from your user page)")] 
	private string githubUsername = "FoxComment";
	[SerializeField][Tooltip("Your Repo name \n(So app can fetch updates from it)")] 
	private string githubRepo = "XRController";
    bool sendDataInOnePackage;
	bool touchActive;
	bool potatoMode;
	byte dbgClicks;
	int disableYAxis;
	float motionSmoothing; 
	string connectionIP;
	bool needsRestart;


	/// <summary>
	/// 0- LeftHand,
	/// 1- RightHand,
	/// 2- Body
	/// </summary>
	enum DeviceType
	{
		LeftHand,
		RightHand,
		Body
    }
    enum StatusLED
    {
		Connection_Error,
		OSC_Error,
		General_Error,
		Normal
    }



    #endregion






    /*
	/ _________________________________
    / |     	PLAYERPREFS VARS	  |
	/ ---------------------------------
	/ string	ConnectionIP			192.168.N
    / float		MotionSmoothing			0 - 1f
    / int		InputType				0 - N
    / int		DisableYAxis			0, 1
    / int		SendDataInOnePackage	-1, 1
    / int		AutoUpdates				-1, 1
    / int		StayAwake				-1, 1
    / int		PotatoMode				-1, 1
    / 
    */


    /*
	 * ________________________________
	 * |			 TODO			  |
	 * -------------------------------- 
	 * TURN POPUPS INTO ONE OBJECT
	 * STICK SNAPPING DISTANCE
	 * STICK DEADZONES SCALER
	 * MOTION SMOOTHING SLIDER
	 * ACTIONS MENU READER
	 * MAKE AN OSC RELAY
	 * USE FLOAT FOR HAPTICS, USE MI-VIBRATION PLUGIN FOR HD VIBRATION
	 * MULTI LANG SUPPORT, BASED ON SYSTEM LOCALIZATION
	 *
	 */






    #region Setup




    void Start()
	{

        Input.gyro.enabled = true;
		correctionQuaternion = Quaternion.Euler(90f, 0f, 0f);
		uiANIM = GetComponent<Animator>();
        githubRepoAPIURL = "https://api.github.com/repos/" + githubUsername + "/" + githubRepo + "/releases?sort=created&direction=desc";

        inputTypeDROPDOWN.options.Clear();
        foreach (OSCAddresses _preset in oscModeAddresses)
            inputTypeDROPDOWN.options.Add(new Dropdown.OptionData() { text = _preset._presetName, image = null });

        for (int i = 0; i < settingsMenuParent.childCount; i++)
            settingsSubMenus.Add(settingsMenuParent.GetChild(i).gameObject);
        //PUT ALL THE SETTINGS PAGES INTO ARRAY

        LoadPrefs();

        motionSmoothingSLIDER.gameObject.SetActive(!IsGyroSupported());
		disableYAxisTOGGLE.gameObject.SetActive(IsGyroSupported());

		StartCoroutine(HandleNotifficationsOrder());
		StartCoroutine(HandleTouchpadMovement());
		StartCoroutine(HandleVibration());

        ListSettingsPage(0);
		//RESET CURRENT PAGE IN SETTINGS

		InvokeRepeating("DebugHUDUpdate", .2f, .2f);
        //START DEBUG DRAW ON SETTINGS SCREEN

        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        try
		{

			ipInput.text = connectionIP;
			 
			SetupOSC(); 
			UpdateLED(StatusLED.Normal);
		}
        catch (Exception)
        {
			UpdateLED(StatusLED.OSC_Error);
			return; 
        }   
		//LOAD SAVED DEVICE CONNECTION IP IF THERES ONE

		//CHECK FOR THE NETWORK CONNECTION (NOT MOBILE ONE~)

		FetchUpdates();
    } 


	void Update()
	{
		controller._conRot = GetDeviceRotation();
		positionalJointTRA.rotation = Quaternion.Euler(controller._conRot);
		debugThreeDCubeTRA.rotation = Quaternion.Euler(controller._conRot);
		//controller._conPos = new Vector3(trackerPosSLIDES[0].value, trackerPosSLIDES[1].value, trackerPosSLIDES[2].value) + positionalJointTRA.GetChild(0).position;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			uiANIM.SetTrigger("Tab");
			if (needsRestart)
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		//REMOVEL?

		//oscOut_Param_Rotation = controller._conRot;
		oscOut_Param_Rotation = new Vector3(debugThreeDCubeTRA.eulerAngles.y, 0,0);
		oscOut_Param_PositionX = controller._conPos.x;
		oscOut_Param_PositionY = controller._conPos.y;
		oscOut_Param_PositionZ = controller._conPos.z;
		oscOut_Param_Vertical = Mathf.RoundToInt(controller._touch.y * 50) / 50f;
		oscOut_Param_Horizontal = Mathf.RoundToInt(controller._touch.x * 50) / 50f;
		oscOut_Param_Back = controller._back;
		oscOut_Param_Use = controller._use;

		if (touchActive)
			touchpadTRA.localScale = Vector3.Lerp(touchpadTRA.localScale, Vector3.one, .3f);
		else
			touchpadTRA.localScale = Vector3.Lerp(touchpadTRA.localScale, Vector3.zero, .3f);

		oscOut_Param_XRController = JsonUtility.ToJson(controller);

#if UNITY_EDITOR
		backgroundDecoStripes.localPosition = Vector3.Lerp(backgroundDecoStripes.localPosition, cam.ScreenToViewportPoint(Input.mousePosition) * 60f, .05f);
		backgroundDecoStripesTwo.localPosition = Vector3.Lerp(backgroundDecoStripesTwo.localPosition, cam.ScreenToViewportPoint(Input.mousePosition) * 30f, .03f);
		backgroundDecoStripesThree.localPosition = Vector3.Lerp(backgroundDecoStripesThree.localPosition, cam.ScreenToViewportPoint(Input.mousePosition)  * 20f, .01f);
#else
		backgroundDecoStripes.localPosition = Vector3.Lerp(backgroundDecoStripes.localPosition, Input.acceleration*60f, .05f);
		backgroundDecoStripesTwo.localPosition = Vector3.Lerp(backgroundDecoStripesTwo.localPosition, Input.acceleration*30f, .03f);
		backgroundDecoStripesThree.localPosition = Vector3.Lerp(backgroundDecoStripesThree.localPosition, Input.acceleration*20f, .01f);
#endif
    }


    void LoadPrefs()
    {

        if (!PlayerPrefs.HasKey("StayAwake"))
            PlayerPrefs.SetInt("StayAwake", 1);

        if (!PlayerPrefs.HasKey("PotatoMode"))
            PlayerPrefs.SetInt("PotatoMode", -1);

        if (!PlayerPrefs.HasKey("AutoUpdates"))
            PlayerPrefs.SetInt("AutoUpdates", 1);

        if (!PlayerPrefs.HasKey("SendDataInOnePackage"))
            PlayerPrefs.SetInt("SendDataInOnePackage", 0);

        if (!PlayerPrefs.HasKey("MotionSmoothing"))
            PlayerPrefs.SetFloat("MotionSmoothing", .2f);

        if (!PlayerPrefs.HasKey("InputType"))
            PlayerPrefs.SetInt("InputType", 0);

        if (!PlayerPrefs.HasKey("DisableYAxis"))
            PlayerPrefs.SetInt("DisableYAxis", 0);

        if (!PlayerPrefs.HasKey("ConnectionIP"))
            PlayerPrefs.SetString("ConnectionIP", "192.168.0.1");
        //CHECK IF ALL THE PARAMETERS ON PLACE, IF NOT, CREATE MISSING ONES


        switch (PlayerPrefs.GetInt("StayAwake"))
        {
            case 1:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
				stayAwakeTOGGLE.isOn = true;
                break;

            case -1:
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
				stayAwakeTOGGLE.isOn = false;
                break;
        }


        switch (PlayerPrefs.GetInt("PotatoMode"))
        {	
            case 1:
                Application.targetFrameRate = 51;
				potatoModeTOGGLE.isOn = true;
                break;

            case -1:
                Application.targetFrameRate = 75;
				potatoModeTOGGLE.isOn = false;
                break;
        }


        switch (PlayerPrefs.GetInt("AutoUpdates"))
        {
            case 1:
                FetchUpdates();
                autoUpdatingTOGGLE.isOn = true;
                break;

            case -1:
                autoUpdatingTOGGLE.isOn = false;
                break;
        }


        switch (PlayerPrefs.GetInt("SendDataInOnePackage"))
        {
            case 0:
                uiANIM.SetBool("Setup", true);
                askPackageTypeOnStartTOGGLE.isOn = false;
                break;

            case 1:
                uiANIM.SetBool("Setup", false);
                askPackageTypeOnStartTOGGLE.isOn = false;
                break;

            case -1:
                uiANIM.SetBool("Setup", false);
                askPackageTypeOnStartTOGGLE.isOn = false;
                break;
        }


        switch (PlayerPrefs.GetInt("DisableYAxis"))
        {
            case 0:
				disableYAxisTOGGLE.isOn = false;
                break;

            case 1:
				disableYAxisTOGGLE.isOn = true;
                break; 
        }
        //SET ALL THE TOGGLES TO RIGHT STATES

		 
        motionSmoothingSLIDER.value = PlayerPrefs.GetFloat("MotionSmoothing");
        //SET ALL THE SLIDERS TO RIGHT STATES

        inputTypeDROPDOWN.value = (PlayerPrefs.GetInt("InputType"));
        //SET ALL THE DROPDOWNS TO RIGHT STATES

        connectionIP = PlayerPrefs.GetString("ConnectionIP");
		//SET ALL THE OTHER NEEDED STUFF
    }


    public void SetDataSendInOnePack(bool _inOne)
	{
		sendDataInOnePackage = _inOne;
		if (_inOne)
			PlayerPrefs.SetInt("SendDataInOnePackage", 1);
		else
			PlayerPrefs.SetInt("SendDataInOnePackage", -1);

		uiANIM.SetBool("Setup", false);
		//SHOW A PACKAGE TYPE DIAG ON APP START, IF NO SAVED SETTING PRESENTED
	}


	bool IsGyroSupported() { return SystemInfo.supportsGyroscope; }


    void SetupOSC()
    {

        oscSender_Use.enabled = false;
        oscSender_Back.enabled = false;
        oscSender_Vertical.enabled = false;
        oscSender_Horizontal.enabled = false;
        oscSender_Rotation.enabled = false;
        oscSender_PositionX.enabled = false;
        oscSender_PositionY.enabled = false;
        oscSender_PositionZ.enabled = false;
        oscSender_XrController.enabled = false;
        //DISABLE ALL OSC SENDERS, SO THEY WON'T LAG WHILE CHANGING

        oscSender_Use._ipAddress = connectionIP;
        oscSender_Back._ipAddress = connectionIP;
        oscSender_Vertical._ipAddress = connectionIP;
        oscSender_Horizontal._ipAddress = connectionIP;
        oscSender_Rotation._ipAddress = connectionIP;
        oscSender_PositionX._ipAddress = connectionIP;
        oscSender_PositionY._ipAddress = connectionIP;
        oscSender_PositionZ._ipAddress = connectionIP;
        oscSender_XrController._ipAddress = connectionIP;
        //SET IP ADDRESS

        oscSender_Use._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._use;
        oscSender_Back._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._back;
        oscSender_Vertical._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._touchVertical;
        oscSender_Horizontal._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._touchHorizontal;
        oscSender_Rotation._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._deviceRotation;
        oscSender_PositionX._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._devicePositionX;
        oscSender_PositionY._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._devicePositionY;
        oscSender_PositionZ._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._devicePositionZ;
        oscSender_XrController._oscAddress = oscModeAddresses[inputTypeDROPDOWN.value]._fullPack;
        //SET DESTINATION ADDRESS

        if (sendDataInOnePackage)
        {
            oscSender_XrController.enabled = true;
        }
        else
        {
            oscSender_Use.enabled = true;
            oscSender_Back.enabled = true;
            oscSender_Vertical.enabled = true;
            oscSender_Horizontal.enabled = true;
            oscSender_Rotation.enabled = true;
            oscSender_PositionX.enabled = true;
            oscSender_PositionY.enabled = true;
            oscSender_PositionZ.enabled = true;
        }

    }


    Vector3 GetDeviceRotation()
    {
#if UNITY_EDITOR_WIN
        return new Vector3(Mathf.RoundToInt(Mathf.Sin(Time.time * 2) * 360), Mathf.RoundToInt(Mathf.Sin(Time.time * 1) * 360), Mathf.RoundToInt(Mathf.Sin(Time.time * .1f) * 360));
#else
		if (IsGyroSupported())
		{ 
			Quaternion calculatedRotation = correctionQuaternion * new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
			return new Vector3(calculatedRotation.eulerAngles.x, calculatedRotation.eulerAngles.y * disableYAxis, calculatedRotation.eulerAngles.z);
		}
        else 
		{
			return Vector3.Lerp(controller._conRot, Quaternion.LookRotation(Input.acceleration, Vector3.forward).eulerAngles, motionSmoothing);
		}
#endif
    }


#endregion


	#region Actions




    public void UpdateDiagClose() { uiANIM.SetBool("Update", false); }


    public void UpdateDiagOpen() { uiANIM.SetBool("Update", true); }


    public void UpdateDiagOpenInWeb() { Application.OpenURL(githubLatestReleaseURL); }


    public void FetchUpdates() { StartCoroutine(HandleGitHubRequest()); }


    public void InteractTouchpad(bool dragging) { touchActive = dragging; }


    public void InteractPageListing(int _list) { ListSettingsPage(_list); }


    public void InteractBackButton(int pressed) { controller._back = pressed; }


    public void InteractUseButton(int pressed) { controller._use = pressed; }


    public void InteractInputType() { PlayerPrefs.SetInt("InputType", inputTypeDROPDOWN.value); }


    public void InteractSmoothingSlider()
    {
        motionSmoothing = motionSmoothingSLIDER.value;

        PlayerPrefs.SetFloat("MotionSmoothing", motionSmoothing);
    }


    public void InteractIgnoreAxisToggle()
    {
        if (disableYAxisTOGGLE.isOn)
            disableYAxis = 1;
        else
            disableYAxis = 0;

        PlayerPrefs.SetInt("DisableYAxis", disableYAxis);
    }


    public void InteractUpdatesToggle()
    {
        if (autoUpdatingTOGGLE.isOn)
            PlayerPrefs.SetInt("AutoUpdates", 1);
        else
            PlayerPrefs.SetInt("AutoUpdates", -1);
    }


    public void InteractPackageToggle()
    {
        if (askPackageTypeOnStartTOGGLE.isOn)
            PlayerPrefs.SetInt("SendDataInOnePackage", 0);
        else
            PlayerPrefs.SetInt("SendDataInOnePackage", -1);
    }


    public void InteractPotatoToggle()
    {
        if (potatoModeTOGGLE.isOn)
            PlayerPrefs.SetInt("PotatoMode", 1);
        else
            PlayerPrefs.SetInt("PotatoMode", -1);
    }


    public void InteractAwakeToggle()
    {
        if (stayAwakeTOGGLE.isOn)
            PlayerPrefs.SetInt("StayAwake", 1);
        else
            PlayerPrefs.SetInt("StayAwake", -1);
    }


    public void InteractIPField()
    {
        if (!ipInput.text.Contains("192.168.")) return;

        PlayerPrefs.SetString("ConnectionIP", ipInput.text);

        notifficationQue.Add("Please, restart an application to apply current settings.");

        SetupOSC();
    }


    /// <summary>
    /// Closes Upd Diag and destroys everything related to it.
    /// </summary>
    void IgnoreUpdate() 
	{
		//DELETE UPDATE DIAG FROM ANIMATOR LAYERS
		//DELETE UPDATE DIAG AS A GAMEOBJECT
		//TO SAVE UP A BIT ON A PERFORMANCE
	}


	/// <summary>
	/// Accesses an Extra debugging menu.
	/// </summary>
	public void DebugModeClick()
	{
		dbgClicks++;

		if (dbgClicks == 5) 
			notifficationQue.Add("OSC.J v0.2.1 \n FoxXRController v"+ System.DateTime.Now.ToString(Application.version));

		if (dbgClicks == 10)
			notifficationQue.Add("Almost there!"); 

		if (dbgClicks == 15)
		{
			notifficationQue.Add("Advanced Configuration had been accessed.");
			debugModeTab.SetActive(true);
		}

		if (dbgClicks == 30)
		{
			notifficationQue.Add("Reverted Changes.");
			debugModeTab.SetActive(false);
			dbgClicks = 0;
		}
	}


    /// <summary>
    /// Lists N pages in settings, rounds each other if goes off limits, won't work if there's less than 2 pages.
    /// </summary> 
    void ListSettingsPage(int _pageAdditive)
	{
		if (settingsSubMenus.Count > 2)
			return;

		foreach (GameObject _menu in settingsSubMenus)
		{ _menu.SetActive(false); }

        settingsCurrentPage += _pageAdditive;

		if (settingsCurrentPage > settingsSubMenus.Count - 1)
			settingsCurrentPage = 0;
		else if (settingsCurrentPage < 0)
			settingsCurrentPage = settingsSubMenus.Count - 1;

		settingsPageTEXT.text = "Page: " + (settingsCurrentPage + 1);
        settingsMenuParent.GetChild(settingsCurrentPage).gameObject.SetActive(true);
	}


	#endregion


	#region Handlers



    
    IEnumerator HandleTouchpadMovement()
    {
        while (true)
        {
            yield return new WaitUntil(() => touchActive);

            while (touchActive)
            {
                touchpadTRA.GetComponent<RectTransform>().position = Input.mousePosition;
                touchpadTRA.GetComponent<RectTransform>().localPosition = Vector3.ClampMagnitude(touchpadTRA.GetComponent<RectTransform>().localPosition, 93);

                controller._touch = touchpadTRA.GetComponent<RectTransform>().localPosition / 93f;

                yield return new WaitForEndOfFrame();
            }

            controller._touch = Vector2.zero;

        }
    }


	//YOU CANNOT COPYYRIGHT TWO REALTIME LIGHT SOURCES AND A DUM CYLINDER.
    IEnumerator HandleNotifficationsOrder()
    { 
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (notifficationQue.Count > 0)
            {

                yield return new WaitForSeconds(.5f);

                notifficationText.text = notifficationQue[notifficationQue.Count - 1];
                notifficationQue.RemoveAt(notifficationQue.Count - 1); 

                uiANIM.SetBool("NotifficationDisplay", true);

                yield return new WaitForSeconds(4);

				uiANIM.SetBool("NotifficationDisplay", false);
				 
            }

        }

    }


    IEnumerator HandleVibration()
	{
		while (true)
		{
			yield return new WaitUntil(()=> oscIn_Param_VibrationPower > .3f);
			Handheld.Vibrate();
            yield return new WaitUntil(() => oscIn_Param_VibrationPower < .2f);

            yield return new WaitForEndOfFrame();

			//replace with Vibrate(oscIn_Param_VibrationPower); after switching to unity 19
		}
	}


    /// <summary>
    /// Sent WebRequest to the repo, and get json file, then compare app version 
    /// and figure out if there's a newer version of it presented on GH.
    /// </summary>
    IEnumerator HandleGitHubRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(githubRepoAPIURL))
        {
            print("Fetching GH releases");

            yield return webRequest.SendWebRequest();

            print("Sent request");

            string _responseST = webRequest.downloadHandler.text.Insert
                (0, "{" + "\n" + "\"" + "_versions" + "\":" + "\n");
            _responseST += "}";

            print("Got response");

			if (_responseST.Contains("documentation_url"))
				yield break;	

            VersionsGithub _responseJSON = JsonUtility.FromJson<VersionsGithub>(_responseST);
            ReleaseApp _latestRelease = _responseJSON._versions[_responseJSON._versions.Count - 1];

            if (_latestRelease.name != Application.version)
            {
                githubLatestReleaseURL = _latestRelease.html_url;
                uiANIM.SetBool("Update", true);
            }

            print("Request satisfied");

        }
    }


	#endregion


	#region Configuration




    /// <summary>
    /// Status LED on the bottom of controller.
    /// </summary> 
    void UpdateLED(StatusLED status)
    {
        switch (status)
		{
			case StatusLED.Normal:
				statusLED.color = statusLEDColors[3];
				break;

			case StatusLED.General_Error:
				statusLED.color = statusLEDColors[2]; 
				notifficationQue.Add("An error occupied, please restart the application.");
				break;

			case StatusLED.OSC_Error:
				statusLED.color = statusLEDColors[1]; 
				notifficationQue.Add("Failed to send data, try restarting application.");
				break;

			case StatusLED.Connection_Error:
				statusLED.color = statusLEDColors[0];
				notifficationQue.Add("You are not connected to a WIFI network.");
				break;

		}
	}


	/// <summary>
	/// Updates TextBox in cfg menu. 
	/// Contains: acceleration, gyro, userAcceleration, deltaTime(FPS)
	/// </summary>
	void DebugHUDUpdate()
	{

		if (IsGyroSupported())
		{
			dataText.text =
				"Accel - " + Input.acceleration.ToString() + "\n" + 
				"Gyro - " + GetDeviceRotation() + "\n" +
				"Accel2 - " + Input.gyro.userAcceleration.ToString() + "\n \n \n \n" +
				"Refresh Rate - " + Mathf.RoundToInt(1.0f / Time.deltaTime).ToString(); 
		}
		else
		{
			dataText.text =
				"Accel - " + GetDeviceRotation().ToString() + "\n" + 
				"No Gyro presented." + "\n \n \n \n" +
				"Refresh Rate - " + Mathf.RoundToInt(1.0f / Time.deltaTime).ToString();
		}  
	}


	#endregion


	#region Sub-classes




    [Serializable]
	private class OSCController
	{
		public Vector3 _conRot;
		public Vector3 _conPos; 
		public Vector2 _touch;
		public int _back;
		public int _use;
	}


	[Serializable]
	private class OSCAddresses
	{
		public string _presetName = "New Preset";
		[Space(10)]
		public string _deviceRotation = "/null";
		public string _devicePositionX = "/null";
		public string _devicePositionY = "/null";
		public string _devicePositionZ = "/null";
		public string _touchHorizontal = "/null";
		public string _touchVertical = "/null";
		public string _back = "/null";
		public string _use = "/null";
		[Space(5)]
		public string _fullPack = "/null";
	}
	 

	[Serializable]
	public class VersionsGithub
	{
		public List<ReleaseApp> _versions;
		public string message;
	}

	[Serializable]
	public class ReleaseApp
	{ 
		public object author; 
		public string name = "";
		public bool draft = false;
		public string html_url;

        public List<assets> assets;	
	}


	[Serializable]
	public class assets
    {
		public int size = 0;
		public int download_count = 0;
		public string browser_download_url = "";
		public string updated_at = "";
    }


	#endregion

}