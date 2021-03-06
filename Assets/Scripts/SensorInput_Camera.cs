﻿using UnityEngine;
using System.Collections;

public class SensorInput_Camera : MonoBehaviour {

	//public Text _accelerometerText;
	//public Text _cameraText;
	
	private string _cameraDeviceName;
	private WebCamTexture _webCamTexture;
	public RenderTexture _webCamRenderTexture;		//there to get the height and with of the texture (to initialize the webcam feed
	public Material _webCamRenderMaterial;
	public GameObject _quadCamTexture;
	private Quaternion _baseRotation;
	
	private float _colorR = 0f;
	private float _colorG = 0f;
	private float _colorB = 0f;
	
	private int _webcamHeight;
	private int _webcamWidth;
	private int _webcamFPS;
	private int _wcH_2;
	private int _wcW_2;
	
	//public Collider2D _foerderbandCollider;
	//public Collider2D _particleCollider;

	// Use this for initialization
	void Start () {
		//_accelerometerText.text = "acc x:";
		
		_webcamHeight = _webCamRenderTexture.height;//60;//120;//240;
		_webcamWidth = _webCamRenderTexture.width;//80;//160;//320;
		_webcamFPS = 5;
		Debug.Log ("rendTex H: " + _webCamRenderTexture.height);
		
		//becasue render texture should always be a power of 2, theres no need to round
		//int wcH_2 = (int)Mathf.Round(_webcamHeight/2);
		//int wcW_2 = (int)Mathf.Round(_webcamWidth/2);
		_wcH_2 = (int)_webcamHeight/2;
		_wcW_2 = (int)_webcamWidth/2;
		
		
		
		initializeWebCamTexture();
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (WebCamTexture.devices.Length > 0) {
			//camera capture texture
			//_quadCamTexture.GetComponent<Renderer> ().material.mainTexture = _webCamTexture;
			if (_webCamTexture == null) {
				initializeWebCamTexture();
			}

			//GlobalVariablesSingleton.instance.webcamColor = _webCamTexture.GetPixel (160, 120);
			ColorController.instance.webcamColor = Color.white;
			//_colorR = (Color.white.r + Color.blue.r + Color.red.r) / 3;
			//_colorG = (Color.white.g + Color.blue.g + Color.red.g) / 3;
			//_colorB = (Color.white.b + Color.blue.b + Color.red.b) / 3;
			//Debug.Log("col: " + WebCamTexture.devices.Length);
			
			//mittenbetont
			/*
			_colorR = (
				_webCamTexture.GetPixel (140, 110).r +
				_webCamTexture.GetPixel (150, 120).r + 3 * _webCamTexture.GetPixel (160, 120).r + _webCamTexture.GetPixel (170, 120).r +
				_webCamTexture.GetPixel (160, 110).r) / 7;
			_colorG = (
				_webCamTexture.GetPixel (140, 110).g +
				_webCamTexture.GetPixel (150, 120).g + 3 * _webCamTexture.GetPixel (160, 120).g + _webCamTexture.GetPixel (170, 120).g +
				_webCamTexture.GetPixel (160, 110).g) / 7;
			_colorB = (
				_webCamTexture.GetPixel (140, 110).b +
				_webCamTexture.GetPixel (150, 120).b + 3 * _webCamTexture.GetPixel (160, 120).b + _webCamTexture.GetPixel (170, 120).b +
				_webCamTexture.GetPixel (160, 110).b) / 7;
			*/
			_colorR = (
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2-10).r +
				_webCamTexture.GetPixel (_wcW_2-10, _wcH_2   ).r + 3 * _webCamTexture.GetPixel (_wcW_2, _wcH_2).r + _webCamTexture.GetPixel (_wcW_2+10, _wcH_2).r +
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2+10).r) / 7;
			_colorG = (
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2-10).g +
				_webCamTexture.GetPixel (_wcW_2-10, _wcH_2   ).g + 3 * _webCamTexture.GetPixel (_wcW_2, _wcH_2).g + _webCamTexture.GetPixel (_wcW_2+10, _wcH_2).g +
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2+10).g) / 7;
			_colorB = (
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2-10).b +
				_webCamTexture.GetPixel (_wcW_2-10, _wcH_2   ).b + 3 * _webCamTexture.GetPixel (_wcW_2, _wcH_2).b + _webCamTexture.GetPixel (_wcW_2+10, _wcH_2).b +
				_webCamTexture.GetPixel (_wcW_2   , _wcH_2+10).b) / 7;
			//only mid
			//_colorR = _webCamTexture.GetPixel (160, 120).r;
			//_colorG = _webCamTexture.GetPixel (160, 120).g;
			//_colorB = _webCamTexture.GetPixel (160, 120).b;
			ColorController.instance.webcamColor = new Color (_colorR, _colorG, _colorB);
		}
	}

	private void initializeWebCamTexture() {
		if (WebCamTexture.devices.Length > 0) {
			
			_cameraDeviceName = WebCamTexture.devices [0].name;
			_webCamTexture = new WebCamTexture (_cameraDeviceName, _webcamWidth, _webcamHeight, _webcamFPS);
			_webCamTexture.Play ();
			
			//_quadCamTexture.GetComponent<Renderer> ().material.mainTexture = _webCamTexture;
			//_webCamRenderTexture = _webCamTexture;
			_webCamRenderMaterial.mainTexture = _webCamTexture;
			//rotating the camera upwards
			_baseRotation = _quadCamTexture.transform.rotation;
			//_quadCamTexture.transform.rotation = _baseRotation * Quaternion.AngleAxis(_webCamTexture.videoRotationAngle, Vector3.up);
			_quadCamTexture.transform.rotation = _baseRotation * Quaternion.AngleAxis (- _webCamTexture.videoRotationAngle, new Vector3 (0f, 0f, 1f));
		}
	}
}
