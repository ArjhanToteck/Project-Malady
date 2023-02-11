using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapFiles : MonoBehaviour
{
    Transform sampleTiles;

    void Start()
    {
        sampleTiles = GameObject.Find("Tiles").transform;
    }

    public string MapToJSON(GameObject map)
    {
        MapData mapData = new MapData();
        Transform floor = map.transform.Find("Floor");
        Transform walls = map.transform.Find("Walls");
        Transform objects = map.transform.Find("Objects");

        // spawn point
        Vector3 spawnPoint = map.transform.Find("SpawnPoint").position;
        mapData.spawnPoint = new float[] { spawnPoint.x, spawnPoint.y };

        // floor
        List<MapObject> floorList = new List<MapObject>();

        // loops through floor and adds data to JSON
        for (var i = 0; i < floor.childCount; i++)
        {
            Transform currentFloor = floor.GetChild(i);
            floorList.Add(new MapObject(currentFloor.position.x, currentFloor.position.y, currentFloor.transform.rotation.eulerAngles.z, currentFloor.GetComponent<SpriteRenderer>().sortingOrder, currentFloor.name));
        }

        mapData.floor = floorList.ToArray();

        // walls
        Walls wallsList = new Walls();

        // loops wall directions and adds data to JSON
        for (var i = 0; i < walls.childCount; i++)
        {
            Transform currentDirection = walls.GetChild(i);
            List<MapObject> currentWallsList = new List<MapObject>();

            // loops through walls in direction and adds data to JSON
            for (var j = 0; j < currentDirection.childCount; j++)
            {
                Transform currentWall = currentDirection.GetChild(j);
                currentWallsList.Add(new MapObject(currentWall.position.x, currentWall.position.y, currentWall.transform.rotation.eulerAngles.z, currentWall.GetComponent<SpriteRenderer>().sortingOrder, currentWall.name));
            }

            // gets property of wallsList corresponding to currentDirection and sets it to currentWallsList, the list of actual walls, to lowercase
            wallsList.GetType().GetField(currentDirection.name.ToLower()).SetValue(wallsList, currentWallsList.ToArray());
        }

        mapData.walls = wallsList;

        // objects
        List<MapObject> objectsList = new List<MapObject>();

        // loops through objects and adds data to JSON
        for (var i = 0; i < objects.childCount; i++)
        {
            Transform currentObject = objects.GetChild(i);
            objectsList.Add(new MapObject(currentObject.position.x, currentObject.position.y, currentObject.transform.rotation.eulerAngles.z, currentObject.GetComponent<SpriteRenderer>().sortingOrder, currentObject.name));
        }

        mapData.objects = objectsList.ToArray();

        // returns JSON
        return JsonUtility.ToJson(mapData);
    }

    public GameObject JSONToMap(string json)
    {
        MapData mapData = JsonUtility.FromJson<MapData>(json);

        GameObject map = new GameObject();
        map.name = "Map";

        // spawn point

        // creates new spawnPoint object and moves it to the position indicated by mapData
        GameObject spawnPoint = new GameObject();
        spawnPoint.transform.position = new Vector3(mapData.spawnPoint[0], mapData.spawnPoint[0]);

        FindObjectOfType<GameManager>().spawnPoint = spawnPoint.transform.position;

        // floor

        // creates floor parent
        GameObject floorParent = new GameObject();
        floorParent.name = "Floor";
        floorParent.transform.SetParent(map.transform);

        for (var i = 0; i < mapData.floor.Length; i++)
        {
            // gets data for current tile
            MapObject tileData = mapData.floor[i];

            // creates current tile from sample
            GameObject currentTile = Instantiate(sampleTiles.Find(tileData.name).gameObject);

            // sets position, layer, and rotation of current tile
            currentTile.transform.position = new Vector3(tileData.x, tileData.y, 0);
            currentTile.transform.rotation = Quaternion.Euler(Vector3.forward * tileData.rotation);
            currentTile.GetComponent<SpriteRenderer>().sortingOrder = tileData.layer;
            
            // sets name and parent
            currentTile.name = tileData.name;
            currentTile.transform.SetParent(floorParent.transform);
            currentTile.SetActive(true);
        }

        // walls

        // creates walls parent
        GameObject wallsParent = new GameObject();
        wallsParent.name = "Walls";
        wallsParent.transform.SetParent(map.transform);

        // loops four times for each direction of walls
        for (var i = 0; i < 4; i++)
        {
            string[] directions = {"Front", "Back", "Left", "Right"};

            // gets mapData.walls[direction]
            MapObject[] currentWalls = (MapObject[])mapData.walls.GetType().GetField(directions[i].ToLower()).GetValue(mapData.walls);

            // creates direction parent
            GameObject directionParent = new GameObject();
            directionParent.name = directions[i];
            directionParent.transform.SetParent(wallsParent.transform);

            for (var j = 0; j < currentWalls.Length; j++)
            {
                // gets data for current tile
                MapObject tileData = currentWalls[j];

                // creates current tile from sample
                GameObject currentTile = Instantiate(sampleTiles.Find(tileData.name).gameObject);

                // sets position, layer, and rotation of current tile
                currentTile.transform.position = new Vector3(tileData.x, tileData.y, 0);
                currentTile.transform.rotation = Quaternion.Euler(Vector3.forward * tileData.rotation);
                currentTile.GetComponent<SpriteRenderer>().sortingOrder = tileData.layer;

                // sets name and parent
                currentTile.name = tileData.name;
                currentTile.transform.SetParent(directionParent.transform);
                currentTile.SetActive(true);
            }
        }

        // objects

        // creates objects parent
        GameObject objectsParent = new GameObject();
        objectsParent.name = "Objects";
        objectsParent.transform.SetParent(map.transform);

        for (var i = 0; i < mapData.objects.Length; i++)
        {
            // gets data for current tile
            MapObject tileData = mapData.objects[i];

            // creates current tile from sample
            GameObject currentTile = Instantiate(sampleTiles.Find(tileData.name).gameObject);

            // sets position, layer, and rotation of current tile
            currentTile.transform.position = new Vector3(tileData.x, tileData.y, 0);
            currentTile.transform.rotation = Quaternion.Euler(Vector3.forward * tileData.rotation);
            currentTile.GetComponent<SpriteRenderer>().sortingOrder = tileData.layer;

            // sets name and parent
            currentTile.name = tileData.name;
            currentTile.transform.SetParent(objectsParent.transform);
            currentTile.SetActive(true);
        }

        // returns created map
        return map;
    }

    [System.Serializable]
    class MapObject
    {
        public float x;
        public float y;
        public float rotation;
        public int layer;
        public string name;

        public MapObject(float xInput, float yInput, float rotationInput, int layerInput, string nameInput)
        {
            x = xInput;
            y = yInput;
            rotation = rotationInput;
            layer = layerInput;
            name = nameInput;
        }
    }

    [System.Serializable]
    class Walls
    {
        public MapObject[] front;
        public MapObject[] back;
        public MapObject[] left;
        public MapObject[] right;
    }

    [System.Serializable]
    class MapData
    {
        public MapObject[] floor;
        public Walls walls;
        public MapObject[] objects;
        public float[] spawnPoint;
    }

    public static void SaveMap(string json)
    {
        string path = "C:/Users/Paolo Hidalgo/Downloads/map.json";
        File.WriteAllText(path, json);
    }
}
