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

namespace TextEditor
{
    public partial class Form1 : Form
    {
        int LineLenght = 44;
        int SymInd = 0;
        int LastSymInd = 0;
        string OpenText;
        bool newLineFlagL = false;
        bool newLineFlagR = false;
        bool isCaretMove = false;
        List<int> SymCount = new List<int>();

        int index = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void DebugInfo()
        {
            int sum = 0;
            index = label1.Text.IndexOf("│");
            label2.Text = index.ToString();
            label2.Text = label2.Text + "\nстрока = " + SymInd.ToString();
            label2.Text = label2.Text + "\nласт строка = " + LastSymInd.ToString();
            for (int i = 0; i < SymCount.Count; i++)
            {
                sum += SymCount[i];
            }
            label2.Text = label2.Text + " \nSymCount: = " + sum.ToString();
            label2.Text = label2.Text + " \nLabel1.Length: = " + label1.Text.Length.ToString();
            label2.Text = label2.Text + " \nnewLineFlagR: = " + newLineFlagR.ToString();
            label2.Text = label2.Text + " \nnewLineFlagR: = " + newLineFlagR.ToString();
            label2.Text = label2.Text + " \niscaretmove: = " + isCaretMove.ToString();//если \n и флаг тру то фалс
        }

        private void removeSymbol()
        {
            index = label1.Text.IndexOf("│");
            if (label1.Text.Length > 1)
            {
                label1.Text = label1.Text.Remove(index - 1, 1);
                SymCount[SymInd]--;//символьной строки
                if (SymCount[SymInd] == 0)
                {
                    SymInd--;
                    LastSymInd--;
                }
                DebugInfo();
            }
        }
        private void DeleteFun()
        {
            index = label1.Text.IndexOf("│");
            if ((label1.Text.Length > 1) && (index < label1.Text.Length - 1))
            {
                label1.Text = label1.Text.Remove(index + 1, 1);
                SymCount[SymInd]--;//символьной строки

            }
        }


        private void correctCursor()
        {
            index = label1.Text.IndexOf("│");
            if (label1.Text.Length > 0)
            {
                label1.Text = label1.Text.Remove(index, 1);
            }
        }

        private void newLineCount()
        {
            index = label1.Text.IndexOf("│");
            if (index == (label1.Text.Length - 1))
            {
                SymInd++;
            }
            else if ((SymInd + 1) * LineLenght == index)
            {
                SymInd++;//менять только при подходе к \n
            }
            LastSymInd++;
            SymCount.Insert(LastSymInd, 0);
        }

        private void NewLineShift()
        {
            int corrSymInd = SymInd + 1;

            for (int i = corrSymInd; i < SymCount.Count; i++)
            {
                label1.Text = label1.Text.Remove(LineLenght * i, 1);
                label1.Text = label1.Text.Insert(LineLenght * i - 1, "\n");
            }
        }


        private void NewLineDebugger()
        {
            index = label1.Text.IndexOf("│");
            if (SymCount[LastSymInd] > LineLenght - 1)//если ласт строка переполнилась
            {
                index = label1.Text.IndexOf("│");
                //каретка в конце текста
                if (index == label1.Text.Length - 1)
                {
                    newLineCount();
                    correctCursor();
                    label1.Text = label1.Text.Insert(index, "\n");
                    SymCount[LastSymInd]++;
                    label1.Text = label1.Text.Insert(index + 1, "│");
                    index = label1.Text.IndexOf("│");
                }
                //каретка в середине текста
                else
                {
                    NewLineShift();
                    newLineCount();
                    correctCursor();
                    label1.Text = label1.Text.Insert(label1.Text.Length - 1, "\n");
                    SymCount[LastSymInd]++;
                    label1.Text = label1.Text.Insert(index, "│");
                    index = label1.Text.IndexOf("│");
                }
            }
            //переполнения нет, просто смещаем \n
            else if ((SymCount[LastSymInd] <= LineLenght - 1) && (SymInd < LastSymInd))
            {
                NewLineShift();
                index = label1.Text.IndexOf("│");
                //при добавлении символов перешли на новую строчку
                if ((index > 0) && (label1.Text[index - 1] == '\n') && (SymCount[LastSymInd] > 1)
                    && (isCaretMove == false))
                    SymInd++;
                DebugInfo();
            }


        }

        private void MyPainCaretFlag()
        {
            index = label1.Text.IndexOf("│");
            if ((index) % LineLenght == 0)
                isCaretMove = true;
            else
                isCaretMove = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SymCount.Insert(0, 1);//учитываем курсор
            label1.Font = new Font(FontFamily.GenericMonospace, label1.Font.Size);
            label1.Text = "│";
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

            //переход на новую строчку
            if (e.KeyChar != 8)
                NewLineDebugger();

            switch (e.KeyChar)
            {
                case (char)Keys.Back:
                    {
                        removeSymbol();
                        break;
                    }

                default:
                    {
                        if (e.KeyChar != 13)
                        {
                            isCaretMove = false;
                            correctCursor();
                            label1.Text = label1.Text.Insert(index, e.KeyChar.ToString());
                            label1.Text = label1.Text.Insert(index + 1, "│");
                            SymCount[LastSymInd]++;//подсчёт символьной строки(добавление всегда в последнюю)
                            newLineFlagL = false;
                            newLineFlagR = false;
                            //отладка
                            DebugInfo();
                        }
                        break;
                    }

            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {


            switch (e.KeyCode)
            {

                case Keys.Left: //сдвиг курсора влево
                    {
                        index = label1.Text.IndexOf("│");
                        if (index > 0)
                        {
                            label1.Text = label1.Text.Remove(index, 1);

                            label1.Text = label1.Text.Insert(index - 1, "│");
                        }
                        index = label1.Text.IndexOf("│");
                        MyPainCaretFlag();
                        if (index > 1)
                        {
                            if (label1.Text[index - 1] == '\n')//двигаясь влево видим \n -> ставим флаг
                            {
                                newLineFlagL = true;
                                DebugInfo();
                                break;
                            }
                            else if (label1.Text[index + 1] == '\n')//если нажали влево, а \n уже позади, значит уже меняем строчку
                            {
                                SymInd--;
                                newLineFlagL = false;
                            }
                        }
                        if ((newLineFlagL == true) && (SymInd > 0))
                        {
                            SymInd--;
                            newLineFlagL = false;
                        }
                        //отладка
                        DebugInfo();
                        break;
                    }
                case Keys.Right://сдвиг курсора вправо
                    {
                        index = label1.Text.IndexOf("│");
                        if (label1.Text.Length - 1 > index)
                        {
                            label1.Text = label1.Text.Remove(index, 1);

                            label1.Text = label1.Text.Insert(index + 1, "│");
                        }
                        index = label1.Text.IndexOf("│");
                        MyPainCaretFlag();
                        if (index < label1.Text.Length - 1)
                        {
                            if (label1.Text[index + 1] == '\n')
                            {
                                newLineFlagR = true;
                                DebugInfo();
                                break;
                            }
                            else if (label1.Text[index - 1] == '\n')
                            {
                                SymInd++;
                                newLineFlagR = false;
                            }
                        }
                        if (newLineFlagR == true)
                        {
                            SymInd++;
                            newLineFlagR = false;
                        }
                        //отладка
                        DebugInfo();
                        break;
                    }
                case Keys.Up:
                    {
                        index = label1.Text.IndexOf("│");
                        if (SymInd > 0)
                        {
                            int tempIndex = index;
                            for (int i = 0; i < SymInd; i++)
                            {
                                tempIndex -= SymCount[i];
                            }
                            if (tempIndex > SymCount[SymInd - 1])
                            {
                                label1.Text = label1.Text.Remove(index, 1);
                                index = index - tempIndex - 1;
                                label1.Text = label1.Text.Insert(index, "│");
                            }
                            else
                            {
                                label1.Text = label1.Text.Remove(index, 1);
                                index = index - SymCount[SymInd - 1];
                                label1.Text = label1.Text.Insert(index, "│");
                            }
                            newLineFlagL = false;
                            newLineFlagR = false;
                            SymInd--;
                        }
                        MyPainCaretFlag();
                        //отладка
                        DebugInfo();

                        break;
                    }
                case Keys.Down:
                    {
                        index = label1.Text.IndexOf("│");

                        if (SymInd < SymCount.Count - 1)
                        {
                            int tempIndex = index;
                            for (int i = 0; i < SymInd; i++)
                            {
                                tempIndex -= SymCount[i];
                            }
                            if (tempIndex >= SymCount[SymInd + 1])//локальный индекс текущий строки больше размера следующей строки
                            {
                                label1.Text = label1.Text.Remove(index, 1);
                                index = index + SymCount[SymInd] - tempIndex + SymCount[SymInd + 1] - 1;
                                label1.Text = label1.Text.Insert(index, "│");
                            }
                            else
                            {
                                label1.Text = label1.Text.Remove(index, 1);
                                index = index + SymCount[SymInd];
                                label1.Text = label1.Text.Insert(index, "│");
                            }
                            newLineFlagL = false;
                            newLineFlagR = false;
                            SymInd++;
                        }
                        MyPainCaretFlag();
                        //отладка
                        DebugInfo();

                        break;
                    }
                case Keys.Home:
                    {
                        correctCursor();
                        if (index < LineLenght)
                            index = 0;
                        else
                            index = LineLenght * SymInd;
                        label1.Text = label1.Text.Insert(index, "│");
                        DebugInfo();
                        break;
                    }
                case Keys.End:
                    {
                        correctCursor();
                        if (index < LineLenght)
                            index = LineLenght - 1;
                        else
                            index = LineLenght * SymInd + SymCount[SymInd] - 1;
                        label1.Text = label1.Text.Insert(index, "│");
                        DebugInfo();
                        break;
                    }
                case Keys.Delete:
                    {
                        DeleteFun();
                        DebugInfo();
                        break;
                    }

                default:
                    {

                        break;
                    }

            }

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "My open file dialog";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    OpenText = sr.ReadToEnd();
                    sr.Close();
                }
            }
            for (int i = 0; i < OpenText.Length; i++)
            {
                if ((OpenText[i] == '\n') || (OpenText[i] == '\r'))
                {
                    OpenText = OpenText.Remove(i, 1);
                    i--;
                }
            }
            int InputIndex = 0;
            while (OpenText.Length != InputIndex)
            {
                NewLineDebugger();
                label1.Text = label1.Text.Insert(index, OpenText[InputIndex].ToString());
                InputIndex++;
                SymCount[LastSymInd]++;//подсчёт символьной строки(добавление всегда в последнюю)
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Title = "Save file as..";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter txtoutput = new StreamWriter(savefile.FileName + ".txt");
                txtoutput.Write(label1.Text);
                txtoutput.Close();
            }
        }
    }
}
