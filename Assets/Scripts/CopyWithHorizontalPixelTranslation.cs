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

    // Start is called before the first frame update
    void Start()
    {
		// populate targetAlbedo
		CopyInto(sourceAlbedo, out targetAlbedo);

		// set mainTexture property. this also creates a new Material Instance
		targetRenderer.material.mainTexture = targetAlbedo;
		// capture a reference to new Material Instance which Unity created
		mTargetMaterial = targetRenderer.material;
    }
	
	/// <summary>
	/// Copies data from sourceAlbedo to targetAlbedo while applying a horizontal pixel translation according to depth information. This is a very basic view synthesis operation
	/// </summary>
	/// <param name="source">A source texture</param>
	/// <param name="destination">A texture to write into</param>
	void CopyInto(Texture2D source, out Texture2D destination) {
		// since texture is divided into [left half = RGB, right half = Depth] we must ignore half of the texture for now
		destination = new Texture2D(source.width / 2, source.height, source.format, false, false) {
			filterMode = FilterMode.Point,
			name = "right albedo tex"
		};

		for (int i = 0; i < source.width / 2; ++i) {
			for (int j = 0; j < source.width; ++j) {
				destination.SetPixel(i, j, source.GetPixel(i, j));
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
