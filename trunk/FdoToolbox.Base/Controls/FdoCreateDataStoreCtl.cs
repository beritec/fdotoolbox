using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Connections;

namespace FdoToolbox.Base.Controls
{
    public partial class FdoCreateDataStoreCtl : UserControl, IViewContent, IFdoCreateDataStoreView
    {
        private FdoCreateDataStorePresenter _presenter;

        public FdoCreateDataStoreCtl()
        {
            InitializeComponent();
            _presenter = new FdoCreateDataStorePresenter(this, ServiceManager.Instance.GetService<FdoConnectionManager>());
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.LoadProviders();
            _presenter.ProviderChanged();
            base.OnLoad(e);
        }

        public void InitializeConnectGrid()
        {
            grdConnectionProperties.Rows.Clear();
            grdConnectionProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdConnectionProperties.Columns.Add(colName);
            grdConnectionProperties.Columns.Add(colValue);
        }

        public void InitializeDataStoreGrid()
        {
            grdDataStoreProperties.Rows.Clear();
            grdDataStoreProperties.Columns.Clear();
            DataGridViewColumn colName = new DataGridViewColumn();
            colName.Name = "COL_NAME";
            colName.HeaderText = "Name";
            colName.ReadOnly = true;
            DataGridViewColumn colValue = new DataGridViewColumn();
            colValue.Name = "COL_VALUE";
            colValue.HeaderText = "Value";

            colValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdDataStoreProperties.Columns.Add(colName);
            grdDataStoreProperties.Columns.Add(colValue);
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_CREATE_DATA_STORE"); }
        }

        public event EventHandler TitleChanged;

        public bool CanClose
        {
            get { return true; }
        }

        public bool Close()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }

        public bool SaveAs()
        {
            return true;
        }

        public event EventHandler ViewContentClosing = delegate { };

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (_presenter.CreateDataStore())
            {
                ViewContentClosing(this, EventArgs.Empty);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        private void cmbProvider_SelectionChanged(object sender, EventArgs e)
        {
            _presenter.ProviderChanged();
        }

        public IList<FdoToolbox.Core.Feature.FdoProviderInfo> ProviderList
        {
            set {
                cmbProvider.DisplayMember = "DisplayName";
                cmbProvider.DataSource = value;
            }
        }

        public System.Collections.Specialized.NameValueCollection ConnectProperties
        {
            get
            {
                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
                foreach (DataGridViewRow row in grdConnectionProperties.Rows)
                {
                    object on = row.Cells[0].Value;
                    object ov = row.Cells[1].Value;
                    if (on != null && ov != null)
                    {
                        nvc.Add(on.ToString(), ov.ToString());
                    }
                }
                return nvc;
            }
        }

        public System.Collections.Specialized.NameValueCollection DataStoreProperties
        {
            get
            {
                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
                foreach (DataGridViewRow row in grdDataStoreProperties.Rows)
                {
                    object on = row.Cells[0].Value;
                    object ov = row.Cells[1].Value;
                    if (on != null && ov != null)
                    {
                        nvc.Add(on.ToString(), ov.ToString());
                    }
                }
                return nvc;
            }
        }

        public void ResetConnectGrid()
        {
            grdConnectionProperties.Rows.Clear();
        }

        public void AddConnectProperty(DictionaryProperty p)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = p.LocalizedName;

            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            if (p.IsFile || p.IsPath)
            {
                valueCell.ContextMenuStrip = ctxHelper;
                valueCell.ToolTipText = "Right click for helpful options";
            }
            valueCell.Value = p.DefaultValue;

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdConnectionProperties.Rows.Add(row);
        }

        public void AddEnumerableConnectProperty(string name, string defaultValue, string[] values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdConnectionProperties.Rows.Add(row);
        }

        public void ResetDataStoreGrid()
        {
            grdDataStoreProperties.Rows.Clear();
        }

        public void AddDataStoreProperty(DictionaryProperty p)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = p.LocalizedName;

            DataGridViewTextBoxCell valueCell = new DataGridViewTextBoxCell();
            if (p.IsFile || p.IsPath)
            {
                valueCell.ContextMenuStrip = ctxHelper;
                valueCell.ToolTipText = "Right click for helpful options";
            }
            valueCell.Value = p.DefaultValue;

            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdDataStoreProperties.Rows.Add(row);
        }

        public void AddEnumerableDataStoreProperty(string name, string defaultValue, string[] values)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = name;
            DataGridViewComboBoxCell valueCell = new DataGridViewComboBoxCell();
            valueCell.DataSource = values;
            valueCell.Value = defaultValue;
            row.Cells.Add(nameCell);
            row.Cells.Add(valueCell);

            grdDataStoreProperties.Rows.Add(row);
        }


        public FdoToolbox.Core.Feature.FdoProviderInfo SelectedProvider
        {
            get { return cmbProvider.SelectedItem as FdoToolbox.Core.Feature.FdoProviderInfo; }
        }

        public void OnClose()
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        public bool CreateEnabled
        {
            set { btnCreate.Enabled = value; }
        }

        private void insertCurrentApplicationPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentCell != null)
            {
                _currentCell.Value = FileUtility.ApplicationRootPath;
            }
        }

        private void insertFilePathOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentCell != null)
            {
                string file = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_ALL"));
                if (FileService.FileExists(file))
                {
                    _currentCell.Value = file;
                }
            }
        }

        private void insertFilePathSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentCell != null)
            {
                string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_FILE"), ResourceService.GetString("FILTER_ALL"));
                if (file != null)
                {
                    _currentCell.Value = file;
                }
            }
        }

        private void insertDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentCell != null)
            {
                string dir = FileService.GetDirectory(ResourceService.GetString("TITLE_SELECT_DIRECTORY"));
                if (dir != null)
                {
                    _currentCell.Value = dir;
                }
            }
        }

        private DataGridViewCell _currentCell;

        private void grdDataStoreProperties_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            _currentCell = null;
            if (e.Button == MouseButtons.Right)
            {
                _currentCell = grdDataStoreProperties.CurrentCell = grdDataStoreProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void grdConnectionProperties_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            _currentCell = null;
            if (e.Button == MouseButtons.Right)
            {
                _currentCell = grdConnectionProperties.CurrentCell = grdConnectionProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }
    }
}
