namespace MissionControl.Definitions
{
    public enum EngineParameters : byte
    {
        TimerCountDown 						= 0,
        TimerPreFeed 						= 1,
        TimerIgnition 						= 2,
        TimerPrestage1 						= 3,
        TimerPrestage2Max 					= 4,
        TimerPrestage2Stable 				= 5,
        TimerRampUpStable 					= 6,
        TimerRampUpMax 						= 7,
        TimerPrestageShutdown1 				= 8,
        TimerPrestageShutdown2 				= 9,
        TimerBurn 							= 10,
        TimerShutdown1 						= 11,
        TimerShutdown2 						= 12,

        PositionOxidizerPreFeed 			= 13,
        PositionOxidizerPrestage1 			= 14,
        PositionOxidizerPrestage2 			= 15,
        PositionOxidizerRampUp 				= 16,
        PositionOxidizerPrestageShutdown1 	= 17,
        PositionOxidizerPrestageShutdown2 	= 18,
        PositionOxidizerShutdown1 			= 19,
        PositionOxidizerShutdown2 			= 20,
        PositionFuelPrestage1 				= 21,
        PositionFuelPrestage2 				= 22,
        PositionFuelRampUp 					= 23,
        PositionFuelPrestageShutdown1 		= 24,
        PositionFuelPrestageShutdown2 		= 25,
        PositionFuelShutdown1 				= 26,
        PositionFuelShutdown2 				= 27,
	
        SweepOxidizerSweepPrestage1 		= 28,
        SweepOxidizerSweepPrestage2 		= 29,
        SweepOxidizerSweepRampUp 			= 30,
        SweepFuelSweepPrestage1 			= 31,
        SweepFuelSweepPrestage2 			= 32,
        SweepFuelSweepRampUp 				= 33,

        PressureChamberStablePrestage2 		= 34,
        PressureChamberThreshold 			= 35,
        None 								= 255,
    };
}