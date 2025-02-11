using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndShoot : MonoBehaviour
{
    [Header("Movement")]
    public float maxPower;
    float shootPower;
    public float gravity = 1;

    [Range(0f, 0.1f)]
    public float slowMotion;

    public bool shootWhileMoving = false;
    public bool forwardDraging = true;
    public bool showLineOnScreen = false;

    Transform direction;
    Rigidbody2D rb;
    LineRenderer line;
    LineRenderer screenLine;

    // Vectors //
    Vector2 startPosition;
    Vector2 targetPosition;
    Vector2 startMousePos;
    Vector2 currentMousePos;
    private Transform textTransform;

    bool canShoot = true;

    //test_ball

    private Rigidbody2D rb2d;
    private Collider2D col2d;

    void Start()
    {
        textTransform = transform.Find("Number_Text"); 

        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = gravity;
            line = GetComponent<LineRenderer>();
            direction = transform.GetChild(0);
            screenLine = direction.GetComponent<LineRenderer>();
        }

        //test_ball
        // 获取 GameObject 上的 Rigidbody2D 和 Collider2D 组件
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
        
    }

    void Update()
    {
        //test_ball

        if (textTransform != null)
        {
            textTransform.position = transform.position; // 保持 text 与 ball 位置一致
        }

        // if (transform.position.y < -3.9f && PlayerControl.nearestBall == null)
        // {
        //     // 切换 Rigidbody2D 和 Collider2D 的勾选状态
        //     rb2d.simulated = false;
        //     col2d.enabled = false;
        //     Debug.Log(rb2d.simulated);
        // }

        if (PlayerControl.nearestBall == null)
        {
            return;
        }

        if (gameObject != PlayerControl.nearestBall)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // if (EventSystem.current.currentSelectedGameObject) return;  //ENABLE THIS IF YOU DONT WANT TO IGNORE UI
            // 切换 Rigidbody2D 和 Collider2D 的勾选状态
            rb2d.simulated = true;
            col2d.enabled = true;
            col2d.isTrigger = false;
            // Debug.Log(rb2d.simulated);
            MouseClick();
        }
        if (Input.GetMouseButton(0))
        {
            // if (EventSystem.current.currentSelectedGameObject) return;  //ENABLE THIS IF YOU DONT WANT TO IGNORE UI
            MouseDrag();

            if (shootWhileMoving)
                rb.velocity /= (1 + slowMotion);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //test_ball
            PlayerControl.nearestBall = null;
            
            // if (EventSystem.current.currentSelectedGameObject) return;  //ENABLE THIS IF YOU DONT WANT TO IGNORE UI
            MouseRelease();

        }

        if (shootWhileMoving)
            return;

        if (rb.velocity.magnitude < 0.7f)
        {
            rb.velocity = new Vector2(0, 0); //ENABLE THIS IF YOU WANT THE BALL TO STOP IF ITS MOVING SO SLOW
            canShoot = true;
        }
    }

    void OnCollisionEnter2D(Collision2D platforms)
    {
        // Debug.Log(col2d.enabled);
        if ((platforms.gameObject.CompareTag("Platform_Solid") || platforms.gameObject.CompareTag("Platform_Mutate") ||platforms.gameObject.CompareTag("Ground")) && PlayerControl.nearestBall == null)
        {
            foreach (ContactPoint2D hit in platforms.contacts)
            {
                if (hit.normal == Vector2.up)
                {
                    rb2d.simulated = false;
                    col2d.isTrigger = true;
                }
            }
            // rb2d.simulated = false;
            // col2d.enabled = false;
        }
    }

    // MOUSE INPUTS
    void MouseClick()
    {
        if (shootWhileMoving)
        {
            Vector2 dir = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.right = dir * 1;

            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            if (canShoot)
            {
                Vector2 dir =
                    transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.right = dir * 1;

                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        if (textTransform != null)
        {
            textTransform.position = transform.position; // 保持 text 与 ball 位置一致
        }
    }

    void MouseDrag()
    {
        if (shootWhileMoving)
        {
            LookAtShootDirection();
            DrawLine();

            if (showLineOnScreen)
                DrawScreenLine();

            float distance = Vector2.Distance(currentMousePos, startMousePos);

            if (distance > 1)
            {
                line.enabled = true;

                if (showLineOnScreen)
                    screenLine.enabled = true;
            }
        }
        else
        {
            if (canShoot)
            {
                LookAtShootDirection();
                DrawLine();

                if (showLineOnScreen)
                    DrawScreenLine();

                float distance = Vector2.Distance(currentMousePos, startMousePos);

                if (distance > 1)
                {
                    line.enabled = true;

                    if (showLineOnScreen)
                        screenLine.enabled = true;
                }
            }
        }
        if (textTransform != null)
        {
            textTransform.position = transform.position; // 保持 text 与 ball 位置一致
        }
    }

    void MouseRelease()
    {
        if (
            shootWhileMoving /*&& !EventSystem.current.IsPointerOverGameObject()*/
        )
        {
            Shoot();
            screenLine.enabled = false;
            line.enabled = false;
            GlobalVariables.numberShot++;
        }
        else
        {
            if (
                canShoot /*&& !EventSystem.current.IsPointerOverGameObject()*/
            )
            {
                Shoot();
                screenLine.enabled = false;
                line.enabled = false;
                GlobalVariables.numberShot++;
            }
        }
    }

    // ACTIONS
    void LookAtShootDirection()
    {
        Vector3 dir = startMousePos - currentMousePos;

        if (forwardDraging)
        {
            transform.right = dir * -1;
        }
        else
        {
            transform.right = dir;
        }

        float dis = Vector2.Distance(startMousePos, currentMousePos);
        dis *= 4;

        if (dis < maxPower)
        {
            direction.localPosition = new Vector2(dis / 6, 0);
            shootPower = dis;
        }
        else
        {
            shootPower = maxPower;
            direction.localPosition = new Vector2(maxPower / 6, 0);
        }
    }

    public void Shoot()
    {
        canShoot = false;
        rb.velocity = transform.right * shootPower;
    }

    void DrawScreenLine()
    {
        screenLine.positionCount = 1;
        screenLine.SetPosition(0, startMousePos);

        screenLine.positionCount = 2;
        screenLine.SetPosition(1, currentMousePos);
    }

    void DrawLine()
    {
        startPosition = transform.position;

        line.positionCount = 1;
        line.SetPosition(0, startPosition);

        targetPosition = direction.transform.position;
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        line.positionCount = 2;
        line.SetPosition(1, targetPosition);
    }

    Vector3[] positions;
}
