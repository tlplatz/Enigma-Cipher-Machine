using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        List<Label> alphaLeftList = new List<Label>();
        List<Label> alphaRightList = new List<Label>();
        List<Label> plugList = new List<Label>();

        public Form1()
        {
            InitializeComponent();

            for(int row = 0; row<13; row++)
            {
                for(int col = 0; col<2; col++)
                {
                    int index = row + col * 2;

                    if (index > 0)
                    {
                        Label newLabel = new Label { Text = ALPHABET[index].ToString(), Font = alphaLeft.Font };

                    }
                }
            }
        }



    }
}
