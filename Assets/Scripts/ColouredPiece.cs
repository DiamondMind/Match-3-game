using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColouredPiece : MonoBehaviour
{
    public enum ColourType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ColourSprite
    {
        public ColourType colour;
        public Sprite sprite;
    }

    public ColourSprite[] colourSprites;

    private ColourType _colour;

    public ColourType Colour
    {
        get { return _colour; }
        set { SetColour(value); }
    }

    private SpriteRenderer _sprite;

    private Dictionary<ColourType, Sprite> _colourSpriteDict;

    private void Awake()
    {
        _sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
        _colourSpriteDict = new Dictionary<ColourType, Sprite>();
        for (int i = 0; i < colourSprites.Length; i++)
        {
            if (!_colourSpriteDict.ContainsKey(colourSprites[i].colour))
            {
                _colourSpriteDict.Add(colourSprites[i].colour, colourSprites[i].sprite);
            }
        }
    }

    public int NumColours
    {
        get { return colourSprites.Length; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetColour(ColourType newColour)
    {
        _colour = newColour;
        if (_colourSpriteDict.ContainsKey(newColour))
        {
            _sprite.sprite = _colourSpriteDict[newColour];
        }
    }
}