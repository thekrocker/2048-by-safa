using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [Header("Board Size")] [SerializeField]
    private int width;

    [SerializeField] private int height;
    [SerializeField] private float margin;
    [SerializeField] private float boardFrameOffset = .2f;


    [Header("References")] [SerializeField]
    private TileNode tilePrefab;

    [SerializeField] private GamePieceNode gamePiecePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private Transform pieceParent;
    [SerializeField] private SpriteRenderer boardFrameRenderer;

    [Header("Node Data")] [SerializeField] private GamePieceNodeData nodeDataContainer;

    private TileNode[,] _allTiles;
    private GamePieceNode[,] _allPieces;

    private void Awake()
    {
        _allTiles = new TileNode[width, height];
        _allPieces = new GamePieceNode[width, height];
    }

    private void Start()
    {
        SpawnTileNodes();
        SpawnGamePieceNode(2, nodeDataContainer.GetNodeDataByValue(2));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Vector2.down);
    }

    private void SpawnGamePieceNode(int count, NodeData nodeDataValue)
    {
        var availableTiles = GetRandomAvailableTiles(count);

        foreach (TileNode availableTile in availableTiles)
        {
            Vector2 position = new Vector2(availableTile.X, availableTile.Y);
            GamePieceNode spawnedNode = Instantiate(gamePiecePrefab, position, Quaternion.identity);
            spawnedNode.Initialize(availableTile.X, availableTile.Y);
            _allPieces[availableTile.X, availableTile.Y] = spawnedNode;
            spawnedNode.SetGamePieceNode(nodeDataValue);
            spawnedNode.SetTileNode(availableTile);
        }
    }

    public bool IsWithinBounds(int x, int y) => (x >= 0 && x < width && y >= 0 && y < height);

    private void MoveTo(Vector2 direction)
    {
        List<GamePieceNode> nodes = new List<GamePieceNode>();
        
        foreach (GamePieceNode piece in _allPieces)
        {
            if (piece == null) continue;
            nodes.Add(piece);
        }

        nodes = nodes.OrderBy(x => x.Position.x).ThenBy(x => x.Position.y).ToList();
        
        if (direction == Vector2.right || direction == Vector2.up) nodes.Reverse();

        foreach (GamePieceNode node in nodes)
        {
            int nextX = node.X + (int)direction.x;
            int nextY = node.Y + (int)direction.y;

            if (!IsWithinBounds(nextX, nextY))
            {
                Debug.Log("Is not within bounds");
                continue;
            }

            if (_allPieces[nextX, nextY] != null) continue;

            _allPieces[node.X, node.Y] = null;
            
            TileNode nextTile = _allTiles[nextX, nextY];
            
            node.SetCoords(nextX, nextY);

            _allPieces[nextX, nextY] = node;
            
            node.Move(nextTile.Position);

            // Clear its own grid
        }
    }


    private List<TileNode> GetRandomAvailableTiles(int count)
    {
        List<TileNode> availableNodes = new List<TileNode>();

        foreach (TileNode tile in _allTiles)
        {
            if (_allPieces[tile.X, tile.Y] == null) availableNodes.Add(tile);
        }

        return availableNodes.OrderBy(_ => Random.value).Take(count).ToList();
        // Check for tiles that has no game pieces on it & randomize them & get tiles with X count
    }

    private void SpawnTileNodes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y);
                TileNode spawnedNode = Instantiate(tilePrefab, position, Quaternion.identity);
                spawnedNode.transform.SetParent(tileParent);
                spawnedNode.Initialize(x, y);
                _allTiles[x, y] = spawnedNode;
            }
        }

        // Set Camera by width / height offset. We add also small ofset because our pivot is middle of the object &, so half scale.
        var pos = new Vector3((width / 2f) - .5f, (height / 2f) - .5f, -10f);
        Camera.main.transform.position = pos;

        // Set frame height & width
        boardFrameRenderer.size = new Vector2(width + boardFrameOffset, height + boardFrameOffset);
        boardFrameRenderer.transform.position = new Vector3(pos.x, pos.y, 0f);
    }
}