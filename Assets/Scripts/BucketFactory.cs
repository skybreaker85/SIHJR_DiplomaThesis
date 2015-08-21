using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BucketFactory : MonoBehaviour {

	public GameObject _bucketObject;
	private GameObject _tempBucket;

	public float _rateOfMicrophoneVolumeCheck = 0.25f;	//viermal je sekunde volume prüfen
	public float _spawnBucketThreshhold = 5f;
	public float _speedForBuckets = 0.5f;
	private float _timeLeft;
	private float _threshholdCount;

	public Text _debugText;

	private int _bucketCount;

	private SensorInput_Microphone _micScriptReference;


	
	// Use this for initialization
	void Start () {
		_timeLeft = 0f;
		_threshholdCount = 0f;

		//set reference to MicrophoneScript
		_micScriptReference = (SensorInput_Microphone) GameObject.Find("Sensor - Script - Layer").GetComponent(typeof(SensorInput_Microphone));
	}
	
	// Update is called once per frame
	void Update () {
		if (_timeLeft <= 0f) {

			_threshholdCount -= Mathf.Min(0.15f, _micScriptReference.GetVolume());	//TODO: correct max value? (first param): check comments on sensorinput_micorphone script -> return value of getVolume()

			//set count to globalvarinstance, to see the value (and later create a gauge out of it)
			GlobalVariablesSingleton.instance.bucketThreshholdCount = _threshholdCount;
			//when enough volume "gathered", spawn bucket
			if (_threshholdCount <= 0f) {
				//spawn bucket
				spawnBucket();
				//reset threshholdCounter
				_threshholdCount = _spawnBucketThreshhold;
			}

			//Debug.Log("tiemeleft b: " + _timeLeft);
			//reset Timer
			_timeLeft = _rateOfMicrophoneVolumeCheck;
			//Debug.Log("tiemeleft a: " + _timeLeft);
		} else {
			//decrease Time
			_timeLeft -= Time.deltaTime;
		}
	}

	private void spawnBucket() {
		_tempBucket = Instantiate (_bucketObject, new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
		//set layer to parent and child sprite
		_tempBucket.transform.gameObject.layer = 9;
		for (int i = 0; i < _tempBucket.transform.childCount; i++)
		{
			//_tempBucket.transform.GetChild(0).gameObject.layer = 9;
			//_tempBucket.transform.GetChild(1).gameObject.layer = 9;
			//_tempBucket.transform.GetChild(2).gameObject.layer = 9;
			//_tempBucket.transform.GetChild(3).gameObject.layer = 9;
			_tempBucket.transform.GetChild(i).gameObject.layer = 9;
		}

		//color target color
		//Debug.Log ("child 0 name: " + _tempBucket.transform.GetChild(0).name + "\nchild 1 name: " + _tempBucket.transform.GetChild(1).name + "\n");

		BucketBehaviour scriptReference = (BucketBehaviour)_tempBucket.GetComponent (typeof(BucketBehaviour));  //TODO: the following things can be done by bucket himself -> remove getcomponent call (performance issue)
		Color col = ColorController.instance.getRandomColor();
		Color colSeason = ColorController.instance.getInfluencedColor(col);
		Color colSun = ColorController.instance.daylightInfluencedColor(col);
		Color colSeasoSun = ColorController.instance.daylightInfluencedColor(colSeason);
        scriptReference.setTargetColor (col);
		scriptReference.setTargetColorAlteredSeason (colSeason);
		scriptReference.setTargetColorAlteredSun (colSun);
		scriptReference.setTargetColorAlteredSeasonSun (colSeasoSun);
		//scriptReference.setTargetColor(Color.black);
		scriptReference.speed = _speedForBuckets;
		scriptReference._debugText = _debugText;
		scriptReference.id = _bucketCount++;
		//following line is now set in bucketbehaviour
		//_tempBucket.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = GlobalVariablesSingleton.instance.getRandomColor();
	}
}
