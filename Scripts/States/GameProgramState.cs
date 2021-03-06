using System;
using System.Collections.Generic;
using Godot;
using EventCallback;

public enum InputActions
{
    UP_PRESSED, UP_RELEASED,
    DOWN_PRESSED, DOWN_RELEASED,
    LEFT_PRESSED, LEFT_RELEASED,
    RIGHT_PRESSED, RIGHT_RELEASED,
    LEFT_CLICK_PRESSED, LEFT_CLICK_RELEASED,
    RIGHT_CLICK_PRESSED, RIGHT_CLICK_RELEASED,
    MOUSE_MOVE
};

//The other program classes willl inherit from this one
//It must have a timer array to keep track of what happed where
public class GameProgramState : State
{
    //Used to keep track of the time intervals and input when something changed
    Dictionary<ulong, InputActions> leftInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, InputActions> rightInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, InputActions> upInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, InputActions> downInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, InputActions> lmbInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, InputActions> rmbInputTimer = new Dictionary<ulong, InputActions>();
    Dictionary<ulong, Vector2> mousePosTimer = new Dictionary<ulong, Vector2>();

    //The packed scene for the map that will be instanced later
    PackedScene mapScene = new PackedScene();
    //The node for the map that will be set to the instanced instance of the map packed scene
    Node map;
    //The tilemap to display
    TileMap displayMap;
    PackedScene playerScene = new PackedScene();
    //The node for the player that will be set to the instanced instance of the players packed scene
    Node player;

    //When the timer was started
    ulong timerStarted;
    //Run when the recording starts up
    public override void Init()
    {
        //Regestrations for the events needed for the scene ===========================================================
        InputCallbackEvent.RegisterListener(GrabInput);
        MouseInputCallbackEvent.RegisterListener(GrabMouseInput);
        //=============================================================================================================
        //Preload the scenes for the state to be used =================================================================
        //Load the map resource scene and instance it as a child of the GameProgramState node
        mapScene = ResourceLoader.Load("res://Scenes/Map.tscn") as PackedScene;
        map = mapScene.Instance();
        AddChild(map);
        TileMap RealMap = GetNode<TileMap>("Map/RealMap");
        RealMap.QueueFree();
        displayMap = GetNode<TileMap>("Map/ProgramMap");
        displayMap.Visible = true;
        playerScene = ResourceLoader.Load("res://Scenes/Player.tscn") as PackedScene;
        //=============================================================================================================
        //Set the ui state to the programming hud =====================================================================
        SendUIEvent suiei = new SendUIEvent();
        suiei.uiState = UIState.PROGRAMMING_HUD;
        suiei.FireEvent();
        //=============================================================================================================
        //Grab the time the program started recording the time for hte user input
        timerStarted = OS.GetTicksMsec();

        BuildMap();

        //Set up the camera follow for hte player
        CameraEvent cei = new CameraEvent();
        cei.target = (Node2D)player;
        cei.FireEvent();
    }
    //Run in the games loop
    public override void Update()
    {
    }
    //Run when the program is unloaded or closed
    public override void Exit()
    {
        //Set up the camera follow for hte player
        CameraEvent cei = new CameraEvent();
        //cei.target = (Node2D)player.GetParent().GetParent().GetParent();
        cei.target = GetNode<Node2D>("../../../Main");
        cei.FireEvent();
        //Call program event and pass along the movement data to the run state
        SendProgramEvent pei = new SendProgramEvent();
        pei.leftInputTimer = leftInputTimer;
        pei.rightInputTimer = rightInputTimer;
        pei.upInputTimer = upInputTimer;
        pei.downInputTimer = downInputTimer;
        pei.lmbInputTimer = lmbInputTimer;
        pei.rmbInputTimer = rmbInputTimer;
        pei.mousePosTimer = mousePosTimer;
        pei.FireEvent();
        //Hide the programing map
        displayMap.Visible = false;
        //Unregister the keyboard and mouse position input methods
        InputCallbackEvent.UnregisterListener(GrabInput);
        MouseInputCallbackEvent.UnregisterListener(GrabMouseInput);
    }
    private void GrabInput(InputCallbackEvent icei)
    {
        //The timestamp is worked out by getting the current tick and subtracting the start of the session tick amount
        ulong timeStamp = OS.GetTicksMsec() - timerStarted;
        if (icei.leftPressed) leftInputTimer.Add(timeStamp, InputActions.LEFT_PRESSED);
        if (icei.leftRelease) leftInputTimer.Add(timeStamp, InputActions.LEFT_RELEASED);
        if (icei.rightPressed) rightInputTimer.Add(timeStamp, InputActions.RIGHT_PRESSED);
        if (icei.rightRelease) rightInputTimer.Add(timeStamp, InputActions.RIGHT_RELEASED);
        if (icei.upPressed) upInputTimer.Add(timeStamp, InputActions.UP_PRESSED);
        if (icei.upRelease) upInputTimer.Add(timeStamp, InputActions.UP_RELEASED);
        if (icei.downPressed) downInputTimer.Add(timeStamp, InputActions.DOWN_PRESSED);
        if (icei.downRelease) downInputTimer.Add(timeStamp, InputActions.DOWN_RELEASED);
        if (icei.lmbClickPressed) lmbInputTimer.Add(timeStamp, InputActions.LEFT_CLICK_PRESSED);
        if (icei.lmbClickRelease) lmbInputTimer.Add(timeStamp, InputActions.LEFT_CLICK_RELEASED);
        if (icei.rmbClickPressed) rmbInputTimer.Add(timeStamp, InputActions.RIGHT_CLICK_PRESSED);
        if (icei.rmbClickRelease) rmbInputTimer.Add(timeStamp, InputActions.RIGHT_CLICK_RELEASED);
    }
    private void GrabMouseInput(MouseInputCallbackEvent micei)
    {
        //The timestamp is worked out by getting the current tick and subtracting the start of the session tick amount
        ulong timeStamp = OS.GetTicksMsec() - timerStarted;
        //Due to the timestamp being the key for the dictionary we have to make sure there are no duplicate keys or we will get and error
        //so we check if 10 seconds have passed and if they have only then do we add a new entry into the dictionary then we reset
        //the lastMousePosTimeEntry to the latest time of entry
        mousePosTimer.Add(timeStamp, micei.mousePos);
    }

    private void BuildMap()
    {
        for (int y = -21; y < 64; y++)
        {
            for (int x = 0; x < 73; x++)
            {
                //Spawn Payer and spawn gate
                if (displayMap.GetCell(x, y) == 6)
                { //Load the player scene
                    displayMap.SetCell(x, y, 2);
                    player = playerScene.Instance();
                    ((Node2D)player).Position = new Vector2(x * 32 + 16, y * 32 + 16);
                    player.Name = "Player";
                    AddChild(player);
                }
            }
        }
    }
}