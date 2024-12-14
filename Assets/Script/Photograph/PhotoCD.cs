using UnityEngine;
[System.Serializable]
public class PhotoCD
{
    #region Variables

    [SerializeField] private int FlsUseTimes;
    [SerializeField] private int _FlsUseTimes;
    [SerializeField] private float ResetTimes;
    [Space(10)]
    [Header("readonly")]
    [SerializeField] private float CurrentTime;
    [SerializeField] private float _FirstRessetTime;
    [SerializeField] private float _LastRessetTime;
    #endregion
    public void IncreaseFlsUseTimes() => _FlsUseTimes++;
    /// <summary>
    /// Bất ngờ không ?
    /// </summary>
    public void DecreaseFlsUseTimes()
    {
        if(IsFirstUseTimes)
        {
            _FirstRessetTime = Time.time;
            _LastRessetTime = Time.time + ResetTimes;
            Debug.Log("thoi gian lan reset ke tiep:" + _LastRessetTime);
        }
        else
        {
            _LastRessetTime += ResetTimes;
            Debug.Log("thoi gian lan reset ke tiep:" + _LastRessetTime);
        }
        _FlsUseTimes--;
    }
    public bool IsFirstUseTimes => _FlsUseTimes == 5;
    public bool IsOutOfUseTime => _FlsUseTimes == 0;
    public bool IsResetingTime => Time.time < _LastRessetTime;
    /// <summary>
    /// Không ngờ tới chứ gì :D ?
    /// </summary>
    public void UseTimeUpdate()
    {
        CurrentTime = Time.time;
        Debug.Log("Da hoi du so lan chua:" + !IsResetingTime);
        if (IsResetingTime)
        {
            
            int total = (int)((Time.time - _FirstRessetTime) / ResetTimes);
            Debug.Log("Tong so lan da hoi:" + total);
            _FlsUseTimes += total;
            Debug.Log(" so lan hien tai:" + _FlsUseTimes);
            _FirstRessetTime += (ResetTimes * total);
            Debug.Log(" Thoi gian lan reset dau tien:" + _FirstRessetTime);
        }
        else
        {
            _FlsUseTimes = 5;
        }
    }
}
