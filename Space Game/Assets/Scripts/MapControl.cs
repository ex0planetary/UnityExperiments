using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    bool freeLook = false;

    public float moveSpeed = 1.0f;
    public float lookSpeed = 1.0f;

    public GameObject universeMaster;
    public GameObject nebulae;

    Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
    }

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int xPos, int yPos);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            freeLook = !freeLook;
        }
        if (freeLook)
        {
            Vector3 movement = (trans.forward * Input.GetAxis("Vertical")) + (trans.right * Input.GetAxis("Horizontal")) + (trans.up * Input.GetAxis("MovementAxis3"));
            trans.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            Vector3 rot = new Vector3(-1 * Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            trans.Rotate(rot * lookSpeed);
            SetCursorPos(1000, 200);
        }
        CreateSectors cs = universeMaster.GetComponent<CreateSectors>();
        if (trans.position.x > 20)
        {
            cs.SectorShift(1, 0, 0);
            nebulae.GetComponent<SpaceCloudPositionData>().center.x += 1;
            trans.Translate(-40, 0, 0, Space.World);
        }
        if (trans.position.x < -20)
        {
            cs.SectorShift(-1, 0, 0);
            nebulae.GetComponent<SpaceCloudPositionData>().center.x -= 1;
            trans.Translate(40, 0, 0, Space.World);
        }
        if (trans.position.y > 20)
        {
            cs.SectorShift(0, 1, 0);
            nebulae.GetComponent<SpaceCloudPositionData>().center.y += 1;
            trans.Translate(0, -40, 0, Space.World);
        }
        if (trans.position.y < -20)
        {
            cs.SectorShift(0, -1, 0);
            nebulae.GetComponent<SpaceCloudPositionData>().center.y -= 1;
            trans.Translate(0, 40, 0, Space.World);
        }
        if (trans.position.z > 20)
        {
            cs.SectorShift(0, 0, 1);
            nebulae.GetComponent<SpaceCloudPositionData>().center.z += 1;
            trans.Translate(0, 0, -40, Space.World);
        }
        if (trans.position.z < -20)
        {
            cs.SectorShift(0, 0, -1);
            nebulae.GetComponent<SpaceCloudPositionData>().center.z -= 1;
            trans.Translate(0, 0, 40, Space.World);
        }
    }
}
