using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SensorInput_GravityAccelerometer : MonoBehaviour {

	//public Text _accelerometerText;

	//public Collider2D _foerderbandCollider;
	//public Collider2D _particleCollider;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		/*
		_accelerometerText.text =
			" acc x: " + Input.acceleration.x + 
			"\n acc y: " + Input.acceleration.y + 
			"\n acc z: " + Input.acceleration.z;

		_cameraText.text = "gx: " + Physics2D.gravity.x + " - gy: " + Physics2D.gravity.y;
		*/

		//comment out for usual test
		/*
		if (Input.acceleration.x > -0.1f && Input.acceleration.x < 0.1f) {
			//check if acceleration is there for y
			if (Input.acceleration.y < -0f) {
				Physics2D.gravity = new Vector2 (0f, -9.81f);
			} else {
				Physics2D.gravity = new Vector2 (0f, +9.81f);
			}
		} else {

			Physics2D.gravity = new Vector2 (9.81f * Input.acceleration.x, 9.81f * Input.acceleration.y);
		}
		*/
		//comment in for deploy on android
		Physics2D.gravity = new Vector2 (9.81f * Input.acceleration.x, 9.81f * Input.acceleration.y);




	}
}



