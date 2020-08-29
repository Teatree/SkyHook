using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State {

    public Vector3 pos;
    public Vector3 dir;

    public MoveState(GameSystem gameSystem) : base(gameSystem)
    {

    }

    Vector3 startDragSaved;
    Vector3 startDragPlayerPosSaved;
    Transform transform;

    public override IEnumerator Start()
    {
        transform = GameSystem.Player.transform;
        SavePosition();
        //terrible
        //LevelSystem.Instance.ResetMoveCounter();

        //AudioSystem.Instance.PlaySong();

        yield return null;
    }

    public override void OnUpdate()
    {
        // Get Direction on Joystick
        dir = JoystickController.Instance.GetDirection();
        dir.z = -dir.y;

        // Translate direction from Joystick to Player's world direction
        Player.Instance.UpdateDirecionIndicator(dir);

        // Constantly move Player at a consistent speed in the direction
        Player.Instance.transform.Translate(Vector3.forward * 10 * Player.CurrentSpeedIncrement * Time.deltaTime, Space.Self);

        // Update Camera's position to follow
        Camera.main.transform.position = new Vector3(Player.Instance.transform.position.x, 120, Player.Instance.transform.position.z - 90);


        //if (Input.GetMouseButtonDown(0))
        //{
        //    SavePosition();
        //}
        //else if (Input.GetMouseButton(0))
        //{
        //    // input and stuff I guess
        //    Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165, 1, Camera.main.nearClipPlane));
        //    Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165, 1, Camera.main.nearClipPlane));

        //    Plane plane = new Plane(Vector3.up, 0);

        //    float distance;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (plane.Raycast(ray, out distance))
        //    {
        //        Vector3 t = ray.GetPoint(distance) - startDragSaved;

        //        transform.position = startDragPlayerPosSaved + t;
        //        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(transform.position.z, 5, 7)), 0.98f); // -17, 34

        //        //transform.LookAt(Hook.Instance.GetTargetPosSaved());
        //        //transform.eulerAngles = new Vector3(-130, transform.eulerAngles.y, transform.eulerAngles.z);
        //    }  
        //}

        //Hook.Instance.SetHookPos(transform.position);
        //Hook.Instance.PullHookPos(transform.position, 10);
        //LevelSystem.Instance.MoveLevel(20);

        //if(Vector3.Distance(transform.position, LevelSystem.Instance.SpinnerGo.transform.position) < 50)
        //{
        //    GameSystem.SetState(new FinishedState(GameSystem));
        //}
    }

    public void SavePosition()
    {
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            startDragSaved = ray.GetPoint(distance);
            startDragPlayerPosSaved = transform.position;
        }
    }
}
