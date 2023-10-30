using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemController : MonoBehaviour
{
    public static SolarSystemController current;
    public GameObject sun;
    public GameObject mercury;
    public GameObject venus;
    public GameObject earth;
    public GameObject mars;
    public GameObject jupiter;
    public GameObject saturn;
    public GameObject uranus;
    public GameObject neptune;
    public GameObject orbits;
    private Dictionary<PlanetData.Planet, GameObject> planets = new Dictionary<PlanetData.Planet, GameObject>();
    private bool rotate = true;

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
        planets.Add(PlanetData.Planet.Mercury, mercury);
        planets.Add(PlanetData.Planet.Venus, venus);
        planets.Add(PlanetData.Planet.Earth, earth);
        planets.Add(PlanetData.Planet.Mars, mars);
        planets.Add(PlanetData.Planet.Jupiter, jupiter);
        planets.Add(PlanetData.Planet.Saturn, saturn);
        planets.Add(PlanetData.Planet.Uranus, uranus);
        planets.Add(PlanetData.Planet.Neptune, neptune);
        updatePosition(PlanetManager.current.timestamp);
        updateOrbitsVisibility(UIManager.current.orbitsToggle.isOn);
        updatePlanetsSize(UIManager.current.unrealisticSizeToggle.isOn);
        PlanetManager.current.OnTimeChange += updatePosition;
        UIManager.current.OnOrbitsToggle += updateOrbitsVisibility;
        UIManager.current.OnRealisticSizeToggle += updatePlanetsSize;
        UIManager.current.OnRotationToggle += updateRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlanetData.Planet whatPlanetIsThis(GameObject planetObject)
    {
        foreach (KeyValuePair<PlanetData.Planet, GameObject> planet in planets)
        {
            if (planet.Value == planetObject)
                return planet.Key;
        }
        throw new Exception();
    }

    private void updatePosition(DateTime t)
    {
        foreach (KeyValuePair<PlanetData.Planet, GameObject> planet in planets)
        {
            Vector3 newPosition = PlanetData.GetPlanetPosition(planet.Key, t);
            newPosition = new Vector3(newPosition.x, newPosition.z, newPosition.y); // my coordinate system forces that (Y is up for me instead of Z)
            planet.Value.transform.position = newPosition * 10;
            float newRotation = 0.0f;
            if (rotate)
                newRotation = PlanetData.GetPlanetRotation(planet.Key, t);
            planet.Value.transform.rotation = Quaternion.Euler(0, newRotation, 0);
        }
    }

    private void updateOrbitsVisibility(bool visible)
    {
        orbits.SetActive(visible);
    }

    private void updatePlanetsSize(bool unrealistic)
    {
        if (unrealistic)
        {
            sun.transform.localScale = new Vector3(1, 1, 1);
            foreach (KeyValuePair<PlanetData.Planet, GameObject> planet in planets)
            {
                planet.Value.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            sun.transform.localScale = new Vector3(0.0093094948012f, 0.0093094948012f, 0.0093094948012f);
            mercury.transform.localScale = new Vector3(0.00003261411461f, 0.00003261411461f, 0.00003261411461f);
            venus.transform.localScale = new Vector3(0.00008091027736f, 0.00008091027736f, 0.00008091027736f);
            earth.transform.localScale = new Vector3(0.00008517504578f, 0.00008517504578f, 0.00008517504578f);
            mars.transform.localScale = new Vector3(0.00004531483561f, 0.00004531483561f, 0.00004531483561f);
            jupiter.transform.localScale = new Vector3(0.0009346393738f, 0.0009346393738f, 0.0009346393738f);
            saturn.transform.localScale = new Vector3(0.0007784873514f, 0.0007784873514f, 0.0007784873514f);
            uranus.transform.localScale = new Vector3(0.00033906914316f, 0.00033906914316f, 0.00033906914316f);
            neptune.transform.localScale = new Vector3(0.00032917594996f, 0.00032917594996f, 0.00032917594996f);
        }
    }

    private void updateRotation(bool rotation)
    {
        rotate = rotation;
    }
}
