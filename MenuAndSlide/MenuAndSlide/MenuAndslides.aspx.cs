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
            //Set connection string
            connectionString = ConfigurationManager.AppSettings["slides_ConnectionString"];
            try
            {
                //Get open connection command
                MySqlCommand command = getDataCommand("Select * FROM categories Where parent is null;");
            
                //callback for creating menu items in unordered lists
                createMenuItems(command.ExecuteReader());

                //close connection and dispose command object
                command.Connection.Close();
                command = null;
            }
            catch (Exception ex)
            {
                message.InnerText = ex.Message;
            }

            //To display slide details under a category if catid query string exists
            string strCategoryId = Request.QueryString["catid"];
            displayDetails(strCategoryId);
        }

        /// <summary>
        /// Displays slide details under the category id
        /// </summary>
        /// <param name="strCategoryId">The category id for displaying all slides in it</param>
        private void displayDetails(string strCategoryId)
        {
            if (strCategoryId != null && strCategoryId.Trim().Length > 0)
            {

                try
                {
                    //Get command with open connection
                    MySqlCommand command = getDataCommand("select *, IF(title2 IS NULL , '', title2) as titlenew, IF(pubDate IS NULL, '', pubDate) as newPubDate from slides where slide in (select slide from categories_slides where category = " + strCategoryId + ")");
                    //get data reader by executing command
                    MySqlDataReader reader = command.ExecuteReader();
                    //loop through data rows
                    while (reader.Read())
                    {
                        //Generate Details items for each slide
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
                    //close data reader object
                    reader.Close();
                    //close connection object
                    command.Connection.Close();
                    //dispose command object
                    command = null;
                }
                catch (Exception ex)
                {
                    message.InnerText = ex.Message;
                }
            }
        }

        /// <summary>
        /// Helper function to create <li> tag with anchor and return it
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="catName"></param>
        /// <returns></returns>
        private HtmlGenericControl getUnorderedLists(string catId, string catName)
        {
            HtmlGenericControl contLI = new HtmlGenericControl("li");
            HtmlGenericControl contDiv = new HtmlGenericControl("div");
            HtmlAnchor anchor = new HtmlAnchor();
            anchor.HRef = "MenuAndSlides.aspx?catid=" + catId;
            anchor.InnerText = catName;
            contDiv.Controls.Add(anchor);
            contLI.Controls.Add(contDiv);
            return contLI;
        }

        /// <summary>
        /// Create main menu lists
        /// </summary>
        /// <param name="reader">SQL data reader object</param>
        private void createMenuItems(MySqlDataReader reader)
        {
            //if data reader is null or closed then throw exception
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();

            //looping through data rows
            while (reader.Read())
            {
                //get the <li> tag to add to main <ul> tag child items
                HtmlGenericControl unorderedListItem = getUnorderedLists(reader.GetString("category"), reader.GetString("name"));
                //get sql command object with connection
                MySqlCommand command = getDataCommand("SELECT * FROM categories where parent = " + reader.GetString("category"));
                
                //get submenu <ul> element with its child <li> items
                HtmlGenericControl conChild = getSubMenu(command.ExecuteReader());
                
                //if submenu is not null
                if (conChild != null)
                    unorderedListItem.Controls.Add(conChild);

                //add menu list items to main un ordered list
                menu.Controls.Add(unorderedListItem);

                //close connection
                command.Connection.Close();
                //dispose command object
                command = null;
            }
            //close data reader object
            reader.Close();
        }

        /// <summary>
        /// Get the submenu list items
        /// </summary>
        /// <param name="reader">The sql open data reader object</param>
        /// <returns>the unordered list of sub menu</returns>
        private HtmlGenericControl getSubMenu(MySqlDataReader reader)
        {
            //if reader is null or closed then throw exception
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();
            //to check if data reader has any data
            bool hasData = reader.HasRows;
            HtmlGenericControl subMenuHolder = new HtmlGenericControl("ul");
            while (reader.Read())
            {
                //hasData = true;
                //Get <li> items to unordered list
                HtmlGenericControl subMenu = getUnorderedLists(reader.GetString("category"), reader.GetString("name"));
                //get data command object with open connection
                MySqlCommand command = getDataCommand("Select * FROM categories_slides Where category = " + reader.GetString("category") + " order by slide;");
                //Get the Slide Menu unordered lists
                HtmlGenericControl conChild = getSlideMenu(command.ExecuteReader());
                
                if (conChild != null)
                    subMenu.Controls.Add(conChild);
                //add slidemenu to <ul> list
                subMenuHolder.Controls.Add(subMenu);
                //close connection object
                command.Connection.Close();
                //dispose data command object
                command = null;
            }
            //close data reader object
            reader.Close();
            //if had data then only return <ul> tags
            if (hasData)
                return subMenuHolder;
            else
                return null;
            
        }

        /// <summary>
        /// method to get the slide <ul> element with items
        /// </summary>
        /// <param name="reader">Sql Data Reader object</param>
        /// <returns>returns the unordered list object</returns>
        private HtmlGenericControl getSlideMenu(MySqlDataReader reader)
        {
            //if data reader is null or do not have rows then throw exception
            if (reader == null || reader.IsClosed)
                throw new InvalidDataReaderException();

            bool hasData = reader.HasRows;
            HtmlGenericControl slideHolder = new HtmlGenericControl("ul");
            while (reader.Read())
            {
                //Get the slide menu <li> to insert into the unordered list
                HtmlGenericControl slideItem = getUnorderedLists(reader.GetString("category"), reader.GetString("slide"));
                //add list to unordered list object
                slideHolder.Controls.Add(slideItem);
            }
            //close data reader object
            reader.Close();
            //if reader had data then returns the <ul> element else null
            if (hasData)
                return slideHolder;
            else
                return null;
        }

        /// <summary>
        /// Get the open connection object
        /// </summary>
        /// <returns>Returns object of MySqlConnection class</returns>
        private MySqlConnection getOpenConnection()
        {
            //Create object of MySqlConnection using connection string
            MySqlConnection conn = new MySqlConnection(connectionString);
            //open the connection object
            conn.Open();
            //return the connection object
            return conn;
        }

        /// <summary>
        /// Gets the command object with open connection
        /// </summary>
        /// <param name="strSQL">SQL to execute on command</param>
        /// <returns>Object of MySqlCommand class</returns>
        private MySqlCommand getDataCommand(string strSQL)
        {
            //create object of MySqlCommand
            MySqlCommand command = new MySqlCommand(strSQL);
            //Set open connection to command
            command.Connection = getOpenConnection();
            //return command object
            return command;
        }
    }

    
}
