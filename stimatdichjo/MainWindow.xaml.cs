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
            ListMataKuliah LMataKuliah = new ListMataKuliah();
            LMataKuliah.ReadFile(TextBox.Text);

            // clears the textbox
            TextboxnyaMathias.Text = String.Empty;
            
            /* Function for debugging
            foreach (MataKuliah MK in LMataKuliah.ListMatKul)
            {
                TextboxnyaMathias.AppendText(MK.PrintMataKuliah());
            }*/

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
        }

        private void BFSButton(object sender, RoutedEventArgs e)
        {
            ListMataKuliah LMataKuliah = new ListMataKuliah();
            LMataKuliah.ReadFile(TextBox.Text);

            // clears the textbox
            TextboxnyaMathias.Text = String.Empty;

            /* Function for debugging
            foreach (MataKuliah MK in LMataKuliah.ListMatKul)
            {
                TextboxnyaMathias.AppendText(MK.PrintMataKuliah());
            }*/

            //Call the BFS method
            List<int> order = new List<int>();
            order = LMataKuliah.topologicalSortBFS();
            //search for each time stamp that hasn't printed
            foreach (int i in order)
            {
                //print the elements
                TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[i].NamaMataKuliah + '\n');
            }

            TampilGraf.Source = convert(LMataKuliah.DrawGraph());
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
        // Print Nama Mata Kuliah and its Prerequisites
        public string PrintMataKuliah()
        {
            string stringToReturn;
            stringToReturn = "Nama Mata Kuliah : " + NamaMataKuliah + '\n';
            stringToReturn += "Nomor ID : " + id + '\n';
            //if no prerequisite
            if (PreRequisite.Count == 0)
            {
                stringToReturn += "No PreRequisite !\n";
            }
            else
            {
                stringToReturn += "PreRequisite :";
                foreach (string _MatKul in PreRequisite)
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
        public int timeStamp = 1;
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
            ListMatKul = ListMatKul.OrderBy(o => o.PreRequisite.Count).ToList();
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
            int width = 1500;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);

            return bitmap;
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

        public List<int> topologicalSortBFS()
        {
            // Lists degree of each nodes
            int[] derajat = new int[ListMatKul.Count];
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                derajat[i] = ListMatKul[i].PreRequisite.Count;
            }

            // Add nodes with zero degrees to the queue
            LinkedList<int> queue = new LinkedList<int>();
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                if (derajat[i] == 0)
                {
                    queue.AddLast(i);
                }
            }

            // Add sorted nodes into the topOrder list
            List<int> topOrder = new List<int>();
            while (queue.Count > 0)
            {   
                // Remove the first element of the queue
                int top = queue.First.Value;
                queue.RemoveFirst();
                topOrder.Add(top);

                // Subtract degree of any other nodes that pointed by the removed node by 1
                foreach (string simpul in ListMatKul[top].PostRequisite)
                {
                    if(--derajat[GetNumberInList(simpul)] == 0)
                    {
                        queue.AddLast(GetNumberInList(simpul));
                    }
                }
            }

            return topOrder;
        }
    }
}
