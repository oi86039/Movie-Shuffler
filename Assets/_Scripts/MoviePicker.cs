using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoviePicker : MonoBehaviour
{
    public AudioSource sounds;

    Title_Manager title;
    public GameObject line; //Line to recenter game movie to

    GameObject chosenMovie;

    bool hitThatPause = false; //Pause the rb when done spining the wheel before moving the movie in place

    // Start is called before the first frame update
    void Start()
    {
        title = Title_Manager.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (title.stopped && chosenMovie)
        {
            if (!hitThatPause)
            {
                hitThatPause = true;
                title.hotBod.velocity = Vector2.zero;
            }

            if (chosenMovie.transform.position.x < line.transform.position.x - 8) //to the left of
            {
                title.hotBod.AddForce(Vector2.right * 100, ForceMode2D.Impulse);
            }
            else if (chosenMovie.transform.position.x > line.transform.position.x + 8) //to the right of
            {
                title.hotBod.AddForce(Vector2.left * 100, ForceMode2D.Impulse);
            }
            else
            {
                title.hotBod.velocity = Vector2.zero;
                chosenMovie.transform.position = new Vector2(line.transform.position.x, chosenMovie.transform.position.y);
                hitThatPause = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Reseter"))
        {
            title.DALIST.transform.position = title.DaListPosDefault;
        }
        else
        {
            if (chosenMovie)
                chosenMovie.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false;

            chosenMovie = other.gameObject;
            chosenMovie.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
        }
    }

}
