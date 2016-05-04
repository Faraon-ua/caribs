using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caribs.Common.Helpers;

namespace Caribs.AutoClicker
{
    public partial class Form1 : Form
    {
        private WebBrowserHelper _browserHelper;
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            _browserHelper = new WebBrowserHelper(webBrowser1);
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //login
            if (webBrowser1.Url.AbsoluteUri.Contains("https://caribbeanbridge.com/office/web/site/login"))
            {
                webBrowser1.Document.GetElementById("loginform-login").InnerText = "Faraon.ua";
                webBrowser1.Document.GetElementById("loginform-password").InnerText = "poas054";
                _browserHelper.GetElementByName("button", "login-button").InvokeMember("click");
            }
            if (webBrowser1.Url.AbsoluteUri == "https://caribbeanbridge.com/office/web/account/index")
            {
//                _browserHelper.GetElementsByClassName("a", "pad").FirstOrDefault(entry=>entry.InnerHtml.Contains("google-plus")).InvokeMember("click");
                webBrowser1.Navigate("https://caribbeanbridge.com/office/web/account/social-pop-up?service=gp");
            }
            if (webBrowser1.Url.AbsoluteUri == "https://caribbeanbridge.com/office/web/account/social-pop-up?service=gp")
            {
                _browserHelper.GetElementsByClassName("a", "clickevent").FirstOrDefault().InvokeMember("click");
            }
        }

        void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
//            throw new NotImplementedException();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://caribbeanbridge.com/office/web/site/login");
        }
    }
}
