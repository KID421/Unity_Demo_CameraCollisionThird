using UnityEngine;

public class ThirdPersonControl : MonoBehaviour
{
    [Header("速度"), Range(0, 100)]
    public float speed = 1;
    [Header("旋轉"), Range(0, 100)]
    public float turn = 1;

    private Rigidbody rig;
    private Animator ani;

    /// <summary>
    /// 攝影機
    /// </summary>
    private Transform cam;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        cam = GameObject.Find("攝影機根物件").transform;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");      // 左右
        float v = Input.GetAxis("Vertical");        // 前後

        Vector3 pos;                                // 要位移的向量

        pos = cam.forward * v + cam.right * h;      // 向量為攝影機的前方與右方
        pos.y = 0;                                  // Y 軸高度不便

        rig.MovePosition(transform.position + pos * speed * Time.fixedDeltaTime);   // 移動

        ani.SetFloat("移動", Mathf.Abs(h) + Mathf.Abs(v));

        // 如果有控制在讓角度旋轉
        if (h != 0 || v != 0)
        {
            // 角度 = 面相角度(位移的向量)
            Quaternion angle = Quaternion.LookRotation(pos);
            // 插值
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, Time.fixedDeltaTime * turn);
        }
    }
}
