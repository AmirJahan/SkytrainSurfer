using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpeedController
{
    public delegate void SpeedChanged(float speed);
    public event SpeedChanged OnSpeedChanged;
    
    // The speed attributes
    public float SpeedIncrease = 1.5f;
    public int SpeedIncreaseSeconds = 5;
    
    private float CurrentSpeed = 1.0f;
    private float MaxSpeed = 100.0f;

    private static SpeedController _instance;
    public static SpeedController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpeedController();
                _instance.ChangeSpeed();
            }
            return _instance;
        }
    }
    
    public static bool IsValid () {return Instance != null;}
    
    public void Setup(float startSpeed, float multiplySpeed, int speedIncreaseInSeconds, float newMaxSpeed)
    {
        SpeedIncrease = multiplySpeed;
        SpeedIncreaseSeconds = speedIncreaseInSeconds;
        CurrentSpeed = startSpeed;
        MaxSpeed = newMaxSpeed;
    }
    
    public float GetSpeed()
    {
        return CurrentSpeed;
    }
    
    async Task ChangeSpeed()
    {
        // slight delay to ensure the OnSpeedChanged event is subscribed to
        await Task.Delay(250);
        // Set the speed right away to avoid any jerky speed changes
        _instance.OnSpeedChanged?.Invoke(_instance.CurrentSpeed);

        // Increase the speed so long as the game is playing
        while (Application.isPlaying )
        {
            await Task.Delay(SpeedIncreaseSeconds * 1000);
            CurrentSpeed *= SpeedIncrease;
            if (MaxSpeed < CurrentSpeed)
                CurrentSpeed = MaxSpeed;
            
            OnSpeedChanged?.Invoke(CurrentSpeed);

            // ensure the value isn't changing while the game is paused
            while (Time.timeScale == 0 || CurrentSpeed < MaxSpeed)
            {
                await Task.Delay(1000);
            }
            
            #if UNITY_EDITOR
                Debug.Log("Speed increased to " + CurrentSpeed);
            #endif
        }
    }
}
