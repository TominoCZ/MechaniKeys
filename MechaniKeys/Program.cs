using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MechaniKeys
{
    internal static class Program
    {
        static SoundPlayer _player = new SoundPlayer();
        static Random _rand = new Random();

        static int _volume = 30;

        [STAThread]
        static void Main()
        {
            var listener = new KeyboardListener();
            var last = string.Empty;

            var down = new HashSet<Key>();

            listener.KeyDown += (_, e) =>
            {
                if (down.Contains(e.Key))
                    return;

                down.Add(e.Key);

                last = e.Character;

                PlaySound(e.Key, e.Character, e.IsSysKey);
            };
            listener.KeyUp += (_, e) =>
            {
                down.Remove(e.Key);
            };

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (File.Exists("volume.txt"))
                        {
                            var vol = int.Parse(File.ReadAllText("volume.txt").Replace(" ", ""));

                            _volume = Math.Min(100, Math.Max(0, vol));
                        }
                        else
                        {
                            File.WriteAllText("volume.txt", $"{_volume}");
                        }
                    }
                    catch
                    {

                    }

                    Thread.Sleep(1000);
                }
            }).Start();

            Application.ThreadExit += (_, __) =>
            {
                listener.Dispose();
            };
            Application.Run();
        }

        static void PlaySound(Key key, string str, bool sys)
        {
            var vol = _volume / 100f;

            switch (key)
            {
                case Key.Back:
                    _player.Play("backspace", vol);
                    break;
                case Key.Space:
                    _player.Play("space", vol);
                    break;
                case Key.Enter:
                    _player.Play("enter", vol);
                    break;
                default:
                    if (str.Length != 1 || sys)
                        return;

                    var id = _player.KeyTaps[_rand.Next(_player.KeyTaps.Count)];

                    _player.Play(id, vol);
                    break;
            }
        }
    }
}
