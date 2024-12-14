using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;

    private const float hoursToDegrees = 360f / 12f; // Số độ mỗi giờ
    private const float minutesToDegrees = 360f / 60f; // Số độ mỗi phút
    // Lấy thời gian bắt đầu và thời gian kết thúc cho kim giờ
    public float startTime = 10f; // 10h sáng
    public float endTime = 6f; // 6h tối

    public float elapsedTime = 0f;
    public float duration = 300f; // Thời gian chạy từ 10h đến 6h là 15 phút
    private void Start()
    {
        StartCoroutine(RunClock());
    }

    private System.Collections.IEnumerator RunClock()
    {
        // Tính góc quay của kim giờ và kim phút tại thời điểm bắt đầu
        float startHourRotation = startTime * hoursToDegrees;
        float startMinuteRotation = 0f; // Bắt đầu từ 12h

        // Tính góc quay của kim giờ và kim phút tại thời điểm kết thúc
        float endHourRotation = endTime * hoursToDegrees;
        float endMinuteRotation = 360f; // Kết thúc tại 12h

        // Áp dụng góc quay cho kim giờ và kim phút tại thời điểm bắt đầu
        hourHand.localRotation = Quaternion.Euler(0f, 0f, -startHourRotation);
        minuteHand.localRotation = Quaternion.Euler(0f, 0f, -startMinuteRotation);

        while (elapsedTime < duration)
        {
            // Tính góc quay hiện tại cho kim giờ và kim phút
            float hourRotation = Mathf.Lerp(0f, (endHourRotation - startHourRotation)*2f, elapsedTime / duration);
            float minuteRotation = Mathf.Lerp(0f, (endMinuteRotation - startMinuteRotation)*8f, elapsedTime / duration);

            // Áp dụng góc quay cho kim giờ và kim phút
            hourHand.localRotation = Quaternion.Euler(0f, 0f, -startHourRotation + hourRotation);
            minuteHand.localRotation = Quaternion.Euler(0f, 0f, -startMinuteRotation - minuteRotation);


            elapsedTime += Time.deltaTime;

            yield return null; // Đợi một frame
        }
        // Đảm bảo kim giờ đạt đến vị trí cuối cùng chính xác
        hourHand.localRotation = Quaternion.Euler(0f, 0f, endHourRotation);
        minuteHand.localRotation = Quaternion.Euler(0f, 0f, endMinuteRotation);

        // In thông báo khi đồng hồ dừng lại
        Debug.Log("Clock stopped.");

        // Dừng kim giờ tại 6 giờ sáng
        yield break;

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
