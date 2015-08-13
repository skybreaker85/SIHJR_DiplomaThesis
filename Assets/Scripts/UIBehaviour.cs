using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBehaviour : MonoBehaviour {

	public Text _scoreTextField;
	public Text _accelerometerText;
	public Text _cameraText;
	public Text _particleRateText;
	public Text _fpsText;
	public Text _bucketText;
	public Text _debugText;

	// Use this for initialization
	void Start () {
		refreshTextAtStart ();

		//set the screen to not sleep / dimm
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Update() {
		_accelerometerText.text =
			" acc x: " + Input.acceleration.x + 
				"\n acc y: " + Input.acceleration.y + 
				"\n acc z: " + Input.acceleration.z;
		
		_cameraText.text = "gx: " + Physics2D.gravity.x + " - gy: " + Physics2D.gravity.y;

		_particleRateText.text = GlobalVariablesSingleton.instance.particleSpawnRate + "=";
		_fpsText.text = "FPS: " + (1 / Time.deltaTime);
		_bucketText.text = "Bucket: " + GlobalVariablesSingleton.instance.bucketThreshholdCount;
	}

	
	private void refreshTextAtStart() {
		_accelerometerText.text = "accTF";
		_cameraText.text = "camTF";
		_scoreTextField.text = "ScoreTF";
	}

	public void refreshScoreText() {
		_accelerometerText.text = "accTF";
		_cameraText.text = "camTF";
		//Debug.Log ("refreshScoreText called !! +++++++++++ !! " + GlobalVariablesSingleton.instance.scoreCount);
		_scoreTextField.text = "Score: " + GlobalVariablesSingleton.instance.scoreCount;
	}
}
