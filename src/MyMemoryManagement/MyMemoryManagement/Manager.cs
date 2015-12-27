using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

namespace MyMemoryManagement
{
    enum OneTwoOneState
    {
        FRONT = 0,
        FLLOWF = 1,
        FLLOWB = 2,
        BACK = 3,
    }

     struct BLOCK
    { 
       public int pagenum;//页号
       public int accessed;//访问字段，其值表示多久未被访问
     }

    class Manager
    {
        MainWindow m = (MainWindow)App.Current.MainWindow;
        public delegate void TimerDelegate();
        public static readonly Object thisLock = new Object();

        private DataTable dt;
        private DataView dv;

        private int count;//程序计数器，用来记录指令的序号
        private int n;//缺页计数器，用来记录缺页的次数
        private List<int> temp;//用来存储320条随机数
        private BLOCK[] block;//块

        private OneTwoOneState state;//121算法当先状态
        private int lastorder;
        private int currentorder;

        private int fifo;//fifo算法指向的被访问内存

        private Timer timer;//计时

        public bool IsInprograss; //是否正在运行
        private bool available;

        public Manager()
        {
            Init();
        }

        public void Init()
        {
            this.dt = new DataTable();

            this.block = new BLOCK[4];
            this.temp = new List<int>();
            this.count = 0;
            this.n = 0;
            
            this.state = OneTwoOneState.BACK;
            this.lastorder = 0;
            this.currentorder = 0;

            this.fifo = 0;

            this.IsInprograss = false;
            this.available = false;

            for (int i = 0; i < 4; i++)
            {
                this.block[i].pagenum = -1;
                this.block[i].accessed = 0;
            }

            for (int i = 0; i < 320; i++)
            {
                temp.Add(i+1);
            }
            
            DataColumn dc = new DataColumn("one", typeof(System.String));
            this.dt.Columns.Add(dc);
            dc = new DataColumn("two", typeof(System.String));
            this.dt.Columns.Add(dc);
            dc = new DataColumn("three", typeof(System.String));
            this.dt.Columns.Add(dc);
            dc = new DataColumn("four", typeof(System.String));
            this.dt.Columns.Add(dc);
            dc = new DataColumn("five", typeof(System.String));
            this.dt.Columns.Add(dc);
            dc = new DataColumn("six", typeof(System.String));
            this.dt.Columns.Add(dc);

            this.dv = new DataView(this.dt);
            this.dv.AllowNew = false;

            m.dataGrid1.DataContext = this.dv;
        }

        public void Auto()
        {
            lock (thisLock)
            {
                if ((string)this.m.Auto.Content == "自动")
                {
                    this.IsInprograss = true;
                    this.available = true;

               //     this.timer = new Timer(TimeMethod, null, 1000, 1000);
                    this.m.dataGrid1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new TimerDelegate(TimerCal));
                }
                else
                {
           //         if (this.timer != null)
          //          {
                //        this.timer.Dispose();
                //        this.timer = null;
               //     }
                    this.available = false;
                }
            }
        }

        private void TimeMethod(object state)
        {
       /*     if (this.count == 320)
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }*/
        }

        private void TimerCal()
        {
            lock (thisLock)
            {
                if (this.available == true/*this.timer != null*/)
                {
                    Step();
                    this.m.dataGrid1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new TimerDelegate(this.TimerCal));
                }
            }
            Thread.Sleep(50);
        }

        public void Step()
        {
            this.IsInprograss = true;
            switch (this.m.Number.SelectedIndex)
            {
                case 0:
                    this.Random();
                    break;
                case 1:
                    this.OneTwoOne();
                    break;
            }
            switch(this.m.Method.SelectedIndex)
            {
                case 0:
                    this.FIFO();
                    break;
                case 1:
                    this.LRU();
                    break;
            }
            for (int i = 0; i < 4; i++)
            {
                if (this.block[i].pagenum != -1)
                    this.block[i].accessed++;
            }
            this.m.dataGrid1.ScrollIntoView(this.m.dataGrid1.Items[this.count-1]);
            if(this.count == 320)
            {
                this.m.Auto.IsEnabled = false;
                this.m.Number.IsEnabled = false;
            }

        }

        public void Clear()
        {
            this.dt.Clear();
            this.count = 0;
            this.n = 0;
            this.lastorder = 0;
            this.currentorder = 0;
            this.state = OneTwoOneState.BACK;
            this.fifo = 0;
            this.IsInprograss = false;

            this.m.Block1.Fill = System.Windows.Media.Brushes.LightGray;
            this.m.Block2.Fill = System.Windows.Media.Brushes.LightGray;
            this.m.Block3.Fill = System.Windows.Media.Brushes.LightGray;
            this.m.Block4.Fill = System.Windows.Media.Brushes.LightGray;
            this.m.Mask1.Content = " ";
            this.m.Mask2.Content = " ";
            this.m.Mask3.Content = " ";
            this.m.Mask4.Content = " ";

            for (int i = 0; i < 4; i++)
            {
                this.block[i].pagenum = -1;
                this.block[i].accessed = 0;
            }
            
            for (int i = 0; i < 320; i++)
            {
                temp.Add(i + 1);
            }
        }

        public void Random()
        {
            Random rnd = new Random();
            this.currentorder = rnd.Next(0, 320 - this.count);
            this.count++;
        }

        public void OneTwoOne()
        {
            Random rnd = new Random();
            if (state == OneTwoOneState.FRONT)
            {
                this.currentorder = rnd.Next(0, this.lastorder);
                state = OneTwoOneState.FLLOWB;
            }
            else
                if (state == OneTwoOneState.FLLOWB)
                {
                    this.currentorder = this.lastorder;
                    state = OneTwoOneState.BACK;
                }
                else
                    if (state == OneTwoOneState.FLLOWF)
                    {
                        this.currentorder = this.lastorder;
                        state = OneTwoOneState.FRONT;
                    }
                    else
                    {
                        this.currentorder = rnd.Next(this.lastorder, 320 - this.count);
                        state = OneTwoOneState.FLLOWF;
                    }
            this.lastorder = this.currentorder;
            this.count++;
        }

        public int Check(int pagetemp)
        {
            int num = 0;
            foreach (var i in block)
            {
                if(i.pagenum == pagetemp)
                    return num;
                num++;
            }
            return -1;
        }

        public int Space()
        {
            int num = 0;
            foreach (var i in block)
            {
                if (i.pagenum == -1)
                    return num;
                num++;
            }
            return -1;
        }

        public void AddPage(int blockspace,int page)
        {
            this.block[blockspace].pagenum = page;
            this.block[blockspace].accessed=0;
            switch(blockspace)
            {
                case 0:
                    this.m.Block2.Fill = System.Windows.Media.Brushes.LightBlue;
                    this.m.Mask1.Content = "page" + page.ToString();
                    break;
                case 1:
                    this.m.Block1.Fill = System.Windows.Media.Brushes.LightBlue;
                    this.m.Mask2.Content = "page" + page.ToString();
                    break;
                case 2:
                    this.m.Block3.Fill = System.Windows.Media.Brushes.LightBlue;
                    this.m.Mask3.Content = "page" + page.ToString();
                    break;
                case 3:
                    this.m.Block4.Fill = System.Windows.Media.Brushes.LightBlue;
                    this.m.Mask4.Content = "page" + page.ToString();
                    break;
            }
        }

        public void LRU()
        {
            int page = this.temp[this.currentorder]/10;
            int check  = Check(page);
            if (check != -1)
            {
                this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "否 ", "无", "无" });
                this.block[check].accessed = 0;
            }
            else
            {
                int blockspace = Space();
                if (blockspace != -1)
                {
                    AddPage(blockspace, page);
                    this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "是 ", "无", page.ToString() });

                }
                else
                {
                    int num = this.block[0].accessed;
                    blockspace = 0;
                    for (int i = 1; i < 4; i++)
                    {
                        if (num < this.block[i].accessed)
                        {
                            num = this.block[i].accessed;
                            blockspace = i;
                        }
                    }
                    this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "是 ",  this.block[blockspace].pagenum.ToString(), page.ToString() });
                    AddPage(blockspace, page);
                }
            } 
            this.temp.Remove(temp[this.currentorder]);
            if (this.count == 320)
            {
            //    if (this.timer != null)
            //    {
                    this.m.Auto.Content = "自动";
                    this.m.Auto.IsEnabled = false;
                    this.available = false;
             //       this.timer.Dispose();
             //       this.timer = null;
            //    }
            }
        }
        public void FIFO()
        {
            int page = this.temp[this.currentorder]/10;
            if (Check(page) != -1)
            {
                this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "否 ", "无", "无" });
            }
            else
            {
                int blockspace = Space();
                if (blockspace != -1)
                {
                    AddPage(blockspace, page);
                    this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "是 ", "无", page.ToString() });
                }
                else
                {     
                    this.dt.Rows.Add(new object[] { this.count.ToString(), this.temp[this.currentorder].ToString(), page.ToString(), "是 ", this.block[fifo].pagenum.ToString(), page.ToString() });
                    AddPage(fifo, page);
                    fifo = (fifo+1)%4;
                }
            }
            this.temp.Remove(temp[this.currentorder]) ;
            if (this.count == 320)
            {
            //    if (this.timer != null)
            //    {
                    this.m.Auto.Content = "自动";
                    this.m.Auto.IsEnabled = false;
                this.available = false;
             //       this.timer.Dispose();
             //       this.timer = null;
             //   }
            }

        }

    }
}
