﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ChapterFour4_2
{
    public partial class frmBooks : Form
    {
        SqlConnection booksConnection;
        string SQLAll;
        Button[] btnRolodex = new Button[26];
        
        public frmBooks()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string pathName = Path.GetFullPath("SQLBooksDB.mdf");
            MessageBox.Show(pathName);

            // connect to the books database
            booksConnection = new SqlConnection($@"Data Source=.\SQLEXPRESS; AttachDbFileName={pathName}; Integrated Security=True; Connect Timeout=10; User Instance=True");
            /*booksConnection = new SqlConnection($@"Data Source=.\SQLEXPRESS; 
                                AttachDbFileName=C:\Users\mholmes022726\source\repos\ChapterFour4-2\ChapterFour4-2\bin\Debug\netcoreapp3.1\SQLBooksDB.mdf; 
                                Integrated Security=True; Connect Timeout=10; User Instance=True");*/

            booksConnection.Open();

            int w, lStart, l, t;
            int buttonHeight = 33; // found by trial and error

            // search buttons
            // determine button width - 13 on a row
            w = Convert.ToInt32(this.ClientSize.Width / 14);

            // center buttons on form 
            lStart = Convert.ToInt32(0.5 * (this.ClientSize.Width - 13 * w));
            l = lStart;
            t = grdBooks.Top + grdBooks.Height + 2;

            // create and position 26 buttons
            for (int i = 0; i < 26; i++) 
            {
                // create new push button 
                btnRolodex[i] = new Button();
                btnRolodex[i].TabStop = false;
                // set text property 
                btnRolodex[i].Text = ((char)(65 + i)).ToString();
                btnRolodex[i].Width = w;
                btnRolodex[i].Height = buttonHeight;
                btnRolodex[i].Left = 1;
                btnRolodex[i].Top = t;
                // give cool colors 
                btnRolodex[i].BackColor = Color.Blue;
                btnRolodex[i].ForeColor = Color.White;
                // add button to form 
                btnRolodex[i].Click += new System.EventHandler(this.btnSQL_Click);
                // next left 
                l += w;
                if (i == 12) 
                {
                    // move to next row
                    l = lStart;
                    t += buttonHeight; 
                }
            }
            // build basic SQL statement 
            SQLAll = "SELECT Authors.Author, Titles.Title, Publishers.Company_Name ";
            SQLAll += "FROM Authors, Titles, Publishers, Title_Author ";
            SQLAll += "WHERE Titles.ISBN = Title_Author.ISBN ";
            SQLAll += "AND Authords.Au_ID = Title_Author.Au_ID ";
            SQLAll += "AND Titles.PubID = Publishers.PubID ";
            // show form and click on all records initially 
            btnAll.PerformClick(); 
        }

        private void frmBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            booksConnection.Close();
            booksConnection.Dispose(); 
        }

        private void btnSQL_Click(object sender, EventArgs e) 
        {
            SqlCommand resultsCommand = null;
            SqlDataAdapter resultsAdapter = new SqlDataAdapter();
            DataTable resultsTable = new DataTable();
            string SQLStatment;
            // determine which button was clicked and form SQL statement
            Button buttonClicked = (Button)sender;
            switch (buttonClicked.Text) 
            {
                case "Show All Records":
                    SQLStatment = SQLAll;
                    break;
                case "Z":
                    // Z clicked
                    // appaend to SQLAll to limit records to Z Authors
                    SQLStatment = SQLAll + "AND Authords.Author > 'Z' ";
                    break;
                default:
                    // letter key other than z clicked 
                    // append to SQLAll to limit records to letter clicked 
                    int index = (int)(Convert.ToChar(buttonClicked.Text)) - 65;
                    SQLStatment = SQLAll + "AND Authors.Author > ' " + btnRolodex[index].Text + " ' ";
                    SQLStatment += "AND Authors.Author < '" + btnRolodex[index = 1].Text + " ' ";
                    break; 
            }
            SQLStatment += "ORDER BY Authors.Author";
            // apply SQL statment 
            try
            {
                // establish command object and data adapter 
                resultsCommand = new SqlCommand(SQLStatment, booksConnection);
                resultsAdapter.SelectCommand = resultsCommand;
                resultsAdapter.Fill(resultsTable);
            }
            catch(Exception ex) // changed from the book
            {
                MessageBox.Show(ex.Message, "Error in Processing SQL", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
            resultsCommand.Dispose();
            resultsAdapter.Dispose();
            resultsTable.Dispose(); 
        }
    }
}
