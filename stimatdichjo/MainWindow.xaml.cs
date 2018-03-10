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
            for (int i = 0; i < count; i++)
            {
                TextboxnyaMathias.AppendText("Nama Mata Kuliah:");
                TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[i].NamaMataKuliah);
                int countlagi = LMataKuliah.ListMatKul[i].PreRequisite.Count;
                if (countlagi > 0)
                {
                    TextboxnyaMathias.AppendText("\n Prerequisite:");
                    for (int j = 0; j < countlagi; j++)
                        TextboxnyaMathias.AppendText(LMataKuliah.ListMatKul[i].PreRequisite[j]);
                } else
                {
                    TextboxnyaMathias.AppendText("\n No Prerequisite!");
                }
                TextboxnyaMathias.AppendText("\n");
            }
        }
    }

    public class MataKuliah
    {
        //Elements : Nama Mata Kuliah and PreRequisites
        public string NamaMataKuliah;
        public List<string> PreRequisite = new List<string>();

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
        public void PrintMataKuliah()
        {
            Console.WriteLine("Nama Mata Kuliah : {0:D}", NamaMataKuliah);
            //if no prerequisite
            if (PreRequisite.Count == 0)
            {
                Console.WriteLine("No PreRequisite !");
            }
            else
            {
                Console.Write("PreRequisite : ");
                foreach (string _MatKul in PreRequisite)
                {
                    Console.Write("{0:D}", _MatKul);
                }
                Console.WriteLine();
            }
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
            //Console.Write(ListMatKul[0].NamaMataKuliah);
            for (int i = 0; i < ListMatKul.Count; i++)
            {
                ListMatKul[i].PrintMataKuliah();
            }
            file.Close();
            // Suspend the screen.  
            System.Console.ReadLine();
        }
    }
}
