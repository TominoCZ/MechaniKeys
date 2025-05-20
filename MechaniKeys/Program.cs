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
            listener.KeyUp += (_, e) => { down.Remove(e.Key); };

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

            Application.ThreadExit += (_, __) => { listener.Dispose(); };
            Application.Run();
        }

        static void PlaySound(Key key, string str, bool sys)
        {
            var vol = _volume / 100f;
            var pitch = 1f;

            var caps = str.Length == 1 && str.ToLower() != str;
            if (caps)
                vol += 0.4f;
            
            switch (key)
            {
                case Key.Back:
                    _player.Play("backspace", vol, pitch);
                    break;
                case Key.Space:
                    _player.Play("space", vol, pitch);
                    break;
                case Key.Enter:
                    _player.Play("enter", vol + 0.35f, pitch);
                    break;
                case Key.Insert:
                case Key.Capital:
                    _player.Play("space", vol + 0.35f, pitch);
                    break;
                case Key.Delete:
                    _player.Play("backspace", vol + 0.35f, pitch);
                    break;
                case Key.RightAlt:
                    _player.PlayKeyTap(vol + 0.35f, pitch);
                    break;
                case Key.LeftAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.RightShift:
                case Key.LeftShift:
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                case Key.PageUp:
                case Key.PageDown:
                    _player.PlayKeyTap(vol, pitch);
                    break;
                default:
                    if (str.Length != 1 || sys)
                        return;
                    _player.PlayKeyTap(vol, pitch);
                    break;
            }
        }
    }
}