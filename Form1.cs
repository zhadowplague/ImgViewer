using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgViewer
{
	public partial class Form1 : Form
	{
		string _path;
		float _zoom = 1;
		float _zoomFactor = 0.1f;
		bool _slowZoom = false;

		public Form1(string[] args)
		{
			InitializeComponent();
			if (args.Length < 2)
			{
				Application.Exit();
				return;
			}
			_path = args[1];
			LoadImg();

			pictureBox1.MouseWheel += PictureBox1_MouseWheel;
			pictureBox1.Paint += PictureBox1_Paint;
			this.KeyUp += Form1_KeyUp;
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
			{
				_slowZoom = false;
			}
		}

		private void PictureBox1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			var width = pictureBox1.Image.Size.Width * _zoom;
			var height = pictureBox1.Image.Size.Height * _zoom;
			var x = pictureBox1.Size.Width * 0.5f - (width * 0.5f);
			var y = pictureBox1.Size.Height * 0.5f - (height * 0.5f);
			e.Graphics.DrawImage(pictureBox1.Image, new RectangleF(x, y, width, height));
		}

		private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			var zoomSpeed = _slowZoom ? _zoomFactor * 0.1f : _zoomFactor;
			SetZoom(_zoom + (e.Delta * zoomSpeed));
		}

		public void SetZoom(float zoomfactor)
		{
			if (zoomfactor <= 0) 
				return;
			_zoom = zoomfactor;
			pictureBox1.Invalidate();
		}

		private void LoadImg()
		{
			try
			{
				pictureBox1.Image = Image.FromFile(_path);
				var imgMax = (float)Math.Max(pictureBox1.Image.Width, pictureBox1.Image.Height);
				var windowMax = (float)Math.Max(pictureBox1.Size.Width, pictureBox1.Size.Height);
				_zoomFactor = 1.0f / imgMax;
				SetZoom((windowMax * 0.5f) / imgMax);
			}
			catch 
			{
				MessageBox.Show($"Failed to load file {_path}");
			}
		}

		string[] GetFilesInFolder()
		{
			var directory = Path.GetDirectoryName(_path);
			return Directory.GetFiles(directory);
		}

		string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif", ".bmp" };
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
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
			else if (e.KeyCode == Keys.Right)
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
			else if (e.KeyCode == Keys.ControlKey)
			{
				_slowZoom = true;
			}
		}
	}
}
