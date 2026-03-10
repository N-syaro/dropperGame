using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    [Header("移動スピード"), SerializeField]
    private float _Sp = 3;
    [Header("ジャンプスピード"), SerializeField]
    private float _JumpSp = 7;
    [Header("重力加速度"), SerializeField]
    private float _gravity = 15;
    [Header("落下速度制限"), SerializeField]
    private float _fallSpeed = 10;
    [Header("落下初速"), SerializeField]
    private float _initFallSpeed = 2;

    private Camera P_Cam;
    private Transform _Tf;
    private CharacterController C_Controller;

    
    private Vector2 input_Move;
    private float ver_Velocity;
    private float _turnVelocity;
    private bool _isGroundedPrev;
    public void OnMove(InputAction.CallbackContext context)
    {
       input_Move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || !C_Controller.isGrounded) return;
        ver_Velocity = _JumpSp;
    }

    private void Awake()
    {
        P_Cam = Camera.main;
        _Tf = transform;
        C_Controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 camForward = Vector3.Scale(P_Cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = P_Cam.transform.right;
        Vector3 moveDirection = camForward * input_Move.y + camRight * input_Move.x;

        var isGrounded = C_Controller.isGrounded;
        if (isGrounded && !_isGroundedPrev)
        {
            // 着地する瞬間に落下の初速を指定しておく
            ver_Velocity = -_initFallSpeed;
        }
        else if (!isGrounded)
        {
            // 空中にいるときは、下向きに重力加速度を与えて落下させる
            ver_Velocity -= _gravity * Time.deltaTime;

            // 落下する速さ以上にならないように補正
            if (ver_Velocity < -_fallSpeed)
                ver_Velocity = -_fallSpeed;
        }
        _isGroundedPrev = isGrounded;

        var moveVelocity = new Vector3(moveDirection.x * _Sp, ver_Velocity, moveDirection.z * _Sp);
        var moveDelta = moveVelocity * Time.deltaTime;
        C_Controller.Move(moveDelta);
        if(input_Move !=Vector2.zero)
        {
            var targetAngleY = -Mathf.Atan2(input_Move.y, input_Move.x) * Mathf.Rad2Deg+90;
            var angleY = Mathf.SmoothDampAngle(
                _Tf.eulerAngles.y, targetAngleY, ref _turnVelocity, 0.1f
                );

            _Tf.rotation = Quaternion.Euler(0,angleY,0);
        }
    }
}
