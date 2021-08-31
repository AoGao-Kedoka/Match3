using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class generateGame : MonoBehaviour
{
    public GameObject[,] gamefieldArray;
    
    /*
     * Game Data
     */
    private int gamefieldRow = 7;
    [SerializeField]
    private int gamefieldColumn = 6;

    /*
     * Runtime Variable
     */ 
    [SerializeField]
    public GameObject red;
    [SerializeField]
    public GameObject blue;
    [SerializeField]
    public GameObject green;
    [SerializeField]
    public GameObject yellow;
    [SerializeField]
    private GameObject block;
    [SerializeField]
    private Transform generatePosition;

    private void Awake()
    {
        gamefieldArray = new GameObject[gamefieldRow, gamefieldColumn];        
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateField();
        GenerateMoveable();
    }

    private void GenerateField()
    {
        for(int i = 0; i < gamefieldColumn; i++)
        {
            for(int j = 0; j < gamefieldRow; j++)
            {
                Vector3 position = new Vector3(generatePosition.position.x + i, 
                                               generatePosition.position.y - j, 
                                               generatePosition.position.z);
                if(i % 2 != 0)
                    position.y -= 0.5f;

                GameObject blockCurrent = Instantiate(block, position, generatePosition.rotation);
                gamefieldArray[j, i] = blockCurrent;
            }
        }
    }

    public void GenerateMoveable()
    {
        // Generate movable
        System.Random random = new System.Random();
        for(int i = 0; i < gamefieldArray.GetLength(0); i++)
        {
            for(int j = 0; j < gamefieldArray.GetLength(1); j++)
            {
                if (gamefieldArray[i, j].transform.childCount == 0)
                {
                    // generate a number between 0 to 4
                    int randomNumber = random.Next(4);
                    GameObject color = red;
                    switch (randomNumber)
                    {
                        case 0:
                            color = red;
                            break;
                        case 1:
                            color = blue;
                            break;
                        case 2:
                            color = green;
                            break;
                        case 3:
                            color = yellow;
                            break;
                        default:
                            Debug.LogError("Cannot generate gamefield");
                            break;
                    }
                GameObject colorObject = Instantiate(color, gamefieldArray[i,j].transform.position, gamefieldArray[i,j].transform.rotation);
                colorObject.transform.parent = gamefieldArray[i,j].transform;
                }
            }
        }
    }

}
