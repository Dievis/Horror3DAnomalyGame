using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeInputFieldColor : MonoBehaviour
{
    public TMP_InputField inputField;  // Kéo InputField vào đây từ Inspector

    void Start()
    {
        // Tìm TMP_Text trong InputField và thay đổi màu
        TMP_Text textComponent = inputField.textComponent;
        textComponent.color = Color.red;  // Đổi màu chữ thành màu đỏ
    }
}
