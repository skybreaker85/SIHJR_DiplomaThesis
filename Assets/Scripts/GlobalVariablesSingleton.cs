using UnityEngine;
using System.Collections;

public class GlobalVariablesSingleton
{
	// SINGLETON		##########################################################################
	private static GlobalVariablesSingleton _instance = null;
	public static GlobalVariablesSingleton instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GlobalVariablesSingleton();
				_instance.init();
			}
			return _instance;
		}
	}

	// CLASS 			##########################################################################

	private float _scoreCount;
	private UIBehaviour _uiScriptReference;

	private void init()
	{
		color = Color.black;
		_scoreCount = 0;
		_uiScriptReference = (UIBehaviour)GameObject.Find("Sensor - Script - Layer").GetComponent(typeof(UIBehaviour));
	}

	public Color getRandomColor()
	{
		Color c = new Color();
		c.r = Random.Range(0f, 1f);
		c.g = Random.Range(0f, 1f);
		c.b = Random.Range(0f, 1f);
		c.a = 1f;
		return c;
	}

	// SCORE CONTROL		##########################################################################

	public float scoreCount
	{
		get
		{
			//Debug.Log ("return value of scorecount is: " + _scoreCount + " ##############");
			return _scoreCount;
		}
		set
		{
			_scoreCount = value;
			_uiScriptReference.refreshScoreText();
		}
	}
	public void addScoreCount(float value)
	{
		_scoreCount += value;
		//Debug.Log ("addScoreCount called with: " + value + "-" + _scoreCount);
		_uiScriptReference.refreshScoreText();
	}

	// GETTER AND SETTER		##########################################################################

	public Color color
	{
		get; set;
	}

	public float particleSpawnRate
	{
		get; set;
	}

	public float bucketThreshholdCount
	{
		get; set;
	}
}
