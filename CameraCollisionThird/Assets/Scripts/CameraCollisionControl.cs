using UnityEngine;

public class CameraCollisionControl : MonoBehaviour
{
    [Header("平滑"), Range(0, 1)]
    public float smoothing = 1;
    [Header("範圍限制：下，上")]
    public Vector2 limit = new Vector2(-60, 60);
    [Header("速度"), Range(0, 100)]
    public float speed = 1;
    [Header("攝影機")]
    public Transform cam;
    [Header("目標")]
    public Transform target;
    [Header("半徑"), Range(0, 1.5f), Tooltip("建議半徑不要太大")]
    public float radius = 1;
    [Header("攝影機原始位置")]
    public Vector3 posOriginal;

    /// <summary>
    /// 攝影機角度
    /// </summary>
    private Quaternion rot;

    private void Start()
    {
        Cursor.visible = false;

        rot = transform.localRotation;      // 取得原始角度
        posOriginal = cam.localPosition;    // 取得攝影機原始位置
    }

    private void Update()
    {
        Track();
        Control();
        CollisionCheck();
    }

    private void OnDrawGizmos()
    {
        // 繪製攝影機到腳色的線條
        Gizmos.color = new Color(1, 0, 0, 0.9f);
        Gizmos.DrawLine(cam.position, target.position + Vector3.up);
        
        // 繪製攝影機圓形碰撞範圍
        Gizmos.color = new Color(1, 0, 1, 0.3f);
        Gizmos.DrawSphere(cam.position, radius);
    }

    /// <summary>
    /// 追蹤
    /// </summary>
    private void Track()
    {
        Vector3 posA = transform.position;      // A 點：攝影機
        Vector3 posB = target.position;         // B 點：目標

        posA = Vector3.Lerp(posA, posB, speed * Time.deltaTime);    // 插值

        transform.position = posA;
    }

    /// <summary>
    /// 控制攝影機
    /// </summary>
    private void Control()
    {
        rot.x += Input.GetAxis("Mouse Y") * -smoothing;                     // 取得滑鼠上下 滑鼠上下與角度相反
        rot.y += Input.GetAxis("Mouse X") * smoothing;                      // 取得滑鼠左右

        rot.x = Mathf.Clamp(rot.x, limit.x, limit.y);                       // 限制上下

        transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);    // 指定角度
    }

    /// <summary>
    /// 碰撞檢查
    /// </summary>
    private void CollisionCheck()
    {
        RaycastHit hit;         // 攝影機到角色的射線碰撞
        Collider[] cols;        // 攝影機圓形碰撞範圍的碰撞資訊

        // 繪製圓形碰撞氣(攝影機位置，半徑，要偵測的圖層 1 << 圖層編號)
        cols = Physics.OverlapSphere(cam.position, radius, 1 << 8);

        // 如果 線條碰撞(攝影機位置，角色位置，射線碰撞資訊，圖層)
        if (Physics.Linecast(cam.position, target.position + Vector3.up, out hit, 1 << 8))
        {
            // 攝影機 - 角色 的距離
            float disTarget = Vector3.Distance(cam.position, target.position + Vector3.up);
            // 攝影機 - 碰撞點 的距離
            float disPoint = Vector3.Distance(cam.position, hit.point);
            // 插值 - 讓攝影機跑到碰撞點
            cam.localPosition = Vector3.Lerp(cam.localPosition, target.position + Vector3.up, (disPoint + 3f) / disTarget * Time.deltaTime * 10);
        }
        // 否則 如果(碰撞資訊.長度 為 零)
        else if (cols.Length == 0)
        {
            // 插值 - 攝影機恢復原本位置
            cam.localPosition = Vector3.Lerp(cam.localPosition, posOriginal, Time.deltaTime * 10);
        }
    }
}
