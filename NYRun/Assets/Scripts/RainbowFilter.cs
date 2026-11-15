
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RainbowFilter : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Canvas canvasRoot;
    public Image rainbowOverlay;

    [Header("Sprite Trail Settings")]
    public global::SpriteTrailRenderer.SpriteTrailRenderer spriteTrail;

    [Header("Rainbow Settings")]
    public float effectDuration = 10f;
    public float rainbowSpeed = 1.5f;

    [Range(0f, 1f)]
    public float minimumAlpha = 0.12f;

    [Range(0f, 1f)]
    public float maximumAlpha = 0.35f;

    public float pulseSpeed = 3f;

    [Header("Screen Shake")]
    public Camera gameCamera;
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.12f;

    [Header("Testing")]
    public bool testWithKey = true;
    public KeyCode testKey = KeyCode.P;

    private Coroutine activeEffect;
    private Vector3 originalCameraPosition;

    // Save the character's original colors.
    private SpriteRenderer[] characterRenderers;
    private Color[] originalCharacterColors;

    private void Start()
    {
        // Find all character SpriteRenderers.
        if (spriteTrail != null)
        {
            characterRenderers =
                spriteTrail.GetComponentsInChildren<SpriteRenderer>(true);

            originalCharacterColors =
                new Color[characterRenderers.Length];

            for (int i = 0; i < characterRenderers.Length; i++)
            {
                originalCharacterColors[i] =
                    characterRenderers[i].color;
            }

            // Keep the character's rainbow trail disabled initially.
            spriteTrail.enabled = false;
        }

        // Keep the rainbow overlay hidden initially.
        if (rainbowOverlay != null)
        {
            rainbowOverlay.color = Color.clear;
            rainbowOverlay.raycastTarget = false;
        }
    }

    private void Update()
    {
        if (testWithKey && Input.GetKeyDown(testKey))
        {
            ActivateRainbowPowerUp();
        }
    }

    public void ActivateRainbowPowerUp()
    {
        if (rainbowOverlay == null || canvasRoot == null)
        {
            Debug.LogError(
                "Please assign the Canvas and Rainbow Overlay!"
            );
            return;
        }

        // Restart the effect if P is pressed again.
        if (activeEffect != null)
        {
            StopCoroutine(activeEffect);

            if (gameCamera != null)
            {
                gameCamera.transform.localPosition =
                    originalCameraPosition;
            }
        }

        // Turn on the Canvas.
        canvasRoot.gameObject.SetActive(true);

        // Turn off all Canvas objects except the rainbow.
        DisableCanvasChildrenExceptRainbow(canvasRoot.transform);

        rainbowOverlay.gameObject.SetActive(true);

        // Turn on the character's rainbow trail.
        if (spriteTrail != null)
        {
            spriteTrail.enabled = true;
        }

        activeEffect = StartCoroutine(RainbowEffect());
    }

    private void DisableCanvasChildrenExceptRainbow(
        Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child == rainbowOverlay.transform ||
                rainbowOverlay.transform.IsChildOf(child))
            {
                DisableCanvasChildrenExceptRainbow(child);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator RainbowEffect()
    {
        if (gameCamera != null)
        {
            originalCameraPosition =
                gameCamera.transform.localPosition;
        }

        float elapsedTime = 0f;
        float shakeTimer = 0f;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;

            // Rainbow color cycling.
            float hue = Mathf.Repeat(
                Time.time * rainbowSpeed, 1f
            );

            Color rainbowColor =
                Color.HSVToRGB(hue, 1f, 1f);

            // Pulsating transparency.
            float pulse = (Mathf.Sin(
                Time.time * pulseSpeed * Mathf.PI * 2f
            ) + 1f) / 2f;

            float alpha = Mathf.Lerp(
                minimumAlpha,
                maximumAlpha,
                pulse
            );

            // Fade out during the final half-second.
            float fadeOut = Mathf.Clamp01(
                (effectDuration - elapsedTime) / 0.5f
            );

            rainbowColor.a = alpha * fadeOut;

            rainbowOverlay.color = rainbowColor;

            // Camera shake.
            if (gameCamera != null &&
                shakeTimer < shakeDuration)
            {
                shakeTimer += Time.deltaTime;

                Vector2 shakeOffset =
                    Random.insideUnitCircle * shakeStrength;

                gameCamera.transform.localPosition =
                    originalCameraPosition +
                    new Vector3(
                        shakeOffset.x,
                        shakeOffset.y,
                        0f
                    );
            }
            else if (gameCamera != null)
            {
                gameCamera.transform.localPosition =
                    originalCameraPosition;
            }

            yield return null;
        }

        // Restore the original screen appearance.
        rainbowOverlay.color = Color.clear;

        // Restore the camera position.
        if (gameCamera != null)
        {
            gameCamera.transform.localPosition =
                originalCameraPosition;
        }

        // Turn off the character's rainbow trail.
        if (spriteTrail != null)
        {
            spriteTrail.enabled = false;
        }

        // Restore every character part to its original color.
        if (characterRenderers != null &&
            originalCharacterColors != null)
        {
            for (int i = 0; i < characterRenderers.Length; i++)
            {
                if (characterRenderers[i] != null)
                {
                    characterRenderers[i].color =
                        originalCharacterColors[i];
                }
            }
        }

        activeEffect = null;
    }
}

