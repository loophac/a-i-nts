using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aints
{
	public partial class ConstantsForm : Form
	{
		private ConstantsHolder data;

		public ConstantsForm()
		{
			InitializeComponent();

			this.data = ConstantsHolder.Singleton;
			this.propertyGrid.SelectedObject = this.data;
		}

		/// <summary>
		/// saves the state in the xml file each time a property is modified
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			data.SaveXml();
			//refresh, needed if some properties are dynamically calculated
			//this.propertyGrid.SelectedObject = data;
		}
	}
}
