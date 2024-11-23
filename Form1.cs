using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
		}

		private void LoadImg()
		{
			try
			{
				pictureBox1.Image = Image.FromFile(_path);
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
		}
	}
}
