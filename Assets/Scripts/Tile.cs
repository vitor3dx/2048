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
    }
}