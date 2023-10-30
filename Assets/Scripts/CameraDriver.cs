using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraDriver : FollowPlanet
{
    public static CameraDriver current;
    private Vector3 orbitingAround;

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
        orbitingAround = Vector3.zero;
        UIManager.current.OnGlobalViewClick += jumpToGlobalView;
        UIManager.current.OnTelluricViewClick += jumpToTelluricView;
    }

    // Update is called once per frame
    void Update()
    {
        if (planet != null)
            orbitingAround = planet.transform.position;
        transform.rotation = Quaternion.LookRotation(orbitingAround - transform.position);
    }

    // I couldn't get that to work with a dynamic setter/event like in PlanetManager for timestamp
    public new void bindToPlanet(GameObject newPlanet)
    {
        planet = newPlanet;
        FollowedPlanetChanged(newPlanet);
    }

    public void zoom(float amount)
    {
        UIManager.current.globalViewButton.interactable = true;
        UIManager.current.telluricViewButton.interactable = true;
        if (planet != null)
        {
            Vector3 paddingVector = new Vector3(xPadding, yPadding, zPadding);
            float distanceFromPlanet = paddingVector.magnitude;
            if (amount > 0 && distanceFromPlanet < 1.1f * planet.transform.lossyScale.magnitude) // zoom in block
                return;
            if (amount < 0 && distanceFromPlanet > 10 * planet.transform.lossyScale.magnitude) // zoom out block
                return;
            paddingVector += -amount * paddingVector * 0.01f * distanceFromPlanet;
            if (paddingVector.x == float.NaN || paddingVector.y == float.NaN || paddingVector.z == float.NaN)
                return;
            xPadding = paddingVector.x;
            yPadding = paddingVector.y;
            zPadding = paddingVector.z;
        }
        else
        {
            if (amount < 0 && transform.position.magnitude > 1000) // HACK: zoom out block (might break if camera is looking away from sun and zooming in)
                return;
            transform.position += amount * transform.forward * 0.1f * transform.position.magnitude;
            LabelController.current.smartUpdateLabelVisibility(transform.position.magnitude);
        }
    }

    public void orbit(Vector2 direction)
    {
        UIManager.current.globalViewButton.interactable = true;
        UIManager.current.telluricViewButton.interactable = true;
        if (planet != null)
        {
            Vector3 paddingVector = new Vector3(xPadding, yPadding, zPadding);
            paddingVector = Quaternion.Euler(direction.y * 0.1f, direction.x * -0.1f, 0) * paddingVector;
            xPadding = paddingVector.x;
            yPadding = paddingVector.y;
            zPadding = paddingVector.z;
        }
        else
        {
            Vector3 vectorToTarget = transform.position - orbitingAround;
            vectorToTarget = Quaternion.Euler(direction.y * 0.1f, direction.x * -0.1f, 0) * vectorToTarget;
            transform.position = orbitingAround + vectorToTarget;
        }
    }

    public void pan(Vector2 direction)
    {
        UIManager.current.globalViewButton.interactable = true;
        UIManager.current.telluricViewButton.interactable = true;
        if (planet != null)
            bindToPlanet(null);
        Vector3 displacement = (direction.x * transform.right + direction.y * transform.up) * 0.1f * transform.position.magnitude;
        transform.position += displacement;
        orbitingAround += displacement;
        LabelController.current.smartUpdateLabelVisibility(transform.position.magnitude);
    }

    public event Action<bool> OnFollowingPlanet;
    public void FollowedPlanetChanged(GameObject newPlanet)
    {
        Camera mainCamera = Camera.main;
        bool followingAPlanet = newPlanet != null;
        if (followingAPlanet)
            mainCamera.nearClipPlane = 0.0001f;
        else
            mainCamera.nearClipPlane = 0.001f;
        OnFollowingPlanet?.Invoke(followingAPlanet);
    }

    private void jumpToGlobalView()
    {
        bindToPlanet(null);
        orbitingAround = Vector3.zero;
        transform.position = new Vector3(0, 150, -450);
    }

    private void jumpToTelluricView()
    {
        bindToPlanet(null);
        orbitingAround = Vector3.zero;
        transform.position = new Vector3(0, 8, -24);
    }
}
