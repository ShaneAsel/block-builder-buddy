//     void PlaceBlock(RaycastHit hit)
// {
//     GameObject blockToPlace = blockPrefabs[selectedBlockIndex];

//     // Get block height from its collider
//     float blockHeight = 1f;
//     if (blockToPlace.TryGetComponent(out Collider col))
//         blockHeight = col.bounds.size.y;

//     // Offset from the hit point
//     Vector3 position = hit.point + hit.normal * (blockHeight / 2f);

//     // Snap to grid
//     float gridSize = 1f;
//     position = new Vector3(
//         Mathf.Round(position.x / gridSize) * gridSize,
//         Mathf.Round(position.y / gridSize) * gridSize,
//         Mathf.Round(position.z / gridSize) * gridSize
//     );

//     // Instantiate
//     GameObject newBlock = Instantiate(blockToPlace, position, Quaternion.identity);

//     // ✅ Tag & layer setup
//     if (IsTagDefined("Block"))
//         newBlock.tag = "Block";

//     // ✅ Set layer to Placeable so future raycasts can hit it
//     newBlock.layer = LayerMask.NameToLayer("Placeable");
// }

using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPlacer : MonoBehaviour
{
    [Header("Block Setup")]
    public GameObject[] blockPrefabs;
    public Camera mainCamera;
    public LayerMask placementMask;
    private int selectedBlockIndex = 0;

    [Header("Snap Settings")]
    public float gridSize = 1f;

    void Update()
    {
    #if UNITY_EDITOR
        HandleMouseInput();
    #else
        HandleTouchInput();
    #endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
            {
                if (Input.GetKey(KeyCode.LeftShift) && hit.collider.CompareTag("Block"))
                    Destroy(hit.collider.gameObject);
                else
                    PlaceBlock(hit);
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
                {
                    PlaceBlock(hit);
                }
            }
        }
    }

    void PlaceBlock(RaycastHit hit)
{
    GameObject blockToPlace = blockPrefabs[selectedBlockIndex];
    if (blockToPlace == null) return;

    // Default to ground-based positioning
    Vector3 basePos;

    if (hit.collider.CompareTag("Block"))
    {
        // Hit a block — place next to it using its snapped position
        basePos = hit.transform.position + hit.normal * gridSize;
    }
    else
    {
        // Hit the ground — use snapped hit.point + upward normal
        basePos = hit.point + hit.normal * (gridSize / 2f);
    }

    // Snap to grid
    Vector3 finalPos = new Vector3(
        Mathf.Round(basePos.x / gridSize) * gridSize,
        Mathf.Round(basePos.y / gridSize) * gridSize,
        Mathf.Round(basePos.z / gridSize) * gridSize
    );

    // Debug
    Debug.Log($"[PLACE] Hit: {hit.collider.name} | Normal: {hit.normal} | Final Position: {finalPos}");
    Debug.DrawRay(hit.point, hit.normal * 2f, Color.red, 1f);

    GameObject newBlock = Instantiate(blockToPlace, finalPos, Quaternion.identity);
    newBlock.name = $"Block_{finalPos.x}_{finalPos.y}_{finalPos.z}";

    if (IsTagDefined("Block"))
        newBlock.tag = "Block";

    newBlock.layer = LayerMask.NameToLayer("Placeable");
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
