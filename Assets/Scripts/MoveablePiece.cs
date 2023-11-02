using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveablePiece : MonoBehaviour
{
    private GamePiece _piece;
    private IEnumerator _moveCoroutine;

    private void Awake()
    {
        _piece = GetComponent<GamePiece>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Move(int newX, int newY, float time)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }

        _moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(_moveCoroutine);
        
        // _piece.transform.localPosition = _piece.GridRef.GetWorldPosition(newX, newY);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        _piece.X = newX;
        _piece.Y = newY;
        Vector3 startPos = transform.position;
        Vector3 endPos = _piece.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            _piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        _piece.transform.position = endPos;
    }
}