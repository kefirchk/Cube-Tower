using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePosition nowCube = new CubePosition(0, 1, 0);
    [SerializeField] private float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject /*cubeToCreate,*/ allCubes, vfx;
    private Rigidbody allCubesRb;

    private Coroutine showCubePlace;

    private bool firstTap = false, IsLose = false;
    public GameObject[] canvasStartPage, cubesToCreate;

    private float camMoveToYPosition, camMoveSpeed = 5f;

    public Color[] bgColors;
    private Color toCameraColor;

    public Text scoreTxt;

    private List<GameObject> posibleCubesToCreate = new List<GameObject>();
    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };

    private Transform mainCam;
    private int prevMaxHor;


    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < 5)
            posibleCubesToCreate.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 10)
            AddPossibleCubes(2);
        else if (PlayerPrefs.GetInt("score") < 15)
            AddPossibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 20)
            AddPossibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 25)
            AddPossibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 30)
            AddPossibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 35)
            AddPossibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 40)
            AddPossibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 50)
            AddPossibleCubes(9);
        else
            AddPossibleCubes(10);

        scoreTxt.text = "<size=45><color=#FB4D4C>Best:</color></size> " + PlayerPrefs.GetInt("score")
            + "\r\n<size=35>Now:</size> 0";

        toCameraColor = Camera.main.backgroundColor;

        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + nowCube.y - 1f;

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) 
            && cubeToPlace != null
            && allCubes != null
            && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
            Touch touch = Input.GetTouch(0);
            
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                // Касание произошло над объектом UI (например, кнопкой)
                Debug.Log("Касание произошло над объектом UI");
                return;
            }
            else
            {
                // Касание произошло вне объектов UI
                Debug.Log("Касание произошло вне объектов UI");
            }

#endif

            if (firstTap == false)
            {
                firstTap = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject createCube = null;
            if (posibleCubesToCreate.Count == 1)
                createCube = posibleCubesToCreate[0];
            else
                createCube = posibleCubesToCreate[UnityEngine.Random.Range(0, posibleCubesToCreate.Count)];


            GameObject newCube = Instantiate(createCube, 
                cubeToPlace.position, 
                Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.SetVectorPosition(cubeToPlace.position);
            allCubesPositions.Add(nowCube.GetVectorPosition());

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            GameObject newVfx = Instantiate(vfx, cubeToPlace.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1.5f);

            // Для обновления компонента RigidBody
            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();
        }

        if (!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            IsLose = true;
            Destroy(cubeToPlace.gameObject);
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(
            mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);

        if(Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);

    }


    private IEnumerator ShowCubePlace()
    {
        while(true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if(targetPos.y == 0)
            return false;

        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }


    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor = 0;
        foreach(Vector3 pos in allCubesPositions)
        {
            if (maxX < Mathf.Abs(Convert.ToInt32(pos.x)))
                maxX = Convert.ToInt32(pos.x);
            if (maxY < Convert.ToInt32(pos.y))
                maxY = Convert.ToInt32(pos.y);
            if (maxZ < Mathf.Abs(Convert.ToInt32(pos.z)))
                maxZ = Convert.ToInt32(pos.z);
        }

        --maxY;
        if(PlayerPrefs.GetInt("score") < maxY)
            PlayerPrefs.SetInt("score", maxY);

        scoreTxt.text = "<size=45><color=#FB4D4C>Best:</color></size> " + PlayerPrefs.GetInt("score") 
            + "\r\n<size=35>Now:</size> " + maxY;

        camMoveToYPosition = 5.9f + nowCube.y - 1f;
        maxHor = Mathf.Abs(maxX) > Mathf.Abs(maxZ) ? maxX : maxZ;
        if(maxHor % 2 == 0 && prevMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            prevMaxHor = maxHor;
        }

        // Изменение заднего фона:
        if (maxY > 23)
            toCameraColor = bgColors[7];
        else if (maxY > 20)
            toCameraColor = bgColors[6];
        else if (maxY > 17)
            toCameraColor = bgColors[5];
        else if (maxY > 14)
            toCameraColor = bgColors[4];
        else if (maxY > 11)
            toCameraColor = bgColors[3];
        else if (maxY > 8)
            toCameraColor = bgColors[2];
        else if (maxY > 5)
            toCameraColor = bgColors[1];
        else if (maxY > 2)
            toCameraColor = bgColors[0];

    }


    private void AddPossibleCubes(int till)
    {
        for (int i = 0; i < till; ++i)
            posibleCubesToCreate.Add(cubesToCreate[i]);
    }
}



struct CubePosition
{
    public int x, y, z;
    public CubePosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVectorPosition()
    {
        return new Vector3(x, y, z);
    }

    public void SetVectorPosition(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }

}
