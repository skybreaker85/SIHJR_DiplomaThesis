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

	private bool _isPingRead;
	private bool _isPingOk;
	private UnityEngine.Ping _pingAdress;
	private WWW _unityRequest;
	private bool _readUnityRequestText;
	private int _waterResourcesCount;
	private bool _isWebRequestSend;

	private bool _isLocationRequestSend;
	private bool _isLocationAvailable;
	private LocationInfo _lastLocationInfo;
	private IDictionary _locationInfoStorage;
	private IList<OsmNodeInfo> _actualLocationInfo;

	// Use this for initialization
	void Start()
	{
		_isPingRead = false;
		_isPingOk = false;
		_pingAdress = null;
		_readUnityRequestText = false;
		_isLocationRequestSend = false;
		_isLocationAvailable = false;
		_isWebRequestSend = false;

		_locationInfoStorage = new Dictionary<string, int>();


		//sendUnityPing();
		_gpsText.text = "GPS Constructor";
		//checkInternetAvailability();
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

		//justr send request once, and maybe other times, when user changed position (-> do in subroutine)
		bool test = true;
		if (test)
		{
			if (!_isWebRequestSend)
			{
				//_lastLocationInfo;
				StartCoroutine("sendUnityRequest", test);

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
				//_lastLocationInfo;
				StartCoroutine("sendUnityRequest", test);

				_isWebRequestSend = true;
			}
		}
	}


	// INTERNET CONNECTION		##########################################################################
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
		*/

		//ping location MUST be in dot-notation
		//_pingGoogle = new Ping("http://www.google.com");
		_pingAdress = new UnityEngine.Ping("8.8.8.8");              //google		//returns true on yield
																	//_pingAdress = new UnityEngine.Ping("139.130.4.5");		//australia		//returns false(?) on yield
																	//_pingAdress = new UnityEngine.Ping("123.456.7.890");      //senseless		//returns false on yield
		yield return _pingAdress;


		Debug.Log("unity PING wait ended: " + _pingAdress.isDone);
		_gpsText.text = "unity PING wait ended: " + _pingAdress.time;

	}

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

	private IEnumerator sendUnityRequest(bool test) //remove test avterwards
	{
		Debug.Log("sendUnityRequest()");
		if (NetworkInterface.GetIsNetworkAvailable())
		{
			Debug.Log("physical inet conection available");
			_gpsText.text = "physical inet conection available";
		}

		string url;
		if (test)
		{
			url = "http://overpass-api.de/api/interpreter?data=node(around:5000,51.06,13.74)[amenity=drinking_water];out;";
		}
		else
		{
			url = "http://overpass-api.de/api/interpreter?data=node(around:5000," +
				_lastLocationInfo.latitude +
				"," + _lastLocationInfo.longitude +
				")[amenity=drinking_water];out;";
		}
		// Start a download of the given URL
		Debug.Log("unity Request create:");
		_unityRequest = new WWW(url);
		//WWW www = new WWW(url);
		//Debug.Log("unity Request send");
		//_gpsText.text = "unity Request send";

		// Wait for download to complete
		//yield return www;
		yield return _unityRequest;
		//yield return StartCoroutine("coroutine");
		Debug.Log("yield");
		// assign texture
		//Renderer renderer = GetComponent<Renderer>();
		//renderer.material.mainTexture = www.texture;
		//Debug.Log("unity Request wait ended: " + www.text);
		//_gpsText.text = "unity Request wait ended: " + www.text.Substring(0, 15);
		Debug.Log("unity Request wait ended: " + _unityRequest.text);
		_gpsText.text = "unity Request wait ended: " + _unityRequest.text.Substring(0, 15);

		///Debug.Log("unity Request: " + request.text);
		//_gpsText.text = request.text.Substring(0, 10);
		//return null;

		readUnityRequest(_unityRequest.text);
	}

	private void readUnityRequest(string text)
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
			dt = DateTime.Parse(t);	//works in editor, works on android
		}
		else
		{
			//not inside -> use DatTime.Now
			dt = DateTime.Now;
		}
		//Debug.Log("DateTime: " + dt.ToString());
		//_gpsText.text = "DateTime: " + dt.ToString();

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
				else oni.id = 0;
				if (actualNode.Attributes["id"] != null) oni.lat = float.Parse(actualNode.Attributes["lat"].Value);
				else oni.lat = 0f;
				if (actualNode.Attributes["id"] != null) oni.lon = float.Parse(actualNode.Attributes["lon"].Value);
				else oni.lon = 0f;

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
								Debug.Log("amenity found and set in node: " + oni.typeValue);
								_gpsText.text = "amenity found and set in node: " + oni.typeValue;

								break;
							//TODO: for other node types
							default:
								oni.type = WaterResources.NONE;
								oni.typeValue = null;
								break;
						}
					}
				}
			}
		}
	}

	public IEnumerator locationRetrieval()
	{
		//checked if input is enabled by user
		//komischerweise: auch wenn gps disabled ist, ist "enabled" gesetzt... mal gucken ob das an den optionseinstellungen des Android Systems liegt (könnte ich mir gut vorstellen, wegen wlan location retrieval etc.
		if (!Input.location.isEnabledByUser)
		{
			Debug.Log("No Location Enabled");
			_gpsText.text = "-Location Disabled";
			yield break;    //stops method immediatly
		}
		else
		{
			Debug.Log("No Location Enabled");
			_gpsText.text = "+Location Enabled";
			//yield break;
		}


		//Unity Example START	#############
		// Start service before querying location
		Input.location.Start();
		//Input.location.Start(10, 500);  //desiredAccuracyInMeters, updateDistanceInMeters
		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
			Debug.Log("reduce MaxWait -1");
			_gpsText.text = "reduce MaxWait -1: " + maxWait;
		}
		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			Debug.Log("Timed out");
			_gpsText.text = "Timed out";
			yield break;
		}
		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			Debug.Log("Unable to determine device location");
			_gpsText.text = "Unable to determine device location";
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			_lastLocationInfo = Input.location.lastData;
			_isLocationAvailable = true;


			Debug.Log("Location detected " + _lastLocationInfo.latitude + " " + _lastLocationInfo.longitude);
			_gpsText.text = "Location: " + _lastLocationInfo.latitude + " " + _lastLocationInfo.longitude + " " + _lastLocationInfo.altitude + " " + _lastLocationInfo.horizontalAccuracy + " " + _lastLocationInfo.timestamp;
		}
		// Stop service if there is no need to query location updates continuously
		//TODO: Stop it for now, later contineously with set: updatedistanceinmeters 
		Input.location.Stop();
		//Unity Example END	#############

		//Debug.Log("location goes on");
		//_gpsText.text = "location goes on";
	}

	public void Button_refreshGPS()
	{
		//just set boolean value to false, so Update() tries to get another location
		_isLocationRequestSend = false;

		//refresh webrequest too? set these two to false:
		_isLocationAvailable = false;
		_isWebRequestSend = false;
	}

	private float distanceBetweenPoints(float p1, float p2)
	{
		//für doc in diplomarbeit: using formulas as variation (but using haversine formula instead of Vincenty solutions for performance reasons, and good enough accuracy)
		return 0f;
	}

	// PRIVATE CLASS STORE INFOS ##########################################################################
	private class LocationStorage
	{
		private List<OsmNodeInfo> _nodes;
		public LocationStorage(string timeFromOsmXml)
		{
			time = DateTime.Parse(timeFromOsmXml);
		}
		public void addNode(OsmNodeInfo node)
		{
			_nodes.Add(node);
		}

		public List<OsmNodeInfo> nodeList
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
	}
}
