﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Empl empl = new Empl();
            empl.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Suppl suppl = new Suppl();
            suppl.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Equip equip = new Equip();
            equip.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MaintenShed maintenShed = new MaintenShed();
            maintenShed.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Repa repa = new Repa();
            repa.ShowDialog();
        }
    }
}
