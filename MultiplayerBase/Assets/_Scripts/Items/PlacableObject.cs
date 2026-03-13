using Unity.Netcode;
using UnityEngine;

public class PlacableObject : Item
{
    [Header("PlacableObj Settings")]
    [SerializeField] GameObject objectPrefab;
    [SerializeField] Material baseMaterial;
    [SerializeField] Material previewMaterial;

    [SerializeField] float maxPlaceDistance = 5f;
    [SerializeField] LayerMask layerMask;
    private bool placing = false;
    private GameObject user;

    private GameObject previewObj; 

    public override void UseItem(GameObject user)
    {
        placing = true;
        this.user = user;
        uses--; 
    }

    bool itemIsSpawned = false;
    private void Update()
    {
        if (!placing) return;

        if (!itemIsSpawned)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxPlaceDistance, layerMask))
            {
                previewObj = Instantiate(objectPrefab, hit.point, Quaternion.identity);
                itemIsSpawned = true;
                previewObj.GetComponent<GFXHandler>().ChangeGFXMaterialServerRpc("CameraGFX", 1);
            }
        }

        if(itemIsSpawned && Input.GetMouseButton(0) && previewObj != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxPlaceDistance, layerMask))
            {
                previewObj.transform.position = hit.point;
            }
        }

        if (itemIsSpawned && Input.GetMouseButtonUp(0) && previewObj != null)
        {
            placing = false; 

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxPlaceDistance, layerMask))
            {
                SpawnObjectOnServerRpc(hit.point); 
                previewObj.GetComponent<GFXHandler>().ChangeGFXMaterialServerRpc("CameraGFX", 0); 
            }
        }

        if (itemIsSpawned && Input.GetKey(KeyCode.E) && previewObj != null) previewObj.transform.Rotate(new Vector3(0, 5, 0)); 
        if (itemIsSpawned && Input.GetKey(KeyCode.Q) && previewObj != null) previewObj.transform.Rotate(new Vector3(0, -5, 0));

    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectOnServerRpc(Vector3 position)
    {
        previewObj.GetComponent<NetworkObject>().Spawn();
        SpawnObjectOnClientRpc(position); 
    }

    [ClientRpc]
    private void SpawnObjectOnClientRpc(Vector3 position) => previewObj.transform.position = position;
}
