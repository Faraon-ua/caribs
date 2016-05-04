using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Caribs.Common.Helpers
{
    public class WebBrowserHelper
    {
        private WebBrowser _browser;
        public WebBrowserHelper(WebBrowser webBrowser)
        {
            _browser = webBrowser;
        }

        public List<HtmlElement> GetElementsByClassName(string tagName, string className)
        {
            var elems = _browser.Document.GetElementsByTagName(tagName);
            return elems.Cast<HtmlElement>().Where(elem => elem.GetAttribute("className") == className).ToList();
        } 
        
        public HtmlElement GetElementByName(string tagName, string name)
        {
            var elems = _browser.Document.GetElementsByTagName(tagName);
            return elems.Cast<HtmlElement>().FirstOrDefault(elem => elem.GetAttribute("name") == name);
        }
    }
}
