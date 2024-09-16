# Chess Online

### 게임 모드
- 1 Player (vs AI) <<<<< 어떻게 구현하지?
- 2 Player (1 device offline PVP)
- 2 Player (with Internet)

### 규칙

#### 기본 행마법
- 폰(P, Pawn): 1칸 전방 전진, 2칸 전방 전진, 1칸 대각선 전방 전진
- 비숍(B, Bishop): 아군을 뛰어넘지 않는 대각선 이동
- 나이트(N, Knight): L자 이동
- 룩(R, Rook): 아군을 뛰어넘지 않는 전/후/좌/우 이동
- 퀸(Q, Queen): 비숍 또는 룩의 역할을 수행함
- 킹(K, King): 킹 주위의 인접한 8칸 중 한 칸으로 이동

#### 특수 행마법
- 프로모션(Promotion)
- 캐슬링(Castling)
- 앙파상(En Passant)

#### 체크메이트(Checkmate)
킹의 현재 위치가 다음 수에 잡힐 수 있는 위치이면서 킹이 어떤 위치로 움직여도 반드시 자충수가 되는 상황이 발생하는 경우에 해당 플레이어는 패배한다.

#### 스테일메이트(Stalemate) 무승부 규칙
킹의 현재 위치가 다음 수에 잡힐 수 있는 위치는 아니지만 어떤 기물을 어떤 위치로 움직여도 반드시 자충수가 되는 상황이 발생하면 무승부 처리한다.

#### 기물 조합에 의한 무승부 규칙
아래 조합은 체크메이트가 불가능한 기물 조합이므로 무승부 처리한다.
- K vs K
- K+N vs K
- K+B vs K
- K+N vs K+N
- K+N vs K+B
- K+B vs K+B