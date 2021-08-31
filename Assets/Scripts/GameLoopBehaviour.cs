using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopBehaviour : MonoBehaviour
{
    // Game Data
    private readonly Color highLightColor = new Color(0.5f, 0.5f, 0.5f);
    private readonly Color normalColor = new Color(1f, 1f, 1f);
    private int timestamp = 2;
    private int score = 0;
    [SerializeField]
    public int movementLeft = 10;
    private int combo = 0;

    // Runtime Variable
    private List<GameObject> highLightedBlocks = new List<GameObject>();
    private string lineColor;
    private float currentTime;
    private generateGame generateGame;
    private ScoreBehaviour scoreBehaviour;


    private void Start()
    {
        currentTime = Time.time;
        generateGame = GameObject.Find("GameGeneration").GetComponent<generateGame>();
        scoreBehaviour = GetComponent<ScoreBehaviour>();
        scoreBehaviour.updateText(score, movementLeft, combo);
    }

    // Update is called once per frame
    void Update()
    {
        // Process Input
        if (Input.GetMouseButtonDown(0))
        {
            // Update currentTime if mouse clicked, used for extending time
            currentTime = Time.time;
            if (Camera.main is { })
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    Debug.Log((hit.collider.transform.GetChild(0).name));
                    if (hit.collider.gameObject.CompareTag("Block"))
                    {
                        if(checkLine(hit.collider.gameObject)) 
                            HighLightBlock(hit.collider.gameObject); 
                    }
                }
            }
        }
        
        // Process Destroying and Droping
        if (highLightedBlocks.Count >= 3 && Time.time >= currentTime + timestamp)
        {
            foreach (var block in highLightedBlocks)
            {
                Destroy(block.transform.GetChild(0).gameObject); 
            }

            if (combo >= 3)
                score += highLightedBlocks.Count * 2;
            else
                score += highLightedBlocks.Count;
            if (highLightedBlocks.Count >= 1)
                combo++;
            else
                combo = 0;
            movementLeft--;
            DisableHighLight();
            scoreBehaviour.updateText(score, movementLeft, combo); 
            StartCoroutine(SpriteDropDown());
        }

        // Process Scene Changing
        if (movementLeft == 0)
        {
            SceneManager.LoadScene("EndScreen");
            PlayerPrefs.SetInt("Score", score); 
        }
    }

    private bool checkLine(GameObject block)
    {
        // Check every hex block with circle 
        if (highLightedBlocks.Count == 0)
            return true;
        double distance = 1.5;
        return highLightedBlocks.Any(hBlock => Vector3.Distance(hBlock.transform.position, block.transform.position) < distance);
    }

    private void HighLightBlock(GameObject block)
    {
        if (block == null) return;

        // Get the color of first selected
        if (highLightedBlocks.Count == 0)
        {
            lineColor = block.transform.GetChild(0).name;
        }

        // color not match
        if (block.transform.GetChild(0).name != lineColor)
        {
            DisableHighLight();
            return;
        }

        // clicked twice
        if (highLightedBlocks.Contains(block))
        {
            DisableHighLight(block);
            return;
        }     
        
        var spriteRenderer = block.GetComponent<SpriteRenderer>();
        spriteRenderer.color = highLightColor;

        highLightedBlocks.Add(block);
    }

    private void DisableHighLight()
    {
        foreach (var block in highLightedBlocks)
        {
            block.GetComponent<SpriteRenderer>().color = normalColor;
        }
        highLightedBlocks.Clear();
    }

    private void DisableHighLight(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().color = normalColor;
        highLightedBlocks.Remove(block);
    }
    
    IEnumerator SpriteDropDown()
    {
        /*
         *  -----------------
         *  | x | x | x | x |
         *  |   |   | x | x |
         *  | x |   | x |   |
         *  -----------------
         */
        yield return new WaitForEndOfFrame();
        // Check every column from bottom to up
        // Column
        for (int j = 0; j < generateGame.gamefieldArray.GetLength(1); j++)
        {
            // Row, bottom to up
            for (int i = generateGame.gamefieldArray.GetLength(0) - 1; i > 0; i--)
            {
                if (generateGame.gamefieldArray[i, j].transform.childCount == 0)
                {
                   TransformColorObject(generateGame.gamefieldArray[i,j].transform, i - 1, j); 
                } 
            }
        }

        yield return new WaitForSeconds(1);
        generateGame.GenerateMoveable(); 
    }

    private void TransformColorObject(Transform targetObject, int fromX, int fromY)
    {
        // fromY default is the block above the target. If above is null => check above whether is null.
        Debug.Log("[" + fromX + ", " + fromY + "]" );
        while (fromX > 0 && generateGame.gamefieldArray[fromX, fromY].transform.childCount == 0)
        {
            fromX -= 1;
        }
        
        // if row 0 is also null, do nothing
        if (generateGame.gamefieldArray[fromX, fromY].transform.childCount != 0)
        {
            Transform colorToMove = generateGame.gamefieldArray[fromX, fromY].transform.GetChild(0).transform;
            colorToMove.parent = null;
            colorToMove.position = targetObject.position;
            colorToMove.parent = targetObject;
        }
    }

}
