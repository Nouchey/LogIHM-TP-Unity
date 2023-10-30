using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LabelController : MonoBehaviour
{
    public static LabelController current;
    public GameObject[] globalLabels, telluricLabels;

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
        UIManager.current.OnGlobalViewClick += jumpToGlobalView;
        UIManager.current.OnTelluricViewClick += jumpToTelluricView;
        CameraDriver.current.OnFollowingPlanet += handleChangePlanetView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void smartUpdateLabelVisibility(float distanceFromSun)
    {
        if (distanceFromSun < 90)
        {
            updateGlobalLabelsVisibility(false);
            updateTelluricLabelsVisibility(true);
        }
        else if (distanceFromSun > 1500)
        {
            updateGlobalLabelsVisibility(false);
            updateTelluricLabelsVisibility(false);
        }
        else
        {
            updateGlobalLabelsVisibility(true);
            updateTelluricLabelsVisibility(false);
        }
    }

    public void updateGlobalLabelsVisibility(bool visible)
    {
        for (int i = 0; i < globalLabels.Length; i++)
        {
            globalLabels[i].SetActive(visible);
        }
    }

    public void updateTelluricLabelsVisibility(bool visible)
    {
        for (int i = 0; i < telluricLabels.Length; i++)
        {
            telluricLabels[i].SetActive(visible);
        }
    }

    private void jumpToGlobalView()
    {
        updateGlobalLabelsVisibility(true);
        updateTelluricLabelsVisibility(false);
    }

    private void jumpToTelluricView()
    {
        updateGlobalLabelsVisibility(false);
        updateTelluricLabelsVisibility(true);
    }

    private void handleChangePlanetView(bool followingAPlanet)
    {
        if (followingAPlanet)
        {
            updateGlobalLabelsVisibility(false);
            updateTelluricLabelsVisibility(false);
        }
    }
}
