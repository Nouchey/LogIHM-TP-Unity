using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LabelDriver : FollowPlanet, IPointerClickHandler
{
    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToCamera = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(vectorToCamera);
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CameraDriver.current.bindToPlanet(planet);
    }
}
