using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>Creates a hand-laid, editable town arena in the existing gameplay scene.</summary>
public static class ArenaMapBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string TileFolder = "Assets/Game/Tiles/Arena";
    private static readonly Dictionary<int, Sprite> Sprites = new Dictionary<int, Sprite>();
    private static Material material;
    private static Transform root;

    public static void Build()
    {
        EditorSceneManager.OpenScene(ScenePath);
        if (GameObject.Find("Town Arena") != null)
            throw new InvalidOperationException("Town Arena already exists; refusing to overwrite edits.");
        foreach (string path in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Game" }).Select(AssetDatabase.GUIDToAssetPath))
        {
            var match = System.Text.RegularExpressions.Regex.Match(path, @"Tileset---_(\d+)_");
            if (!match.Success) continue;
            var sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderByDescending(s => s.rect.width * s.rect.height).FirstOrDefault();
            if (sprite != null) Sprites[int.Parse(match.Groups[1].Value)] = sprite;
        }
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) throw new InvalidOperationException("Player is missing.");
        material = player.GetComponent<SpriteRenderer>().sharedMaterial;
        root = new GameObject("Town Arena").transform;
        var grid = new GameObject("Ground and Roads", typeof(Grid));
        grid.transform.SetParent(root, false);
        var ground = Map("Ground", grid.transform, -30);
        var roads = Map("Roads", grid.transform, -20);
        var sidewalks = Map("Sidewalks", grid.transform, -25);
        Directory.CreateDirectory(TileFolder);
        AssetDatabase.Refresh();
        var random = new System.Random(42);
        for (int x = -24; x < 24; x++)
        for (int y = -18; y < 18; y++)
        {
            var cell = new Vector3Int(x, y, 0);
            ground.SetTile(cell, Tile(77 + (random.Next(10) < 2 ? random.Next(1, 4) : 0)));
            if (IsRoad(x, y))
            {
                int id = 53;
                if (!IsRoad(x - 1, y)) id = 29;
                else if (!IsRoad(x + 1, y)) id = 39;
                else if (!IsRoad(x, y + 1)) id = 44;
                else if (!IsRoad(x, y - 1)) id = 46;
                // Crosswalks on the four approaches to the central intersection.
                if ((x == -5 || x == 4) && y >= -2 && y < 2) id = 48;
                if ((y == -5 || y == 4) && x >= -2 && x < 2) id = 51;
                roads.SetTile(cell, Tile(id));
            }
            else if (Mathf.Abs(x) < 21 && Mathf.Abs(y) < 15 &&
                     (IsRoad(x - 1, y) || IsRoad(x + 1, y) || IsRoad(x, y - 1) || IsRoad(x, y + 1)))
                sidewalks.SetTile(cell, Tile(163));
        }
        var buildings = Group("Buildings");
        Building(buildings, "Northwest - Workshop", -12, 5, 6, 4);
        Building(buildings, "Northeast - Store", 6, 5, 6, 4);
        Building(buildings, "Southwest - Shelter", -12, -9, 6, 4);
        Building(buildings, "Southeast - Garage", 6, -9, 6, 4);

        var props = Group("Street Props");
        Prop(props, "Red wreck west", 179, -11, 1.25f, true);
        Prop(props, "Pale wreck east", 183, 11, -1.35f, true);
        Prop(props, "Abandoned car north", 186, 1.6f, 8, true);
        Prop(props, "Burned car south", 182, -1.65f, -8, true);
        Prop(props, "West ring wreck", 184, -18, -6, true);
        Prop(props, "East ring wreck", 180, 18, 6, true);
        foreach (int x in new[] { -4, 4 })
        foreach (int y in new[] { -4, 4 })
        {
            Prop(props, "Corner traffic sign", x < 0 ? 155 : 157, x + .3f, y, false);
            Prop(props, "Corner bollard", 150, x, y + .9f, false);
        }
        foreach (int x in new[] { -14, 14 })
        foreach (int y in new[] { -8, 8 })
        {
            Prop(props, "Mailbox", 145, x, y, false);
            Prop(props, "Discarded tires", 192, x + .8f, y - .7f, false);
        }
        foreach (Vector2 p in new[] { new Vector2(-12, 2.8f), new Vector2(-10, 2.8f), new Vector2(10, -2.8f), new Vector2(12, -2.8f), new Vector2(2.7f, 9.5f), new Vector2(-2.7f, -9.5f) })
            Prop(props, "Traffic cone", 154, p.x, p.y, false);
        Prop(props, "North barricade", 151, -1.6f, 15.8f, true);
        Prop(props, "South barricade", 151, 1.6f, -15.8f, true);
        Prop(props, "West road sign", 152, -21, 2, false);
        Prop(props, "East road sign", 162, 21, -2, false);

        var perimeter = Group("Perimeter - Solid Boundaries");
        for (int x = -24; x < 24; x++)
        {
            Prop(perimeter, "North fence", 132, x + .5f, 17.5f, false);
            Prop(perimeter, "South fence", 132, x + .5f, -17.5f, false);
        }
        for (int y = -17; y < 17; y++)
        {
            Prop(perimeter, "West fence", 130, -23.5f, y + .5f, false);
            Prop(perimeter, "East fence", 130, 23.5f, y + .5f, false);
        }
        Wall(perimeter, "North Wall", new Vector2(0, 18), new Vector2(50, 2));
        Wall(perimeter, "South Wall", new Vector2(0, -18), new Vector2(50, 2));
        Wall(perimeter, "West Wall", new Vector2(-24, 0), new Vector2(2, 36));
        Wall(perimeter, "East Wall", new Vector2(24, 0), new Vector2(2, 36));
        var boundsObject = new GameObject("Camera Bounds (trigger)");
        boundsObject.transform.SetParent(root, false);
        var bounds = boundsObject.AddComponent<BoxCollider2D>();
        bounds.isTrigger = true;
        bounds.size = new Vector2(48, 36);
        player.transform.position = Vector3.zero;
        player.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var camera = Camera.main;
        camera.transform.position = new Vector3(0, 0, -10);
        camera.orthographicSize = 6;
        var follow = camera.GetComponent<CameraFollow>();
        if (follow == null) follow = camera.gameObject.AddComponent<CameraFollow>();
        var serialized = new SerializedObject(follow);
        serialized.FindProperty("target").objectReferenceValue = player.transform;
        serialized.FindProperty("cameraBounds").objectReferenceValue = bounds;
        serialized.FindProperty("smoothSpeed").floatValue = 7;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        Vector2[] spawns = { new Vector2(-21, 0), new Vector2(21, 0), new Vector2(0, 15), new Vector2(0, -15) };
        for (int i = 0; i < spawns.Length; i++)
            GameObject.Find("SpawnPoint_" + (i + 1)).transform.position = spawns[i];
        EditorSceneManager.SaveScene(camera.gameObject.scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("ARENA_BUILD_OK: 48x36 town, 4 buildings, cross and ring roads, decorated streets, 4 boundary walls, bounded player camera.");
    }

    private static bool IsRoad(int x, int y)
    {
        if (x < -23 || x >= 23 || y < -17 || y >= 17) return false;
        bool cross = (x >= -3 && x < 3) || (y >= -3 && y < 3);
        bool ring = x >= -20 && x < 20 && y >= -14 && y < 14 &&
                    (x < -16 || x >= 16 || y < -10 || y >= 10);
        return cross || ring;
    }

    private static Tilemap Map(string name, Transform parent, int order)
    {
        var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
        go.transform.SetParent(parent, false);
        var renderer = go.GetComponent<TilemapRenderer>();
        renderer.sortingOrder = order;
        renderer.sharedMaterial = material;
        return go.GetComponent<Tilemap>();
    }

    private static Tile Tile(int id)
    {
        string path = TileFolder + "/Arena_" + id + ".asset";
        var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile != null) return tile;
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = Sprites[id];
        // Normalize full tiles to one world unit without changing shared artwork imports.
        tile.transform = Matrix4x4.Scale(new Vector3(1f / tile.sprite.bounds.size.x, 1f / tile.sprite.bounds.size.y, 1));
        tile.colliderType = UnityEngine.Tilemaps.Tile.ColliderType.None;
        AssetDatabase.CreateAsset(tile, path);
        return tile;
    }

    private static Transform Group(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root, false);
        return go.transform;
    }

    private static void Building(Transform parent, string name, int x, int y, int width, int height)
    {
        var block = new GameObject(name);
        block.transform.SetParent(parent, false);
        for (int col = 0; col < width; col++)
        for (int row = 1; row < height; row++)
            Prop(block.transform, "Roof", col == 0 ? 92 : col == width - 1 ? 104 : 95, x + col + .5f, y + row + .5f, false, -5);
        for (int col = 0; col < width; col++)
        {
            bool door = col == width / 2;
            Prop(block.transform, door ? "Door" : "Facade", door ? 100 : col == 0 ? 91 : col == width - 1 ? 105 : 96, x + col + .5f, y + .5f, false, -5);
            if (!door)
            {
                var facade = block.transform.GetChild(block.transform.childCount - 1);
                facade.localScale = new Vector3(facade.localScale.x, facade.localScale.y * 16f / 9f, 1);
            }
        }
        var collider = block.AddComponent<BoxCollider2D>();
        collider.offset = new Vector2(x + width / 2f, y + height / 2f);
        collider.size = new Vector2(width, height);
        Prop(block.transform, "Entrance sign", 162, x + width / 2f + 1.15f, y - .45f, false);
    }

    private static void Prop(Transform parent, string name, int id, float x, float y, bool solid, int order = -3)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(x, y, 0);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprites[id];
        renderer.sharedMaterial = material;
        renderer.sortingOrder = order;
        float scale = renderer.sprite.pixelsPerUnit / 16f;
        go.transform.localScale = new Vector3(scale, scale, 1);
        if (solid)
        {
            var collider = go.AddComponent<BoxCollider2D>();
            collider.size = (Vector2)renderer.sprite.bounds.size * .8f;
        }
    }

    private static void Wall(Transform parent, string name, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.AddComponent<BoxCollider2D>().size = size;
    }
}
