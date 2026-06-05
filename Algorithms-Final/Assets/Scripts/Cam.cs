using UnityEngine;

public class Cam : MonoBehaviour
{
    public Camera cam;
    private NewDungeonGenerator dungeonGen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGen = FindAnyObjectByType<NewDungeonGenerator>();
        // 64 - 32
        // 30 - 50 - 15

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
        Vector3 CamPos = new(dungeonGen.startRoom.width / 256f * 120f, 100f * (dungeonGen.startRoom.width / 256f + dungeonGen.startRoom.height / 128f), dungeonGen.startRoom.height / 64f * 35f);
        cam.transform.position = CamPos;
    }
}
