﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    //scripts
    private World scriptW = null;

    //player
    public float movex = 0;
    public float movey = 0;
    private Rigidbody playerR = null;
    public int meeleTime = 15;
    bool meele = false;
    private bool jumped = false;
    private GameObject rayPosition = null;
    private bool move = false;
    public bool inventoryKeyHit = false;

    //camera's
    private Camera camFirst;
    private Camera camThird;
    private bool firstPersonCamera = true;

    //UI components
    //private Text player_Health = null;
    //private Text player_Food = null;
    //private Text player_Water = null;
    private Text playerSlot0 = null;
    private Text playerSlot1 = null;
    private Text playerSlot2 = null;
    private Text playerSlot3 = null;

    //bool aim = false;

    //enable and updating
    private byte update_UI = 0; //if UI needs to update
    private bool enableMovement = true; //enables the player to move and interact
    private bool enableInventory = false;

    //inventory
    private GameObject reachableObject = null;
    public sbyte[] inventorySlot = new sbyte[4];
    public byte[] inventorySize = new byte[4];
    public string[] ITEMNAMES = { "Wood", "Rock" };

    // Use this for initialization
    void Start()
    {
        //loads previous save (if any) and sets everything up
        LoadGame();
    }
    //updates once per frame
    void Update()
    {
        //player movement
        Movement(enableMovement);

        //Inventory
        Inventory(enableInventory);

    }
    void FixedUpdate()
    {
        if (move)
        {
            //interaction
            if (Physics.Raycast(rayPosition.transform.position, rayPosition.transform.forward, out RaycastHit hit, (float)1.5))
            {
                //if you interact with an object
                if (reachableObject == null)
                {
                    reachableObject = hit.collider.gameObject;
                    Debug.Log(reachableObject.name);
                }
                //if you interact with a new object
                else if (reachableObject != hit.collider.gameObject)
                {
                    reachableObject = hit.collider.gameObject;
                    Debug.Log(reachableObject.name);
                }
            }
            //if not interacting with anything then set reachable_object to null
            else
            {
                reachableObject = null;
            }
        }
        //meele
        //melee timer
        if (meele == true)
        {
            meeleTime -= 1;
        }

        return;
    }

    //checks for update on inventory slot
    private void InventoryUpdate(byte slot)
    {
        if (slot == 0)
        {
            playerSlot0.text = "" + ItemToString((byte)inventorySlot[0]) + "x" + inventorySize[0];
        }
        else if (slot == 1)
        {
            playerSlot1.text = "" + ItemToString((byte)inventorySlot[1]) + "x" + inventorySize[1];
        }
        else if (slot == 2)
        {
            playerSlot2.text = "" + ItemToString((byte)inventorySlot[2]) + "x" + inventorySize[2];
        }
        else if (slot == 3)
        {
            playerSlot3.text = "" + ItemToString((byte)inventorySlot[3]) + "x" + inventorySize[3];
        }
        return;
    }

    //adds item to inventory
    private void InventoryAdd(byte item)
    {

        bool slotFound = false;

        for (byte i = 0; i < inventorySlot.Length && !slotFound; i++)
        {
            //finds first open slot
            if (inventorySlot[i] < 0 && !slotFound)
            {
                inventorySlot[i] = (sbyte)item;
                inventorySize[i]++;
                slotFound = true;
                InventoryUpdate(i);
            }

            //add to existing slot
            else if (inventorySlot[i] == item)
            {
                inventorySize[i]++;
                slotFound = true;
                InventoryUpdate(i);
            }
        }
        return;
    }

    //returns string of item
    private string ItemToString(byte item)
    {
        return ITEMNAMES[item];
    }

    //player movement and interaction
    public bool Movement(bool status)
    {
        //if movement is enabled 
        if (status)
        {
            //resets move detected
            move = false;

            //gets's mouse input
            movex = Input.GetAxis("Mouse X");
            movey = -Input.GetAxis("Mouse Y");

            //rotates player
            if (movex != 0)
            {
                //rotates player (left/right)
                transform.Rotate(0, movex, 0);
                move = true;
            }

            //moves camera's y-axis and ray_position
            if (movey != 0)
            {
                //rotates camera's (up/down)
                camFirst.transform.Rotate(movey, 0, 0);

                camThird.transform.Rotate(movey, 0, 0);

                rayPosition.transform.Rotate(movey, 0, 0);

                if (move == false)
                {
                    move = true;
                }
            }
            //melee detection
            if (Input.GetMouseButtonDown(0))
            {

            }
            //move forward
            if (Input.GetKey(KeyCode.W))
            {
                //running
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(0, 0, 4 * Time.deltaTime);
                }
                //walk
                else
                {
                    transform.Translate(0, 0, 2 * Time.deltaTime);
                }
                if (move == false)
                {
                    move = true;
                }
            }
            //move back
            if (Input.GetKey(KeyCode.S))
            {
                //running
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(0, 0, -4 * Time.deltaTime);
                }
                //walk
                else
                {
                    transform.Translate(0, 0, -2 * Time.deltaTime);
                }
                if (move == false)
                {
                    move = true;
                }
            }
            //move left
            if (Input.GetKey(KeyCode.A))
            {
                //running
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(-4 * Time.deltaTime, 0, 0);
                }
                //walk
                else
                {
                    transform.Translate(-2 * Time.deltaTime, 0, 0);
                }
                if (move == false)
                {
                    move = true;
                }
            }
            //move right
            if (Input.GetKey(KeyCode.D))
            {
                //running
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(4 * Time.deltaTime, 0, 0);
                }
                //walk
                else
                {
                    transform.Translate(2 * Time.deltaTime, 0, 0);
                }
                if (move == false)
                {
                    move = true;
                }
            }
            //interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                //if the object is a resource then 
                if (reachableObject != null && reachableObject.tag.CompareTo("resource") == 0)
                {
                    InventoryAdd(reachableObject.GetComponentInParent<Resource>().HitResource());
                }
            }
            //jumping
            if (Input.GetKey(KeyCode.Space))
            {
                if (jumped == false)
                {
                    playerR.AddForce(Vector3.up * 250);
                    jumped = true;
                }
            }
            //aim
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Aim not implemented yet :(");
            }
            //first person
            if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == false)
            {
                playerR.GetComponent<MeshRenderer>().enabled = false;
                camThird.gameObject.SetActive(false);
                camFirst.gameObject.SetActive(true);
                firstPersonCamera = true;
            }
            //third person 
            else if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == true)
            {
                playerR.GetComponent<MeshRenderer>().enabled = true;
                camFirst.gameObject.SetActive(false);
                camThird.gameObject.SetActive(true);
                firstPersonCamera = false;
            }
            //speed up
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (Time.timeScale == 10)
                {
                    Time.timeScale = 1;
                    Debug.Log("Speed normal");
                }
                else
                {
                    Time.timeScale = 10;
                    Debug.Log("Speed up");
                }

            }
            //open inventory
            if (Input.GetKeyDown(KeyCode.I))
            {
                enableInventory = true;
                enableMovement = false;
            }
            //quits game
            if (Input.GetKeyDown(KeyCode.L))
            {
                SaveGame();
                Application.Quit();
            }

        }
        return false;
    }

    //Saves the game (WIP)
    private void SaveGame()
    {
        //path to the File
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //Checks if file exists and pulls it, othwerwise creates a new file 
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }
        //File writer
        StreamWriter write = new StreamWriter(file);

        //adds the player position to be written (Queue's it)
        write.WriteLine("" + gameObject.transform.position.x + "," + gameObject.transform.position.y + "," + gameObject.transform.position.z);

        //pushes the output to the File
        write.Flush();

        //closes File
        file.Close();

        return;
    }

    //Loads objects and save file (if present)
    private void LoadGame()
    {

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptW = GameObject.Find("world").GetComponent<World>();

        //player
        playerR = GetComponent<Rigidbody>();
        rayPosition = GameObject.Find("rayPosition");

        //camera's
        camFirst = GameObject.Find("cameraFP").GetComponent<Camera>();
        camThird = GameObject.Find("cameraTP").GetComponent<Camera>();

        //inventory
        playerSlot0 = GameObject.Find("playerSlot0").GetComponent<Text>();
        playerSlot1 = GameObject.Find("playerSlot1").GetComponent<Text>();
        playerSlot2 = GameObject.Find("playerSlot2").GetComponent<Text>();
        playerSlot3 = GameObject.Find("playerSlot3").GetComponent<Text>();

        //Loads previous save (if it exists
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //if file exists then load
        if (File.Exists(destination))
        {
            //get file
            file = File.OpenRead(destination);
            StreamReader read = new StreamReader(file);

            //gets line from file
            string line = read.ReadLine();

            //player position
            string[] playerData = line.Split(',');
            Vector3 playerPOS = new Vector3(float.Parse(playerData[0]), float.Parse(playerData[1]), float.Parse(playerData[2]));
            gameObject.transform.SetPositionAndRotation(playerPOS, Quaternion.Euler(0, 0, 0));

            file.Close();
        }

        //settings before game starts (change later when saving is implemented)
        camThird.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        Application.targetFrameRate = 60; //should create or find a method that keeps everything running at certain rate (Time.deltatime?) to not limit FPS

        return;
    }

    //the invenetory method (interacting, opening and closing)
    private void Inventory(bool status)
    {
        //if inventory is enabled
        if (status)
        {
            //detects when inventory key is released (prevents opening and closing at same time)
            if (Input.GetKeyUp(KeyCode.I))
            {
                inventoryKeyHit = true;
            }
            //closes inventory and re-enables movement
            if (Input.GetKeyDown(KeyCode.Escape) && inventoryKeyHit || Input.GetKeyDown(KeyCode.I) && inventoryKeyHit)
            {
                enableMovement = true;
                enableInventory = false;
                inventoryKeyHit = false;
            }
        }
        return;
    }

    //collision detection
    private void OnCollisionEnter(Collision collision)
    {
        //detects collision from the bottom of player
        var normal = collision.GetContact(0).normal;

        //if bottom of collider hits object
        if (normal.y > 0)
        {
            //resets jump
            if (jumped == true)
            {
                jumped = false;
            }
        }
        return;
    }
}