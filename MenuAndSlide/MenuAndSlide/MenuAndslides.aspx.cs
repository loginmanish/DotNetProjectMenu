using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace MenuAndSlide
{
    public partial class MenuAndSlides : System.Web.UI.Page
    {
        string connectionString = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            //SQL string goes here
            string sSQL = "Select * FROM categories Where parent is null;";
            connectionString = ConfigurationManager.AppSettings["slides_ConnectionString"];
            
            MySqlCommand command = new MySqlCommand(sSQL);
            command.Connection = getOpenConnection();
            MySqlDataReader reader = command.ExecuteReader();
            createMenuItems(reader);
            command.Connection.Close();
            command = null;

            string strCategoryId = Request.QueryString["catid"];
            displayDetails(strCategoryId);
        }

        private void displayDetails(string strCategoryId)
        {
            if (strCategoryId != null && strCategoryId.Trim().Length > 0)
            {
                MySqlCommand command = new MySqlCommand("select *, IF(title2 IS NULL , '', title2) as titlenew, IF(pubDate IS NULL, '', pubDate) as newPubDate from slides where slide in (select slide from categories_slides where category = " + strCategoryId + ")");
                command.Connection = getOpenConnection();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    HtmlGenericControl art = new HtmlGenericControl("details");
                    HtmlGenericControl aHead = new HtmlGenericControl("summary");
                    aHead.InnerText = reader.GetString("title");
                    art.Controls.Add(aHead);
                    HtmlGenericControl artPar = new HtmlGenericControl("p");
                    artPar.InnerText = reader.GetString("titlenew");
                    art.Controls.Add(artPar);
                    HtmlGenericControl artPar2 = new HtmlGenericControl("p");
                    if (reader.GetString("newPubDate").Length > 0)
                        artPar2.InnerHtml = "published date: " + reader.GetDateTime("pubDate").ToString("dd-mmm-yyyy") + "<br />";
                    artPar2.InnerHtml += "Monthly Searches: " + reader.GetString("monthlySearches") + "<br />";
                    artPar2.InnerHtml += "Google Pos: " + reader.GetString("googlePos");
                    art.Controls.Add(artPar2);

                    detailsView.Controls.Add(art);
                }
                reader.Close();
                command.Connection.Close();
                command = null;
            }
        }

        private void createMenuItems(MySqlDataReader reader)
        {
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();
            while (reader.Read())
            {
                //TODO Create Menu
                HtmlGenericControl contLI = new HtmlGenericControl("li");
                HtmlGenericControl contSpan = new HtmlGenericControl("div");
                HtmlAnchor anc = new HtmlAnchor();
                anc.HRef = "MenuAndSlides.aspx?catid=" + reader.GetString("category");
                anc.InnerText = reader.GetString("name");
                contSpan.Controls.Add(anc);
                MySqlCommand command = new MySqlCommand("SELECT * FROM categories where parent = " + reader.GetString("category"));
                command.Connection = getOpenConnection();
                MySqlDataReader subMenuReader = command.ExecuteReader();
                contLI.Controls.Add(contSpan);
                HtmlGenericControl conChild = getSubMenu(subMenuReader);
                if (conChild != null)
                    contLI.Controls.Add(conChild);
                menu.Controls.Add(contLI);

                command.Connection.Close();
                command = null;
            }
            reader.Close();
        }

        private HtmlGenericControl getSubMenu(MySqlDataReader reader)
        {
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();
            bool hasData = false;
            HtmlGenericControl subMenuHolder = new HtmlGenericControl("ul");
            while (reader.Read())
            {
                hasData = true;
                HtmlGenericControl subMenu = new HtmlGenericControl("li");
                HtmlGenericControl contSpan = new HtmlGenericControl("div");
                HtmlAnchor anc = new HtmlAnchor();
                anc.HRef = "MenuAndSlides.aspx?catid=" + reader.GetString("category");
                anc.InnerText = reader.GetString("name");
                contSpan.Controls.Add(anc);
                MySqlCommand command = new MySqlCommand("Select * FROM categories_slides Where category = " + reader.GetString("category") + " order by slide;");
                command.Connection = getOpenConnection();
                MySqlDataReader slideReader = command.ExecuteReader();
                subMenu.Controls.Add(contSpan);
                HtmlGenericControl conChild = getSlideMenu(slideReader);
                if (conChild != null)
                    subMenu.Controls.Add(conChild);
                subMenuHolder.Controls.Add(subMenu);

                command.Connection.Close();
                command = null;
            }
            reader.Close();
            if (hasData)
                return subMenuHolder;
            else
                return null;
            
        }

        private HtmlGenericControl getSlideMenu(MySqlDataReader reader)
        {
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();
            bool hasData = false;
            HtmlGenericControl slideHolder = new HtmlGenericControl("ul");
            while (reader.Read())
            {
                hasData = true;
                HtmlGenericControl slideItem = new HtmlGenericControl("li");
                HtmlGenericControl contSpan = new HtmlGenericControl("div");
                HtmlAnchor anc = new HtmlAnchor();
                anc.HRef = "MenuAndSlides.aspx?catid=" + reader.GetString("category");
                anc.InnerText = reader.GetString("slide");
                contSpan.Controls.Add(anc);
                slideItem.Controls.Add(contSpan);
                slideHolder.Controls.Add(slideItem);
            }
            reader.Close();
            if (hasData)
                return slideHolder;
            else
                return null;
        }

        private MySqlConnection getOpenConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }

    
}
