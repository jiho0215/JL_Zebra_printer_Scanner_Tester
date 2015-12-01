using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace JL_Zebra_Printer_Scanner_Tester
{
    public partial class Form1 : Form
    {
        NotifyIcon noti = new NotifyIcon();
        string notiTitle = "JL_Soodas";
        string notiText = "JL_Zebra Printer_Scanner_Tester";
        List<SerialPort> SerialP = new List<SerialPort>();
        List<string> PortList = SerialPort.GetPortNames().ToList();
        string ZPLcommand = "^XA^FO,20,10^ADN,40,25^FDHello World^FS^XZ";//string.Empty;
        List<string> zplElements = new List<string>() { "X", "Y", "H", "W", "Font-Size" };
        List<string> gridviewColumns = new List<string>() { "Type", "X", "Y", "Font", "W", "H", "Input" };
        Dictionary<string, string> diczpl = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            InitialDisplay();
            AdminSetting();
            
        }

        private void AdminSetting()
        {
            Controls["Main"].Visible = false;
        }

        private void InitialDisplay()
        {
            GetConfig(this);
            int primaryW = Screen.PrimaryScreen.Bounds.Width;
            int primaryH = Screen.PrimaryScreen.Bounds.Height;
            this.Icon = Properties.Resources._1445534907_Hat;
            this.Size = new Size(900, 600);
            int clientW = this.FormBorderStyle == FormBorderStyle.None ? this.ClientSize.Width : this.ClientSize.Width;
            int clientH = this.FormBorderStyle == FormBorderStyle.None ? this.ClientSize.Height : this.ClientSize.Height;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightBlue;
            this.Layout += Form1_Layout;

            noti.Icon = Properties.Resources._1445534907_Hat;
            noti.BalloonTipText = this.Text;
            noti.BalloonTipTitle = "JL";
            noti.Text = "JL_Tester";
            noti.Visible = false;
            noti.MouseUp += Noti_MouseUp;

            ///Form Size Setting///
            this.MouseClick += Form1_MouseClick;

            ///Virtical Position Numbers///
            int topH = 30;
            int gap = 5;
            int topLabelH = clientH * 15 / 100;
            int top1stPoint = topLabelH * 30 / 100;
            int top2ndPoint = (topLabelH * 70 / 100) - (gap * 2);
            int bottomLabelH = clientH * 6 / 100;
            int section = clientH * 80 / 100;
            int sec1stH = section * 50 / 100;
            int sec2ndH = section * 50 / 100 - (gap * 2);
            int sec1stPoint = topLabelH;
            int sec2ndPoint = topLabelH + sec1stH + gap;
            ///Virtical Position Numbers///

            //Top Panel
            Panel topP = new Panel();
            PictureBox topPb = new PictureBox();
            Label topPLbl = new Label();
            Button btnMin = new Button();
            Button btnExit = new Button();
            //Top Panel

            topP.Size = new Size(clientW, topH);
            if (this.FormBorderStyle != FormBorderStyle.None) topP.Height = 0;
            topP.Location = new Point(0, 0);
            topP.BorderStyle = BorderStyle.FixedSingle;
            topP.BackColor = Color.Navy;
            topP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            topP.MouseDown += MoveForm;
            topP.Parent = this;

            topPb.Size = new Size(topH - 5, topH - 5);
            topPb.Location = new Point(2, 2);
            topPb.BorderStyle = BorderStyle.None;
            topPb.SizeMode = PictureBoxSizeMode.StretchImage;
            topPb.Image = Properties.Resources._1445562055_Hat;
            topPb.Parent = topP;

            topPLbl.Size = new Size(200, topH - 10);
            topPLbl.Location = new Point(topPb.Right, (topH - topPLbl.Height) / 2);
            topPLbl.Text = this.Text;
            topPLbl.Font = new Font("arial", 11, FontStyle.Regular);
            topPLbl.TextAlign = ContentAlignment.MiddleLeft;
            topPLbl.ForeColor = Color.White;
            topPLbl.BorderStyle = BorderStyle.None;
            topPLbl.Anchor = AnchorStyles.Left;
            topPLbl.MouseDown += MoveForm;
            topPLbl.Parent = topP;

            btnExit.Size = new Size(25, 25);
            btnExit.Location = new Point(topP.Width - btnExit.Width - 2, 2);
            btnExit.BackgroundImage = Properties.Resources._1445543925_close_red;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Name = "btnExit";
            btnExit.MouseClick += BtnExit_MouseUp;
            btnExit.Parent = topP;

            btnMin.Size = new Size(25, 25);
            btnMin.Location = new Point(btnExit.Left - btnMin.Width - 2, 2);
            btnMin.BackgroundImage = Properties.Resources._1445548524_minimize_window;
            btnMin.BackgroundImageLayout = ImageLayout.Stretch;
            btnMin.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnMin.FlatStyle = FlatStyle.Flat;
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.Name = "btnMin";
            btnMin.MouseClick += BtnExit_MouseUp;
            btnMin.Parent = topP;

            Label lbl2 = new Label();
            lbl2.Size = new Size(150, 18);
            lbl2.Location = new Point(gap, topP.Bottom);
            lbl2.BorderStyle = BorderStyle.None;
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
            lbl2.Text = "ver." + ver.ToString();
            lbl2.Font = new Font("arial", 11, FontStyle.Regular);
            lbl2.TextAlign = ContentAlignment.MiddleLeft;
            lbl2.ForeColor = Color.Black;
            lbl2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lbl2.Parent = this;

            GroupBox gb = new GroupBox();
            gb.Text = "Serial Port List";
            gb.Font = new Font("arial", 14, FontStyle.Regular);
            gb.Size = new Size(200, clientH - 60);
            gb.Location = new Point(gap, lbl2.Bottom + 5);
            gb.BackColor = Color.LightBlue;
            gb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            gb.Name = "gb";
            this.Controls.Add(gb);

            PortList.Reverse();
            PortList.ForEach(x =>
            {
                int i = PortList.IndexOf(x) + 1;
                RadioButton port = new RadioButton();
                port.Text = i + " : " + x;
                port.Name = x;
                port.Font = new Font("arial", 14, FontStyle.Regular);
                port.CheckedChanged += Radiobutton_CheckedChanged;
                port.Size = new Size(120, 30);
                port.Location = new Point(0, 30 * i); //(-13, 30);
                port.Checked = false;
                port.FlatStyle = FlatStyle.Flat;
                port.FlatAppearance.BorderSize = 5;
                gb.Controls.Add(port);

                SerialP.Add(new SerialPort(x));
                SerialP[i - 1].ReadTimeout = 500;
                SerialP[i - 1].WriteTimeout = 500;
                SerialP[i - 1].Open();
                SerialP[i - 1].DataReceived += new SerialDataReceivedEventHandler(SerialP_DataReceived);
                i++;

                port.MouseClick += Port_MouseClick;
            });

            Label lblPort = new Label();
            lblPort.Size = new Size(80, 25);
            lblPort.Location = new Point(gb.Right + gap, gb.Top);
            lblPort.Text = "PORT";
            lblPort.Font = new Font("arial", 18, FontStyle.Regular);
            lblPort.TextAlign = ContentAlignment.MiddleLeft;
            lblPort.ForeColor = Color.Black;
            lblPort.BorderStyle = BorderStyle.None;
            lblPort.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lblPort.Parent = this;

            TextBox tb1 = new TextBox();
            tb1.Name = "MainTb";
            tb1.Size = new Size(clientW - lblPort.Right - gap * 2, lblPort.Height);
            tb1.Location = new Point(lblPort.Right + gap, gb.Top);
            tb1.Font = new Font("arial", 18, FontStyle.Regular);
            tb1.ForeColor = Color.Black;
            tb1.BorderStyle = BorderStyle.FixedSingle;
            tb1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            tb1.KeyDown += KeyDown;
            tb1.Parent = this;

            RichTextBox Main = new RichTextBox();
            Main.Size = new Size(clientW - gb.Right - gap * 2, gb.Height - tb1.Height - gap);
            Main.Location = new Point(gb.Right + gap, tb1.Bottom + gap);
            Main.BackColor = Color.LightBlue;
            Main.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Main.BorderStyle = BorderStyle.Fixed3D;
            Main.Font = new Font("arial", 12, FontStyle.Regular);
            Main.Text = (PortList.Count != 0) ? PortList.Count + " port(s) found. Ready to scan or print." : "Connected port not found, please check the port connection";
            Main.Name = "Main";
            Main.SelectionFont = new Font("tahoma", 12);
            Main.ReadOnly = true;
            this.Controls.Add(Main);

            Panel Main_Print = new Panel();
            Main_Print.Size = new Size(clientW - gb.Right - gap * 2, gb.Height - tb1.Height - gap);
            Main_Print.Location = new Point(gb.Right + gap, tb1.Bottom + gap);
            Main_Print.BackColor = Color.FromArgb(215, 198, 121);
            Main_Print.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Main_Print.BorderStyle = BorderStyle.Fixed3D;
            Main_Print.Name = "Main_Print";
            this.Controls.Add(Main_Print);

            {//Start Main_Print Elements//
                GroupBox gb3 = new GroupBox();
                gb3.Text = "Input Type";
                gb3.Font = new Font("arial", 10, FontStyle.Regular);
                gb3.Size = new Size(152, 45);
                gb3.Location = new Point(gap, gap);
                gb3.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                gb3.Tag = "";
                gb3.Name = "gb3";
                Main_Print.Controls.Add(gb3);
                {//Start Input Type GroupBox
                    RadioButton rbText = new RadioButton();
                    rbText.Size = new Size(60, 23);
                    rbText.Location = new Point(gap, (gb3.Height - rbText.Height) / 2 + 5);
                    rbText.Font = new Font("arial", 12, FontStyle.Regular);
                    rbText.Text = "Text";
                    rbText.Name = "TypeText";
                    rbText.Checked = true;
                    rbText.CheckedChanged += Radiobutton_CheckedChanged;
                    rbText.Parent = gb3;

                    RadioButton rbBarcode = new RadioButton();
                    rbBarcode.Size = new Size(86, 23);
                    rbBarcode.Location = new Point(rbText.Right, rbText.Top);
                    rbBarcode.Font = new Font("arial", 12, FontStyle.Regular);
                    rbBarcode.Text = "Barcode";
                    rbBarcode.Name = "TypeBarcode";
                    rbBarcode.CheckedChanged += Radiobutton_CheckedChanged;
                    rbBarcode.Checked = false;
                    rbBarcode.Parent = gb3;
                }//End Input Type GroupBox
                
                int i = 0;
                zplElements.ForEach(x =>
                {
                    Panel zplElementsPanel = new Panel();
                    zplElementsPanel.Size = new Size(62, 30);
                    zplElementsPanel.Location = new Point(gb3.Right + gap * (i + 1) + (zplElementsPanel.Width * i), gb3.Controls["TypeText"].Top + 3);
                    zplElementsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    zplElementsPanel.BorderStyle = BorderStyle.None;
                    zplElementsPanel.Name = x;
                    Main_Print.Controls.Add(zplElementsPanel);
                    {
                        Label lblzpl = new Label();
                        lblzpl.Size = new Size(32, 25);
                        lblzpl.Location = new Point(0, 0);
                        lblzpl.Text = x + ":";
                        lblzpl.Name = x + "lbl";
                        lblzpl.Font = new Font("Tahoma", 13, FontStyle.Regular);
                        lblzpl.TextAlign = ContentAlignment.MiddleLeft;
                        lblzpl.ForeColor = Color.Black;
                        lblzpl.BorderStyle = BorderStyle.None;
                        lblzpl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        lblzpl.Parent = zplElementsPanel;

                        TextBox tbzpl = new TextBox();
                        tbzpl.Size = new Size(30, 25);
                        tbzpl.Location = new Point(lblzpl.Right, 0);
                        tbzpl.Font = new Font("Tahoma", 11, FontStyle.Regular);
                        tbzpl.Name = x + "Value";
                        tbzpl.ForeColor = Color.Black;
                        tbzpl.BorderStyle = BorderStyle.FixedSingle;
                        tbzpl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        tbzpl.Parent = zplElementsPanel;
                        tbzpl.KeyDown += KeyDown;
                    }
                    i++;
                });//End Foreach
                Main_Print.Controls["Font-Size"].Width = 120;
                Main_Print.Controls["Font-Size"].Location = new Point(Main_Print.Controls["Y"].Right + gap, Main_Print.Controls["Y"].Top);
                Main_Print.Controls["Font-Size"].Controls["Font-Sizelbl"].Width = 89;
                Main_Print.Controls["Font-Size"].Controls["Font-SizeValue"].Location = new Point(89, 0);
                Main_Print.Controls["Font-Size"].Controls["Font-SizeValue"].Text = "11";
                Main_Print.Controls["Font-Size"].Visible = true;
                Main_Print.Controls["X"].Controls["XValue"].Text = "20";
                Main_Print.Controls["Y"].Controls["YValue"].Text = "20";
                Main_Print.Controls["H"].Controls["HValue"].Text = "30";
                Main_Print.Controls["W"].Controls["WValue"].Text = "30";
                Main_Print.Controls["H"].Visible = false;
                Main_Print.Controls["W"].Visible = false;


                NumericUpDown NumPrint = new NumericUpDown();
                NumPrint.Size = new Size(88, 10);
                NumPrint.Location = new Point(Main_Print.Controls["W"].Right + gap, Main_Print.Controls["W"].Top - 1);
                NumPrint.Font = new Font("Tahoma", 30, FontStyle.Regular);
                NumPrint.ForeColor = Color.Black;
                NumPrint.BorderStyle = BorderStyle.FixedSingle;
                NumPrint.Minimum = 1;
                NumPrint.Maximum = 200;
                NumPrint.Value = 1;
                NumPrint.Name = "NumPrint";
                NumPrint.Parent = Main_Print;

                Button btnPrintOut = new Button();
                btnPrintOut.Size = new Size(NumPrint.Height, NumPrint.Height);
                btnPrintOut.Location = new Point(NumPrint.Right + gap, NumPrint.Top);
                btnPrintOut.BackColor = Color.LightGray;
                btnPrintOut.Font = new Font("tahoma", 12, FontStyle.Regular);
                //btnPrintOut.FlatStyle = FlatStyle.Flat;
                //btnPrintOut.FlatAppearance.BorderSize = 0;
                btnPrintOut.Text = "Print";
                btnPrintOut.Name = "btnPrintOut";
                btnPrintOut.MouseClick += Button_MouseClick;
                Main_Print.Controls.Add(btnPrintOut);

                TextBox input = new TextBox();
                input.Size = new Size(Main_Print.Controls["Y"].Right - gap, 20);
                input.Location = new Point(gap, Main_Print.Controls["gb3"].Height + gap * 2);
                input.Font = new Font("arial", 14, FontStyle.Regular);
                input.ForeColor = Color.Black;
                input.BorderStyle = BorderStyle.FixedSingle;
                input.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                input.Name = "Input";
                input.KeyDown += KeyDown;
                input.Parent = Main_Print;

                Button addText = new Button();
                addText.Size = new Size(Main_Print.Controls["W"].Right - input.Right - gap, 30);
                addText.Location = new Point(input.Right + gap, input.Top);
                addText.BackColor = Color.LightGray;
                addText.Font = new Font("tahoma", 11, FontStyle.Regular);
                //btnPrintOut.FlatStyle = FlatStyle.Flat;
                //btnPrintOut.FlatAppearance.BorderSize = 0;
                addText.Text = "Add Input";
                addText.Name = "addInput";
                addText.MouseClick += Button_MouseClick;
                Main_Print.Controls.Add(addText);

                //DataGridView dg1 = new DataGridView();
                //dg1.Size = new Size(400, 150);
                //dg1.Location = new Point(gap, input.Bottom + gap);
                //dg1.BackColor = Color.White;
                //dg1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                //dg1.BorderStyle = BorderStyle.Fixed3D;
                //dg1.Font = new Font("arial", 9, FontStyle.Regular);
                //dg1.Name = "dg1";
                //gridviewColumns.ForEach(x => dg1.Columns.Add(x, x));
                //dg1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                //dg1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //dg1.Rows.Add("dfl");
                //dg1.RowHeadersVisible = false;
                //dg1.ReadOnly = true;
                //Main_Print.Controls.Add(dg1);

                ListView dg1 = new ListView();
                dg1.Size = new Size(400, 150);
                dg1.Location = new Point(gap, input.Bottom + gap);
                dg1.BackColor = Color.White;
                dg1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                dg1.BorderStyle = BorderStyle.Fixed3D;
                dg1.Font = new Font("arial", 9, FontStyle.Regular);
                dg1.Name = "dg1";
                dg1.View = View.Details;
                gridviewColumns.ForEach(x => dg1.Columns.Add(x));
                dg1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                dg1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                dg1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                Main_Print.Controls.Add(dg1);

                RichTextBox ZPLText = new RichTextBox();
                ZPLText.Size = new Size(250,150);
                ZPLText.Location = new Point(dg1.Right + gap, dg1.Top);
                ZPLText.BackColor = Color.White;
                ZPLText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                ZPLText.BorderStyle = BorderStyle.FixedSingle;
                ZPLText.Font = new Font("arial", 12, FontStyle.Regular);
                ZPLText.Name = "ZPLText";
                Main_Print.Controls.Add(ZPLText);



                Button shiftLabel = new Button();
                shiftLabel.Size = new Size(120 ,30);
                shiftLabel.Location = new Point(gap, dg1.Bottom + gap);
                shiftLabel.BackColor = Color.LightGray;
                shiftLabel.Font = new Font("tahoma", 11, FontStyle.Regular);
                shiftLabel.Text = "Shift Label Print";
                shiftLabel.Name = "btn_ShiftLabel";
                shiftLabel.MouseClick += Button_MouseClick;
                Main_Print.Controls.Add(shiftLabel);
                                
                GroupBox gb_preview = new GroupBox();
                gb_preview.Text = "Output may vary";
                gb_preview.Font = new Font("arial", 11, FontStyle.Regular);
                gb_preview.Size = new Size(input.Width, 100);
                gb_preview.Location = new Point(gap, shiftLabel.Bottom + gap);
                gb_preview.BackColor = Color.White;
                gb_preview.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                gb_preview.Name = "gb_preview";
                Main_Print.Controls.Add(gb_preview);
                
                    

            }//End Main_Print Elements//

            CheckBox sound = new CheckBox();
            sound.Size = new Size(100, 30);
            sound.Location = new Point(gap, gb.Bottom);
            sound.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            sound.Font = new Font("arial", 12, FontStyle.Regular);
            sound.Text = "Sound On";
            sound.Name = "Sound";
            this.Controls.Add(sound);

            CheckBox Linecolor = new CheckBox();
            Linecolor.Size = new Size(100, 30);
            Linecolor.Location = new Point(Main.Left, Main.Bottom);
            Linecolor.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            Linecolor.Font = new Font("arial", 12, FontStyle.Regular);
            Linecolor.Text = "Line Color";
            Linecolor.Name = "Linecolor";
            this.Controls.Add(Linecolor);

            CheckBox checkPrint = new CheckBox();
            checkPrint.Size = new Size(100, 30);
            checkPrint.Location = new Point(Linecolor.Right, Main.Bottom);
            checkPrint.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            checkPrint.Font = new Font("arial", 12, FontStyle.Regular);
            checkPrint.Text = "Test Print";
            checkPrint.Name = "checkPrint";
            checkPrint.MouseClick += CheckBox_MouseClick;
            this.Controls.Add(checkPrint);

            Panel gb1 = new Panel();
            gb1.Size = new Size(180, 27);
            gb1.Location = new Point(checkPrint.Right, Main.Bottom);
            gb1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            gb1.BorderStyle = BorderStyle.FixedSingle;
            gb1.Name = "gb1";
            gb1.Visible = false;
            this.Controls.Add(gb1);


            Label lblgb = new Label();
            lblgb.Size = new Size(90, 25);
            lblgb.Location = new Point(0, (gb1.Height - lblgb.Height) / 2);
            lblgb.Text = "Label Size";
            lblgb.Font = new Font("arial", 12, FontStyle.Regular);
            lblgb.TextAlign = ContentAlignment.MiddleLeft;
            lblgb.ForeColor = Color.Black;
            lblgb.BorderStyle = BorderStyle.None;
            lblgb.Parent = gb1;

            RadioButton rb1 = new RadioButton();
            rb1.Size = new Size(50, 30);
            rb1.Location = new Point(lblgb.Right, (gb1.Height - rb1.Height) / 2);
            rb1.Font = new Font("arial", 12, FontStyle.Regular);
            rb1.Text = "S";
            rb1.Name = "S";
            rb1.Checked = true;
            rb1.CheckedChanged += Radiobutton_CheckedChanged;
            rb1.Parent = gb1;

            RadioButton rb2 = new RadioButton();
            rb2.Size = new Size(50, 30);
            rb2.Location = new Point(rb1.Right, rb1.Top);
            rb2.Font = new Font("arial", 12, FontStyle.Regular);
            rb2.Text = "L";
            rb2.Name = "L";
            rb2.Checked = false;
            rb2.CheckedChanged += Radiobutton_CheckedChanged;
            rb2.Parent = gb1;

            Panel gb2 = new Panel();
            gb2.Size = new Size(230, 27);
            gb2.Location = new Point(clientW - gb2.Width - gap, Main.Bottom);
            gb2.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            gb2.BorderStyle = BorderStyle.None;
            gb2.Name = "gb2";
            this.Controls.Add(gb2);

            Button btnPrint = new Button();
            btnPrint.Size = new Size(110, 25);
            btnPrint.Location = new Point(0, 0);
            btnPrint.BackColor = Color.Silver;
            btnPrint.Font = new Font("arial", 12, FontStyle.Regular);
            btnPrint.FlatStyle = FlatStyle.Popup;
            btnPrint.FlatAppearance.BorderSize = 1;
            btnPrint.Text = "Manual Print";
            btnPrint.Name = "btnPrint";
            btnPrint.MouseClick += Button_MouseClick;
            gb2.Controls.Add(btnPrint);
            btnPrint.Visible = false;

            Label lblFontSize = new Label();
            lblFontSize.Size = new Size(70, 25);
            lblFontSize.Location = new Point(btnPrint.Right + 2, btnPrint.Top);
            lblFontSize.Text = "Font Size";
            lblFontSize.Font = new Font("arial", 10, FontStyle.Regular);
            lblFontSize.TextAlign = ContentAlignment.MiddleLeft;
            lblFontSize.ForeColor = Color.Black;
            lblFontSize.BorderStyle = BorderStyle.None;
            lblFontSize.Parent = gb2;

            NumericUpDown lb = new NumericUpDown();
            lb.Size = new Size(gb2.Width - lblFontSize.Right - gap, 24);
            lb.Location = new Point(gb2.Width - lb.Width, 0);
            lb.Font = new Font("arial", 11, FontStyle.Regular);
            lb.ForeColor = Color.Black;
            lb.BorderStyle = BorderStyle.FixedSingle;
            lb.Minimum = 8;
            lb.Maximum = 100;
            lb.Value = int.Parse(Controls["Main"].Font.Size.ToString());
            lb.ValueChanged += Lb_ValueChanged;
            lb.Name = "FontSize";
            lb.Parent = gb2;





            GetConfig2(this);
        }//End InitialDisplay

        private void Radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                Control ctrlTemp = (Control)rb.Parent;
                ctrlTemp.Tag = getCheckedName(ctrlTemp);
                if (ctrlTemp.Name == "gb3")
                {
                    ctrlTemp.Parent.Controls["H"].Visible = !ctrlTemp.Parent.Controls["H"].Visible;
                    ctrlTemp.Parent.Controls["W"].Visible = !ctrlTemp.Parent.Controls["W"].Visible;
                    ctrlTemp.Parent.Controls["Font-Size"].Visible = !ctrlTemp.Parent.Controls["Font-Size"].Visible;
                }else if(ctrlTemp.Name == "gb1")
                {
                    if(rb.Name=="S")this.Controls["Main_Print"].Controls["gb_preview"].Size = new Size(300, 100);
                    else this.Controls["Main_Print"].Controls["gb_preview"].Size = new Size(400, 300);
                }
            }
            else
            {
                return;
            }
        }

        private string getCheckedName(Control gbTemp)
        {
            string rbName = string.Empty;
            foreach (Control rbTemp in gbTemp.Controls)
            {
                if (rbTemp.GetType().Name == "RadioButton")
                {
                    RadioButton rb = (RadioButton)rbTemp;
                    if (rb.Checked) rbName = rbTemp.Name;
                }
            }
            return rbName;
        }

        private void Lb_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numUpDown = (NumericUpDown)sender;
            RichTextBox rtb = (RichTextBox)Controls["Main"];
            rtb.Font = new Font(rtb.Font.FontFamily, int.Parse(numUpDown.Value.ToString()));
        }

        private void Button_MouseClick(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            switch(btn.Name)
            {
                case "btnPrint":
                    if (Controls["Main"].Visible)
                    {
                        Controls["Main"].Visible = false;
                        Controls["gb2"].Controls["FontSize"].Enabled = false;
                    }
                    else
                    {
                        Controls["Main"].Visible = true;
                        Controls["gb2"].Controls["FontSize"].Enabled = true;
                    }
                    break;
                case "btnPrintOut":
                    NumericUpDown nud = (NumericUpDown)Controls["Main_Print"].Controls["NumPrint"];
                    if (Controls["gb"].Tag != null)
                    {
                        string tempP = Controls["gb"].Tag.ToString();
                        for (int i = 0; i < nud.Value; i++)
                        {
                            Print(tempP, ZPLcommand);
                        }
                    }
                    break;
                case "addInput":
                    //Linecolor.Font = new Font("arial", 12, FontStyle.Regular);
                    int ft = int.Parse(this.Controls["Main_Print"].Controls["Font-Size"].Controls["Font-SizeValue"].Text);
                    Func.AddPrintInput(this.Controls["Main_Print"].Controls["Input"], this.Controls["Main_Print"].Controls["gb_preview"], ft);
                    break;
                default: break;
            }

        }

        private void CheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            
            CheckBox cb = (CheckBox)sender;

            switch (cb.Name)
            {
                case "checkPrint":
                    if (cb.Checked)
                    {
                        Controls["gb1"].Visible = true;
                        Controls["gb2"].Controls["btnPrint"].Visible = true;
                    }
                    else
                    {
                        Controls["gb1"].Visible = false;
                        Controls["gb2"].Controls["btnPrint"].Visible = false;
                        Controls["Main"].Visible = true;
                        Controls["gb2"].Controls["FontSize"].Enabled = true;
                    }
                    break;

                default: break;
            }

        }
        private void GetConfig2(object sender)
        {
            Form1 form1 = (Form1)sender;
            
        }

        private void GetConfig(object sender)
        {
            Form1 form1 = (Form1)sender;
            form1.Text = Func.GetEntryValue("Info", "Name");
            form1.FormBorderStyle = Func.GetEntryValue("Info", "Formborder") == "None" ? FormBorderStyle.None : FormBorderStyle.Sizable;

        }

        private void SerialP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort SerialP = (SerialPort)sender;
            Thread.Sleep(100);
            Func.AddText(SerialP.ReadExisting(), Controls["Main"]);
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if(tb.Name == "MainTb") Func.AddText(tb, this.Controls["Main"]);
                    if (tb.Name == "Input")
                    {
                        ListView lv = (ListView)this.Controls["Main_Print"].Controls["dg1"];
                        lv.Items.Add(GetListViewItem());
                        Func.AddPrintInput(tb, this.Controls["Main_Print"].Controls["gb_preview"], int.Parse(this.Controls["Main_Print"].Controls["Font-Size"].Controls["Font-SizeValue"].Text));
                    }
                    break;




                default:
                    break;
            }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            TextBox tb1 = (TextBox)sender;
            this.Controls["Main"].Text += tb1.Text;
        }

        private void BtnExit_MouseUp(object sender, MouseEventArgs e)
        {
            Control x = (Control)sender;
            switch (x.Name)
            {
                case "btnExit":
                    Func.ApplicationExit();
                    break;
                case "btnMin":
                    Func.Minimize(this, noti, notiTitle, notiText);
                    break;
                default: break;
            }
        }

        private void MoveForm(object sender, MouseEventArgs e)
        {
            Control Ctrl = (Control)this;
            Func.ReleaseCapture();
            Func.SendMessage(Ctrl.Handle, Func.WM_NCLBUTTONDOWN, Func.HT_CAPTION, 0);
        }

        private void Noti_MouseUp(object sender, MouseEventArgs e)
        {
            UnMinimized();
        }

        private void Form1_Layout(object sender, LayoutEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) WindowsMinimizedEvent();
            else UnMinimized();
        }

        private void UnMinimized()
        {
            this.Show();
            noti.Visible = false;
        }

        private void WindowsMinimizedEvent()
        {
            noti.Visible = true;
            noti.ShowBalloonTip(2000);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //    MouseEventArgs me = (MouseEventArgs)e;
            if (e.Button == System.Windows.Forms.MouseButtons.Left) return;// MessageBox.Show("Clicked Left Mouse");
            if (e.Button == System.Windows.Forms.MouseButtons.Right) RightMouseClick();
        }

        private void RightMouseClick()
        {
            return;
        }

        private void Print(string port, string ZPLcommand)
        {
            int i = PortList.IndexOf(port);
            try
            {
                SerialP[i].Write(ZPLcommand);
            }
            catch (Exception e)
            {
                return;
            }
        }
        private void Port_MouseClick(object sender, MouseEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            try
            {
                Print(rb.Name, ZPLcommand);
            }
            catch (Exception ee)
            {
                return;
            }
        }

        private ListViewItem GetListViewItem()
        {
            
            List<string> tempList = new List<string>();
  
            //Control it = (Control)this.Controls["Main_Pring"];
         
            tempList.Add(getCheckedName(this.Controls["Main_Print"].Controls["gb3"]));
            tempList.Add(this.Controls["Main_Print"].Controls["X"].Controls["XValue"].Text);
            tempList.Add(this.Controls["Main_Print"].Controls["Y"].Controls["YValue"].Text);
            tempList.Add(this.Controls["Main_Print"].Controls["Font-Size"].Controls["Font-SizeValue"].Text);
            tempList.Add(this.Controls["Main_Print"].Controls["W"].Controls["WValue"].Text);
            tempList.Add(this.Controls["Main_Print"].Controls["H"].Controls["HValue"].Text);
            tempList.Add(this.Controls["Main_Print"].Controls["Input"].Text);
            ListViewItem LVI = new ListViewItem(tempList.ToArray());
            try {
                int a = int.Parse(this.Controls["Main_Print"].Controls["Y"].Controls["YValue"].Text);
                int b = int.Parse(this.Controls["Main_Print"].Controls["Font-Size"].Controls["Font-SizeValue"].Text);
                this.Controls["Main_Print"].Controls["Y"].Controls["YValue"].Text = (a + b).ToString();
                    }catch(Exception e)
            {
                MessageBox.Show("Only Numbers on Character Setting.");
            }

            return LVI;
        }
    }
}
