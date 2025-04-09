using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPlacer : MonoBehaviour
{
    [Header("Block Setup")]
    public GameObject[] blockPrefabs;
    public Camera mainCamera;
    public LayerMask placementMask;
    private int selectedBlockIndex = 0;
    private Vector2 touchStartPos;
    private float maxTapMovement = 10f; // pixels

    void Update()
    {
        // Mobile Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Only act on touch release
            if (touch.phase == TouchPhase.Ended)
            {
                // Ignore if finger is over UI
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
                {
                    PlaceBlock(hit);
                }
            }
        }

        #if UNITY_EDITOR
        // Editor Mouse Input
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Hold Shift to delete blocks
                if (Input.GetKey(KeyCode.LeftShift) && hit.collider.CompareTag("Block"))
                {
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    PlaceBlock(hit);
                }
            }
        }
        #endif
    }

    void PlaceBlock(RaycastHit hit)
    {
        // Add half of the normal so the new block sits on top of the hit surface
        Vector3 position = hit.point + hit.normal / 2f;

        // Round the position so the block snaps to the grid
        position = new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            Mathf.Round(position.z)
        );

        GameObject block = Instantiate(blockPrefabs[selectedBlockIndex], position, Quaternion.identity);

        if (IsTagDefined("Block"))
        {
            block.tag = "Block";
        }
    }

    public void SetBlockIndex(int index)
    {
        selectedBlockIndex = index;
    }

    public void ResetAllBlocks()
    {
        if (!IsTagDefined("Block"))
            return;

        GameObject[] allBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in allBlocks)
        {
            Destroy(block);
        }
    }

    bool IsTagDefined(string tagName)
    {
        try
        {
            GameObject temp = new GameObject();
            temp.tag = tagName;
            Destroy(temp);
            return true;
        }
        catch
        {
            Debug.LogWarning($"Tag '{tagName}' is not defined. Please add it via Unity > Tags and Layers.");
            return false;
        }
    }
}
