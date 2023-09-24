using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Opak3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        StreamReader sr;

        int Vek(string rc, ref string mesicSlovne)
        {
            try
            {
                int rok,mesic,den;
                if (int.Parse(rc[0].ToString() + rc[1]) > 23)
                    rok = 1900 + int.Parse(rc[0].ToString() + rc[1]);
                else 
                    rok = 2000 + int.Parse(rc[0].ToString() + rc[1]);
                if (int.Parse(rc[2].ToString()) > 1)
                    mesic = int.Parse(rc[2].ToString() + rc[3]) - int.Parse(rc[2].ToString()) * 10;
                else
                    mesic = int.Parse(rc[2].ToString() + rc[3]);
                den = int.Parse(rc[4].ToString() + rc[5]);
                TimeSpan ts = DateTime.Now - DateTime.Parse(den + "." + mesic + "." + rok);
                switch (mesic)
                {
                    case 1:
                        mesicSlovne = "Leden";
                        break;
                    case 2:
                        mesicSlovne = "Únor";
                        break;
                    case 3:
                        mesicSlovne = "Březen";
                        break;
                    case 4:
                        mesicSlovne = "Duben";
                        break;
                    case 5:
                        mesicSlovne = "Květen";
                        break;
                    case 6:
                        mesicSlovne = "Červen";
                        break;
                    case 7:
                        mesicSlovne = "Červenec";
                        break;
                    case 8:
                        mesicSlovne = "Srpen";
                        break;
                    case 9:
                        mesicSlovne = "Září";
                        break;
                    case 10:
                        mesicSlovne = "Říjen";
                        break;
                    case 11:
                        mesicSlovne = "Listopad";
                        break;
                    case 12:
                        mesicSlovne = "Prosinec";
                        break;
                }
                return (int)(ts.Days/365.25);
            }
            catch
            {
                return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("rodna_cis.txt");
            List<string> jmena = new List<string>();
            List<int> znamka = new List<int>();
            List<string> rc = new List<string>();
            try
            {
                while (!sr.EndOfStream)
                {
                    string r = sr.ReadLine();
                    listBox1.Items.Add(r);
                    string[] s = r.Split(';');
                    jmena.Add(s[0]);
                    znamka.Add(int.Parse(s[1]));
                    rc.Add(s[2]);
                }
            }
            catch
            {
                MessageBox.Show("Chyba v souboru!!");
            }
            sr.Close();

            StreamWriter sw = new StreamWriter("rodna_cis.txt", false);
            string prvniRC = "";
            int index = 0;
            foreach (string s in rc)
            {
                string mes = "";
                int vek = Vek(s, ref mes);
                if (mes == "Prosinec")
                    prvniRC = s;
                sw.WriteLine(jmena[index] + ";" + znamka[index]+";"+s+";"+vek);
                index++;
            }
            if (prvniRC != "")
            {
                MessageBox.Show("Člověk narozen v prosinci: " + jmena[rc.IndexOf(prvniRC)]);
            }
            sw.WriteLine(znamka.Average());
            sw.Close();
            sr = new StreamReader("rodna_cis.txt");
            while (!sr.EndOfStream)
            {
                string r = sr.ReadLine();
                listBox2.Items.Add(r);
            }
            sr.Close();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string file = "";
            while (file == "")
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = saveFileDialog.FileName;
                }
            }

            sw = new StreamWriter(file, false);
            index = 0;
            foreach (string s in rc)
            {
                string mes = "";
                int vek = Vek(s, ref mes);
                if (znamka[index]<3)
                    sw.WriteLine(jmena[index] + ";" + vek + ";" + mes);
                index++;
            }
            sw.Close();

            sr = new StreamReader(file);
            while (!sr.EndOfStream)
            {
                string r = sr.ReadLine();
                listBox3.Items.Add(r);
            }
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            string file = "";
            while (file == "")
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = openFileDialog.FileName;
                }
            }

            StreamReader sr = new StreamReader(file);
            FileStream fs = new FileStream("cisla.dat", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            
            while (!sr.EndOfStream)
            {
                string r = sr.ReadLine();
                listBox1.Items.Add(r);
                string[] s = r.Split(';');
                int max = int.MinValue;
                foreach(string s2 in s)
                {
                    if(s2.Length > max)
                        max = s2.Length;
                }
                bw.Write(Math.Round((double)max/10,1));
            }
            bw.Close();
            sr.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("cisla.dat", FileMode.Open);
            BinaryReader bw = new BinaryReader(fs);

            listBox4.Items.Clear();
            while(bw.BaseStream.Position < bw.BaseStream.Length)
            {
                listBox4.Items.Add(bw.ReadDouble());
            }
            bw.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("cisla.dat", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            List<double> l = new List<double>();
            List<double> l2 = new List<double>();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                l.Add(br.ReadDouble());
            }
            br.Close();

            foreach(double d in l)
            {
                if (d < 1)
                    l2.Add(d * 10);
                else
                    l2.Add(d);
            }

            fs = new FileStream("cisla.dat", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            foreach(double d in l2)
            {
                bw.Write(d);
            }
            bw.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("cisla.dat", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            List<double> l = new List<double>();
            double soucet = 0, pocet = 0; ;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                l.Add(br.ReadDouble());
            }
            br.Close();

            foreach (double d in l)
            {
                if (d > 2)
                {
                    soucet+=d;
                    pocet++;
                }
            }

            fs = new FileStream("cisla.dat", FileMode.Append);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(soucet/pocet);
            bw.Close();
        }
    }
}
