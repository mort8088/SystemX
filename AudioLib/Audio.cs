// -----------------------------------------------------------------------
// <copyright file="Audio.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using SystemX.Config;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SystemX.AudioLib {
    public static class Audio {
        public const string MusicPath = @"Audio\Music\";
        public const string SfxPath = @"Audio\SFX\";
        private static I_Config _settings;
        private static readonly Dictionary<string, SoundEffect> Sfx = new Dictionary<string, SoundEffect>();
        private static readonly Dictionary<string, List<Song>> Playlists = new Dictionary<string, List<Song>>();
        private static List<Song> _curPlaylist;
        private static int _curTrack;
        private static readonly List<SoundEffectInstance> Playing = new List<SoundEffectInstance>();
        private static readonly List<SoundEffectInstance> Toremove = new List<SoundEffectInstance>();
        private static ContentManager _content;
        private static float _musicVol;
        private static float _sfxVol;
        private static bool _isPlaying = true;

        public static void Initialize(IServiceProvider services) {
            _content = new ContentManager(services, "Content");
            _settings = (I_Config)services.GetService(typeof(I_Config));

            Update();
        }

        public static bool RegisterSfx(string name) {
            if (string.IsNullOrEmpty(name))
                return false;

            try {
                if (Sfx.ContainsKey(name))
                    return true;

                SoundEffect m = _content.Load<SoundEffect>(SfxPath + name);
                Sfx.Add(name, m);

                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public static SoundEffectInstance PlaySfx(string name) {
            if (Sfx.ContainsKey(name)) {
                SoundEffectInstance inst = Sfx[name].CreateInstance();
                inst.Volume = _sfxVol;
                Playing.Add(inst);
                try {
                    inst.Play();
                    if (!_isPlaying)
                        inst.Pause();
                }
                catch (InstancePlayLimitException) {
                    Playing.Remove(inst);
                }

                return inst;
            }

            return null;
        }

        public static bool RegisterPlayList(string name, ref List<string> playlist) {
            try {
                if (Playlists.ContainsKey(name))
                    return false;

                List<Song> pl = new List<Song>(playlist.Count);

                pl.AddRange(playlist.Select(t => _content.Load<Song>(MusicPath + t)));

                Playlists.Add(name, pl);

                return true;
            }
#if DEBUG
            catch (Exception ex) {
                WindowManager.Interaction.MsgBox(string.Format("Audio.RegisterPlayList - \n{0}", ex.Message));
#else
            catch
            {
#endif
                return false;
            }
        }

        public static void PlayPlaylist(string name, bool immediate) {
            if (!Playlists.ContainsKey(name) ||
                !MediaPlayer.GameHasControl)
                return;

            if (immediate)
                StopPlaylist();

            _curPlaylist = Playlists[name];
            _curTrack = _curPlaylist.Count;
        }

        public static void StopPlaylist() {
            MediaPlayer.Stop();
            _curPlaylist = null;
        }

        public static void Update() {
            bool sfxChanged = false;
            if (_settings["Audio"]["MusicVolume"].GetValueAsFloat() != _musicVol) {
                _musicVol = _settings["Audio"]["MusicVolume"].GetValueAsFloat();

                MediaPlayer.Volume = _musicVol;
            }

            if ((_musicVol == 0) ||
                (_settings["Audio"]["Mute"].GetValueAsBool()))
                MediaPlayer.IsMuted = true;
            else
                MediaPlayer.IsMuted = false;

            if (_settings["Audio"]["SfxVolume"].GetValueAsFloat() != _sfxVol) {
                _sfxVol = _settings["Audio"]["SfxVolume"].GetValueAsFloat();
                sfxChanged = true;
            }

            foreach (SoundEffectInstance effect in Playing) {
                if (effect.State == SoundState.Stopped) Toremove.Add(effect);
                else if (sfxChanged) effect.Volume = _sfxVol;
            }

            foreach (SoundEffectInstance effect in Toremove) Playing.Remove(effect);

            Toremove.Clear();

            if (!MediaPlayer.GameHasControl ||
                (MediaPlayer.State != MediaState.Paused && MediaPlayer.State != MediaState.Stopped) ||
                _curPlaylist == null ||
                _curPlaylist.Count <= 0 ||
                !_isPlaying)
                return;

            _curTrack = (_curTrack + 1) % _curPlaylist.Count;
            MediaPlayer.Play(_curPlaylist[_curTrack]);
        }

        public static void PauseAll() {
            foreach (SoundEffectInstance effect in Playing) effect.Pause();

            _isPlaying = false;
        }

        public static void ResumeAll() {
            foreach (SoundEffectInstance effect in Playing) effect.Resume();

            _isPlaying = true;
        }

        public static void UnloadContent() {
            if (_content != null)
                _content.Unload();

            Sfx.Clear();
            Playlists.Clear();
        }

        public static void KillAllSfx() {
            foreach (SoundEffectInstance effect in Playing) effect.Stop(true);
        }
    }
}