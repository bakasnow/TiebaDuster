using System;
using DotNet4.Utilities;

public class http
{
    public static string Get(string url, string cookie = "")
    {
        HttpHelper http = new HttpHelper();
        HttpItem item = new HttpItem()
        {
            URL = url,//URL
            Method = "get",//URL
            IsToLower = false,//得到的HTML代码是否转成小写
            Cookie = cookie,//字符串Cookie
            Referer = "",//来源URL
            Postdata = "",//Post数据
            Timeout = 3000,//连接超时时间
            ReadWriteTimeout = 3000,//写入Post数据超时时间
            UserAgent = "bdtb for 7.8.1",//用户的浏览器类型，版本，操作系统
            ContentType = "text/html",//返回类型
            Allowautoredirect = false,//是否根据301跳转
            ProxyIp = "",//代理服务器ID
            ResultType = ResultType.String
        };
        HttpResult result = http.GetHtml(item);
        return result.Html;
        //string cookie = result.Cookie;
    }

    public static string Post(string url, string postData, string cookie = "")
    {
        HttpHelper http = new HttpHelper();
        HttpItem item = new HttpItem()
        {
            URL = url,//URL     必需项    
            Method = "post",//URL     可选项 默认为Get   
            IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
            Cookie = cookie,//字符串Cookie     可选项   
            Referer = "",//来源URL     可选项   
            Postdata = postData,//Post数据     可选项GET时不需要写   
            Timeout = 3000,//连接超时时间     可选项默认为100000    
            ReadWriteTimeout = 3000,//写入Post数据超时时间     可选项默认为30000   
            UserAgent = "bdtb for 7.8.1",//用户的浏览器类型，版本，操作系统     可选项有默认值   
            ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
            Allowautoredirect = false,//是否根据301跳转     可选项   
                                      //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                                      //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
            ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                         //ProxyPwd = "123456",//代理服务器密码     可选项    
                         //ProxyUserName = "administrator",//代理服务器账户名     可选项   
            ResultType = ResultType.String
        };
        HttpResult result = http.GetHtml(item);
        return result.Html;
        //string cookie = result.Cookie;
    }
}

public class snow
{
    public static string MD5加密(string str)
    {
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }

    }

    public static string BASE64编码(string str)
    {
        // 按照BASE64进行编码
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(str));
        }

    }

    public static string URL编码(string str)
    {
        // 按照GBK进行编码
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            string text = System.Web.HttpUtility.UrlEncode(str, System.Text.Encoding.GetEncoding("GBK"));
            return text.ToUpper();
        }
    }

    public static string URL解码(string str)
    {
        // 按照GBK进行解码
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            string text = System.Web.HttpUtility.UrlDecode(str, System.Text.Encoding.GetEncoding("GBK"));
            return text.ToUpper();
        }
    }

    public static string URL编码UTF8(string str)
    {
        // 按照UTF-8进行编码
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            return Uri.EscapeDataString(str);
        }
    }

    public static string URL解码UTF8(string str)
    {
        // 按照UTF-8进行解码
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            return Uri.UnescapeDataString(str);
        }
    }

    public static string usc2转ansi(string str)
    {
        if (str == "" || str == null)
        {
            return "";
        }
        else
        {
            System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(str, "\\\\u([\\w]{4})");
            if (mc != null && mc.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match m2 in mc)
                {
                    string v = m2.Value;
                    string word = v.Substring(2);
                    byte[] codes = new byte[2];
                    int code = Convert.ToInt32(word.Substring(0, 2), 16);
                    int code2 = Convert.ToInt32(word.Substring(2), 16);
                    codes[0] = (byte)code2;
                    codes[1] = (byte)code;
                    str = str.Replace(v, System.Text.Encoding.Unicode.GetString(codes));
                }
            }
            return str;
        }
    }

    public static string 截取文本(string str, string str1, string str2)
    {
        //隐雨轩某大神写的
        int leftlocation;//左边位置
        int rightlocation;//右边位置
        int strmidlength; ;//中间字符串长度
        string strmid;//中间字符串
        leftlocation = str.IndexOf(str1);//获取左边字符串头所在位置
        if (leftlocation == -1)//判断左边字符串是否存在于总字符串中
        {
            return "";
        }
        leftlocation = leftlocation + str1.Length;//获取左边字符串尾所在位置 
        rightlocation = str.IndexOf(str2, leftlocation);//获取右边字符串头所在位置
        if (rightlocation == -1 || leftlocation > rightlocation)//判断右边字符串是否存在于总字符串中，左边字符串位置是否在右边字符串前
        {
            return "";
        }
        strmidlength = rightlocation - leftlocation;//计算中间字符串长度
        strmid = str.Substring(leftlocation, strmidlength);//取出中间字符串
        return strmid;//返回中间字符串
    }

    public static string 读入文件(string 文件路径)
    {
        string 返回文本 = "";

        try
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(文件路径, System.Text.Encoding.Default);
            返回文本 = sr.ReadToEnd();
            sr.Close();
            return 返回文本;
        }
        catch
        {
            return "";
        }
    }

    public static void 写到文件(string 路径, string text)
    {
        try
        {
            //System.IO.FileMode.Create 覆盖原文件
            System.IO.FileStream fs = new System.IO.FileStream(路径, System.IO.FileMode.Create);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
            sw.Write(text);
            sw.Close();
            fs.Close();
        }
        catch
        {

        }

    }

    public static string[] 合并数组(string[] 前置文本, string[] 后置文本)
    {
        string[] result = new string[前置文本.Length + 后置文本.Length];
        前置文本.CopyTo(result, 0);
        后置文本.CopyTo(result, 前置文本.Length);
        return result;
    }

    public static bool 文本是否为空(string 文本)
    {
        if (文本 == null || 文本 == "")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string 安卓取Stamp()
    {
        Random ra = new Random();

        int[] stamp;
        stamp = new int[4];
        stamp[0] = ra.Next(10000, 99999);
        stamp[1] = ra.Next(1000, 9999);
        stamp[2] = ra.Next(1000, 9999);
        stamp[3] = ra.Next(100, 999);

        return "wappc_" + stamp[0] + stamp[1] + stamp[2] + "_" + stamp[3];
    }

    public static string 百度tbs(string cookie)
    {
        string str = http.Get("http://tieba.baidu.com/dc/common/tbs", cookie);
        return 截取文本(str, "tbs\":\"", "\"");
    }

    public static string 贴吧fid(string tiebaname)
    {
        string str = http.Get("http://tieba.baidu.com/f/commit/share/fnameShareApi?fname=" + URL编码UTF8(tiebaname) + "&ie=utf-8");
        return 截取文本(str, "fid\":", ",\"can");
    }

    public static string 取用户名(string cookie)
    {
        string str = http.Get("http://tieba.baidu.com/f/user/json_userinfo", cookie);
        return usc2转ansi(截取文本(str, "user_name_weak\":\"", "\""));
    }

    public static string 百度uid(string 用户名)
    {
        string str = http.Get("http://tieba.baidu.com/i/sys/user_json?un=" + URL编码(用户名));
        string uid = 截取文本(str, "\"id\":", ",");
        if (uid == "0" || uid == "")
        {
            str = http.Get("http://tieba.baidu.com/home/main?un=" + URL编码(用户名) + "&fr=home");
            uid = 截取文本(str, "home_user_id\" : ", ",");
        }
        return uid;
    }

    public static string 贴吧uid(string 用户名)
    {
        string str = http.Get("http://tieba.baidu.com/i/sys/user_json?un=" + URL编码(用户名));
        return 截取文本(str, "itieba_id\":", ",\"is_online");
    }

    public static string 取Sign(string str)
    {
        return MD5加密(URL解码UTF8(str.Replace("&", "") + "tiebaclient!!!")).ToUpper();
    }

    public static string 时间处理(int 输出类型, bool 边框)
    {
        string[] time = new string[6];

        //年
        time[0] = DateTime.Now.Year.ToString();
        while (time[0].Length < 4)
        {
            time[0] = "0" + time[0];
        }

        //月
        time[1] = DateTime.Now.Month.ToString();
        time[1] = time[1].Length < 2 ? "0" + time[1] : time[1];

        //日
        time[2] = DateTime.Now.Day.ToString();
        time[2] = time[2].Length < 2 ? "0" + time[2] : time[2];

        //时
        time[3] = DateTime.Now.Hour.ToString();
        time[3] = time[3].Length < 2 ? "0" + time[3] : time[3];

        //分
        time[4] = DateTime.Now.Minute.ToString();
        time[4] = time[4].Length < 2 ? "0" + time[4] : time[4];

        //秒
        time[5] = DateTime.Now.Second.ToString();
        time[5] = time[5].Length < 2 ? "0" + time[5] : time[5];

        string 返回文本;
        if (输出类型 == 2)//时分秒
        {
            返回文本 = time[3] + ":" + time[4] + ":" + time[5];
        }
        else if (输出类型 == 1)//年月日
        {
            返回文本 = time[0] + "/" + time[1] + "/" + time[2];
        }
        else//年月日时分秒
        {
            //DateTime.Now 没有格式化 不使用
            返回文本 = time[0] + "/" + time[1] + "/" + time[2] + " " + time[3] + ":" + time[4] + ":" + time[5];
        }

        if (边框)
        {
            返回文本 = "[" + 返回文本 + "]";
        }

        return 返回文本;
    }

    public static string 取时间戳()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static string 取网络资源MD5(string 资源链接)
    {
        if (资源链接 == "" || 资源链接 == null)
        {
            return "";
        }
        else
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            try
            {
                // 通过网络链接读取文件
                System.Net.WebClient http = new System.Net.WebClient();

                // 转换成MD5，此时的MD5是byte
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(http.DownloadData(资源链接));

                // 用这个sb把byte转换成string
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                // 输出string形式的MD5
                return sb.ToString();
            }
            catch
            {
                // 迷之报错提示
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
                return "";
            }
        }
    }
}

public class Access
{
    public static string getStr(string 参数_数据库路径, string 参数_SQL语句)
    {
        //初始化
        string 返回文本 = "";

        try
        {
            //1、建立连接 C#操作Access之读取mdb
            string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + 参数_数据库路径 + ";";
            System.Data.OleDb.OleDbConnection odcConnection = new System.Data.OleDb.OleDbConnection(strConn);

            //2、打开连接 C#操作Access之读取mdb
            odcConnection.Open();

            //建立SQL查询   
            System.Data.OleDb.OleDbCommand odCommand = odcConnection.CreateCommand();

            //3、输入查询语句 C#操作Access之读取mdb
            odCommand.CommandText = 参数_SQL语句;

            //建立读取
            System.Data.OleDb.OleDbDataReader odrReader = odCommand.ExecuteReader();

            //查询并显示数据
            int size = odrReader.FieldCount;

            odrReader.Read();
            返回文本 = odrReader[odrReader.GetName(0)].ToString();

            //关闭连接 C#操作Access之读取mdb
            odrReader.Close();
            odcConnection.Close();

            return 返回文本;
        }
        catch
        {
            return 返回文本;
        }
    }

    public static System.Data.DataTable getTable(string 参数_数据库路径, string 参数_SQL语句)
    {
        //初始化
        System.Data.DataTable dt = new System.Data.DataTable();

        try
        {
            //1、建立连接 C#操作Access之读取mdb
            string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + 参数_数据库路径 + ";";
            System.Data.OleDb.OleDbConnection odcConnection = new System.Data.OleDb.OleDbConnection(strConn);

            //2、打开连接 C#操作Access之读取mdb
            odcConnection.Open();

            //建立SQL查询   
            System.Data.OleDb.OleDbCommand odCommand = odcConnection.CreateCommand();

            //3、输入查询语句 C#操作Access之读取mdb
            odCommand.CommandText = 参数_SQL语句;

            //建立读取
            System.Data.OleDb.OleDbDataReader odrReader = odCommand.ExecuteReader();

            //查询并显示数据
            int size = odrReader.FieldCount;

            for (int i = 0; i < size; i++)
            {
                //Console.WriteLine("{0} {1}", "列", odrReader.GetName(i).ToString());
                dt.Columns.Add(odrReader.GetName(i).ToString(), typeof(string));
            }

            while (odrReader.Read())
            {
                System.Data.DataRow dr = dt.NewRow();
                for (int i = 0; i < size; i++)
                {
                    //Console.WriteLine("{0} {1}", odrReader.GetName(i).ToString(), odrReader[odrReader.GetName(i)].ToString());
                    dr[odrReader.GetName(i).ToString()] = odrReader[odrReader.GetName(i)].ToString();
                }
                dt.Rows.Add(dr);
            }

            //关闭连接 C#操作Access之读取mdb
            odrReader.Close();
            odcConnection.Close();

            return dt;
        }
        catch
        {
            return dt;
        }
    }

    public static bool set(string 参数_数据库路径, string 参数_SQL语句)
    {
        try
        {
            System.Data.OleDb.OleDbConnection odcConnection = new System.Data.OleDb.OleDbConnection();
            odcConnection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + 参数_数据库路径 + ";";
            odcConnection.Open();

            string accessstr = 参数_SQL语句;
            System.Data.OleDb.OleDbCommand odCommand = new System.Data.OleDb.OleDbCommand(accessstr, odcConnection);

            int odcEN = odCommand.ExecuteNonQuery();

            odcConnection.Close();

            if (odcEN > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    #region "保留源码"

    /*

    // 初始化
    protected static System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
    protected static System.Data.OleDb.OleDbCommand comm = new System.Data.OleDb.OleDbCommand();

    /// <summary>
    /// 打开数据库
    /// </summary>
    private static void openConnection(string source)
    {
        if (conn.State == System.Data.ConnectionState.Closed)
        {
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + source + ";";
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch (Exception e)
            { throw new Exception(e.Message); }

        }

    }

    /// <summary>
    /// 根据sql语句获取表信息
    /// </summary>
    /// <param name="source">数据库路径</param>
    /// <param name="sqlstr">sql语句</param>
    /// <returns></returns>
    public static System.Data.DataTable getTable2(string source, string sqlstr)
    {
        System.Data.DataTable dt = new System.Data.DataTable();
        System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter();
        try
        {
            openConnection(source);
            comm.CommandType = System.Data.CommandType.Text;
            comm.CommandText = sqlstr;
            da.SelectCommand = comm;
            da.Fill(dt);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        {
            closeConnection();
        }
        return dt;
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    private static void closeConnection()
    {
        if (conn.State == System.Data.ConnectionState.Open)
        {
            conn.Close();
            conn.Dispose();
            comm.Dispose();
        }
    }

    /// <summary>
    /// 执行sql语句
    /// </summary>
    /// <param name="source">数据库路径</param>
    /// <param name="sqlstr"></param>
    public static void excuteSql(string source, string sqlstr)
    {
        try
        {
            openConnection(source);
            comm.CommandType = System.Data.CommandType.Text;
            comm.CommandText = sqlstr.ToString();
            comm.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        { closeConnection(); }
    }

    */

    #endregion

}