using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

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
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Vector2Int.down);
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
        }
    }

    public bool IsWithinBounds(int x, int y) => (x >= 0 && x < width && y >= 0 && y < height);

    private void MoveTo(Vector2Int direction)
    {
        List<GamePieceNode> nodes = new List<GamePieceNode>();
        Sequence seq = DOTween.Sequence();

        foreach (GamePieceNode piece in _allPieces)
        {
            if (piece == null) continue;
            nodes.Add(piece);
        }

        nodes = nodes.OrderBy(x => x.Position.x).ThenBy(x => x.Position.y).ToList();

        if (direction == Vector2.right || direction == Vector2.up) nodes.Reverse();

        for (int i = 0; i < nodes.Count; i++)
        {
            GamePieceNode node = nodes[i];
            int nextX = node.X;
            int nextY = node.Y;

            while (true)
            {
                int possibleNextX = node.X + direction.x;
                int possibleNextY = node.Y + direction.y;

                if (IsWithinBounds(possibleNextX, possibleNextY))
                {
                    GamePieceNode possibleNextPiece = _allPieces[possibleNextX, possibleNextY];

                    if (possibleNextPiece == null)
                    {
                        // Clean own grid & set next one & set coords.

                        _allPieces[node.X, node.Y] = null;
                        nextX = possibleNextX;
                        nextY = possibleNextY;
                        _allPieces[nextX, nextY] = node;
                        node.SetCoords(nextX, nextY);

                        // Next tile is null. Just move there.
                    }
                    else
                    {
                        // There is an object on the next tile.. Check for merging? 
                        if (node.AvailableForMatch(possibleNextPiece.Value))
                        {
                            // Clean own grid & set next one & set coords.
                            _allPieces[node.X, node.Y] = null;
                            nextX = possibleNextX;
                            nextY = possibleNextY;
                            _allPieces[nextX, nextY] = node;
                            node.SetCoords(nextX, nextY);
                            nodes.Remove(possibleNextPiece);
                            Destroy(possibleNextPiece.gameObject);
                            node.SetGamePieceNode(nodeDataContainer.GetNodeDataByValue(node.Value * 2));
                        }
                        else
                        {
                            Debug.Log("Next piece is not matchable.. Stop there..");
                        }

                        break;
                    }
                }
                else
                {
                    // Arrived at the end.. Break the loop.
                    break;
                }
            }

            seq.Join(node.Move(new Vector2(nextX, nextY)));
            
            // Clear its own grid
        }

        seq.OnComplete(() => SpawnGamePieceNode(1, nodeDataContainer.GetNodeDataByValue(2)));
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