using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BBKLevelEditor {
	public partial class BBKLevelEditor : Form {

		enum BlockType { None = 0, Red = 1, Pink, Yellow, Green, Cyan, SkyBlue, Cobalt, PurPle, Brick = 20, Steel = 21, Ice = 30 }
		Dictionary<BlockType, int> blockType2ImageIdx = new Dictionary<BlockType, int>();
		Boolean _changeFlag = false;
		struct cell {
			public BlockType type;
			public int count;
		}
		cell[,] board = new cell[8, 8];


		public BBKLevelEditor() {
			InitializeComponent();
			blockType2ImageIdx.Add(BlockType.Red, 1);
			blockType2ImageIdx.Add(BlockType.Pink, 2);
			blockType2ImageIdx.Add(BlockType.Yellow, 3);
			blockType2ImageIdx.Add(BlockType.Green, 4);
			blockType2ImageIdx.Add(BlockType.Cyan, 5);
			blockType2ImageIdx.Add(BlockType.SkyBlue, 6);
			blockType2ImageIdx.Add(BlockType.Cobalt, 7);
			blockType2ImageIdx.Add(BlockType.PurPle, 8);
			blockType2ImageIdx.Add(BlockType.Brick, 9);
			blockType2ImageIdx.Add(BlockType.Steel, 10);
			blockType2ImageIdx.Add(BlockType.Ice, 11);

		}


		private int curTaskIndex = 0;
		private PictureBox[] picTask;
		private TextBox[] label;
		private int level_type;
		private int score;
		private int[] color = new int[8];

		private void onLoad(object sender, EventArgs e) {
			for (int i = 1; i < 1001; i++) {
				listBox1.Items.Add(i);
			}
			labelLevel.Text = "1";
			picTask = new PictureBox[3] { picTask1, picTask2, picTask3 };
			label = new TextBox[3] { labelTask1, labelTask2, labelTask3 };
			currentBlockTag = (int)BlockType.Brick;
			_changeFlag = false;

		}

		private int currentBlockTag = -1;
		private void onBlockClick(object sender, EventArgs e) {
			Button btn_mul = sender as Button;
			if (btn_mul != null) {
				currentBlockTag = Int32.Parse(btn_mul.Tag.ToString());
				currentBlock.BackgroundImage = btn_mul.BackgroundImage;
			}
			else {
				PictureBox pic = sender as PictureBox;
				currentBlock.BackgroundImage = pic.BackgroundImage;
			}
		}
		private void onMainTask(object sender, MouseEventArgs e) {
			PictureBox pic = sender as PictureBox;
			Point point = new Point(e.X, e.Y);
			contextMenu.Show(pic, point);
		}


		private void onTask(object sender, MouseEventArgs e) {
			PictureBox pic = sender as PictureBox;
			Point point = new Point(e.X, e.Y);
			curTaskIndex = Int32.Parse((sender as PictureBox).Tag.ToString());
			contextMenu1.Show(pic, point);
		}
		private void onLevelChange(object sender, EventArgs e) {
			labelLevel.Text = (listBox1.SelectedIndex + 1).ToString();
			_changeFlag = true;
			String level_path = string.Format("map/{0:D4}.dat", listBox1.SelectedIndex + 1);
			//String level_path = string.Format("{0:D4}.dat", 1111);

			if (File.Exists(level_path)) {
				using (BinaryReader reader = new BinaryReader(File.Open(level_path, FileMode.Open))) {
					int taskCount = 0;
					level_type = reader.ReadInt32();
					score = reader.ReadInt32();
					for (int i = 0; i < 8; i++) {
						color[i] = reader.ReadInt32();

					}
					switch (level_type) {
						case 1:
							picMainTask.BackgroundImage = blockImages.Images[13];
							labelMainTask.Text = score.ToString();
							labelMainTask.Visible = true;
							disable();
							break;
						case 2:
							picMainTask.BackgroundImage = btn_20.BackgroundImage;
							labelMainTask.Visible = false;
							disable();
							break;
						case 4:
							picMainTask.BackgroundImage = btn_30.BackgroundImage;
							labelMainTask.Visible = false;
							disable();
							break;
						case 8:
							labelMainTask.Visible = true;
							disable();

							for (int i = 0; i < 8; i++) {
								if (color[i] != 0) {
									taskCount++;
									switch (taskCount) {
										case 1:
											picMainTask.BackgroundImage = blockImages.Images[i + 1];
											picMainTask.BackgroundImage.Tag = i + 1;
											labelMainTask.Text = color[i].ToString();
											break;
										case 2:
											picTask[0].Visible = true;
											label[0].Visible = true;
											picTask[0].BackgroundImage = blockImages.Images[i + 1];
											picTask[0].BackgroundImage.Tag = i + 1;
											label[0].Text = color[i].ToString();
											break;
										case 3:
											picTask[0].Visible = true;
											label[0].Visible = true;
											picTask[1].Visible = true;
											label[1].Visible = true;
											picTask[1].BackgroundImage = blockImages.Images[i + 1];
											picTask[1].BackgroundImage.Tag = i + 1;
											label[1].Text = color[i].ToString();
											break;
										case 4:
											enable();
											picTask[2].BackgroundImage = blockImages.Images[i + 1];
											picTask[2].BackgroundImage.Tag = i + 1;
											label[2].Text = color[i].ToString();
											break;
										default:
											break;
									}
								}
							}

							break;
						default:
							break;
					}
					//board init part
					for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {
							board[i, 7 - j].type = (BlockType)(reader.ReadInt32() / 100);
							board[i, 7 - j].count = reader.ReadInt32();
						}
					}
				}

				picBoard.Invalidate();
			}


		}

		private void disable() {
			foreach (PictureBox pic in picTask)
				pic.Visible = false;
			foreach (TextBox txt in label)
				txt.Visible = false;

		}
		private void enable() {
			foreach (PictureBox pic in picTask)
				pic.Visible = true;
			foreach (TextBox txt in label)
				txt.Visible = true;
		}
		private void onItemSelected(object sender, ToolStripItemClickedEventArgs e) {
			//Flag add
			_changeFlag = true;
			PictureBox pic = sender as PictureBox;

			int tag = Int32.Parse(e.ClickedItem.Tag.ToString());

			picMainTask.BackgroundImage = blockImages.Images[tag];
			picMainTask.BackgroundImage.Tag = tag;
			if (tag > 8) {
				labelMainTask.Visible = false;
				disable();
				if (tag == 13) {
					level_type = 1;
					labelMainTask.Visible = true;
				}
				if (tag == 9) level_type = 2;
				if (tag == 11) level_type = 4;
			}
			else {
				level_type = 8;
				enable();
				labelMainTask.Visible = true;
			}
		}


		private void onTaskItem(object sender, ToolStripItemClickedEventArgs e) {
			_changeFlag = true;
			int tag = Int32.Parse(e.ClickedItem.Tag.ToString());
			if (tag == 12) {
				//todo None
				label[curTaskIndex].Visible = false;
				picTask[curTaskIndex].BackgroundImage = blockImages.Images[12];
				picTask[curTaskIndex].BackgroundImage.Tag = 12;
			}
			else {
				picTask[curTaskIndex].BackgroundImage = blockImages.Images[tag];
				picTask[curTaskIndex].BackgroundImage.Tag = tag;
			}
		}

		private void onDraw(object sender, PaintEventArgs e) {
			PictureBox pic = sender as PictureBox;
			int width = pic.Width;
			int height = pic.Height;
			float cellwidth = width / 8f;
			float cellheight = height / 8f;

			Graphics g = e.Graphics;
			Font countfnt = new Font("arial", 12);
			Brush countbrush = new SolidBrush(Color.White);

			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					if (board[i, j].type != BlockType.None) {

						g.DrawImage(blockImages.Images[blockType2ImageIdx[board[i, j].type]], cellwidth * i, cellheight * j, cellwidth, cellheight);
						if (board[i, j].type == BlockType.Ice || board[i, j].type == BlockType.Brick) {
							int cnt = board[i, j].count;
							g.DrawString(cnt == 0 ? "?" : cnt.ToString(), countfnt, countbrush, new PointF(i * cellwidth, j * cellheight));
						}
					}
				}
			}
		}

		private void onBoardAdd(object sender, MouseEventArgs e) {
			_changeFlag = true;
			int button_flag = 0;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				button_flag = 1;
			else
				button_flag = -1;
			int width = picBoard.Width;
			int height = picBoard.Height;
			float cellwidth = width / 8f;
			float cellheight = height / 8f;
			int mousex = e.X;
			int mousey = e.Y;
			int cellx = (int)Math.Min(7, mousex / cellwidth);
			int celly = (int)Math.Min(7, mousey / cellheight);

			int shift = 0, alt = 1, none = 0;

			if ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) > 0)
				shift = (shift + 5) * button_flag;
			else
				none = (none + 1) * button_flag;
			if ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Alt) > 0)
				alt = 0;
			if (button_flag == -1) {
				if (board[cellx, celly].type != BlockType.None) {
					board[cellx, celly].count = board[cellx, celly].count + none + shift;
				}
				if (board[cellx, celly].count <= 0) {
					board[cellx, celly].type = BlockType.None;
				}
			}
			else {
				if (board[cellx, celly].type != (BlockType)currentBlockTag) {
					board[cellx, celly].count = 1;
					board[cellx, celly].type = (BlockType)currentBlockTag;
				}
				else {
					switch (board[cellx, celly].type) {
						case BlockType.None:

							break;
						case BlockType.Brick:
							if ((BlockType)currentBlockTag == BlockType.Brick)
								board[cellx, celly].count = (board[cellx, celly].count + none + shift) * alt;
							break;
						case BlockType.Ice:

							if ((BlockType)currentBlockTag == BlockType.Ice)
								board[cellx, celly].count = (board[cellx, celly].count + none + shift) * alt;
							break;
						default:
							break;
					}
				}
			}
			if (alt == 0 && button_flag == -1) {
				board[cellx, celly].count = 0;
				board[cellx, celly].type = BlockType.None;
			}
			picBoard.Invalidate();
		}

		private void onKeyDown(object sender, KeyEventArgs e) {

			String str = e.KeyCode.ToString();
			if (str == "D1" || str == "NumPad1") {
				currentBlock.BackgroundImage = btn_01.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_01.Tag.ToString());
			}
			if (str == "D2" || str == "NumPad2") {
				currentBlock.BackgroundImage = btn_02.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_02.Tag.ToString());
			}
			if (str == "D3" || str == "NumPad3") {
				currentBlock.BackgroundImage = btn_03.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_03.Tag.ToString());
			}
			if (str == "D4" || str == "NumPad4") {
				currentBlock.BackgroundImage = btn_04.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_04.Tag.ToString());
			}
			if (str == "D5" || str == "NumPad5") {
				currentBlock.BackgroundImage = btn_05.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_05.Tag.ToString());
			}
			if (str == "D6" || str == "NumPad6") {
				currentBlock.BackgroundImage = btn_06.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_06.Tag.ToString());
			}
			if (str == "D7" || str == "NumPad7") {
				currentBlock.BackgroundImage = btn_07.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_07.Tag.ToString());
			}
			if (str == "D8" || str == "NumPad8") {
				currentBlock.BackgroundImage = btn_08.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_08.Tag.ToString());
			}
			if (str == "D9" || str == "NumPad9") {
				currentBlock.BackgroundImage = btn_20.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_20.Tag.ToString());
			}
			if (str == "D0" || str == "NumPad0") {
				currentBlock.BackgroundImage = btn_21.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_21.Tag.ToString());
			}
			if (str == "Decimal" || str == "Oemtilde") {
				currentBlock.BackgroundImage = btn_30.BackgroundImage;
				currentBlockTag = Int32.Parse(btn_30.Tag.ToString());
			}
			currentBlock.BackgroundImageChanged += onBlockClick;
		}

		private void onSave(object sender, MouseEventArgs e) {
			int level = Int32.Parse(labelLevel.Text);
			String level_path = string.Format("map/{0:D4}.dat", Int32.Parse(labelLevel.Text));
			using (BinaryWriter writer = new BinaryWriter(File.Open(level_path, FileMode.Open))) {
				if (_changeFlag) {
					writer.Write(level_type);
					int wscore = 0;
					if (level_type == 1) {
						if (labelMainTask.Text != "")
							wscore = Int32.Parse(labelMainTask.Text);
					}
					writer.Write(wscore);
					int[] wcolor = new int[8];
					if (level_type != 8) {
						for (int i = 0; i < 8; i++) {
							wcolor[i] = 0;
						}
					}
					// leve type color image type
					else {

						for (int i = 1; i < 9; i++) {
							if (Int32.Parse(picMainTask.BackgroundImage.Tag.ToString()) == i)
								wcolor[i - 1] = Int32.Parse(labelMainTask.Text);
						}
						for (int i = 1; i < 9; i++) {
							if (Int32.Parse(picTask[0].BackgroundImage.Tag.ToString()) == i)
								wcolor[i - 1] = Int32.Parse(label[0].Text);
						}
						for (int i = 1; i < 9; i++) {
							if (Int32.Parse(picTask[1].BackgroundImage.Tag.ToString()) == i)
								wcolor[i - 1] = Int32.Parse(label[1].Text);
						}
						for (int i = 1; i < 9; i++) {
							if (Int32.Parse(picTask[2].BackgroundImage.Tag.ToString()) == i)
								wcolor[i - 1] = Int32.Parse(label[2].Text);
						}
					}
					for (int i = 0; i < 8; i++) {
						writer.Write(wcolor[i]);
					}
					for (int i = 0; i < 8; i++) {
						for (int j = 0; j < 8; j++) {
							writer.Write(((int)board[i, 7 - j].type) * 100);
							writer.Write(board[i, 7 - j].count);
						}
					}

					//save_ok message
					String str = "Save Completed!";
					if (MessageBox.Show(str) == System.Windows.Forms.DialogResult.OK) {
						this.Load += onLoad;
					}
				}

				else {
					String s = "Save Error!";
					if (MessageBox.Show(s) == System.Windows.Forms.DialogResult.OK) {
						this.Load += onLoad;
					}
				}
			}
		}
	}
}
