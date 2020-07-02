using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Data;

namespace TiebaDuster
{
    class Program
    {
        #region "专用函数"
        public static void 控制台_输出(int 颜色, string 文本)
        {
            //初始化字体颜色为白色
            Console.ForegroundColor = ConsoleColor.White;

            if (颜色 == 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (颜色 == 11)
            {
                全局.报错计数 = 0;//清零，只统计连续报错
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (颜色 == 12)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (颜色 == 13)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }

            Console.WriteLine(snow.时间处理(2, false) + " " + 文本);

            if (全局.报错计数 > 30)
            {
                if (MessageBox.Show("程序错误过多，已自动停机\n\n点击“是”解除限制\n点击“否”关闭程序", "笨蛋雪说：", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    全局.报错计数 = 0;
                }
                else
                {
                    Environment.Exit(0);
                }
            }

        }

        public static void 等待(int 时间)
        {
            int time = Environment.TickCount;
            do
            {
                Thread.Sleep(1);
            } while (Environment.TickCount - time < 时间);
        }

        public static string 文本过滤(string 原文本)
        {
            string 临时文本 = 原文本;

            //格式化顺序：转半角 >> 转小写 >> 转简体 >> 删空格 >> 删换行
            临时文本 = 繁体转简体(全角转半角(临时文本).ToLower()).Replace(" ", "").Replace("\n", "").Replace("\r", "");

            //文本过滤
            for (int i = 0; i < 全局.字符过滤.Length; i++)
            {
                临时文本 = 临时文本.Replace(全局.字符过滤[i], "");
            }
            for (int i = 0; i < 全局.文本替换.Length / 2; i++)
            {
                临时文本 = 临时文本.Replace(全局.文本替换[i, 0], 全局.文本替换[i, 1]);
            }

            return 临时文本;
        }

        public static string 全角转半角(string input)
        {
            if (input == "" || input == null)
            {
                return "";
            }

            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }

        public static string 繁体转简体(string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }

        public static string 简易正则(string 原文本, string 表达式, int 子索引)
        {
            Regex 正则;
            MatchCollection 结果;
            正则 = new Regex(表达式);
            结果 = 正则.Matches(原文本);
            if (结果.Count > 0)
            {
                return 结果[0].Groups[子索引].Value;
            }
            else
            {
                return "";
            }
        }

        public static void 操作量统计(int 类型)
        {
            //0报错 1删帖 2封号
            if (类型 == 0)
            {
                全局.报错计数 += 1;
            }
            else if (类型 == 1)
            {
                全局.删帖量 += 1;
            }
            else if (类型 == 2)
            {
                全局.封号量 += 1;
            }
        }

        public static int 取用户某吧等级(string 用户名, string 贴吧名)
        {
            string Cookie = null;
            string url = "http://c.tieba.baidu.com/c/f/forum/like";
            string postString
                = Cookie
                + "&_client_id="
                + snow.安卓取Stamp()
                + "&_client_type=2&_client_version=7.8.1&friend_uid="
                + snow.百度uid(用户名)
                + "&is_guest=1&page_no=1&uid=1";

            postString += "&sign=" + snow.取Sign(postString);

            string srcString = http.Post(url, postString);

            Regex 正则;
            MatchCollection 结果;

            string 正则表达式 = "{\"id\":\"[0-9]*\",\"name\":\"(.*?)\",\"favo_type\":\"[0-9]*\",\"level_id\":\"([0-9]*)\",\"level_name\":\".*?\",\"cur_score\":\"[0-9]*\",\"levelup_score\":\"[0-9]*\",\"avatar\":\".*?\",\"slogan\":\".*?\"}";
            int 等级 = 0;

            正则 = new Regex(正则表达式);
            结果 = 正则.Matches(srcString);
            for (int i = 0; i < 结果.Count; i++)
            {
                if (snow.usc2转ansi(结果[i].Groups[1].Value) == 贴吧名)
                {
                    等级 = Convert.ToInt32(结果[i].Groups[2].Value);
                    break;
                }
            }

            return 等级;
        }

        public static DataTable 取全部贴吧等级(string 用户名)
        {
            string Cookie = "";
            string url = "http://c.tieba.baidu.com/c/f/forum/like";
            //Console.WriteLine(snow.百度uid(用户名));
            string postString
                = Cookie
                + "&_client_id="
                + snow.安卓取Stamp()
                + "&_client_type=2&_client_version=5.8.0&friend_uid="
                + snow.百度uid(用户名)
                + "&is_guest=1&page_no=1&uid=1";

            postString += "&sign=" + snow.取Sign(postString);

            string srcString = http.Post(url, postString);

            Regex 正则;
            MatchCollection 结果;

            string 正则表达式 = "{\"id\":\"[0-9]*\",\"name\":\"(.*?)\",\"favo_type\":\"[0-9]*\",\"level_id\":\"([0-9]*)\",\"level_name\":\".*?\",\"cur_score\":\"[0-9]*\",\"levelup_score\":\"[0-9]*\",\"avatar\":\".*?\",\"slogan\":\".*?\"}";

            正则 = new Regex(正则表达式);
            结果 = 正则.Matches(srcString);

            DataTable dt = new DataTable();
            dt.Columns.Add("贴吧名", typeof(string));
            dt.Columns.Add("等级", typeof(string));

            for (int i = 0; i < 结果.Count; i++)
            {
                dt.Rows.Add(snow.usc2转ansi(结果[i].Groups[1].Value), Convert.ToInt32(结果[i].Groups[2].Value));
            }

            return dt;
        }

        public static bool 文本规则(string 用户名, string 表达式)
        {

            Regex 正则;
            MatchCollection 结果;

            正则 = new Regex(表达式);
            结果 = 正则.Matches(用户名);

            if (结果.Count > 0)
            {
                if (结果[0].Value == 用户名)
                {
                    return true;
                }
            }

            return false;

        }

        public static string 取用户吧龄(string 用户名)
        {
            try
            {
                string str = http.Get("http://tieba.baidu.com/home/get/panel?ie=utf-8&un=" + snow.URL编码UTF8(用户名));
                return snow.截取文本(str, "tb_age\":\"", "\"");
            }
            catch
            {
                return "0";
            }

        }

        public static int 取头像上传时间(string 用户名)
        {
            try
            {
                string str = http.Get("http://tieba.baidu.com/home/get/panel?ie=utf-8&un=" + snow.URL编码UTF8(用户名));
                string 临时文本 = snow.截取文本(str, "portrait_time\":", ",");
                if (临时文本 == "")
                {
                    临时文本 = "0";
                }

                return Convert.ToInt32(临时文本);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        //主程序
        static void Main(string[] args)
        {
            //测试用代码
            DataTable Debug = 取全部贴吧等级("哎呦喂我去去去");
            foreach (DataRow dr in Debug.Rows)
            {
                控制台_输出(0, dr["贴吧名"].ToString() + " " + dr["等级"].ToString());
            }
            等待(1);

            //Console.WriteLine(Access.setAccess(Application.StartupPath + "\\配置文件.mdb", "update 内容关键词 SET 触发次数 = '10' WHERE 关键词 = '=511,640#'"));


            //初始化
            Console.Title = "鸡毛掸子 v" + 全局.版本号;
            Console.ForegroundColor = ConsoleColor.White;//初始化字体颜色
            Console.BackgroundColor = ConsoleColor.Black;//初始化背景颜色

            string 访问Cookie, 访问账号, fid;
            string[] 贴吧名 = new string[3];
            int 主题获取间隔, 帖子访问间隔, 主题删除间隔, 回复删除间隔, 主题最大缓存;
            bool 监控主题, 监控回复, 图片特征, 启用违规用户, 启用贴吧黑名单;

            DataTable 主题帖缓存 = new DataTable();
            主题帖缓存.Columns.Add("tid", typeof(string));
            主题帖缓存.Columns.Add("reply_num", typeof(int));//回复数
            主题帖缓存.Columns.Add("last_time", typeof(int));//最后回复时间

            DataTable 标题关键词, 一楼关键词, 内容关键词, 信任用户, 违规用户, 帖号白名单, 贴吧黑名单;

            string 数据库路径 = Application.StartupPath + "\\配置文件.mdb";

            if (!File.Exists(数据库路径))
            {
                控制台_输出(10, "数据库异常：找不到配置文件，任意键退出。");
                Console.ReadKey();
                return;
            }

            //全局.帐号库 = Access.getTable(数据库路径, "select 用户名,Cookie from Cookies WHERE 帐号用途 = '删帖'");
            //for (int i = 全局.帐号库.Rows.Count - 1; i >= 0; i--)
            //{
            //    if (snow.取用户名(全局.帐号库.Rows[i]["Cookie"].ToString()) == 全局.帐号库.Rows[i]["用户名"].ToString())
            //    {
            //        控制台_输出(11, "帐号状态验证：" + 全局.帐号库.Rows[i]["用户名"].ToString() + " 在线可用。");
            //    }
            //    else
            //    {
            //        控制台_输出(10, "帐号状态异常：" + 全局.帐号库.Rows[i]["用户名"].ToString() + " Cookie已失效。");
            //        全局.帐号库.Rows[i].Delete();
            //    }
            //}

            //等级墙参数
            string 默认头像MD5 = "ce3718fa249200e715a3551165731a0f";
            string 作者头像MD5;

            //初始化部署参数
            部署参数 部署参数_外部 = new 部署参数();

            //记录数据库更新时间
            FileInfo dbFi = new FileInfo(数据库路径);
            string dbTime = dbFi.LastWriteTime.ToString();

            while (true)
            {
                //读取Cookie
                访问Cookie = Access.getStr(数据库路径, "select Cookie from Cookies WHERE 帐号用途 = '访问'");
                访问账号 = Access.getStr(数据库路径, "select 用户名 from Cookies WHERE 帐号用途 = '访问'");

                //while (snow.取用户名(访问Cookie) != 访问账号)
                //{
                //    控制台_输出(10, "帐号状态异常：" + 访问账号 + " Cookie已失效，将在 3 秒后重试...");
                //    等待(3000);
                //}

                //运行参数
                贴吧名[0] = Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '所在贴吧'");
                贴吧名[1] = snow.URL编码(贴吧名[0]);
                贴吧名[2] = snow.URL编码UTF8(贴吧名[0]);

                主题获取间隔 = Convert.ToInt32(Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '主题获取间隔'"));
                帖子访问间隔 = Convert.ToInt32(Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '帖子访问间隔'"));
                主题删除间隔 = Convert.ToInt32(Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '主题删除间隔'"));
                回复删除间隔 = Convert.ToInt32(Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '回复删除间隔'"));
                主题最大缓存 = Convert.ToInt32(Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '主题最大缓存'"));

                //更新主题缓存
                if (主题帖缓存.Rows.Count > 主题最大缓存)
                {
                    主题帖缓存.Rows.Clear();
                }

                //更新标题
                Console.Title = "鸡毛掸子 v" + 全局.版本号 + "  帐号：" + 访问账号 + "  贴吧：" + 贴吧名[0] + "  缓存：" + 主题帖缓存.Rows.Count.ToString();


                if (Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '监控主题'") == "1")
                {
                    监控主题 = true;
                }
                else
                {
                    监控主题 = false;
                }

                if (Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '监控回复'") == "1")
                {
                    监控回复 = true;
                }
                else
                {
                    监控回复 = false;
                }

                if (Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '图片特征'") == "1")
                {
                    图片特征 = true;
                }
                else
                {
                    图片特征 = false;
                }

                if (Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '违规用户'") == "1")
                {
                    启用违规用户 = true;
                }
                else
                {
                    启用违规用户 = false;
                }

                if (Access.getStr(数据库路径, "select 内容 from 运行参数 WHERE 参数名 = '贴吧黑名单'") == "1")
                {
                    启用贴吧黑名单 = true;
                }
                else
                {
                    启用贴吧黑名单 = false;
                }

                //关键词列表
                标题关键词 = Access.getTable(数据库路径, "SELECT 索引,关键词,正则匹配,等级小于,处理方式,封号理由,执行时间,调试输出 FROM 标题关键词");
                一楼关键词 = Access.getTable(数据库路径, "SELECT 索引,关键词,正则匹配,等级小于,处理方式,封号理由,执行时间,调试输出 FROM 一楼关键词");
                内容关键词 = Access.getTable(数据库路径, "SELECT 索引,关键词,正则匹配,等级小于,处理方式,封号理由,执行时间,调试输出 FROM 内容关键词");

                违规用户 = Access.getTable(数据库路径, "SELECT 索引,用户名,正则匹配,等级大于,是否封号,封号理由 FROM 违规用户");
                贴吧黑名单 = Access.getTable(数据库路径, "SELECT 贴吧名,匹配方式,等级大于 FROM 贴吧黑名单");

                /*foreach (DataRow dr in 贴吧黑名单.Rows)
                {
                    控制台_输出(0, dr["贴吧名"].ToString()+" "+ dr["匹配方式"].ToString()+" "+ dr["等级大于"].ToString());
                }*/

                //用户名单
                帖号白名单 = Access.getTable(数据库路径, "SELECT 帖号 FROM 帖号白名单");
                信任用户 = Access.getTable(数据库路径, "SELECT 用户名 FROM 信任用户");

                //加载文本过滤
                全局.字符过滤 = Regex.Split(snow.读入文件(Application.StartupPath + "\\文本过滤\\字符过滤.txt"), "\r\n", RegexOptions.IgnoreCase);
                string[] 分割文本 = Regex.Split(snow.读入文件(Application.StartupPath + "\\文本过滤\\文本替换.txt"), "\r\n", RegexOptions.IgnoreCase);
                string[,] 文本替换 = new string[分割文本.Length, 2];
                string[] 临时文本;
                for (int i = 0, count = 分割文本.Length; i < count; i++)
                {
                    临时文本 = 分割文本[i].Split(',');
                    文本替换[i, 0] = 临时文本[0];
                    文本替换[i, 1] = 临时文本[1];
                }
                全局.文本替换 = 文本替换;


                //贴吧fid
                fid = "";
                while (true)
                {
                    fid = snow.贴吧fid(贴吧名[0]);
                    if (fid != "")
                    {
                        break;
                    }
                    else
                    {
                        控制台_输出(10, 贴吧名[0] + "吧 fid参数获取失败，将在 3 秒后重试...");
                        等待(3000);
                    }
                }


                //while (snow.取用户名(访问Cookie) != 访问账号)
                //{
                //    控制台_输出(10, "帐号状态异常：请检查Cookie与配置文件，将在 3 秒后重试...");
                //    等待(3000);
                //}

                //时间初始化
                int[] timeTampInt = new int[3];
                string[] timeTampStr = new string[2];
                string[] timeSplit = new string[0];
                timeTampStr[0] = DateTime.Now.Hour.ToString();
                timeTampStr[1] = DateTime.Now.Minute.ToString();
                while (timeTampStr[0].Length < 2)
                {
                    timeTampStr[0] = "0" + timeTampStr[0];
                }
                while (timeTampStr[1].Length < 2)
                {
                    timeTampStr[1] = "0" + timeTampStr[1];
                }
                timeTampInt[0] = Convert.ToInt32((timeTampStr[0] + timeTampStr[1]));

                string 主题链接 = "http://c.tieba.baidu.com/c/f/frs/page";
                string 主题参数文本 = 访问Cookie + "&_client_id=" + snow.安卓取Stamp() + "&_client_type=2&_client_version=7.8.1&kw=" + 贴吧名[2] + "&pn=1&rn=50";

                主题参数文本 += "&sign=" + snow.取Sign(主题参数文本);

                string 主题返回文本 = http.Post(主题链接, 主题参数文本, 访问Cookie);

                //snow.写到文件(@"E:\Desktop\主题源码1.txt", snow.usc2转ansi(主题返回文本));

                if (主题返回文本 == "")
                {
                    控制台_输出(10, 贴吧名[0] + "吧 主题帖获取失败，将在 3 秒后重试...");
                    等待(3000);
                    continue;
                }

                JObject 主题源码 = new JObject();
                try
                {
                    主题源码 = JObject.Parse(主题返回文本);
                }
                catch
                {
                    控制台_输出(10, "主题解析失败。");
                    等待(1000);
                    continue;
                }

                if (主题源码["error_code"].ToString() != "0")
                {
                    控制台_输出(10, 贴吧名[0] + "吧 主题获取失败：" + 主题源码["error_msg"].ToString());
                    等待(2000);
                    continue;
                }

                JArray 主题jar = (JArray)主题源码["thread_list"];

                控制台_输出(12, "获取到" + 主题jar.Count.ToString() + "个主题");

                主题参数结构 主题参数 = new 主题参数结构();
                for (int 主题计数 = 0; 主题计数 < 主题jar.Count; 主题计数++)
                {
                    //跳过直播帖
                    if ((string)主题源码["thread_list"][主题计数]["is_livepost"] == "1")
                    {
                        goto 至下一个帖子;
                    }

                    try
                    {
                        //主题参数
                        主题参数.tid = (string)主题源码["thread_list"][主题计数]["tid"];

                        //帖号白名单
                        for (int 计数 = 0; 计数 < 帖号白名单.Rows.Count; 计数++)
                        {
                            if (主题参数.tid == 帖号白名单.Rows[计数]["帖号"].ToString())
                            {
                                goto 至下一个帖子;
                            }
                        }

                        主题参数.标题 = 文本过滤((string)主题源码["thread_list"][主题计数]["title"]);
                        主题参数.回复数 = (int)主题源码["thread_list"][主题计数]["reply_num"];
                        主题参数.最后回复时间 = (int)主题源码["thread_list"][主题计数]["last_time_int"];

                        //帖子缓存
                        bool 主题帖缓存_帖号是否存在 = false;
                        for (int 计数 = 0; 计数 < 主题帖缓存.Rows.Count; 计数++)
                        {
                            if (主题帖缓存.Rows[计数]["tid"].ToString() == 主题参数.tid)
                            {
                                主题帖缓存_帖号是否存在 = true;
                                if (Convert.ToInt32(主题帖缓存.Rows[计数]["last_time"]) == 主题参数.最后回复时间)
                                {
                                    goto 至下一个帖子;
                                }
                                else
                                {
                                    主题帖缓存.Rows[计数]["last_time"] = 主题参数.最后回复时间;
                                    break;
                                }
                            }
                        }
                        if (主题帖缓存_帖号是否存在 == false)
                        {
                            主题帖缓存.Rows.Add(主题参数.tid, 主题参数.回复数, 主题参数.最后回复时间);
                        }

                        //直播帖
                        if ((string)主题源码["thread_list"][主题计数]["thread_type"] == "33")
                        {
                            //直播帖没有这些参数
                            主题参数.发帖时间 = 0;
                            主题参数.是否置顶
                            = 主题参数.是否精品
                            = 主题参数.是否会员置顶
                            = 主题参数.是否视频帖
                            = false;
                        }
                        else
                        {
                            //是否话题帖
                            if (主题源码["thread_list"][主题计数].ToString().IndexOf("create_time") != -1)
                            {
                                主题参数.发帖时间 = (int)主题源码["thread_list"][主题计数]["create_time"];
                            }
                            else
                            {
                                //话题帖没有发帖时间
                                主题参数.发帖时间 = 0;
                            }

                            //通用参数                         
                            主题参数.是否置顶 = (string)主题源码["thread_list"][主题计数]["is_top"] == "1" ? true : false;
                            主题参数.是否精品 = (string)主题源码["thread_list"][主题计数]["is_good"] == "1" ? true : false;
                            主题参数.是否会员置顶 = (string)主题源码["thread_list"][主题计数]["is_membertop"] == "1" ? true : false;
                            主题参数.是否视频帖 = (string)主题源码["thread_list"][主题计数]["thread_type"] == "40" ? true : false;
                        }

                    }
                    catch
                    {
                        控制台_输出(10, "帖号：" + 主题参数.tid + " 主题参数获取失败");
                        goto 至下一个帖子;
                    }

                    if (!监控主题)
                    {
                        goto 至帖子内容;
                    }

                    #region "主题UID处理"
                    JObject 主题UID源码 = JObject.Parse(主题返回文本);
                    JArray 主题UIDjar = (JArray)主题UID源码["user_list"];

                    string[,] 主题UID列表 = new string[主题UIDjar.Count, 3];
                    for (int 主题UID计数 = 0; 主题UID计数 < 主题UIDjar.Count; 主题UID计数++)
                    {
                        try
                        {
                            主题UID列表[主题UID计数, 0] = (string)主题UID源码["user_list"][主题UID计数]["id"];
                            主题UID列表[主题UID计数, 1] = (string)主题UID源码["user_list"][主题UID计数]["name_show"];
                            主题UID列表[主题UID计数, 2] = (string)主题UID源码["user_list"][主题UID计数]["portrait"];
                        }
                        catch
                        {
                            主题UID列表[主题UID计数, 0]
                                = 主题UID列表[主题UID计数, 1]
                                = 主题UID列表[主题UID计数, 2]
                                = "#获取失败#";

                            控制台_输出(10, 贴吧名[0] + "吧 用户UID列表：获取失败。");
                        }
                    }
                    #endregion

                    //楼主信息
                    主题参数.作者 = 主题参数.作者头像 = "#获取失败#";
                    for (int i = 0; i < 主题UID列表.Length / 3; i++)
                    {
                        //匹配UID
                        if (主题UID列表[i, 0] == (string)主题源码["thread_list"][主题计数]["author_id"])
                        {
                            主题参数.作者 = 主题UID列表[i, 1];
                            主题参数.作者头像 = 主题UID列表[i, 2];
                            break;
                        }
                    }

                    //判断帖子类型
                    if (主题参数.是否置顶 || 主题参数.是否精品 || 主题参数.是否会员置顶)
                    {
                        goto 至下一个帖子;//统一写法
                        //continue;//到循环尾
                    }

                    if (主题参数.作者.ToString() != "")
                    {
                        //是否白名单用户
                        for (int 计数 = 0; 计数 < 信任用户.Rows.Count; 计数++)
                        {
                            if (主题参数.作者 == 信任用户.Rows[计数]["用户名"].ToString())
                            {
                                goto 至下一个帖子;
                            }
                        }

                        //是否违规用户
                        if (启用违规用户)
                        {
                            for (int 计数 = 0; 计数 < 违规用户.Rows.Count; 计数++)
                            {
                                if (违规用户.Rows[计数]["等级大于"].ToString() != "-1")
                                {
                                    //初始化公共参数
                                    部署参数_外部 = new 部署参数();
                                    部署参数_外部.fid = fid;
                                    部署参数_外部.tid = 主题参数.tid;
                                    部署参数_外部.pid = 主题参数.tid;
                                    部署参数_外部.楼层 = "1";
                                    部署参数_外部.时间戳 = 主题参数.发帖时间.ToString();

                                    if (违规用户.Rows[计数]["正则匹配"].ToString() == "T")
                                    {
                                        if (Regex.IsMatch(主题参数.作者.ToString(), 违规用户.Rows[计数]["用户名"].ToString()))
                                        {
                                            部署参数_外部.类型 = "删主题";
                                            部署参数_外部.贴吧名 = 贴吧名[0];
                                            部署参数_外部.关键词 = "违规用户：" + 主题参数.作者.ToString();
                                            部署参数_外部.关键词类型 = "违规用户";
                                            部署参数_外部.关键词索引 = 违规用户.Rows[计数]["索引"].ToString();
                                            任务部署(部署参数_外部);

                                            if (违规用户.Rows[计数]["是否封号"].ToString() == "T")
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 主题参数.作者.ToString();
                                                部署参数_外部.封号原因 = 违规用户.Rows[计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            等待(主题删除间隔);
                                            goto 至下一个帖子;
                                        }
                                    }
                                    else
                                    {
                                        if (主题参数.作者.ToString() == 违规用户.Rows[计数]["用户名"].ToString())
                                        {
                                            部署参数_外部.类型 = "删主题";
                                            部署参数_外部.贴吧名 = 贴吧名[0];
                                            部署参数_外部.关键词 = "违规用户：" + 主题参数.作者.ToString();
                                            部署参数_外部.关键词类型 = "违规用户";
                                            部署参数_外部.关键词索引 = 违规用户.Rows[计数]["索引"].ToString();
                                            任务部署(部署参数_外部);

                                            if (违规用户.Rows[计数]["是否封号"].ToString() == "T")
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 主题参数.作者.ToString();
                                                部署参数_外部.封号原因 = 违规用户.Rows[计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            等待(主题删除间隔);
                                            goto 至下一个帖子;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        控制台_输出(10, "帖号：" + 主题参数.tid + " 作者获取失败");
                        goto 至下一个帖子;
                    }

                    //是否视频帖
                    if (主题参数.是否视频帖)
                    {
                        /*
                        if (取用户某吧等级(主题参数.作者.ToString(), "微粉wefan") > 2)
                        {
                            任务部署(1, 贴吧名[0], fid, 主题参数.tid, "删帖策略：贴吧视频广告");
                            等待(主题删除间隔);
                            goto 至下一个帖子;
                        }
                        */
                    }

                    //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    //stopwatch.Start(); //  开始监视代码运行时间

                    //贴吧黑名单
                    if (启用贴吧黑名单)
                    {
                        DataTable 用户贴吧列表 = 取全部贴吧等级(主题参数.作者.ToString());

                        try
                        {
                            for (int i = 0; i < 用户贴吧列表.Rows.Count; i++)
                            {
                                for (int j = 0; j < 贴吧黑名单.Rows.Count; j++)
                                {
                                    if (贴吧黑名单.Rows[j]["匹配方式"].ToString() == "字段")
                                    {
                                        //关键词匹配
                                        if (用户贴吧列表.Rows[i]["贴吧名"].ToString().IndexOf(贴吧黑名单.Rows[j]["贴吧名"].ToString()) != -1)
                                        {
                                            if (Convert.ToInt32(用户贴吧列表.Rows[i]["等级"].ToString()) >= Convert.ToInt32(贴吧黑名单.Rows[j]["等级大于"].ToString()))
                                            {
                                                控制台_输出(0, 主题参数.作者.ToString() + " " + 用户贴吧列表.Rows[i]["贴吧名"].ToString() + " " + 用户贴吧列表.Rows[i]["等级"].ToString());
                                                //任务部署(0, 主题参数.作者.ToString(), fid, 主题参数.tid, "贴吧视频广告", 主题参数.tid);
                                                goto 跳出贴吧黑名单;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //全文匹配
                                        if (用户贴吧列表.Rows[i]["贴吧名"].ToString() == 贴吧黑名单.Rows[j]["贴吧名"].ToString())
                                        {
                                            if (Convert.ToInt32(用户贴吧列表.Rows[i]["等级"].ToString()) >= Convert.ToInt32(贴吧黑名单.Rows[j]["等级大于"].ToString()))
                                            {
                                                控制台_输出(0, 主题参数.tid.ToString() + " " + 主题参数.标题.ToString());
                                                控制台_输出(0, 主题参数.作者.ToString() + " " + 用户贴吧列表.Rows[i]["贴吧名"].ToString() + " " + 用户贴吧列表.Rows[i]["等级"].ToString());
                                                //任务部署(0, 主题参数.作者.ToString(), fid, 主题参数.tid, "贴吧视频广告", 主题参数.tid);
                                                goto 跳出贴吧黑名单;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            控制台_输出(10, e.ToString());
                        }

                        跳出贴吧黑名单:;//标记
                    }

                    //stopwatch.Stop(); //  停止监视
                    //TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                    //Console.WriteLine("{0}", timespan.TotalMilliseconds);

                    //for (int i = 0; i < 用户贴吧列表.Rows.Count; i++)
                    //{
                    //    控制台_输出(0, 用户贴吧列表.Rows[i][0].ToString() + " " + 用户贴吧列表.Rows[i][1].ToString());
                    //}


                    //########## 等级墙预留位 ##########
                    /*
                    if (取用户贴吧等级(主题参数.作者.ToString(), "沈阳理工大学") == 1)
                    {
                        任务部署(1, 贴吧名[0], fid, 主题参数.tid, "原因：等级墙2");
                        等待(主题删除间隔);
                        goto 至下一个帖子;
                    }*/

                    /*作者头像MD5 = snow.取贴吧头像MD5("http://tb.himg.baidu.com/sys/portrait/item/" + 主题参数.作者头像.ToString());

                    if (作者头像MD5 == 默认头像MD5)
                    {
                        if (取用户贴吧等级(主题参数.作者.ToString(), 贴吧名[0]) < 2)
                        {
                            任务部署(1, 贴吧名[0], fid, 主题参数.tid, "原因：等级墙1");
                            等待(主题删除间隔);
                            goto 至下一个帖子;
                        }
                    }*/

                    //########## 等级墙预留位 ##########

                    Regex 关键词正则;
                    MatchCollection 关键词结果;
                    for (int 关键词计数 = 0; 关键词计数 < 标题关键词.Rows.Count; 关键词计数++)
                    {
                        timeSplit = 标题关键词.Rows[关键词计数]["执行时间"].ToString().Split(',');
                        timeTampInt[1] = Convert.ToInt32(timeSplit[0]);
                        timeTampInt[2] = Convert.ToInt32(timeSplit[1]);
                        if (timeTampInt[0] < timeTampInt[1] || timeTampInt[0] > timeTampInt[2])
                        {
                            continue;
                        }

                        //初始化公共参数
                        部署参数_外部 = new 部署参数();
                        部署参数_外部.fid = fid;
                        部署参数_外部.tid = 主题参数.tid;
                        部署参数_外部.pid = 主题参数.tid;
                        部署参数_外部.楼层 = "1";
                        部署参数_外部.时间戳 = 主题参数.发帖时间.ToString();

                        if (标题关键词.Rows[关键词计数]["正则匹配"].ToString() == "T")
                        {
                            关键词正则 = new Regex(标题关键词.Rows[关键词计数]["关键词"].ToString());
                            关键词结果 = 关键词正则.Matches(主题参数.标题.ToString());
                            if (关键词结果.Count > 0)
                            {
                                if (标题关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                {
                                    控制台_输出(0, "<" + 主题参数.tid + "> 调试 正则匹配：" + 关键词结果[0].Value
                                        + "\n原标题：" + 主题参数.标题.ToString());
                                }
                                else
                                {
                                    if (标题关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                    {
                                        部署参数_外部.类型 = "删主题";
                                        部署参数_外部.贴吧名 = 贴吧名[0];
                                        部署参数_外部.关键词 = "原因：" + 关键词结果[0].Value;
                                        部署参数_外部.关键词类型 = "标题关键词";
                                        部署参数_外部.关键词索引 = 标题关键词.Rows[关键词计数]["索引"].ToString();
                                        任务部署(部署参数_外部);
                                    }
                                    if (标题关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                    {
                                        部署参数_外部.类型 = "封号";
                                        部署参数_外部.用户名 = 主题参数.作者.ToString();
                                        部署参数_外部.封号原因 = 标题关键词.Rows[关键词计数]["封号理由"].ToString();
                                        任务部署(部署参数_外部);
                                    }

                                    等待(主题删除间隔);
                                }

                                goto 至下一个帖子;
                            }
                        }
                        else
                        {
                            if (主题参数.标题.IndexOf(标题关键词.Rows[关键词计数]["关键词"].ToString()) != -1)
                            {
                                if (标题关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                {
                                    控制台_输出(0, "<" + 主题参数.tid + "> 调试 关键词：" + 标题关键词.Rows[关键词计数]["关键词"].ToString()
                                        + "\n原标题：" + 主题参数.标题.ToString());
                                }
                                else
                                {
                                    if (标题关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                    {
                                        部署参数_外部.类型 = "删主题";
                                        部署参数_外部.贴吧名 = 贴吧名[0];
                                        部署参数_外部.关键词 = "原因：" + 标题关键词.Rows[关键词计数]["关键词"].ToString();
                                        部署参数_外部.关键词类型 = "标题关键词";
                                        部署参数_外部.关键词索引 = 标题关键词.Rows[关键词计数]["索引"].ToString();
                                        任务部署(部署参数_外部);
                                    }
                                    if (标题关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                    {
                                        部署参数_外部.类型 = "封号";
                                        部署参数_外部.用户名 = 主题参数.作者.ToString();
                                        部署参数_外部.封号原因 = 标题关键词.Rows[关键词计数]["封号理由"].ToString();
                                        任务部署(部署参数_外部);
                                    }

                                    等待(主题删除间隔);
                                }

                                goto 至下一个帖子;
                            }
                        }
                    }

                    至帖子内容:;

                    if (!监控回复)
                    {
                        goto 至下一个帖子;
                    }

                    //获取帖子内容
                    string 内容链接 = "http://c.tieba.baidu.com/c/f/pb/page";
                    string 内容参数文本
                        = 访问Cookie
                        + "&_client_id="
                        + snow.安卓取Stamp()
                        + "&_client_type=2&_client_version=7.8.1&kz="
                        + 主题参数.tid;

                    内容参数文本 = 主题参数.回复数 > 60 ? 内容参数文本 += "&r=1" : 内容参数文本;//大于60回复倒序
                    内容参数文本 += "&sign=" + snow.取Sign(内容参数文本);

                    string 内容返回文本 = http.Post(内容链接, 内容参数文本, 访问Cookie);

                    //Console.WriteLine(snow.usc2转ansi(内容返回文本));
                    //写到文件(@"C:\Users\Administrator\Desktop\123.txt", 内容返回文本);

                    if (内容返回文本 == "")
                    {
                        控制台_输出(10, "帖号：" + 主题参数.tid + " 内容获取失败，已跳过...");
                        goto 至下一个帖子;
                    }

                    JObject 内容源码 = new JObject();
                    try
                    {
                        内容源码 = JObject.Parse(内容返回文本);
                    }
                    catch
                    {
                        控制台_输出(10, "帖号：" + 主题参数.tid + " 内容解析失败。");
                        //写到文件(Application.StartupPath + "\\操作日志\\内容报错" + 全局.报错计数.ToString() + ".txt", snow.usc2转ansi(内容返回文本));
                        goto 至下一个帖子;
                    }

                    if (内容源码["error_code"].ToString() != "0")
                    {
                        控制台_输出(10, "帖号：" + 主题参数.tid + " 内容获取失败：" + 内容源码["error_msg"].ToString());
                        goto 至下一个帖子;
                    }

                    JArray 内容jar = (JArray)内容源码["post_list"];
                    //控制台_输出(12, "获取到" + 内容jar.Count.ToString() + "个内容");
                    内容参数结构 内容参数 = new 内容参数结构();
                    for (int 内容计数 = 0; 内容计数 < 内容jar.Count; 内容计数++)
                    {
                        try
                        {
                            //帖子参数
                            内容参数.pid = (string)内容源码["post_list"][内容计数]["id"];
                            内容参数.楼层 = (int)内容源码["post_list"][内容计数]["floor"];
                            内容参数.发言时间 = (int)内容源码["post_list"][内容计数]["time"];

                            //帖内缓存待定


                        }
                        catch
                        {
                            控制台_输出(10, "帖号：" + 主题参数.tid + " 楼层：" + 内容参数.楼层.ToString() + " 内容参数获取失败");
                        }

                        #region "内容UID处理"
                        JObject 内容UID源码 = JObject.Parse(内容返回文本);
                        JArray 内容UIDjar = (JArray)内容UID源码["user_list"];

                        string[,] 内容UID列表 = new string[内容UIDjar.Count, 5];
                        for (int 内容UID计数 = 0; 内容UID计数 < 内容UIDjar.Count; 内容UID计数++)
                        {
                            try
                            {
                                内容UID列表[内容UID计数, 0] = (string)内容UID源码["user_list"][内容UID计数]["id"];
                                内容UID列表[内容UID计数, 1] = (string)内容UID源码["user_list"][内容UID计数]["name_show"];
                                内容UID列表[内容UID计数, 2] = (string)内容UID源码["user_list"][内容UID计数]["portrait"];
                                内容UID列表[内容UID计数, 3] = (string)内容UID源码["user_list"][内容UID计数]["level_id"];
                                内容UID列表[内容UID计数, 4] = (string)内容UID源码["user_list"][内容UID计数]["is_bawu"];

                                //内容参数.是否吧务 = (string)内容源码["post_list"][内容计数]["author"]["is_bawu"] == "1" ? true : false;
                            }
                            catch
                            {
                                内容UID列表[内容UID计数, 0]
                                    = 内容UID列表[内容UID计数, 1]
                                    = 内容UID列表[内容UID计数, 2]
                                    = 内容UID列表[内容UID计数, 3]
                                    = 内容UID列表[内容UID计数, 4]
                                    = "#获取失败#";

                                控制台_输出(10, "帖号：" + 主题参数.tid + " 内容UID列表 " + 内容UID计数.ToString() + " 获取失败。");
                            }
                        }
                        #endregion

                        //层主信息
                        内容参数.用户名 = 内容参数.头像 = "#获取失败#";
                        内容参数.等级 = 1;
                        内容参数.是否吧务 = false;
                        for (int i = 0; i < 内容UID列表.Length / 5; i++)
                        {
                            //匹配UID
                            if (内容UID列表[i, 0] == (string)内容源码["post_list"][内容计数]["author_id"])
                            {
                                内容参数.用户名 = 内容UID列表[i, 1];
                                内容参数.头像 = 内容UID列表[i, 2];
                                内容参数.是否吧务 = 内容UID列表[i, 4] == "1" ? true : false;

                                try
                                {
                                    内容参数.等级 = Convert.ToInt32(内容UID列表[i, 3]);
                                }
                                catch
                                {
                                    内容参数.等级 = 0;
                                }

                                break;
                            }
                        }
                        //Console.WriteLine("{0} {1} {2} {3}", 内容参数.用户名, 内容参数.头像, 内容参数.等级, 内容参数.是否吧务);

                        if (内容参数.用户名.ToString() != "")
                        {
                            //是否白名单用户
                            for (int 计数 = 0; 计数 < 信任用户.Rows.Count; 计数++)
                            {
                                //如果是白名单用户 或者 是吧务
                                if (内容参数.用户名.ToString() == 信任用户.Rows[计数]["用户名"].ToString() || 内容参数.是否吧务)
                                {
                                    goto 至下一个回复;
                                }
                            }

                            //是否违规用户
                            if (启用违规用户)
                            {
                                for (int 计数 = 0; 计数 < 违规用户.Rows.Count; 计数++)
                                {
                                    if (违规用户.Rows[计数]["等级大于"].ToString() != "-1")
                                    {
                                        if (内容参数.等级 > Convert.ToInt32(违规用户.Rows[计数]["等级大于"].ToString()))
                                        {
                                            //初始化公共参数
                                            部署参数_外部 = new 部署参数();
                                            部署参数_外部.fid = fid;
                                            部署参数_外部.tid = 主题参数.tid;
                                            部署参数_外部.pid = 内容参数.pid.ToString();
                                            部署参数_外部.楼层 = 内容参数.楼层.ToString();
                                            部署参数_外部.时间戳 = 内容参数.发言时间.ToString();

                                            if (违规用户.Rows[计数]["正则匹配"].ToString() == "T")
                                            {
                                                if (Regex.IsMatch(内容参数.用户名.ToString(), 违规用户.Rows[计数]["用户名"].ToString()))
                                                {
                                                    部署参数_外部.类型 = "删回复";
                                                    部署参数_外部.贴吧名 = 贴吧名[0];
                                                    部署参数_外部.关键词 = "违规用户：" + 内容参数.用户名.ToString();
                                                    部署参数_外部.关键词类型 = "违规用户";
                                                    部署参数_外部.关键词索引 = 违规用户.Rows[计数]["索引"].ToString();
                                                    任务部署(部署参数_外部);

                                                    if (违规用户.Rows[计数]["是否封号"].ToString() == "T")
                                                    {
                                                        部署参数_外部.类型 = "封号";
                                                        部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                        部署参数_外部.封号原因 = 违规用户.Rows[计数]["封号理由"].ToString();
                                                        任务部署(部署参数_外部);
                                                    }

                                                    等待(回复删除间隔);
                                                    goto 至下一个回复;
                                                }
                                            }
                                            else
                                            {
                                                if (内容参数.用户名.ToString() == 违规用户.Rows[计数]["用户名"].ToString())
                                                {
                                                    部署参数_外部.类型 = "删回复";
                                                    部署参数_外部.贴吧名 = 贴吧名[0];
                                                    部署参数_外部.关键词 = "违规用户：" + 内容参数.用户名.ToString();
                                                    部署参数_外部.关键词类型 = "违规用户";
                                                    部署参数_外部.关键词索引 = 违规用户.Rows[计数]["索引"].ToString();
                                                    任务部署(部署参数_外部);

                                                    if (违规用户.Rows[计数]["是否封号"].ToString() == "T")
                                                    {
                                                        部署参数_外部.类型 = "封号";
                                                        部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                        部署参数_外部.封号原因 = 违规用户.Rows[计数]["封号理由"].ToString();
                                                        任务部署(部署参数_外部);
                                                    }

                                                    等待(回复删除间隔);
                                                    goto 至下一个回复;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            控制台_输出(10, "帖号：" + 主题参数.tid + " 楼层：" + 内容参数.楼层.ToString() + " 用户名获取失败");
                        }

                        //内容处理
                        内容参数.内容文本 = "";//初始化
                        for (int 文本计数 = 0; 文本计数 < 内容源码["post_list"][内容计数]["content"].Count(); 文本计数++)
                        {
                            string 内容类型 = (string)内容源码["post_list"][内容计数]["content"][文本计数]["type"];

                            if (内容类型 == "0")//文本
                            {
                                内容参数.内容文本 += 文本过滤((string)内容源码["post_list"][内容计数]["content"][文本计数]["text"]);
                            }
                            else if (内容类型 == "1")//链接
                            {
                                //{"type":1,"link":"网址","text":"网址"}
                                内容参数.内容文本 += "#链接=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["text"] + "#";
                                //控制台_输出(0, "#链接# " + (string)内容源码["post_list"][内容计数]["content"][文本计数]["link"]);
                            }
                            else if (内容类型 == "2")//表情
                            {
                                内容参数.内容文本 = "#表情=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["c"] + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "3")//图片
                            {
                                //{"type":3,"src":"链接","bsize":"189,199","size":"49634"}
                                //控制台_输出(0, (string)内容源码["post_list"][内容计数]["content"].ToString());
                                //if (图片特征)
                                //{
                                //    内容参数.内容文本 = "#图片=" + snow.取网络资源MD5((string)内容源码["post_list"][内容计数]["content"][文本计数]["origin_src"]) + "=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["bsize"] + "#" + 内容参数.内容文本;
                                //}
                                //else
                                //{
                                //    内容参数.内容文本 = "#图片=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["bsize"] + "+" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["size"] + "#" + 内容参数.内容文本;
                                //}

                                内容参数.内容文本 = "#图片=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["origin_src"] + "=size=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["bsize"] + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "4")//艾特
                            {
                                内容参数.内容文本 = "#艾特=" + 内容源码["post_list"][内容计数]["content"][文本计数]["text"].ToString().Replace("@", "") + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "5")//视频
                            {
                                //{"type":5,"e_type":15,"width":"480","height":"480","bsize":"480,480","during_time":"2","origin_size":"168046","text":"http:\/\/tieba.baidu.com\/mo\/q\/movideo\/page?thumbnail=d109b3de9c82d158d3fcee1d880a19d8bc3e421b&video=10363_ed294eae88371575b3dbcf9f1990f68d","link":"http:\/\/tb-video.bdstatic.com\/tieba-smallvideo\/10363_ed294eae88371575b3dbcf9f1990f68d.mp4","src":"http:\/\/imgsrc.baidu.com\/forum\/pic\/item\/d109b3de9c82d158d3fcee1d880a19d8bc3e421b.jpg","is_native_app":0,"native_app":[]}
                                内容参数.内容文本 += "#视频#";
                            }
                            else if (内容类型 == "7")//换行
                            {
                                //{"type":"7","text":"\n"}
                            }
                            else if (内容类型 == "9")//电话号码
                            {
                                //{"type":"9","text":"6666666","phonetype":"2"}
                                内容参数.内容文本 += 文本过滤((string)内容源码["post_list"][内容计数]["content"][文本计数]["text"]);
                            }
                            else if (内容类型 == "10")//语音
                            {
                                //{"type":"10","during_time":"15000","voice_md5":"e25ef2db5076f825e229c6cdb1613f38_1064475243"}
                                内容参数.内容文本 = "#语音=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["voice_md5"] + "," + (string)内容源码["post_list"][内容计数]["content"][文本计数]["during_time"] + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "11")//动态表情
                            {
                                //{"type":"11","c":"白发魔女传之明月天国_女屌丝","static":"png静态图链接","dynamic":"gif动态图链接","height":"160","width":"160","icon":"http://tb2.bdstatic.com/tb/editor/images/faceshop/1058_baifa/panel.png","packet_name":"白发魔女传之明月天国"}
                                内容参数.内容文本 = "#表情=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["c"] + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "16")//涂鸦
                            {
                                //{"type":"16","bsize":"560,560","graffiti_info":{"url":"jpg网页端原图","gid":"123456"},"cdn_src":"客户端缩略图","big_cdn_src":"客户端大图"}
                                if (图片特征)
                                {
                                    内容参数.内容文本 = "#图片=" + snow.取网络资源MD5((string)内容源码["post_list"][内容计数]["content"][文本计数]["graffiti_info"]["url"]) + "=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["bsize"] + "#" + 内容参数.内容文本;
                                }
                                else
                                {
                                    内容参数.内容文本 = "#图片=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["graffiti_info"]["url"] + "=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["bsize"] + "#" + 内容参数.内容文本;
                                }
                            }
                            else if (内容类型 == "17")//活动帖
                            {
                                //{"type":"17","high_together":{"album_id":"478448408116821906","album_name":"关于众筹西游记歌曲演唱会活动","start_time":"0","end_time":"0","location":"","num_join":"0","pic_urls":[]}}
                                内容参数.内容文本 = "#活动=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["album_name"] + "#" + 内容参数.内容文本;
                            }
                            else if (内容类型 == "18")//贴吧热议
                            {
                                //{"type":"18","text":"#白狐狸不改国庆礼包就滚出dnf#","link":"http://tieba.baidu.com/mo/q/hotMessage?topic_id=0&topic_name=白狐狸不改国庆礼包就滚出dnf"}
                                内容参数.内容文本 = "#热议=" + (string)内容源码["post_list"][内容计数]["content"][文本计数]["text"] + "#" + 内容参数.内容文本;
                            }
                            else //未知
                            {
                                控制台_输出(10, "内容参数：未知类型" + 内容类型 + " 帖号：" + 主题参数.tid + " 楼层：" + 内容参数.楼层);
                                snow.写到文件(Application.StartupPath + "\\" + 主题参数.tid + ".txt", 内容返回文本);
                            }
                        }

                        //开始判断
                        if (内容参数.楼层 == 1)
                        {
                            //1楼
                            //########## 关联规则策略 ##########

                            /*if (主题参数.tid == "4918527223")
                            {
                                控制台_输出(0, 内容参数.内容文本.ToString());
                            }*/

                            //########## 关联规则策略 ##########

                            //1楼
                            for (int 关键词计数 = 0; 关键词计数 < 一楼关键词.Rows.Count; 关键词计数++)
                            {
                                if (!(内容参数.等级 < Convert.ToInt32(一楼关键词.Rows[关键词计数]["等级小于"].ToString())))
                                {
                                    //下一个关键词
                                    continue;
                                }

                                timeSplit = 一楼关键词.Rows[关键词计数]["执行时间"].ToString().Split(',');
                                timeTampInt[1] = Convert.ToInt32(timeSplit[0]);
                                timeTampInt[2] = Convert.ToInt32(timeSplit[1]);
                                if (timeTampInt[0] < timeTampInt[1] || timeTampInt[0] > timeTampInt[2])
                                {
                                    continue;
                                }

                                //初始化公共参数
                                部署参数_外部 = new 部署参数();
                                部署参数_外部.fid = fid;
                                部署参数_外部.tid = 主题参数.tid;
                                部署参数_外部.pid = 内容参数.pid.ToString();
                                部署参数_外部.楼层 = 内容参数.楼层.ToString();
                                部署参数_外部.时间戳 = 内容参数.发言时间.ToString();

                                if (一楼关键词.Rows[关键词计数]["正则匹配"].ToString() == "T")
                                {
                                    关键词正则 = new Regex(一楼关键词.Rows[关键词计数]["关键词"].ToString());
                                    关键词结果 = 关键词正则.Matches(内容参数.内容文本.ToString());
                                    if (关键词结果.Count > 0)
                                    {
                                        if (一楼关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                        {
                                            控制台_输出(0, "<" + 主题参数.tid + " " + 内容参数.楼层.ToString() + "L " + 内容参数.pid.Substring(内容参数.pid.Length - 4, 4) + "> 调试 正则匹配：" + 关键词结果[0].Value
                                                + "\n原内容：" + 内容参数.内容文本.ToString());
                                        }
                                        else
                                        {
                                            if (一楼关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                            {
                                                部署参数_外部.类型 = "删主题";
                                                部署参数_外部.贴吧名 = 贴吧名[0];
                                                部署参数_外部.关键词 = "原因：" + 关键词结果[0].Value;
                                                部署参数_外部.关键词类型 = "一楼关键词";
                                                部署参数_外部.关键词索引 = 一楼关键词.Rows[关键词计数]["索引"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            if (一楼关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                部署参数_外部.封号原因 = 一楼关键词.Rows[关键词计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }

                                            等待(回复删除间隔);
                                        }

                                        goto 至下一个回复;
                                    }
                                }
                                else
                                {
                                    if (内容参数.内容文本.ToString().IndexOf(一楼关键词.Rows[关键词计数]["关键词"].ToString()) != -1)
                                    {
                                        if (一楼关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                        {
                                            控制台_输出(0, "<" + 主题参数.tid + " " + 内容参数.楼层.ToString() + "L " + 内容参数.pid.Substring(内容参数.pid.Length - 4, 4) + "> 调试 关键词：" + 一楼关键词.Rows[关键词计数]["关键词"].ToString()
                                                + "\n原内容：" + 内容参数.内容文本.ToString());
                                        }
                                        else
                                        {
                                            if (一楼关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                            {
                                                部署参数_外部.类型 = "删主题";
                                                部署参数_外部.贴吧名 = 贴吧名[0];
                                                部署参数_外部.关键词 = "原因：" + 一楼关键词.Rows[关键词计数]["关键词"].ToString();
                                                部署参数_外部.关键词类型 = "一楼关键词";
                                                部署参数_外部.关键词索引 = 一楼关键词.Rows[关键词计数]["索引"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            if (一楼关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                部署参数_外部.封号原因 = 一楼关键词.Rows[关键词计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }

                                            等待(回复删除间隔);
                                        }

                                        goto 至下一个回复;
                                    }
                                }

                                //楼中楼代码

                            }
                        }
                        else
                        {
                            //2楼及以下
                            //########## 关联规则策略 ##########

                            /*if (主题参数.tid == "4918558934")
                            {
                                控制台_输出(0, 内容参数.内容文本.ToString());
                            }*/

                            if (主题参数.tid == "5090998898")
                            {
                                goto 至下一个帖子;
                            }
                            if (主题参数.tid == "5434154039")
                                控制台_输出(0, "标题 " + 主题参数.标题);

                            if (主题参数.tid == "5434154039")
                            {

                                控制台_输出(0, 内容参数.内容文本);

                            }

                            //DNF飞机群 判断内容是否有标题文本，判断是否有图片
                            if (内容参数.内容文本.ToString().IndexOf(主题参数.标题) != -1)
                            {
                                if (内容参数.内容文本.ToString().IndexOf("#图片=") != -1)
                                {
                                    部署参数_外部 = new 部署参数();
                                    部署参数_外部.fid = fid;
                                    部署参数_外部.tid = 主题参数.tid;
                                    部署参数_外部.pid = 内容参数.pid.ToString();
                                    部署参数_外部.楼层 = 内容参数.楼层.ToString();
                                    部署参数_外部.时间戳 = 内容参数.发言时间.ToString();

                                    部署参数_外部.类型 = "删回复";
                                    部署参数_外部.贴吧名 = 贴吧名[0];
                                    部署参数_外部.关键词 = "原因：DNF飞机群 策略1";
                                    部署参数_外部.关键词类型 = "内容关键词";
                                    //部署参数_外部.关键词索引 = "0";
                                    任务部署(部署参数_外部);
                                    goto 至下一个回复;

                                }
                            }

                            //########## 关联规则策略 ##########



                            for (int 关键词计数 = 0; 关键词计数 < 内容关键词.Rows.Count; 关键词计数++)
                            {
                                if (!(内容参数.等级 < Convert.ToInt32(内容关键词.Rows[关键词计数]["等级小于"].ToString())))
                                {
                                    //下一个关键词
                                    continue;
                                }

                                timeSplit = 内容关键词.Rows[关键词计数]["执行时间"].ToString().Split(',');
                                timeTampInt[1] = Convert.ToInt32(timeSplit[0]);
                                timeTampInt[2] = Convert.ToInt32(timeSplit[1]);
                                if (timeTampInt[0] < timeTampInt[1] || timeTampInt[0] > timeTampInt[2])
                                {
                                    //下一个关键词
                                    continue;
                                }

                                //初始化公共参数
                                部署参数_外部 = new 部署参数();
                                部署参数_外部.fid = fid;
                                部署参数_外部.tid = 主题参数.tid;
                                部署参数_外部.pid = 内容参数.pid.ToString();
                                部署参数_外部.楼层 = 内容参数.楼层.ToString();
                                部署参数_外部.时间戳 = 内容参数.发言时间.ToString();

                                if (内容关键词.Rows[关键词计数]["正则匹配"].ToString() == "T")
                                {
                                    关键词正则 = new Regex(内容关键词.Rows[关键词计数]["关键词"].ToString());
                                    关键词结果 = 关键词正则.Matches(内容参数.内容文本.ToString());
                                    if (关键词结果.Count > 0)
                                    {
                                        if (内容关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                        {
                                            控制台_输出(0, "<" + 主题参数.tid + " " + 内容参数.楼层.ToString() + "L " + 内容参数.pid.Substring(内容参数.pid.Length - 4, 4) + "> 调试 正则匹配：" + 关键词结果[0].Value
                                                + "\n原内容：" + 内容参数.内容文本.ToString());
                                        }
                                        else
                                        {
                                            if (内容关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                            {
                                                部署参数_外部.类型 = "删回复";
                                                部署参数_外部.贴吧名 = 贴吧名[0];
                                                部署参数_外部.关键词 = "原因：" + 关键词结果[0].Value;
                                                部署参数_外部.关键词类型 = "内容关键词";
                                                部署参数_外部.关键词索引 = 内容关键词.Rows[关键词计数]["索引"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            if (内容关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                部署参数_外部.封号原因 = 内容关键词.Rows[关键词计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }

                                            等待(回复删除间隔);
                                        }

                                        goto 至下一个回复;
                                    }
                                }
                                else
                                {
                                    if (内容参数.内容文本.ToString().IndexOf(内容关键词.Rows[关键词计数]["关键词"].ToString()) != -1)
                                    {
                                        if (内容关键词.Rows[关键词计数]["调试输出"].ToString() == "T")
                                        {
                                            控制台_输出(0, "<" + 主题参数.tid + " " + 内容参数.楼层.ToString() + "L " + 内容参数.pid.Substring(内容参数.pid.Length - 4, 4) + "> 调试 关键词：" + 内容关键词.Rows[关键词计数]["关键词"].ToString()
                                                + "\n原内容：" + 内容参数.内容文本.ToString());
                                        }
                                        else
                                        {
                                            if (内容关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("删") != -1)
                                            {
                                                部署参数_外部.类型 = "删回复";
                                                部署参数_外部.贴吧名 = 贴吧名[0];
                                                部署参数_外部.关键词 = "原因：" + 内容关键词.Rows[关键词计数]["关键词"].ToString();
                                                部署参数_外部.关键词类型 = "内容关键词";
                                                部署参数_外部.关键词索引 = 内容关键词.Rows[关键词计数]["索引"].ToString();
                                                任务部署(部署参数_外部);
                                            }
                                            if (内容关键词.Rows[关键词计数]["处理方式"].ToString().IndexOf("封") != -1)
                                            {
                                                部署参数_外部.类型 = "封号";
                                                部署参数_外部.用户名 = 内容参数.用户名.ToString();
                                                部署参数_外部.封号原因 = 内容关键词.Rows[关键词计数]["封号理由"].ToString();
                                                任务部署(部署参数_外部);
                                            }

                                            等待(回复删除间隔);
                                        }

                                        goto 至下一个回复;
                                    }
                                }
                            }
                        }

                        至下一个回复:;//回复标记
                    }

                    等待(帖子访问间隔);

                    至下一个帖子:;//帖子标记
                }

                等待(主题获取间隔);
            }

        }

        public static void 任务部署(部署参数 部署参数_数据)
        {
            全局.等待部署 = true;

            //部署参数_数据.cookie = 全局.帐号库.Rows[全局.帐号计数]["Cookie"].ToString();

            //if (全局.帐号计数 >= 全局.帐号库.Rows.Count - 1)
            //{
            //    全局.帐号计数 = 0;
            //}
            //else
            //{
            //    全局.帐号计数 += 1;
            //}

            string 数据库路径 = Application.StartupPath + "\\配置文件.mdb";
            部署参数_数据.cookie = Access.getStr(数据库路径, "select Cookie from Cookies WHERE 帐号用途 = '访问'");

            全局.部署参数 = 部署参数_数据;

            if (全局.部署参数.类型 == "封号")
            {
                //封号
                if (全局.部署参数.tid.ToString() == 全局.部署参数.pid.ToString())
                {
                    全局.部署参数.pid += "10" + 全局.部署参数.pid;
                }

                Thread 封号线程 = new Thread(new ThreadStart(封号));
                封号线程.Start();
            }
            else if (全局.部署参数.类型 == "删主题" || 全局.部署参数.类型 == "删回复")
            {
                //删帖
                Thread 删帖线程 = new Thread(new ThreadStart(删帖));
                删帖线程.Start();
            }
            else
            {
                全局.等待部署 = false;
                return;
            }

            do
            {
                等待(10);
            } while (全局.等待部署);
        }

        public static void 删帖()
        {
            //http://c.tieba.baidu.com/c/c/bawu/delthread 报错有问题
            //http://tieba.baidu.com/mo/q/m wapp参数太多，首页获取不到pid

            string 数据库路径 = Application.StartupPath + "\\配置文件.mdb";
            string 类型, Cookie, fid, tid, pid, 楼层, 贴吧名, 关键词, 关键词类型, 关键词索引, 时间戳;

            Cookie = 全局.部署参数.cookie;
            类型 = 全局.部署参数.类型;
            fid = 全局.部署参数.fid;
            tid = 全局.部署参数.tid;
            pid = 全局.部署参数.pid;
            楼层 = 全局.部署参数.楼层;
            贴吧名 = 全局.部署参数.贴吧名;
            关键词 = 全局.部署参数.关键词;
            关键词类型 = 全局.部署参数.关键词类型;
            关键词索引 = 全局.部署参数.关键词索引;
            时间戳 = 全局.部署参数.时间戳;

            全局.等待部署 = false;

            string url, postString, 输出文本;
            int 输出颜色;

            int 当前时间戳 = Convert.ToInt32(snow.取时间戳());
            int 发帖时间戳 = Convert.ToInt32(时间戳);
            int 间隔时间 = 当前时间戳 - 发帖时间戳;
            string 输出时间文本 = "时差：";

            if (间隔时间 < 60)
            {
                输出时间文本 += 间隔时间.ToString() + "秒";
            }
            else
            {
                间隔时间 = 间隔时间 / 60;
                if (间隔时间 < 60)
                {
                    输出时间文本 += 间隔时间.ToString() + "分";
                }
                else
                {
                    间隔时间 = 间隔时间 / 60;
                    if (间隔时间 < 24)
                    {
                        输出时间文本 += 间隔时间.ToString() + "时";
                    }
                    else
                    {
                        间隔时间 = 间隔时间 / 24;
                        输出时间文本 += 间隔时间.ToString() + "天";
                    }
                }
            }

            if (类型 == "删主题")
            {
                //删主题
                url = "http://tieba.baidu.com/f/commit/thread/delete";
                postString = "commit_fr=pb&fid=" + fid + "&ie=utf-8&kw=" + snow.URL编码UTF8(贴吧名) + "&tbs=" + snow.百度tbs(Cookie) + "&tid=" + tid;
                输出文本 = tid;
            }
            else if (类型 == "删回复")
            {
                //删回复
                url = "http://tieba.baidu.com/f/commit/post/delete";
                postString = "commit_fr=pb&fid=" + fid + "&ie=utf-8&is_finf=false&is_vipdel=0&kw=" + snow.URL编码UTF8(贴吧名) + "&pid=" + pid + "&tbs=" + snow.百度tbs(Cookie) + "&tid=" + tid;
                输出文本 = tid + " " + 楼层 + "L " + pid.Substring(pid.Length - 4, 4);
            }
            else
            {
                控制台_输出(11, "未知删帖类型：" + 类型);
                return;
            }

            string srcString = http.Post(url, postString, Cookie);

            JObject 删帖源码 = new JObject();
            try
            {
                删帖源码 = JObject.Parse(srcString);

                //{"no":0,"err_code":0,"error":null,"data":{"mute_text":null}}
                if ((string)删帖源码["no"] == "0")
                {
                    操作量统计(1);

                    输出颜色 = 11;
                    输出文本 = 全局.删帖量.ToString() + ".<" + 输出文本 + "> " + 输出时间文本 + " " + 关键词;

                    if (关键词索引 != "0")
                    {
                        Access.set(数据库路径, "UPDATE " + 关键词类型 + " SET 最后触发时间 = '" + snow.时间处理(0, false) + "' WHERE 索引 = " + 关键词索引 + "");
                    }
                }
                else
                {
                    操作量统计(0);

                    输出颜色 = 10;
                    输出文本 = 全局.报错计数.ToString() + ".<" + 输出文本 + "> 失败 " + 关键词;
                }
            }
            catch
            {
                操作量统计(0);

                输出颜色 = 10;
                输出文本 = 全局.报错计数.ToString() + ".<" + 输出文本 + "> 网络错误 " + 关键词;
            }

            控制台_输出(输出颜色, 输出文本);
        }

        public static void 封号()
        {
            //http://tieba.baidu.com/pmc/blockid

            //day=1&fid=81570&tbs=dd4bdb7923713a5e1477900773&ie=gbk&user_name%5B%5D=%E5%8D%8A%E6%83%8C%E6%B8%85%E6%84%81&pid%5B%5D=99755853457&reason=

            //day:1
            //fid: 81570
            //tbs: dd4bdb7923713a5e1477900773
            //ie:gbk
            //user_name[]:半惌清愁
            //pid[]:99755853457
            //reason: 挖坟,闪光弹，无意义复制粘贴刷经验(─.─|||

            string Cookie, 用户名, fid, tid, pid, 封号原因;

            Cookie = 全局.部署参数.cookie;
            用户名 = 全局.部署参数.贴吧名;
            fid = 全局.部署参数.fid;
            tid = 全局.部署参数.tid;
            pid = 全局.部署参数.pid;
            封号原因 = 全局.部署参数.关键词;

            全局.等待部署 = false;

            string url, postString, 输出文本;
            int 输出颜色;

            url = "http://tieba.baidu.com/pmc/blockid";
            postString = "day=1&fid=" + fid + "&tbs=" + snow.百度tbs(Cookie) + "&ie=gbk&user_name%5B%5D=" + snow.URL编码UTF8(用户名) + "&pid%5B%5D=" + pid + "&reason=" + snow.URL编码UTF8(封号原因);
            输出文本 = tid;

            string srcString = http.Post(url, postString, Cookie);

            JObject 封号源码 = new JObject();
            try
            {
                封号源码 = JObject.Parse(srcString);
            }
            catch
            {
                操作量统计(0);

                输出颜色 = 10;
                输出文本 = 全局.报错计数.ToString() + ".<" + tid + "> ID：" + 用户名 + " 封号失败：网络错误";
            }

            if ((string)封号源码["errno"] == "0")
            {
                操作量统计(2);

                输出颜色 = 13;
                输出文本 = 全局.封号量.ToString() + ".<" + tid + "> ID：" + 用户名 + " 封号成功：" + 封号原因;
            }
            else
            {
                操作量统计(0);

                输出颜色 = 10;
                输出文本 = 全局.报错计数.ToString() + ".<" + tid + "> ID：" + 用户名 + " 封号失败：" + snow.usc2转ansi((string)封号源码["errmsg"]);
            }

            控制台_输出(输出颜色, 输出文本);
        }

    }

    public class 全局
    {
        public static string 版本号 = "0.8.9.16 170411 Debug";
        public static DataTable 帐号库;
        public static string[] 字符过滤;
        public static string[,] 文本替换;

        public static bool 等待部署;
        public static int 帐号计数 = 0;

        public static int 删帖量;
        public static int 封号量;
        public static int 报错计数;

        public static 部署参数 部署参数;
    }

    public class 部署参数
    {
        public string 类型;
        public string cookie;
        public string fid;
        public string tid;
        public string pid;
        public string 楼层;
        public string 贴吧名;
        public string 关键词;
        public string 关键词类型;
        public string 关键词索引;
        public string 用户名;
        public string 封号原因;
        public string 时间戳;
    }

    class 主题参数结构
    {
        //主题参数
        public string tid; //tid
        public string 标题; //title
        public int 回复数; //reply_num
        public int 最后回复时间; //last_time_int
        public int 发帖时间; //create_time
        public bool 是否置顶; //is_top
        public bool 是否精品; //is_good
        public bool 是否会员置顶; //is_membertop
        public bool 是否视频帖; //thread_type 40

        //作者信息 author
        public string 作者; //name
        public string 作者头像; //portrait
                            //public bool 作者i; //is_verify

        //最后回复 last_replyer
        //public string 最后回复人; //name
        //public bool 最后回复人i; //is_verify
    }

    class 内容参数结构
    {
        //帖子参数
        public string pid; //id
        public int 楼层; //floor
        public int 发言时间; //发言时间

        //回复内容 content type
        //0 文本内容(text)/n
        //1 链接
        //2 表情(c)
        //3 图片(src) 图片大小(bsize)长,宽
        //4 艾特
        //5 视频
        //9 长数字 可作为文本处理
        //10 语音
        //16 涂鸦
        public string 内容文本;

        //层主信息 author
        public string 用户名;//name
        public string 头像;//portrait
                         //public bool 是否关注;//is_like
        public int 等级;//level_id
        public bool 是否吧务;//is_bawu
    }
}