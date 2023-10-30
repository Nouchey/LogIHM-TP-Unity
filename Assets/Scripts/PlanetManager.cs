using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager current;
    [SerializeField]
    private UDateTime ts;
    public UDateTime timestamp
    {
        get => ts;
        set
        {
            ts = value;
            TimeChanged(value.dateTime);
        }
    }

    [Tooltip("Time speed in hours per second")]
    public int speed = 480;
    public bool play = true;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(obj: this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; // important for speed consistency
        timestamp = DateTime.Now;
        UIManager.current.OnPlaySpeedChanged += changePlaySpeed;
        UIManager.current.OnPlayPauseToggle += playPause;
        UIManager.current.OnDateChanged += changeDate;
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
            timestamp = timestamp.dateTime.AddMinutes(speed);
    }

    public event Action<DateTime> OnTimeChange;
    public void TimeChanged(DateTime newTimestamp)
    {
        OnTimeChange?.Invoke(newTimestamp);
    }

    private void changePlaySpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    private void playPause(bool playing)
    {
        play = playing;
    }

    private void changeDate(DateTime newTimestamp)
    {
        timestamp = newTimestamp;
    }
}
