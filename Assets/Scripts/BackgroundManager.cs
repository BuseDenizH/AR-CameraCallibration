using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Texture2D[] images; // Array of background images
    public Image canvasImage; // Reference to the Image component on the Canvas

    private int currentImageIndex = 0;

    void Start()
    {
        // Load the first image into the canvas
        UpdateBackgroundImage();
    }

    public void UpdateBackgroundImage()
    {
        if (images.Length == 0) return;

        // Convert the Texture2D to a Sprite and assign it
        canvasImage.sprite = Sprite.Create(
            images[currentImageIndex],
            new Rect(0, 0, images[currentImageIndex].width, images[currentImageIndex].height),
            new Vector2(0.5f, 0.5f)
        );
    }

    public void NextImage()
    {
        currentImageIndex = (currentImageIndex + 1) % images.Length;
        UpdateBackgroundImage();
    }

    public void PreviousImage()
    {
        currentImageIndex = (currentImageIndex - 1 + images.Length) % images.Length;
        UpdateBackgroundImage();
    }
}
