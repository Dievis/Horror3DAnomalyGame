using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    public Camera playerCamera; // Camera của người chơi
    public float rayLength = 10f; // Độ dài tia ray
    public Color highlightColor = Color.yellow; // Màu chiếu sáng
    private GameObject currentHighlightedObject = null; // Đối tượng hiện tại đang được chiếu sáng
    private Color originalColor; // Lưu màu sắc ban đầu của đối tượng
    public LayerMask interactableLayer; // Layer chứa các đối tượng có thể tương tác

    void Update()
    {
        // Tạo ray từ vị trí camera, hướng ra theo hướng mà camera đang nhìn
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Kiểm tra raycast có va chạm với đối tượng nào không và đối tượng phải nằm trong layer Interactable
        if (Physics.Raycast(ray, out hit, rayLength, interactableLayer))
        {
            // Kiểm tra nếu đối tượng va chạm chưa được chiếu sáng
            if (currentHighlightedObject != hit.collider.gameObject)
            {
                // Nếu có đối tượng trước đó được chiếu sáng, hoàn tác hiệu ứng
                ResetHighlight();

                // Gán đối tượng mới bị chiếu sáng
                currentHighlightedObject = hit.collider.gameObject;

                // Lưu màu sắc ban đầu của đối tượng
                Renderer renderer = currentHighlightedObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalColor = renderer.material.color; // Lưu màu sắc ban đầu
                    renderer.material.color = highlightColor; // Thay đổi màu sắc đối tượng
                }
            }
        }
        else
        {
            // Nếu ray không trúng vào đối tượng nào, reset đối tượng được chiếu sáng
            ResetHighlight();
        }
    }

    // Reset hiệu ứng chiếu sáng
    void ResetHighlight()
    {
        if (currentHighlightedObject != null)
        {
            Renderer renderer = currentHighlightedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Phục hồi màu sắc ban đầu
                renderer.material.color = originalColor;
            }
            currentHighlightedObject = null;
        }
    }
}


//using UnityEngine;

//public class RaycastDebugger : MonoBehaviour
//{
//    public Camera playerCamera; // Camera của người chơi
//    public float rayLength = 10f; // Độ dài tia ray
//    public LineRenderer lineRenderer; // LineRenderer để vẽ tia ray

//    void Start()
//    {
//        if (lineRenderer == null)
//        {
//            lineRenderer = GetComponent<LineRenderer>();
//        }
//    }

//    void Update()
//    {
//        // Tạo ray từ vị trí camera, hướng ra theo hướng mà camera đang nhìn
//        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;

//        // Kiểm tra raycast có va chạm với đối tượng nào không
//        if (Physics.Raycast(ray, out hit, rayLength))
//        {
//            // Nếu raycast va chạm với đối tượng, vẽ một đường thẳng từ camera tới điểm va chạm
//            lineRenderer.SetPosition(0, ray.origin); // Vị trí bắt đầu của tia
//            lineRenderer.SetPosition(1, hit.point); // Vị trí kết thúc của tia (chạm vào đối tượng)

//            // Hiển thị điểm va chạm (nếu cần)
//            Debug.Log("Raycast hit at: " + hit.point);
//        }
//        else
//        {
//            // Nếu ray không va chạm với gì, vẽ tia đến cuối đoạn ray
//            lineRenderer.SetPosition(0, ray.origin);
//            lineRenderer.SetPosition(1, ray.origin + ray.direction * rayLength);
//        }
//    }
//}




