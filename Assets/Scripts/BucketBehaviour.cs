using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BucketBehaviour : MonoBehaviour {

	//public float _speed;
	//private Vector2 _speedVector;

	private int _id;

	private IList _particleInBucketList;

	//public Collider2D _bucketEffector;
	public Collider2D _countCollider;
	private float _tempScoreCount;
	private float _tempScoreScale;
	private bool _isBucketCounted;

	//private BucketEffectorStatus _effectorStatus;

	private Color _tempParticleColor;
	private Color _targetColor;
	private float _targetColorDifferenceR;
	private float _targetColorDifferenceG;
	private float _targetColorDifferenceB;

	public Text _debugText;
	
	private AudioSource _soundEffect;
	private float _soundEffectDuration;
	private float _soundEffectDurationCount;
	

	// Use this for initialization
	void Start () {
		//_speedVector = new Vector2 (_speed, 0f);
		//_effectorStatus = BucketEffectorStatus.Outside;

		_particleInBucketList = new ArrayList();
		_isBucketCounted = false;
		_soundEffectDuration = 0.3f;
		_soundEffectDurationCount = 0.0f;

		//at start, hide bar elements
		GameObject ch = getChild ("bar");
		if (ch != null) {
			//Debug.Log ("bar child 0: " + ch.transform.GetChild(0).name);	//should be TOP
			//Debug.Log ("bar child 1: " + ch.transform.GetChild(1).name);	//should be BOTTOM
			//Debug.Log ("bar child 2: " + ch.transform.GetChild(2).name);	//should be QUAD
			ch.transform.GetChild(0).gameObject.SetActive(false);
			ch.transform.GetChild(1).gameObject.SetActive(false);
			ch.transform.GetChild(2).gameObject.SetActive(false);
		}

		//play sound effect
		_soundEffect = GetComponent<AudioSource>();
		//_waterDropSound.pitch = -3;
		//_soundEffect.pitch = 1;
		//_waterDropSound.Play(44100);
	}
	
	// Update is called once per frame
	void Update () {

		//move the body above the foerderband
		transform.position = new Vector3 (transform.position.x + speed * Time.deltaTime, transform.position.y, 0f);


		/*
		if (_effectorStatus == BucketEffectorStatus.Inside) {
			Debug.Log ("giving velocity");
			this.GetComponent<Rigidbody2D> ().velocity = _speedVector;
			//transform.Translate(new Vector3(Time.deltaTime * _speed * 100,0f,0f));
		}
		*/
		_tempScoreScale = getScale (countBucket());

		if (_soundEffectDurationCount <= 0) {
			//stop sound
			_soundEffect.pitch = 0;
		} else {
			_soundEffect.pitch = _tempScoreScale;
			_soundEffectDurationCount -= Time.deltaTime;
		}


		//set hight of bar color
		GameObject ch = getChild ("bar");
		if (ch != null) {
			//Debug.Log ("bar child 0: " + ch.transform.GetChild(0).name);	//should be TOP
			//Debug.Log ("bar child 1: " + ch.transform.GetChild(1).name);	//should be BOTTOM
			//Debug.Log ("bar child 2: " + ch.transform.GetChild(2).name);	//should be QUAD
			if (_tempScoreScale > 0) {
				//ch.transform.GetChild(0).gameObject.SetActive(false);

				//set bottom bar as active
				ch.transform.GetChild(1).gameObject.SetActive(true);
				ch.transform.GetChild(2).gameObject.SetActive(true);
				//and scale
				//ch.transform.GetChild(2).localScale.Set(3f,1f,1f);
				ch.transform.GetChild(2).localScale = new Vector3(ch.transform.GetChild(2).localScale.x,
				                                                  _tempScoreScale,
				                                                  ch.transform.GetChild(2).localScale.z);
				ch.transform.GetChild(2).localPosition = new Vector3(ch.transform.GetChild(2).localPosition.x,
				                                                     _tempScoreScale / 2f - 0.4f,
				                                                     ch.transform.GetChild(2).localPosition.z);

				//activate top bar if gathered enough points
				if (_tempScoreScale > 0.55) {
					ch.transform.GetChild(0).gameObject.SetActive(true);
				}
			}
		}
	}


	void OnTriggerEnter2D(Collider2D other) {
		/*
		if (other.name == _bucketEffector.name) {
			Debug.Log ("Trigger Enter - " + other.name);
			_effectorStatus = BucketEffectorStatus.Entered;
		}
		*/
		switch (other.tag) {//switch (other.name) {
		/*
		case _bucketEffector.name:
			//told ya
			Debug.Log("Bucket Effector");
			break;
		*/
		case "Tag_CountCollider"://_countCollider.name:
			//Debug.Log("[" + _id + "] Count Collider with elements: " + _particleInBucketList.Count);
			//count bubbles inside collider

			if (!_isBucketCounted) {
				//Debug.Log("Count elements were: " + _tempScoreCount + "/" +_particleInBucketList.Count);
				GlobalVariablesSingleton.instance.addScoreCount(countBucket());
				_isBucketCounted = true;
			}

			//then remove bucket
			//Destroy(this);

			break;
		//case "ParticleElement 1":
		//case "ParticleElement 1(Clone)":
		case "Tag_ParticleElement":
			//Debug.Log("Particle Collider");
			//count bubbles inside collider
			_particleInBucketList.Add(other.gameObject);

			//enable sound
			_soundEffectDurationCount = _soundEffectDuration;
			//_soundEffect.pitch = 1;
			break;
		default:
			//Debug.Log("[" + _id + "] other.name enter: " + other.name);
			break;
		}
	}


	void OnTriggerExit2D(Collider2D other) {
		//Debug.Log ("[" + _id + "] OnTriggerExit2D: " + other.name + " with: " + gameObject.name);
		switch (other.tag) {//switch (other.name) {
		//case "ParticleElement 1":
		//case "ParticleElement 1(Clone)":
		case "Tag_ParticleElement":
			//Debug.Log("Particle Collider");
			//count bubbles inside collider
			if (_particleInBucketList.IndexOf(other.gameObject) >= 0)
			{
						_particleInBucketList.Remove(other.gameObject);
			}
			break;
		default:
			//Debug.Log("[" + _id + "] other.name exit: " + other.name);
			break;
		}
	}
	/*
	void OnTriggerStay2D(Collider2D other) {
		if (other.name == _bucketEffector.name) {
			Debug.Log ("Trigger Stay - " + other.name);
			_effectorStatus = BucketEffectorStatus.Inside;
		}
	}
	*/

	/*
	void OnCollisionExit2D(Collision2D other) {
		Debug.Log ("[" + _id + "] OnCollisionExit2D: " + other.collider.name);
	}
	*/

	public void setTargetColor (Color targetColor)
	{
		_targetColor = targetColor;
		GameObject ch = getChild ("targetColorSprite");
		if (ch != null) {
			ch.GetComponent<SpriteRenderer> ().color = _targetColor;
		}

		/*
		for (int i = 0; i < transform.childCount; i++) {
			Debug.Log("child name: " + transform.GetChild(i).name);
			if (transform.GetChild(i).name == "targetColorSprite") {
				transform.GetChild(i).gameObject.GetComponent<SpriteRenderer> ().color = _targetColor;
			}
		}
		*/
	}

	public void setTargetColorAlteredSeason(Color targetColor)
	{
		_targetColor = targetColor;
		GameObject ch = getChild("targetColorSpriteAlteredSeason");
		if (ch != null)
		{
			ch.GetComponent<SpriteRenderer>().color = _targetColor;
		}

		/*
		for (int i = 0; i < transform.childCount; i++) {
			Debug.Log("child name: " + transform.GetChild(i).name);
			if (transform.GetChild(i).name == "targetColorSprite") {
				transform.GetChild(i).gameObject.GetComponent<SpriteRenderer> ().color = _targetColor;
			}
		}
		*/
	}

	public void setTargetColorAlteredSun(Color targetColor)
	{
		_targetColor = targetColor;
		GameObject ch = getChild("targetColorSpriteAlteredSun");
		if (ch != null)
		{
			ch.GetComponent<SpriteRenderer>().color = _targetColor;
		}

		/*
		for (int i = 0; i < transform.childCount; i++) {
			Debug.Log("child name: " + transform.GetChild(i).name);
			if (transform.GetChild(i).name == "targetColorSprite") {
				transform.GetChild(i).gameObject.GetComponent<SpriteRenderer> ().color = _targetColor;
			}
		}
		*/
	}

	public void setTargetColorAlteredSeasonSun(Color targetColor)
	{
		_targetColor = targetColor;
		GameObject ch = getChild("targetColorSpriteAlteredSeasonSun");
		if (ch != null)
		{
			ch.GetComponent<SpriteRenderer>().color = _targetColor;
		}

		/*
		for (int i = 0; i < transform.childCount; i++) {
			Debug.Log("child name: " + transform.GetChild(i).name);
			if (transform.GetChild(i).name == "targetColorSprite") {
				transform.GetChild(i).gameObject.GetComponent<SpriteRenderer> ().color = _targetColor;
			}
		}
		*/
	}

	public void OnDestroy() {
		//Debug.Log("Element was destroyed");
		//_gpsText.text = "destroyed at " + this.transform.position.x;
		//_gpsText.text = "destroyed at " + this.transform.GetComponent<Rigidbody2D>().velocity;
		//THIS WAS USED: //_gpsText.text = "destroyed at " + this.transform.position.x + "\n__ with: " + GetComponent<Rigidbody2D>().velocity;
	}



	public int id {
		get {return _id;}
		set {_id = value;}
	}

	//with this variation of get/set: theres no need for private variable
	public float speed {
		get;
		set;
	}



	private float countBucket() {
		_tempScoreCount = 0;
		foreach (GameObject go in _particleInBucketList)
		{
			//go to sprite child (with color)
			//Debug.Log("particle Col: " + go.transform.GetChild(0).GetComponent<SpriteRenderer>().color);
			//Debug.Log("target Col: " + _targetColor);
			
			/*
			 * example:
			 * 		target color	: r0.5 g0.5 b0.5					r0.5 g0.5 b0.5
			 * 		particle color	: r1.0 g0.0 b0.5					r0.5 g0.5 b0.5
			 * -----
			 * 		vorschrift: 1-(target-particle) -> multiplikativ jeder channel
			 * 		result			: r1.5 g0.5 b1.0					r1.0 g1.0 b1.0
			 * 		punktzahl		: 1.5*0.5*1.0 = 0.75				 1.0* 1.0* 1.0 = 1.0
			 * -----
			 * 		vorschrift: 1-( | target-particle | ) -> multiplikativ jeder channel
			 * 		result			: r0.5 g0.5 b1.0
			 * 		punktzahl		:  0.5* 0.5* 1.0 = 0.25
			 * -----
			 * 		sum up color differences
			 * 		sum				: 0.25 * particleCount = 0.25% + dependant from gathered particles		=> (seems useful)
			 * 
			 */
			if (go != null) {
				_tempParticleColor = go.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
				
				_targetColorDifferenceR = 1f - Mathf.Abs(_targetColor.r - _tempParticleColor.r);
				_targetColorDifferenceG = 1f - Mathf.Abs(_targetColor.g - _tempParticleColor.g);
				_targetColorDifferenceB = 1f - Mathf.Abs(_targetColor.b - _tempParticleColor.b);
				
				_tempScoreCount += _targetColorDifferenceR * _targetColorDifferenceG * _targetColorDifferenceB;
			}
			
		}
		//Debug.Log("Count elements were: " + _tempScoreCount + "/" +_particleInBucketList.Count);
		//GlobalVariablesSingleton.instance.addScoreCount(_tempScoreCount);
		//_isBucketCounted = true;
		//Debug.Log ("[" + _id + "] bucketScore: " + _tempScoreCount);


		//!!!! Max Value of bucket should always be around 100
		//atm max 20particles fit in the bucket
		//

		//20 * 1.5*root(x, 3)
		//return 1.5f*Mathf.Pow (_tempScoreCount, 1f / 3f);
		//20 * sqrt(x)
		return 20 * Mathf.Sqrt (_tempScoreCount);
	}

	private float getScale(float count) {
		//100f depends on the max result for "20 particles inside the bucket" from countBucket()
		//at the moment there are approximately 20particles to fit in the bucket
		//so the MAX result from countBucket should always be around 100
		return Mathf.Clamp (count / 100f, 0f, .9f);
	}

	private GameObject getChild(string name) {
		for (int i = 0; i < transform.childCount; i++) {
			//Debug.Log("child name: " + transform.GetChild(i).name);
			if (transform.GetChild(i).name == name) {
				return transform.GetChild(i).gameObject;
				//transform.GetChild(i).gameObject.GetComponent<SpriteRenderer> ().color = _targetColor;
			}
		}
		return null;
	}
}


enum BucketEffectorStatus {Entered, Inside, Outside}