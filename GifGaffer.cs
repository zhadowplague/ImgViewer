using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace ImgViewer
{
	internal class GifGaffer
	{
		List<Image> _frames = new List<Image>();
		PictureBox _target;
		Speeds _speed;
		Timer _timer;

		enum Speeds
		{
			Slow, Medium, Fast, VeryFast
		}

		public void ToggleSpeed()
		{
			Start();
			switch (_speed)
			{
				case Speeds.Slow:
					_speed = Speeds.Medium;
					break;
				case Speeds.Medium:
					_speed = Speeds.Fast;
					break;
				case Speeds.Fast:
					_speed = Speeds.VeryFast;
					break;
				case Speeds.VeryFast:
					_speed = Speeds.Slow;
					break;
			}
			_timer.Interval = GetInterval();
			if (_frames.Count > 0)
				_timer.Start();
		}

		int GetInterval()
		{
			switch (_speed)
			{
				case Speeds.Slow:
					return 500;
				case Speeds.Medium:
					return 250;
				case Speeds.Fast:
					return 100;
					case Speeds.VeryFast:
					return 33;
			}
			return 0;
		}

		public GifGaffer(PictureBox pictureBox)
		{
			_timer = new Timer();
			_timer.Interval = GetInterval();
			_timer.Tick += Tick;
			_target = pictureBox;
		}

		public void Start()
		{
			if (_frames.Count > 0)
				return;

			_speed = Speeds.Slow;
			foreach (var dimension in _target.Image.FrameDimensionsList.Select(x => new FrameDimension(x)))
			{
				for (int i = 0; i < _target.Image.GetFrameCount(dimension); i++)
				{
					_target.Image.SelectActiveFrame(dimension, i);
					_frames.Add(new Bitmap(_target.Image));
				}
			}
			_timer.Start();
		}

		public void Step()
		{
			Start();
			_timer.Stop();
			Tick(null, null);
		}

		public void Stop()
		{
			foreach (var frame in _frames)
				frame.Dispose();
			_frames.Clear();
			_currentFrame = 0;
			_timer.Stop();
		}

		int _currentFrame = 0;
		private void Tick(object sender, EventArgs e)
		{
			_target.Image = _frames[_currentFrame];
			_currentFrame = (_currentFrame + 1) % _frames.Count;
		}
	}
}
