﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YARG.Core.Extensions;
using YARG.Core.IO.Ini;

namespace YARG.Core.IO
{
    public enum ChartEventType
    {
        Bpm,
        Time_Sig,
        Anchor,
        Text,
        Note,
        Special,
        Unknown = 255,
    }

    public enum NoteTracks_Chart
    {
        Single,
        DoubleGuitar,
        DoubleBass,
        DoubleRhythm,
        Drums,
        Keys,
        GHLGuitar,
        GHLBass,
        GHLRhythm,
        GHLCoop,
        Invalid,
    };

    public struct DotChartEvent
    {
        private long _position;

        public ChartEventType Type;
        public long Position
        {
            get { return _position; }
            set
            {
                if (_position <= value)
                    _position = value;
                else
                    throw new Exception($".chart position out of order (previous: {_position})");
            }
        }
    }

    public struct DotChartNote
    {
        public int Lane;
        public long Duration;
    }

    public readonly struct DotChartEventCombo<T>
        where T : unmanaged, IEquatable<T>
    {
        public readonly T[] descriptor;
        public readonly ChartEventType eventType;
        public DotChartEventCombo(ReadOnlySpan<T> descriptor, ChartEventType eventType)
        {
            this.descriptor = descriptor.ToArray();
            this.eventType = eventType;
        }

        public bool DoesEventMatch(ReadOnlySpan<T> span)
        {
            if (span.Length != descriptor.Length)
                return false;

            for (int i = 0; i < descriptor.Length; i++)
                if (!span[i].Equals(descriptor[i]))
                    return false;
            return true;
        }
    }

    public interface IDotChartBases<T>
        where T : unmanaged, IEquatable<T>
    {
        public ReadOnlySpan<T> HEADERTRACK { get; }
        public ReadOnlySpan<T> SYNCTRACK { get; }
        public ReadOnlySpan<T> EVENTTRACK { get; }
        public DotChartEventCombo<T> TEMPO { get; }
        public DotChartEventCombo<T> TIMESIG { get; }
        public DotChartEventCombo<T> ANCHOR { get; }
        public DotChartEventCombo<T> TEXT { get; }
        public DotChartEventCombo<T> NOTE { get; }
        public DotChartEventCombo<T> SPECIAL { get; }

        public (T[], Difficulty)[] DIFFICULTIES { get; }
        public (T[], NoteTracks_Chart)[] NOTETRACKS { get; }
        public DotChartEventCombo<T>[] EVENTS_SYNC { get; }
        public DotChartEventCombo<T>[] EVENTS_EVENTS { get; }
        public DotChartEventCombo<T>[] EVENTS_DIFF { get; }
    }

    public readonly struct DotChartByte : IDotChartBases<byte>
    {
        private static readonly byte[] _HEADERTRACK = Encoding.ASCII.GetBytes("[Song]");
        private static readonly byte[] _SYNCTRACK = Encoding.ASCII.GetBytes("[SyncTrack]");
        private static readonly byte[] _EVENTTRACK = Encoding.ASCII.GetBytes("[Events]");
        private static readonly DotChartEventCombo<byte> _TEMPO = new(Encoding.ASCII.GetBytes("B"), ChartEventType.Bpm);
        private static readonly DotChartEventCombo<byte> _TIMESIG = new(Encoding.ASCII.GetBytes("TS"), ChartEventType.Time_Sig);
        private static readonly DotChartEventCombo<byte> _ANCHOR = new(Encoding.ASCII.GetBytes("A"), ChartEventType.Anchor);
        private static readonly DotChartEventCombo<byte> _TEXT = new(Encoding.ASCII.GetBytes("E"), ChartEventType.Text);
        private static readonly DotChartEventCombo<byte> _NOTE = new(Encoding.ASCII.GetBytes("N"), ChartEventType.Note);
        private static readonly DotChartEventCombo<byte> _SPECIAL = new(Encoding.ASCII.GetBytes("S"), ChartEventType.Special);

        private static readonly (byte[] name, Difficulty difficulty)[] _DIFFICULTIES =
        {
            (Encoding.ASCII.GetBytes("[Easy"), Difficulty.Easy),
            (Encoding.ASCII.GetBytes("[Medium"), Difficulty.Medium),
            (Encoding.ASCII.GetBytes("[Hard"), Difficulty.Hard),
            (Encoding.ASCII.GetBytes("[Expert"), Difficulty.Expert),
        };

        private static readonly (byte[], NoteTracks_Chart)[] _NOTETRACKS =
        {
            new(Encoding.ASCII.GetBytes("Single]"),       NoteTracks_Chart.Single ),
            new(Encoding.ASCII.GetBytes("DoubleGuitar]"), NoteTracks_Chart.DoubleGuitar ),
            new(Encoding.ASCII.GetBytes("DoubleBass]"),   NoteTracks_Chart.DoubleBass ),
            new(Encoding.ASCII.GetBytes("DoubleRhythm]"), NoteTracks_Chart.DoubleRhythm ),
            new(Encoding.ASCII.GetBytes("Drums]"),        NoteTracks_Chart.Drums ),
            new(Encoding.ASCII.GetBytes("Keyboard]"),     NoteTracks_Chart.Keys ),
            new(Encoding.ASCII.GetBytes("GHLGuitar]"),    NoteTracks_Chart.GHLGuitar ),
            new(Encoding.ASCII.GetBytes("GHLBass]"),      NoteTracks_Chart.GHLBass ),
            new(Encoding.ASCII.GetBytes("GHLRhythm]"),    NoteTracks_Chart.GHLGuitar ),
            new(Encoding.ASCII.GetBytes("GHLCoop]"),      NoteTracks_Chart.GHLBass ),
        };

        private static readonly DotChartEventCombo<byte>[] _EVENTS_SYNC = { _TEMPO, _TIMESIG, _ANCHOR };
        private static readonly DotChartEventCombo<byte>[] _EVENTS_EVENTS = { _TEXT, };
        private static readonly DotChartEventCombo<byte>[] _EVENTS_DIFF = { _NOTE, _SPECIAL, _TEXT, };

        public ReadOnlySpan<byte> HEADERTRACK => _HEADERTRACK;
        public ReadOnlySpan<byte> SYNCTRACK => _SYNCTRACK;
        public ReadOnlySpan<byte> EVENTTRACK => _EVENTTRACK;
        public DotChartEventCombo<byte> TEMPO => _TEMPO;
        public DotChartEventCombo<byte> TIMESIG => _TIMESIG;
        public DotChartEventCombo<byte> ANCHOR => _ANCHOR;
        public DotChartEventCombo<byte> TEXT => _TEXT;
        public DotChartEventCombo<byte> NOTE => _NOTE;
        public DotChartEventCombo<byte> SPECIAL => _SPECIAL;

        public (byte[], Difficulty)[] DIFFICULTIES => _DIFFICULTIES;
        public (byte[], NoteTracks_Chart)[] NOTETRACKS => _NOTETRACKS;
        public DotChartEventCombo<byte>[] EVENTS_SYNC => _EVENTS_SYNC;
        public DotChartEventCombo<byte>[] EVENTS_EVENTS => _EVENTS_EVENTS;
        public DotChartEventCombo<byte>[] EVENTS_DIFF => _EVENTS_DIFF;
    }

    public readonly struct DotChartChar : IDotChartBases<char>
    {
        private static readonly string _HEADERTRACK = "[Song]";
        private static readonly string _SYNCTRACK = "[SyncTrack]";
        private static readonly string _EVENTTRACK = "[Events]";
        private static readonly DotChartEventCombo<char> _TEMPO = new("B", ChartEventType.Bpm);
        private static readonly DotChartEventCombo<char> _TIMESIG = new("TS", ChartEventType.Time_Sig);
        private static readonly DotChartEventCombo<char> _ANCHOR = new("A", ChartEventType.Anchor);
        private static readonly DotChartEventCombo<char> _TEXT = new("E", ChartEventType.Text);
        private static readonly DotChartEventCombo<char> _NOTE = new("N", ChartEventType.Note);
        private static readonly DotChartEventCombo<char> _SPECIAL = new("S", ChartEventType.Special);

        private static readonly (char[] name, Difficulty difficulty)[] _DIFFICULTIES =
        {
            ("[Easy".ToCharArray(),   Difficulty.Easy),
            ("[Medium".ToCharArray(), Difficulty.Medium),
            ("[Hard".ToCharArray(),   Difficulty.Hard),
            ("[Expert".ToCharArray(), Difficulty.Expert),
        };

        private static readonly (char[], NoteTracks_Chart)[] _NOTETRACKS =
        {
            new("Single]".ToCharArray(),       NoteTracks_Chart.Single ),
            new("DoubleGuitar]".ToCharArray(), NoteTracks_Chart.DoubleGuitar ),
            new("DoubleBass]".ToCharArray(),   NoteTracks_Chart.DoubleBass ),
            new("DoubleRhythm]".ToCharArray(), NoteTracks_Chart.DoubleRhythm ),
            new("Drums]".ToCharArray(),        NoteTracks_Chart.Drums ),
            new("Keyboard]".ToCharArray(),     NoteTracks_Chart.Keys ),
            new("GHLGuitar]".ToCharArray(),    NoteTracks_Chart.GHLGuitar ),
            new("GHLBass]".ToCharArray(),      NoteTracks_Chart.GHLBass ),
            new("GHLRhythm]".ToCharArray(),    NoteTracks_Chart.GHLGuitar ),
            new("GHLCoop]".ToCharArray(),      NoteTracks_Chart.GHLBass ),
        };

        private static readonly DotChartEventCombo<char>[] _EVENTS_SYNC = { _TEMPO, _TIMESIG, _ANCHOR };
        private static readonly DotChartEventCombo<char>[] _EVENTS_EVENTS = { _TEXT, };
        private static readonly DotChartEventCombo<char>[] _EVENTS_DIFF = { _NOTE, _SPECIAL, _TEXT, };

        public ReadOnlySpan<char> HEADERTRACK => _HEADERTRACK;
        public ReadOnlySpan<char> SYNCTRACK => _SYNCTRACK;
        public ReadOnlySpan<char> EVENTTRACK => _EVENTTRACK;
        public DotChartEventCombo<char> TEMPO => _TEMPO;
        public DotChartEventCombo<char> TIMESIG => _TIMESIG;
        public DotChartEventCombo<char> ANCHOR => _ANCHOR;
        public DotChartEventCombo<char> TEXT => _TEXT;
        public DotChartEventCombo<char> NOTE => _NOTE;
        public DotChartEventCombo<char> SPECIAL => _SPECIAL;
        public (char[], Difficulty)[] DIFFICULTIES => _DIFFICULTIES;
        public (char[], NoteTracks_Chart)[] NOTETRACKS => _NOTETRACKS;
        public DotChartEventCombo<char>[] EVENTS_SYNC => _EVENTS_SYNC;
        public DotChartEventCombo<char>[] EVENTS_EVENTS => _EVENTS_EVENTS;
        public DotChartEventCombo<char>[] EVENTS_DIFF => _EVENTS_DIFF;
    }

    public sealed class YARGChartFileReader<TChar, TBase>
        where TChar : unmanaged, IEquatable<TChar>, IConvertible
        where TBase : unmanaged, IDotChartBases<TChar>
    {
        private static readonly TBase CONFIG = default;
        private readonly YARGTextReader<TChar> reader;

        private DotChartEventCombo<TChar>[] eventSet = Array.Empty<DotChartEventCombo<TChar>>();
        private NoteTracks_Chart _instrument;
        private Difficulty _difficulty;

        public NoteTracks_Chart Instrument => _instrument;
        public Difficulty Difficulty => _difficulty;

        public YARGChartFileReader(YARGTextReader<TChar> reader)
        {
            this.reader = reader;
        }

        public bool IsStartOfTrack()
        {
            return !reader.IsEndOfFile() && reader.IsCurrentCharacter('[');
        }

        public bool ValidateHeaderTrack()
        {
            return ValidateTrack(CONFIG.HEADERTRACK);
        }

        public bool ValidateSyncTrack()
        {
            if (!ValidateTrack(CONFIG.SYNCTRACK))
                return false;

            eventSet = CONFIG.EVENTS_SYNC;
            return true;
        }

        public bool ValidateEventsTrack()
        {
            if (!ValidateTrack(CONFIG.EVENTTRACK))
                return false;

            eventSet = CONFIG.EVENTS_EVENTS;
            return true;
        }

        public bool ValidateDifficulty()
        {
            for (int diff = 3; diff >= 0; --diff)
            {
                var (name, difficulty) = CONFIG.DIFFICULTIES[diff];
                if (DoesStringMatch(name))
                {
                    _difficulty = difficulty;
                    eventSet = CONFIG.EVENTS_DIFF;
                    reader.Position += name.Length;
                    return true;
                }
            }
            return false;
        }

        public bool ValidateInstrument()
        {
            foreach (var track in CONFIG.NOTETRACKS)
            {
                if (ValidateTrack(track.Item1))
                {
                    _instrument = track.Item2;
                    return true;
                }
            }
            return false;
        }

        private bool ValidateTrack(ReadOnlySpan<TChar> track)
        {
            if (!DoesStringMatch(track))
                return false;

            reader.GotoNextLine();
            return true;
        }

        private bool DoesStringMatch(ReadOnlySpan<TChar> str)
        {
            if (reader.Next - reader.Position < str.Length)
                return false;
            return reader.PeekBasicSpan(str.Length).SequenceEqual(str);
        }

        public bool IsStillCurrentTrack()
        {
            int position = reader.Position;
            if (position == reader.Length)
                return false;

            if (reader.IsCurrentCharacter('}'))
            {
                reader.GotoNextLine();
                return false;
            }

            return true;
        }

        public bool TryParseEvent(ref DotChartEvent ev)
        {
            if (!IsStillCurrentTrack())
                return false;

            ev.Position = YARGNumberExtractor.Int64(reader);

            int start = reader.Position;
            int end = start;
            while (true)
            {
                char curr = reader.Data[end].ToChar(null);
                if (!curr.IsAsciiLetter())
                    break;
                ++end;
            }
            reader.Position = end;

            ReadOnlySpan<TChar> span = new(reader.Data, start, end - start);
            foreach (var combo in eventSet)
            {
                if (combo.DoesEventMatch(span))
                {
                    reader.SkipWhiteSpace();
                    ev.Type = combo.eventType;
                    return true;
                }
            }

            ev.Type = ChartEventType.Unknown;
            return true;
        }

        public void SkipEvent()
        {
            reader.GotoNextLine();
        }

        public void NextEvent()
        {
            reader.GotoNextLine();
        }

        public void ExtractLaneAndSustain(ref DotChartNote note)
        {
            note.Lane = YARGNumberExtractor.Int32(reader);
            note.Duration = YARGNumberExtractor.Int64(reader);
        }

        public void SkipTrack()
        {
            reader.GotoNextLine();
            int position = reader.Position;
            while (GetDistanceToTrackCharacter(position, out int next))
            {
                int point = position + next - 1;
                while (point > position)
                {
                    char character = reader.Data[point].ToChar(null);
                    if (!character.IsAsciiWhitespace() || character == '\n')
                        break;
                    --point;
                }

                if (reader.Data[point].ToChar(null) == '\n')
                {
                    reader.Position = position + next;
                    reader.SetNextPointer();
                    reader.GotoNextLine();
                    return;
                }

                position += next + 1;
            }

            reader.Position = reader.Length;
            reader.SetNextPointer();
        }

        private bool GetDistanceToTrackCharacter(int position, out int i)
        {
            int distanceToEnd = reader.Length - position;
            i = 0;
            while (i < distanceToEnd)
            {
                if (reader.Data[position + i].ToChar(null) == '}')
                    return true;
                ++i;
            }
            return false;
        }

        public Dictionary<string, List<IniModifier>> ExtractModifiers<TDecoder>(TDecoder decoder, Dictionary<string, IniModifierCreator> validNodes)
            where TDecoder : StringDecoder<TChar>
        {
            Dictionary<string, List<IniModifier>> modifiers = new();
            while (IsStillCurrentTrack())
            {
                string name = decoder.ExtractModifierName(reader);
                if (validNodes.TryGetValue(name, out var node))
                {
                    var mod = node.CreateModifier(reader, decoder);
                    if (modifiers.TryGetValue(node.outputName, out var list))
                        list.Add(mod);
                    else
                        modifiers.Add(node.outputName, new() { mod });
                }
                reader.GotoNextLine();
            }
            return modifiers;
        }
    }
}
