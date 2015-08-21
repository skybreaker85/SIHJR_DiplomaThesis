using UnityEngine;
using System.Collections;

public class PanelBehaviour : MonoBehaviour {

	//refrence for the pause menu panel in the hierarchy
	public GameObject panelLeftMenu;
	public GameObject panelRightMenu;
	//animator reference
	private Animator animLeft;
	private Animator animRight;
	//variable for checking if the game is paused 
	//private bool isPaused = false;
	// Use this for initialization
	void Start()
	{
		//unpause the game on start
		//Time.timeScale = 1;
		//get the animator component
		animLeft = panelLeftMenu.GetComponent<Animator>();
		animRight = panelRightMenu.GetComponent<Animator>();
		//disable it on start to stop it from playing the default animation
		animLeft.enabled = false;
		animRight.enabled = false;
	}

	// Update is called once per frame
	public void Update()
	{
		/*
		//pause the game on escape key press and when the game is not already paused
		if (Input.GetKeyUp(KeyCode.Escape) && !isPaused)
		{
			OpenMenu();
		}
		//unpause the game if its paused and the escape key is pressed
		else if (Input.GetKeyUp(KeyCode.Escape) && isPaused)
		{
			CloseMenu();
		}
		*/
	}

	//function to pause the game
	public void OpenLeftMenu()
	{
		//enable the animator component
		animLeft.enabled = true;
		//play the Slidein animation
		animLeft.Play("PanelLeftSlideIn");
		//set the isPaused flag to true to indicate that the game is paused
		//isPaused = true;
		//freeze the timescale
		//Time.timeScale = 0;
	}
	//function to unpause the game
	public void CloseLeftMenu()
	{
		//set the isPaused flag to false to indicate that the game is not paused
		//isPaused = false;
		//play the SlideOut animation
		animLeft.Play("PanelLeftSlideOut");
		//set back the time scale to normal time scale
		//Time.timeScale = 1;
	}
	//function to pause the game
	public void OpenRightMenu()
	{
		//enable the animator component
		animRight.enabled = true;
		//play the Slidein animation
		animRight.Play("PanelRightSlideIn");
		//set the isPaused flag to true to indicate that the game is paused
		//isPaused = true;
		//freeze the timescale
		//Time.timeScale = 0;
	}
	//function to unpause the game
	public void CloseRightMenu()
	{
		//set the isPaused flag to false to indicate that the game is not paused
		//isPaused = false;
		//play the SlideOut animation
		animRight.Play("PanelRightSlideOut");
		//set back the time scale to normal time scale
		//Time.timeScale = 1;
	}
}
