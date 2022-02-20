using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Linq;

namespace HTTP_WPF_Client_Project
{
    class BrowsersLinks
    {
        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        public static IEnumerable<Site> getLinks()
        {
            List<Site> sites = new List<Site>();
            var browsersList = new List<string>
            {
                "chrome",
                "firefox",
                "iexplore",
                "opera",
                "msedge"
            };

            foreach (var singleBrowser in browsersList)
            {
                if (singleBrowser == "chrome")
                {
                    var rootCollection = AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1"));
                    foreach (AutomationElement root in rootCollection)
                    {
                        var textP = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                        var vpi = textP.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
                        string uri = vpi.ToString();
                        if(uri.IndexOf(@"//")==-1)
                        {
                            uri = "https://" + uri;
                        }
                        Site s = new Site()
                        {
                            Browser = "chrome",
                            SiteUri = uri == "https://" ? null : new Uri(uri),
                            Header = root.Current.Name
                        };
                        sites.Add(s);
                    }
                }
                else
                {
                    var process = Process.GetProcessesByName(singleBrowser);
                    if (process.Length > 0)
                    {
                        foreach (Process singleProcess in process)
                        {
                            IntPtr hWnd = singleProcess.MainWindowHandle;
                            int length = GetWindowTextLength(hWnd);

                            StringBuilder text = new StringBuilder(length + 1);
                            GetWindowText(hWnd, text, text.Capacity);
                            if (text.ToString() != "")
                            {
                                Site s = new Site()
                                {
                                    Browser = singleBrowser,
                                    SiteUri = null,
                                    Header = text.ToString()
                                };
                                sites.Add(s);
                            }
                        }
                    }
                }
            }
            return sites;
        }
    }
}
