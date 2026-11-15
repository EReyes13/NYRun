
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvents : MonoBehaviour
{
    public int KeyPressedAmount = 0;

    public char TargetKey;

    public char[] QuickTimeKeys = { 'E', 'F', 'M', 'T', 'A' };

    // Canvas
    public GameObject QTECanvas;

    // UI Images
    public Image EImage;
    public Image FImage;
    public Image MImage;
    public Image TImage;
    public Image AImage;

    void Start()
    {
        if (QTECanvas != null)
        {
            QTECanvas.SetActive(false);
        }

        HideAllImages();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            QuickTimeEvent('E');
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            QuickTimeEvent('F');
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            QuickTimeEvent('M');
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            QuickTimeEvent('T');
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            QuickTimeEvent('A');
        }

        if (KeyPressedAmount == 10)
        {
            QuickTimeEventSuccess();
        }
    }

    public void QuickTimeEventStart()
    {
        KeyPressedAmount = 0;

        if (QTECanvas != null)
        {
            QTECanvas.SetActive(true);
        }

        TargetKey = QuickTimeKeys[Random.Range(0, QuickTimeKeys.Length)];

        HideAllImages();
        ShowTargetImage();

        Debug.Log(("Mash The KEY!!!!!") + TargetKey);
    }

    public void QuickTimeEvent(char PlayerKey)
    {
        if (PlayerKey == TargetKey)
        {
            KeyPressedAmount++;
        }
    }

    public void ShowTargetImage()
    {
        switch (TargetKey)
        {
            case 'E':
                if (EImage != null) EImage.gameObject.SetActive(true);
                break;

            case 'F':
                if (FImage != null) FImage.gameObject.SetActive(true);
                break;

            case 'M':
                if (MImage != null) MImage.gameObject.SetActive(true);
                break;

            case 'T':
                if (TImage != null) TImage.gameObject.SetActive(true);
                break;

            case 'A':
                if (AImage != null) AImage.gameObject.SetActive(true);
                break;
        }
    }

    public void HideAllImages()
    {
        if (EImage != null) EImage.gameObject.SetActive(false);
        if (FImage != null) FImage.gameObject.SetActive(false);
        if (MImage != null) MImage.gameObject.SetActive(false);
        if (TImage != null) TImage.gameObject.SetActive(false);
        if (AImage != null) AImage.gameObject.SetActive(false);
    }

    public void QuickTimeEventSuccess()
    {
        Debug.Log("You Did It!");

        KeyPressedAmount = 0;

        HideAllImages();

        if (QTECanvas != null)
        {
            QTECanvas.SetActive(false);
        }

        GameManager.Instance.EndQuickEvent();
    }
}


