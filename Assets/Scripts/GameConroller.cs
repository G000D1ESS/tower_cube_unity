using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameConroller : MonoBehaviour
{
    public float cubeChangeSpeed = 0.5f;
    public float cameraMoveSpeed = 2f;
    private float cameraMoveY;
    private Coroutine showCubePlace;

    private int maxHor;
    
    
    public Transform cubeToPlace;
    private CubePosition nowCube = new CubePosition(0, 1, 0);
    public GameObject cubeToCreate, allCubes;
    private Rigidbody allCubesRB;

    private bool isLose, firstCube;
    public GameObject RestartButton;
    public GameObject[] canvasStartPage;

    private Transform mainCam;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1)
    };

    void Start()
    {
        mainCam = Camera.main.transform;
        cameraMoveY = 6f + nowCube.y - 1f;

        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    // Update is called once per frame
    void Update()
    {

        if (allCubesRB.velocity.magnitude > 0.1f && isLose == false)
        {
            Destroy(cubeToPlace.gameObject);
            StopCoroutine(showCubePlace);
            isLose = true;
            RestartButton.SetActive(true);
        }

        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && isLose == false && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began) return;
#endif

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                {
                    Destroy(obj);
                }
            }

            GameObject newCube =  Instantiate(
                cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);

            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(cubeToPlace.position);

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();
        }

        Vector3 movePos = mainCam.localPosition;
        movePos.y = cameraMoveY;

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, movePos, cameraMoveSpeed * Time.deltaTime);
    }
    
    private void MoveCameraChangeBg()
    {
        int maxX, maxY, maxZ, newHorizontal;
        maxX = maxY = maxZ = 0;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(pos.x) > Mathf.Abs(maxX))
            {
                maxX = Convert.ToInt32(pos.x);
                maxX = Mathf.Abs(maxX);
            }
            if (Mathf.Abs(pos.y) > Mathf.Abs(maxY))
            {
                maxY = Convert.ToInt32(pos.y);
                maxY = Mathf.Abs(maxY);
            }
            if (Mathf.Abs(pos.z) > Mathf.Abs(maxZ))
            {
                maxZ = Convert.ToInt32(pos.z);
                maxZ = Mathf.Abs(maxZ);
            }
        }


        newHorizontal = maxX > maxZ ? maxX : maxZ;
        if (maxHor < newHorizontal)
        {
            maxHor = newHorizontal;
            if (maxHor % 3 == 0)
            {
                mainCam.localPosition += new Vector3(0, 0, -2f);
            }
        }

        cameraMoveY = 6f + nowCube.y - 1f;

    }


    IEnumerator ShowCubePlace()
    {
        while (!isLose)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangeSpeed);
        }
    }

    private void SpawnPositions()
    {
        if (isLose == true)
        {
            return;
        }

        List<Vector3> freePositions = new List<Vector3>();

        if (IsPositionFree(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)))
        {
            freePositions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        }

        if (IsPositionFree(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)))
        {
            freePositions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }
        
        if (IsPositionFree(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)))
        {
            freePositions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        }
        
        if (IsPositionFree(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)))
        {
            freePositions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        }
        
        if (IsPositionFree(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)))
        {
            freePositions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        }

        if (IsPositionFree(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)))
        {
            freePositions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }

        freePositions.Remove(cubeToPlace.position);
        cubeToPlace.position = freePositions[UnityEngine.Random.Range(0, freePositions.Count)];
        
    }

    public bool IsPositionFree(Vector3 targetPosition)
    {

        if(targetPosition.y <= 0)
        {
            return false;
        }

        foreach(Vector3 position in allCubesPositions)
        {
            if (targetPosition.Equals(position))
            {
                return false;
            }
        }
        return true;
    }
}


class CubePosition
{
    private Vector3 vector;
    public int x, y, z;



    public CubePosition(int x, int y, int z)
    {
        this.vector = new Vector3(x, y, z);
        this.x = x;
        this.y = y;
        this.z = z;
    }

    private void UpdateCords()
    {
        this.x = Convert.ToInt32(this.vector.x);
        this.y = Convert.ToInt32(this.vector.y);
        this.z = Convert.ToInt32(this.vector.z);
    }

    public Vector3 GetVector()
    {
        return vector;
    }

    public void setVector(Vector3 pos)
    {
        vector = pos;
        UpdateCords();
    }
}