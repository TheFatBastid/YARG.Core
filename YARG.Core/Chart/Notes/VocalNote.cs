using System;

namespace YARG.Core.Chart
{
    /// <summary>
    /// A note on a vocals track.
    /// </summary>
    public class VocalNote : Note<VocalNote>
    {
        /// <summary>
        /// The type of vocals note (either a phrase, a lyrical note or a percussion hit).
        /// </summary>
        public VocalNoteType Type { get; }

        /// <summary>
        /// 0-based index for the harmony part this note is a part of.
        /// HARM1 is 0, HARM2 is 1, HARM3 is 2.
        /// </summary>
        public int HarmonyPart { get; }

        /// <summary>
        /// The MIDI pitch of the note, as a float.
        /// -1 means the note is unpitched.
        /// </summary>
        public float Pitch { get; }

        /// <summary>
        /// The octave of the vocal pitch.
        /// Octaves start at -1 in MIDI: note 60 is C4, note 12 is C0, note 0 is C-1.
        /// </summary>
        public int Octave => (int) (Pitch / 12) - 1;

        /// <summary>
        /// The pitch of the note wrapped relative to an octave (0-11).
        /// C is 0, B is 11. -1 means the note is unpitched.
        /// </summary>
        public float OctavePitch => Pitch % 12;

        /// <summary>
        /// The length of this note and all of its children, in seconds.
        /// </summary>
        public double TotalTimeLength { get; private set; }

        /// <summary>
        /// The time-based end of this note and all of its children.
        /// </summary>
        public double TotalTimeEnd => Time + TotalTimeLength;

        /// <summary>
        /// The length of this note and all of its children, in ticks.
        /// </summary>
        public uint TotalTickLength { get; private set; }
        /// <summary>
        /// The tick-based end of this note and all of its children.
        /// </summary>
        public uint TotalTickEnd => Tick + TotalTickLength;

        /// <summary>
        /// Whether or not this note is non-pitched.
        /// </summary>
        public bool IsNonPitched => Pitch < 0;

        /// <summary>
        /// Whether or not this note is a percussion note.
        /// </summary>
        public bool IsPercussion => Type == VocalNoteType.Percussion;

        /// <summary>
        /// Whether or not this note is a vocal phrase.
        /// </summary>
        public bool IsPhrase => Type == VocalNoteType.Phrase;

        /// <summary>
        /// Creates a new <see cref="VocalNote"/> with the given properties.
        /// This constructor should be used for notes only.
        /// </summary>
        public VocalNote(float pitch, int harmonyPart, VocalNoteType type,
            double time, double timeLength, uint tick, uint tickLength)
            : base(NoteFlags.None, time, timeLength, tick, tickLength)
        {
            Type = type;
            Pitch = pitch;
            HarmonyPart = harmonyPart;

            TotalTimeLength = timeLength;
            TotalTickLength = tickLength;
        }

        /// <summary>
        /// Creates a new <see cref="VocalNote"/> phrase with the given properties.
        /// This constructor should be used for vocal phrases only.
        /// </summary>
        public VocalNote(NoteFlags noteFlags,
            double time, double timeLength, uint tick, uint tickLength)
            : base(noteFlags, time, timeLength, tick, tickLength)
        {
            Type = VocalNoteType.Phrase;

            TotalTimeLength = timeLength;
            TotalTickLength = tickLength;
        }

        public VocalNote(VocalNote other) : base(other)
        {
            Type = other.Type;
            Pitch = other.Pitch;
            HarmonyPart = other.HarmonyPart;

            TotalTimeLength = other.TotalTickLength;
            TotalTickLength = other.TotalTickLength;
        }

        /// <summary>
        /// Gets the pitch of this note and its children at the specified tick.
        /// Clamps to the start and end if the time is out of bounds.
        /// </summary>
        public float PitchAtSongTick(uint tick)
        {
            if (Type == VocalNoteType.Phrase)
            {
                return -1f;
            }

            // Clamp to start
            if (tick < TickEnd || ChildNotes.Count < 1)
            {
                return Pitch;
            }

            // Search child notes
            var firstNote = this;
            foreach (var secondNote in ChildNotes)
            {
                // Check note bounds
                if (tick >= firstNote.Tick && tick < secondNote.TickEnd)
                {
                    // Check if tick is in a specific pitch
                    if (tick < firstNote.TickEnd)
                        return firstNote.Pitch;

                    if (tick >= secondNote.Tick)
                        return secondNote.Pitch;

                    // Tick is between the two pitches, lerp them
                    return YargMath.Lerp(firstNote.Pitch, secondNote.Pitch, firstNote.TickEnd, secondNote.Tick, tick);
                }

                firstNote = secondNote;
            }

            // Clamp to end
            return ChildNotes[^1].Pitch;
        }

        /// <summary>
        /// Adds a child note to this vocal note.
        /// Use <see cref="AddNoteToPhrase"/> instead if this is a phrase!
        /// </summary>
        public override void AddChildNote(VocalNote note)
        {
            // Use AddNoteToPhrase instead!
            if (IsPhrase) return;

            if (note.Tick <= Tick || note.ChildNotes.Count > 0)
                return;

            _childNotes.Add(note);

            // Sort child notes by tick
            _childNotes.Sort((note1, note2) =>
            {
                if (note1.Tick > note2.Tick) return 1;
                if (note1.Tick < note2.Tick) return -1;
                return 0;
            });

            // Track total length
            TotalTimeLength = _childNotes[^1].TimeEnd - Time;
            TotalTickLength = _childNotes[^1].TickEnd - Tick;
        }

        /// <summary>
        /// Adds a child note to this vocal phrase.
        /// Use <see cref="AddChildNote"/> instead if this is a note!
        /// </summary>
        public void AddNoteToPhrase(VocalNote note)
        {
            // Use AddChildNote instead!
            if (!IsPhrase) return;

            if (note.Tick <= Tick)
                return;

            _childNotes.Add(note);

            // Sort child notes by tick
            _childNotes.Sort((note1, note2) =>
            {
                if (note1.Tick > note2.Tick) return 1;
                if (note1.Tick < note2.Tick) return -1;
                return 0;
            });
        }

        protected override VocalNote CloneNote()
        {
            return new(this);
        }
    }

    /// <summary>
    /// Possible vocal note types.
    /// </summary>
    public enum VocalNoteType
    {
        Phrase,
        Lyric,
        Percussion
    }
}