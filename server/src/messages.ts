

type PlayerNo = 1|2

interface State
{
    playing: boolean
    players: {[key: number]: Player}
    rounds: {[key: number]: Round}
    currentRound: number
}

interface Player 
{
    userId: string,
    playerNo: PlayerNo
}

enum MOVE{
    Air = 0,
    Rock = 1,
    Paper = 2,
    Scissor = 3
}

enum OpCode{
    MOVE = 0,
    REVEAL = 1,
    MOVE_REJECT = 2,
    REJECTED = 3
}

interface Round
{
    roundNo: number
    moves: {[key: number]: MOVE}
    active: boolean
    completed: boolean
}

interface MoveMessage
{
    move: MOVE
}

interface RevealMessage
{
    moves: {[key in PlayerNo]: MOVE}
    winner: PlayerNo|null
    isTie: boolean
}