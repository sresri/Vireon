using UnityEngine;
using TMPro;

public class MovieSphere : MonoBehaviour
{
    public string movieTitle;
    private Renderer rend;
    private Color originalColor;
    private bool isSelected = false;
    public TMP_Text titleText;
    public GameObject screen;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        if (titleText != null)
        {
            titleText.text = movieTitle;
        }

        if (screen != null)
        {
            screen.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        isSelected = !isSelected;
        rend.material.color = isSelected ? Color.green : originalColor;

        if (screen != null)
        {
            screen.SetActive(isSelected);
        }
    }
}