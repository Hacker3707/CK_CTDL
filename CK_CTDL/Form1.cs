using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;

namespace CK_CTDL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            lvHistory.Columns.Add("Tên file", 125);
            lvHistory.Columns.Add("Kết quả", 150);
            lvHistory.Columns.Add("Thời gian", 100);
            lvHistory.MouseClick += lvHistory_MouseClick;

        }

        //==================== Drag file vào =========================
        private void txtHtml_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                txtHtml.TextAlign = HorizontalAlignment.Left;
                txtHtml.Font = new Font("Consolas", 9, FontStyle.Regular);
                e.Effect = DragDropEffects.Copy;
            }

        }

        private void txtHtml_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                int x = 0;
                int i = files.Length;
                lblGuide.Text = $"Bạn đã kéo thêm vào {i} file !";
                foreach (string filePath in files)
                {
                    try
                    {
                        if (files.Length == 1)
                        {
                            txtHtml.Text = "";
                        }
                        string content = File.ReadAllText(filePath);
                        // xử lý file, thêm vào txtHtml:
                        txtHtml.AppendText(content + Environment.NewLine);
                        x++;

                        if (x == i - 1)
                        {
                            txtHtml.Text = "";
                        }

                        ShowResult(content, Path.GetFileName(filePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi đọc file: {filePath}\n{ex.Message}");
                    }
                }
            }
        }



        //====================== Nút OPEN FILE ==========================

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string html = File.ReadAllText(ofd.FileName);
                    txtHtml.Font = new Font("Consolas", 9, FontStyle.Regular);
                    txtHtml.TextAlign = HorizontalAlignment.Left;
                    txtHtml.Text = html;
                    ShowResult(html, Path.GetFileName(ofd.FileName));
                }
            }
        }


        //====================== Nút CHECK ==========================

        private void btnCheck_Click(object sender, EventArgs e)
        {
            ShowResult(txtHtml.Text, "Nội dung nhập tay");
        }



        //====================== Nút Xóa ==========================


        private void btnClear_Click(object sender, EventArgs e)
        {

            if (lvHistory.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvHistory.SelectedItems[0];

                // Kiểm tra nếu còn item trong danh sách
                if (lvHistory.Items.Count > 0)
                {
                    int index = selectedItem.Index;

                    // Kiểm tra nếu có item trước item đã chọn
                    if (index >= 0 && index < lvHistory.Items.Count - 1)
                    {
                        // Lấy item trước đó và hiển thị nội dung
                        ListViewItem previousItem = lvHistory.Items[index + 1];
                        string previousItemContent = previousItem.SubItems[1].Text;
                        lblOutput.Text = $"⟳ Đang xem lại: [{previousItem.Text}] \n {previousItemContent}";
                        txtHtml.Text = previousItem.Tag as string; // Load lại nội dung của item trước
                    }
                    else
                    {
                        if (index == lvHistory.Items.Count - 1 && lvHistory.Items.Count > 1)
                        {
                            ListViewItem lastItem = lvHistory.Items[index - 1];
                            lblOutput.Text = $"⟳ Đang xem lại: [{lastItem.Text}] \n {lastItem.SubItems[1].Text}";
                            txtHtml.Text = lastItem.Tag as string; // Load lại nội dung của item đầu tiên
                        }
                        else
                        {
                            if (index == 0)
                            {
                                txtHtml.Text = "";
                                txtHtml.TextAlign = HorizontalAlignment.Center;
                                txtHtml.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                                txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
                                lblOutput.ForeColor = Color.LightGray;
                                lblOutput.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                                lblOutput.Text = "Hiển thị kết quả tại đây";
                            }
                            else
                            {
                                // Nếu không có item được chọn, chỉ xóa toàn bộ dữ liệu trong ListView
                                if (lvHistory.Items.Count > 0)
                                {
                                    lvHistory.Items.Clear();
                                    txtHtml.Clear();
                                    txtHtml.TextAlign = HorizontalAlignment.Center;
                                    txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
                                    txtHtml.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                                    lblOutput.ForeColor = Color.LightGray;
                                    lblOutput.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                                    lblOutput.Text = "Hiển thị kết quả tại đây";
                                }
                            }
                        }
                    }
                }
                lblGuide.Text = $"Đã xóa 1 file: [{selectedItem.Text}]";
                lvHistory.Items.Remove(selectedItem);
            }
            else
            {
                // Nếu không có item được chọn, chỉ xóa toàn bộ dữ liệu trong ListView

                lvHistory.Items.Clear();
                txtHtml.Clear();
                txtHtml.TextAlign = HorizontalAlignment.Center;
                txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
                txtHtml.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                lblOutput.ForeColor = Color.LightGray;
                lblOutput.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                lblOutput.Text = "Hiển thị kết quả tại đây";

            }

        }


        // ================= HIỆN KQUẢ ==========================

        private void ShowResult(string htmlContent, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                lblOutput.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblOutput.Text = "⚠ HTML trống. Không có gì để kiểm tra.";
                lblOutput.ForeColor = Color.DarkOrange;
                txtHtml.TextAlign = HorizontalAlignment.Center;
                txtHtml.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
                return;
            }

            string result = HtmlValidator.Validate(htmlContent);

            // Cập nhật lên giao diện
            lblOutput.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblOutput.Text = $"Hiển thị kết quả từ: [{sourceName}] \n {result}";
            lblOutput.ForeColor = result.Contains("❌") ? Color.Red : Color.Green;
            AddToHistory(sourceName, result, htmlContent);
        }

        //====================== ADD TO HISTORY (Của ListView) ==========================


        private void AddToHistory(string fileName, string result, string htmlContent)
        {
            ListViewItem item = new ListViewItem(fileName);
            item.SubItems.Add(result);
            item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
            item.Tag = htmlContent; // Gắn HTML vào Tag
            lvHistory.Items.Add(item);
        }


        // ======================== CUSTOM QUEUE ========================

        public class MyQueue<T>
        {
            private class Node<T>
            {
                public T Data;
                public Node<T>? Next;
                public Node(T data) { Data = data; Next = null; }
            }

            private Node<T>? front, rear;

            public void Enqueue(T item)
            {
                Node<T> newNode = new Node<T>(item);
                if (rear == null)
                    front = rear = newNode;
                else
                {
                    rear.Next = newNode;
                    rear = newNode;
                }
            }

            public T Dequeue()
            {
                if (IsEmpty()) throw new InvalidOperationException("Queue rỗng");
                T item = front.Data;
                front = front.Next;
                if (front == null) rear = null;
                return item;
            }

            public bool IsEmpty() => front == null;

            public void Clear() => front = rear = null;

        }

        // ======================== VALIDATOR ========================
        public static class HtmlValidator
        {
            static string[] voidTags =
            {
               "area", "base", "br", "col", "embed", "hr",
               "img", "input", "link", "meta", "source", "track", "wbr"
            };

            public static string Validate(string html)
            {
                MyQueue<string> queue = new MyQueue<string>();  // Queue dùng để lưu trữ thẻ mở
                MyQueue<string> errors = new MyQueue<string>();  // Queue dùng để lưu trữ lỗi
                MatchCollection matches = Regex.Matches(html, @"<\s*(\/)?\s*([a-zA-Z][a-zA-Z0-9]*)\b([^<>]*)\/?\s*>");
                if (matches.Count == 0)
                {
                    return "❌ Không phát hiện thẻ HTML nào.\n Đây có thể không phải là tài liệu HTML.";
                }
                foreach (Match match in matches)
                {
                    string tag = match.Value.Trim();

                    // Bỏ qua doctype
                    if (tag.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Bỏ qua self-closing tag
                    if (tag.EndsWith("/>"))
                        continue;

                    string tagName = GetTagName(tag).ToLower();
                    if (IsVoidTag(tagName)) continue;

                    // Nếu là thẻ mở
                    if (!tag.StartsWith("</"))
                    {
                        queue.Enqueue(tag);  // Thêm thẻ mở vào Queue

                    }
                    else
                    {
                        // Thẻ đóng
                        string closeTagName = tagName.ToLower().Replace("/", "");
                        bool found = false;
                        
                        MyQueue<string> tempQueue = new MyQueue<string>();

                        // Tìm kiếm thẻ mở tương ứng với thẻ đóng
                        while (!queue.IsEmpty())
                        {
                            string openTag = queue.Dequeue();
                            string openTagName = GetTagName(openTag).ToLower();
                            if (openTagName == closeTagName)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                tempQueue.Enqueue(openTag); // Lưu các thẻ mở chưa tìm thấy
                            }
                        }

                        // Khôi phục Queue (những thẻ mở không khớp)
                        while (!tempQueue.IsEmpty())
                        {
                            queue.Enqueue(tempQueue.Dequeue());
                        }

                        if (!found)
                        {
                            errors.Enqueue($"❌ Lỗi: Thẻ đóng </{tagName}> không có thẻ mở tương ứng.");
                        }
                    }
                }

                // Nếu còn thẻ mở trong Queue, nghĩa là chúng chưa được đóng
                while (!queue.IsEmpty())
                {
                    string unclosedTag = queue.Dequeue();
                    string unclosedName = GetTagName(unclosedTag).ToLower();
                    errors.Enqueue($"❌ Lỗi: Thẻ mở <{unclosedName}> chưa được đóng.");
                }

                // Trả về tất cả lỗi (nếu có)
                if (!errors.IsEmpty())
                {
                    string allErrors = "";
                    while (!errors.IsEmpty())
                    {
                        allErrors += errors.Dequeue() + "\n";
                    }
                    return allErrors.Trim();
                }

                return "✅ HTML hợp lệ.";
            }

            private static string GetTagName(string tag)
            {
                Match m = Regex.Match(tag, @"^<\/?\s*([a-zA-Z0-9]+)");
                return m.Success ? m.Groups[1].Value : "";
            }
            private static bool IsVoidTag(string tagName)
            {
                foreach (var tag in voidTags)
                {
                    if (tag == tagName)
                        return true;
                }
                return false;
            }

        }
    


        // ================= Thay đổi vị trí nhập ======================

        private void txtHtml_Click(object sender, EventArgs e)
        {
            txtHtml.Font = new Font("Consolas", 9, FontStyle.Regular);
            txtHtml.TextAlign = HorizontalAlignment.Left;
        }

        // ================== Select xem lại nội dung trg History ======================

        private void lvHistory_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvHistory.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvHistory.SelectedItems[0];
                string selectedItemPrevCon = selectedItem.SubItems[1].Text;
                string htmlContent = selectedItem.Tag as string;
                if (!string.IsNullOrEmpty(htmlContent))
                {
                    txtHtml.Text = htmlContent;
                    lblOutput.ForeColor = Color.MediumAquamarine;
                    lblOutput.Text = $"⟳ Đang xem lại: [{selectedItem.Text}] \n {selectedItemPrevCon}";
                }
            }
        }

        private void btnCheck_MouseEnter(object sender, EventArgs e)
        {
            lblGuide.Font = new Font("Segoe UI", 9);
            lblGuide.ForeColor = Color.SlateGray;
            lblGuide.Text = "Nhấn vào đây để kiểm tra !";
        }

        private void MouseLeave(object sender, EventArgs e)
        {
            lblGuide.Text = "";
        }

        private void btnClear_MouseEnter(object sender, EventArgs e)
        {
            lblGuide.Font = new Font("Segoe UI", 9);
            lblGuide.ForeColor = Color.SlateGray;
            if (lvHistory.SelectedItems.Count > 0)
            {
                ListViewItem TenFile = lvHistory.SelectedItems[0];
                string name = TenFile.Text;
                lblGuide.Text = $"Bạn có muốn xóa file [{name}] không?";
            }
            else
                
                lblGuide.Text = "Nhấn vào đây để xóa toàn bộ dữ liệu/ Nhấn vào file bạn muốn xóa !";
        }


        private void btnOpen_MouseEnter(object sender, EventArgs e)
        {
            lblGuide.Font = new Font("Segoe UI", 9);
            lblGuide.ForeColor = Color.SlateGray;
            lblGuide.Text = "Nhấn vào đây để mở file !";
        }


        private void lvHistory_MouseEnter(object sender, EventArgs e)
        {
            lblGuide.Font = new Font("Segoe UI", 9);
            lblGuide.ForeColor = Color.SlateGray;
            lblGuide.Text = "Nhấp vào nội dung cũ bạn muốn xem !";
        }

    }
}
