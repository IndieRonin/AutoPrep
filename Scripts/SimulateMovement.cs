using Godot;
using System;

public class SimulateMovement : KinematicBody2D
{
    int speed = 350;
    float turnSpeed = 0.07f;

    //The movement keys are pressed or not
    public bool up, down, left, right;
    //the velocity of the charecter
    Vector2 inputVelocity = new Vector2();
    Vector2 velocity = new Vector2();
    //The global position for hte mouse
    public Vector2 mousePos;

    public override void _PhysicsProcess(float delta)
    {
        if (up) inputVelocity.y = -1;
        else if (down) inputVelocity.y = 1;
        else inputVelocity.y = 0;
        if (left) inputVelocity.x = -1;
        else if (right) inputVelocity.x = 1;
        else inputVelocity.x = 0;

        //Look in the directio of the mouses global position
        float angleToMouse = GetAngleTo(mousePos);
        if (Mathf.Abs(angleToMouse) < turnSpeed)
        {
            Rotation += angleToMouse;
        }
        else
        {
            if (angleToMouse > 0) Rotation += turnSpeed;
            if (angleToMouse < 0) Rotation -= turnSpeed;
        }
        velocity = inputVelocity.Normalized() * speed;
        //Move and slide the rigidbaody 2d
        MoveAndSlide(velocity);
    }
}
