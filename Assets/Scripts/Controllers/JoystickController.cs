using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform _rectTransform;

    public bool IsDown = false;
    public Vector2 MoveDirection = Vector2.zero;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rectTransform = transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveDirection = MoveDirection.ClampX(-1, 1).ClampY(-1, 1);
        _rectTransform.anchoredPosition = Vector2.Lerp(
            _rectTransform.anchoredPosition, MoveDirection * 25f, Time.deltaTime * 5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
        _rectTransform.anchoredPosition = _rectTransform.anchoredPosition.ClampX(-25, 25).ClampY(-25, 25);

        MoveDirection = _rectTransform.anchoredPosition / new Vector2(25, 25);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDown = false;
        MoveDirection = Vector2.zero;
    }
}
