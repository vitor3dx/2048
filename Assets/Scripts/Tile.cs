using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text numberText;

    public int Value { get; private set; }

    public void SetValue(int value)
    {
        Value = value;

        if (value == 0)
        {
            numberText.text = "";
        }
        else
        {
            numberText.text = value.ToString();
        }

        SetColor(value);
    }

    public void SpawnAnimation()
    {
        StartCoroutine(SpawnAnimationCoroutine());
    }

    private IEnumerator SpawnAnimationCoroutine()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.localScale = Vector3.zero;

        float duration = 0.12f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            // Suaviza a animação
            t = 1f - Mathf.Pow(1f - t, 3f);

            rectTransform.localScale = Vector3.one * t;

            yield return null;
        }

        rectTransform.localScale = Vector3.one;
    }

    private void SetColor(int value)
    {
        switch (value)
        {
            case 0:
                background.color = Color.white;
                break;

            case 2:
                background.color = new Color32(255, 210, 70, 255); // Amarelo
                break;

            case 4:
                background.color = new Color32(70, 150, 255, 255); // Azul
                break;

            case 8:
                background.color = new Color32(170, 80, 220, 255); // Roxo
                break;

            case 16:
                background.color = new Color32(255, 120, 60, 255); // Laranja
                break;

            case 32:
                background.color = new Color32(240, 60, 70, 255); // Vermelho
                break;

            case 64:
                background.color = new Color32(255, 70, 150, 255); // Rosa
                break;

            case 128:
                background.color = new Color32(70, 220, 150, 255); // Verde
                break;

            case 256:
                background.color = new Color32(40, 190, 220, 255); // Ciano
                break;

            case 512:
                background.color = new Color32(255, 190, 40, 255); // Dourado
                break;

            case 1024:
                background.color = new Color32(255, 130, 20, 255); // Laranja forte
                break;

            case 2048:
                background.color = new Color32(255, 215, 0, 255); // Ouro
                break;

            default:
                background.color = Color.black;
                break;
        }
    }
}