using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQChat
{
    public partial class PluginForm : Form
    {
        private dynamic _plugin = null;
        public PluginForm()
        {
            InitializeComponent();
        }

        public void InitPlugin(dynamic plugin)
        {
            _plugin = plugin;
            if (_plugin == null)
            {
                return;
            }
            InternalInitPlugin();
        }

        private void InternalInitPlugin()
        {
            labelTitle.Text = _plugin.PluginName;
            labelDesc.Text = _plugin.AboutMessage;
            richTextBox1.Text = _plugin.Setting;
            Dictionary<string, string> filters = _plugin.Filters;
            if (filters != null && filters.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    sb.AppendFormat("{0} {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                }
                var str = sb.ToString();
                labelFilter.Text = str;
            }
            else
            {
                labelFilter.Text = "无内容";
            }
            flowLayoutPanel1.Controls.Clear();
            Dictionary<string, string> menus = _plugin.Menus;
            if (menus != null && menus.Count > 0)
            {
                foreach (KeyValuePair<string, string> menu in menus)
                {
                    var button = new Button()
                    {
                        Text = menu.Key,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink
                    };
                    button.Click += delegate { _plugin.MenuClicked(menu.Value); };
                    flowLayoutPanel1.Controls.Add(button);
                }
            }
        }

        private void PluginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (_plugin == null)
            {
                return;
            } 
            InternalInitPlugin();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_plugin == null)
            {
                return;
            }
            _plugin.Setting = richTextBox1.Text;
            this.Hide();
        }
    }
}
