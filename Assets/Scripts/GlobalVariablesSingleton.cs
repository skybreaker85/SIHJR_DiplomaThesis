using System;
using UnityEngine;

public class GlobalVariablesSingleton
{
	// SINGLETON		##########################################################################
	//should be a singleton, to swap out score count, depending on difficulty/gamemode (maybe implemented later)
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
		_scoreCount = 0;
		isWaterTapOpen = false;
		particleSpawnRate = 0.25f;
		actualLatitude = -999f;
		actualLongitude = -999f;
		sunrise = DateTime.Now;
		sunset = DateTime.Now;
		isSunrise = true;
		isSunset = false;
		Now = DateTime.Now;
        _uiScriptReference = (UIBehaviour)GameObject.Find("Sensor - Script - Layer").GetComponent(typeof(UIBehaviour));
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
	public float particleSpawnRate { get; set; }

	public float bucketThreshholdCount { get; set; }

	public bool isWaterTapOpen { get; set; }

	public float actualLatitude { get; set; }

	public float actualLongitude { get; set; }

	public DateTime Now { get; set; }

	public DateTime sunrise { get; set; }
	public DateTime sunset { get; set; }
	public bool isSunrise { get; set; }
	public bool isSunset { get; set; }
}
