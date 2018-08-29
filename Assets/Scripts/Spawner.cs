using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour
{
    #region Unity Public Variables
    public float boundsSafeArea; //X axis, amount to subtract in order to keep shapes in view completely.
    public float bottomGravScale; //Lowest value that the random grav scale of each spawned shape can be
    public float topGravScale; //Highest value that the random grav scale of each spawned shape can be.
    //All the shapes
	//White
    public Sprite WhiteCircle;
    public Sprite WhiteHexagon;
    public Sprite WhiteSquare;
    public Sprite WhiteTriangle;
	//Orange
	public Sprite OrangeCircle;
	public Sprite OrangeHexagon;
	public Sprite OrangeSquare;
	public Sprite OrangeTriangle;
	//Green
	public Sprite GreenCircle;
	public Sprite GreenHexagon;
	public Sprite GreenSquare;
	public Sprite GreenTriangle;
	//Blue
	public Sprite BlueCircle;
	public Sprite BlueHexagon;
	public Sprite BlueSquare;
	public Sprite BlueTriangle;
    //Customizable Difficulty
    public int levelIncreaseFactor; //1 is normal, 2 would mean twice as many shapes and so forth.
    public int numberOfLives;
    //UI
    public Text livesLabel;
    public Text untappableLabel;
    public Text levelLabel;
    public CanvasGroup gameOverUI;
    public Button gameOverButton;
    #endregion
    #region Unity Private Variables
    private Sprite[] shapeArray;
    private List<GameObject> spawnedShapesList = new List<GameObject>();
    private int size = 16; //Eventually it will be 16
    private int level = 0; //The first level is actually 1, this variable is 0 because it will be increased by one when StartNextLevel() is called at Start()
    private Sprite untappableSPR; //This is the shape that you cannot tap or you will lose.
    private bool isPause = false;
    private float camSize;
    private int privateLives;
    private bool shouldCheckForFall = true;
    private bool isGameOver = false;
	private int gravScale = 1;
    #endregion
    #region Unity Functions
    // Use this for initialization
    void Start ()
    {
        //Camera Stuff
        float aspectRatio = Screen.width / Screen.height;
        camSize = Camera.main.orthographicSize;
        float near = Camera.main.nearClipPlane;
        float far = Camera.main.farClipPlane;
        //Camera.main.projectionMatrix = Matrix4x4.Ortho(-camSize * aspectRatio, camSize * aspectRatio, -camSize, camSize, near, far);
        //Init some stuff
		privateLives = numberOfLives;
        //untappableSPR = new Sprite();
        shapeArray = new Sprite[size]; //Setup the array that will hold all of the shapes that can be spawned. Do not confuse this with spawnedShapesList.
        //White Shapes
		shapeArray[0] = WhiteCircle;
        shapeArray[1] = WhiteHexagon;
        shapeArray[2] = WhiteSquare;
        shapeArray[3] = WhiteTriangle;
		//Orange Shapes
		shapeArray [4] = OrangeCircle;
		shapeArray [5] = OrangeHexagon;
		shapeArray [6] = OrangeSquare;
		shapeArray [7] = OrangeTriangle;
		//Blue Shapes 
		shapeArray [8] = BlueCircle;
		shapeArray [9] = BlueHexagon;
		shapeArray [10] = BlueSquare;
		shapeArray [11] = BlueTriangle;
		//Green Shapes
		shapeArray [12] = GreenCircle;
		shapeArray [13] = GreenHexagon;
		shapeArray [14] = GreenSquare;
		shapeArray [15] = GreenTriangle;
        //UI

        gameOverUI.alpha = 0f;
        gameOverButton.GetComponent<Button>().onClick.AddListener(RestartClicked);
        //Start Game!
        StartNextLevel();
    }

    void RestartClicked()
    {
		print ("RESTART BUTTON CLICKED");
        RestartGame();
    }

    //Called once per frame.
    void Update()
    {
        //sine stuff
        for (int i = 0; i < spawnedShapesList.Count; i++)
        {
            
        }
        //UI
        levelLabel.text = "Level " + level;
        livesLabel.text = "Lives: " + privateLives.ToString();
        //untappableLAbel.text = "Do not tap " + untappableSPR.name.ToString();

        //Check to see if there are any shapes left.
        if (isGameOver == false)
        {
            if (spawnedShapesList.Count == 0)
            {
				//Problem?
				print("Game is not over and there are no more spawned shapes");
                StartNextLevel();
            }
        }

        //Mouse Click
        if (isGameOver == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                GameObject hitObject = hit.collider.gameObject;
                //Check for hit
                if (hit)
                {

                    if (hitObject.tag == untappableSPR.name.ToString())
                    {
                        //Mistap
                        Mistap();
                    }

                    if (hitObject.tag != untappableSPR.name.ToString())
                    {
                        //Not game over
                        RemoveShape(hitObject);
                    }
                }
            }
        }
        //Touch

        //Checking for out of bounds. (This might be where the error is).
        if (shouldCheckForFall == true)
        {
            for (int i = 0; i < spawnedShapesList.Count; i++)
            {

                GameObject go = spawnedShapesList[i]; //Try removing this.
                if (go != null)
                {
					if (go.transform.position.y < -camSize && gravScale == 1)
                    {
                        //Decide whether mistap or not
                        if (go.tag != untappableSPR.name.ToString())
                        {
                            //Shape Fell
                            shouldCheckForFall = false;
                            ShapeFell(go);
                        }
                        if (go.tag == untappableSPR.name.ToString())
                        {
                            //Not game over
                            //print(go.name.ToString() + " Has left bounds in Y axis.");
                            RemoveShape(spawnedShapesList[i]);
                            //print("Destroyed " + go.name.ToString());
                        }
                    }

					if (go.transform.position.y > camSize && gravScale == -1)
					{

						print ("GRAVITY SCALE IS NEGATIVE");

						//Decide whether mistap or not
						if (go.tag != untappableSPR.name.ToString())
						{
							//Shape Fell
							shouldCheckForFall = false;
							ShapeFell(go);
						}
						if (go.tag == untappableSPR.name.ToString())
						{
							//Not game over
							//print(go.name.ToString() + " Has left bounds in Y axis.");
							RemoveShape(spawnedShapesList[i]);
							//print("Destroyed " + go.name.ToString());
						}
					}
                }
            }
        }
    }
    #endregion
    #region Game Event Functions
    /*void EndLevel() //Called at end of level
    {


        StartNextLevel();
    }*/

    void SineShape(GameObject go)
    {
        //go.transform.position.x = Mathf.Sin(go, transform.position.y);
        //turn on bool
    }

    void RemoveShape(GameObject go)
    {
        Destroy(go);
        spawnedShapesList.Remove(go);
    }

	public void RestartGame()
    {
		print ("RESTARTING GAME...");
        isGameOver = false;
        gameOverUI.alpha = 0f;
        level = 0;
        privateLives = numberOfLives;
		gravScale = 1;
		for (int i = 0; i < spawnedShapesList.Count; i++)
		{
			Destroy (spawnedShapesList [i]);
		}
		spawnedShapesList.Clear ();
		StartNextLevel();
    }

    public void StartNextLevel() //Called at end of level (EndLevel)
    {
		print ("STARTING NEXT LEVEL...");
        //Increase level variable by 1
        level += 1;

		for (int i = 0; i < spawnedShapesList.Count; i++)
		{
			Destroy (spawnedShapesList [i]);
		}
		spawnedShapesList.Clear ();
        //Determine the shape that should not be touched.
        untappableSPR = SelectUntappableShape();
        //UI
        untappableLabel.text = "Do not tap " + untappableSPR.name.ToString();
        //Determine amount of shapes to be spawned
        float squared = Mathf.Sqrt(level);
        float rounded = Mathf.Round(squared);
        float final = rounded * levelIncreaseFactor;
        print("Final level calculation is " + final.ToString());

        int converted = Mathf.RoundToInt(final);

		if (converted >= 0) {
			int chance = Random.Range (0, 101);
			if (chance > 50) {
				gravScale *= -1;
			}
		}

        SpawnShape(converted);


		for (int i = 0; i < spawnedShapesList.Count; i++) {
			spawnedShapesList [i].GetComponent<Rigidbody2D> ().gravityScale *= gravScale;
		}

    }

    void ShapeFell(GameObject shape) //Called when a shape that is not supposed to fall through does so.
    {
        //Still has lives left
        if (privateLives > 0)
        {
            privateLives--;
            RemoveShape(shape);
            shouldCheckForFall = true;
        }
        //No lives left
        if (privateLives <= 0)
        {
			print("GAME OVER DUE TO: SHAPEFELL");
            GameOver();
        }
    }

    void Mistap()
    {
        //Still has lives left
        if (privateLives > 0)
        {
            privateLives--;
        }
        //No lives left
        if (privateLives <= 0)
        {
			print("GAME OVER DUE TO: MISTAP");
            GameOver();
        }
    }

    void GameOver() //Called on game over
    {
        isGameOver = true;
        gameOverUI.alpha = 1f;
        gameOverUI.blocksRaycasts = true;
		for (int i = 0; i < spawnedShapesList.Count; i++)
        {
			Destroy (spawnedShapesList [i]);
        }
		spawnedShapesList.Clear ();
    }

    void SpawnShape(int times) //Initializes and sets position by using 'getLocationToSpawn' function. Also makes sure of no overlaps automatically and recalls itself.
    {
        for (int i = 0; i < times; i++)
        {
            Vector2 loc = getLocationToSpawn();
            Sprite spr = ReturnShapeToSpawn();
            GameObject go = new GameObject(spr.name.ToString());
            go.tag = spr.name.ToString();
            CircleCollider2D collider2d = go.AddComponent<CircleCollider2D>();
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = Random.Range(bottomGravScale, topGravScale); //Random value for the grav scale



            renderer.sprite = spr;
            go.transform.position = loc;
            //print("Spawned a " + spr.name.ToString() + " at:" + loc.ToString());
            spawnedShapesList.Add(go);
        }
        //if (spawnedShapesList.Count !=0)
        //{
            //print("Passed");
			for (int i = 0; i < spawnedShapesList.Count-1; i++)
            {
				try {
                	if (spawnedShapesList[i].GetComponent<SpriteRenderer>().bounds.Intersects(spawnedShapesList[i + 1].GetComponent<SpriteRenderer>().bounds)) {
                    	print(spawnedShapesList[i].name.ToString() + " Intersects with " + spawnedShapesList[i + 1].name.ToString());
                    	RemoveShape(spawnedShapesList[i + 1]);
                    	SpawnShape(1);
                	}
				} catch (UnityException e) {
					
				}
            }
        //}
    }

    #endregion
    #region Game Return Functions
    Vector2 getLocationToSpawn() //Returns a Vector2 with a static y value and an x value calculated between two points.
	{
		Vector2 vector;

		float camSizeWidth = camSize * Camera.main.aspect;

		float range1 = (camSizeWidth); //-5
		//range1 = Mathf.Abs(range1); //5
		range1 -= boundsSafeArea; //If boundsSafeArea were to be lets say 0.5, then range1 after this life would be 4.5

		float range2 = -range1;

		float x = Random.Range (range2, range1);


		float count = 0.0f;
		try {
			//print(spawnedShapesList.Count);
			for (int i = 0; i < spawnedShapesList.Count; i++) {
				count += spawnedShapesList [i].GetComponent<SpriteRenderer> ().bounds.size.x;
			}
		} catch (MissingReferenceException e) {
			
		}

		float yModifier = 0.0f;

		yModifier = camSize *2f + (count - (camSize * Camera.main.aspect));

		 
		//print ("Gravity Scale here is : " + gravScale);
		vector = new Vector2(x, gravScale * (camSize * (count % (camSize * Camera.main.aspect))));

        //print("Location to spawn is: " + vector.ToString());
        return vector;
    }
	
    Sprite SelectUntappableShape() //Returns a sprite in the shapeArray to be the untappable shape.
    {

        int selector = Random.Range(0, size);

        return shapeArray[selector];
    }

    Sprite ReturnShapeToSpawn() //Returns a sprite to spawn from the shapeArray.
    {
        int selector = Random.Range(0, size);

        return shapeArray[selector];
    }
    #endregion
}
