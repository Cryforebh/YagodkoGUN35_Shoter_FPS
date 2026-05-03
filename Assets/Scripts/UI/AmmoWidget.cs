using UnityEngine;
using UnityEngine.UI;


public class AmmoWidget : MonoBehaviour
{
    public TMPro.TMP_Text ammoText;
    public TMPro.TMP_Text clipText;
    public Image _image;
    public Sprite _spriteAssault;
    public Sprite _spritePistol;
    public Sprite _spriteKnife;
    public Sprite _spriteNull;

    private void Start()
    {
        _image.sprite = _spriteNull;
        ammoText.text = "";
        clipText.text = "";
    }

    public void Refresh(int ammoCount, int clipCount, int slot)
    {
        ammoText.text = ammoCount.ToString();
        clipText.text = clipCount.ToString();

        if (ammoText.text.Length < 2)
            ammoText.text = "0" + ammoText.text;
        if (clipText.text.Length < 2)
            clipText.text = "0" + clipText.text;

        switch (slot)
        {
            case 1:
                _image.sprite = _spritePistol;
                break;
            case 2:
                _image.sprite = _spriteKnife;
                ammoText.text = "";
                clipText.text = "";
                break;
            default:
                _image.sprite = _spriteAssault;
                break;
        }
    }

    public void Reset()
    {
        _image.sprite = _spriteNull;
        ammoText.text = "";
        clipText.text = "";
    }
}