﻿using NUnit.Framework;
using YARG.Core.Song.Cache;

namespace YARG.Core.UnitTests.Scanning
{
    public class SongScanningTests
    {
        private string[] songDirectories;
        private readonly bool MULTITHREADING = true;
        private static readonly string SongCachePath = Path.Combine(Environment.CurrentDirectory, "songcache.bin");
        private static readonly string BadSongsPath = Path.Combine(Environment.CurrentDirectory, "badsongs.txt");

        [SetUp]
        public void Setup()
        {
            List<string> directories = new()
            {
                
            };       
            Assert.That(directories, Is.Not.Empty, "Add directories to scan for the test");
            songDirectories = directories.ToArray();
        }

        [TestCase]
        public void FullScan()
        {
            YargTrace.AddListener(new YargDebugTraceListener());
            CacheHandler handler = new(SongCachePath, BadSongsPath, MULTITHREADING, songDirectories);

            var cache = handler.RunScan(false);
            // TODO: Any cache properties we want to check here?
            // Currently the only fail condition would be an unhandled exception
        }

        [TestCase]
        public void QuickScan()
        {
            YargTrace.AddListener(new YargDebugTraceListener());
            CacheHandler handler = new(SongCachePath, BadSongsPath, MULTITHREADING, songDirectories);

            var cache = handler.RunScan(true);
            // TODO: see above
        }
    }
}
