using Godot;
using System;
using EventCallback;

public enum UIState
{
    MENU,
    WAIT_HUD,
    PROGRAMMING_HUD,
    RUN_HUD,
    WIN,
    LOSE
};

public class UIManager : Node
{
    //The references to the different ui screens
    VBoxContainer menu;
    VBoxContainer wait;
    Node2D programming;
    Node2D run;
    Node2D win;
    Node2D lose;

    //The current state thte ui is in
    private UIState currentState;

    public override void _Ready()
    {
        //Referencing all the displays in the UI node for later use
        menu = GetNode<VBoxContainer>("Menu");
        wait = GetNode<VBoxContainer>("Wait");
        programming = GetNode<Node2D>("Programming");
        run = GetNode<Node2D>("Run");
        win = GetNode<Node2D>("Win");
        lose = GetNode<Node2D>("Lose");

        SendUIEvent.RegisterListener(ChangeState);
        SendProgramEvent.RegisterListener(GetProgram);
    }

    //Hides all the ui elements that are in the ui scen
    public void HideAllUI()
    {
        //Hide all the ui elements, used when transitioning from one scene to another
        menu.Hide();
        wait.Hide();
        programming.Hide();
        run.Hide();
        win.Hide();
        lose.Hide();
    }

    private void ChangeState(SendUIEvent suiei)
    {
        //Hide all the ui screens
        HideAllUI();
        //Set the current ui state
        currentState = suiei.uiState;
        //Depending on the state show the needed ui element
        switch (currentState)
        {
            case UIState.MENU:
                menu.Show();
                break;
            case UIState.WAIT_HUD:
                wait.Show();
                break;
            case UIState.PROGRAMMING_HUD:
                programming.Show();
                break;
            case UIState.RUN_HUD:
                run.Show();
                break;
            case UIState.WIN:
                win.Show();
                break;
            case UIState.LOSE:
                lose.Show();
                break;
        }
    }
    private void GetProgram(SendProgramEvent pei)
    {
        //If a program has been recorded then the disable button wil become enables, this is to prevent errors treing to run an empty program
        GetNode<Button>("Wait/Run").Disabled = false;
    }

    public override void _ExitTree()
    {
        SendUIEvent.UnregisterListener(ChangeState);
        SendProgramEvent.UnregisterListener(GetProgram);
    }
}
