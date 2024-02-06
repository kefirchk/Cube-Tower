using UnityEngine;

public class IsCubeEnabled : MonoBehaviour
{
    public int needToUnlock;
    public Material closedMaterial;

    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < needToUnlock)
            GetComponent<MeshRenderer>().material = closedMaterial;
    }
}
