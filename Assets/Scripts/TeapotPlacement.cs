using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeapotPlacement : MonoBehaviour
{
    public GameObject teapotPrefab; // Çaydanlık prefab
    public Transform teapotParent; // Çaydanlıkların atanacağı parent
    public Texture2D[] images; // Arka plan resimleri
    public Canvas canvas; // Resimlerin gösterileceği UI canvas
    private List<Vector3>[] referencePointsPerImage; // Her resim için referans noktaları

    private int currentImageIndex = 0;
    private bool teapotPlaced = false;

    void Start()
    {
        // Tüm resimler için referans noktaları listesi oluşturma
        referencePointsPerImage = new List<Vector3>[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            referencePointsPerImage[i] = new List<Vector3>();
        }

        UpdateBackgroundImage();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol tıklama
        {
            AddReferencePoint();

            // 3 nokta seçildiğinde çaydanlığı yerleştirme
            if (referencePointsPerImage[currentImageIndex].Count == 3)
            {
                CalculateRedDot();
            }
        }
    }

    public void NextImage()
    {
        currentImageIndex = (currentImageIndex + 1) % images.Length;
        UpdateBackgroundImage();
        ResetScene(); // Eski noktaları ve çaydanlıkları temizleme
    }

    public void PreviousImage()
    {
        currentImageIndex = (currentImageIndex - 1 + images.Length) % images.Length;
        UpdateBackgroundImage();
        ResetScene(); // Eski noktaları ve çaydanlıkları temizleme
    }

    private void UpdateBackgroundImage()
    {
        // Canvas üzerindeki resmi güncelleme
        Image canvasImage = canvas.GetComponentInChildren<Image>();
        canvasImage.sprite = Sprite.Create(
            images[currentImageIndex],
            new Rect(0, 0, images[currentImageIndex].width, images[currentImageIndex].height),
            new Vector2(0.5f, 0.5f)
        );
    }

    private void AddReferencePoint()
    {
        if (referencePointsPerImage[currentImageIndex].Count >= 3)
        {
            Debug.LogWarning("Already 3 reference points added for this image.");
            return;
        }

        // Tıklanan noktayı dünya pozisyonuna çevirme
        Vector3 screenPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane + 1));

        // Referans noktalarını listeye ekleme
        referencePointsPerImage[currentImageIndex].Add(worldPosition);

        // Küreyi sahnede oluşturma
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = worldPosition;
        sphere.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        sphere.GetComponent<Renderer>().material.color = Color.red;
        sphere.tag = "ReferenceSphere"; // Etiket ekleme

        Debug.Log($"Added reference point at: {worldPosition} for image {currentImageIndex + 1}");
    }

    private void CalculateRedDot()
    {
        if (teapotPlaced)
        {
            Debug.LogWarning("Teapot already placed for this image.");
            return;
        }

        if (referencePointsPerImage[currentImageIndex].Count < 3)
        {
            Debug.LogError("At least 3 reference points are needed to calculate the red dot position.");
            return;
        }

        // Kırmızı noktanın pozisyonunu hesaplama
        Vector3 redDotPosition = (referencePointsPerImage[currentImageIndex][0] +
                                  referencePointsPerImage[currentImageIndex][1] +
                                  referencePointsPerImage[currentImageIndex][2]) / 3;

        // Çaydanlığı sahnede yerleştirme
        GameObject teapot = Instantiate(teapotPrefab, redDotPosition, Quaternion.identity, teapotParent);
        teapot.transform.localScale = new Vector3(1, 1, 1); // Çaydanlık boyutunu kontrol etme
        Debug.Log($"Teapot placed at: {redDotPosition}");
        teapotPlaced = true;
    }

    private void ResetScene()
    {
        // Mevcut resmi sıfırlama ve eski noktaları temizleme
        teapotPlaced = false;
        referencePointsPerImage[currentImageIndex].Clear();

        // Parent altındaki tüm çocuk nesneleri silme (Çaydanlık ve küreler dahil)
        foreach (Transform child in teapotParent)
        {
            Destroy(child.gameObject);
        }

        // Etiketlenmiş tüm küreleri sahneden kaldırma
        var spheres = GameObject.FindGameObjectsWithTag("ReferenceSphere");
        foreach (var sphere in spheres)
        {
            Destroy(sphere);
        }

        Debug.Log($"Scene reset for image {currentImageIndex + 1}");
    }
}
