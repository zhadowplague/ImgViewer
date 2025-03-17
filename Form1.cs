using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImgViewer
{
	public partial class Form1 : Form
	{
		string _path;
		float _zoom = 1;
		float _zoomFactor = 0.1f;
		bool _slowZoom = false;
		bool _mouseDown = false;
		bool _filter = false;
		PointF _offset = PointF.Empty;
		Point _position = Point.Empty;
		DateTime _lastClick;
		GifGaffer _gifGaffer;

		public Form1(string[] args)
		{
			InitializeComponent();
			if (args.Length < 2)
			{
				Application.Exit();
				return;
			}
			_path = args[1];
			_gifGaffer = new GifGaffer(pictureBox1);
			LoadImg();

			pictureBox1.MouseWheel += PictureBox1_MouseWheel;
			pictureBox1.Paint += PictureBox1_Paint;
			KeyUp += Form1_KeyUp;
			pictureBox1.MouseDown += Form1_MouseDown;
			MouseDown += Form1_MouseDown;
			pictureBox1.MouseUp += Form1_MouseUp;
			MouseUp += Form1_MouseUp;
			pictureBox1.MouseMove += Form1_MouseMove;
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_mouseDown || _position ==  Point.Empty)
			{
				_position = e.Location;
				return;
			}

			_offset.X += e.Location.X - _position.X;
			_offset.Y += e.Location.Y - _position.Y;
			_position = e.Location;
			pictureBox1.Invalidate();
		}

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			if (DateTime.UtcNow.Subtract(_lastClick).TotalSeconds < 0.2)
			{
				if (e.Location.X < Size.Width * 0.15)
				{
					PreviousImg();
				}
				else if (e.Location.X > Size.Width * 0.85)
				{
					NextImg();
				}
			}
			_mouseDown = false;
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			_mouseDown = true;
			_lastClick = DateTime.UtcNow;
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
			{
				_slowZoom = false;
			}
		}

		private float Clamp(float target, float min, float max) => Math.Min(max, Math.Max(min, target));

		private void PictureBox1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			e.Graphics.InterpolationMode = _filter ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			var width = pictureBox1.Image.Size.Width * _zoom;
			var height = pictureBox1.Image.Size.Height * _zoom;
			_offset.X = Clamp(_offset.X, -width * 0.5f, width * 0.5f);
			_offset.Y = Clamp(_offset.Y, -height * 0.5f, height * 0.5f);
			var x = pictureBox1.Size.Width * 0.5f - (width * 0.5f) + _offset.X;
			var y = pictureBox1.Size.Height * 0.5f - (height * 0.5f) + _offset.Y;
			e.Graphics.DrawImage(pictureBox1.Image, new RectangleF(x, y, width, height));
		}

		private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			var zoomSpeed = _slowZoom ? _zoomFactor * 0.1f : _zoomFactor;
			SetZoom(_zoom + (e.Delta * zoomSpeed), e.Location);
		}

		public void SetZoom(float zoomfactor, Point mousePos)
		{
			if (zoomfactor <= 0) 
				return;

			float oldZoom = _zoom;
			_zoom = zoomfactor;

			var imageX = (mousePos.X - pictureBox1.Width * 0.5f + _offset.X) / oldZoom;
			var imageY = (mousePos.Y - pictureBox1.Height * 0.5f + _offset.Y) / oldZoom;
			_offset.X += (imageX * (oldZoom - _zoom));
			_offset.Y += (imageY * (oldZoom - _zoom));

			pictureBox1.Invalidate();
		}

		private void LoadImg()
		{
			_gifGaffer.Stop();
			_offset = PointF.Empty;
			try
			{
				pictureBox1.Image = Image.FromFile(_path);
				var imgMax = (float)Math.Max(pictureBox1.Image.Width, pictureBox1.Image.Height);
				var windowMax = (float)Math.Max(pictureBox1.Size.Width, pictureBox1.Size.Height);
				_zoomFactor = 1.0f / imgMax;
				_zoom = (windowMax * 0.5f) / imgMax;
				pictureBox1.Invalidate();
				Text = Path.GetFileName(_path);	
			}
			catch
			{
				MessageBox.Show($"Failed to load file {_path}");
			}
		}

		string[] GetFilesInFolder()
		{
			var directory = Path.GetDirectoryName(_path);
			var files = Directory.GetFiles(directory);
			Array.Sort(files, new NaturalStringComparer());
			return files;
		}

		string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif", ".bmp" };
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
			{
				NextImg();
			}
			else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
			{
				PreviousImg();
			}
			else if (e.KeyCode == Keys.F)
			{
				_filter = !_filter;
			}
			else if (e.KeyCode == Keys.ControlKey)
			{
				_slowZoom = true;
			}
			else if (e.KeyCode == Keys.S)
			{
				_gifGaffer.ToggleSpeed();
			}
			else if (e.KeyCode == Keys.W)
			{
				_gifGaffer.Step();
			}
		}

		private void PreviousImg()
		{
			var files = GetFilesInFolder();
			var index = Array.IndexOf(files, _path);
			while (index < files.Length - 1)
			{
				index++;
				var extension = Path.GetExtension(files[index]);
				if (!allowedExtensions.Contains(extension))
					continue;
				_path = files[index];
				LoadImg();
				break;
			}
		}

		private void NextImg()
		{
			var files = GetFilesInFolder();
			var index = Array.IndexOf(files, _path);
			while (index > 0)
			{
				index--;
				var extension = Path.GetExtension(files[index]);
				if (!allowedExtensions.Contains(extension))
					continue;
				_path = files[index];
				LoadImg();
				break;
			}
		}
	}
}
