
const tickRate: number = 1

function getPlayerByNo(state: State, playerNo: number): Player {
    return state.players[playerNo]
}

function getPlayerById(state: State, userId: string): Player {
    if(userId === null)
        throw "why is dis null?"

    let player = getPlayerByNo(state, 1)
    if(player.userId === userId)
        return player

    player = getPlayerByNo(state, 2)
    if(player.userId === userId)
        return player
    
    throw "who da fuck is dis?"
}

let matchInit: nkruntime.MatchInitFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, params: { [key: string]: string }): { state: State, tickRate: number, label: string } {
    logger.info("is dis a test?")
    let state: State = { playing: false, players: {}, rounds: {}, currentRound: 1 }
    let label = { game: module }
    return { state: state, tickRate: tickRate, label: JSON.stringify(label) }
}

let matchJoinAttempt: nkruntime.MatchJoinAttemptFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, presence: nkruntime.Presence, metadata: { [key: string]: any }): { state: State, accept: boolean, rejectMessage?: string } | null {
    if (state.playing)
        return { state: state, accept: false, rejectMessage: "you cant join a game that is still going on" }

    return { state: state, accept: true }
}

function join(state: State, presence: nkruntime.Presence): boolean{
    if (state.players[1] === null) {
        state.players[1] = {userId: presence.userId, playerNo: 1}
        return true
    }
    else if (state.players[2] === null) {
        state.players[2] = {userId: presence.userId, playerNo: 2}
    }
    return false
}

let matchJoin: nkruntime.MatchJoinFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, presences: nkruntime.Presence[]): { state: State } | null {
    let canJoin = true
    for (const presence of presences) {
        if(!canJoin)
            break
        canJoin = join(state, presence)
    }
    if(canJoin){
        state.playing = true
    }
    return { state }
}

let matchLeave: nkruntime.MatchLeaveFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, presences: nkruntime.Presence[]): { state: State } | null {
    return { state }
}

let matchLoop: nkruntime.MatchLoopFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, messages: nkruntime.MatchMessage[]): { state: State } | null {
    if (!state.playing)
        return { state }

    if(state.currentRound === 11)
        return null

    let round = state.rounds[state.currentRound]
    if(round === null)
        round = state.rounds[state.currentRound] = {active: true, completed: false, roundNo: state.currentRound, moves: {}}

    for (const message of messages) {
        switch (message.opCode) {
            case OpCode.MOVE:
                let player = getPlayerById(state, message.sender.userId)
                let move = round.moves[player.playerNo]

                if ((move ?? MOVE.Air) !== MOVE.Air) {
                    dispatcher.broadcastMessage(OpCode.MOVE_REJECT, "cant change your move", [message.sender])
                    continue
                }

                let msg = {} as MoveMessage;
                try {
                    msg = JSON.parse(nk.binaryToString(message.data));
                } catch (error) {
                    dispatcher.broadcastMessage(OpCode.REJECTED, "wat did you send to me?", [message.sender])
                    logger.debug('Bad data received: %v', error);
                    continue;
                }

                move = round.moves[player.playerNo] = msg.move
                dispatcher.broadcastMessage(OpCode.MOVE, JSON.stringify({playerNo: player.playerNo}))

                if (checkRoundComplete(round)) {
                    
                    let winlose = WinLose(round)
                    let reveal: RevealMessage = {isTie: winlose === null, moves: {1: round.moves[1], 2: round.moves[2]}, winner: winlose?.winner ?? null}
                    dispatcher.broadcastMessage(OpCode.REVEAL, JSON.stringify(reveal))
                    
                    if (winlose !== null) {
                        nk.walletUpdate(state.players[winlose.winner].userId, { coins: +5 }, undefined, true)
                        nk.walletUpdate(state.players[winlose.loser].userId, { coins: -5 }, undefined, true)
                    }
                    state.currentRound++
                }

                break;

            default:
                break;
        }
    }


    return { state }
}

let matchTerminate: nkruntime.MatchTerminateFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, graceSeconds: number): { state: State } | null {
    return { state }
}

let matchSignal: nkruntime.MatchSignalFunction<State> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: State, data: string): { state: State, data?: string } | null {
    return { state }
}

function checkRoundComplete(round: Round) {
    if (!round.active)
        throw new Error("this round not active")

    if ((round.moves[1] ?? MOVE.Air) !== MOVE.Air && (round.moves[2] ?? MOVE.Air) !== MOVE.Air) {
        round.completed = true
        round.active = false
    }

    return round.completed
}

function WinLose(round: Round): { winner: PlayerNo, loser: PlayerNo } | null {
    let m1 = round.moves[1]
    let m2 = round.moves[2]

    if (m1 === m2)
        return null

    let winner: PlayerNo, loser: PlayerNo

    let is1win = (m1 === MOVE.Rock && m2 === MOVE.Scissor) || (m1 === MOVE.Paper && m2 === MOVE.Rock) || (m1 === MOVE.Scissor && m2 === MOVE.Paper)

    if (is1win) {
        winner = 1
        loser = 2
    } else {
        winner = 2
        loser = 1
    }

    return { winner, loser }
}