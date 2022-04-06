namespace MissionControl.Definitions;

[Flags]
public enum RocketStates : byte
{
    Idle = 0,
    PreFillCheck = 1,
    PreFillError = 2,
    PreFillWait = 3,

    //Post-Fill
    PostFillCheck = 4,
    PostFillError = 5,
    PostFillWait = 6,

    //Internal-Power
    InternalPower = 7,
    InternalCheck = 8,
    InternalError = 9,
    InternalWait = 10,

    //Pressurization
    Pressurization = 11,
    PressurizationCheck = 12,
    PressurizationError = 13,

    //Abort Launch
    AbortLaunch = 14,
    AbortDump = 15,

    //Launch
    HandOverWait = 16,
    LaunchCountDown = 17,
    PreFeed = 18,
    LaunchIgnite = 19,
    PreStage1 = 20,
    PreStage2 = 21,
    RampUp = 22,
    PreStageShutdown1 = 23,
    PreStageShutdown2 = 24,

    //Flight
    Burn = 25,
    Shutdown1 = 26,
    Shutdown2 = 27,
    WaitTouchdown = 28,

    //Post flight
    Recovery = 29,
}