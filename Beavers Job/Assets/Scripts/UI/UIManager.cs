using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI messageText;

    public GameObject resourceGainPrefab;
    public GameObject resourceLosePrefab;
    public RectTransform canvasTransform;

    public GameObject actionButtonsContainer;
    public GameObject beaverUIModal; 
    public TextMeshProUGUI movementPointsText;
    public Image actedImage;
    public Image beaverImage;
    public TextMeshProUGUI beaverNameText;

    public Slider musicSlider;
    public Slider sfxSlider;

    private float notificationHeightOffset = 0;

    void Start()
    {
        UpdateButtonVisibility(false);
        beaverUIModal.SetActive(false);
    }
    public void ShowBeaverUI()
    {
        beaverUIModal.SetActive(true);
    }

    public void HideBeaverUI()
    {
        beaverUIModal.SetActive(false);
    }
    public void DisplayMessage(string message)
    {
        messageText.text = "";
        messageText.text = message;
        CancelInvoke();
        Invoke("ClearMessage", 10);
    }
    public void UpdateResourceUI(int wood, int food, int stone)
    {
        woodText.text = " " + wood;
        foodText.text = " " + food;
        stoneText.text = " " + stone;
    }

    void ClearMessage()
    {
        messageText.text = "";
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        HideBeaverUI();
    }
    public void UpdateButtonVisibility(bool showButtons)
    {
        actionButtonsContainer.SetActive(showButtons);
    }

    
    public void UpdateBeaverStats(Beaver beaver)
    {
        beaverNameText.text = beaver.beaverName;
        movementPointsText.text = "" + beaver.movementPoints.ToString();
        beaverImage.sprite = beaver.beaverImage;
        actedImage.gameObject.SetActive(beaver.hasActed);
    }
    public void ShowResourceChange(Vector3 worldPosition, int amount, string resourceType)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        GameObject prefab = amount >= 0 ? resourceGainPrefab : resourceLosePrefab;
        GameObject textPopup = Instantiate(prefab, canvasTransform);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPosition, null, out Vector2 localPoint);

        localPoint.y += notificationHeightOffset;
        textPopup.GetComponent<RectTransform>().localPosition = localPoint;
        textPopup.GetComponent<TextMeshProUGUI>().text = (amount > 0 ? "+" : "") + amount.ToString() + " " + resourceType;

        notificationHeightOffset += 100; 

        StartCoroutine(FadeOutTextPopup(textPopup));
    }

    IEnumerator FadeOutTextPopup(GameObject textPopup)
    {
        TextMeshProUGUI textMesh = textPopup.GetComponent<TextMeshProUGUI>();
        Color originalColor = textMesh.color;
        for (float t = 0; t < 1; t += Time.deltaTime / 2)
        {
            if (textMesh != null)
            {
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
            }
            yield return null;
        }
        if (textMesh != null)
        {
            Destroy(textPopup);
        }

        notificationHeightOffset -= 100;
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(sfxSlider.value);
    }
}