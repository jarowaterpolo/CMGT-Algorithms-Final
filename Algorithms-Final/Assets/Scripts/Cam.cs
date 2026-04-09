using UnityEngine;

public class Cam : MonoBehaviour
{
    public Camera cam;
    NewDungeonGenerator DungeonGen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonGen = FindAnyObjectByType<NewDungeonGenerator>();
        // 128 - 64
        // 60 - 100 - 35
        ///
        // 256 - 64
        // 120 - 200 - 70

        // x is width startroom
        // z is heigth startroom
    }
    public void UpdateCam()
    {
        Vector3 CamPos = new(DungeonGen.StartRoom[0].width / 128 * 60, 100 * (DungeonGen.StartRoom[0].width / 256f + DungeonGen.StartRoom[0].height / 128f), DungeonGen.StartRoom[0].height / 64 * 35);
        cam.transform.position = CamPos;
    }
}
