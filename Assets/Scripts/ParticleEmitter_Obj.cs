using UnityEngine;
using System.Collections;

public class ParticleEmitter_Obj : MonoBehaviour {
	
	
	public GameObject _particle;
	
	private GameObject _tempParticle;
	
	//private AudioSource _waterDropSound;
	
	public float speed = 0.1F;
	//public float _spawnRate = 0.1F;
	private float _spawnTime;
	public float _forceMaxRandom = 2f;

	private bool _showParticle;
	
	//private Color _color;
	
	//private int _screenWidth;
	//private int _screenHeight;

	//public int _mobileScreenToWorldCoordinatesConstant = 50;
	//private float _speedFactorOfEmitter;

	public Collider2D _foerderbandCollider;
	
	
	// Use this for initialization
	void Start () {
		_spawnTime = 0;
		_showParticle = true;
		//_mobileScreenToWorldCoordinatesConstant = 50;
		//_speedFactorOfEmitter = 0.5f;
		//_color = Color.red;

		//becasue its portrait -> use max value as "width"
		/*
		if (Screen.width > Screen.height) {
			_screenWidth = Screen.height;	//width should be smaller than the height
			_screenHeight = Screen.width;
		} else {
			_screenWidth = Screen.width;
			_screenHeight = Screen.height;
		}
		*/
		
		//_waterDropSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalVariablesSingleton.instance.isWaterTapOpen)
		{
			//count the time
			_spawnTime += Time.deltaTime;

			//Debug.Log("Time: " + Time.time);
			if (_spawnTime > GlobalVariablesSingleton.instance.particleSpawnRate)
			{
				//yes -> new particle + set time lower
				_spawnTime -= GlobalVariablesSingleton.instance.particleSpawnRate;


				//create new particle
				//TODO: check for performance (child 1 maybe later not needed

				//Debug.Log("Time Mod 2: " + Time.time);
				_tempParticle = Instantiate(_particle, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation) as GameObject;
				//Debug.Log ("child: " + _tempParticle.transform.GetChild(0).name);
				_tempParticle.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ColorController.instance.webcamColor;
				if (_tempParticle.transform.childCount > 1)
				{
					_tempParticle.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", ColorController.instance.webcamColor);
				}
				//_tempParticle.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Emission", Color.red);

				//set layer to parent and child sprite
				//_tempParticle.gameObject.layer = 1;//LayerMask.NameToLayer("Particles");	//ParticleLayer
				_tempParticle.transform.gameObject.layer = 10;
				_tempParticle.transform.GetChild(0).gameObject.layer = 10;
				if (_tempParticle.transform.childCount > 1)
				{
					_tempParticle.transform.GetChild(0).gameObject.layer = 10;
				}
				//show or hide particle
				_tempParticle.transform.GetChild(0).gameObject.SetActive(_showParticle);
				if (_tempParticle.transform.childCount > 1)
				{
					_tempParticle.transform.GetChild(1).gameObject.SetActive(!_showParticle);
				}


				//initialize so that particles prevent collision with foerderband (but still are pushed away)
				//Physics2D.IgnoreCollision (_foerderbandCollider, _tempParticle.GetComponents<Collider2D>()[0]);	//TODO: check if [0] is the correct collider


				//Debug.Log("pos X: " + transform.position.x);
				//Debug.Log("pos Y: " + transform.position.y);
				//_tempParticle.transform.position.Set(transform.position.x, transform.position.y, 0);
				//_tempParticle.GetComponent<Rigidbody2D>().position = new Vector2 (transform.position.x, transform.position.y);



				//Debug.Log ("Constr. : " + _tempParticle.GetComponent<Rigidbody2D>().constraints);
				if (_tempParticle.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.FreezePosition)
				{
					_tempParticle.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
				}
				_tempParticle.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-_forceMaxRandom, _forceMaxRandom), -10));

				//play sound effect
				//_waterDropSound.pitch = -3;
				//_waterDropSound.Play();
				//_waterDropSound.Play(44100);

			}
		}
		
		
		//Begin
		/* Dont use begin -> use delta value and constrain it
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			// Get movement of the finger since last frame
			Vector2 touchPosition = Input.GetTouch(0).position;
			Debug.Log("touch pos: " + transform.position + " - sW: " + _screenWidth + " - sH: " + _screenHeight);
			
			// Move object across XY plane
			transform.position = new Vector2((touchPosition.x-100) / 100, 2);//.Translate(touchPosition.x / 100, touchPosition.y / 100, 0);
		}
		*/
		//Move
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {

			//transoform touch coordinates to world coordinates
			//Debug.Log ("touch d: " + Input.GetTouch(0).position + " - w: " + Camera.main.ScreenPointToRay(Input.GetTouch(0).position));
			Vector3 convertedPoint = Camera.main.ScreenPointToRay(Input.GetTouch(0).position).origin;
			convertedPoint.y = 2;
			convertedPoint.z = 0;
			//set point
			transform.position = new Vector2(convertedPoint.x, transform.position.y);

			/*
			// Get movement of the finger since last frame
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

			//use "world coordinate value" to clamp touch coordinates to world coordinates
			touchDeltaPosition.x /= 1;//00;
			touchDeltaPosition.x *= _speedFactorOfEmitter;

			// Move object across XY plane
			transform.Translate(touchDeltaPosition.x, 0, 0);

			//clamp to correct value
			//transform.position.x = Mathf.Clamp(touchDeltaPosition.x, -_screenWidth/2, _screenWidth/2);
			transform.position = new Vector2(Mathf.Clamp(transform.position.x, -_screenWidth/(2 * _mobileScreenToWorldCoordinatesConstant), _screenWidth/(2 *_mobileScreenToWorldCoordinatesConstant)), transform.position.y);
			//then move it in neg direction 1/2 screenwidth (to center it)
			
			*/
		}
		
		
	}



	// BUTTON BEHAVIOURS		##########################################################################
	public void Button_setSpawnRate(float addValue) {
		if (GlobalVariablesSingleton.instance.particleSpawnRate >= 0.5f) {
			GlobalVariablesSingleton.instance.particleSpawnRate = 0.05f;
		} else {
			GlobalVariablesSingleton.instance.particleSpawnRate += addValue;
		}
		//GlobalVariablesSingleton.instance.particleSpawnRate = _spawnRate;
	}
	public void Button_toggleWaterTap()
	{
		GlobalVariablesSingleton.instance.isWaterTapOpen = !GlobalVariablesSingleton.instance.isWaterTapOpen;
		/*if (_showParticle ) {
			_spawnRate = 0.05f;
		} else {
			_spawnRate += addValue;
		}
		*/
	}

	public void Button_toggleParticleShow() {
		_showParticle = !_showParticle;
		/*if (_showParticle ) {
			_spawnRate = 0.05f;
		} else {
			_spawnRate += addValue;
		}
		*/
	}
}
