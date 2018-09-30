﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using NUnit.Framework;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class MediaPlayerTests : BaseSetup
    {
        [Test]
        public void CreateAndDestroy()
        {
            var mp = new MediaPlayer(new LibVLC());
            mp.Dispose();
            Assert.AreEqual(IntPtr.Zero, mp.NativeReference);
        }

        [Test]
        public void OutputDeviceEnum()
        {
            var mp = new MediaPlayer(new LibVLC());
            var t = mp.OutputDeviceEnum;
            Debug.WriteLine(t);
        }

        string RealMediaPath
        {
            get
            {
                var dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
           //     var binDir = Path.Combine(dir, "..\\..\\..\\");
                var files = Directory.GetFiles(dir);
                return files.First();
            }
        }

        [Test]
        public async Task TrackDescription()
        {
            var libVLC = new LibVLC();
            var mp = new MediaPlayer(libVLC);
            var media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var tcs = new TaskCompletionSource<bool>();
            
            mp.Media = media;
            mp.Play();
            mp.Playing += (sender, args) =>
            {
                Assert.Zero(mp.AudioTrack);
                var description = mp.AudioTrackDescription;
                Assert.True(mp.SetAudioTrack(description.First().Id));
                Assert.IsNotEmpty(description);
                tcs.SetResult(true);
            };
            await tcs.Task;
            Assert.True(tcs.Task.Result);
        }

        [Test]
        public async Task Play()
        {
            var libVLC = new LibVLC();
            var media = new Media(libVLC, "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4", Media.FromType.FromLocation);
            var mp = new MediaPlayer(media);
            var called = false;
            mp.Playing += (sender, args) =>
            {
                called = true;
            };
            mp.Play();
            await Task.Delay(5000);
            Assert.True(called);
            Assert.True(mp.IsPlaying);
        }
    }
}
