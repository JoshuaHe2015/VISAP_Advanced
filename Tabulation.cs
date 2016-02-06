using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Web.UI;
using System.Drawing;
namespace VISAP商科应用
{
    public class Tabulation
    {
        public static DataTable NewTable(int NumRows, int NumCols){
            //NewTable的作用是创建一张空白表格
            //NumRows为添加的行数，NumCols为添加的列数
            DataTable dt = new DataTable();
            int i;
            for (i = 0; i < NumCols; i++)
            {
                dt.Columns.Add("");
            }
            //添加列
            //默认会添加Colum1，Colum2，……
            for (i = 0; i < NumRows; i++)
            {
                dt.Rows.Add("");
            }
            //添加行
            //要多少列和行就add多少就好
            return dt;
        }
		
        public static DataTable ImportExcel(string filePath, string SheetName)
        {
            //该函数的目的是为了导入Excel表格，并返回DataTable
            //根据路径打开一个Excel文件并将数据填充到DataSet中
            //filePath为文件路径，SheetName为表格名称
            string strConn = "";
            if (Path.GetExtension(filePath) == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=NO;IMEX=1'";
            }
            else
            {
                //strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0;", path);
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=NO;IMEX=1'";
            }
            //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=NO;IMEX=1'";//导入时包含Excel中的第一行数据，并且将数字和字符混合的单元格视为文本进行导入
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            //strExcel = "select  * from   [sheet1$]";
            strExcel = "select  * from   [" + SheetName + "$]";
            //在这里选择Excel表格的名称
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            //根据DataGridView的列构造一个新的DataTable
            DataTable tb = new DataTable();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                DataColumn dc = new DataColumn();
                //dc.DataType = dgvc.ValueType;//若需要限制导入时的数据类型则取消注释，前提是DataGridView必须先绑定一个数据源那怕是空的DataTable
                tb.Columns.Add(dc);
            }

            //根据Excel的行逐一对上面构造的DataTable的列进行赋值
            foreach (DataRow excelRow in ds.Tables[0].Rows)
            {
                int i = 0;
                DataRow dr = tb.NewRow();
                foreach (DataColumn dc in tb.Columns)
                {
                    dr[dc] = excelRow[i];
                    i++;
                }
                tb.Rows.Add(dr);
            }
            //在DataGridView中显示导入的数据
            //dgv.DataSource = tb;
            return tb;
            //返回DataTable
        }
        public static DataTable ImportCSV()
        {
            string fileToLoad = " ";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv文件(*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileToLoad = openFileDialog1.FileName;
                //LoadFromCSVFile(fileToLoad);
                return LoadFromCSVFile(fileToLoad); ;
            }
            else
            {
                return null;
            }
        }
        private static DataTable LoadFromCSVFile(string pCsvPath)
        {

            DataTable dt = new DataTable();
            try
            {
                String line;
                String[] split = null;
                DataRow row = null;
                StreamReader sr = new StreamReader(pCsvPath, System.Text.Encoding.Default);
                //创建与数据源对应的数据列 
                line = sr.ReadLine();
                split = line.Split(',');
                foreach (String colname in split)
                {
                    dt.Columns.Add(colname, System.Type.GetType("System.String"));
                }
                //将数据填入数据表 
                int j = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    j = 0;
                    row = dt.NewRow();
                    split = line.Split(',');
                    foreach (String colname in split)
                    {
                        row[j] = colname;
                        j++;
                    }
                    dt.Rows.Add(row);
                }
                sr.Close();
                //显示数据 
                //this.dataGridView1.DataSource = table.DefaultView;
                return dt;
            }
            catch (Exception vErr)
            {
                MessageBox.Show(vErr.Message);
                return null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public static void ExportCSV(DataTable dt)
        {
            string path = "";

            //File info initialization
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "csv文件";
            ofd.FileName = "";
            ofd.RestoreDirectory = true;
            ofd.CreatePrompt = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            ofd.Filter = "csv文件(*.csv)|*.csv";
            string strName = string.Empty;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                strName = ofd.FileName;
            }
            if (strName == "")
            {
                MessageBox.Show("没有选择Excel文件！无法进行数据导入");
                return;
            }
            path = strName;



            System.IO.FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding());
            //Tabel header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");
            //Table body
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(DelQuota(dt.Rows[i][j].ToString()));
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();

            DownLoadFile(path);
        }

        private static bool DownLoadFile(string _FileName)
        {
            //导出CSV文件需要使用该函数
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenRead(_FileName);
                byte[] FileData = new byte[fs.Length];
                fs.Read(FileData, 0, (int)fs.Length);
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.AddHeader("Content-Type", "application/notepad");
                string FileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(_FileName));
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + System.Convert.ToChar(34) + FileName + System.Convert.ToChar(34));
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", fs.Length.ToString());
                System.Web.HttpContext.Current.Response.BinaryWrite(FileData);
                fs.Close();
                System.IO.File.Delete(_FileName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }
        }
        private static string DelQuota(string str)
        {
            //Delete special symbol
            //导出CSV文件需要使用该函数
            string result = str;
            string[] strQuota = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "`", ";", "'", ",", ".", "/", ":", "/,", "<", ">", "?" };
            for (int i = 0; i < strQuota.Length; i++)
            {
                if (result.IndexOf(strQuota[i]) > -1)
                    result = result.Replace(strQuota[i], "");
            }
            return result;
        }

        public static void ExportExcel(DataGridView dataGridView1)
        {
            //用于导出Excel文件
            //参数为dataGridView
            #region
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Export Excel File";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == "")
                return;
            Stream myStream;
            myStream = saveFileDialog.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            string str = "";
            try
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += dataGridView1.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                //行数减去1
                for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                {
                   
                    string tempStr = "";
                    for (int k = 0; k < dataGridView1.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        tempStr += dataGridView1.Rows[j].Cells[k].Value.ToString();
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
            #endregion
        }
        public static string GetGridCols(DataGridView dataGridView1){
            int selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
            int[] ColumnsChosen = new int[selectedCellCount];
            //声明数组，长度为选中的单元格数量
            ColumnsChosen = ColumnsSelected(dataGridView1);
            //单元格的列数不可能超过单元格数量
            string AllSelectedCol = "";
            int Record_Zero = 0;
            //对列数做记录
            if (ColumnsChosen != new int[] { -1, -1, -1, -1 })
            {
                Array.Sort(ColumnsChosen);
                //对列数进行排序
                for (int i = 0; i < selectedCellCount; i++)
                {
                    if (ColumnsChosen[i] >= 0)
                    {
                        Record_Zero = i;
                        //如果发现大于0的列数，则跳出循环
                        //之所以这样操作，原因在于，有可能一些数组中元素因空缺被赋值为-1
                        //所以要记录从何时开始为0，即第一列
                        break;
                    }
                }
                for (int i = Record_Zero; i < selectedCellCount; i++)
                {
                    if (i == Record_Zero)
                    {
                        AllSelectedCol = (ColumnsChosen[i] + 1).ToString();

                    }
                    else
                    {
                        AllSelectedCol = AllSelectedCol + "," + (ColumnsChosen[i] + 1).ToString();
                    }
                }
            }
            return AllSelectedCol;
        }
        private static int[] ColumnsSelected(DataGridView dataGridView1)
        {
            int counts = 0;
            int UseToIdentify = 0;
            int countforNew = 0;
            int selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
            if (selectedCellCount > 0)
            {
                int[] ColumnsChosen = new int[selectedCellCount];
                for (int i = 0; i < selectedCellCount; i++)
                {
                    ColumnsChosen[i] = -100;
                }
                if (dataGridView1.AreAllCellsSelected(true))
                {
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        counts = 0;
                        foreach (int EachCol in ColumnsChosen)
                        {
                            if (EachCol == i)
                            {
                                counts++;
                                break;
                            }
                        }
                        if (counts == 0)
                        {
                            ColumnsChosen[i] = i;

                        }
                        else
                        {
                            ColumnsChosen[i] = -100;
                        }

                    }

                }
                else
                {
                    for (int i = 0; i < selectedCellCount; i++)
                    {
                        UseToIdentify = dataGridView1.SelectedCells[i].ColumnIndex;
                        counts = 0;
                        foreach (int EachCol in ColumnsChosen)
                        {
                            if (EachCol == UseToIdentify)
                            {
                                counts++;
                                break;
                            }
                        }
                        if (counts == 0)
                        {
                            ColumnsChosen[countforNew] = UseToIdentify;
                            countforNew++;
                        }
                        else
                        {
                            ColumnsChosen[countforNew] = -100;
                            countforNew++;
                        }
                    }
                }
                return ColumnsChosen;
            }
            return new int[] { -1, -1, -1, -1 };
        }
        public static string GetColsName(DataGridView dataGridView1,string ColNums)
        {
            char [] separator = {','};
            //string是以逗号分隔的
            string[] AllNum = ColNums.Split(separator);
            //按照逗号分割
            string AllNames ="";
            foreach (string SingleNum in AllNum){
                if (SingleNum != ""){
                    AllNames += dataGridView1.Columns[Convert.ToInt32(SingleNum) - 1].Name+",";
                }
            }
            if (AllNames != ""){
                AllNames = AllNames.Substring(0,AllNames.Length -1);
            }
            return AllNames;
        }
        public static List<string> ReadVector(DataGridView dataGridView1, int ColNum){
            List <string> Values= new List <string>();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[ColNum].Value.ToString() != "")
                {
                    Values.Add(dataGridView1.Rows[i].Cells[ColNum].Value.ToString());
                }
            }
            return Values;
        }
        public static DataTable GetSubset(DataGridView dataGridView1, string Cols)
        {
            DataTable OriginDT = new DataTable();
            DataTable NewDT = new DataTable();
            OriginDT = dataGridView1.DataSource as DataTable;
            if (OriginDT == null)
            {
                return null;
            }
            DataColumn AddCol = new DataColumn();
            char[] separator = { ',' };
            string UseToAddCol = "";
            int AddTimes = 1;
            string[] ColWanted = Cols.Split(separator);
            if (OriginDT != null)
            {
                for (int q = 0; q < OriginDT.Rows.Count; q++)
                {
                    NewDT.Rows.Add();
                }
                for (int i = 0; i < OriginDT.Columns.Count; i++)
                {
                    foreach (string EachCol in ColWanted)
                    {
                        if (Convert.ToInt32(EachCol) - 1 == i)
                        {
                            NewDT.Columns.Add(OriginDT.Columns[i].ColumnName);
                            for (int m = 0; m < OriginDT.Rows.Count; m++)
                            {
                                UseToAddCol = OriginDT.Rows[m].ItemArray[i].ToString();
                                NewDT.Rows[m][OriginDT.Columns[i].ColumnName] = UseToAddCol;
                                AddTimes++;
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
            return NewDT;
        }
        public static int FindCol(DataGridView dataGridView1,string ID)
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (dataGridView1.Columns[i].Name == ID)
                {
                    return i;
                }
            }
            return -1;
        }
        private static bool IsStrDouble(string Str)
        {
            //判断一个字符串是否为双浮点型
            if (Str.Contains('E'))
            {
                int position = Str.IndexOf('E');
                if (Str.Substring(0, position - 1).Contains('E') || Str.Substring(position + 1, Str.Length).Contains('E'))
                {
                    return false;
                }
                if (IsStrDouble(Str.Substring(0, position - 1)) && IsStrDouble(Str.Substring(position + 2, Str.Length)))
                    //这里加2主要是为了去掉符号
                    //例：3.0E+12
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                int DecimalTimes = 0;
                //计算小数点出现的次数，最高不得超过一次
                for (int i = 0; i < Str.Length; i++)
                {
                    if (i == 0 && Str[0] == '-')
                    {
                        continue;
                    }
                    if (i == 0 && Str[0] == '+')
                    {
                        continue;
                    }
                    if (Str[i] == '1' || Str[i] == '2' 
                        ||Str[i] == '3' ||Str[i] == '4' 
                        ||Str[i] == '5' ||Str[i] == '6' 
                        ||Str[i] == '7' ||Str[i] == '8' ||
                        Str[i] == '9' || Str[i] == '0')
                    {
                        continue;
                    }
                    if (DecimalTimes > 1)
                    {
                        return false;
                    }
                    if (Str[i] == '.')
                    {
                        DecimalTimes++;
                        continue;
                    }
                    return false;
                }
                return true;
            }
        }
        public static void DataType(DataGridView datagridview1)
        {
            //这个函数的作用是分辨每一列的数据类型，并对字体进行变色
            //遍历datagridview每一个单元格
            for (int j = 0; j < datagridview1.Columns.Count; j++ )
            {
                for (int i = 0; i < datagridview1.Rows.Count - 1; i++)
                {
                    if (datagridview1.Rows[i].Cells[j].Value.ToString() != null && datagridview1.Rows[i].Cells[j].Value.ToString().Trim() != "")
                    {
                        if (!IsStrDouble(datagridview1.Rows[i].Cells[j].Value.ToString()))
                        {
                            //如果单元格内容为字符串
                            datagridview1.Columns[j].DefaultCellStyle.ForeColor = Color.Firebrick;
                            //整列设置为深红色，然后跳出内层循环
                            break;
                        }
                    }
                }
            }
        }
        public static void BulkImportCSV(DataGridView dataGridView1){
            //批量导入CSV文件
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv文件(*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = new DataTable();
                string[] files = openFileDialog1.FileNames.ToArray();
                foreach (string EachFile in files)
                {
                    dt.Merge(LoadFromCSVFile(EachFile));
                }
                dataGridView1.DataSource = dt;
            }
            
    }
    }
}
