﻿namespace YARG.Core.Engine
{
    public abstract class BaseEngineState
    {
        public int NoteIndex;

        public double CurrentTime;
        public double LastUpdateTime;

        public uint CurrentTick;
        public uint LastTick;

        public int CurrentTimeSigIndex;
        public int NextTimeSigIndex;

        public int CurrentStarIndex;

        public uint TicksEveryEightMeasures;

        public virtual void Reset()
        {
            NoteIndex = 0;

            CurrentTime = 0;
            LastUpdateTime = 0;

            CurrentTick = 0;
            LastTick = 0;

            CurrentTimeSigIndex = 0;
            NextTimeSigIndex = 1;

            CurrentStarIndex = 0;

            TicksEveryEightMeasures = 0;
        }
    }
}