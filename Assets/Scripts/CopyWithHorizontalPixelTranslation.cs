using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyWithHorizontalPixelTranslation : MonoBehaviour
{
	// an image which is expected to be RGB in left half of image, and scaled Depth (blue = fore, red = background) in right half of texture
	[SerializeField] private Texture2D sourceAlbedo;
	// an image which we desire to show RGB at similar aspect ratio to sourceAlbedo, after cutting out the right half of sourceAlbedo / depth section of texture
	// we want a reference to this texture so that we can easily save it later
	private Texture2D targetAlbedo;

	// a renderer to update with our targetAlbedo data once finished
	[SerializeField] private Renderer targetRenderer;
	// a Material instance which is created in order to draw info
	[SerializeField] private Material mTargetMaterial;

	[SerializeField] float maxPixelBaseline = 10f;

	// weight vector to dot our color with. This scales different channels and ultimately gives an unsigned depth value which we wish to use
	[SerializeField] private Vector3 colorWeightToDepth01;

    // Start is called before the first frame update
    void Start()
    {
		// populate targetAlbedo
		CopyInto(sourceAlbedo, ref targetAlbedo);

		// set mainTexture property. this also creates a new Material Instance
		targetRenderer.material.mainTexture = targetAlbedo;
		// capture a reference to new Material Instance which Unity created
		mTargetMaterial = targetRenderer.material;
    }

	private void Update() {
		if (Input.GetMouseButton(0)) 
		{
			CopyInto(sourceAlbedo, ref targetAlbedo);
		}
	}

	const float minNegativeValue = -2.0f;

	/// <summary>
	/// Copies data from sourceAlbedo to targetAlbedo while applying a horizontal pixel translation according to depth information. This is a very basic view synthesis operation
	/// </summary>
	/// <param name="source">A source texture</param>
	/// <param name="destination">A texture to write into</param>
	void CopyInto(Texture2D source, ref Texture2D destination) {
		// since texture is divided into [left half = RGB, right half = Depth] we must ignore half of the texture for now
		int maxAlbedoX = source.width / 2;
		int maxAlbedoY = source.height;

		// create a depth map with default value of -2.0f. We will do depth tests and keep pixels with depth values greater than -2.0f
		float[,] depthSurface = ArrayUtil.ArrayOfValue(minNegativeValue, maxAlbedoX, maxAlbedoY);

		if (destination == null) {
			destination = new Texture2D(maxAlbedoX, source.height, source.format, false, false) {
				filterMode = FilterMode.Point,
				name = "synthesized albedo tex"
			};
		}

		for (int i = 0; i < maxAlbedoX; ++i) {
			for (int j = 0; j < maxAlbedoY; ++j) {
				destination.SetPixel(i, j, Color.gray);
			}
		}

		// the format that the demo image uses is non-overlapping R/G/B. R = background (-k2 to -k), green = (-k to +k), b = foreground (k to +k2)
		// in our case the sign is really important. other things less important. so just do this linear sum
		System.Func<Color, float> rgbdToDepth = (c) => 
		{
			// ensure normalizedDepth is in span [0... 1]
			float normalizedDepth = Vector3.Dot(colorWeightToDepth01, new Vector3(c.r, c.g, c.b));

			// then shift to the [-1 to +1] interval
			return (normalizedDepth - 0.5f) * 2;
		};
		
		for (int i = 0; i < maxAlbedoX; ++i) {
			for (int j = 0; j < maxAlbedoY; ++j) {
				// retrieve color in color section of original image
				Color albedoPixel = source.GetPixel(i, j);
				// retrieve depth in depth section of original image. convert to a float in span [-1, +1]
				float depthAtPixelXY = rgbdToDepth(source.GetPixel(maxAlbedoX + i, j));

				float baselineXY = Mathf.Cos(Time.timeSinceLevelLoad) * maxPixelBaseline;
				int parallaxXTranslationX = Mathf.RoundToInt(Mathf.Clamp(i - depthAtPixelXY * baselineXY, 0, maxAlbedoX -  1));

				// if at position p we can overwrite pixel, do so
				if (depthSurface[parallaxXTranslationX, j] < depthAtPixelXY) 
				{
					destination.SetPixel(parallaxXTranslationX, j, albedoPixel);
					depthSurface[parallaxXTranslationX, j] = depthAtPixelXY;
				}
			}
		}
		
		destination.Apply();
	}

	private void OnDestroy() 
	{
		foreach (Object obj in new Object[] { mTargetMaterial, targetAlbedo }) 
		{
			// free memory of target
			GameObject.Destroy(obj);
		}
	}

}
