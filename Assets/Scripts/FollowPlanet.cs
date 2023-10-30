using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlanet : MonoBehaviour
{
    public GameObject planet;
    public float xPadding, yPadding, zPadding;

    // Start is called before the first frame update
    void Start()
    {
        LateUpdate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // LateUpdate is called once per frame, but later than all other (regular) Update functions
    void LateUpdate()
    {
        if (planet == null)
            return;
        Vector3 planetPosition = planet.transform.position;
        float planetScale = (planet.transform.localScale.x + planet.transform.localScale.y + planet.transform.localScale.z) / 3;
        transform.position = new Vector3(planetPosition.x + (xPadding * planetScale), planetPosition.y + (yPadding * planetScale), planetPosition.z + (zPadding * planetScale));
    }

    public void bindToPlanet(GameObject newPlanet)
    {
        planet = newPlanet;
    }
}
