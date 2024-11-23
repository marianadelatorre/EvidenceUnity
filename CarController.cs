using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Networking;

[System.Serializable]
public class Point
{
    public float x, y;
    public Vector3 ToVector3() => new Vector3(x, 0, y); // Mapear y como z en Unity
}

[System.Serializable]
public class CarData
{
    public List<Point> points;
}

public class CarController : MonoBehaviour
{
    [SerializeField]
    GameObject carPrefab;
    GameObject car;
    [SerializeField]
    List<Vector3> path;
    List<Vector3> originals;
    Matrix4x4 mem, tra, rot, m, Sca;
    int index, rotCounter; // Index for the path
    bool corner;
    Vector3 pivot;

    // Tamaño de cada celda en Unity
    private const float cellSize = 24.0f;
    // Offset del plano en Unity
    private static readonly Vector3 planeOffset = new Vector3(50, 0, 50);

    // Start is called before the first frame update
    void Start()
    {
        index = rotCounter = 0;
        car = Instantiate(carPrefab, Vector3.zero, Quaternion.identity);
        originals = new List<Vector3>(car.GetComponent<MeshFilter>().mesh.vertices);
        mem = VecOps.TranslateM(path[index]);
        tra = VecOps.TranslateM(new Vector3(0, 0, -0.1f));
        m = Matrix4x4.identity;

        // Iniciar la solicitud para obtener posiciones de los carros
        StartCoroutine(RequestCarPositions());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currPos = new Vector3(mem[0, 3], mem[1, 3], mem[2, 3]);

        if (!corner) //go forwards
        {
            Vector3 nextPath = path[index + 1];
            Vector3 currPath = path[index];
            float dist = VecOps.Magnitude(nextPath - currPos);
            if (dist <= 0.01f)
            {
                index++;
                nextPath = path[index +1];
                Vector3 afterNextPath = path[index + 2];
                if (currPath.x != afterNextPath.x && currPath.z != afterNextPath.z)
                {
                    corner = true;
                    pivot = new Vector3(
                        (afterNextPath.x + nextPath.x) / 2.0f,
                        0,
                        (afterNextPath.z + nextPath.z) / 2.0f
                    );
                    Debug.Log(pivot);
                    tra = Matrix4x4.identity;
                }
            }
        }
        else
        {
            tra = Matrix4x4.identity;
            if (rotCounter < 90)
            {
                Matrix4x4 mpiv = VecOps.TranslateM(pivot);
                Matrix4x4 Mpneg = VecOps.TranslateM(-pivot);
                Matrix4x4 rotM = VecOps.RotateYM(rotCounter);
                Matrix4x4 Tc = VecOps.TranslateM(currPos);
                rot = mpiv * rotM * Mpneg * Tc;
                rotCounter++;
            }
            else
            {
                rotCounter = 0;
                corner = false;
                index++;
                rot = Matrix4x4.identity;
                tra = VecOps.TranslateM(new Vector3(0, 0, -0.1f));
            }
        }

        m = mem * tra * rot * Sca;
        car.GetComponent<MeshFilter>().mesh.vertices = VecOps.ApplyTransform(originals, m).ToArray();
        mem = mem * tra * rot;
    }

    private IEnumerator RequestCarPositions()
    {
        string url = "url"; // Reemplazar con el endpoint real
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al acceder al endpoint: {www.error}");
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"Respuesta del JSON:\n{jsonResponse}");

                CarData carData = JsonUtility.FromJson<CarData>(jsonResponse);
                if (carData != null)
                {
                    HandleCarData(carData);
                }
                else
                {
                    Debug.LogError("Error al deserializar el JSON.");
                }
            }
        }
    }

    private void HandleCarData(CarData carData)
    {
        Debug.Log("Puntos recibidos para los carros:");
        path = new List<Vector3>();

        foreach (var point in carData.points)
        {
            // Escalar las coordenadas del JSON al plano de Unity
            float scaledX = point.x * cellSize;
            float scaledZ = point.y * cellSize; // y del JSON se mapea como z en Unity
            Vector3 scaledPosition = new Vector3(scaledX, 0, scaledZ) + planeOffset;

            Debug.Log($"Posición escalada: {scaledPosition}");
            path.Add(scaledPosition);
        }

        // Reinicia el movimiento del carro en la nueva ruta
        index = 0;
    }
}
