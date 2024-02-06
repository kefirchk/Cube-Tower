using Unity.VisualScripting;
using UnityEngine;

public class ExploideCube : MonoBehaviour
{
    [SerializeField] private float force = 100f;
    [SerializeField] private float radius = 5f;
    private bool isCollision = false;
    public GameObject restartButton, explosion;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && !isCollision)
        {
            for(int i = collision.transform.childCount - 1; i >= 0; --i) 
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, Vector3.up, radius);
                child.SetParent(null);
            }
            restartButton.SetActive(true);
            Camera.main.transform.localPosition -= new Vector3(0, 0, 3f);
            Camera.main.gameObject.AddComponent<CameraShake>();

            GameObject newVfx = Instantiate(explosion, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(newVfx, 2.5f);

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            Destroy(collision.gameObject);
            isCollision = true;
        }
    }
}
