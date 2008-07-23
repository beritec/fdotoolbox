using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Commands.SQL;

namespace FdoToolbox.Core.Controls
{
    public partial class DataPreviewCtl : BaseDocumentCtl
    {
        internal DataPreviewCtl()
        {
            InitializeComponent();
            cmbQueryMethod.SelectedIndex = 0;
            cmbLimit.SelectedIndex = 0;
        }

        public DataPreviewCtl(IConnection conn)
            : this()
        {
            _BoundConnection = conn;
        }

        private IConnection _BoundConnection;

        protected override void OnLoad(EventArgs e)
        {
            using (IDescribeSchema desc = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema) as IDescribeSchema)
            {
                FeatureSchemaCollection schemas = desc.Execute();
                cmbSchema.DataSource = schemas;
            }
            base.OnLoad(e);
        }

        private void cmbQueryMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isStandard = (cmbQueryMethod.SelectedIndex == 0);
            grpStandard.Enabled = isStandard;
            grpSQL.Enabled = !isStandard;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            bool isStandard = (cmbQueryMethod.SelectedIndex == 0);
            if (isStandard)
            {
                int limit = Convert.ToInt32(cmbLimit.SelectedItem.ToString());
                using (ISelect select = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select) as ISelect)
                {
                    try
                    {
                        ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
                        if (classDef != null)
                        {
                            select.SetFeatureClassName(classDef.Name);
                            if (!string.IsNullOrEmpty(txtFilter.Text))
                                select.SetFilter(txtFilter.Text);
                            using (IFeatureReader reader = select.Execute())
                            {
                                DataTable table = new DataTable(cmbClass.SelectedItem.ToString());
                                int count = 0;
                                PrepareGrid(table, reader.GetClassDefinition());
                                while (reader.ReadNext() && count < limit)
                                {
                                    ProcessReader(table, reader);
                                    count++;
                                }
                                reader.Close();
                                grdPreview.DataSource = table;
                            }
                        }
                    }
                    catch (OSGeo.FDO.Common.Exception ex)
                    {
                        AppConsole.Alert("Error", ex.Message);
                        AppConsole.WriteException(ex);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
                /*
                int limit = Convert.ToInt32(cmbLimit.SelectedItem.ToString());
                using (ISQLCommand select = _BoundConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_SQLCommand) as ISQLCommand)
                {
                    select.SQLStatement = txtSQL.Text;
                    using (ISQLDataReader reader = select.ExecuteReader())
                    {
                        ClearGrid();
                        while (reader.ReadNext())
                        {
                            ProcessSQLReader(reader);
                        }
                        reader.Close();
                    }
                }
                 */
            }
        }

        private void PrepareGrid(DataTable table, ClassDefinition classDefinition)
        {
            foreach (PropertyDefinition def in classDefinition.Properties)
            {
                table.Columns.Add(def.Name);
            }
        }

        private void ProcessReader(DataTable table, IFeatureReader reader)
        {
            ClassDefinition classDef = reader.GetClassDefinition();
            DataRow row = table.NewRow();
            foreach (PropertyDefinition def in classDef.Properties)
            {
                if (!reader.IsNull(def.Name))
                {
                    DataPropertyDefinition dataDef = def as DataPropertyDefinition;
                    GeometricPropertyDefinition geomDef = def as GeometricPropertyDefinition;
                    if (dataDef != null)
                    {
                        switch (dataDef.DataType)
                        {
                            case DataType.DataType_String:
                                row[dataDef.Name] = reader.GetString(dataDef.Name);
                                break;
                            case DataType.DataType_Int16:
                                row[dataDef.Name] = reader.GetInt16(dataDef.Name);
                                break;
                            case DataType.DataType_Int32:
                                row[dataDef.Name] = reader.GetInt32(dataDef.Name);
                                break;
                            case DataType.DataType_Int64:
                                row[dataDef.Name] = reader.GetInt64(dataDef.Name);
                                break;
                            case DataType.DataType_Double:
                                row[dataDef.Name] = reader.GetDouble(dataDef.Name);
                                break;
                            case DataType.DataType_Boolean:
                                row[dataDef.Name] = reader.GetBoolean(dataDef.Name);
                                break;
                            case DataType.DataType_Byte:
                                row[dataDef.Name] = reader.GetByte(dataDef.Name);
                                break;
                            case DataType.DataType_BLOB:
                                row[dataDef.Name] = reader.GetLOB(dataDef.Name).Data;
                                break;
                            case DataType.DataType_CLOB:
                                row[dataDef.Name] = reader.GetLOB(dataDef.Name).Data;
                                break;
                            case DataType.DataType_DateTime:
                                row[dataDef.Name] = reader.GetDateTime(dataDef.Name);
                                break;
                            case DataType.DataType_Decimal:
                                row[dataDef.Name] = reader.GetDouble(dataDef.Name);
                                break;
                            case DataType.DataType_Single:
                                row[dataDef.Name] = reader.GetSingle(dataDef.Name);
                                break;
                        }
                    }
                    else if (geomDef != null)
                    {
                        row[geomDef.Name] = reader.GetGeometry(geomDef.Name);
                    }
                }
                else
                {
                    row[def.Name] = null;
                }
            }
            table.Rows.Add(row);
        }

        private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureSchema schema = cmbSchema.SelectedItem as FeatureSchema;
            if (schema != null)
            {
                cmbClass.DataSource = schema.Classes;
            }
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassDefinition classDef = cmbClass.SelectedItem as ClassDefinition;
            if(classDef != null)
                this.Title = "Data Preview - " + classDef.Name;
        }
    }
}