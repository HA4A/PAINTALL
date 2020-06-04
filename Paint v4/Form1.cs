using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;


namespace Paint_v4
{
	public partial class Form1 : MetroForm
	{
		Color color = Color.Black;
		bool Presed = false;
		int x, y, x1, y1 = 0;
		int X, Y;
		Item item;
		Graphics gr;
		Graphics gr1;
		Bitmap bitmap;
		Bitmap bitmap1;
		
		public Form1()
		{
			InitializeComponent();
			item = Item.Brush;

			metroRadioButton2.Checked = true;

			metroTrackBar1.Value = 2;
			metroTrackBar1.Minimum = 2;
			metroTrackBar1.Maximum = 24;

			bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			bitmap1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);

			pictureBox1.Image = bitmap;
		}

		public enum Item
		{
			Rectangle, Ellipse, Brush, Erase, Picker, Line,
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			Presed = true;
			x = e.X;
			y = e.Y;
		}

		private void toolStripButton3_Click(object sender, EventArgs e)// кисть
		{
			item = Item.Brush;
			pictureBox1.Cursor = Cursors.Hand;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)// прямоугольник
		{
			item = Item.Rectangle;
			pictureBox1.Cursor = Cursors.Cross;
		}

		private void toolStripButton2_Click(object sender, EventArgs e)// круг
		{
			item = Item.Ellipse;
			pictureBox1.Cursor = Cursors.Cross;
		}

		private void toolStripButton4_Click(object sender, EventArgs e)// открытие
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Png files|*.png|jpeg files|*jpg";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (var fs = File.OpenRead(openFileDialog.FileName))
				using (var img = Image.FromStream(fs))
				{
					bitmap = (Bitmap)img.Clone();
				}
				pictureBox1.Image = bitmap;
			}
		}

		private void toolStripButton5_Click(object sender, EventArgs e)//сохранение
		{
			SaveFileDialog s = new SaveFileDialog();
			s.Filter = "Png files|*.png|jpeg files|*jpg";
			if (s.ShowDialog() == DialogResult.OK)
			{
				if (File.Exists(s.FileName))
				{
					File.Delete(s.FileName);
				}
				if (s.FileName.Contains(".jpg"))
				{
					bitmap.Save(s.FileName, ImageFormat.Jpeg);
				}
				else if (s.FileName.Contains(".png"))
				{
					bitmap.Save(s.FileName, ImageFormat.Png);
				}
			}
			toolStripButton6_Click(this, null);
		}

		private void toolStripButton6_Click(object sender, EventArgs e)// очистка
		{
			gr.Clear(Color.White);
			gr1.Clear(Color.White);
			pictureBox1.Invalidate();
		}

		private void toolStripButton8_Click(object sender, EventArgs e)// стиралка
		{
			item = Item.Erase;
			pictureBox1.Cursor = Cursors.Default;
		}

		public void pictureBox1_MouseMove(object sender, MouseEventArgs e)// 1 рисовалка
		{
			X = Convert.ToInt32(e.X);
			Y = Convert.ToInt32(e.Y);

			label1.Text = X.ToString();
			label2.Text = Y.ToString();

			if (e.Button == MouseButtons.Left)
			{
				gr = Graphics.FromImage(bitmap);
				gr1 = Graphics.FromImage(bitmap1);
				Pen pen = new Pen(color);
				switch (item)
				{
					case Item.Brush:
						gr.FillEllipse(new SolidBrush(color), e.X - x + x, e.Y - y + y, Convert.ToInt32(metroTrackBar1.Value), Convert.ToInt32(metroTrackBar1.Value));
						break;
					case Item.Erase:
						gr.FillRectangle(new SolidBrush(pictureBox1.BackColor), e.X - x + x, e.Y - y + y, Convert.ToInt32(metroTrackBar1.Value), Convert.ToInt32(metroTrackBar1.Value));
						break;
				}


				if (metroRadioButton1.Checked)
				{
					switch (item)
					{
						case Item.Rectangle:
							gr1.Clear(Color.White);
							gr1.FillRectangle(new SolidBrush(color), x1, y1, Math.Abs(e.X - x), Math.Abs(e.Y - y));
							break;
						case Item.Ellipse:
							gr1.Clear(Color.White);
							gr1.FillEllipse(new SolidBrush(color), x, y, e.X - x, e.Y - y);
							break;
					}
				}

				if (metroRadioButton2.Checked)
				{
					switch (item)
					{
						case Item.Rectangle:
							gr1.Clear(Color.White);
							gr1.DrawRectangle(pen, x1, y1, Math.Abs(e.X - x), Math.Abs(e.Y - y));
							break;
						case Item.Ellipse:
							gr1.Clear(Color.White);
							gr1.DrawEllipse(pen, x, y, e.X - x, e.Y - y);
							break;
					}
				}
				pictureBox1.Invalidate();
			}
		}

		private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
		{
			Presed = true;
		}

		private void pictureBox2_MouseMove(object sender, MouseEventArgs e)// краска
		{
			if (Presed)
			{
				Bitmap bitmap2 = (Bitmap)pictureBox2.Image.Clone();
				color = bitmap2.GetPixel(e.X, e.Y);
				pictureBox3.BackColor = color;
			}
		}

		private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
		{
			Presed = false;
		}

		private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
		{
			metroLabel1.Text = metroTrackBar1.Value.ToString();
		}

		private void toolStripButton7_Click(object sender, EventArgs e)// пипетка
		{
			item = Item.Picker;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)//закрытие формы
		{
			switch (MessageBox.Show("Сохранить работу в файл?", "Выход", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
			{
				case DialogResult.Yes:
					SaveFileDialog s = new SaveFileDialog();
					s.Filter = "Png files|*.png|jpeg files|*jpg";
					if (s.ShowDialog() == DialogResult.OK)
					{
						if (File.Exists(s.FileName))
						{
							File.Delete(s.FileName);
						}
						if (s.FileName.Contains(".jpg"))
						{
							bitmap.Save(s.FileName, ImageFormat.Jpeg);
						}
						else if (s.FileName.Contains(".png"))
						{
							bitmap.Save(s.FileName, ImageFormat.Png);
						}
					}
					else
						e.Cancel = true;
					break;
				case DialogResult.No:
					return;
				default:
					e.Cancel = true;
					return;
			}
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)// реализация пипетки
		{
			if (item == Item.Picker)
			{
				Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				Rectangle rectangle = pictureBox1.RectangleToScreen(pictureBox1.ClientRectangle);
				graphics.CopyFromScreen(rectangle.Location, Point.Empty, pictureBox1.Size);
				graphics.Dispose();
				color = bitmap.GetPixel(e.X, e.Y);
				pictureBox3.BackColor = color;
				bitmap.Dispose();
			}
		}

		private void toolStripButton9_Click(object sender, EventArgs e)//линия
		{
			item = Item.Line;
			pictureBox1.Cursor = Cursors.Default;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)//2 рисовалка
		{

			Presed = false;
			x1 = e.X;
			y1 = e.Y;
			if (e.Button == MouseButtons.Left)
			{
				Pen pen1 = new Pen(color, metroTrackBar1.Value);
				gr = Graphics.FromImage(bitmap);
				switch (item)
				{
					case Item.Line:
						gr.DrawLine(pen1, new Point(x, y), new Point(x1, y1));
						break;
					case Item.Brush:
						gr.FillEllipse(new SolidBrush(color), e.X - x + x, e.Y - y + y, Convert.ToInt32(metroTrackBar1.Value), Convert.ToInt32(metroTrackBar1.Value));
						break;
					case Item.Erase:
						gr.FillRectangle(new SolidBrush(pictureBox1.BackColor), e.X - x + x, e.Y - y + y, Convert.ToInt32(metroTrackBar1.Value), Convert.ToInt32(metroTrackBar1.Value));
						break;
				}

				if (metroRadioButton2.Checked)
				{
					switch (item)
					{
						case Item.Rectangle:
							int o, p;
							o = x;
							p = y;
							if (o > e.X) o = e.X;
							if (p > e.Y) p = e.Y;
							gr.FillRectangle(new SolidBrush(color), o, p, Math.Abs(e.X - x), Math.Abs(e.Y - y));
							break;
						case Item.Ellipse:
							gr.FillEllipse(new SolidBrush(color), x, y, e.X - x, e.Y - y);
							break;
					}
				}

				if (metroRadioButton1.Checked)
				{
					switch (item)
					{
						case Item.Rectangle:
							int o, p;
							o = x;
							p = y;
							if (o > e.X) o = e.X;
							if (p > e.Y) p = e.Y;
							gr.DrawRectangle(pen1, o, p, Math.Abs(e.X - x), Math.Abs(e.Y - y));
							break;
						case Item.Ellipse:
							gr.DrawEllipse(pen1, x, y, e.X - x, e.Y - y);
							break;
					}
				}
				pictureBox1.Invalidate();
			}
		}
	}
} 