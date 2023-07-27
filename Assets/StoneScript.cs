using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ThrowStone : MonoBehaviour
{
    public GameObject collisionPrefab;
    public AudioSource audioSource; 

    public float throwForce = 15.0f;

    private Rigidbody2D rb;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private bool isDragging = false;

    public static bool set = false;
    public static float X;
    public static float Y;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌이 일어나면 스파크 효과를 생성합니다.
        GameObject spark = Instantiate(collisionPrefab, collision.contacts[0].point, Quaternion.identity);
        spark.SetActive(true);
        // 충돌의 강도에 따라 스파크의 크기를 조정합니다.
        float collisionForce = collision.relativeVelocity.magnitude;
        spark.transform.localScale = Vector3.one * collisionForce;

        // 충돌의 강도에 따라 소리의 볼륨을 조절하고, 소리를 재생합니다.
        
        audioSource.volume = collisionForce / 30f;  // 볼륨은 0.0에서 1.0 사이의 값이어야 합니다.
        audioSource.Play();

        // 스파크가 일정 시간 후에 사라지게 합니다. (예를 들어 1초 후에)
        Destroy(spark, 1f);
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if ((GameManager.instance.placementTurns/10)%2==0) {
            return;
        }

        
        if (GameManager.currentPlayer == this.gameObject.tag && SelectStone.selectedStone != null && SelectStone.selectedStone.gameObject == this.gameObject) // 선택된 돌이 이 돌일 경우에만
        {
            // 마우스 버튼을 누르면 시작점을 저장하고, 끌기 시작합니다.
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            // 마우스 버튼을 놓으면 끌기를 종료하고, 돌을 던집니다.
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                GameManager.ShootStone((startPoint - endPoint).normalized.x, (startPoint - endPoint).normalized.y);

            }
        }

        if(set && GameManager.currentPlayer == this.gameObject.tag && SelectStone.selectedStone != null && SelectStone.selectedStone.gameObject == this.gameObject){
            Vector2 direction = new Vector2(X, Y); // 시작점에서 종료점으로의 벡터 방향을 계산합니다.

            rb.AddForce(direction * throwForce, ForceMode2D.Impulse); // 돌을 해당 방향으로 던집니다.
            SelectStone.selectedStone.Deselect(); // 돌을 던진 후에는 선택을 해제합니다.
            GameManager.currentPlayer = GameManager.currentPlayer == "Black" ? "White" : "Black";
            GameManager.instance.placementTurns++;
            

            // GameManager의 allStones 리스트에 접근
            List<GameObject> allStones = GameManager.instance.allStones;
            // 새로운 리스트 생성
            List<GameObject> newStones = new List<GameObject>();

            // allStones 리스트의 모든 돌을 순회
            foreach (GameObject stone in allStones)
            {
                Vector2 pos = stone.transform.position;

                // 돌의 위치가 범위 내에 있는 경우 newStones 리스트에 추가
                if (pos.x >= -9 && pos.x <= 9 && pos.y >= -9 && pos.y <= 9)
                {
                    newStones.Add(stone);
                }
                // 그렇지 않은 경우 돌 제거
                else
                {
                    Destroy(stone);
                }
            }
            // allStones 리스트를 newStones 리스트로 갱신
            GameManager.instance.allStones = newStones;

            set = false;
        }
    }
}
