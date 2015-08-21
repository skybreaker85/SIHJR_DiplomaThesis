using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

public class SensorInput_Time : MonoBehaviour
{

	public GameObject _quad1;
	public GameObject _quad2;
	public GameObject _quad3;
	public GameObject _quad4;
	public GameObject _quad5;


	private Color _color_q1_rgb;
	private Color _color_q2_rgb;
	private Color _color_q3_rgb;

	private DateTime _sunrise;
	private DateTime _sunset;

	// Use this for initialization
	void Start()
	{
		

		//sunset/sunrise algorithm
		//sunFormula(51f, 13.7f, ref _sunrise, ref _sunset);
		refreshSunTimes();
		//Debug.Log("sunRise: " + DateTime.FromOADate(sunInfos[0]));
		//Debug.Log("sunSet: " + DateTime.FromOADate(sunInfos[1]));

	}

	public void Button_colorRefresh()
	{

		int test = 1;
		//set golors
		switch (test)
		{
			case 1: //R + Y => orange?
				_quad1.transform.GetComponent<SpriteRenderer>().color = Color.red;//.SetColor("_Emission", Color.red);
				_quad2.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f);// Color.yellow; //.SetColor("_Emission", Color.yellow);
				break;
			case 2://B + Y => greeen?
				_quad1.transform.GetComponent<SpriteRenderer>().color = Color.blue;//.SetColor("_Emission", Color.red);
				_quad2.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f);// Color.yellow; //.SetColor("_Emission", Color.yellow);
				break;
			case 3://R + G => brown?
				_quad1.transform.GetComponent<SpriteRenderer>().color = Color.red;//.SetColor("_Emission", Color.red);
				_quad2.transform.GetComponent<SpriteRenderer>().color = Color.green;// Color.yellow; //.SetColor("_Emission", Color.yellow);
				break;
			case 4://R + G => brown?
				_quad1.transform.GetComponent<SpriteRenderer>().color = new Color(0f, 0.5f, 0f);//.SetColor("_Emission", Color.red);
				_quad2.transform.GetComponent<SpriteRenderer>().color = new Color(0f, 0.5f, 0f);// Color.yellow; //.SetColor("_Emission", Color.yellow);
				break;
			default://W + B => grey?
				_quad1.transform.GetComponent<SpriteRenderer>().color = Color.white;//.SetColor("_Emission", Color.red);
				_quad2.transform.GetComponent<SpriteRenderer>().color = Color.black;
				break;
		}

		//get colors
		//_color_q1_rgb = _quad1.transform.GetComponent<MeshRenderer>().material.color;
		_color_q1_rgb = _quad1.transform.GetComponent<SpriteRenderer>().color;
		//Debug.Log("color q1: " + _color_q1_rgb);
		//_color_q2_rgb = _quad2.transform.GetComponent<MeshRenderer>().material.color;
		_color_q2_rgb = _quad2.transform.GetComponent<SpriteRenderer>().color;
		//Debug.Log("color q2: " + _color_q2_rgb);
		
		//blending colors
		_quad3.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.blendColors(_color_q1_rgb, _color_q2_rgb);
		/*
		_quad4.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.darkenColor(_color_q1_rgb, 0.25f);
		_quad5.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.brigthenColor(_color_q1_rgb, 0.25f);
		*/
		//DateTime dt = GlobalVariablesSingleton.instance.Now;
		//dt = new DateTime(2015, 1, 28);
		//dt = dt.AddHours(6);
		//dt = new DateTime(dt.Year, dt.Month, dt.Day, 12, 0, 0);
		//dt = new DateTime(2015, 3, 28);
		//_quad4.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.getInfluencedColor(dt, _color_q1_rgb, GlobalVariablesSingleton.instance.actualLatitude, GlobalVariablesSingleton.instance.actualLongitude);
		//_quad4.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.getInfluencedColor(dt, _color_q1_rgb, GlobalVariablesSingleton.instance.actualLatitude, GlobalVariablesSingleton.instance.actualLongitude);
		_quad4.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.daylightInfluencedColor(_color_q1_rgb);
		//dt = new DateTime(2015, 6, 28);
		//dt = new DateTime(2015, 9, 28);
		_quad5.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.getInfluencedColor(_color_q1_rgb);
		//_quad5.transform.GetComponent<SpriteRenderer>().color = ColorController.instance.getInfluencedColor(_color_q1_rgb);


	}

	public void Button_changeMonth()
	{
		GlobalVariablesSingleton.instance.Now = GlobalVariablesSingleton.instance.Now.AddMonths(1);
	}
	public void Button_changeHour()
	{
		GlobalVariablesSingleton.instance.Now = GlobalVariablesSingleton.instance.Now.AddHours(1);
	}
	public void Button_changeMinute()
	{
		GlobalVariablesSingleton.instance.Now = GlobalVariablesSingleton.instance.Now.AddMinutes(1);
	}

	public static void refreshSunTimes()
	{
		//set references for ref parameter
		DateTime r, s;
		r = s = DateTime.Now;
		bool isr, iss;
		isr = iss = true;

		//request times
		sunFormula(
			GlobalVariablesSingleton.instance.actualLatitude,
			GlobalVariablesSingleton.instance.actualLongitude,
			ref r,
			ref s,
			ref isr,
			ref iss);

		//store global
		GlobalVariablesSingleton.instance.sunrise = r;
		GlobalVariablesSingleton.instance.sunset = s;
		GlobalVariablesSingleton.instance.isSunrise = isr;
		GlobalVariablesSingleton.instance.isSunset = iss;
	}

	private static void sunFormula(float lat, float lon, ref DateTime sunrise, ref DateTime sunset, ref bool isSunrise, ref bool isSunset)
	{
		//https://en.wikipedia.org/wiki/Sunrise_equation
		//
		//https://en.wikipedia.org/wiki/Julian_day said: the Julian day number for the day starting at 12:00 UT on January 1, 2000, was 2,451,545
		//	Julian day number 0 assigned to the day starting at noon on January 1, 4713 BC, proleptic Julian calendar (November 24, 4714 BC, in the proleptic Gregorian calendar)

		/*
		Calculate current Julian cycle
			n^{\star} = J_{date} - 2451545.0009 - \dfrac{l_w}{360^\circ}
			n = \left\lfloor n^{\star} + \frac{1}{2} \right\rfloor
		where:
			J_{date} is the Julian date;
			l_\omega is the longitude west (west is positive, east is negative) of the observer on the Earth;
			n is the Julian cycle since Jan 1st, 2000. 
		
		Approximate solar noon
			J^{\star} = 2451545.0009 + \dfrac{l_w}{360^\circ} + n
		where:
			J^{\star} is an approximation of solar noon at l_w.
		
		Solar mean anomaly
			M = [357.5291 + 0.98560028 \times (J^{\star} - 2451545)] \mod 360
		where:
			M is the solar mean anomaly.
		
		Equation of the center
			C = 1.9148 \sin(M) + 0.0200 \sin(2 M) + 0.0003 \sin(3 M)
		where:
			C is the Equation of the center.
		
		Ecliptic longitude
			\lambda = (M + 102.9372 + C + 180) \mod 360
		where:
			λ is the ecliptic longitude.
		
		Solar transit
			J_{transit} = J^{\star} + 0.0053 \sin M - 0.0069 \sin \left( 2 \lambda \right) 
		where:
			Jtransit is the hour angle for solar transit (or solar noon).
		
		Declination of the Sun
			\sin \delta = \sin \lambda \times \sin 23.45^\circ 
		where:
			\delta is the declination of the sun.
		
		Hour angle
			This is the equation from above with corrections for astronomical refraction and solar disc diameter.
			\cos \omega_\circ = \dfrac{\sin(-0.83^\circ) - \sin \phi \times \sin \delta}{\cos \phi \times \cos \delta}
		where:
			ωo is the hour angle;
			\phi is the north latitude of the observer (north is positive, south is negative) on the Earth.
			For observations on a sea horizon an elevation-of-observer correction, add -1.15^\circ\sqrt{\text{elevation in feet}}/60^\circ, or -2.076^\circ\sqrt{\text{elevation in metres}}/60^\circ to the -0.83° in the numerator's sine term. This corrects for both apparent dip and terrestrial refraction. For example, for an observer at 10,000 feet, add (-115°/60°) or about -1.92° to -0.83°.
		
		Calculate sunrise and sunset
			J_{set} = J_{transit} + \dfrac{\omega_\circ}{360^\circ}
			J_{rise} = J_{transit} - \dfrac{\omega_\circ}{360^\circ}
		where:
			Jset is the actual Julian date of sunset;
			Jrise is the actual Julian date of sunrise. 
		*/


		//if (GlobalVariablesSingleton.instance.actualLatitude < -180) GlobalVariablesSingleton.instance.actualLatitude = 51.06f;	//for testing purposes
		//if (GlobalVariablesSingleton.instance.actualLongitude < -180) GlobalVariablesSingleton.instance.actualLongitude = 13.74f;   //for testing purposes
		//if (GlobalVariablesSingleton.instance.actualLatitude < -180) GlobalVariablesSingleton.instance.actualLatitude = 11.06f; //for testing purposes
		//if (GlobalVariablesSingleton.instance.actualLongitude < -180) GlobalVariablesSingleton.instance.actualLongitude = 43.74f;   //for testing purposes

		/*
		DateTime now = DateTime.Now;
		DateTime julianStart = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
		float julianStartNumber = 2451545.0009f;
		JulianCalendar jc = new JulianCalendar();
		DateTime julianNow = DateTime.Now; // jc.ToDateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
		Debug.Log("now: " + now);
		Debug.Log("julianNow: " + julianNow);
		Debug.Log("datetime2000: " + julianStart);
		Debug.Log("julian2000: " + jc.ToDateTime(julianStart.Year, julianStart.Month, julianStart.Day, julianStart.Hour, julianStart.Minute, julianStart.Second, julianStart.Millisecond));

		double j_date = (julianNow - julianStart).TotalDays;
		//n_star dont need to substract the Julian day number for the day starting at 12:00 UT on January 1, 2000, was 2,451,545
		double n_start = j_date - (GlobalVariablesSingleton.instance.actualLatitude / 360f);
		Debug.Log("n_start: " + n_start);
		double n = Math.Floor(n_start - 0.5);
		Debug.Log("n: " + n);

		double J_star = (float) (julianStartNumber + (GlobalVariablesSingleton.instance.actualLatitude / 360) + n);
		Debug.Log("J_star: " + J_star);

		double M = (357.5291f + 0.98560028f * (J_star - julianStartNumber)) % 360;
		Debug.Log("M: " + M);

		double C = 1.9148f * Math.Sin(M) + 0.0200f * Math.Sin(2 * M) + 0.0003f * Math.Sin(3 * M);
		Debug.Log("C: " + C);

		//ecliptic longitude, thats why i dont need to retrieve the actual long loc
		double lambda = (M + 102.9372f + C + 180) % 360;
		Debug.Log("lambda: " + lambda);

		double J_transit = J_star + 0.0053f * Math.Sin(M) - 0.0069f * Math.Sin(2 * lambda);
		Debug.Log("J_transit: " + J_transit);

		double sin_delta = Math.Sin(lambda) * Math.Sin((Math.PI / 180) * 23.45f);
		Debug.Log("sin_delta: " + sin_delta);

		double cos_omega_zero = (Math.Sin((Math.PI / 180) * -0.83f) - Math.Sin((Math.PI / 180) * GlobalVariablesSingleton.instance.actualLongitude) * sin_delta) / (Math.Cos((Math.PI / 180) * GlobalVariablesSingleton.instance.actualLongitude) * Math.Cos(Math.Asin(sin_delta)));
		Debug.Log("cos_omega_zero: " + cos_omega_zero);

		double omega_zero = Math.Acos(cos_omega_zero);
		Debug.Log("omega_zero: " + omega_zero);
		Debug.Log("omega_zero/Pi: " + (omega_zero/Math.PI));

		double J_set = J_transit + omega_zero/Math.PI;
		double J_rise = J_transit - omega_zero / Math.PI;
		Debug.Log("J_set: " + J_set);
		Debug.Log("J_rise: " + J_rise);
		*/



		DateTime date = DateTime.Today;
		//bool isSunrise = false;
		//bool isSunset = false;
		//DateTime sunrise = DateTime.Now;
		//DateTime sunset = DateTime.Now;

		//int latH, latM, latS, lonH, lonM, lonS;
		//latH = (int)Mathf.Floor(GlobalVariablesSingleton.instance.actualLatitude);
		//latM = (int)(((GlobalVariablesSingleton.instance.actualLatitude - latH) * 60) % 60);
		//latS = (int)(((GlobalVariablesSingleton.instance.actualLatitude - latH - (latM/60)) * 3600) % 3600);
		//lonH = (int)Mathf.Floor(GlobalVariablesSingleton.instance.actualLongitude);
		//lonM = (int)(((GlobalVariablesSingleton.instance.actualLongitude - lonH) * 60) % 60);
		//lonS = (int)(((GlobalVariablesSingleton.instance.actualLongitude - lonH - (lonM / 60)) * 3600) % 3600);
		//Debug.Log("latH: " + latH + ", latM: " + latM + ", latS: " + latS);

		//using the class from http://www.codeproject.com/Articles/29306/C-Class-for-Calculating-Sunrise-and-Sunset-Times
		//decimal lat/lon
		SunTimes.Instance.CalculateSunRiseSetTimes(
			lat,
			lon,
			date, ref sunrise, ref sunset,
			ref isSunrise, ref isSunset);

		/* 
		//Coords:
		SunTimes.Instance.CalculateSunRiseSetTimes(
			new SunTimes.LatitudeCoords (latH, latM, latS, SunTimes.LatitudeCoords.Direction.North),
			new SunTimes.LongitudeCoords (lonH, lonM, lonS, SunTimes.LongitudeCoords.Direction.East),
			date, ref sunrise, ref sunset,
			ref isSunrise, ref isSunset);
		*/
		// Print out the Sunrise and Sunset times
		Debug.Log(date + ": Sunrise @" + sunrise.ToString("HH:mm") + " Sunset @" + sunset.ToString("HH: mm"));


		//return new double[]{ J_set - julianStartNumber, J_rise - julianStartNumber };
		//return new DateTime[]{ sunrise, sunset };
	}

	// Update is called once per frame
	void Update()
	{

	}
}
