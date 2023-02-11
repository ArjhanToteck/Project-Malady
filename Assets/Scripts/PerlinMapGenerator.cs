using UnityEngine;

public class PerlinMapGenerator : MonoBehaviour
{
	public Vector2 seed;
	public int width = 100;
	public int height = 100;
	public float sensitivity = 0.35f;
	public float scale = 0.1f;

	GameObject floor;
	GameObject wallLeft;
	GameObject wallRight;
	GameObject wallFront;
	GameObject wallBack;
	Transform map;

	void Start()
    {
		// gets tiles
		Transform sampleTiles = GameObject.Find("Tiles").transform;
		floor = sampleTiles.Find("Floor").gameObject;
		wallLeft = sampleTiles.Find("WallLeft").gameObject;
		wallRight = sampleTiles.Find("WallRight").gameObject;
		wallFront = sampleTiles.Find("WallFront").gameObject;
		wallBack = sampleTiles.Find("WallBack").gameObject;

		// creates map game object for later deletion
		map = new GameObject().transform;

		// random seed
		seed = new Vector2(Random.Range(-10000, 10000f), Random.Range(-10000f, 10000f));

		GenerateMap();
	}

    void Update()
    {
		if (Input.GetKey("1"))
		{
			GenerateMap();
		}
	}

    public void GenerateMap()
    {
		// deletes previous map
		Destroy(map.gameObject);

		// resets map and children
		map = new GameObject().transform;
		map.gameObject.name = "Map";

		// floor
		Instantiate(map, map).name = "Floor";

		// walls
		Transform walls = Instantiate(map, map);
		walls.name = "Walls";

		Instantiate(walls, walls).name = "Front";
		Instantiate(walls, walls).name = "Left";
		Instantiate(walls, walls).name = "Back";
		Instantiate(walls, walls).name = "Right";

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// checks if tile matches perlin noise sensitivity
				if (Mathf.PerlinNoise(seed.x + x * scale, seed.y + y * scale) >= sensitivity)
				{
					// places floor tile
					GameObject currentTile = Instantiate(floor, map.Find("Floor"));
					currentTile.transform.position = new Vector3(x, y, 0);
					currentTile.SetActive(true);

					// checks if on left border
					if (x == 0)
                    {
                        // places wallLeft tile
                        GameObject currentWall = Instantiate(wallLeft, map.Find("Walls").Find("Left"));
                        currentWall.transform.position = new Vector3(x - 1, y, 0);
                        currentWall.SetActive(true);
                    }
                    else if (x == width - 1) // checks if on right border
                    {
                        // places wallRight tile
                        GameObject currentWall = Instantiate(wallRight, map.Find("Walls").Find("Right"));
                        currentWall.transform.position = new Vector3(x + 1, y, 0);
                        currentWall.SetActive(true);
                    }

					// checks if on bottom border
					if (y == 0)
                    {
						// places wallBack tile
						GameObject currentWall = Instantiate(wallBack, map.Find("Walls").Find("Back"));
						currentWall.transform.position = new Vector3(x, y, 0);
						currentWall.SetActive(true);
					} else if(y == height - 1) // if on top border
                    {
						// places wallFront tile
						GameObject currentWall = Instantiate(wallFront, map.Find("Walls").Find("Front"));
						currentWall.transform.position = new Vector3(x, y, 0);
						currentWall.SetActive(true);
					}
				}
                else
                {
					// checks if tile above matches sensitivity  and is on map
					if (Mathf.PerlinNoise(seed.x + x * scale, seed.y + (y + 1) * scale) >= sensitivity && y + 1 < height)
					{
						// places wallBack tile
						GameObject currentTile = Instantiate(wallBack, map.Find("Walls").Find("Back"));
						currentTile.transform.position = new Vector3(x, y + 1, 0);
						currentTile.SetActive(true);
					}

					// checks if tile below matches sensitivity and is on map
					if (Mathf.PerlinNoise(seed.x + x * scale, seed.y + (y - 1) * scale) >= sensitivity && y - 1 >= 0)
					{
						// places wallFront tile
						GameObject currentTile = Instantiate(wallFront, map.Find("Walls").Find("Front"));
						currentTile.transform.position = new Vector3(x, y, 0);
						currentTile.SetActive(true);

						// checks if tile below and to right doesn't match sensitivity
						if (Mathf.PerlinNoise(seed.x + (x + 1) * scale, seed.y + (y - 1) * scale) < sensitivity || x + 1 >= width)
						{
							// places wallRight tile
							GameObject currentWall = Instantiate(wallRight, map.Find("Walls").Find("Right"));
							currentWall.transform.position = new Vector3(x + 1, y, 0);
							currentWall.SetActive(true);
						}

						// checks if tile below and to left doesn't match sensitivity
						if (Mathf.PerlinNoise(seed.x + (x - 1) * scale, seed.y + (y - 1) * scale) < sensitivity || x - 1 < 0)
						{
							// places wallLeft tile
							GameObject currentWall = Instantiate(wallLeft, map.Find("Walls").Find("Left"));
							currentWall.transform.position = new Vector3(x - 1, y, 0);
							currentWall.SetActive(true);
						}
					} else // only places side walls if front wall is not present
                    {
						// checks if tile to right matches sensitivity and if the tile to right is on map
						if (Mathf.PerlinNoise(seed.x + (x + 1) * scale, seed.y + y * scale) >= sensitivity && x + 1 < width)
						{
							// places wallLeft tile
							GameObject currentTile = Instantiate(wallLeft, map.Find("Walls").Find("Left"));
							currentTile.transform.position = new Vector3(x, y, 0);
							currentTile.SetActive(true);
						}

						// checks if tile to left matches sensitivity and if the tile to left is on map
						if (Mathf.PerlinNoise(seed.x + (x - 1) * scale, seed.y + y * scale) >= sensitivity && x - 1 >= 0)
						{
							// places wallRight tile
							GameObject currentTile = Instantiate(wallRight, map.Find("Walls").Find("Right"));
							currentTile.transform.position = new Vector3(x, y, 0);
							currentTile.SetActive(true);
						}
					}
				}
			}
		}
	}
}
