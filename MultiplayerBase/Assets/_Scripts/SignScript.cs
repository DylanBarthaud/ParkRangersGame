using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SignScript : MonoBehaviour
{

    // Drag the text or image you want to appear here, in the Editor.
    public Text interactionText;
    [TextArea]
    public string message;

    // How far you can interact with the object from
    public float interactionDistance = 1.5f;

    void Start()
    {
        interactionText.text = message;

        // Turns the text off if it isn't already.
        interactionText.gameObject.SetActive(false);
    }

    void Update()
    {

        // Creates a ray going from the camera.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Stores information about what the Raycast hit.
        RaycastHit hit;

        // Raycast for detecting the object.
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {

            // Optional visualization of the ray.
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

            // Checks for tag or other condition for recognising the object.
            if (hit.collider.tag == "Sign" && hit.collider.gameObject == gameObject)
            {

                // Turns on the interaction prompt.
                interactionText.gameObject.SetActive(true);

            }
        }
        else
        {
            // Turns the prompt back off when you're not looking at the object.
            interactionText.gameObject.SetActive(false);
        }
    }
}