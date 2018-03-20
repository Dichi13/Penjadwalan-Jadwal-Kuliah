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
using System.Collections;

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

        private void DFSButton(object sender, RoutedEventArgs e)
        {
            // clears the textbox
            TextboxnyaMathias.Text = String.Empty;

            ListMataKuliah LMataKuliah = new ListMataKuliah();
            try
            {
                LMataKuliah.ReadFile(TextBox.Text);
            }
            catch
            {
                TextboxnyaMathias.AppendText("Berkas tidak dapat ditemukan");
            }

            //Call the DFS method
            LMataKuliah.topologicalSortDFS();
            bool[] printed = new bool[LMataKuliah.ListMatKul.Count];
            //search for each time stamp that hasn't printed
            int count = 1;
            for (int i = LMataKuliah.ListMatKul.Count * 2; i >=1; i--)
            {
                //search in each MatKul
                for (int j = 0; j < LMataKuliah.ListMatKul.Count; j++)
                {
                    if (LMataKuliah.ListMatKul[j].outTimeStamp == i && !printed[j])
                    {
                        //print the elements
                        TextboxnyaMathias.AppendText("Semester " + count + ": ");
                        TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[j].NamaMataKuliah + " (");
                        TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[j].inTimeStamp + "/");
                        TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[j].outTimeStamp + ")\n");
                        count++;
                        break;
                    }
                }
            }

            TampilGraf.Source = convert(LMataKuliah.DrawGraph());

            // force garbage collection
            LMataKuliah = null;
            printed = null;
            GC.Collect();
        }

        private void BFSButton(object sender, RoutedEventArgs e)
        {
            // clears the textbox
            TextboxnyaMathias.Text = String.Empty;

            ListMataKuliah LMataKuliah = new ListMataKuliah();
            try
            {
                LMataKuliah.ReadFile(TextBox.Text);
            } catch
            {
                TextboxnyaMathias.AppendText("Berkas tidak dapat ditemukan");
            }

            //Call the BFS method
            List<List<int>> order = new List<List<int>>();
            order = LMataKuliah.topologicalSortBFS();
            //search for each time stamp that hasn't printed
            for (int i = 0; i < order.Count; i++)
            {
                // print number of semester
                TextboxnyaMathias.AppendText("Semester " + (i + 1) + ": ");
                int count = 0;
                foreach (int j in order[i])
                {
                    //print the elements
                    if (count > 0) TextboxnyaMathias.AppendText(", ");
                    TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[j].NamaMataKuliah);
                    count++;
                }
                TextboxnyaMathias.AppendText("\n");
            }

            LMataKuliah.DrawGraph(order, TampilGraf);

            // force garbage collection
            LMataKuliah = null;
            order = null;
            GC.Collect();
        }

        // Function to convert Bitmap into BitmapSource which will be used to embed graph image
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
        public int inTimeStamp;
        public int outTimeStamp;
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
    }

    class ListMataKuliah
    {
        public int timeStamp = 1;
        //Element : List of MataKuliah
        public List<MataKuliah> ListMatKul = new List<MataKuliah>();
        /** Read file from specific location, example : "c:\fileTubes.txt"
         * 
         * */
        public void ReadFile(string FileName)
        {
            if (FileName != null && FileName.Length != 0 && new FileInfo(FileName).Exists)
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
                ListMatKul = ListMatKul.OrderBy(o => o.PreRequisite.Count).ToList();
                //Add their ID
                AddID();
                AddPostRequisite();
                file.Close();
            } else
            {
                FileNotFoundException dichi = new FileNotFoundException();
                throw dichi;
            }
        }
        public void AddPostRequisite()
        {
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                //for each other matkul
                for (int j = 0; j < ListMatKul.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < ListMatKul[j].PreRequisite.Count; k++)
                        {
                            //if the name is same
                            if (ListMatKul[i].NamaMataKuliah == ListMatKul[j].PreRequisite[k])
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
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                ListMatKul[i].id = i;
            }
        }
        public Bitmap DrawGraph()
        {
            // For DFS graph drawing
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                for (int j = 0; j < ListMatKul[i].PreRequisite.Count; j++)
                {
                    graph.AddEdge(ListMatKul[i].PreRequisite[j] + '\n' + ListMatKul[GetNumberInList(ListMatKul[i].PreRequisite[j])].inTimeStamp + '/' + ListMatKul[GetNumberInList(ListMatKul[i].PreRequisite[j])].outTimeStamp, ListMatKul[i].NamaMataKuliah + '\n' + ListMatKul[i].inTimeStamp + '/' + ListMatKul[i].outTimeStamp);
                }

            }
            
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 1500;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);

            return bitmap;
        }

        public void DrawGraph(List<List<int>> order, System.Windows.Controls.Image Graph)
        {
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

            for (int i = 0; i < ListMatKul.Count; i++)
            {
                for (int j = 0; j < ListMatKul[i].PreRequisite.Count; j++)
                {
                    graph.AddEdge(ListMatKul[i].PreRequisite[j], ListMatKul[i].NamaMataKuliah);
                }
            }

            //Bitmap creation
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 1500;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);

            //Convert and show the image
            BitmapSource image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Graph.Source = image;
            bitmap.Dispose();

            AnimateBFS(order, Graph, graph);
        }

        public async void AnimateBFS(List<List<int>> order, System.Windows.Controls.Image Graph, Microsoft.Msagl.Drawing.Graph graph)
        {
            // Animation handling
            for (int i = 0; i < order.Count; i++)
            {
                // Delay for the animation
                await Task.Delay(1000);
                //Color node for each semester with green
                foreach (int j in order[i])
                {
                    graph.FindNode(ListMatKul[j].NamaMataKuliah).Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                }

                //Bitmap creation
                Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                renderer.CalculateLayout();
                int width = 1500;
                Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                renderer.Render(bitmap);

                //Convert and show the image
                BitmapSource image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                Graph.Source = image;
                bitmap.Dispose();
            }
        }

        //Visit all of the list in the PreRequisites of a node that hasn't visited
        public void topologicalSortRecDFS(int node, bool[] visited)
        {
            if (!visited[node])
            {
                visited[node] = true;
                ListMatKul[node].inTimeStamp = timeStamp++;
            }

            for (int i = 0; i < ListMatKul[node].PostRequisite.Count; i++)
            {
                if (!visited[GetNumberInList(ListMatKul[node].PostRequisite[i])])
                {
                    topologicalSortRecDFS(GetNumberInList(ListMatKul[node].PostRequisite[i]), visited);
                }
            }
            //Give time stamp
            ListMatKul[node].outTimeStamp = timeStamp++;
        }
        //find the number of an NamaMatKuliah, must be in the list
        public int GetNumberInList(string namaMatKul)
        {
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                if (ListMatKul[i].NamaMataKuliah == namaMatKul)
                {
                    return i;
                }
            }
            return 0;
        }
        //Visit all of the nodes in graph
        public void topologicalSortDFS()
        {
            timeStamp = 1;
            bool[] visited = new bool[ListMatKul.Count];
            //Default bool type in C# is False, so don't need to initialize to false
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                if (!visited[i])
                {
                    topologicalSortRecDFS(i, visited);
                }
            }
        }

        public List<List<int>> topologicalSortBFS()
        {
            // Lists degree of each nodes
            int[] derajat = new int[ListMatKul.Count];
            for (int k = 0; k < ListMatKul.Count; k++)
            {
                derajat[k] = ListMatKul[k].PreRequisite.Count;
            }

            // Add nodes with zero degrees to the queue
            LinkedList<int> queue = new LinkedList<int>();
            for (int k = 0; k < ListMatKul.Count; k++)
            {
                if (derajat[k] == 0)
                {
                    queue.AddLast(k);
                }
            }

            // Add sorted nodes into the topOrder list
            List<List<int>> topOrder = new List<List<int>>();
            int i = 0;
            while (queue.Count > 0)
            {
                List<int> temp = new List<int>();
                // Remove the first element of the queue
                while (queue.Count > 0)
                {
                    int top = queue.First.Value;
                    queue.RemoveFirst();
                    temp.Add(top);
                    
                }
                topOrder.Add(temp);

                // Subtract degree of any other nodes that pointed by the removed node by 1
                foreach (int top in topOrder[i])
                    foreach (string simpul in ListMatKul[top].PostRequisite)
                    {
                        if(--derajat[GetNumberInList(simpul)] == 0)
                        {
                            queue.AddLast(GetNumberInList(simpul));
                        }
                    }
                i++;
            }

            return topOrder;
        }
    }
}
