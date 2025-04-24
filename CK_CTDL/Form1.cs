using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

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
            lvHistory.Columns.Add("Kết quả", 200);
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
                lblGuild.Text = $"Bạn đã kéo thêm vào {i} file !";
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

                        if(x == i - 1)
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
            txtHtml.Clear();
            txtHtml.TextAlign = HorizontalAlignment.Center;
            txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
            txtHtml.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblOutput.ForeColor = Color.LightGray;
            lblOutput.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblOutput.Text = "Hiển thị kết quả tại đây";
            lvHistory.Items.Clear();

        }



        // ================= HIỆN KQUẢ ==========================

        private void ShowResult(string htmlContent, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                lblOutput.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblOutput.Text = "⚠ HTML trống. Không có gì để kiểm tra.";
                lblOutput.ForeColor = Color.DarkOrange;
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
            // Danh sách các void tags theo chuẩn HTML5
            static string[] voidTags = 
            {
              "area", "base", "br", "col", "embed", "hr",
              "img", "input", "link", "meta", "source", "track", "wbr"
            };

            public static string Validate(string html)
            {
                MyQueue<string> queue = new MyQueue<string>();
                MatchCollection matches = Regex.Matches(html, @"<\/?[a-zA-Z][a-zA-Z0-9]*[^>]*>");

                foreach (Match match in matches)
                {
                    queue.Enqueue(match.Value);
                }

                // Nếu không có thẻ nào thì không hợp lệ
                if (queue.IsEmpty())
                {
                    return "❌ Không phát hiện thẻ HTML nào.\n Đây có thể không phải là tài liệu HTML.";
                }

                MyQueue<string> temp = new MyQueue<string>();

                while (!queue.IsEmpty())
                {
                    string tag = queue.Dequeue().Trim();

                    // Bỏ qua doctype
                    if (tag.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Bỏ qua self-closing tag dạng <... />
                    if (tag.EndsWith("/>")) continue;

                    // Lấy tên thẻ
                    string tagName = GetTagName(tag).ToLower();

                    // Nếu là void tag thì không cần đóng
                    if (IsVoidTag(tagName)) continue;

                    // Nếu là thẻ mở
                    if (!tag.StartsWith("</"))
                    {
                        temp.Enqueue(tag);
                    }
                    else
                    {
                        // Là thẻ đóng
                        bool found = false;
                        MyQueue<string> backup = new MyQueue<string>();

                        while (!temp.IsEmpty())
                        {
                            string openTag = temp.Dequeue();
                            string openName = GetTagName(openTag).ToLower();

                            if (openName == tagName)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                backup.Enqueue(openTag);
                            }
                        }

                        // Khôi phục temp
                        while (!backup.IsEmpty())
                        {
                            temp.Enqueue(backup.Dequeue());
                        }

                        if (!found)
                        {
                            return $"❌ Lỗi: Thẻ đóng </{tagName}> không có thẻ mở tương ứng.";
                        }
                    }
                }

                if (!temp.IsEmpty())
                {
                    return "❌ Lỗi: Có thẻ mở chưa được đóng.";
                }

                return "✅ HTML hợp lệ.";
            }

            // Hàm lấy tên thẻ từ tag <p class="x"> → p
            private static string GetTagName(string tag)
            {
                Match m = Regex.Match(tag, @"^<\/?\s*([a-zA-Z0-9]+)");
                return m.Success ? m.Groups[1].Value : "";
            }

            // Kiểm tra có phải void tag không
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
            lblGuild.Font = new Font("Segoe UI", 9);
            lblGuild.ForeColor = Color.SlateGray;
            lblGuild.Text = "Nhấn vào đây để kiểm tra !";
        }

        private void MouseLeave(object sender, EventArgs e)
        {
            lblGuild.Text = "";
        }

        private void btnClear_MouseEnter(object sender, EventArgs e)
        {
            lblGuild.Font = new Font("Segoe UI", 9);
            lblGuild.ForeColor = Color.SlateGray;
            lblGuild.Text = "Nhấn vào đây để xóa toàn bộ dữ liệu !";
        }


        private void btnOpen_MouseEnter(object sender, EventArgs e)
        {
            lblGuild.Font = new Font("Segoe UI", 9);
            lblGuild.ForeColor = Color.SlateGray;
            lblGuild.Text = "Nhấn vào đây để mở file !";
        }

       
        private void lvHistory_MouseEnter(object sender, EventArgs e)
        {
            lblGuild.Font = new Font("Segoe UI", 9);
            lblGuild.ForeColor = Color.SlateGray;
            lblGuild.Text = "Nhấp vào nội dung cũ bạn muốn xem !";
        }

       

    }
}
