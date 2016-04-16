using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckSTP
{
	public partial class Form2 : Form
	{
		private const string FileDataPath = "RuleData.xml";

		public Form2()
		{
			InitializeComponent();
			ruleSTD.ReadXml( FileDataPath );
			dataGridView1.DataSource = ruleSTD.Tables[0];
			
		}

		private void button1_Click( object sender, EventArgs e )
		{
			ruleSTD.AcceptChanges();
			ruleSTD.WriteXml( FileDataPath );
		}
	}
}
