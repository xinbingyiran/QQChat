using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace QQChat.Classes
{
    public enum RichMessageType
    {
        TYPETEXT = 1,
        TYPERTF = 2,
        TYPEBYTE = 3,
    }

    public interface IRichMessage
    {
        RichMessageType MessageType { get; }
        object Message { get; }
        Color? MessageColor { get; set; }
        bool AppendTo(RichTextBox rtb);
    }

    public class RichMessageText : IRichMessage
    {
        public RichMessageType MessageType
        {
            get { return RichMessageType.TYPETEXT; }
        }

        public object Message
        {
            get;
            private set;
        }

        public Color? MessageColor
        {
            get;
            set;
        }

        public RichMessageText(string message)
        {
            Message = message;
        }

        public bool AppendTo(RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException();
            rtb.Select(int.MaxValue, 0);
            rtb.SelectionColor = (MessageColor == null) ? rtb.SelectionColor : MessageColor.Value;
            rtb.SelectedText = this.Message as string;
            return true;
        }
    }

    public class RichMessageRtf : IRichMessage
    {

        public RichMessageType MessageType
        {
            get { return RichMessageType.TYPERTF; }
        }

        public object Message
        {
            get;
            private set;
        }

        public Color? MessageColor
        {
            get;
            set;
        }

        public RichMessageRtf(Image image)
        {
            Message = image;
        }


        public bool AppendTo(RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException();
            string rtf = RTB_InsertImg.GetImageRtf(rtb, Message as Image);
            rtb.Select(int.MaxValue, 0);
            rtb.SelectionColor = (MessageColor == null) ? rtb.SelectionColor : MessageColor.Value;
            rtb.SelectedRtf = rtf;
            return true;
        }
    }

    public class RichMessageByte : IRichMessage
    {

        public RichMessageType MessageType
        {
            get { return RichMessageType.TYPERTF; }
        }

        public object Message
        {
            get;
            private set;
        }

        public Color? MessageColor
        {
            get;
            set;
        }

        public RichMessageByte(Image image)
        {
            Message = image;
        }


        public bool AppendTo(RichTextBox rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException();
            Image image = Message as Image;
            //MemoryStream stream = new MemoryStream();
            //image.Save(stream, image.RawFormat);
            rtb.AppendImage(image);
            return true;
        }
    }

    public static class RichBoxAddtion
    {
        //public static void AppendImage(this RichTextBox textbox, Image image)
        //{
        //    if (image == null)
        //    {
        //        return;
        //    }
        //    MemoryStream stream = new MemoryStream();
        //    image.Save(stream, image.RawFormat);
        //    var imageBytes = stream.GetBuffer();
        //    if (stream == null)
        //    {
        //        return;
        //    }
        //    string _Guid = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "");
        //    StringBuilder _RtfText = new StringBuilder(@"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052{\fonttbl{\f0\fnil\fcharset134 \'cb\'ce\'cc\'e5;}}\uc1\pard\lang2052\f0\fs18{\object\objemb{\*\objclass Paint.Picture}");
        //    int _Width = image.Width * 15;
        //    int _Height = image.Height * 15;
        //    _RtfText.Append(@"\objw" + _Width.ToString() + @"\objh" + _Height.ToString());
        //    _RtfText.AppendLine(@"{\*\objdata");
        //    _RtfText.AppendLine(@"010500000200000007000000504272757368000000000000000000" + BitConverter.ToString(BitConverter.GetBytes(imageBytes.Length + 20)).Replace("-", ""));
        //    _RtfText.Append("7A676B65" + _Guid); //标记            
        //    _RtfText.AppendLine(BitConverter.ToString(imageBytes).Replace("-", ""));
        //    _RtfText.AppendLine(@"0105000000000000}{\result{\pict\wmetafile0}}}}");
        //    textbox.SelectedRtf = _RtfText.ToString();
        //}

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
