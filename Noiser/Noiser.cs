using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Noiser
{
    public class Noiser
    {
        private Logger _logger;
        private Random _rnd;
        private TimeBank _timeBank;
        private List<Track> _tracks;

        public Noiser()
        {
            _logger = new Logger();
            _rnd = new Random();
            _timeBank = new TimeBank();
            _tracks = LoadTracks();
        }

        private List<Track> LoadTracks()
        {
            var tracks = new List<Track>();
            var d = new DirectoryInfo(@"../../Tracks"); // move to config later...
            var files = d.GetFiles("*.mp3");
            if (files.Count() == 0) throw new Exception("No tracks found.");
            foreach (var file in files)
            {
                string path = file.FullName;
                using (var reader = new Mp3FileReader(path))
                {
                    int duration = (int)reader.TotalTime.TotalSeconds;
                    tracks.Add(new Track { Path = path, LengthInSeconds = duration });
                }
            }
            return tracks;
        }

        private int CalcLength(int proposedSpan, int remainingTrackLength) => proposedSpan < remainingTrackLength ? proposedSpan : remainingTrackLength;

        private void Play(Track track, int start, int length)
        {
            var file = new AudioFileReader(track.Path);
            var trimmed = new OffsetSampleProvider(file);
            trimmed.SkipOver = TimeSpan.FromSeconds(start);
            trimmed.Take = TimeSpan.FromSeconds(length);

            var player = new WaveOutEvent();
            if (track.Path.Contains("ReallyLoudBass.mp3")) player.Volume = 0.25F;
            else player.Volume = 1.0F;
            player.Init(trimmed);
            player.Play();
            Thread.Sleep(length * 1000);
            player.Dispose();
            file.Close();
        }

        private void Work()
        {
            var track = _tracks[_rnd.Next(0, _tracks.Count())];
            var start = _rnd.Next() % track.LengthInSeconds;
            var length = CalcLength(_rnd.Next(5, 60), track.LengthInSeconds - start); // minute
            _timeBank.AddTime(DateTime.Now, length);
            _logger.LogStart();
            DiagLog(track, start, length);
            Play(track, start, length);
            _logger.LogEnd();
        }

        private void DiagLog(Track track, int start, int length)
        {
            _logger.LogInfo($"Track: {track.Path}, Offset: {start}s, Duration: {length}s");
            string playHours = _timeBank.IsWithinOkHours() ? "Yes" : "No";
            _logger.LogInfo($"TimeBank remaining: {_timeBank.GetTimeAvailable()}s. Is within playing hours? {playHours}");
        }

        public void Run()
        {
            while (true)
            {
                var waitTime = _timeBank.GenerateWaitTime(_rnd);
                _logger.LogInfo($"Will wait for: {waitTime / 1000}s");
                Thread.Sleep(waitTime);
                if (!_timeBank.IsTimeAvailable() || _rnd.Next(0, 100) > _timeBank.GetTimeAvailablePercentage() * 2) continue;
                Work();
            }
        }
    }
}
