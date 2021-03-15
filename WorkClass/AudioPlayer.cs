using NAudio.Wave;
using NLayer.NAudioSupport;
using SKYNET;
using SkynetDota2Mods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

internal class AudioPlayer
{
    private static WaveOutEvent heroPlayer;
    public static WaveOutEvent soundPlayer;

    private static WaveStream waveStream;
    public static bool Playing { get; internal set; }
    static AudioPlayer()
    {
        heroPlayer = new WaveOutEvent();
        soundPlayer = new WaveOutEvent();
        soundPlayer.PlaybackStopped += SoundPlayer_PlaybackStopped;
    }

    private static void SoundPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
        frmMain.frm.stopSounds.Visible = false;

    }

    public static void StopSounds()
    {
        heroPlayer.Stop();
        soundPlayer.Stop();
        Playing = false;
    }

    public static void PlayHeroName(string HeroName)
    {
        if (!Settings.ActiveSounds)
            return;
        HeroName = HeroName.Replace("npc_dota_hero_", "");
        string[] names = HeroName.Split('_');
        HeroName = names[0];

        if (HeroName.ToLower() == "pangolier")
            HeroName = "pangoli";

        if (modCommon.package != null)
        {
            try
            {
                try
                {
                    List<PackageEntry> list = (from l in modCommon.package.Entries["vsnd_c"]
                                               where l.DirectoryName.Contains("sounds/vo/" + HeroName)
                                               select l).ToList();
                    if (list.Count == 0)
                        list = (from l in modCommon.package.Entries["vsnd_c"]
                                                   where (l.DirectoryName.Contains("sounds/vo/") && l.DirectoryName.Contains(names[1]))
                                                   select l).ToList();

                    if (list.Any())
                    {

                        PackageEntry entry = null;
                        if (names.Count() > 1)
                        {
                            entry = list.Find(x => x.FileName.Contains("_spawn_01") && x.DirectoryName.Contains(names[1]));

                            if (entry == null)
                                entry = list.Find(x => x.FileName.Contains("spawn") && x.DirectoryName.Contains(names[1]));

                            if (entry == null)
                                entry = list.Find(x => x.FileName.Contains("level") && x.DirectoryName.Contains(names[1]));

                            if (entry != null)
                                Play(entry, heroPlayer, false);
                        }
                        else
                        {
                            entry = list.Find(x => x.FileName.Contains("_spawn_01"));

                            if (entry == null)
                                entry = list.Find(x => x.FileName.Contains("spawn"));

                            if (entry == null)
                                entry = list.Find(x => x.FileName.Contains("level"));

                            if (entry != null)
                                Play(entry, heroPlayer, false);
                        }
                    }
                }
                finally
                {
                    ((IDisposable)modCommon.package)?.Dispose();
                }
            }
            catch (Exception ex)
            {

                return;
            }
        }


    }
    //                    
    public static void PlaySound(string filePath, bool toStop, int x = 0)
    {
        if (!Settings.ActiveSounds)
            return;

        if (modCommon.package != null)
        {
            try
            {
                PackageEntry item = modCommon.package.FindEntry(filePath + "_c");
                if (item == null)
                {
                    string path = Path.GetDirectoryName(filePath);
                    path = path.Replace(@"\", "/");

                    List<PackageEntry> list = (from l in modCommon.package.Entries["vsnd_c"]
                                               where l.DirectoryName.Contains(path.ToLower())
                                               select l).ToList();

                    if (list.Any())
                    {
                        if (filePath.ToLower().Contains("announcer"))
                        {
                            bool used = false;
                            foreach (PackageEntry ann in list)
                            {
                                if (ann.FileName.ToLower().Contains("welcome_01"))
                                {
                                    used = true;
                                    Play(ann, soundPlayer, toStop, x);
                                }
                            }
                            if (!used)
                            {
                                foreach (PackageEntry ann in list)
                                {
                                    if (ann.FileName.ToLower().Contains("rampage_01"))
                                    {
                                        Play(ann, soundPlayer, toStop, x);
                                        return;
                                    }
                                }
                            }
                        }
                        //
                    }
                }
                else
                {
                    Play(item, soundPlayer, toStop, x);
                }
            }
            finally
            {

            }

        }


    }



    public static void Play(PackageEntry entry, WaveOutEvent waveOut, bool toStop, int x = 0)
    {
        //modCommon.Show(entry.GetFullPath());

        if (toStop)
        {
            frmMain.frm.stopSounds.Visible = true;
            frmMain.frm.stopSounds.Location = new Point(x, frmMain.frm.stopSounds.Location.Y);
        }

        modCommon.package.ReadEntry(entry, out byte[] output);

        Resource resource = new Resource();
        resource.Read(new MemoryStream(output));

        Sound sound = (Sound)resource.Blocks[BlockType.DATA];
        MemoryStream soundStream = sound.GetSoundStream();
        //waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        waveOut.Volume = 0.5f;
        switch (sound.Type)
        {
            case Sound.AudioFileType.WAV:
                waveStream = new WaveFileReader(soundStream);
                break;
            case Sound.AudioFileType.MP3:
                waveStream = new Mp3FileReader(soundStream, (WaveFormat wf) => new Mp3FrameDecompressor(wf));
                break;
            case Sound.AudioFileType.AAC:
                waveStream = new StreamMediaFoundationReader(soundStream);
                break;
        }
        waveOut.Stop();
        waveOut.Init(waveStream);
        waveOut.Play();

    }
}
