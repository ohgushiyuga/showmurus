using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
public Sprite highlightedSprite; // カーソルが重なったときの画像
private Image buttonImage;
private Sprite normalSprite; // ボタンの通常の画像

void Start()
{
buttonImage = GetComponent<Image>();
normalSprite = buttonImage.sprite; // ボタンの通常の画像を記憶する
}

// カーソルがボタンに重なったときの処理
public void OnPointerEnter()
{
buttonImage.sprite = highlightedSprite; // カーソルが重なったときの画像に変更する
}

// カーソルがボタンから離れたときの処理
public void OnPointerExit()
{
buttonImage.sprite = normalSprite; // 通常の画像に戻す
}
}