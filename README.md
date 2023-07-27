# 오목 & 알까기

> 박근영, 허도영

> Unity, Express

|홈|오목|알까기|
|--|--|--|
|<img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="200" height="400">|<img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="200" height="400">|<img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="200" height="400">|

## 홈
> 방을 생성하거나 생성된 방에 참여할 수 있습니다.

- ### 방 생성

  <img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="200" height="400">

   - 좌측 상단의 Create Room 버튼을 눌러 방을 생성할 수 있음
   - 방을 생성하면 서버로부터 방 번호를 전달 받음

- ### 방 참여

  <img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="200" height="400">

  - 가운데 하단의 Text 입력 필드에 방 번호를 입력하고 우측 상단의 Join Game 버튼을 눌러 방에 참여할 수 있음
  - 방에 참여 시 게임이 시작되어 오목을 할 수 있음


## Round1 : 오목
> 방을 만든 사람이 흑, 참여한 사람이 백이 됩니다.
> 기존의 오목 룰처럼 번갈아가며 돌을 둘 수 있습니다.
> 일정 턴이 지나도 게임이 끝나지 않으면 현재 판 그대로 게임이 알까기로 바뀝니다.

- ### 오목

  <img src="https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width="500" height="400">

  - 격자점 근처를 좌클릭 하여 돌을 놓을 수 있음

## Round2 : 알까기
> 번갈아가며 본인의 돌을 클릭하고 당겼다 놓음으로써 알까기를 할 수 있습니다.
> 일정 턴이 지나도 게임이 끝나지 않으면 돌들이 가장 가까운 격자점으로 이동하며 게임이 다시 오목으로 바뀝니다.

- ### 알까기
  <img src = "https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width = "200" height = "400" >
  
  - 원하는 돌을 우클릭하여 선택할 수 있음
  - 돌을 선택한 후 좌클릭으로 드래그 했다 놓으면 돌이 움직임

- ### 돌 위치 변환
  <img src = "https://github.com/Sebyeol23/FlowCamp4/assets/98662998/288d065b-0acd-417d-86b6-e9360a4cfd5c" width = "500" height = "400" >

  - 판 위의 모든 돌들이 가장 가까운 격자점으로 이동함