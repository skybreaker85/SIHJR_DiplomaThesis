using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net.NetworkInformation;
//using System.Net;
//using System.IO;
//using System;
using System.Xml;
using System.Collections.Generic;
using System;

/*
	OpenStreetMap Map Features
	http://wiki.openstreetmap.org/wiki/Map_Features

	amenity 	watering_place
	amenity 	water_point 
	amenity 	bar
	amenity 	biergarten
	amenity 	cafe
	amenity 	drinking_water
	amenity 	pub
	amenity 	restaurant 


	natural 	water
	natural 	wetland
	natural 	glacier
	natural 	bay
	natural 	beach
	natural 	coastline
	natural 	spring 

	landuse 	reservoir
	landuse 	basin 

	leisure 	fishing
	leisure 	marina
	leisure 	swimming_pool
	leisure 	swimming_area
	leisure 	water_park 

	shop 	kiosk
	shop 	mall
	shop 	supermarket 

	waterway 	river
	waterway 	riverbank
	waterway 	stream
	waterway 	wadi
	waterway 	drystream
	waterway 	canal
	waterway 	drain
	waterway 	ditch
	waterway 	waterfall
	waterway 	water_point 
*/
/// <summary>
///		represents the OpenStreetMap MapFeatures
/// </summary>
enum WaterResources { NONE, AMENITY, NATURAL, LANDUSE, LEISURE, SHOP, WATERWAY }

public class SensorInput_GPS_Water : MonoBehaviour
{
	/**
		maybe its something for diploma thesis !!!!!!!!!!!!!!
		unity cant operate on threads beside the main thread, so asynchroneous calls are bad... 
		one have to use the WWW class from unity
	*/


	public Text _gpsText;
	public Text _debugText;
	public Text _sunText;

	private float _refreshTimerDefault;
	private float _refreshTimer;

//	private bool _isPingRead;
//	private bool _isPingOk;
//	private UnityEngine.Ping _pingAdress;
	private WWW _unityRequest;
//	private bool _readUnityRequestText;
	//private int _waterResourcesCount;
	private bool _isWebRequestSend;
	private bool _isWebRequestRetrieved;

	private bool _isLocationRequestSend;
	private bool _isLocationAvailable;
//	private LocationInfo _lastLocationInfoRequested;
	private LocationStorage _actualWebRequestLocationInfo;
	//private IDictionary _locationInfoStorage;
	private IList _storedLocationInfo;
	private float[] _requestPositionArray;  //just for testing purpose
	private int _requestPositionArrayCounter;

	private float _waterRateBoundMax;
	private float _waterRateBoundMin;

	//for testing purpose:
	private float _walkingDistance;

	// Use this for initialization
	void Start()
	{
		_refreshTimerDefault = 5f;
		_refreshTimer = 0f;

		//_isPingRead = false;
		//_isPingOk = false;
		//_pingAdress = null;
		//_readUnityRequestText = false;
		_isLocationRequestSend = false;
		_isLocationAvailable = false;
		_isWebRequestSend = false;
		_isWebRequestRetrieved = false;

		//_locationInfoStorage = new Dictionary<string, LocationStorage>();
		_storedLocationInfo = new ArrayList();

		//just for testing purpose
		_requestPositionArray = new float[] { 51.06f, 13.74f, 51.02f, 14.21f, 51.085f, 13.683f };//1st:home, 2nd:polenz, 3rd: kaditz near location
		_requestPositionArrayCounter = 0;

		//TODO: water rate bounds (from tries), can and need to be balanced?!
		_waterRateBoundMax = 0.5f;
		_waterRateBoundMin = 0.05f;


		//for testing purpose:
		_walkingDistance = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{

		/*
		//wehen ping isnt "read" yet AND ping exists
		if (!_isPingRead && _pingAdress != null)
		{
			//check if ping is finished
			Debug.Log("is Ping done: " + _pingAdress.isDone);
			if (_pingAdress.isDone)
			{
				Debug.Log("Ping done within (ms): " + _pingAdress.time);
				_isPingRead = true;
				if (_pingAdress.time < 1000) {
					_isPingOk = true;
				}
			}
		}

		if (_isPingOk)
		{
			//do something
			Debug.Log("Ping is OK");
		}
		*/

		/*
		if (_unityRequest.isDone)
		{
			//when it is done, read text ONE
			if (!_readunityRequestText)
			{
				readUnityRequest(_unityRequest.text);
				Debug.Log("unityReq said: " + _unityRequest.text);
				_readunityRequestText = true;
			}
		}
		*/


		//check status of locatio0nservice
		//	if stopped/failed -> start up and break (do nothing)
		//	if running -> retrieve gps constantly
		//		compare distances to webrequest constantly
		//		if retrieved -> compare location to last requested location
		//			if wider away than 500m -> request new, set new location
		//			if within radius, recalculate node distances to actual location and set new spawnrate
		bool testEditor = false;
		bool testWalkingDistance = false;
		float latToCheck = 0f;
		float lonToCheck = 0f;
		if (!_isLocationRequestSend)	//to just do it once, use this variable
		{
			StartCoroutine("locationRetrieval");
			_isLocationRequestSend = true;
		} else
		{
			if (testEditor)
			{
				//TODO: remove this ... values from code, when game will be published (its just for testing purposes)
				if (testWalkingDistance) latToCheck = _requestPositionArray[_requestPositionArrayCounter * 2] + _walkingDistance;
				else latToCheck = _requestPositionArray[_requestPositionArrayCounter * 2];
                lonToCheck = _requestPositionArray[_requestPositionArrayCounter * 2 + 1];

				GlobalVariablesSingleton.instance.actualLatitude = latToCheck;
				GlobalVariablesSingleton.instance.actualLongitude = lonToCheck;

				_isLocationRequestSend = true;
				_isLocationAvailable = true;
			}
		}
		if (_isLocationAvailable)   //thats not the same var, this one turns true, when location realy IS available (not when before)
		{
			//TODO: swap this Input...values with a better retrieval (e.g. locationRetrieval())
			if (!testEditor)
			{
				if (testWalkingDistance) latToCheck = Input.location.lastData.latitude + _walkingDistance;
				else latToCheck = Input.location.lastData.latitude;
				lonToCheck = Input.location.lastData.longitude;

				//TODO: when setting up formula for retrieval, take this line with it //store latitude & longitude in global vars, its needed for sunrise/sunset
				GlobalVariablesSingleton.instance.actualLatitude = latToCheck;
				GlobalVariablesSingleton.instance.actualLongitude = lonToCheck;
			}
			//check if there is a webrequest that was sent
			if (!_isWebRequestSend)
			{
				//	\1/: no webrequest was send -> send a new request and wait to retrieve
				StartCoroutine(sendUnityRequest(latToCheck, lonToCheck));
				_isWebRequestSend = true;
				//and refresh waterspawn
				_refreshTimer = 0.0f;
            } else
			{
				if (_isWebRequestRetrieved && _refreshTimer <= 0f)
				{
					//reset timer
					_refreshTimer = _refreshTimerDefault;

					//check distance of actual location to web requested location
					//Debug.Log("havDist from Last Req Loc " + haversineDistance(latToCheck, lonToCheck, _actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude));
					//_gpsText.text = "havDist from Last Req Loc " + haversineDistance(latToCheck, lonToCheck, _actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude);

					if (haversineDistance(latToCheck, lonToCheck, _actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude) < 500)
					{
						//distance below 500m, so dont request new location info, but set new spawn rate
						changeWaterSpawnRate(latToCheck, lonToCheck);
						
						//TODO: just request new sun data when distance > 25km? or better > 0.25longitude -> store own variables
						//and set new sunset/sunrise with sunFormula
						DateTime r, s;
						//TODO: do a refresh
						SensorInput_Time.refreshSunTimes();
						//SensorInput_Time.sunFormula(_actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude, ref r, ref s);
						r = GlobalVariablesSingleton.instance.sunrise;
						s = GlobalVariablesSingleton.instance.sunset;
                        _sunText.text = "sunrise: " + r;					//do it with lat lon as arguments, dont touch globalvars
						_sunText.text += "\nsunset: " + s;                   //do it with lat lon as arguments, dont touch globalvars
						//Debug.Log("position swapped ##########################");
					}
					else
					{
						//actual position is too far away from last reqtested location
						//so go through all stored locations
						//	if no stored location is near actual location -> request new location info
						//	if near location found in stored locations -> set this as new
						bool isPositiuonStored = false;
						_actualWebRequestLocationInfo = null;

						//Debug.Log("go through _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored");
						//_gpsText.text = "go through  _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored";

						foreach (LocationStorage ls in _storedLocationInfo)
						{
							//Debug.Log("haversineDistance: " + haversineDistance(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude, ls.latitude, ls.longitude));
							//_debugText.text = "haversineDistance: " + haversineDistance(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude, ls.latitude, ls.longitude);

							if (haversineDistance(latToCheck, lonToCheck, ls.latitude, ls.longitude) < 500)
							{
								//set pos
								_actualWebRequestLocationInfo = ls;
								isPositiuonStored = true;
								//becasue an old location was read, set the spawn rate now
								//changeWaterSpawnRate(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude);
								//break foreach
								break;
							}
						}

						if (!isPositiuonStored)     //location not stored yet
						{

							//request a new one
							//maybe its enough to turn __isWebRequestSend to false, so that a new one will be send from if-switch before (see: \1/
							_isWebRequestSend = false;
							//StartCoroutine(sendUnityRequest(latToCheck, lonToCheck));
						}
					}
				} else
				{
					//not retrieved yet and not the time
					//Debug.Log("no location retrieved yet or timer is ticking");

					//count down refreshTimer
					_refreshTimer -= Time.deltaTime;

					//for testing purpose, test if one can "walk" away
					if (testWalkingDistance) _walkingDistance += 0.000005f;
				}
            }

		}



		//print status
		if (Input.location.status == LocationServiceStatus.Stopped)
		{
			//Debug.Log("Input.location.status: " + Input.location.status + " [\nlat: " + latToCheck + ", lon: " + lonToCheck + "]");
			//_gpsText.text = "Input.location.status: " + Input.location.status + " [\nlat: " + latToCheck + ",\nlon: " + lonToCheck + "]";

		}
		else
		{
			Debug.Log("loc statis was at least once: " + Input.location.status + " -- LastData[\nlat:" + latToCheck + ",\nlon:" + lonToCheck + "]\nhavDist: " + haversineDistance(latToCheck, lonToCheck, _actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude));
			_debugText.text = "loc statis was at least once: " + Input.location.status + " -- LastData[\nlat:" + latToCheck + ",\nlon:" + lonToCheck + "]\nhavDist: " + haversineDistance(latToCheck, lonToCheck, _actualWebRequestLocationInfo.latitude, _actualWebRequestLocationInfo.longitude);
        }
		//jusr send request once, and maybe other times, when user changed position (-> do in subroutine)

		/*
		bool test = false;
		if (test)
		{
			if (!_isWebRequestSend)
			{
				//TODO: check here, if there is a near location stored, or request a new one
				bool isPositiuonStored = false;


				Debug.Log("go through _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored");
				_gpsText.text = "go through  _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored";

				foreach (LocationStorage ls in _storedLocationInfo)
				{
					
					//Debug.Log("haversineDistance: " + haversineDistance(_requestPositionArray[_requestPositionArrayCounter * 2], _requestPositionArray[_requestPositionArrayCounter * 2 + 1], ls.latitude, ls.longitude) +
					//	" - while: [rLa:" + _requestPositionArray[_requestPositionArrayCounter * 2] + ", rLo:" + _requestPositionArray[_requestPositionArrayCounter * 2 + 1] + "," + ls.latitude + "," + ls.longitude + "]");
					//_debugText.text = "haversineDistance: " + haversineDistance(_requestPositionArray[_requestPositionArrayCounter * 2], _requestPositionArray[_requestPositionArrayCounter * 2 + 1], ls.latitude, ls.longitude);
					
					if (haversineDistance(_requestPositionArray[_requestPositionArrayCounter * 2], _requestPositionArray[_requestPositionArrayCounter * 2 + 1], ls.latitude, ls.longitude) < 500)
					{
						//set pos
						_actualWebRequestLocationInfo = ls;
						isPositiuonStored = true;
						//becasue an old location was read, set the spawn rate now
						changeWaterSpawnRate(_requestPositionArray[_requestPositionArrayCounter * 2], _requestPositionArray[_requestPositionArrayCounter * 2 + 1]);
						//break foreach
						break;
					}
				}

				if (!isPositiuonStored)     //location not stored yet
				{
					//request a new one
					//_lastLocationInfo;
					//StartCoroutine("sendUnityRequest", test);
					StartCoroutine(sendUnityRequest(test, _requestPositionArray[_requestPositionArrayCounter * 2], _requestPositionArray[_requestPositionArrayCounter * 2 + 1]));
				}

				_isWebRequestSend = true;
			}
		}
		else
		{
			if (!_isLocationRequestSend)
			{
				//StartCoroutine("sendUnityPing");
				StartCoroutine("locationRetrieval");
				_isLocationRequestSend = true;
			}
			if (!_isWebRequestSend && _isLocationAvailable)
			{
				//TODO: check here, if there is a near location stored, or request a new one
				bool isPositiuonStored = false;


				//Debug.Log("go through _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored");
				//_gpsText.text = "go through  _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored";

				foreach (LocationStorage ls in _storedLocationInfo)
				{
					//Debug.Log("haversineDistance: " + haversineDistance(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude, ls.latitude, ls.longitude));
					//_debugText.text = "haversineDistance: " + haversineDistance(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude, ls.latitude, ls.longitude);
				
					if (haversineDistance(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude, ls.latitude, ls.longitude) < 500)
					{
						//set pos
						_actualWebRequestLocationInfo = ls;
						isPositiuonStored = true;
						//becasue an old location was read, set the spawn rate now
						changeWaterSpawnRate(_lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude);
						//break foreach
						break;
					}
				}

				if (!isPositiuonStored)     //location not stored yet
				{
					//request a new one
					//_lastLocationInfo;
					//StartCoroutine("sendUnityRequest", test);
					StartCoroutine(sendUnityRequest(test, _lastLocationInfoRequested.latitude, _lastLocationInfoRequested.longitude));
				}

				_isWebRequestSend = true;
			}
		}
		*/
	}

	// INTERNET CONNECTION		##########################################################################
	/*
	private IEnumerator sendUnityPing()
	{
		/*
		http://etherealmind.com/what-is-the-best-ip-address-to-ping-to-test-my-internet-connection/

		Sometimes you just need an IP address to be check your internet connection. 
		My current favourite IP address is to use the Google DNS servers, 
		which are the IPv4 addresses 8.8.8.8 and 8.8.4.4.

		I have a favourite IP address to ping in Australia at 139.130.4.5 which
		is the primary name server for the largest carrier in Australia. 
		(And reminds me that latency of 500 milliseconds is normal for some people).

		I’ve also used the servers at OpenDNS 208.67.222.222 and 208.67.220.220. 
		OpenDNS provides a secure and safe DNS service which I recommend that you 
		check out for home and commercial use. Norton Connectsafe also have secure 
		DNS servers for home users at 198.153.192.1 and 198.153.194.2 that respond to ICMP requests.

		DNSResolvers.com is another DNS servers 205.210.42.205 and 64.68.200.200 as a free service from EasyDNS.
		* /

		//ping location MUST be in dot-notation
		//_pingGoogle = new Ping("http://www.google.com");
		_pingAdress = new UnityEngine.Ping("8.8.8.8");              //google		//returns true on yield
																	//_pingAdress = new UnityEngine.Ping("139.130.4.5");		//australia		//returns false(?) on yield
																	//_pingAdress = new UnityEngine.Ping("123.456.7.890");      //senseless		//returns false on yield
		yield return _pingAdress;


		Debug.Log("unity PING wait ended: " + _pingAdress.isDone);
		_gpsText.text = "unity PING wait ended: " + _pingAdress.time;

	}
	*/

	/* old std C# sendRequest

	public void checkInternetAvailability()
	{
		_gpsText.text = "checkInternetAvailability()";

		//check if there IS a network connection
		if (NetworkInterface.GetIsNetworkAvailable())
		{
			_gpsText.text = "physical inet conection available";
			//yes it is, but it doesnt mean you are connected to the internet (becasue it only checks for "physical" network)
			// -> use ping class and send a ping to a website
			System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping(); //needs to be this long, becasue unity got its own ping that i wont use
			p.PingCompleted += pingCompletedListener;
			p.Disposed += pingDisposedListener;
			try
			{
				p.SendAsync("google.com", 5000, new byte[32], new PingOptions(32, true));    //dont know what the buffer should be used for //pingoptions = how much is ticking through, true = dontFragment
				Debug.Log("inet con send async");
				_gpsText.text = "inet con send async";
			}
			catch
			{
				Debug.Log("error catched async inet send");
				_gpsText.text = "error catched async inet send";
			}
		}

	}

	private void pingDisposedListener(object sender, EventArgs e)
	{
		//not connected to internet, do something...
		Debug.Log("inet con disposed: " + e.ToString());
		_gpsText.text = "inet con disposed: " + e.ToString();
		//e.Reply.Status
	}

	private void pingCompletedListener(object sender, PingCompletedEventArgs e) //auto generated listener
	{
		_gpsText.text = "pingCompletedListener(...) [C: " + e.Cancelled + "; E: " + e.Error + "; R: " + e.Reply;
		/*
		if (e.Cancelled)
		{
			//ok connected to internet, do something...
			Debug.Log("internet connection cancelled: " + e.Cancelled);
			_gpsText.text = "internet connection cancelled: " + e.Cancelled;
		};

		if (e.Error != null)
		{
			//ok connected to internet, do something...
			Debug.Log("internet connection error: " + e.Error.ToString());
			_gpsText.text = "internet connection error: " + e.Error.ToString();
		};

		if (e.Reply.Status == IPStatus.Success)
		{
			//ok connected to internet, do something...
			Debug.Log("internet connection is up");
			_gpsText.text = "internet connection is up";

			//so its okay, lets try to send a request
			sendRequest();
		}
		* /

	}


	/**
    the checkAvailability Method must be called before!
    * /
	public void sendRequest()
	{

		//var request = (HttpWebRequest)WebRequest.Create("http://www.example.com/recepticle.aspx");
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://overpass-api.de/api/interpreter?data=(node(around:5000,51.06,13.74)[amenity~'watering_place | water_point | bar | biergarten | cafe | drinking_water | pub | restaurant'];way(around:5000,51.06,13.74)[waterway='river'];(._;>;);way(around:5000,51.06,13.74)[natural=water];(._;>;););out;");

		//API Doc: Die GetResponse-Methode führt eine synchrone Anforderung aus
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

		Debug.Log("inet String: " + responseString);
		_gpsText.text = "RS: " + responseString.Substring(0, 10);

	}
	*/

	private IEnumerator sendUnityRequest(float lat, float lon) //remove test avterwards
	{
		_isWebRequestRetrieved = false; //set this to false, so that just one request will be send

		//Debug.Log("sendUnityRequest() " + lat);
		if (NetworkInterface.GetIsNetworkAvailable())
		{
			//Debug.Log("physical inet conection available");
			//_gpsText.text = "physical inet conection available";
		}

		string url = "http://overpass-api.de/api/interpreter?data=node(around:5000," + lat + "," + lon + ")[amenity=drinking_water];out;";

		// Start a download of the given URL
		//Debug.Log("unity Request create:");
		_unityRequest = new WWW(url);
		//WWW www = new WWW(url);
		//Debug.Log("unity Request send");
		//_gpsText.text = "unity Request send";

		// Wait for download to complete
		//yield return www;
		yield return _unityRequest;
		//yield return StartCoroutine("coroutine");
		//Debug.Log("unity Request wait ended: " + www.text);
		//_gpsText.text = "unity Request wait ended: " + www.text.Substring(0, 15);
		//Debug.Log("unity Request wait ended: " + _unityRequest.text);
		//_gpsText.text = "unity Request wait ended: " + _unityRequest.text.Substring(0, 15);

		///Debug.Log("unity Request: " + request.text);
		//_gpsText.text = request.text.Substring(0, 10);
		//return null;

		readUnityRequest(_unityRequest.text, lat, lon);
	}

	// CONVERT REQUEST 		##########################################################################
	private void readUnityRequest(string text, float lat, float lon)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(text);
		XmlNode baseNode = xmlDoc.GetElementsByTagName("osm")[0]; //0 because its usually the only node (base node)

		//read timestamp
		DateTime dt;
		//usually the meta node is the second node after "note"-notation... so grab it... may hurt later (when osm changes)
		if (baseNode.ChildNodes[1].Name.Equals("meta") && baseNode.ChildNodes[1].Attributes["osm_base"] != null)
		{
			//inside -> read osm time string 
			string t = baseNode.ChildNodes[1].Attributes["osm_base"].Value;
			//and parse data
			dt = DateTime.Parse(t); //works in editor, works on android
		}
		else
		{
			//not inside -> use DatTime.Now
			dt = DateTime.Now;
		}
		//Debug.Log("DateTime: " + dt.ToString());
		//_gpsText.text = "DateTime: " + dt.ToString();

		//create location info storage
		_actualWebRequestLocationInfo = new LocationStorage(dt);
		//_actualLocationInfo.latitude = _lastLocationInfoRequested.latitude;
		//_actualLocationInfo.longitude = _lastLocationInfoRequested.longitude;
		_actualWebRequestLocationInfo.latitude = lat;
		_actualWebRequestLocationInfo.longitude = lon;


		//create osm node var holder
		OsmNodeInfo oni;
		foreach (XmlNode actualNode in baseNode.ChildNodes)
		{
			//Debug.Log("xmlDoc FirstChild Name: " + actualNode.Name.ToString());
			//_gpsText.text = "xmlDoc FirstChild Name: " + actualNode.Name.ToString();

			if (actualNode.Name.Equals("node"))
			{
				oni = new OsmNodeInfo();
				if (actualNode.Attributes["id"] != null) oni.id = long.Parse(actualNode.Attributes["id"].Value);
				else oni.id = -1;
				if (actualNode.Attributes["id"] != null) oni.lat = float.Parse(actualNode.Attributes["lat"].Value);
				else oni.lat = -999f;
				if (actualNode.Attributes["id"] != null) oni.lon = float.Parse(actualNode.Attributes["lon"].Value);
				else oni.lon = -999f;

				//also, store distance to retrieval point (necessary? or should i track it contineously
				//if (oni.lat > -900f)

				//go through sub nodes
				foreach (XmlNode subNode in actualNode.ChildNodes)
				{
					//Debug.Log("foreach node");
					//_gpsText.text = "foreach node";

					if (subNode.Name.Equals("tag") && subNode.Attributes["k"] != null)
					{
						//Debug.Log("tag+v found in node");
						//_gpsText.text = "tag+v found in node";

						switch (subNode.Attributes["k"].Value)
						{
							case "amenity":
								oni.type = WaterResources.AMENITY;
								oni.typeValue = subNode.Attributes["v"].Value;
								//debug output
								//Debug.Log("amenity found and set in node: " + oni.typeValue);
								//_gpsText.text = "amenity found and set in node: " + oni.typeValue;

								break;
							//TODO: for other node types
							default:
								oni.type = WaterResources.NONE;
								oni.typeValue = null;
								break;
						}
					}
				}

				//store in actualLocationStorage
				_actualWebRequestLocationInfo.addNode(oni);
			}
		}

		//Debug.Log("_actualLocationInfo has: " + _actualWebRequestLocationInfo.nodeList.Count + " nodes stored");
		//_gpsText.text = "_actualLocationInfo has: " + _actualWebRequestLocationInfo.nodeList.Count + " nodes stored";

		//last of it, store actual pos in storage list
		_storedLocationInfo.Add(_actualWebRequestLocationInfo);
		//Debug.Log("_actualLocationInfo added to _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored");
		//_gpsText.text = "_actualLocationInfo added to _storedLocationInfo with: " + _storedLocationInfo.Count + " locations stored";

		//set this to true to tell application that the request was ok and read
		_isWebRequestRetrieved = true;
	}

	private void changeWaterSpawnRate(float lat, float lon)
	{
		float newSpawnRate = 0;
		//float baseSpawnRate = 0.05f; 
		int waterResourcesCount = _actualWebRequestLocationInfo.nodeList.Count;
		//compute here, that one dont need to look up storage.... and its should just be a storage with no computation logic
		/*
		Summe						346,8237153	20,55083128	13,83487271	10,61718265	0,998003992	0,502008032	0,142857143
		Anz							500			21			21			21			1			1			1
		Sum/Anz						0,693647431	0,978611013	0,658803462	0,505580126	0,998003992	0,502008032	0,142857143
		0,5-(,45*(Sum/Anz) + 0,05)	0,203176285	0,060694493	0,220598269	0,297209937	0,050998004	0,298995984	0,478571429
		
		distanz-formel: 2500/((x)+2500)	-> 2500=distanceModifier (can be 5000, 10000, etc... with this formular, value of formula at 1*distanceModifier == 0.5

		clamp: 0,5-(,5*(Sum/Anz) + 0,05) while values == max and min
		*/
		float d, sumD = 0f;
		float distanceModifier = 500;
		bool isWaterResourceNear = false;
		foreach (OsmNodeInfo oni in _actualWebRequestLocationInfo.nodeList)
		{
			//distances in meters
			d = haversineDistance(lat, lon, oni.lat, oni.lon);
			//Debug.Log("havDist from waterResource: " + d);
			//sum up distance as 1..0 value
			sumD += (distanceModifier / (d + distanceModifier));

			//TODO: when using differnet weights for different types of /WaterResources/ ... then use a root of a value, e.g. weighted factor 5: sumD-formula^(1/5) -> the higher the factor, the higher the 0..1 value


			//also: look if there is a waterResource near 100m	//TODO: hier mal noch mehr einfügen, beziehungsweise andere formel finden, die "nahe" quellen stärker berücksichtigt?, aber funktioniert bisher, wenn man näher an quellen heran geht, geht die spawnrate herunter... und wenn man < 100m ist, sinkt sie drastisch... funktioniert bisher ganz gut!
			if (!isWaterResourceNear && d < 100)
			{
				//set bool to true
				isWaterResourceNear = true;
			}
		}
		//clamp it to waterrate bounds with formula
		newSpawnRate = _waterRateBoundMax - (_waterRateBoundMax * (sumD / waterResourcesCount) + _waterRateBoundMin);
		//when waterrresource was near 100m -> reduce waterrate drastically
		if (isWaterResourceNear) newSpawnRate = newSpawnRate / 2f;
		//clamp again (MathfClamp just in case)
		newSpawnRate = Mathf.Clamp(newSpawnRate, _waterRateBoundMin, _waterRateBoundMax);

		GlobalVariablesSingleton.instance.particleSpawnRate = newSpawnRate;
		Debug.Log("newSpawnRate: " + newSpawnRate + "");
		_gpsText.text = "newSpawnRate: " + newSpawnRate + "";
	}

	// GPS LOCATION			##########################################################################
	public IEnumerator locationRetrieval()
	{
		//checked if input is enabled by user
		//komischerweise: auch wenn gps disabled ist, ist "enabled" gesetzt... mal gucken ob das an den optionseinstellungen des Android Systems liegt (könnte ich mir gut vorstellen, wegen wlan location retrieval etc.
		if (!Input.location.isEnabledByUser)
		{
			//its not -> stop retrieval here

			//Debug.Log("No Location Enabled");
			//_gpsText.text = "-Location Disabled";
			yield break;    //stops method immediatly
		}


		//Unity Example START	#############
		// Start service before querying location (if not already running)
		if (Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
		{
			//service is not running at the moment -> start it

			//Input.location.Start();
			Input.location.Start(10, 10);  //desiredAccuracyInMeters, updateDistanceInMeters
			
		}

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
			//Debug.Log("reduce MaxWait -1");
			//_gpsText.text = "reduce MaxWait -1: " + maxWait;
		}
		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			//Debug.Log("Timed out");
			//_gpsText.text = "Timed out";
			yield break;
		}

		// if Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			//stop retrieval here
			
			//Debug.Log("Unable to determine device location");
			//_gpsText.text = "Unable to determine device location";
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			//_lastLocationInfoRequested = Input.location.lastData;
			_isLocationAvailable = true;


			//Debug.Log("Location detected " + _lastLocationInfoRequested.latitude + " " + _lastLocationInfoRequested.longitude);
			//_gpsText.text = "Location: " + _lastLocationInfoRequested.latitude + " " + _lastLocationInfoRequested.longitude + " " + _lastLocationInfoRequested.altitude + " " + _lastLocationInfoRequested.horizontalAccuracy + " " + _lastLocationInfoRequested.timestamp;
		}
		// Stop service if there is no need to query location updates continuously
		//TODO: Stop it for now, later contineously with set: updatedistanceinmeters 
		//Input.location.Stop();
		//Unity Example END	#############

		//Debug.Log("location goes on");
		//_gpsText.text = "location goes on";
	}

	/// <summary>
	///		calculates distance between 2 points (longitude and latitude)
	/// </summary>
	/// <param name="latitude1"></param>
	/// <param name="longitude1"></param>
	/// <param name="latitude2"></param>
	/// <param name="longitude2"></param>
	/// <returns>distance in meters</returns>
	private float haversineDistance(float latitude1, float longitude1, float latitude2, float longitude2)
	{
		/*
			http://www.movable-type.co.uk/scripts/latlong.html
			Haversine formula: 
			a = sin²(Δφ / 2) + cos φ1 ⋅ cos φ2 ⋅ sin²(Δλ / 2)
			c = 2 ⋅ atan2( √a, √(1−a) )
			d = R ⋅ c
			where   φ is latitude, λ is longitude, R is earth’s radius (mean radius = 6, 371km);
					note that angles need to be in radians to pass to trig functions!
			
			
			var R = 6371000; // metres
			var φ1 = lat1.toRadians();
			var φ2 = lat2.toRadians();
			var Δφ = (lat2 - lat1).toRadians();
			var Δλ = (lon2 - lon1).toRadians();

			var a = Math.sin(Δφ / 2) * Math.sin(Δφ / 2) +
					Math.cos(φ1) * Math.cos(φ2) *
					Math.sin(Δλ / 2) * Math.sin(Δλ / 2);
			var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

			var d = R * c;
		*/

		int R = 6371000; // metres
		float mathPi = (float)(Mathf.PI / 180);
		float φ1 = mathPi * latitude1;
		float φ2 = mathPi * latitude2;
		float Δφ = mathPi * (latitude2 - latitude1);
		float Δλ = mathPi * (longitude2 - longitude1);
		float a = (float)(
			Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
			Math.Cos(φ1) * Math.Cos(φ2) *
			Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2)
		);
		//Debug.Log("a: " + a);
		float c = (float)(2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
		//Debug.Log("c: " + c);

		float d = R * c;

		return d;
	}

	// BUTTONS		##########################################################################
	public void Button_refreshGPS()
	{
		//just set boolean value to false, so Update() tries to get another location
		_isLocationRequestSend = false;

		//refresh webrequest too? set these two to false:
		_isLocationAvailable = false;
		_isWebRequestSend = false;
	}

	public void Button_changeGPSPosition()
	{
		//swap two locations
		_requestPositionArrayCounter = (_requestPositionArrayCounter + 1) % (_requestPositionArray.Length / 2);

		//reset retrieval, to get new query
		Button_refreshGPS();

		//for testing purpose:
		_walkingDistance = 0f;

		//Debug.Log("Button_changeGPSPosition: " + _requestPositionArrayCounter);
		//_gpsText.text = "Button_changeGPSPosition: " + _requestPositionArrayCounter;
	}


	// PRIVATE CLASS STORE INFOS ##########################################################################
	private class LocationStorage
	{
		private IList _nodes;
		private DateTime _requestTime;

		public LocationStorage(DateTime dt)
		{
			this._requestTime = dt;
			this._nodes = new ArrayList();  //not typed
		}

		public void addNode(OsmNodeInfo node)
		{
			_nodes.Add(node);
		}

		public IList nodeList
		{
			get { return _nodes; }
		}
		public float latitude { get; set; }
		public float longitude { get; set; }
		public DateTime time { get; set; }

	}
	private class OsmNodeInfo
	{
		public OsmNodeInfo()
		{
			id = -1;
		}
		/// <summary>
		///		represents the osmNodeId
		/// </summary>
		public long id { get; set; }
		/// <summary>
		///		represents the osmNodeTag for amenity, etc. 
		/// </summary>
		public WaterResources type { get; set; }
		/// <summary>
		///		represents the osmNodeTagValue for amenity etc.
		/// </summary>
		public string typeValue { get; set; }
		public float lat { get; set; }
		public float lon { get; set; }
		public float distance { get; set; }
	}
}
