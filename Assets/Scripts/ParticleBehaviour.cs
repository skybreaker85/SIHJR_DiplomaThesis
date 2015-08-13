using UnityEngine;
using System.Collections;

public class ParticleBehaviour : MonoBehaviour {

	private Color _tempColOwn;
	private Color _tempColOther;
	private Color _tempCollideOwn;
	private Color _tempCollideOther;

	private SpriteRenderer _ownSpriteRenderer;
	private MeshRenderer _ownMeshRenderer;
	private SpriteRenderer _otherSpriteRenderer;
	private MeshRenderer _otherMeshRenderer;

	public float _colorDiffuseValue = 0.1f;

	// Use this for initialization
	void Start () {
		_tempColOwn = Color.white;
		_tempColOther = Color.white;

		//disable own Sprite
		//transform.GetChild (0).gameObject.SetActive (false);
		//gameObject.GetChComponentsInChildren<SpriteRenderer>()[0].
		_ownSpriteRenderer = null;
		if (gameObject.GetComponentsInChildren<SpriteRenderer> ().Length > 0) {
			_ownSpriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer> () [0];
		} else {
			_ownMeshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>()[0];
		}
	}
	
	// Update is called once per frame
	/*
	void Update () {
	
	}
	*/

	void OnCollisionEnter2D(Collision2D other) {
		//call mix colors
		mixColors (other);
	}
	void OnCollisionStay2D(Collision2D other) {
		//call mix colors
		mixColors (other);
	}

	private void mixColors(Collision2D other) {
		//collide with same particle object
		if (other.gameObject.name == this.gameObject.name) {
			//Debug.Log ("Collision between: " + other.gameObject.GetInstanceID() + " and " + this.gameObject.GetInstanceID() + " - childCount: " + gameObject.GetComponentsInChildren<SpriteRenderer>().Length);
			//gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;

			if (_ownSpriteRenderer != null) {
				_tempCollideOwn = _ownSpriteRenderer.color;
				_otherSpriteRenderer = other.gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
				_tempCollideOther = _otherSpriteRenderer.color;
				
				_tempColOwn.r = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.r + 
				                 _colorDiffuseValue 		*	_tempCollideOther.r);
				_tempColOwn.g = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.g + 
				                 _colorDiffuseValue 		*	_tempCollideOther.g);
				_tempColOwn.b = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.b + 
				                 _colorDiffuseValue			*	_tempCollideOther.b);
				//and use some of color if own psrite in other sprite
				_tempColOther.r = (_colorDiffuseValue		* 	_tempCollideOwn.r + 
				                   (1f - _colorDiffuseValue)	* 	_tempCollideOther.r);
				_tempColOther.g = (_colorDiffuseValue		* 	_tempCollideOwn.g + 
				                   (1f - _colorDiffuseValue)	* 	_tempCollideOther.g);
				_tempColOther.b = (_colorDiffuseValue		* 	_tempCollideOwn.b + 
				                   (1f - _colorDiffuseValue) * 	_tempCollideOther.b);
				
				//Debug.Log ("color own: " + _tempColOwn);
				//Debug.Log ("color other: " + _tempColOther);
				
				//set colors
				_ownSpriteRenderer.color = _tempColOwn;
				_otherSpriteRenderer.color = _tempColOther;
			} else {
				_tempCollideOwn = _ownMeshRenderer.material.color;
				_otherMeshRenderer = other.gameObject.GetComponentsInChildren<MeshRenderer>()[0];
				_tempCollideOther = _otherMeshRenderer.material.color;
				
				_tempColOwn.r = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.r + 
				                 _colorDiffuseValue 		*	_tempCollideOther.r);
				_tempColOwn.g = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.g + 
				                 _colorDiffuseValue 		*	_tempCollideOther.g);
				_tempColOwn.b = ((1f - _colorDiffuseValue)	* 	_tempCollideOwn.b + 
				                 _colorDiffuseValue			*	_tempCollideOther.b);
				//and use some of color if own psrite in other sprite
				_tempColOther.r = (_colorDiffuseValue		* 	_tempCollideOwn.r + 
				                   (1f - _colorDiffuseValue)	* 	_tempCollideOther.r);
				_tempColOther.g = (_colorDiffuseValue		* 	_tempCollideOwn.g + 
				                   (1f - _colorDiffuseValue)	* 	_tempCollideOther.g);
				_tempColOther.b = (_colorDiffuseValue		* 	_tempCollideOwn.b + 
				                   (1f - _colorDiffuseValue) * 	_tempCollideOther.b);

				_ownMeshRenderer.material.SetColor("_Emission", _tempColOwn);
				_otherMeshRenderer.material.color = _tempColOther;
			}
		}

	}
}
