using System;
using UnityEngine;

class ColorController
{
	// SINGLETON		##########################################################################
	//should be a singleton, to swap out color correction, depending on time of play (maybe implemented later)
	private static ColorController _instance = null;
	public static ColorController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ColorController();
				_instance.init();
			}
			return _instance;
		}
	}

	private void init()
	{

		//_lighten = new Color(1f, 1f, 1f);
		//_darken = new Color(0f, 0f, 0f);
	}

	// CLASS 			##########################################################################

	//private Color _darken;
	//private Color _lighten;
	public UnityEngine.Color getRandomColor()
	{
		UnityEngine.Color c = new UnityEngine.Color();
		c.r = UnityEngine.Random.Range(0f, 1f);
		c.g = UnityEngine.Random.Range(0f, 1f);
		c.b = UnityEngine.Random.Range(0f, 1f);
		c.a = 1f;
		return c;
	}
	public UnityEngine.Color webcamColor { get; set; }

	/*
	public Color blendColors(Color c1, Color c2)
	{
		return blendColors(c1, c2, 0.5f);
	}

	public Color blendColors(Color c1, Color c2, float t)
	{
		Color retCol = new Color(0f, 0f, 0f);

		//(1)	standard	(A + B) / 2
		retCol.r = (c1.r + c2.r) / 2f;
		retCol.g = (c1.g + c2.g) / 2f;
		retCol.b = (c1.b + c2.b) / 2f;
		//(2)	interpolating with: A + t(B-A)		=> case2 => violet != green
		retCol.r = c1.r + t * (c2.r - c1.r);
		retCol.g = c1.g + t * (c2.g - c1.g);
		retCol.b = c1.b + t * (c2.b - c1.b);
		//(3)	interpolating with: A + t² (3−2t)(B−A)		=> a little brighter than (2)
		//retCol.r = c1.r + t * t * (3 - 2 * t) * (c2.r - c1.r);
		//retCol.g = c1.g + t * t * (3 - 2 * t) * (c2.r - c1.g);
		//retCol.b = c1.b + t * t * (3 - 2 * t) * (c2.r - c1.b);

		return retCol;
	}
	*/
	/*
	public Color brigthenColor(Color c1, float t)
	{
		Debug.Log("lightenCol: " + _lighten + ", prevCol: " + c1);
		Color retCol = c1;
		//for( int i = 0; i < steps; i++)
		//{
		retCol = blendColors(retCol, _lighten, t);
		//}
		Debug.Log("lightenCol: " + _lighten + ", nextCol: " + retCol);
		return retCol;
	}

	public Color darkenColor(Color c1, float t)
	{
		Debug.Log("darkenCol: " + _darken + ", prevCol: " + c1);
		Color retCol = c1;
		//for (int i = 0; i < steps; i++)
		//{
		retCol = blendColors(retCol, _darken, t);
		//}
		Debug.Log("darkenCol: " + _darken + ", nextCol: " + retCol);
		return retCol;
	}
	*/

	// CLASS 			##########################################################################
	private const float _constSpringDistance = 10f;    //lower == less grey	(should not be over 10 -> brings artifacts)
	private const float _constSummerDistance = 40f;    //lower == less yellow
	private const float _constFallDistance = 10f;      //lower == less white	(should not be over 10 -> brings artifacts)
	private const float _constWinterDistance = 40f;    //lower == more blue	(should not be below 40 -> brings artifacts)
	private const float _constSpringInfluence = 2f;   //influences control alpha value of additional color: 0=none, 1=full
	private const float _constSummerInfluence = 2.95f;
	private const float _constFallInfluence = 2f;
	private const float _constWinterInfluence = 2.95f;
	private const float _constSpringClarityInfluence = 0.5f;  //spring clear makes the color brighter, more satureated... this is an arbitrary value from 0..1 to lighten up the color (0=actual clarity of color, 1=full white)
															  //private int _constFallGray = 66;					//color value for falls Grey value
	private const float _constFallGray = 0.3f;                //color value for falls Grey value
	private const float _constAxialTilt = 23.4f;              //the aproximated earth axial tilt
	private const float _constSinusAmplitudeFactor = 23.4f;   //derived from aproximated earth axial tilt
	private const float _constSinusCorrection = 1f;   //correction, becasue the 0 value for sinusFunction, doesnt align with greenwich meridian	//TODO find correct correctionvalue
	private DateTime _beginningOfAYear = new DateTime(2000, 1, 1, 0, 0, 00);//can be any year[within datetime-bounds] (becasue of trigonometric sinus), it just have to be the first second of the year

	/*
	private Color _darken;
	private Color _lighten;
	public UnityEngine.Color getRandomColor()
	{
		UnityEngine.Color c = new UnityEngine.Color();
		c.r = UnityEngine.Random.Range(0f, 1f);
		c.g = UnityEngine.Random.Range(0f, 1f);
		c.b = UnityEngine.Random.Range(0f, 1f);
		c.a = 1f;
		return c;
	}
	public UnityEngine.Color webcamColor
	{
		get; set;
	}

	public Color blendColors(Color c1, Color c2)
	{
		return blendColors(c1, c2, 0.5f);
	}

	public Color blendColors(Color c1, Color c2, float t)
	{
		Color retCol = new Color(0f, 0f, 0f);

		//(1)	standard	(A + B) / 2
		retCol.r = (c1.r + c2.r) / 2f;
		retCol.g = (c1.g + c2.g) / 2f;
		retCol.b = (c1.b + c2.b) / 2f;
		//(2)	interpolating with: A + t(B-A)		=> case2 => violet != green
		retCol.r = c1.r + t * (c2.r - c1.r);
		retCol.g = c1.g + t * (c2.g - c1.g);
		retCol.b = c1.b + t * (c2.b - c1.b);
		//(3)	interpolating with: A + t² (3−2t)(B−A)		=> a little brighter than (2)
		//retCol.r = c1.r + t * t * (3 - 2 * t) * (c2.r - c1.r);
		//retCol.g = c1.g + t * t * (3 - 2 * t) * (c2.r - c1.g);
		//retCol.b = c1.b + t * t * (3 - 2 * t) * (c2.r - c1.b);

		return retCol;
	}

	public Color brigthenColor(Color c1, float t)
	{
		Debug.Log("lightenCol: " + _lighten + ", prevCol: " + c1);
		Color retCol = c1;
		//for( int i = 0; i < steps; i++)
		//{
		retCol = blendColors(retCol, _lighten, t);
		//}
		Debug.Log("lightenCol: " + _lighten + ", nextCol: " + retCol);
		return retCol;
	}

	public Color darkenColor(Color c1, float t)
	{
		Debug.Log("darkenCol: " + _darken + ", prevCol: " + c1);
		Color retCol = c1;
		//for (int i = 0; i < steps; i++)
		//{
		retCol = blendColors(retCol, _darken, t);
		//}
		Debug.Log("darkenCol: " + _darken + ", nextCol: " + retCol);
		return retCol;
	}
	*/


	public Color getInfluencedColor(Color col)
	{
		//read rest from global vars
		return getInfluencedColor(
			GlobalVariablesSingleton.instance.Now,
			col,
			GlobalVariablesSingleton.instance.actualLatitude,
			GlobalVariablesSingleton.instance.actualLongitude);
	}

	public Color getInfluencedColor(DateTime dateTime, Color originalColor, float lat, float lon)
	{
		Color col = new Color();
		//form the Datetime to a value from 0..12 where days and months are counter
		float monthPassedSinceBeginningOfYear = (float)(dateTime - _beginningOfAYear).TotalDays;
		monthPassedSinceBeginningOfYear = (monthPassedSinceBeginningOfYear / 366) * 12;   //just divide by 366 for a year and multiply by 12 to get values from 0..12

		//botmap starts at 0, not at -latMAx/-lonMax, so correct it to fill the correct pixel
		//bitmap.SetPixel(i + latMax, j + lonMax, summerYellow(i, j));
		//bitmap.SetPixel(i + latMax, j + lonMax, winterBlue(i, j));
		//bitmap.SetPixel(i + latMax, j + lonMax, springClear(Color.Crimson, i, j));
		//bitmap.SetPixel(i + latMax, j + lonMax, fallGrey(i, j));

		//col = blendColors(summerYellow(i, j), winterBlue(i, j));
		//col = blendColors(summerYellow(i, j), fallGrey(i, j));
		//col = blendColors(springClear(actualCol, i, j, 0.3f), actualCol);
		//col = blendColors(blendColors(summerYellow(i, j), fallGrey(i, j)), blendColors(springClear(Color.Red, i, j), winterBlue(i, j)));
		Debug.Log("col at[lat:" + lat + ",lon:" + lon + "] before: " + originalColor);
		col = blendColors(
				blendColors(
					blendColors(
						summerYellow(monthPassedSinceBeginningOfYear, lat, lon, _constSummerInfluence),
						fallGrey(monthPassedSinceBeginningOfYear, lat, lon, _constFallInfluence)
					),
					blendColors(
						springClear(originalColor, monthPassedSinceBeginningOfYear, lat, lon, _constSpringClarityInfluence, _constSummerInfluence),
						winterBlue(monthPassedSinceBeginningOfYear, lat, lon, _constWinterInfluence)
					)
				),
				originalColor
			);
		Debug.Log("col afterwards: " + col);
		return col;
	}


	private Color summerYellow(float timeFrom_0_To_12, float lat, float lon, float t)
	{
		//calculate influence from function
		//Color col = Color.FromArgb(0, 255, 255, 0);


		double sin = sinusValue(SinusSeason.SUMMER, lon, timeFrom_0_To_12);
		double dist = distance(lat, sin);
		double distanceMax = (90d + _constAxialTilt) - _constSummerDistance;
		//if (lon == 80) Debug.WriteLine("[" + lat + "," + lon + "] sin: " + sinusValue(SinusSeason.SUMMER, lat, true) + ", dist: " + dist + ", distFraction: " + ((int)Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f)) + ", col: " + Color.FromArgb(255 - Math.Min(255, (int)distanceFactor(SinusSeason.SUMMER, dist, distanceMax)), 255, 255, 0));
		/*
		if (dist > 10)
		{
			return col;
		}
		else return Color.Yellow;
		*/

		//max distance can be 90 + 23.4 (the front value of equation, maybe store this in static variable)
		int a = 255 - Math.Min(255, (int)(distanceFactor(SinusSeason.SUMMER, dist, distanceMax)));
		a = Math.Min(255, (int)Math.Floor(a * t));
		//return Color.FromArgb(a, 255, 255, 0);
		return new Color(1f, 1f, 0f, a / 255f); //convertet in 0..1
	}

	private Color winterBlue(float timeFrom_0_To_12, float lat, float lon, float t)
	{
		//Color col = Color.FromArgb(0, 0, 255, 255);


		double sin = sinusValue(SinusSeason.WINTER, lon, timeFrom_0_To_12);
		double dist = distance(lat, sin);

		//cover up more winter area, esp. go deeper into the "valleys" of the sinus sun
		//		this is necessary to get a better color correction
		double distRemover = 0;
		//if (sin > 0 && lon < 0) distRemover = +Math.Abs(sin);
		//if (sin < 0 && lon > 0) distRemover = +Math.Abs(sin);
		if ((sin > 0 && lat < 0) || (sin < 0 && lat > 0)) distRemover = Math.Abs(sin);
		//distRemover = 0;	//setback for testing purposes

		double distanceMax = _constWinterDistance - distRemover; //min distance schould be 
		int dFac = (int)distanceFactor(SinusSeason.WINTER, dist, distanceMax);
		//if (lon == 80) Debug.WriteLine("[" + lat + "," + lon + "] sin: " + sinusValue(SinusSeason.NONE, lat, true) + ", dist: " + dist + ", distFraction: " + distanceFactor(SinusSeason.WINTER, dist, distanceMax) + ", col: " + Color.FromArgb(255 - Math.Min(255, (int)Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f)), 0, 255, 255));
		/*
		if (dist > 10)
		{
			return col;
		}
		else return Color.Yellow;
		*/

		//max distance can be 90 + 23.4 (the front value of equation, maybe store this in static variable)
		//return Color.FromArgb(255 - Math.Min(255, (int)Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f)), 0, 255, 255);
		//becasue it should be a "negative" comapred to the summers color... use inverted sign
		int a = Math.Max(0, Math.Min(255, dFac));
		a = Math.Min(255, (int)Math.Floor(a * t));
		//return Color.FromArgb(a, 0, 255, 255);
		return new Color(0f, 1f, 1f, a / 255f); //convertet in 0..1
	}

	private Color springClear(Color colOrig, float timeFrom_0_To_12, float lat, float lon, float lighten, float t)
	{
		//calculate influence from function
		//Color col = Color.Red;// Color.FromArgb(255, 255, 255, 255);
		//colOrig = Color.green;
		//HSB
		float hue, sat, bri;
		hue = sat = bri = 0f;
		RGBtoHSL(colOrig.r, colOrig.g, colOrig.b, ref hue, ref sat, ref bri);
		//float hue = colOrig.GetHue();
		//float sat = colOrig.GetSaturation();
		//dont use adiition & treshhold ... maybe use as factor => turns out it is much more viable
		//sat = Math.Min(1f, colOrig.GetSaturation() * (1.00f + lighten));
		sat = Math.Min(1f, sat * (1.00f + lighten));
		bri = Math.Min(1f, bri + 0.33f * lighten);
		int r, g, b;
		r = g = b = 0;

		//HSBtoRGB(hue, sat, bri, ref r, ref g, ref b);//well, false friend -> getBrighntess returns Luminance Value of HSL
		HSLtoRGB(hue, sat, bri, ref r, ref g, ref b);//well, false friend -> getBrighntess returns Luminance Value of HSL

		//Debug.WriteLine("h:" + hue + ", s:" + sat + ", b:" + bri + " == r:" + r + ", g:" + g + ", b:" + b);

		double sin = sinusValue(SinusSeason.SPRING, lon, timeFrom_0_To_12);
		double dist = distance(lat, sin);
		double distRemover = Math.Abs(sin) * 2;
		//if ((sin > 0 && lon < 0) || (sin < 0 && lon > 0)) distRemover = Math.Abs(sin);
		//distRemover = 0;	//setback for testing purposes

		//double distanceMax = (90d + 23d) - 40d + distRemover;
		double distanceMax = (90d + _constAxialTilt) - distRemover - _constSpringDistance;

		//if (lon == 80) Debug.WriteLine("[" + lat + "," + lon + "] sin: " + sinusValue(SinusSeason.SPRING, lat, true) + ", dist: " + dist + ", distFraction: " + ((int)Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f)) + ", col: " + Color.FromArgb(255 - Math.Min(255, (int)distanceFactor(SinusSeason.SPRING, dist, distanceMax)), 255, 0, 0));
		//max distance can be 90 + 23.4 (the front value of equation, maybe store this in static variable)
		//return Color.FromArgb(255 - Math.Min(255, (int)distanceFactor(SinusSeason.SPRING, dist, distanceMax)), 255, 0, 0);

		int a = 255 - Math.Min(255, (int)(distanceFactor(SinusSeason.SPRING, dist, distanceMax)));
		a = Math.Min(255, (int)Math.Floor(a * t));
		//return Color.FromArgb(a, r, g, b);
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f); //convertet in 0..1

	}
	private void HSLtoRGB(double hue, double saturation, double lightness, ref int red, ref int green, ref int blue)
	{
		//https://en.wikipedia.org/wiki/HSL_and_HSV#Converting_to_RGB
		//if the saturation is 0, then all colors are the same


		if (saturation == 0)
		{

			red = green = blue = (int)Math.Floor(lightness);
		}

		//Given a color with hue H ∈ [0°, 360°], saturation SHSV ∈ [0, 1], and value V ∈ [0, 1], we first find chroma:
		double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;

		//Then we can find a point (R1, G1, B1) along the bottom three faces of the RGB cube, with the same hue and chroma as our color (using the intermediate value X for the second largest component of this color):
		double h_s = hue / 60.0f;
		double x = c * (1 - Math.Abs(h_s % 2 - 1));
		double r1, g1, b1;

		int sectorNumber = (int)(Math.Floor(h_s));
		switch (sectorNumber)
		{
			case 0:
				r1 = c;
				g1 = x;
				b1 = 0;
				break;
			case 1:
				r1 = x;
				g1 = c;
				b1 = 0;
				break;
			case 2:
				r1 = 0;
				g1 = c;
				b1 = x;
				break;
			case 3:
				r1 = 0;
				g1 = x;
				b1 = c;
				break;
			case 4:
				r1 = x;
				g1 = 0;
				b1 = c;
				break;
			case 5:
				r1 = c;
				g1 = 0;
				b1 = x;
				break;
			default:    //hue not defined?
				r1 = g1 = b1 = 0;
				break;
		}
		//Finally, we can find R, G, and B by adding the same amount to each component, to match value:
		//double m = lightness - c;
		double m = lightness - 0.5 * c;
		red = (int)Math.Floor(255 * (r1 + m));
		green = (int)Math.Floor(255 * (g1 + m));
		blue = (int)Math.Floor(255 * (b1 + m));

	}

	private void HSBtoRGB(double hue, double saturation, double brightness, ref int red, ref int green, ref int blue)
	{
		//https://en.wikipedia.org/wiki/HSL_and_HSV#Converting_to_RGB
		//if the saturation is 0, then all colors are the same


		if (saturation == 0)
		{

			red = green = blue = (int)Math.Floor(brightness);
		}

		//Given a color with hue H ∈ [0°, 360°], saturation SHSV ∈ [0, 1], and value V ∈ [0, 1], we first find chroma:
		double c = brightness * saturation;

		//Then we can find a point (R1, G1, B1) along the bottom three faces of the RGB cube, with the same hue and chroma as our color (using the intermediate value X for the second largest component of this color):
		double h_s = hue / 60.0f;
		double x = c * (1 - Math.Abs(h_s % 2 - 1));
		double r1, g1, b1;

		int sectorNumber = (int)(Math.Floor(h_s));
		switch (sectorNumber)
		{
			case 0:
				r1 = c;
				g1 = x;
				b1 = 0;
				break;
			case 1:
				r1 = x;
				g1 = c;
				b1 = 0;
				break;
			case 2:
				r1 = 0;
				g1 = c;
				b1 = x;
				break;
			case 3:
				r1 = 0;
				g1 = x;
				b1 = c;
				break;
			case 4:
				r1 = x;
				g1 = 0;
				b1 = c;
				break;
			case 5:
				r1 = c;
				g1 = 0;
				b1 = x;
				break;
			default:    //hue not defined?
				r1 = g1 = b1 = 0;
				break;
		}
		//Finally, we can find R, G, and B by adding the same amount to each component, to match value:
		double m = brightness - c;
		red = (int)Math.Floor(255 * (r1 + m));
		green = (int)Math.Floor(255 * (g1 + m));
		blue = (int)Math.Floor(255 * (b1 + m));



		/*
		//if the saturation is 0, then all colors are the same
		if (saturation == 0)
		{

			red = green = blue = (int)Math.Floor(brightness);
		}
		else
		{
			// the color wheel consists of 6 sectors. Figure out which sector you're in.
			double sectorPos = hue / 60.0;
			int sectorNumber = (int)(Math.Floor(sectorPos));
			// get the fractional part of the sector
			double fractionalSector = sectorPos - sectorNumber;

			// calculate values for the three axes of the color. 
			double p = brightness * (1.0 - saturation);
			double q = brightness * (1.0 - (saturation * fractionalSector));
			double t = brightness * (1.0 - (saturation * (1 - fractionalSector)));
			int pi = (int)Math.Floor(255f*p);
			int qi = (int)Math.Floor(255f * q);
			int ti = (int)Math.Floor(255f * t);
			int brightnessi = (int)Math.Floor(255f * brightness);

			// assign the fractional colors to r, g, and b based on the sector the angle is in.
			switch (sectorNumber)
			{
				case 0:
					red = brightnessi;
					green = ti;
					blue = pi;
					break;
				case 1:
					red = qi;
					green = brightnessi;
					blue = pi;
					break;
				case 2:
					red = pi;
					green = brightnessi;
					blue = ti;
					break;
				case 3:
					red = pi;
					green = qi;
					blue = brightnessi;
					break;
				case 4:
					red = ti;
					green = pi;
					blue = brightnessi;
					break;
				case 5:
					red = brightnessi;
					green = pi;
					blue = qi;
					break;
					/*
					case 0:
						red = brightness;
						green = t;
						blue = p;
						break;
					case 1:
						red = q;
						green = brightness;
						blue = p;
						break;
					case 2:
						red = p;
						green = brightness;
						blue = t;
						break;
					case 3:
						red = p;
						green = q;
						blue = brightness;
						break;
					case 4:
						red = t;
						green = p;
						blue = brightness;
						break;
					case 5:
						red = brightness;
						green = p;
						blue = q;
						break;
					* /
			}
			*/
	}

	public void RGBtoHSL(float R, float G, float B, ref float h, ref float s, ref float l)
	{
		//float _R = (R / 255f);
		//float _G = (G / 255f);
		//float _B = (B / 255f);
		float _R = R;// (R / 255f);
		float _G = G;// (G / 255f);
		float _B = B;// (B / 255f);

		float _Min = Math.Min(Math.Min(_R, _G), _B);
		float _Max = Math.Max(Math.Max(_R, _G), _B);
		float _Delta = _Max - _Min;

		float H = 0;
		float S = 0;
		float L = (float)((_Max + _Min) / 2.0f);

		if (_Delta != 0)
		{
			if (L < 0.5f)
			{
				S = (float)(_Delta / (_Max + _Min));
			}
			else
			{
				S = (float)(_Delta / (2.0f - _Max - _Min));
			}


			if (_R == _Max)
			{
				H = (_G - _B) / _Delta;
			}
			else if (_G == _Max)
			{
				H = 2f + (_B - _R) / _Delta;
			}
			else if (_B == _Max)
			{
				H = 4f + (_R - _G) / _Delta;
			}
		}

		h = H * 60.0f;
		s = S;
		l = L;
		//return new HSLColor(H, S, L);
	}


	private Color fallGrey(float timeFrom_0_To_12, float lat, float lon, float t)
	{
		//calculate influence from function
		//Color col = Color.FromArgb(0, 127, 127, 127);


		double sin = sinusValue(SinusSeason.FALL, lon, timeFrom_0_To_12);
		double dist = distance(lat, sin);
		double distRemover = Math.Abs(sin) * 2;
		//if ((sin > 0 && lon < 0) || (sin < 0 && lon > 0)) distRemover = Math.Abs(sin);
		//distRemover = 0;	//setback for testing purposes

		//double distanceMax = (90d + 23d) - 40d + distRemover;
		double distanceMax = (90d + _constAxialTilt) - distRemover - _constFallDistance;

		//if (lon == 80) Debug.WriteLine("[" + lat + "," + lon + "] sin: " + sinusValue(SinusSeason.FALL, lat, true) + ", dist: " + dist + ", distFraction: " + ((int)Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f)) + ", col: " + Color.FromArgb(255 - Math.Min(255, (int)distanceFactor(SinusSeason.FALL, dist, distanceMax)), 127, 127, 127));
		//max distance can be 90 + 23.4 (the front value of equation, maybe store this in static variable)

		int a = 255 - Math.Min(255, (int)(distanceFactor(SinusSeason.FALL, dist, distanceMax)));
		a = Math.Min(255, (int)Math.Floor(a * t));
		//return Color.FromArgb(a, 66, 66, 66);
		return new Color(_constFallGray, _constFallGray, _constFallGray, a / 255f);
	}

	private float sinusValue(SinusSeason season, float lon, float timeFrom_0_To_12)
	{
		//returns for calculation of sinsvalue for months (like first intended)
		return sinusValue(season, lon, timeFrom_0_To_12, _constSinusCorrection, 6f, _constSinusAmplitudeFactor);
	}
	private float sinusValue(SinusSeason season, float lon, float timeFrom_0_To_12, float biasAdd, float biasDiv, float biasAmplitude)
	//the added bias-arguments are there to enable the calculation for daylight sinusvalue too
	{

		//use values to swap sinus computation, becasue the sinus for spring and fall is a little different
		int month, exp;
		switch (season)
		{
			case SinusSeason.SPRING:
				month = 3;  //spring is approximatly 3 month before summer			(ahead)
				exp = 5;    //should be odd N to get negative sinus values
				break;
			case SinusSeason.FALL:
				month = -3;  //fall/autumn is approximatly 3 month after summer		(behind)
				exp = 5;    //should be odd N to get negative sinus values
				break;
			case SinusSeason.WINTER:
				month = 0;  //winter equals summer (negative values from function value, its okay)
				exp = 3;//exp = 3;    //1 is normal and should be okay... maybe we can use a 3 to dont interfere with spring&fall
				break;
			case SinusSeason.SUMMER:
			default:
				month = 0;
				exp = 1;
				break;
		}

		//move value of latitude from -180 - +180  to 0-12, becasue it should fit the months, not the longitude
		lon += 180;
		//lon = (lon / 360) * 12; //convert longitude to "month adopted from latitude" to change latitude to the actual season/month it should be, the "should month" can be further compared with actual month	(not done within this function)
		lon = (lon / 360) * 2 * biasDiv; //convert longitude to "month adopted from latitude" to change latitude to the actual season/month it should be, the "should month" can be further compared with actual month	(not done within this function)
										 //the axis for "0" at starting month is not the 0-greenwitch meridian, so try to find a correction
										 //lon += _constSinusCorrection;       //correct for 1 month ... amybe it should correct for longitude some lines before
		lon += biasAdd;       //correct for 1 month ... amybe it should correct for longitude some lines before

		//return with +timefrom0to12 for actual influence and +month for spring or fall influence
		//return _constSinusAmplitudeFactor * Math.Pow(Math.Cos((lon + timeFrom_0_To_12 + month) * Math.PI / 6f), exp);
		//return _constSinusAmplitudeFactor * Mathf.Pow(Mathf.Cos((lon + timeFrom_0_To_12 + month) * Mathf.PI / 6f), exp);
		//return sinusValueRaw((lon + timeFrom_0_To_12 + month + biasAdd) * Mathf.PI / biasDiv, _constSinusAmplitudeFactor, exp);
		return sinusValueRaw((lon + timeFrom_0_To_12 + month) * Mathf.PI / biasDiv, biasAmplitude, exp);
	}

	//
	private float sinusValueRaw(float x, float amplitudeFactor, int exp)
	{
		//actually, the cosine is better so no x-correction is neccessary
		return amplitudeFactor * Mathf.Pow(Mathf.Cos(x), exp);
	}

	private double distance(float lat, double sinusValue)
	{
		return Math.Abs(lat - sinusValue);
	}

	private double distanceFactor(SinusSeason season, double dist, double distanceMax)
	{
		switch (season)
		{
			case SinusSeason.SPRING:
				break;
			case SinusSeason.FALL:
				break;
			case SinusSeason.WINTER:
				distanceMax = -distanceMax;
				break;
			case SinusSeason.SUMMER:
			default:
				break;
		}
		//change this from liniar to maybe quadric? so the transition is more smooth
		return Math.Floor(((dist + distanceMax) / (90f + 25f)) * 255f);
	}

	public Color blendColors(Color c1, Color c2)
	{
		return blendColors(c1, c2, 0.5f);
	}

	public Color blendColors(Color c1, Color c2, float t)    //remove t, it was moved to season/Color/functions
	{
		Color retCol = new Color(0f, 0f, 0f);

		//(1)	standard	(A + B) / 2
		//retCol.r = (c1.R + c2.R) / 2f;
		//retCol.g = (c1.G + c2.G) / 2f;
		//retCol.b = (c1.B + c2.B) / 2f;
		//(2)	interpolating with: A + t(B-A)		=> case2 => violet != green
		/*
		int c1_r = r(c1);
		int c1_g = g(c1);
		int c1_b = b(c1);
		int c2_r = r(c2);
		int c2_g = g(c2);
		int c2_b = b(c2);


		int c_a = Math.Max(c1.A, c2.A);// (int)Math.Floor(c1.A + t * (c2.A - c1.A));
		int c_r = (int)Math.Floor(r(c1) + t * (r(c2) - r(c1)));
		int c_g = (int)Math.Floor(g(c1) + t * (g(c2) - g(c1)));
		int c_b = (int)Math.Floor(b(c1) + t * (b(c2) - b(c1)));
		retCol = Color.FromArgb(c_a, c_r, c_g, c_b);
		*/
		/*
		retCol = Color.FromArgb(
			(int)Math.Floor(c1.A + t * (c2.A - c1.A)),
			(int)Math.Floor(r(c1) + t * (r(c2) - r(c1))),
			(int)Math.Floor(g(c1) + t * (g(c2) - g(c1))),
			(int)Math.Floor(b(c1) + t * (b(c2) - b(c1))));

		*/
		//(3) interpolating with Porter Duff Algorithmus


		/*
		https://de.wikipedia.org/wiki/Alpha_Blending

		Hat man zwei Farben A und B gegeben und möchte A über B legen, benutzt man folgende Gleichung, um den neuen Wert für die nicht transparente Endfarbe C zu erhalten:
		(1)
		C = \alpha_A A + (1 - \alpha_A) B .

		Zum Überblenden zweier transparenter Farben ist dagegen der Porter Duff Algorithmus geeignet. Dabei berechnet sich die Endfarbe C zu
		(2)
		C = \frac{ 1}
					{\alpha_C} (\alpha_A A +(1 - \alpha_A) \alpha_B B)
		und die Transparenz der Endfarbe bestimmt sich durch
		\alpha_C = \alpha_A + (1 - \alpha_A) \alpha_B
		*/

		//break if both are transparent
		//if (t == 0) return c1;
		//if (t == 1) return c2;
		if (c1.a == 0 && c2.a == 0) return new Color(0, 0, 0, 0);

		/*
		float c1_a_f = c1.A / 255.0f;
		float c1_r_f = c1.R / 255.0f;
		float c1_g_f = c1.G / 255.0f;
		float c1_b_f = c1.B / 255.0f;

		float c2_a_f = c2.A / 255.0f;
		float c2_r_f = c2.R / 255.0f;
		float c2_g_f = c2.G / 255.0f;
		float c2_b_f = c2.B / 255.0f;
		*/
		float c1_a_f = c1.a;
		float c1_r_f = c1.r;
		float c1_g_f = c1.g;
		float c1_b_f = c1.b;

		float c2_a_f = c2.a;
		float c2_r_f = c2.r;
		float c2_g_f = c2.g;
		float c2_b_f = c2.b;

		float cr_a, cr_r, cr_g, cr_b;
		/*
		if (c1.A == 1 || c2.A == 1)
		{
			//just one transparent, use (1) algorithm
			if (c1.A == 1) {
				//B is transparent
				cr_a = 1f;
				cr_r = c2_a_f * c1_r_f + (1 - c2_a_f) * c2_r_f;
				cr_g = c2_a_f * c1_g_f + (1 - c2_a_f) * c2_g_f;
				cr_b = c2_a_f * c1_b_f + (1 - c2_a_f) * c2_b_f;
			} else
			{
				//A is transparent
				cr_a = 1f;
				cr_r = c1_a_f * c1_r_f + (1 - c1_a_f) * c2_r_f;
				cr_g = c1_a_f * c1_g_f + (1 - c1_a_f) * c2_g_f;
				cr_b = c1_a_f * c1_b_f + (1 - c1_a_f) * c2_b_f;
			}
		} else
		{
			//both transparent, use (2) algorithm
			cr_a = c1_a_f + (1f - c1_a_f) * c2_a_f;
			cr_r = (1f / cr_a) * (c1_a_f * c1_r_f + (1f - c1_a_f) * c2_a_f * c2_r_f);
			cr_g = (1f / cr_a) * (c1_a_f * c1_g_f + (1f - c1_a_f) * c2_a_f * c2_g_f);
			cr_b = (1f / cr_a) * (c1_a_f * c1_b_f + (1f - c1_a_f) * c2_a_f * c2_b_f);
		}
		*/

		if (c1.a == 1 && c2.a == 1)
		{
			//(2)	interpolating with: A + t(B-A)		=> case2 => violet != green
			retCol.r = c1.r + t * (c2.r - c1.r);
			retCol.g = c1.g + t * (c2.g - c1.g);
			retCol.b = c1.b + t * (c2.b - c1.b);
		}
		else
		{
			//std algo
			cr_a = c1_a_f + (1f - c1_a_f) * c2_a_f;
			cr_r = (1f / cr_a) * (c1_a_f * c1_r_f + (1f - c1_a_f) * c2_a_f * c2_r_f);
			cr_g = (1f / cr_a) * (c1_a_f * c1_g_f + (1f - c1_a_f) * c2_a_f * c2_g_f);
			cr_b = (1f / cr_a) * (c1_a_f * c1_b_f + (1f - c1_a_f) * c2_a_f * c2_b_f);
			//Debug.Log("c1: " + c1 + "  --  c2: " + c2);
			//Debug.Log("cR: [" + cr_a);

			/*
			cr_a = (0+t)*c1_a_f + (1 - t*c1_a_f) * c2_a_f;
			cr_r = (1f / cr_a) * ((1 - t) * c1_a_f * c1_r_f + (t - c1_a_f) * c2_a_f * c2_r_f);
			cr_g = (1f / cr_a) * ((1 - t) * c1_a_f * c1_g_f + (t - c1_a_f) * c2_a_f * c2_g_f);
			cr_b = (1f / cr_a) * ((1 - t) * c1_a_f * c1_b_f + (t - c1_a_f) * c2_a_f * c2_b_f);
			*/

			/*
			int cr_a_i = (int)Math.Floor(255 * cr_a);
			int cr_r_i = (int)Math.Floor(255 * cr_r);
			int cr_g_i = (int)Math.Floor(255 * cr_g);
			int cr_b_i = (int)Math.Floor(255 * cr_b);
			*/
			//retCol = Color.FromArgb(cr_a_i, cr_r_i, cr_g_i, cr_b_i);
			float cr_a_i = cr_a;
			float cr_r_i = cr_r;
			float cr_g_i = cr_g;
			float cr_b_i = cr_b;
			retCol = new Color(cr_r_i, cr_g_i, cr_b_i, cr_a_i);
		}




		//(3)	interpolating with: A + t² (3−2t)(B−A)		=> a little brighter than (2)
		//retCol.r = c1.r + t * t * (3 - 2 * t) * (c2.r - c1.r);
		//retCol.g = c1.g + t * t * (3 - 2 * t) * (c2.r - c1.g);
		//retCol.b = c1.b + t * t * (3 - 2 * t) * (c2.r - c1.b);

		return retCol;
	}

	/*
	private static int r(Color c)
	{
		return (int)Math.Floor(Math.Max(0f, c.R - (255 - c.A)));
	}
	private static int g(Color c)
	{
		return (int)Math.Floor(Math.Max(0f, c.G - (255 - c.A)));
	}
	private static int b(Color c)
	{
		return (int)Math.Floor(Math.Max(0f, c.B - (255 - c.A)));
	}
	*/

	public Color daylightInfluencedColor(Color originalColor)
	{
		float lat = GlobalVariablesSingleton.instance.actualLatitude;
		float lon = GlobalVariablesSingleton.instance.actualLongitude;
		DateTime sunrise = GlobalVariablesSingleton.instance.sunrise;
		DateTime sunset = GlobalVariablesSingleton.instance.sunset;
		bool isSunrise = GlobalVariablesSingleton.instance.isSunrise;
		bool isSunset = GlobalVariablesSingleton.instance.isSunset;
		return daylightInfluencedColor(GlobalVariablesSingleton.instance.Now, originalColor, lat, lon, sunrise, sunset, isSunrise, isSunset);
	}

	public Color daylightInfluencedColor(DateTime dt, Color originalColor, float lat, float lon, DateTime sunrise, DateTime sunset, bool isSunrise, bool isSunset)
	{
		//suntimes were stored global -> change it

		//either the daylight influence can be calculated by a up/down shifeted triginometric function, or by shifting the hours to match the zero-values of the trigonometric function
		//since the shifting of trig function (set zero of function to sunrise/sunset) wont work for case 2 and 3, the shifted hours method is used
		//	after getting the shifted-Hour-Value, the daylight influence is calculated from the sinus value and the distance to latitude
		//	that should give a reliable "estimated-should-be"-lighting value for the daylight

		//four cases:
		//1: sunrise and sunset			//most cases
		//2: sunrise, but no sunset		//start of (ant)arctic summer
		//3: sunset but no sunrise		//end of (ant)arctic summer
		//								//start/end of arctic winter is not inside this, becasue its needed to be determined with next day
		//								//		(e.g. end of arctic winter has no sunset but a sunrise at next day)
		//								//		BUT: it is inculded in case 4 so there is no need to calculate it solely
		//4: no sunset & no sunrise		//middle of (ant)arctic winter/summer

		//float lat = GlobalVariablesSingleton.instance.actualLatitude;
		//float lon = GlobalVariablesSingleton.instance.actualLongitude;
		//DateTime sunrise = GlobalVariablesSingleton.instance.sunrise;
		//DateTime sunset = GlobalVariablesSingleton.instance.sunset;

		//the four cases mean:
		//1: sunrise/sunset make 3 parts of the day	(night before morning, day, night after evening)
		//2: there are just two parts of the day (night before morning, day)
		//3: there are just two parts of the day (day, night after evening)
		//4: there is either a whole day or a whole night
		float hoursOfDay = (float)(dt - _beginningOfAYear).TotalHours % 24;
		float hoursOfSunrise = (float)(sunrise - _beginningOfAYear).TotalHours % 24;
		float hoursOfSunset = (float)(sunset - _beginningOfAYear).TotalHours % 24;
		float shiftedHoursOfDay = 0;

		//lon = 0;
		//lon = 180;
		//Debug.Log("hours: " + hoursOfDay);


		//map the hoursOfDay Value to a sun-shining-amount value... this is done, because otherwise, the sun-sinusvalue will always set the 
		//		sunevents to <0...6...12...18...24> (midnight,sunrise,noon,sunset,midnight
		//		so its important to map the hours value to <0..rise..X..set..24>
		//		the hours :
		//				0..6 will become 0..sunrise
		//				6..18 will become sunrise..sunset
		//				18..24 will become sunset..24
		//			the "four quadrants" are actually three, because noon(highest sun position) is spared and mapped in between sunrise and sunset

		//	BUT NOW:
		//		the four cases will determine, if there is a need for the qudrants
		//		respectivly, the quadrants wont match when there is no sunset/sunrise
		//		thats why the hours wont ALWAYS be mapped  but will be mapped this way:
		//				case 1: 0..24 -> 0..24	(with mapped hours in between)
		//				case 2: 0..24 -> 0..12	(with mapped sunrise in between)
		//				case 3: 0..24->12..24	(with mapped sunset in between)
		//				case 4: 0..24->6..18	( for arctic summer) or 18..6 (for arctic winter)


		int hoursQuadrant = (int)Mathf.Floor(hoursOfDay / 6);

		//BIG TODO (Do it immediatly when gooing on
		// quadrants schould not be determiend from currenct hour, but from sunset/sunrse hour... so the quadrants are set here (not later to get shiftedHours value)
		//		what happens now: i can shift hours correct, but the quadrant-switch get false quadrant int values... becasue its calculated from actual time, but should already be calculated from sunset/sunrise times
		//		so what to do?: calculate quadrants based on time-sunrise and time-sinset and so on, check if there is a sunset/sunrise and so on and set the quadrants accordingly
		//		the cases change from 0..4 to 0..x where x = amount of correction formulas for shifting (0..6,6..18,18..24,0..18,18..24,...articWinter,arctiSummer,...)
		//			this should bring new values for the switchase
		//		the hours then can be shifted and furthermore, dont have a gap in between
		//				i.e. they change now from 18..gap..18.8	(becasue the case put the times before or after 18o'clock in different cases (but should put it in different cases when time is near before/after sunset)

		//-->	das wird eine verschachtelte if anweisung benötigen, das kann man nicht berechnen (wie vorher)

		// BUT BUT BUT BUT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//maybe this is not neccessary, becasue a use of -lon, to correct for this, seems to bring up very very useful values
		//		i.e. the full black  value (daylight = 0) istn reached at the exact sunset, but within "less than 2 virtual minutes"...	(see [dv 1])
		//		this seems to be okay, especially becasue i use  a function that gets some daylight even some minutes after sunset (like 30min) like in real life ...	(see [dv 2])

		/*if ((GlobalVariablesSingleton.instance.Now - sunset).TotalHours < 0)
		{
			hoursQuadrant = 3;
		}*/
		float hoursQuadrantRest = (hoursOfDay % 6);




		//swap sunrise/sunset values	-> 
		//	its not a swap, its a "meant correction" e.g. actual time wont be streched to 22:00 sunset, but shorted to 15:00 sunset (make actual time brighter), respectivley becasue susnet sinus == 18 (always)
		//	so: its neccessary, becasue when sunset is 22:00, the sinus should NOT become 22 as value,m becasue at 22, sinusvalue is negative (becasue 18 is 0 turning point)
		//	thyts why the time isnt directly converted to sunrise/sunset, but to the "meant" values of the sunset... e.g. at 21:30 it realy has the lightness of 18 o'clock and at 18 o'clock it really has the lightness of e.g.15 o'clock
		hoursOfSunrise = 6 + (6 - hoursOfSunrise);
		hoursOfSunset = 18 + (18 - hoursOfSunset);
		/*
		int hoursQuadrant = 0;
		float hoursQuadrantRest = 0f;
		//for testing purpose: //TODO: remove
		float[] hoursArray = new float[] { 0f, 0.001f, 5.999f, 6.001f, 11.999f, 12.001f, 17.999f, 18.001f, 23.999f };
		for (int i = 0; i < hoursArray.Length; i++)
		{
			hoursOfDay = hoursArray[i];
			hoursQuadrant = (int)Mathf.Floor(hoursOfDay / 6);
			hoursQuadrantRest = (hoursOfDay % 6);

			Debug.Log("hoursOfDay: " + hoursOfDay + ": hoursQuad: " + hoursQuadrant + ", rest: " + hoursQuadrantRest);
			//shiftedHoursOfDay = (hoursQuadrantRest / 6) * hoursOfSunrise;
			shiftedHoursOfDay = hoursOfSunrise + ((((hoursQuadrant - 1) * 6 + hoursQuadrantRest) / 12) * (hoursOfSunset - hoursOfSunrise));
			//shiftedHoursOfDay = hoursOfSunrise + (((hoursQuadrant*6 + hoursQuadrantRest) / 24) * (hoursOfSunset - hoursOfSunrise));
			//shiftedHoursOfDay = hoursOfSunrise + ((((hoursQuadrant-1) * 6 + hoursQuadrantRest) / (18)) * (12 - hoursOfSunrise));
			//shiftedHoursOfDay = hoursOfSunset - ((hoursOfSunset - 12) * ((1 - ((hoursQuadrant * 6 + hoursQuadrantRest) / 18))    ));
			//shiftedHoursOfDay = (hoursQuadrantRest / 6) * (24 - hoursOfSunset) + hoursOfSunset;
			Debug.Log("shiftedHoursOfDay: " + shiftedHoursOfDay);
		}
		*/
		

		if (isSunrise)
		{
			if (isSunset)
			{
				//case 1:
				#region case 1
				switch (hoursQuadrant)
				{
					case 0:
						//shift values from 0..6 to 0..sunrise
						shiftedHoursOfDay = (hoursQuadrantRest / 6) * hoursOfSunrise;
						break;
					case 1:
					case 2:
						//shift values from 6..18 to sunrise..sunset
						//shiftedHoursOfDay = hoursOfSunrise + (hoursQuadrant-1)*6 + (hoursQuadrantRest / 6) * hoursOfSunset;
						shiftedHoursOfDay = hoursOfSunrise + ((((hoursQuadrant - 1) * 6 + hoursQuadrantRest) / 12) * (hoursOfSunset - hoursOfSunrise));
						break;
					/*case 2:
						break;*/
					case 3:
						//shift values from 18..24 to sunset..24
						//shiftedHoursOfDay = (hoursQuadrantRest / 6) * 24 + hoursOfSunset;
						shiftedHoursOfDay = (hoursQuadrantRest / 6) * (24 - hoursOfSunset) + hoursOfSunset;
						break;
					default:        //just can be exactly 24o'clock, so its midnight
						shiftedHoursOfDay = 0;
						break;
				}
				#endregion
			}
			else
			{
				//case 2:
				#region case 2
				switch (hoursQuadrant)
				{
					case 0:
						//shift values from 0..6 to 0..sunrise
						shiftedHoursOfDay = (hoursQuadrantRest / 6) * hoursOfSunrise;
						break;
					case 1:
					case 2:
					case 3:
						//shift values from 6..24 to sunrise..12
						//shiftedHoursOfDay = hoursOfSunrise + (hoursQuadrant-1)*6 + (hoursQuadrantRest / 6) * hoursOfSunset;
						shiftedHoursOfDay = hoursOfSunrise + ((((hoursQuadrant - 1) * 6 + hoursQuadrantRest) / (18)) * (12 - hoursOfSunrise));
						break;
					default:        //just can be exactly 24o'clock, so its midnight
						shiftedHoursOfDay = 12;
						break;
				}
				#endregion
			}
		}
		else
		{
			if (isSunset)
			{
				//case 3:
				#region case 3
				switch (hoursQuadrant)
				{
					case 0:
					case 1:
					case 2:
						//shift values from 0..18 to 12..sunrset
						shiftedHoursOfDay = hoursOfSunset - ((hoursOfSunset - 12) * ((1 - ((hoursQuadrant * 6 + hoursQuadrantRest) / 18))));
						break;
					case 3:
						//shift values from 18..24 to sunset..24
						//shiftedHoursOfDay = hoursOfSunrise + (hoursQuadrant-1)*6 + (hoursQuadrantRest / 6) * hoursOfSunset;
						//shiftedHoursOfDay = hoursOfSunrise + ((((hoursQuadrant - 1) * 6 + hoursQuadrantRest) / (18)) * (12 - hoursOfSunrise));
						shiftedHoursOfDay = (hoursQuadrantRest / 6) * (24 - hoursOfSunset) + hoursOfSunset;
						break;
					default:        //just can be exactly 24o'clock, so its midnight
						shiftedHoursOfDay = 12;
						break;
				}
				#endregion
			}
			else
			{
				//this case only happens, when u are high/low enough on latitude, so dont bother an equatorial calculation artifact

				//case 4:
				//determine if its summer/winter (light or dark)

				//becasue it is not really depending on longitude if its winter  or not (the whole circle == winter OR summer)
				//		it is possible to get an approximation of the suns equinox (thats a kinda wrong word, but i know what i mean)
				//		the apoproximation can be derived from the raw sinus
				float sin = sinusValueRaw(dt.Month * Mathf.PI / 12, 1, 1);	//depending on value which month it is, int is enough for aproximation
				if (sin > 0)
				{
					//check where i am on globe
					if (lat > 0)
					{
						//it's summer in arctica
						shiftedHoursOfDay = 12;
					} else
					{
						//it's winter in antarctica
						shiftedHoursOfDay = 0;
					}
				} else
				{
					//check where i am on globe
					if (lat > 0)
					{
						//it's winter in arctica
						shiftedHoursOfDay = 0;
					}
					else
					{
						//it's summer in antarctica
						shiftedHoursOfDay = 12;
					}
				}
				//determine the height of the sun derived from latitude/longitude
			}
		}
		//the value of shiftedHoursOfDay contains a aproximated hour of the positions daytime from 0..24	(depending on position, because sunrise/sunset depends on position)
		//		now, we need to determine the sinus value of the actual position and calculate the distance from the sun's equinox('!')
		//		the distance then can be used as the "lightness" the colour should have
		//		it also reflects, weather the arctic winters/summers light is visible or not (or better: the amount of light visible in summer [theres no visible sunlight in winter])
		//		distance in degress?

		float monthPassedSinceBeginningOfYear = (float)(dt - _beginningOfAYear).TotalDays;
		monthPassedSinceBeginningOfYear = (monthPassedSinceBeginningOfYear / 366) * 12;   //just divide by 366 for a year and multiply by 12 to get values from 0..12

		float sinV = sinusValue(SinusSeason.NONE, lon, monthPassedSinceBeginningOfYear);
		float dist = (float) distance(lat, sinV);
		//shiftedHoursOfDay = 0;
		float daylight = sinusValue(SinusSeason.NONE, -lon, shiftedHoursOfDay, 0f, 12f, 1f);	//[dv 1] lon-argument can be -lon, becasue it was already considered in sunset/sunrise calculation
		Debug.Log("sinV: " + sinV + ", dist: " + dist + ", shiftedHours: " + shiftedHoursOfDay + ", daylight: " + daylight);
		//now we got the daylight value... but its more long lighed and its not like a sinus function... it appears to be more long lighted than dark
		//	so what we do: we use another function max(0,((-1*((1*(x-1))^4))+1)*0.6 + 0.4)
		//http://fooplot.com/?lang=de#W3sidHlwZSI6MCwiZXEiOiJtYXgoMCwoKC0xKigoMSooeC0xKSleNCkpKzEpKjAuNiswLjQpIiwiY29sb3IiOiIjMDAwMDAwIn0seyJ0eXBlIjoxMDAwLCJ3aW5kb3ciOlsiLTIuNTQzNjQ2ODAxOTE5OTk4IiwiMi43ODExNTMxOTgwNzk5OTc1IiwiLTEuNzIwMzcxNTExMjk1OTk4MyIsIjEuNTU2NDI4NDg4NzAzOTk4MyJdfV0-
		//	this transform daylight parameter from -1..1 to -inf...1	(only used 0..1->0..1 becasue negative values mean: darkness)
		//int funFac = 10;
		int funFac = 4;
		//daylight = Mathf.Max(0, daylight);
		//daylight = (-1 * Mathf.Pow(2 * (daylight - 0.5f), funFac)) + 1;
		//[dv 2]: following fomrula adds some daylight after sunset/before sunrise
		daylight = Mathf.Max(0f, 0.4f + 0.6f * ((-1 * Mathf.Pow(1 * (daylight - 1f), funFac)) + 1));
		//TODO: maybe dont let it be minimum 0, so it will get black... maybe its better to just end up at (arbitrary picked) 0.3?
		//		and then one can enable the light inside the phone
		Debug.Log("sinV: " + sinV + ", dist: " + dist + ", shiftedHours: " + shiftedHoursOfDay + ", daylight NEW: " + daylight);

		return blendColors(Color.black, originalColor, daylight);
	}

	private enum SinusSeason { NONE, SPRING, SUMMER, FALL, WINTER }
}