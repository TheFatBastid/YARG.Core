//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using NUnit.Framework;
using YARG.Core.Chart;

#nullable enable

namespace YARG.Core.UnitTests.Chart;

public class ChartEventTrackerTests
{
    private List<Beatline> _events =
    [
        new(BeatlineType.Measure, 0.5 * 0, 480 * 0),
        new(BeatlineType.Strong,  0.5 * 1, 480 * 1),
        new(BeatlineType.Strong,  0.5 * 2, 480 * 2),
        new(BeatlineType.Strong,  0.5 * 3, 480 * 3),

        new(BeatlineType.Measure, 0.5 * 4, 480 * 4),
        new(BeatlineType.Strong,  0.5 * 5, 480 * 5),
        new(BeatlineType.Strong,  0.5 * 6, 480 * 6),
        new(BeatlineType.Strong,  0.5 * 7, 480 * 7),

        new(BeatlineType.Measure, 0.5 * 8,  480 * 8),
        new(BeatlineType.Strong,  0.5 * 9,  480 * 9),
        new(BeatlineType.Strong,  0.5 * 10, 480 * 10),
        new(BeatlineType.Strong,  0.5 * 11, 480 * 11),

        new(BeatlineType.Measure, 0.5 * 12, 480 * 12),
        new(BeatlineType.Strong,  0.5 * 13, 480 * 13),
        new(BeatlineType.Strong,  0.5 * 14, 480 * 14),
        new(BeatlineType.Strong,  0.5 * 15, 480 * 15),
    ];

    [Test]
    public void Update_Tick()
    {
        void TestUpdate(uint increment)
        {
            var tracker = new ChartEventTickTracker<Beatline>(_events);

            int expectedIndex = -1;
            for (uint tick = 0; tick < _events[^1].Tick; tick += increment)
            {
                // Updates must occur when reaching or exceeding the tick of a new event
                bool updateExpected = false;
                while (expectedIndex + 1 < _events.Count && _events[expectedIndex + 1].Tick <= tick)
                {
                    expectedIndex++;
                    updateExpected = true;
                }

                bool updated = tracker.Update(tick);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.EqualTo(updateExpected));
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                });

                // No more updates should occur until the next event once this update has run
                updated = tracker.Update(tick);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.False);
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                });
            }
        }

        TestUpdate(100);
        TestUpdate(100 * 10);
    }

    [Test]
    public void UpdateOnce_Tick()
    {
        void TestUpdate(uint increment)
        {
            var tracker = new ChartEventTickTracker<Beatline>(_events);

            int expectedIndex = -1;
            for (uint tick = 0; tick < _events[^1].Tick; tick += increment)
            {
                Beatline? current;
                bool updated;

                // Updates must occur when reaching or exceeding the tick of a new event
                while (expectedIndex + 1 < _events.Count && _events[expectedIndex + 1].Tick <= tick)
                {
                    expectedIndex++;

                    updated = tracker.UpdateOnce(tick, out current);
                    Assert.Multiple(() =>
                    {
                        Assert.That(updated, Is.True);
                        Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                        Assert.That(tracker.Current, Is.EqualTo(_events[expectedIndex]));
                        Assert.That(current, Is.EqualTo(tracker.Current));
                    });
                }

                // No more updates should occur until the next event once all available updates are run
                updated = tracker.UpdateOnce(tick, out current);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.False);
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                    Assert.That(current, Is.EqualTo(tracker.Current));
                });
            }
        }

        TestUpdate(100);
        TestUpdate(100 * 10);
    }

    [Test]
    public void Reset_Tick()
    {
        var tracker = new ChartEventTickTracker<Beatline>(_events);

        // Update to end to ensure proper reset functionality
        bool updated = tracker.Update(_events[2].Tick);
        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.True);
            Assert.That(tracker.CurrentIndex, Is.EqualTo(2));
            Assert.That(tracker.Current, Is.EqualTo(_events[2]));
        });

        // Resetting fully nullifies the current event
        tracker.Reset();
        Assert.Multiple(() =>
        {
            Assert.That(tracker.CurrentIndex, Is.EqualTo(-1));
            Assert.That(tracker.Current, Is.Null);
        });

        // Resetting to the tick of an event should result in that event being current
        for (int i = 0; i < _events.Count; i++)
        {
            tracker.ResetToTick(_events[i].Tick);
            Assert.Multiple(() =>
            {
                Assert.That(tracker.CurrentIndex, Is.EqualTo(i));
                Assert.That(tracker.Current, Is.EqualTo(_events[i]));
            });
        }
    }

    [Test]
    public void Update_Time()
    {
        void TestUpdate(double increment)
        {
            var tracker = new ChartEventTimeTracker<Beatline>(_events);

            int expectedIndex = -1;
            for (double time = 0; time < _events[^1].Time; time += increment)
            {
                // Updates must occur when reaching or exceeding the time of a new event
                bool updateExpected = false;
                while (expectedIndex + 1 < _events.Count && _events[expectedIndex + 1].Time <= time)
                {
                    expectedIndex++;
                    updateExpected = true;
                }

                bool updated = tracker.Update(time);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.EqualTo(updateExpected));
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                });

                // No more updates should occur until the next event once this update has run
                updated = tracker.Update(time);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.False);
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                });
            }
        }

        TestUpdate(0.1);
        TestUpdate(0.1 * 10);
    }

    [Test]
    public void UpdateOnce_Time()
    {
        void TestUpdate(double increment)
        {
            var tracker = new ChartEventTimeTracker<Beatline>(_events);

            int expectedIndex = -1;
            for (double time = 0; time < _events[^1].Time; time += increment)
            {
                Beatline? current;
                bool updated;

                // Updates must occur when reaching or exceeding the time of a new event
                while (expectedIndex + 1 < _events.Count && _events[expectedIndex + 1].Time <= time)
                {
                    expectedIndex++;

                    updated = tracker.UpdateOnce(time, out current);
                    Assert.Multiple(() =>
                    {
                        Assert.That(updated, Is.True);
                        Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                        Assert.That(tracker.Current, Is.EqualTo(_events[expectedIndex]));
                        Assert.That(current, Is.EqualTo(tracker.Current));
                    });
                }

                // No more updates should occur until the next event once all available updates are run
                updated = tracker.UpdateOnce(time, out current);
                Assert.Multiple(() =>
                {
                    Assert.That(updated, Is.False);
                    Assert.That(tracker.CurrentIndex, Is.EqualTo(expectedIndex));
                    Assert.That(tracker.Current, Is.EqualTo(expectedIndex >= 0 ? _events[expectedIndex] : null));
                    Assert.That(current, Is.EqualTo(tracker.Current));
                });
            }
        }

        TestUpdate(0.1);
        TestUpdate(0.1 * 10);
    }

    [Test]
    public void Reset_Time()
    {
        var tracker = new ChartEventTimeTracker<Beatline>(_events);

        // Update to end to ensure proper reset functionality
        bool updated = tracker.Update(_events[2].Time);
        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.True);
            Assert.That(tracker.CurrentIndex, Is.EqualTo(2));
            Assert.That(tracker.Current, Is.EqualTo(_events[2]));
        });

        // Resetting fully nullifies the current event
        tracker.Reset();
        Assert.Multiple(() =>
        {
            Assert.That(tracker.CurrentIndex, Is.EqualTo(-1));
            Assert.That(tracker.Current, Is.Null);
        });

        // Resetting to the time of an event should result in that event being current
        for (int i = 0; i < _events.Count; i++)
        {
            tracker.ResetToTime(_events[i].Time);
            Assert.Multiple(() =>
            {
                Assert.That(tracker.CurrentIndex, Is.EqualTo(i));
                Assert.That(tracker.Current, Is.EqualTo(_events[i]));
            });
        }
    }
}