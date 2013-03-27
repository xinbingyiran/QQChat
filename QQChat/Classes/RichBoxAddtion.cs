using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace QQChat.Classes
{
    public static class RichBoxAddtion
    {
        public static void AppendLine(this RichTextBox textbox, string lineStr)
        {
            int lineCount = 1000;
            string[] lines = textbox.Lines;
            int selectStart = textbox.SelectionStart;
            int selectLength = textbox.SelectionLength;
            if (lines.Count() > lineCount)
            {
                int index = textbox.GetFirstCharIndexFromLine(lines.Count() - lineCount);
                if (selectStart > index)
                {
                    selectStart -= index;
                }
                else
                {
                    selectStart = 0;
                }
                textbox.SelectionStart = 0;
                textbox.SelectionLength = index;
                textbox.SelectedText = "";
            }
            int p1 = textbox.TextLength;
            textbox.AppendText(lineStr + "\n");
            int p2 = lineStr.Length;
            textbox.SelectionStart = p1;
            textbox.SelectionLength = p2;
            if (selectLength > 0)
            {
                textbox.SelectionStart = selectStart;
                textbox.SelectionLength = selectLength;
            }
            else
            {
                textbox.SelectionStart = textbox.TextLength;
            }
        }

        public static void AppendLine(this RichTextBox textbox, string lineStr, Color lineColor)
        {
            int lineCount = 1000;
            string[] lines = textbox.Lines;
            int selectStart = textbox.SelectionStart;
            int selectLength = textbox.SelectionLength;
            if (lines.Count() > lineCount)
            {
                int index = textbox.GetFirstCharIndexFromLine(lines.Count() - lineCount);
                if (selectStart > index)
                {
                    selectStart -= index;
                }
                else
                {
                    selectStart = 0;
                }
                textbox.SelectionStart = 0;
                textbox.SelectionLength = index;
                textbox.SelectedText = "";
            }
            int p1 = textbox.TextLength;  
            textbox.AppendText(lineStr + "\n");
            int p2 = lineStr.Length;
            textbox.SelectionStart = p1;
            textbox.SelectionLength = p2;
            textbox.SelectionColor = lineColor;
            if (selectLength > 0)
            {
                textbox.SelectionStart = selectStart;
                textbox.SelectionLength = selectLength;
            }
            else
            {
                textbox.SelectionStart = textbox.TextLength;
            }
        }
    }
}
