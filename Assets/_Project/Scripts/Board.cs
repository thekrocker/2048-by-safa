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

    public Action OnBoardMoveCompleted;

    private void Awake()
    {
        _allTiles = new TileNode[width, height];
        _allPieces = new GamePieceNode[width, height];
    }

    private void Start()
    {
        SpawnInitialElements();
    }

    private void SpawnInitialElements()
    {
        SpawnTileNodes();
        SpawnGamePieceNode(2, nodeDataContainer.GetNodeDataByValue(2));
    }

    public void TryGetMoveInput()
    {
        SwipeManager swipeManager = SwipeManager.Instance;
        
        if (swipeManager.SwipeLeft) MoveTo(Vector2Int.left);
        if (swipeManager.SwipeRight) MoveTo(Vector2Int.right);
        if (swipeManager.SwipeUp)  MoveTo(Vector2Int.up);
        if (swipeManager.SwipeDown) MoveTo(Vector2Int.down);
    }

    private Sequence SpawnGamePieceNode(int count, NodeData nodeDataValue)
    {
        Sequence seq = DOTween.Sequence();
        var availableTiles = GetRandomAvailableTiles(count);

        foreach (TileNode availableTile in availableTiles)
        {
            Vector2 position = new Vector2(availableTile.X, availableTile.Y);
            GamePieceNode spawnedNode = Instantiate(gamePiecePrefab, position, Quaternion.identity);
            spawnedNode.Initialize(availableTile.X, availableTile.Y);
            _allPieces[availableTile.X, availableTile.Y] = spawnedNode;
            spawnedNode.SetGamePieceNode(nodeDataValue);

            // Scale up
            seq.Join(spawnedNode.ScaleUp());
        }

        return seq;
    }

    public Sequence SpawnAfterBoardTurn()
    {
        return SpawnGamePieceNode(1, nodeDataContainer.GetNodeDataByValue(2));
    }

    public bool IsWithinBounds(int x, int y) => (x >= 0 && x < width && y >= 0 && y < height);

    public void MoveTo(Vector2Int direction)
    {
        List<GamePieceNode> nodes = new List<GamePieceNode>();
        int moveCount = 0;

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
                        moveCount++;

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

                            possibleNextPiece.transform.DOKill();
                            Destroy(possibleNextPiece.gameObject);
                            node.SetGamePieceNode(nodeDataContainer.GetNodeDataByValue(node.Value * 2));
                            moveCount++;
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

            node.Move(new Vector2(nextX, nextY));

            // Clear its own grid
        }
        
        if (moveCount > 0)
        {
            OnBoardMoveCompleted?.Invoke();
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