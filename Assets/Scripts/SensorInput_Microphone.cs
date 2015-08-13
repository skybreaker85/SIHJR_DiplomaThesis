using UnityEngine;
using System.Collections;

public class SensorInput_Microphone : MonoBehaviour {

	//THSI APPROCH DONT NEED AN EXTRA AudioSource Object !!!!!!!!!!!!!

	
	public float _sensitivity = 1.5f;
	private static float _volume;
	private string _deviceName;
	private AudioClip _recordedAudoClip = new AudioClip ();
	private int _sampleRate = 128;								//samples are acutally the volume of specific frequencies
	private bool _isInitialized;




	// Use this for initialization
	void Start () {
		//call init or OnEnbale?, check if both fire
	}
	
	// Update is called once per frame
	void Update () {
		//get volume of audio

		//Update is for testing purpose, empty it, when mic input is used where it should be
		//_volume = GetVolume ();
		//Debug.Log ("MicVolume: " + _volume);

	}

	
	//get data from microphone into audioclip
	public float  GetVolume ()
	{
		float volumeMax = 0;
		float[] sampleArray = new float[_sampleRate];
		//set mic listening position from first mic device
		int micPosition = Microphone.GetPosition (null) - (_sampleRate + 1); // null == first mic
		//check if position < 0, becasue 0 == no input, GetData dont accept -1 input -> when negative, just return 0 volume
		if (micPosition < 0) {
			return 0;
		}
		//sample the data
		_recordedAudoClip.GetData (sampleArray, micPosition);
		// like in getdfata example, go through samples and do something with it
		for (int i = 0; i < _sampleRate; i++) {
			/*
			float wavePeak = sampleArray [i] * sampleArray [i];
			if (volumeMax < wavePeak) {
				volumeMax = wavePeak;
			}
			*/

			//max float of samples
			//volumeMax = Mathf.Max(volumeMax, sampleArray [i]);
			//or use kumulative added samples:
			volumeMax += Mathf.Abs(sampleArray [i]); //SAMPLES CAN RETURN negative VALUES -> turn into positive ones
		}
		//for average volume over sampleSum: (comment it out when not used)
		volumeMax = volumeMax / _sampleRate;


		//TODO: maybe it can get too loud, so maybe clamp the returned value on e.g. 0.25f? becasue its checked four times a second, so it can just spawn every second?
		//TODO: or maybe do it in bucket Factory code
		return _sensitivity * volumeMax;
	}


	private void InitMicrophone() {
		if (_deviceName == null) {
			_deviceName = Microphone.devices [0];	//annahme: gibt nru ein mic
		}
		//Mic.Start(name, loop, length, kHz) // returns null if fails to start //returns audioclip
		_recordedAudoClip = Microphone.Start (_deviceName, true, 999, 44100);
	}

	
	private void EndMicrophone ()
	{
		Microphone.End (_deviceName);
	}

	
	// start mic when scene starts
	void OnEnable ()
	{
		InitMicrophone ();
		Debug.Log ("Init enable: " + _deviceName);
		_isInitialized = true;
	}

	//stop mic when pausing or quit application
	void OnDisable ()
	{
		EndMicrophone ();
	}
	void OnDestroy ()
	{
		EndMicrophone ();
	}

	// app focus changed, so change mic status 	//even focusing out of unity gamecam window disables mic (consider it when testing)
	void OnApplicationFocus (bool focused)
	{
		if (focused) {
			//dont init it again, when already enabled/initialized
			if (!_isInitialized) {
				//Debug.Log("focus: start mic");
				InitMicrophone ();
				_isInitialized = true;
			}
		}
		else {
		//if (!focused) {
			//Debug.Log("focus: stop mic");
			EndMicrophone ();
			_isInitialized = false;
			
		}
	}
}
