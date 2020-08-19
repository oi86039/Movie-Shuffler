using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Title_Manager : MonoBehaviour
{

    private static Title_Manager instance;

    public static Title_Manager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Title_Manager>();
            return instance;
        }
    }

    #region Input
    [Tooltip("read the text file")]
    StreamReader reader;

    [Header("Input")]
    [Tooltip("directory of text file to read")]
    public string InputDir;

    [Tooltip("text file to read")]
    public string InputTextFile;

    [Tooltip("img file to read")]
    private string InputImageFile;
    #endregion Input

    //Dictionary containing text value and image values
    public Dictionary<string, Sprite> dic = new Dictionary<string, Sprite>();

    #region GameObjects
    [Header("GameObjects")]
    [Tooltip("Prefab to instantiate with img and txt")]
    public GameObject Poster;
    public GameObject Reseter; //When hitting this game object 

    public GameObject DALIST;
    public Rigidbody2D hotBod;
    #endregion GameObjects

    [Tooltip("Place to put new item")]
    Vector2 spawnPos;
    public Vector3 DaListPosDefault;

    [Tooltip("Place to put new item")]
    public Vector2 offset;

    [Header("Audio")]
    public AudioClip itemCycle;
    public AudioClip itemConfirm;
    public AudioClip glassBreak;
    AudioSource source; //Audiosource for sound effects
    public AudioSource musicSource; //Audiosource for just music
    public AudioLowPassFilter filter; //For muffled sound effect

    private bool soundMuted = false; //Mutes item box sound audio if true;
    private bool muted = false; //Mutes game music audio if true;

    public bool stopped; //Stops sounds from playing all corrupted and shit
                         //public Slider forceGauge;

    bool canPress; //Activate for a delay between space bar presses

    // Start is called before the first frame update
    void Start()
    {
        stopped = true;
        canPress = true;

        spawnPos = DALIST.transform.position;
        source = GetComponent<AudioSource>();
        hotBod = DALIST.GetComponent<Rigidbody2D>();
        DaListPosDefault = DALIST.transform.position;

        InputDir = Application.streamingAssetsPath + "/" + InputDir + "/";

        //Initialize reader
        reader = new StreamReader(InputDir + InputTextFile + ".txt");

        //If successful
        while (!reader.EndOfStream)
        {
            //Get line
            string whatTheHell = reader.ReadLine();

            //Get image
            dic[whatTheHell] = Resources.Load<Sprite>(whatTheHell);
        }

        //Actually make the game objects with these things
        DisplayIOWA();
    }

    // Update is called once per frame
    void Update()
    {
        ////Hold button to fill gauge, release to run with given force
        //if (Input.GetButtonDown("Click") && hotBod.velocity == Vector2.zero)
        //{
        //    forceGauge.value = forceGauge.minValue;
        //}

        

        //else if (Input.GetButtonUp("Click") && hotBod.velocity == Vector2.zero)
        //{
        //    Running();
        //    forceGauge.value = forceGauge.minValue;
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!stopped) //State where wheel is turning
        {
            //hotBod.drag += Random.Range(0.0001f,0.002f);
            filter.cutoffFrequency = 22000.0f;

            if (Input.GetButtonDown("Click"))
            {
                StopWheel();
            }
        }
        else //State where wheel is completely stopped
        {
            if (filter.cutoffFrequency > 500.0f)
            {
                filter.cutoffFrequency -= 200;
            }
            else if (filter.cutoffFrequency < 500.0f)
            {
                filter.cutoffFrequency = 500.0f;
            }
        }

        if (hotBod.velocity.x >= -350 && !stopped)
        {
            StopWheel();
        }

        if (Input.GetButtonDown("Click") && hotBod.velocity == Vector2.zero && canPress)
        {
            Running();
            canPress = false;
        }
    }

    public void DisplayIOWA()
    {
        foreach (KeyValuePair<string, Sprite> s in dic)
        {
            //Creates a Poster Prefab (BoiseIdaho) at the specified position and makes it a child of the canvas
            var clone = Instantiate(Poster, new Vector3(spawnPos.x, spawnPos.y, 0), Poster.transform.rotation, DALIST.transform);

            clone.name = s.Key;

            var image = clone.transform.GetChild(1);
            var text = clone.transform.GetChild(2);
            text.GetComponent<Text>().text = s.Key;
            image.GetComponent<Image>().sprite = s.Value;

            spawnPos += offset;
        }
        //Make Empty game object
        var reseter = Instantiate(Reseter, new Vector3(spawnPos.x, spawnPos.y, 0), Reseter.transform.rotation, DALIST.transform);

        reseter.name = "RESET";

    }

    public void Running()
    {
        stopped = false;
       // hotBod.AddForce(new Vector2(-forceGauge.value * Random.Range(8000f, 12000f), 0), ForceMode2D.Impulse);
        hotBod.AddForce(new Vector2(-12000f, 0), ForceMode2D.Impulse);
        hotBod.drag = 0;
        source.Stop();
        source.PlayOneShot(glassBreak);
        source.PlayOneShot(glassBreak);
        source.PlayOneShot(glassBreak);
        source.loop = true;
        source.clip = itemCycle;
        source.Play();
    }

    public void StopWheel()
    {
        hotBod.velocity = Vector2.zero;
        stopped = true;
        source.Stop();
        source.loop = false;
        source.clip = itemConfirm;
        source.Play();
        StartCoroutine(PressDelay());
    }

    public void MuteSound()
    {

        soundMuted = !soundMuted;
        if (soundMuted) source.volume = 0;
        else source.volume = 1;
    }

    public void MuteMusic()
    {

        muted = !muted;
        if (muted) musicSource.volume = 0;
        else musicSource.volume = 1;

    }

    public IEnumerator PressDelay() {

        yield return new WaitForSeconds(0.8f);
        canPress = true;
    }

}
