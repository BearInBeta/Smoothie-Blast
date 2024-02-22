using UnityEngine;

public class TouchInput : MonoBehaviour
{
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    public float minMag;
    private bool isDragging = false;

    // Update is called once per frame
    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check touch phase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Record touch start position
                    touchStartPosition = touch.position;
                    getColliderHit();
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    // Record touch end position while dragging
                    //getColliderHit(touch);
                    if (isDragging)
                    {
                        touchEndPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    // Process the touch gesture
                    if (isDragging)
                    {
                        DetermineRotationDirection();
                        isDragging = false;
                    }
                    break;
            }
        }
    }
    private void getColliderHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(touchStartPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<Hex>() != null)
        {
            // Check if the collider hit is the one you want
            hit.collider.gameObject.GetComponent<Hex>().chooseRing();
        }
    }
    // Determine rotation direction based on touch positions
    private void DetermineRotationDirection()
    {
        // Calculate the difference between start and end positions
        Vector2 touchDelta = touchEndPosition - touchStartPosition;
        if(touchDelta.magnitude <= minMag)
        {
            return;
        }
        //theta = atan2((y1 - y0), (x1 - x0))

        float theta = GlobalMethods.RoundToNearestPI(Mathf.Atan2(touchDelta.y, touchDelta.x));
        bool horizontal = Mathf.Abs(theta) != Mathf.PI / 2;
        // Check if the touch is on the right half of the screen
        bool isRightHalf = touchStartPosition.x > Screen.width / 2;

        // Check if the touch is on the top half of the screen
        bool isTopHalf = touchStartPosition.y > Screen.height / 2;

        // Determine clockwise or counterclockwise based on direction
        if (horizontal)
        {
            GetComponent<GameController>().rotateRing((isTopHalf && touchDelta.x > 0) || (!isTopHalf && touchDelta.x < 0));
        }
        else
        {
            GetComponent<GameController>().rotateRing((isRightHalf && touchDelta.y < 0) || (!isRightHalf && touchDelta.y > 0));
        }
            

         
    }




}


