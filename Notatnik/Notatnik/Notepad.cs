using Microsoft.Win32;
using Notatnik.Elements;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Notatnik
{
    public partial class Notepad
    {
        //As when I am writing this UI is not implemented, so this var acts like text in TextBox (for OpenFile and SaveFile purposes)!
        public string writtenText = "# Test of Header\nTest of *paragraph*\nwhich has __two__ lines\n_________________\n1. This is first element of list\n8. that's a __second__ element\n- and this is unordered list\n- which has two elements";

        public List<Element> content = [];
        public GitHubUser user = new();


        //Format text from string form into list of elements
        public void FormatText()
        {
            string[] writtenTextByLine = writtenText.Split('\n');
            for (int i = 0; i < writtenTextByLine.Length; i++)
            {
                string currentLine = writtenTextByLine[i];
                //If is header:
                if (currentLine.Length > 0 && currentLine[0] == '#')
                {
                    int headerType = currentLine.Length - currentLine.TrimStart('#').Length;
                    string text = currentLine.TrimStart('#').TrimStart();
                    content.Add(new Header(text, headerType));
                    continue;
                }

                if (RuleRegex().IsMatch(currentLine))
                {
                    content.Add(new Rule());
                    continue;
                }

                //If is unordered list
                if(UnorderedListRegex().IsMatch(currentLine))
                {
                    Elements.MarkdownList list;
                    if(content.Count > 0 && content[^1] is Elements.MarkdownList existingList && !existingList.isOrdered)
                    {
                        list = existingList;
                    }
                    else
                    {
                        list = new Elements.MarkdownList(false);
                        content.Add(list);
                    }
                    string listItemContent = currentLine[2..]; // Remove the list marker and space
                    list.AddListElement(SplitStringByStyles(listItemContent));
                    continue;
                }

                //if is ordered list

                if (OrderedListRegex().IsMatch(currentLine))
                {
                    Elements.MarkdownList? list = null;
                    if (content.Count > 0 && content[^1] is Elements.MarkdownList existingList && existingList.isOrdered)
                    {
                        list = existingList;
                    }
                    else if(OrderedListFirstElementRegex().IsMatch(currentLine))
                    {
                        list = new Elements.MarkdownList(true);
                        content.Add(list);
                    }

                    if (list != null)
                    {
                        string listItemContent = currentLine[2..];
                        list.AddListElement(SplitStringByStyles(listItemContent));
                        continue;
                    }
                }

                //Else is paragraph
                if (content.Count > 0 && content[^1] is Elements.Paragraph paragraph && currentLine != "")
                {
                    paragraph.content.AddRange(SplitStringByStyles(currentLine + "\n"));
                }
                else
                {
                    content.Add(new Elements.Paragraph(SplitStringByStyles(currentLine + "\n")));
                }
            }
        }

        //Takes string, tries to find bold, italic or italic bold, if finds styles then returns list of elements where every style is different element in list
        public static List<Element> SplitStringByStyles(string input)
        {
            string[] markers = ["***", "___", "*__", "_**", "**", "__", "*", "_"];
            List<Element> result = [];

            foreach (var marker in markers)
            {
                int first = input.IndexOf(marker);
                if (first + marker.Length > input.Length)
                    continue;
                int last = input.IndexOf(marker, first + marker.Length);

                if (first != -1 && last != -1)
                {
                    string before = input[..first];
                    string between = input[(first + marker.Length)..last];
                    if(between == "")
                        continue;
                    string after = input[(last + marker.Length)..];

                    if (before != "")
                        result.Add(new Elements.TextBlock(before));

                    switch (marker.Length)
                    {
                        case 3:
                            result.Add(new Elements.TextBlock(between) { style = Elements.TextBlock.TextStyle.ItalicBold });
                            break;
                        case 2:
                            result.Add(new Elements.TextBlock(between) { style = Elements.TextBlock.TextStyle.Bold });
                            break;
                        case 1:
                            result.Add(new Elements.TextBlock(between) { style = Elements.TextBlock.TextStyle.Italic });
                            break;
                    }

                    result.AddRange(SplitStringByStyles(after));
                }
            }
            if(result.Count == 0)
            {
                result.Add(new Elements.TextBlock(input));
            }

            return result;
        }

        public string ParseIntoString()
        {
            string text = "";
            for (int i = 0; i < content.Count; i++)
            {
                text += content[i].ParseToString();
            }
            return text;
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select File",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt|Markdown File (*.md)|*.md"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(filePath);
                writtenText = fileContent;
                FormatText();
                MessageBox.Show($"Opened text: {ParseIntoString()}");
            }
        }

        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new() {
                Title = "Select Save File Destination",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt|Markdown File (*.md)|*.md",
                FileName = "file.md"
            };
            if (saveFileDialog.ShowDialog() == true) { 
                string filePath = saveFileDialog.FileName;
                //TODO: Convert content into string using function
                string fileContent = ParseIntoString();

                File.WriteAllText(filePath, fileContent);
                MessageBox.Show($"Saved file in: {filePath}");
            }
        }

        public void SendFile(int fileId, GitHubUser user)
        {
            throw new NotImplementedException();
        }

        public void GetFile(int fileId)
        {
            throw new NotImplementedException();
        }

        public void AddElement(Element element)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex(@"^(?:\*{3,}|-{3,}|_{3,})$")] private static partial Regex RuleRegex();
        [GeneratedRegex(@"^[-\*\+]\s")] private static partial Regex UnorderedListRegex();
        [GeneratedRegex(@"^\d+\.\s")] private static partial Regex OrderedListRegex();
        [GeneratedRegex(@"^1\.\s")] private static partial Regex OrderedListFirstElementRegex();
    }
}