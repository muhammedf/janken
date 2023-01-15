
const module = "janken"

function InitModule(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer): void {
    initializer.registerMatch(module, {
        matchInit: matchInit,
        matchJoin: matchJoin,
        matchJoinAttempt: matchJoinAttempt,
        matchLeave: matchLeave,
        matchLoop: matchLoop,
        matchSignal: matchSignal,
        matchTerminate: matchTerminate
    })

    initializer.registerMatchmakerMatched(matchMakerMatched)
    initializer.registerRtBefore("MatchmakerAdd", beforeMatchmakerAdd)

    logger.info('JavaScript logic loaded.');
}

let matchMakerMatched: nkruntime.MatchmakerMatchedFunction = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, matches: nkruntime.MatchmakerResult[]): string | void {
    try {
        const matchId = nk.matchCreate(module);
        return matchId;
    }
    catch (err) {
        logger.error("error", err)
    }
}

let beforeMatchmakerAdd: nkruntime.RtBeforeHookFunction<nkruntime.Envelope> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, envelope: nkruntime.Envelope): nkruntime.Envelope | void {
    let env = envelope as nkruntime.EnvelopeMatchmakerAdd

    env.matchmakerAdd.maxCount = 2
    env.matchmakerAdd.minCount = 2

    return env
}

