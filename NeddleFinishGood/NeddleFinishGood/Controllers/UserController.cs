using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NeddleFinishGood.Controllers
{
    public class UserController : Controller
    {
        // GET: Good
        Models.SqlHelper sql = new Models.SqlHelper();
        public ActionResult Index()
        {
            return View();
        }
   


        public void setCookies(string name, string pass)
        {
            Response.Cookies["username"].Value = name;
            Response.Cookies["username"].Expires = DateTime.Now.AddDays(300);
            Response.Cookies["pass"].Value = pass;
            Response.Cookies["pass"].Expires = DateTime.Now.AddDays(300);

        }
        public ActionResult getCookies()
        {
            //读取cookies
            string name = "";
            string pass = "";
            if (Request.Cookies["username"] != null)
            {
                name = Request.Cookies["username"].Value;
            }
            if (Request.Cookies["pass"] != null)
            {
                pass = Request.Cookies["pass"].Value;
            }

            object res = sql.ExecuteScalar("  select *  from UserInfo where UserName=@username and Pass=@password", new SqlParameter[] { new SqlParameter("@username", name), new SqlParameter("@password", pass) });
            if (res == null)
            {
                return Json(new { isSuccess = 1, username = name, password = pass });
            }
            else
            {
                // Session["Permission"] = res.ToString();


                DataSet dt = sql.ExecuteDataSet("select * from Menu where UserID=@res;select MainMenuID, Micon, Mname   FROM Menu where USERID =@res group by MainMenuID, Micon, Mname", new SqlParameter[] { new SqlParameter("@res", res.ToString()) });

                DataTable menuInfo = dt.Tables[0];
                DataTable mainMenu = dt.Tables[1];

                List<string> menuAccess = new List<string>();




                List<object> menu = new List<object>();
                for (int i = 0; i < mainMenu.Rows.Count; i++)
                {
                    List<object> Two = new List<object>();
                    for (int j = 0; j < menuInfo.Rows.Count; j++)
                    {
                        if (menuInfo.Rows[j]["MainMenuID"].ToString() == mainMenu.Rows[i]["MainMenuID"].ToString())
                        {
                            Two.Add(new { title = menuInfo.Rows[j]["title"].ToString(), name = menuInfo.Rows[j]["name"].ToString(), content = menuInfo.Rows[j]["content"].ToString(), icon = menuInfo.Rows[j]["icon"].ToString() });

                            menuAccess.Add(menuInfo.Rows[j]["title"].ToString());
                        }
                    }
                    menu.Add(new { name = mainMenu.Rows[i]["Mname"].ToString(), icon = mainMenu.Rows[i]["Micon"].ToString(), children = Two });
                }


                Session["menuAccess"] = menuAccess;

                return Json(menu, JsonRequestBehavior.AllowGet);
            }



        }
        public void ClearCookie()
        {
            //清除cookies
            Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["pass"].Expires = DateTime.Now.AddDays(-1);

        }
        public ActionResult ClearCookie1()
        {
            //清除cookies
            Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["pass"].Expires = DateTime.Now.AddDays(-1);
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        public ActionResult getUername()
        {
            string name = "";
            if (Request.Cookies["username"] != null)
            {
                name = Request.Cookies["username"].Value;
            }

            return Json(name, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login(string username, string password)
        {


            sql.ExecuteNonQuery("insert into updateTime (updateTime) values (@updateTime)", new SqlParameter[] { new SqlParameter("@updateTime", System.DateTime.Now.ToString()) });

            object res = sql.ExecuteScalar("  select *  from UserInfo where UserName=@username and Pass=@password", new SqlParameter[] { new SqlParameter("@username", username), new SqlParameter("@password", password) });

            if (res == null)
            {
                return Json(new { isSuccess = 1 });
            }
            else
            {
                // Session["username"] = username;

                setCookies(username, password);


                DataSet dt = sql.ExecuteDataSet("select * from Menu where UserID=@res;select MainMenuID, Micon, Mname   FROM Menu where USERID =@res group by MainMenuID, Micon, Mname", new SqlParameter[] { new SqlParameter("@res", res.ToString()) });

                //    Session["Permission"] = res.ToString();


                DataTable menuInfo = dt.Tables[0];
                DataTable mainMenu = dt.Tables[1];

                List<string> menuAccess = new List<string>();




                List<object> menu = new List<object>();
                for (int i = 0; i < mainMenu.Rows.Count; i++)
                {
                    List<object> Two = new List<object>();
                    for (int j = 0; j < menuInfo.Rows.Count; j++)
                    {
                        if (menuInfo.Rows[j]["MainMenuID"].ToString() == mainMenu.Rows[i]["MainMenuID"].ToString())
                        {
                            Two.Add(new { title = menuInfo.Rows[j]["title"].ToString(), name = menuInfo.Rows[j]["name"].ToString(), content = menuInfo.Rows[j]["content"].ToString(), icon = menuInfo.Rows[j]["icon"].ToString() });

                            menuAccess.Add(menuInfo.Rows[j]["title"].ToString());
                        }
                    }
                    menu.Add(new { name = mainMenu.Rows[i]["Mname"].ToString(), icon = mainMenu.Rows[i]["Micon"].ToString(), children = Two });
                }


                Session["menuAccess"] = menuAccess;

                return Json(menu, JsonRequestBehavior.AllowGet);


            }






        }

        public int fifler(string title)
        {
            //    List<string> meun = Session["menuAccess"] as List<string>;

            //    if (meun == null) return -1;

            //    if (meun.Contains(title))
            //    {
            //        return 1;
            //    }
            //    else
            //    {
            //        return -1;
            //    }


            string username = Request.Cookies["username"].Value.ToString();

            DataTable dt = sql.ExecuteDataSet("select * from Menu where UserName=@UserName and title=@title;", new SqlParameter[] { new SqlParameter("@Username", username.ToString()), new SqlParameter("@title", title.ToString()) }).Tables[0];

            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }

        }

        public ActionResult SelectUser(string Username)
        {

            if (fifler("用户管理") == 1)
            {

                if (Username == "")
                {
                    DataSet dt = sql.ExecuteDataSet("select * from UserInfo ", new SqlParameter[] { });

                    DataTable user = dt.Tables[0];



                    List<object> menu = new List<object>();
                    for (int i = 0; i < user.Rows.Count; i++)
                    {
                        menu.Add(new { UserID = user.Rows[i]["UserID"].ToString(), username = user.Rows[i]["Username"].ToString(), Dept = user.Rows[i]["Dept"].ToString(), Email = user.Rows[i]["Email"].ToString(), Pass = user.Rows[i]["Pass"].ToString() });
                    }




                    return Json(menu, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    DataSet dt = sql.ExecuteDataSet("select * from UserInfo  where Username like '%" + Username + "%'", new SqlParameter[] { });

                    DataTable user = dt.Tables[0];



                    List<object> menu = new List<object>();
                    for (int i = 0; i < user.Rows.Count; i++)
                    {
                        menu.Add(new { UserID = user.Rows[i]["UserID"].ToString(), username = user.Rows[i]["Username"].ToString(), Dept = user.Rows[i]["Dept"].ToString(), Email = user.Rows[i]["Email"].ToString() });
                    }




                    return Json(menu, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(-2, JsonRequestBehavior.AllowGet);
            }







        }

        //UserAccess

        public ActionResult UserAccess(string UserID)
        {
            if (fifler("用户管理") == 1)
            {

                DataTable Access = sql.ExecuteDataSet("select MenuID from Menu  where UserID=@UserID ", new SqlParameter[] { new SqlParameter("@UserID", UserID) }).Tables[0];

                DataTable Menu = sql.ExecuteDataSet("select * from MenuInfo  ", new SqlParameter[] { }).Tables[0];

                List<object> menuAccess = new List<object>();

                for (int i = 0; i < Access.Rows.Count; i++)
                {
                    menuAccess.Add(new { MenuID = Access.Rows[i]["MenuID"].ToString() });
                }

                List<object> menu = new List<object>();

                for (int i = 0; i < Menu.Rows.Count; i++)
                {
                    menu.Add(new { key = Menu.Rows[i]["MenuID"].ToString(), label = Menu.Rows[i]["name"].ToString() });
                }
                return Json(new { menu = menu, Access = menuAccess }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(-2, JsonRequestBehavior.AllowGet);
            }
        }

        //MangementAccess
        public ActionResult MangementAccess(string UserID, string direction, string MenuID)
        {

            if (fifler("用户管理") == 1)
            {
                List<int> MenuArray = JsonConvert.DeserializeObject<List<int>>(MenuID);

                if (direction == "left")
                {
                    string Delete = string.Join(",", MenuArray.ToArray());

                    int res = sql.ExecuteNonQuery("delete MenuAccess where UseID=@UserID and MenuID  in (" + Delete + ")", new SqlParameter[] { new SqlParameter("@UserID", UserID) });
                }
                else
                {

                    for (int i = 0; i < MenuArray.Count; i++)
                    {

                        sql.ExecuteNonQuery("insert into MenuAccess (useID,MenuID) values (@useID,@MenuID)", new SqlParameter[] { new SqlParameter("@useID", UserID), new SqlParameter("@MenuID", MenuArray[i].ToString()) });

                    }
                }



                //DataTable dt = sql.ExecuteDataSet("select MenuID  from  MenuAccess where UseID=@UserID ", new SqlParameter[] { new SqlParameter("@UserID", UserID) }).Tables[0];

                //for (int i = 0; i < MenuArray.Count; i++)
                //{
                //    DataRow[] row = dt.Select("MenuID=" + MenuArray[i].ToString());
                //    if (row.Count() == 0)
                //    {
                //        sql.ExecuteNonQuery("insert into MenuAccess (useID,MenuID) values (@useID,@MenuID)", new SqlParameter[] { new SqlParameter("@useID", UserID), new SqlParameter("@MenuID", MenuArray[i].ToString()) });
                //    }
                //}


                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(-2, JsonRequestBehavior.AllowGet);
            }
        }

        //usercurd


        public ActionResult usercurd(string useID, string name, string Dept, string Email, string Pass)
        {
            int res = -1;
            if (fifler("用户管理") == 1)
            {
                if (useID == "")
                {
                    object ID = sql.ExecuteScalar("select * from UserInfo where Username=@Username", new SqlParameter[] { new SqlParameter("@Username", name) });
                    if (ID == null)
                    {
                        res = sql.ExecuteNonQuery("insert into UserInfo ( Username, Dept, Email, Pass) values (@Username, @Dept, @Email, @Pass) ", new SqlParameter[] { new SqlParameter("@Username", name), new SqlParameter("@Dept", Dept), new SqlParameter("@Email", Email), new SqlParameter("@Pass", Pass) });
                    }
                    else
                    {
                        res = -2;
                    }



                }
                else
                {
                    res = sql.ExecuteNonQuery("update UserInfo set Pass=@Pass,Dept=@Dept,Email=@Email where UserID=@UserID ", new SqlParameter[] { new SqlParameter("@Pass", Pass), new SqlParameter("@Dept", Dept), new SqlParameter("@Email", Email), new SqlParameter("@UserID", useID) });
                }


                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(-2, JsonRequestBehavior.AllowGet);
            }
        }


        //userdelete

        public ActionResult userdelete(string UserID)
        {
            int res = -1;
            if (fifler("用户管理") == 1)
            {


                if (UserID == "")
                {




                }
                else
                {
                    res = sql.ExecuteNonQuery("delete UserInfo where  UserID=@UserID ", new SqlParameter[] { new SqlParameter("@UserID", UserID) });
                }


                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(-2, JsonRequestBehavior.AllowGet);
            }
        }
        //MainMenu

        public ActionResult MainMenu()
        {
          
            if (fifler("菜单管理") == 1)
            {

                DataSet dt = sql.ExecuteDataSet("select * from MainMenu ;select * from MenuInfo" );

                List<object> main = new List<object>();
                List<object> children = new List<object>();
                DataTable dtMain = dt.Tables[0];
                DataTable dtChildren = dt.Tables[1];


                for(int i = 0; i < dtMain.Rows.Count; i++)
                {
                    main.Add(new
                    {
                        MainMenuID = dtMain.Rows[i]["MainMenuID"].ToString(),
                        name = dtMain.Rows[i]["name"].ToString(),
                        icon = dtMain.Rows[i]["icon"].ToString()

                    });
                }

                for (int i = 0; i < dtChildren.Rows.Count; i++)
                {
                    children.Add(new
                    {
                        MenuID = dtChildren.Rows[i]["MenuID"].ToString(),
                        title = dtChildren.Rows[i]["title"].ToString(),
                        name = dtChildren.Rows[i]["name"].ToString(),
                        content = dtChildren.Rows[i]["content"].ToString(),
                        icon = dtChildren.Rows[i]["icon"].ToString(),
                        MMID = dtChildren.Rows[i]["MMID"].ToString(),

                    });
                }



                return Json(new { main= main, children= children,res=1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {  res = -2 }, JsonRequestBehavior.AllowGet);
            }
        }

        //addMainData
        public ActionResult addMainData(string name,string icon, string MainMenuID)
        {

            if (fifler("菜单管理") == 1)
            {


                if (MainMenuID == "0")
                {
                    int res = sql.ExecuteNonQuery("insert into MainMenu(name,icon) values (@name,@icon)", new SqlParameter[] { new SqlParameter("@name", name), new SqlParameter("@icon", icon) });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int res = sql.ExecuteNonQuery("update MainMenu set name=@name,icon=@icon where MainMenuID=@MainMenuID", new SqlParameter[] { new SqlParameter("@name", name), new SqlParameter("@icon", icon), new SqlParameter("@MainMenuID", MainMenuID) });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);
                }

         


              
            }
            else
            {
                return Json(new { res = -2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult addChildData(string title, string icon,string content,string MMID, string MenuID)
        {

            if (fifler("菜单管理") == 1)
            {

                if (MenuID == "0")
                {
                    int res = sql.ExecuteNonQuery("insert into MenuInfo(title,name,icon,content,MMID) values (@title,@name,@icon,@content,@MMID)", new SqlParameter[] {
                    new SqlParameter("@name", title),
                    new SqlParameter("@icon", icon),
                         new SqlParameter("@content", content),
                    new SqlParameter("@title", title),
                         new SqlParameter("@MMID", MMID)

                });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    int res = sql.ExecuteNonQuery("update MenuInfo set title=@title,name=@name,icon=@icon,content=@content,MMID=@MMID where MenuID=@MenuID", new SqlParameter[] {
                    new SqlParameter("@name", title),
                    new SqlParameter("@icon", icon),
                         new SqlParameter("@content", content),
                    new SqlParameter("@title", title),
                         new SqlParameter("@MMID", MMID),
                           new SqlParameter("@MenuID", MenuID)

                });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);
                }
              


               
            }
            else
            {
                return Json(new { res = -2 }, JsonRequestBehavior.AllowGet);
            }
        }

        //deleteMain

        public ActionResult deleteMain( string MainMenuID)
        {

            if (fifler("菜单管理") == 1)
            {


             
                    
             DataTable dt=sql.ExecuteDataSet("select * from MenuInfo where MMID=@MainMenuID", new SqlParameter[] { new SqlParameter("@MainMenuID", MainMenuID) }).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    return Json(new { res = -3 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int res = sql.ExecuteNonQuery("delete MainMenu  where MainMenuID=@MainMenuID", new SqlParameter[] { new SqlParameter("@MainMenuID", MainMenuID) });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);
                }
        
              





            }
            else
            {
                return Json(new { res = -2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult deleteChild( string MenuID)
        {

            if (fifler("菜单管理") == 1)
            {

               
                    int res = sql.ExecuteNonQuery("delete MenuInfo  where MenuID=@MenuID", new SqlParameter[] {
                   
                           new SqlParameter("@MenuID", MenuID)

                });
                    return Json(new { res = res }, JsonRequestBehavior.AllowGet);
              




            }
            else
            {
                return Json(new { res = -2 }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Dates()
        {

                DataSet dt = sql.ExecuteDataSet("select * from updateTime ");

                List<object> main = new List<object>();
              
                DataTable dtMain = dt.Tables[0];
             


                for (int i = 0; i < dtMain.Rows.Count; i++)
                {
                    main.Add(new
                    {
                        updateTime = dtMain.Rows[i]["updateTime"].ToString(),
                     

                    });
                }

            


                return Json(new { main = main}, JsonRequestBehavior.AllowGet);
          
        }

    }
}