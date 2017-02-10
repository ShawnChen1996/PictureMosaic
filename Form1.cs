using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PictureMosaic
{
    public partial class Form1 : Form
    {

        int difficulty;
        FileInfo pic;
        bool inGame = false;
        Point nullBox;
        int length;
        int num;
        Button btnShowPic = new Button();
        Panel panel = new Panel();
        Label lbltxt = new Label();
        Label lbldiff = new Label();
        ListBox picList = new ListBox();
        Button btnStart = new Button();
        ComboBox diffBox = new ComboBox();
        Button btnAddPic = new Button();
        PictureBox picBox = new PictureBox();
        private const string picPath = "Image";
        private const string PictureFormat = "*.jpg *.png *.gif";
        StatusBar status = new StatusBar();
        string statusTxt;
        Dictionary<PictureBox,Point> boxDic = new Dictionary<PictureBox,Point>();


        public Form1()
        {
            InitializeComponent();

            //双缓冲绘图
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Size = new Size(800, 600);
            this.PreviewKeyDown += Form1_PreviewKeyDown;

            //penal
            panel.Location = new Point(250, 30);
            panel.Size = new Size(500, 500);
            panel.BackColor = Color.AliceBlue;
            this.Controls.Add(panel);
            panel.TabIndex = 0;

            //picBox
            picBox.Location = new Point(0, 0);
            picBox.Size = new Size(500,500);

            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            panel.Controls.Add(picBox);

            //label
            lbltxt.Text = "请选择图片：";
            lbltxt.Location = new Point(30, 30);
            lbltxt.AutoSize = true;
            this.Controls.Add(lbltxt);

            lbldiff.Text = "请选择难度：";
            lbldiff.Location = new Point(30, 300);
            lbldiff.AutoSize = true;
            this.Controls.Add(lbldiff);

            //list
            picList.Location = new Point(30, 50);
            picList.Size = new Size(150, 200);
            picList.SelectionMode = SelectionMode.One;
            picList.SelectedIndexChanged +=picList_SelectedIndexChanged;
            this.Controls.Add(picList);

            //ComboBox
            diffBox.Location = new Point(30, 320);
            diffBox.Items.Add("3*3");
            diffBox.Items.Add("4*4");
            diffBox.Items.Add("5*5");
            this.Controls.Add(diffBox);
            diffBox.Size = new Size(150, 30);
            diffBox.SelectedIndex = 0;

            //button
            btnAddPic.Location = new Point(50, 380);
            btnAddPic.Text = "添加图片";
            btnAddPic.Size = new Size(100, 30);
            btnAddPic.Click += btnAddPic_Click;
            this.Controls.Add(btnAddPic);

            btnStart.Location = new Point(50, 470);
            btnStart.Text = "开始";
            btnStart.Size = new Size(100, 50);
            btnStart.Click += btnStart_Click;
            this.Controls.Add(btnStart);

            btnShowPic.Location = new Point(50, 430);
            btnShowPic.Text = "提示完整图片";
            btnShowPic.Size = new Size(100, 30);  
            btnShowPic.MouseDown += btnShowPic_MouseDown;
            btnShowPic.MouseUp += btnShowPic_MouseUp;
            btnShowPic.Enabled = false;
            this.Controls.Add(btnShowPic);

            //status
            statusTxt = "右下角最后一块图块被去除，按上下左右键以移动图块";
            status.Text =statusTxt ;
            this.Controls.Add(status);
        }



          void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            PictureBox SelectBox = null;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (nullBox.Y != difficulty)
                    {
                        foreach (PictureBox Box in boxDic.Keys)
                        {
                            if (boxDic[Box] == new Point(nullBox.X, nullBox.Y + 1))
                            {
                                SelectBox = Box;
                                break;
                            }
                        }
                    }
                    break;
                case Keys.Down:
                    if (nullBox.Y != 1)
                    {
                        foreach (PictureBox Box in boxDic.Keys)
                        {
                            if (boxDic[Box] == new Point(nullBox.X, nullBox.Y - 1))
                            {
                                SelectBox = Box; 
                                break;
                            }
                               
                            
                        }
                    }
                    break;
                case Keys.Left:
                    if (nullBox.X != difficulty)
                    {
                        foreach (PictureBox Box in boxDic.Keys)
                        {
                            if (boxDic[Box] == new Point(nullBox.X + 1, nullBox.Y))
                            {
                                SelectBox = Box;
                                break;
                            }
                        }
                    }
                    break;
                case Keys.Right:
                    if (nullBox.X != 1)
                    {
                        foreach (PictureBox Box in boxDic.Keys)
                        {
                            if (boxDic[Box] == new Point(nullBox.X - 1, nullBox.Y ))
                            {
                                SelectBox = Box;
                                break;
                            }
                        }
                    }
                    break;
            }

            if (SelectBox != null)
            {
                
                move(SelectBox, nullBox);

                 Point temp = boxDic[SelectBox];
                 boxDic[SelectBox]= nullBox;
                 nullBox= temp;
            }

              if(chekwin())
              {
                  MessageBox.Show("恭喜你，你成功复原了图像！");
              }


            status.Text = statusTxt +"  |  还原状态：" + chekwin().ToString();
            
        }

       

        void move(PictureBox Box, Point toPnt)
        {
            int lx= Box.Location.X-(toPnt.X-1) * length;
            int ly  = Box.Location.Y- (toPnt.Y-1) * length;
            int dstnce = (int)Math.Sqrt(lx*lx + ly *ly);

            while (dstnce > 10)
                {
                    Box.Location = new Point((int)Box.Location.X - 10* lx / dstnce, (int)Box.Location.Y - 10 * ly / dstnce);
                    lx = Box.Location.X - (toPnt.X - 1) * length;
                    ly = Box.Location.Y - (toPnt.Y - 1) * length;
                    dstnce = (int)Math.Sqrt(lx * lx + ly * ly);
                    Thread.Sleep(1);
                }
            Box.Location = new Point((toPnt.X - 1) * length, (toPnt.Y - 1) * length);
        }

        void btnShowPic_MouseUp(object sender, MouseEventArgs e)
        {
            picBox.Visible = false;

            regainFocus();
        }

        void btnShowPic_MouseDown(object sender, MouseEventArgs e)
        {
            picBox.Visible = true;
        }

       void regainFocus()
        {
            //强制form获得焦点
            picList.Enabled = false;
            diffBox.Enabled = false;
            btnAddPic.Enabled = false;
            btnShowPic.Enabled = false;
            btnStart.Enabled = false;

            //this.Focus();
            panel.TabIndex = 0;

            picList.Enabled = true;
            diffBox.Enabled = true;
            btnAddPic.Enabled = true;
            btnShowPic.Enabled = true;
            btnStart.Enabled = true;
        }

        private void picList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (inGame)
                return;
            pic = picList.SelectedItem as FileInfo;
            picBox.Image = Image.FromFile(pic.FullName);

        }

        void btnStart_Click(object sender, EventArgs e)
         {

            if (inGame)
            {
                if (MessageBox.Show("你正在游戏中,要重新开始吗？？？","重新开始" ,MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    regainFocus();
                    return;
                }
                    
            }
            //regainFocus();
            pic = picList.SelectedItem as FileInfo;
            picBox.Image = Image.FromFile(pic.FullName); 
            inGame = true;
            btnShowPic.Enabled = true;
            picBox.Visible = false;
            difficulty = diffBox.SelectedIndex+3;

            //分割图片
            splitPic();

            //打乱图片
            shufflePic();

            regainFocus();
            
        }

        private void shufflePic()
        {
            Random rnd = new Random();
           for (int i = 0; i<10; i++)
           {
               int s1 = rnd.Next(1, difficulty* difficulty);
               int s2 = rnd.Next(1, difficulty*difficulty);   
               //交换图块
               Point temp;
               temp = panel.Controls[s1].Location;
               panel.Controls[s1].Location = panel.Controls[s2].Location;
               panel.Controls[s2].Location = temp;

               //记录交换后位置，存入dic
               
               Point tempPnt;
               tempPnt = boxDic[(PictureBox)panel.Controls[s1]];
               boxDic[(PictureBox)panel.Controls[s1]] = boxDic[(PictureBox)panel.Controls[s2]];
               boxDic[(PictureBox)panel.Controls[s2]] = tempPnt;
           }
        }

        private void splitPic()
        {
            //销毁之前图像
            for (int i = panel.Controls.Count - 1; i > 0; i--)
            {
                PictureBox picItem = panel.Controls[i] as PictureBox;
                picItem.Image.Dispose();
                picItem.Image = null;
                panel.Controls[i].Dispose();
            }  
            //销毁之前的dic
            boxDic.Clear();

            length = panel.Width / difficulty;
            num = difficulty * difficulty - 1;
            Bitmap sourcebmp = picBox.Image as Bitmap;
            for (int i = 0; i<num; i++)
            {
                int x= i % difficulty;
                int y = i / difficulty;
                int scrHeight = sourcebmp.Height / difficulty;
                int scrWidth = sourcebmp.Width / difficulty;


                Bitmap block = new Bitmap(length,length);
                Graphics p = Graphics.FromImage(block);
                p.DrawImage(sourcebmp, new Rectangle(0, 0, length, length), new Rectangle(x* scrWidth,y*scrHeight,scrWidth, scrHeight), GraphicsUnit.Pixel);
                // picBlock
                PictureBox picBlock = new PictureBox();
                picBlock.SizeMode = PictureBoxSizeMode.StretchImage;
                picBlock.Location = new Point(x * length, y * length);
                picBlock.Size = new System.Drawing.Size(length,length);
                picBlock.Image = block;
                picBlock.BorderStyle = BorderStyle.Fixed3D;
                panel.Controls.Add(picBlock);
                boxDic[picBlock]=new Point(x+1,y+1);
            }

            nullBox = new Point(difficulty,difficulty);
        }

        void btnAddPic_Click(object sender, EventArgs e)
        {
            OpenFileDialog addPicDialog= new OpenFileDialog();
            addPicDialog.Filter="jpg文件|*.jpg|png图片|*.png|gif图片|*.gif";
            if (addPicDialog.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    // 把文件拷贝到 Image目录下 
                    File.Copy(addPicDialog.FileName, picPath + "\\" + System.IO.Path.GetFileName(addPicDialog.FileName));
                }
                catch (Exception ex)
                {
                    // 文件拷贝失败
                    MessageBox.Show("文件复制失败！" + ex.Message);
                }
                picListShow();
            }
        }

        private void picListShow()
        {
            if (!Directory.Exists(picPath))
            {
                Directory.CreateDirectory(picPath);
            }

            DirectoryInfo directory = new DirectoryInfo(picPath);

            // 获取Image目录下所有图片文件
            List <FileInfo> fileList = new List<FileInfo>();

            foreach (var item in PictureFormat.Split(' '))
            {
                fileList.AddRange(directory.GetFiles(item));
            }

            
            // ListBox 初始化
            picList.Items.Clear();
            foreach (var item in fileList)
            {
                picList.Items.Add(item);
            }

            if(picList.Items.Count!=0)
                picList.SelectedIndex = 0;
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            picListShow();
        }
        public bool chekwin()
        {
            for( int i = 1; i< panel.Controls.Count; i++)
            {
                if (boxDic[(PictureBox)panel.Controls[i]] != new Point((i-1)% difficulty+1,(i-1) / difficulty+1))
                    return false;   
            }
            return true;
        }
    }
}
