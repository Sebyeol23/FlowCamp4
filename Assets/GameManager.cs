using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using WebSocketSharp;
using Newtonsoft.Json;


public class Type
{
    public string type { get; set; }
}

public class Create
{
    public string type { get; set; }
}

public class Join
{
    public string type { get; set; }
    public string roomId { get; set; }
}

public class Player{
    public string type { get; set; }
    public string roomId { get; set; }
    public string color { get; set; }
    public string currentPlayer { get; set; }
}

public class SwitchTurn{
    public string type { get; set; }
    public string roomId { get; set; }
}

public class CurrentPlayer{
    public string type { get; set; }
    public string currentPlayer { get; set; }
}

public class SetStone{
    public string type { get; set; }
    public string roomId { get; set; }
    public string color { get; set; }
    public int roundX { get; set; }
    public int roundY { get; set; }
}

public class Select{
    public string type { get; set; }
    public string roomId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
}


public class ShootStone{
    public string type { get; set; }
    public string roomId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // 싱글톤 인스턴스
    public static string currentPlayer; // 현재 플레이어를 나타내는 문자열
    public static string myColor;
    public static string roomId;

    public bool set = false;
    public int X;
    public int Y;

    public Text victoryText; 

    private static string serverAddress = "wss://port-0-gomoku-server-cu6q2blkgm7b0f.sel4.cloudtype.app";
    public static WebSocket webSocket = new WebSocket(serverAddress);

    [SerializeField]
    private Button createButton;
    [SerializeField]
    private Button joinButton;
    [SerializeField]
    private TMPro.TMP_InputField joinInputField;

    private bool isWebSocketConnecting = false;
    private int maxReconnectAttempts = 5; // 최대 재연결 시도 횟수
    private int reconnectAttempts = 0; // 현재 재연결 시도 횟수

    private IEnumerator ReconnectWebSocket()
    {
        // 재연결 시도 횟수 제한
        if (reconnectAttempts >= maxReconnectAttempts)
        {
            Debug.LogError("WebSocket reconnect attempts reached the limit. Cannot reconnect.");
            yield break;
        }

        isWebSocketConnecting = true;
        yield return new WaitForSeconds(2f); // 재연결 시도 간격 (2초)

        // 웹소켓 연결이 끊겼을 경우 다시 연결
        if (webSocket != null && !webSocket.IsAlive)
        {
            webSocket.Connect();
            reconnectAttempts++;
        }

        isWebSocketConnecting = false;
    }

    private void Start()
    {
        currentPlayer = null;
        myColor = null;
        roomId = null;

        // 연결 및 메시지 수신 이벤트 핸들러 등록
        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnWebSocketMessage;
        webSocket.OnError += OnWebSocketError;
        webSocket.OnClose += OnWebSocketClose;

        // 웹소켓 연결 시작
        webSocket.Connect();

        createButton.onClick.AddListener(createOnClick);
        joinButton.onClick.AddListener(joinOnClick);
    }

    private void createOnClick()
    {
        Create create = new Create
        {
            type = "create_room",
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(create));
    }

    private void joinOnClick()
    {
        Join join = new Join
        {
            type = "join_room",
            roomId = joinInputField.text,
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(join));
    }

    private void OnDestroy()
    {
        // 스크립트가 제거될 때 웹소켓 연결 닫기
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }

    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket is open!");
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"Received message: {e.Data}");
        Type type = JsonConvert.DeserializeObject<Type>(e.Data);
        switch (type.type)
        {
            case "game_start":
            Player player = JsonConvert.DeserializeObject<Player>(e.Data);
            roomId = player.roomId;
            Debug.Log(roomId);
            myColor = player.color;
            currentPlayer = player.currentPlayer;
            break;

            case "turn_switch":
            CurrentPlayer current = JsonConvert.DeserializeObject<CurrentPlayer>(e.Data);
            currentPlayer = current.currentPlayer;
            Debug.Log(currentPlayer);
            break;

            case "stone_set":
            SetStone setStone = JsonConvert.DeserializeObject<SetStone>(e.Data);
            X = setStone.roundX;
            Y = setStone.roundY;
            set = true;
            break;

            case "stone_select":
            Select select = JsonConvert.DeserializeObject<Select>(e.Data);
            SelectStone.X = select.X;
            SelectStone.Y = select.Y;
            SelectStone.set = true;
            break;

            case "stone_shoot":
            ShootStone shootStone = JsonConvert.DeserializeObject<ShootStone>(e.Data);
            ThrowStone.X = shootStone.X;
            ThrowStone.Y = shootStone.Y;
            ThrowStone.set = true;
            break;
        }
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError($"WebSocket error: {e.Message}");
    }

    private void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket is closed!");
    }

    // 메시지 보내는 함수
    public static void SendWebSocketMessage(string message)
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Send(message);
        }
    }

    private void Awake()
    {
        if (instance == null){
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SwitchPlayer() // 플레이어 교체 함수
    {
        SwitchTurn switchTurn = new SwitchTurn
        {
            type = "switch_turn",
            roomId = roomId
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(switchTurn));
    }

    public void SetStone(string color, int roundX, int roundY){
        SetStone setStone = new SetStone{
            type = "set_stone",
            roomId = roomId,
            color = color,
            roundX = roundX,
            roundY = roundY
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(setStone));
    }

    public static void Select(float X, float Y){
        Select select = new Select{
            type = "select_stone",
            roomId = GameManager.roomId,
            X = X,
            Y = Y
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(select));
    }

    public static void ShootStone(float X, float Y){
        ShootStone shootStone = new ShootStone{
            type = "shoot_stone",
            roomId = GameManager.roomId,
            X = X,
            Y = Y
        };
        SendWebSocketMessage(JsonConvert.SerializeObject(shootStone));
    }
    
    public bool checkStonesMoving = false;
    void CheckStonesMoving()
    {
        checkStonesMoving = true;
    }

    public GameObject whiteStonePrefab;  // White Stone Prefab을 가리킬 변수. Unity에서 설정.
    public GameObject blackStonePrefab;  // Black Stone Prefab을 가리킬 변수. Unity에서 설정.
    public GameObject positionPrefab;
    public AudioSource positionsound; 
    public int placementTurns = 0;  // 배치된 돌의 수를 세기 위한 변수

    public List<GameObject> allStones = new List<GameObject>();
    
    private bool CanPlaceStone(int x, int y)
    {
        foreach(GameObject stone in allStones)
        {
            if(Mathf.RoundToInt(stone.transform.position.x) == x && Mathf.RoundToInt(stone.transform.position.y) == y)
            {
                return false;  // 돌이 이미 있는 위치에는 놓을 수 없습니다.
            }
        }
        return true;  // 돌이 없는 위치입니다.
    }
    
    public int GetPlacementTurns()
    {
        return placementTurns;
    }

    public bool CheckForVictory(string playerColor)
    {
        int consecutiveCount;
        Vector2[] directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.one, Vector2.right - Vector2.up };

        foreach (GameObject stone in allStones)
        {
            if (stone.tag == playerColor)
            {
                foreach (Vector2 direction in directions)
                {
                    consecutiveCount = 1;

                    for (int i = 1; i < 5; i++)
                    {
                        Vector2 checkPos = (Vector2)stone.transform.position + 2*direction * i;

                        if (allStones.Exists(otherStone =>
                            otherStone.transform.position == (Vector3)checkPos && otherStone.tag == playerColor))
                        {
                            consecutiveCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (consecutiveCount == 5)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void GameEndCheck()
    {
        bool blackExist = false, whiteExist = false;
        
        if ((placementTurns / 10)%2 != 0)
        {
            foreach (GameObject stone in allStones)
            {
                // 검은돌과 흰돌이 필드 내에 존재하는지 체크
                if ((stone.transform.position.x >= -9 && stone.transform.position.x <= 9) && 
                    (stone.transform.position.y >= -9 && stone.transform.position.y <= 9))
                {
                    if (stone.tag == "Black" && stone.activeInHierarchy)
                        blackExist = true;
                    else if (stone.tag == "White" && stone.activeInHierarchy)
                        whiteExist = true;
                }

                // 둘 다 존재하면 더 이상 체크할 필요 없으므로 break
                if (blackExist && whiteExist)
                    break;
            }

            // 검은돌과 흰돌의 존재 유무에 따라 승패 결정
            if (blackExist && !whiteExist)
            {
                Debug.Log("Black Wins!");
                // 게임 종료 처리 코드를 여기에 작성...
                victoryText.text = "Black Wins!"; 
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = Color.black;
                victoryText.gameObject.SetActive(true);
            }
            else if (!blackExist && whiteExist)
            {
                Debug.Log("White Wins!");
                // 게임 종료 처리 코드를 여기에 작성...
                victoryText.text = "White Wins!"; 
                victoryText.color = Color.black;
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = Color.white;
                victoryText.gameObject.SetActive(true);
            }
            else if (!blackExist && !whiteExist)
            {
                Debug.Log("Tie!");
                // 게임 종료 처리 코드를 여기에 작성...
                victoryText.text = "Tie!"; 
                victoryText.color = Color.red;
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = Color.grey;
                victoryText.gameObject.SetActive(true);
            }            
        }
        else
        {
            if (CheckForVictory("Black"))
            {
                Debug.Log("Black Wins!");
                // 게임 종료 처리 코드를 여기에 작성...
                victoryText.text = "Black Wins!"; 
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = Color.black;
                victoryText.gameObject.SetActive(true);
            }
            else if (CheckForVictory("White"))
            {
                Debug.Log("White Wins!");
                // 게임 종료 처리 코드를 여기에 작성...
                victoryText.text = "White Wins!"; 
                victoryText.color = Color.black;
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = Color.white;
                victoryText.gameObject.SetActive(true);
            }
        }
    }
    

    public void PlaceStone()
    {
        // 10번 돌을 배치하고 나면, 그만 배치하도록 설정
        if ((placementTurns/10)%2 != 0)
        {
            GameEndCheck();
            return;
        }
        else{
            if (placementTurns % 10 == 0){
                Debug.Log("OMOCK Turn");
            }
            
            if(myColor == currentPlayer){

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                int roundX = Mathf.RoundToInt(mousePos.x);
                if (roundX % 2 == 0) // If the number is even
                {
                    roundX += (mousePos.x > roundX) ? 1 : -1; // Check the direction of rounding
                }
                
                int roundY = Mathf.RoundToInt(mousePos.y);
                if (roundY % 2 == 0) // If the number is even
                {
                    roundY += (mousePos.y > roundY) ? 1 : -1; // Check the direction of rounding
                }
                // 범위를 -9부터 9까지로 제한
                roundX = Mathf.Clamp(roundX, -9, 9);
                roundY = Mathf.Clamp(roundY, -9, 9);

                if (CanPlaceStone(roundX, roundY))
                {
                    SetStone(myColor, roundX, roundY);
                    
                    GameObject spark = Instantiate(positionPrefab, new Vector3(roundX, roundY, 0), Quaternion.identity);
                    spark.SetActive(true);
                    
                    Destroy(spark, 1f); 
                    
                    
                }
                else{
                    Debug.Log("Please allocate another point");
                }
                Vector3 roundedPos = new Vector3(roundX, roundY, 0);
            }
        }
    }

    public void StoneSet(int roundX, int roundY){
        Vector3 stonePosition = new Vector3(roundX, roundY, 0);
        GameObject newStone;
        if (currentPlayer == "Black") {
            newStone = Instantiate(blackStonePrefab, stonePosition, Quaternion.identity);
        } else {
            newStone = Instantiate(whiteStonePrefab, stonePosition, Quaternion.identity);
        }
        newStone.tag = currentPlayer;  // 현재 플레이어의 색으로 태그를 설정
        allStones.Add(newStone);

        // 컬러 세팅
        newStone.GetComponent<SpriteRenderer>().color = currentPlayer == "Black" ? Color.black : Color.white;
        newStone.SetActive(true);
        positionsound.Play();
        
        if ((placementTurns%10 == 9)||(placementTurns%10 == 0)){
            Camera mainCamera = Camera.main;
            mainCamera.backgroundColor = new Color(Random.value, Random.value, Random.value);
        }
        placementTurns++;

        

        GameEndCheck();

        set = false;

        currentPlayer = currentPlayer == "Black" ? "White" : "Black";
    }
    
    public void HandleOverlappingStones()
    {
        // 새로운 위치에서 돌이 겹치는 경우 처리합니다.
        // 각 위치에 있는 돌의 목록을 추적하는 dictionary를 생성합니다.
        Dictionary<Vector2, List<GameObject>> stonePositions = new Dictionary<Vector2, List<GameObject>>();

        // 각 돌의 원래 위치를 저장하는 dictionary를 생성합니다.
        Dictionary<GameObject, Vector2> originalPositions = new Dictionary<GameObject, Vector2>();

        foreach (GameObject stone in allStones)
        {
            Vector2 position = new Vector2(stone.transform.position.x, stone.transform.position.y);
            if (!stonePositions.ContainsKey(position))
            {
                stonePositions[position] = new List<GameObject>();
            }
            stonePositions[position].Add(stone);

            // 각 돌의 원래 위치를 저장합니다.
            originalPositions[stone] = position;
        }

        // 각 위치에서, 만약 돌이 둘 이상 있다면, 원래 위치와 가장 가까운 돌만 선택하고 나머지는 제거합니다.
        foreach (KeyValuePair<Vector2, List<GameObject>> entry in stonePositions)
        {
            if (entry.Value.Count > 1)
            {
                GameObject stoneToKeep = null;
                float shortestDistance = float.MaxValue;
                foreach (GameObject stone in entry.Value)
                {
                    float distance = Vector2.Distance(originalPositions[stone], entry.Key);
                    if (distance < shortestDistance)
                    {
                        stoneToKeep = stone;
                        shortestDistance = distance;
                    }
                }
                foreach (GameObject stone in entry.Value)
                {
                    if (stone != stoneToKeep)
                    {
                        allStones.Remove(stone);
                        Destroy(stone);
                    }
                }
            }
        }
    }

    public void MoveStonesToNearestOddCoordinates()
    {
        // 일단 모든 돌들을 돌며 그 위치를 가장 가까운 홀수 정수 좌표로 이동시킵니다.
        foreach (GameObject stone in allStones)
        {
            Vector3 position = stone.transform.position;
            int roundX_move = Mathf.RoundToInt(position.x);
            int roundY_move = Mathf.RoundToInt(position.y);

            if (roundX_move % 2 == 0)
            {
                roundX_move += (position.x > roundX_move) ? 1 : -1; // Check the direction of rounding
            }
            if (roundY_move % 2 == 0)
            {
                roundY_move += (position.y > roundY_move) ? 1 : -1; // Check the direction of rounding
            }
            stone.transform.position = new Vector3(roundX_move, roundY_move, 0);
        }

        HandleOverlappingStones();
    }


    void Update()
    {
        if (!isWebSocketConnecting && webSocket != null && !webSocket.IsAlive)
        {
            StartCoroutine(ReconnectWebSocket());
        }
        if(currentPlayer == null){
            return;
        }
        if (Input.GetMouseButtonDown(0) && placementTurns%20==0 && !EventSystem.current.IsPointerOverGameObject())
        {
            if (checkStonesMoving){
                bool allStonesStopped = true;

                foreach (GameObject stone in allStones)
                {
                    Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
                    if (rb.velocity.magnitude > 0)
                    {
                        stone.GetComponent<SelectStone>().enabled = false;
                        stone.GetComponent<ThrowStone>().enabled = false;
                        allStonesStopped = false;
                        break;
                    }
                }

                // 모든 돌들이 움직임을 멈추면 SelectStone과 StoneScript 활성화
                if (allStonesStopped)
                {
                    foreach (GameObject stone in allStones)
                    {
                        stone.GetComponent<SelectStone>().enabled = true;
                        stone.GetComponent<ThrowStone>().enabled = true;
                    }
                    checkStonesMoving = false;
                }
            }
            else{
                MoveStonesToNearestOddCoordinates();
            }
        }
        if (Input.GetMouseButtonDown(0) && (placementTurns/10)%2==0 && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceStone();
            MoveStonesToNearestOddCoordinates();
            if ((placementTurns % 10 == 0)&&(placementTurns!=0)){
                Debug.Log("AllKKAGI Turn");
                // placementTurns가 10으로 나누어 떨어질 때마다 배경색을 변경합니다.
                Camera mainCamera = Camera.main;
                mainCamera.backgroundColor = new Color(Random.value, Random.value, Random.value);
            }
        }
        if(set){
            StoneSet(X, Y);
        }
        
    }

}
