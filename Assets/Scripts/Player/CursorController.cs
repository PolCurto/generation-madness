using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [HideInInspector] public static CursorController Instance;
    public Color CursorColor { get; set; }

    [SerializeField] private Texture2D cursorSprite;

    private Vector2 _mousePosition;

    void Awake()
    {
        if (Instance == null) Instance = this;

        Cursor.SetCursor(cursorSprite, new Vector2(8, 8), CursorMode.Auto);
    }

    void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2 MousePosition => _mousePosition;
}
