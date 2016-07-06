using UnityEngine;
using System.Collections;

public class theStack : MonoBehaviour {

    public Color32[] gameColors = new Color32[4];
    public Material stackMat;

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED=5.0f;
    private const float ERROR_MARGING=0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3;

    private GameObject[] TheStack;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int stackIndex ;
    private int scoreCount = 0;
    private int combo = 0; 

    private float tileTransform = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPositoin;

    private bool isMovingOnX = true;
    private bool gameOver = false; 

    private Vector3 desiredPosition;
    private Vector3 lastTileposition;

	// Use this for initialization
	private void Start ()
    {
        TheStack = new GameObject[transform.childCount];
        for (int i= 0; i < transform.childCount ; i++)
        {
            TheStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(TheStack[i].GetComponent<MeshFilter>().mesh);
        }
            

        stackIndex = transform.childCount - 1;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(placeTile())
            {
                SpawnTile();
                scoreCount++;
            }
            else
            {
                EndGame();
            }
            
        }

        MoveTile();

        //Move the Stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

    private void createRubble(Vector3 pos , Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        go.GetComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }

    private void MoveTile()
    {
        if (gameOver)
            return;

        tileTransform += Time.deltaTime * tileSpeed;
        
        if(isMovingOnX)
            TheStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransform) * BOUNDS_SIZE, scoreCount, secondaryPositoin);
        else
            TheStack[stackIndex].transform.localPosition = new Vector3(secondaryPositoin, scoreCount, Mathf.Sin(tileTransform) * BOUNDS_SIZE);
    }

    private void SpawnTile()
    {
        lastTileposition = TheStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPosition = (Vector3.down) * scoreCount;
        TheStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        TheStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(TheStack[stackIndex].GetComponent<MeshFilter>().mesh);
    }

    private bool placeTile()
    {
        Transform t = TheStack[stackIndex].transform;
         
        if(isMovingOnX)
        {
            float deltaX = lastTileposition.x - t.position.x;
            if(Mathf.Abs(deltaX) > ERROR_MARGING)
            {
                //Cut The Tile
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTileposition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
                createRubble(
                    new Vector3((t.position.x>0)
                    ? t.position.x + (t.localScale.x/2)
                    : t.position.x - (t.localScale.x / 2)
                    , t.position.y 
                    ,t.position.z),
                    new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
                    );
                t.localPosition = new Vector3( middle - (lastTileposition.x / 2), scoreCount, lastTileposition.z);
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUNDS_SIZE)
                        stackBounds.x = BOUNDS_SIZE;

                    float middle = lastTileposition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTileposition.x / 2), scoreCount, lastTileposition.z);
                }
                combo++;
                t.localPosition = new Vector3(lastTileposition.x,scoreCount,lastTileposition.z);
            }
        }
        else
        {
            float deltaZ = lastTileposition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGING)
            {
                //Cut The Tile
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTileposition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                createRubble(
                   new Vector3((t.position.x > 0)
                   ? t.position.x + (t.localScale.x / 2)
                   : t.position.x - (t.localScale.x / 2)
                   , t.position.y
                   , t.position.z),
                   new Vector3(Mathf.Abs(deltaZ), 1, t.localScale.z)
                   );
                t.localPosition = new Vector3(lastTileposition.x, scoreCount, middle - (lastTileposition.z / 2));
            }
            else
            {
                if(combo>COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUNDS_SIZE)
                        stackBounds.y = BOUNDS_SIZE;

                    float middle = lastTileposition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTileposition.x, scoreCount, middle - (lastTileposition.z / 2));
                }
                combo++;
                t.localPosition = new Vector3(lastTileposition.x, scoreCount, lastTileposition.z);
            }
        }

        secondaryPositoin = (isMovingOnX)
            ? t.localPosition.x
            : t.localPosition.z;

        isMovingOnX = !isMovingOnX;
        return true;
    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.25f);
        
        for(int i=0;i<vertices.Length;i++)
        {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3],f);
        }

        mesh.colors32 = colors;
    }

    private Color32 Lerp4(Color32 a , Color32 b, Color32 c , Color32 d , float t)
    {
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
    } 

    private void EndGame()
    {
        Debug.Log("Lose");
        gameOver = true;
        TheStack[stackIndex].AddComponent<Rigidbody>();
    }
}
