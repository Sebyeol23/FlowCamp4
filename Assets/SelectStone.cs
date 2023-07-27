using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStone : MonoBehaviour
{
    public static SelectStone selectedStone = null; // static 변수를 추가하여 한 번에 하나의 돌만 선택되도록 합니다.
    public bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.red; // 선택 시 돌에 주는 빛의 색깔을 정의합니다. 원하는 색으로 변경 가능합니다.
    private Color originalColor; // 돌의 원래 색깔을 저장합니다.

    public static bool set = false;
    public static float X;
    public static float Y;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 초기 색깔을 저장합니다.
    }


    private void Update()
    {   
        
        if ((GameManager.instance.placementTurns/10)%2==0) {
            return;
        }
        Vector3 pos = this.transform.position;

        // 돌이 정사각형 영역 밖으로 나갔다면
        
        if (pos.x < -9.5f || pos.x > 9.5f || pos.y < -9.5f || pos.y > 9.5f)
        {
            this.gameObject.SetActive(false); // 돌을 비활성화하여 보이지 않게 합니다.
        }
        
        else // 돌이 정사각형 영역 안에 있다면
        {
            this.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(1) && GameManager.myColor == GameManager.currentPlayer) //른쪽 마우스 버튼을 눌렀을 때
            {
                Vector2 mousePos1 = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos1, Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == this.gameObject) // 현재 객체를 클릭한 경우
                {
                    GameManager.Select(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                }       
           }
        }

        if(set){
            Vector2 mousePos = new Vector2(X, Y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);           

            if (hit.collider != null && hit.collider.gameObject == this.gameObject) // 현재 객체를 클릭한 경우
            {
                if (selectedStone != null) // 이전에 선택된 돌이 있다면
                {
                    selectedStone.Deselect(); // 그 돌의 선택을 해제합니다.
                }
                Select();
                set = false;
            }
        }
    }
    public void Select()
    {
        isSelected = true;
        selectedStone = this;
        spriteRenderer.color = highlightColor; // 선택된 돌에 빛을 줍니다.
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = originalColor; // 빛을 제거하고 원래 색으로 돌려놓습니다.
    }
}