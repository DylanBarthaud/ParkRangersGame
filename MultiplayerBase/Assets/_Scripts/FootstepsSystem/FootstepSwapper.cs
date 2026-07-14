using UnityEngine;

public class FootstepSwapper : MonoBehaviour
{
    private TerrainChecker checker;
    private FootstepsPlayerScript fpcFoot;
    private string currentLayer;
    public FootstepsCollection[] terrainFootstepCollections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checker = new TerrainChecker();
        fpcFoot = GetComponent<FootstepsPlayerScript>();
    }

    public void CheckLayers()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            if(hit.transform.GetComponent<Terrain>() != null)
            {
                Terrain t = hit.transform.GetComponent<Terrain>();
                if (currentLayer != checker.GetLayerName(transform.position,t))
                {
                    currentLayer = checker.GetLayerName(transform.position,t);

                    foreach (FootstepsCollection collection in terrainFootstepCollections)
                    {
                        if (currentLayer == collection.name)
                        {
                            fpcFoot.SwapFootsteps(collection);
                        }
                    }
                }
            }
            if (hit.transform.GetComponent<SurfaceType>() != null)
            {
                FootstepsCollection collection = hit.transform.GetComponent<SurfaceType>().footstepsCollection;
                currentLayer = collection.name;
                fpcFoot.SwapFootsteps(collection);
            }
        }
    }
}
