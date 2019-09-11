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

namespace Eren_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private String[] dataFromTxt;
        private String path;

        public class Person
        {
            private int dimens = 10000;
            private String name;
            public int[] x;
            public double[] y;
            public int arrayindex;

            public Person(int index, String allinfo)
            {
                
                x = new int[dimens];
                y = new double[dimens];
                arrayindex = 0;
                char[] b = new char[allinfo.Length];

                using (StringReader sr = new StringReader(allinfo))
                {
                    sr.Read(b, 0, allinfo.Length);
                    int startdata = 0;
                    while (b[startdata] != '/')
                        startdata++;

                    char[] nm = new char[startdata];
                    for (int i = 0; i < startdata; i++)
                        nm[i] = b[i];

                    name = new String(nm); 

                    char[] xdimension = new char[100];
                    char[] ydimension = new char[1000];
                    String variable1;
                    int odd = 0;
                    int numberindex = 0;
                    for (int i = startdata+1; i < allinfo.Length; i++)
                    {
                        
                        if (b[i] != '*')
                        {
                            if (odd == 0)
                            {
                                xdimension[numberindex] = b[i];
                                numberindex++;
                            }
                            else
                            {
                                ydimension[numberindex] = b[i];
                                numberindex++;
                            }
                        }
                        else
                        {
                            if (odd == 0)
                            {
                                odd = 1;
                                variable1 = new String(xdimension);
                                x[arrayindex] = Convert.ToInt32(variable1);
                                Array.Clear(xdimension, 0, xdimension.Length);    

                            }
                            else
                            {
                                odd = 0;
                                variable1 = new String(ydimension);
                                y[arrayindex] = Convert.ToDouble(variable1);
                                Array.Clear(ydimension, 0, ydimension.Length);
                                arrayindex++;
                            }
                            numberindex = 0;
                            
                        }
                        
                    }
                    
                }
                /*Console.WriteLine(name);
                for (int i = 0; i < arrayindex; i++)
                {
                    Console.WriteLine("X - >" + x[i]);
                    Console.WriteLine("Y - >" + y[i]);
                }*/


            }

            public void addGrade(int xpoint, double ypoint)
            {
                x[arrayindex] = xpoint;
                y[arrayindex] = ypoint;
                arrayindex++;
            }
            public void changeName(String nm)
            {
                name = nm;
            }

            public String getName()
            {
                return name;
            }

            public void saveData(String pth)
            {
                String output;
                output = name + "/";
                for (int i = 0; i < arrayindex; i++)
                {
                    output = output + x[i].ToString() + "*";
                    output = output + y[i].ToString() + "*";
                }
                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(output);
                }


            }

        }


        Person[] persons;
        public int numberOfPeople;
        Graphics g;
        Bitmap b;
        public int width, height;
        public int day = 180;
        public int maxscore = 160;
        public double worstdeviation, bestdeviation;
        public double m = 0, n = 0;
        public double m_aim = 0, n_aim = 0;

 
        void refr()
        {
            for (int i = 0; i < 8; i++)
                g.DrawLine(new Pen(Color.Black, 1), 0, 20 * i * height / maxscore, width, 20 * i * height / maxscore);
            for (int i = 0; i < 9; i++)
                g.DrawLine(new Pen(Color.Black, 1),20 * i * width / day, 0, 20 * i * width / day, height);
            picturebox1.Image = b;
        }

        public void loadPoints()
        {
            g.Clear(Color.FromArgb(255, 255, 255));
            int curindex = comboBox1.SelectedIndex;
            for (int i = 0; i < persons[curindex].arrayindex; i++)
            {
                float realcordx = (float)persons[curindex].x[i] * width / day;
                float realcordy = (float)persons[curindex].y[i] * height / maxscore;
                realcordy = height - realcordy;
                g.DrawEllipse(new Pen(Color.Blue, 3), realcordx, realcordy, 4, 4);
                
            }
            refr();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

            path = Directory.GetCurrentDirectory();
            path += "\\data.txt";
            
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path);
                System.Windows.Forms.MessageBox.Show("New File Has Been Created");
            }

            else {

                dataFromTxt = System.IO.File.ReadAllLines(path);

                numberOfPeople = dataFromTxt.Length;
                persons = new Person[numberOfPeople + 1];

                for (int i = 0; i < numberOfPeople; i++)
                {
                    persons[i] = new Person(i, dataFromTxt[i]);
                    comboBox1.Items.Insert(i, persons[i].getName());
                }
                
            }

            picturebox1.BackColor = Color.FromArgb(255, 255, 255);
            b = new Bitmap(picturebox1.Width, picturebox1.Height);
    
            g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            width = picturebox1.Width;
            height = picturebox1.Height;

            comboBox1.SelectedIndex = 0;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Value Has Been Changed -> "+ comboBox1.SelectedIndex);
            loadPoints();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int max=0;
            double yval;
            for (int i = 0; i < persons[comboBox1.SelectedIndex].arrayindex; i++)
            {
                if (max < persons[comboBox1.SelectedIndex].x[i])
                    max = persons[comboBox1.SelectedIndex].x[i];             
            }
            yval = m * max + n;
            m_aim = ((double)numericUpDown4.Value - yval) / ((double)numericUpDown5.Value - max);
            n_aim = yval - m_aim * max;
            textBox6.Text = (m_aim * (double)numericUpDown3.Value + n_aim).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String xpoint, ypoint;
            xpoint = textBox1.Text;
            ypoint = textBox2.Text;
            persons[comboBox1.SelectedIndex].addGrade(Convert.ToInt32(xpoint), Convert.ToDouble(ypoint));
            loadPoints();
            System.Windows.Forms.MessageBox.Show("New exams are added ");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            double k1x = 0, k1y = 0, k2x = 0, kxy = 0;
            int counter = 0;
            Console.WriteLine(numericUpDown2.Value);

            for (int i = 0; i < persons[comboBox1.SelectedIndex].arrayindex; i++)
            {
                if (persons[comboBox1.SelectedIndex].x[i] >= numericUpDown1.Value && persons[comboBox1.SelectedIndex].x[i] <= numericUpDown2.Value)
                {
                    k1x += persons[comboBox1.SelectedIndex].x[i];
                    k2x += persons[comboBox1.SelectedIndex].x[i] * persons[comboBox1.SelectedIndex].x[i];
                    k1y += persons[comboBox1.SelectedIndex].y[i];
                    kxy += persons[comboBox1.SelectedIndex].x[i] * persons[comboBox1.SelectedIndex].y[i];
                    counter++;
                }
            }

            //////////////////////////////////////////////////////	
            

            n = k2x * k1y - k1x * kxy;
            n /= (double)(counter * k2x - k1x * k1x);
            m = kxy - n * k1x;
            m /= (double)(k2x);

            //Console.WriteLine(m +"X + "+n);
            float x1, x2, y1, y2;
            x1 = (float)numericUpDown1.Value * ((float)width / day);
            y1 = (float)(height-((float)numericUpDown1.Value * m + n) * ((float)height / maxscore));
            x2 = (float)numericUpDown2.Value * ((float)width / day);
            y2 = (float)(height-((float)numericUpDown2.Value * m + n) * ((float)height / maxscore));
            loadPoints();
            g.DrawLine(new Pen(Color.Red, 3), x1, y1, x2,y2);
            refr();

            double best=0, worst=0;
            int bestcount=0, worstcount=0;
            for (int i = 0; i < persons[comboBox1.SelectedIndex].arrayindex; i++)
            {
                if (persons[comboBox1.SelectedIndex].x[i] >= numericUpDown1.Value && persons[comboBox1.SelectedIndex].x[i] <= numericUpDown2.Value)
                {
                    if (((float)persons[comboBox1.SelectedIndex].x[i] * m + n) > persons[comboBox1.SelectedIndex].y[i])//Altinda
                    {
                        worst = (persons[comboBox1.SelectedIndex].x[i] * m + n - persons[comboBox1.SelectedIndex].y[i]);
                        worst *= worst;
                        worstdeviation += worst;
                        worstcount++;

                    }
                    else//Ustunde
                    {
                        best = (float)(persons[comboBox1.SelectedIndex].y[i] - persons[comboBox1.SelectedIndex].x[i] * m - n);
                        best *= best;
                        bestdeviation += best;
                        bestcount++;
                    }

                }
            }
            bestdeviation /= persons[comboBox1.SelectedIndex].arrayindex;
            worstdeviation /= persons[comboBox1.SelectedIndex].arrayindex;
            bestdeviation = Math.Sqrt(bestdeviation);
            worstdeviation = Math.Sqrt(worstdeviation);

            textBox3.Text = bestdeviation.ToString();
            textBox4.Text = worstdeviation.ToString();
            textBox5.Text = m.ToString();

            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            File.Create(path).Close();
            for (int i = 0; i < numberOfPeople; i++)
            {
                persons[i].saveData(path);
            }
        }
    }
}
