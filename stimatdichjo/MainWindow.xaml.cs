using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;

namespace stimatdichjo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImporFileButton(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dial = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".txt",
                Filter = "Berkas TXT|*.txt"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dial.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dial.FileName;
                TextBox.Text = filename;
            }
        }

        private void TampilkeunButton(object sender, RoutedEventArgs e)
        {
            ListMataKuliah LMataKuliah = new ListMataKuliah();
            LMataKuliah.ReadFile(TextBox.Text);

            int count = LMataKuliah.ListMatKul.Count;
            foreach(MataKuliah MK in LMataKuliah.ListMatKul)
            {
                TextboxnyaMathias.AppendText(MK.PrintMataKuliah());
            }
            TampilGraf.Source = convert(LMataKuliah.DrawGraph());
        }

        private BitmapSource convert(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return i;
        }
    }

    public class MataKuliah
    {
        //Elements : Nama Mata Kuliah and PreRequisites
        public string NamaMataKuliah;
        public List<string> PreRequisite = new List<string>();
        public List<string> PostRequisite = new List<string>();
        public int id;
        public MataKuliah()
        {

        }
        /** User defined constructor
         * @param : Nama Mata Kuliah, List of Prerequisite */
        public MataKuliah(string _NamaMatKul, List<string> _PreReq)
        {
            NamaMataKuliah = _NamaMatKul;
            PreRequisite = _PreReq;
        }
        // Print Nama Mata Kuliah and its Prerequisites
        public string PrintMataKuliah()
        {
            string stringToReturn;
            stringToReturn = "Nama Mata Kuliah : " + NamaMataKuliah +'\n';
            stringToReturn += "Nomor ID : " + id +'\n';
            //if no prerequisite
            if (PreRequisite.Count == 0)
            {
                stringToReturn += "No PreRequisite !\n";
            }
            else
            {
                stringToReturn += "PreRequisite :";
                foreach(string _MatKul in PreRequisite)
                {
                    stringToReturn += " " + _MatKul;
                }
                stringToReturn += '\n';
            }
            if (PostRequisite.Count == 0)
            {
                stringToReturn += "No PostRequisite !\n";
            }
            else
            {
                stringToReturn += "PostRequisite :";
                foreach (string _MatKul in PostRequisite)
                {
                    stringToReturn += " " + _MatKul;
                }
                stringToReturn += '\n';
            }
            return stringToReturn;
        }
        
    }
    class ListMataKuliah
    {
        //Element : List of MataKuliah
        public List<MataKuliah> ListMatKul = new List<MataKuliah>();
        /** Read file from specific location, example : "c:\fileTubes.txt"
         * 
         * */
        public void ReadFile(string FileName)
        {
            StreamReader file = new StreamReader(FileName);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                //System.Console.WriteLine(line);
                line = line.Substring(0, line.Length - 1);
                string[] arrMatKul = line.Split(',');
                for (int i = 0; i < arrMatKul.Length; i++)
                {
                    arrMatKul[i] = arrMatKul[i].Replace(" ", string.Empty);
                }
                MataKuliah MK = new MataKuliah();
                MK.NamaMataKuliah = arrMatKul[0];
                if (arrMatKul.Length > 1)
                {
                    for (int i = 1; i < arrMatKul.Length; i++)
                    {
                        MK.PreRequisite.Add(arrMatKul[i]);
                    }
                }
                ListMatKul.Add(MK);
            }
            //Sort the ListMatKul by its NamaMataKuliah
            ListMatKul = ListMatKul.OrderBy(o => o.NamaMataKuliah).ToList();
            //Add their ID
            AddID();
            AddPostRequisite();
            //Console.Write(ListMatKul[0].NamaMataKuliah);
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                ListMatKul[i].PrintMataKuliah();
            }
            file.Close();
            
            // Suspend the screen.  
            System.Console.ReadLine();
        }
        public void AddPostRequisite()
        {

            
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                //for each other matkul
                for(int j = 0; j < ListMatKul.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    else
                    {
                        for(int k = 0; k < ListMatKul[j].PreRequisite.Count; k++)
                        {
                            //if the name is same
                            if(ListMatKul[i].NamaMataKuliah == ListMatKul[j].PreRequisite[k])
                            {
                                ListMatKul[i].PostRequisite.Add(ListMatKul[j].NamaMataKuliah);
                            }
                        }
                    }
                }
            }
        }

        public void AddID()
        {
            //The List has already sorted
            for(int i = 0; i < ListMatKul.Count; i++)
            {
                ListMatKul[i].id = i;
            }
        }
        public Bitmap DrawGraph()
        {
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                for (int j = 0; j < ListMatKul[i].PreRequisite.Count; j++)
                {
                    graph.AddEdge(ListMatKul[i].PreRequisite[j], ListMatKul[i].NamaMataKuliah);
                }

            }

            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 150;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);

            return bitmap;
        }
    }
}
