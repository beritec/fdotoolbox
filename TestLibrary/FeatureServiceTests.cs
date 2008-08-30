#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using System.IO;
using OSGeo.FDO.Common;
using OSGeo.FDO.ClientServices;
using FdoToolbox.Core.Common;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Connections.Capabilities;
using NMock2;

namespace FdoToolbox.Tests
{
    [Category("FdoToolboxCore")]
    [TestFixture]
    public class FeatureServiceTests : BaseTest
    {
        [Test]
        [ExpectedException(typeof(FeatureServiceException))]
        public void TestUnopenedConnection()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn)) { }
                    Assert.Fail("Service should not accept un-opened connection");
                }
            }
            finally
            {
                if(File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestSchemaClone()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.CloneSchemas();

                            using (schemas)
                            {
                                foreach (FeatureSchema schema in schemas)
                                {
                                    FeatureSchema fs = service.GetSchemaByName(schema.Name);

                                    Assert.IsNotNull(fs);
                                    AssertHelper.EqualSchemas(service, schema, fs);
                                }
                            }
                        }
                        catch (OSGeo.FDO.Common.Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
                    }

                    conn.Close();
                }
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestCloneClass()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.DescribeSchema();
                            ClassDefinition classDef = schemas[0].Classes[0];

                            Assert.IsNotNull(classDef);

                            ClassDefinition cd = FeatureService.CloneClass(classDef);
                            AssertHelper.EqualClass(classDef, cd);
                        }
                        catch (OSGeo.FDO.Common.Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
                    }

                    conn.Close();
                }
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestCloneProperty()
        {
            try
            {
                bool result = ExpressUtility.CreateSDF("Test.sdf");
                Assert.IsTrue(result);

                IConnection conn = ExpressUtility.CreateSDFConnection("Test.sdf", false);
                using (conn)
                {
                    conn.Open();
                    Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);

                    using (FeatureService service = new FeatureService(conn))
                    {
                        try
                        {
                            service.LoadSchemasFromXml("Test.xml");
                            FeatureSchemaCollection schemas = service.DescribeSchema();
                            ClassDefinition classDef = schemas[0].Classes[0];
                            Assert.IsNotNull(classDef);
                            foreach (PropertyDefinition propDef in classDef.Properties)
                            {
                                PropertyDefinition pd = FeatureService.CloneProperty(propDef);
                                AssertHelper.EqualProperty(propDef, pd);
                            }
                        }
                        catch (OSGeo.FDO.Common.Exception ex)
                        {
                            Assert.Fail(ex.ToString());
                        }
                    }

                    conn.Close();
                }
            }
            finally
            {
                if (File.Exists("Test.sdf"))
                    File.Delete("Test.sdf");
            }
        }

        [Test]
        public void TestCreateSpatialContext()
        {
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                SpatialContextInfo ctx = new SpatialContextInfo();
                ctx.Name = "Default";
                ctx.CoordinateSystem = "";
                ctx.CoordinateSystemWkt = "";
                ctx.Description = "Default Spatial Context";
                ctx.ExtentType = OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                ctx.XYTolerance = 0.0001;
                ctx.ZTolerance = 0.0001;

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                conn.Open();
                Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn))
                    {
                        service.CreateSpatialContext(ctx, false);

                        List<SpatialContextInfo> contexts = service.GetSpatialContexts();
                        SpatialContextInfo sc = service.GetSpatialContext(ctx.Name);

                        Assert.IsNotNull(sc);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);
                    }
                }
            }
            finally
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestUpdateSpatialContext()
        {
            string file = "Test.sdf";
            try
            {
                bool result = ExpressUtility.CreateSDF(file);
                Assert.IsTrue(result);

                SpatialContextInfo ctx = new SpatialContextInfo();
                ctx.Name = "Default";
                ctx.CoordinateSystem = "";
                ctx.CoordinateSystemWkt = "";
                ctx.Description = "Default Spatial Context";
                ctx.ExtentType = OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Dynamic;
                ctx.XYTolerance = 0.0001;
                ctx.ZTolerance = 0.0001;

                IConnection conn = ExpressUtility.CreateSDFConnection(file, false);
                conn.Open();
                Assert.IsTrue(conn.ConnectionState == ConnectionState.ConnectionState_Open);
                using (conn)
                {
                    using (FeatureService service = new FeatureService(conn))
                    {
                        service.CreateSpatialContext(ctx, false);

                        List<SpatialContextInfo> contexts = service.GetSpatialContexts();
                        SpatialContextInfo sc = service.GetSpatialContext(ctx.Name);

                        Assert.IsNotNull(sc);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);

                        sc.XYTolerance = 0.2;
                        sc.ZTolerance = 0.3;
                        sc.Description = "Foobar";

                        service.CreateSpatialContext(sc, true);

                        ctx = service.GetSpatialContext(sc.Name);
                        Assert.IsNotNull(ctx);
                        Assert.AreEqual(sc.CoordinateSystem, ctx.CoordinateSystem);
                        Assert.AreEqual(sc.CoordinateSystemWkt, ctx.CoordinateSystemWkt);
                        Assert.AreEqual(sc.Description, ctx.Description);
                        Assert.AreEqual(sc.ExtentGeometryText, ctx.ExtentGeometryText);
                        Assert.AreEqual(sc.ExtentType, ctx.ExtentType);
                        Assert.AreEqual(sc.IsActive, ctx.IsActive);
                        Assert.AreEqual(sc.Name, ctx.Name);
                        Assert.AreEqual(sc.XYTolerance, ctx.XYTolerance);
                        Assert.AreEqual(sc.ZTolerance, ctx.ZTolerance);

                    }
                }
            }
            finally
            {
                if (!File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public void TestSchemaCanBeApplied()
        {
            FeatureSchema schema = new FeatureSchema("Default", "");
            FeatureClass fc = new FeatureClass("Class1", "");

            DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
            id.DataType = DataType.DataType_Int32;
            id.IsAutoGenerated = true;

            fc.Properties.Add(id);
            fc.IdentityProperties.Add(id);

            GeometricPropertyDefinition geom = new GeometricPropertyDefinition("Geometry", "");
            geom.GeometryTypes = (int)GeometryType.GeometryType_Point;

            fc.Properties.Add(geom);
            schema.Classes.Add(fc);

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
            conn.ConnectionString = "DefaultFileLocation=" + AppGateway.RunningApplication.AppPath;
            using (conn)
            {
                conn.Open();
                using (FeatureService service = new FeatureService(conn))
                {
                    IncompatibleSchema incSchema = null;
                    bool result = service.CanApplySchema(schema, out incSchema);
                    Assert.IsNull(incSchema);
                    Assert.IsTrue(result);
                }
                conn.Close();
            }
        }

        [Test]
        public void TestSchemaCannotBeApplied()
        {
            FeatureSchema schema = new FeatureSchema("Default", "");
            FeatureClass fc = new FeatureClass("Class1", "");

            DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
            id.DataType = DataType.DataType_Int32;
            id.IsAutoGenerated = true;

            fc.Properties.Add(id);
            fc.IdentityProperties.Add(id);

            //Unsupported property in SHP
            DataPropertyDefinition d1 = new DataPropertyDefinition("Unsupported", "");
            d1.DataType = DataType.DataType_Int64;
            d1.Nullable = true;

            fc.Properties.Add(d1);

            GeometricPropertyDefinition geom = new GeometricPropertyDefinition("Geometry", "");
            geom.GeometryTypes = (int)GeometryType.GeometryType_Point;

            fc.Properties.Add(geom);
            schema.Classes.Add(fc);

            IConnection conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SHP");
            conn.ConnectionString = "DefaultFileLocation=" + AppGateway.RunningApplication.AppPath;
            using (conn)
            {
                conn.Open();
                using (FeatureService service = new FeatureService(conn))
                {
                    IncompatibleSchema incSchema = null;
                    bool result = service.CanApplySchema(schema, out incSchema);
                    Assert.IsNotNull(incSchema);
                    Assert.IsFalse(result);

                    foreach (IncompatibleClass incClass in incSchema.Classes)
                    {
                        foreach (IncompatibleProperty incProp in incClass.Properties)
                        {
                            Assert.AreEqual(incProp.Reasons.Count, incProp.ReasonCodes.Count);
                        }
                    }
                }
                conn.Close();
            }
        }

        [Test]
        public void TestBoolPromotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };
            //bool -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64 };
            //bool -> int64
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
            Assert.AreEqual(DataType.DataType_Int64, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64, DataType.DataType_Int32 };
            //bool -> int32
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
            Assert.AreEqual(DataType.DataType_Int32, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64, DataType.DataType_Int32, DataType.DataType_Int16 };
            //bool -> int16
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
            Assert.AreEqual(DataType.DataType_Int16, dt);

            dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //bool -> byte
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
            Assert.AreEqual(DataType.DataType_Byte, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Boolean, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestBytePromotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };

            //byte -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Byte, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64 };
            //byte -> int64
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Byte, dtypes);
            Assert.AreEqual(DataType.DataType_Int64, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64, DataType.DataType_Int32 };
            //byte -> int32
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Byte, dtypes);
            Assert.AreEqual(DataType.DataType_Int32, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64, DataType.DataType_Int32, DataType.DataType_Int16 };
            //byte -> int16
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Byte, dtypes);
            Assert.AreEqual(DataType.DataType_Int16, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Byte, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestDateTimePromotion()
        {
            DataType[] dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_DateTime, dtypes);

            Assert.AreEqual(DataType.DataType_String, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_DateTime, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestDecimalPromotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };
            //decimal -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Decimal, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //decimal -> double
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Decimal, dtypes);
            Assert.AreEqual(DataType.DataType_Double, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Decimal, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestInt16Promotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };
            //int16 -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Int16, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = new DataType[] { DataType.DataType_String, DataType.DataType_Int64 };
            //int16 -> int64
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Int16, dtypes);
            Assert.AreEqual(DataType.DataType_Int64, dt);

            dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //int16 -> int32
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Int16, dtypes);
            Assert.AreEqual(DataType.DataType_Int32, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Int16, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestInt32Promotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };
            //int32 -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Int32, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //int32 -> int64
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Int32, dtypes);
            Assert.AreEqual(DataType.DataType_Int64, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Int32, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestInt64Promotion()
        {
            DataType[] dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //int32 -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Int64, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Int64, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestSinglePromotion()
        {
            DataType[] dtypes = new DataType[] { DataType.DataType_String };
            //decimal -> string
            DataType dt = FeatureService.GetPromotedDataType(DataType.DataType_Single, dtypes);
            Assert.AreEqual(DataType.DataType_String, dt);

            dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            //decimal -> double
            dt = FeatureService.GetPromotedDataType(DataType.DataType_Single, dtypes);
            Assert.AreEqual(DataType.DataType_Double, dt);

            //No suitable data type
            dtypes = new DataType[] { DataType.DataType_CLOB, DataType.DataType_BLOB };
            try
            {
                dt = FeatureService.GetPromotedDataType(DataType.DataType_Single, dtypes);
                Assert.Fail("Should have failed to find suitable data type");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestFailedDataTypePromotion()
        {
            DataType[] dtypes = (DataType[])Enum.GetValues(typeof(DataType));

            try
            {
                FeatureService.GetPromotedDataType(DataType.DataType_BLOB, dtypes);
                Assert.Fail("BLOB is not convertible");
            }
            catch (FeatureServiceException)
            { }

            try
            {
                FeatureService.GetPromotedDataType(DataType.DataType_CLOB, dtypes);
                Assert.Fail("CLOB is not convertible");
            }
            catch (FeatureServiceException)
            { }

            try
            {
                FeatureService.GetPromotedDataType(DataType.DataType_Double, dtypes);
                Assert.Fail("double is not convertible");
            }
            catch (FeatureServiceException)
            { }

            try
            {
                FeatureService.GetPromotedDataType(DataType.DataType_String, dtypes);
                Assert.Fail("string is not convertible");
            }
            catch (FeatureServiceException)
            { }
        }

        [Test]
        public void TestAlterSchemaPassAutoId()
        {
            IConnection conn = CreateMockedConnection();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition cls = new Class("Test", "");

                //ID
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Int32;
                id.IsAutoGenerated = true; //Should be converted to int64
                id.ReadOnly = true;
                id.Nullable = false;

                cls.Properties.Add(id);
                cls.IdentityProperties.Add(id);

                //Name
                DataPropertyDefinition name = new DataPropertyDefinition("Name", "");
                name.DataType = DataType.DataType_String;
                name.Nullable = true;
                name.Length = 100;

                cls.Properties.Add(name);

                schema.Classes.Add(cls);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[0];
                //Should be converted to int64
                Assert.AreEqual(DataType.DataType_Int64, newClass.IdentityProperties[0].DataType);
            }
        }

        [Test]
        public void TestAlterSchemaPassValueConstraints()
        {
            IConnection conn = CreateMockedConnection();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition cls = new Class("Test", "");

                //ID
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Int64;
                id.IsAutoGenerated = true; //Should be converted to int64
                id.ReadOnly = true;
                id.Nullable = false;

                cls.Properties.Add(id);
                cls.IdentityProperties.Add(id);

                //Age
                DataPropertyDefinition age = new DataPropertyDefinition("Age", "");
                PropertyValueConstraintRange range = new PropertyValueConstraintRange();
                range.MinValue = new Int32Value(0);
                range.MinInclusive = true;
                range.MaxValue = new Int32Value(100);
                range.MaxInclusive = true;
                age.ValueConstraint = range;
                age.DataType = DataType.DataType_Int32;
                age.Nullable = true;

                cls.Properties.Add(age);

                //Gender
                DataPropertyDefinition gender = new DataPropertyDefinition("Gender", "");
                PropertyValueConstraintList list = new PropertyValueConstraintList();
                list.ConstraintList.Add(new StringValue("M"));
                list.ConstraintList.Add(new StringValue("F"));
                gender.ValueConstraint = list;
                age.DataType = DataType.DataType_String;
                age.Nullable = false;
                age.Length = 1;

                cls.Properties.Add(gender);

                schema.Classes.Add(cls);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[0];
                
                DataPropertyDefinition age2 = newClass.Properties[newClass.Properties.IndexOf("Age")] as DataPropertyDefinition;

                //Should have constraint removed
                Assert.IsNull(age2.ValueConstraint);

                DataPropertyDefinition gender2 = newClass.Properties[newClass.Properties.IndexOf("Gender")] as DataPropertyDefinition;
                //Should have constraint removed
                Assert.IsNull(gender2.ValueConstraint);
            }
        }

        [Test]
        public void TestAlterSchemaPassCompositeId()
        {
            //Test composite id not supported
            IConnection conn = CreateMockedConnection();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition cls = new Class("Test", "");

                //ID
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Int32;
                id.IsAutoGenerated = true; //Should be converted to int64
                id.ReadOnly = true;
                id.Nullable = false;

                cls.Properties.Add(id);
                cls.IdentityProperties.Add(id);

                //ID2
                DataPropertyDefinition id2 = new DataPropertyDefinition("ID2", "");
                id2.DataType = DataType.DataType_Int32;
                id2.IsAutoGenerated = true; //Should be converted to int64
                id2.ReadOnly = true;
                id2.Nullable = false;

                cls.Properties.Add(id2);
                cls.IdentityProperties.Add(id2);

                //Name
                DataPropertyDefinition name = new DataPropertyDefinition("Name", "");
                name.DataType = DataType.DataType_String;
                name.Nullable = true;
                name.Length = 100;

                cls.Properties.Add(name);

                schema.Classes.Add(cls);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[0];

                Assert.AreEqual(1, newClass.IdentityProperties.Count);
                Assert.AreEqual(4, newClass.Properties.Count);

                //ID1 and ID2 should be replaced with an autogenerated property
                Assert.AreEqual(DataType.DataType_Int64, newClass.IdentityProperties[0].DataType);
                Assert.IsTrue(newClass.IdentityProperties[0].IsAutoGenerated);
                Assert.AreNotEqual("ID1", newClass.IdentityProperties[0].Name);
                Assert.AreNotEqual("ID2", newClass.IdentityProperties[0].Name);
            }
        }

        [Test]
        public void TestAlterSchemaPassIdentityType()
        {
            IConnection conn = CreateMockedConnection();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition cls = new Class("Test", "");

                //ID - float
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Single;
                id.Nullable = false;

                cls.Properties.Add(id);
                cls.IdentityProperties.Add(id);

                //Name
                DataPropertyDefinition name = new DataPropertyDefinition("Name", "");
                name.DataType = DataType.DataType_String;
                name.Nullable = true;
                name.Length = 100;

                cls.Properties.Add(name);

                schema.Classes.Add(cls);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[0];

                //Should have been "promoted" to string
                Assert.AreEqual(DataType.DataType_String, newClass.IdentityProperties[0].DataType);
            }
        }

        [Test]
        public void TestAlterSchemaBaseClass()
        {
            IConnection conn = CreateMockedConnection();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition baseClass = new Class("Base", "");

                //ID
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Int64;
                id.Nullable = false;

                baseClass.Properties.Add(id);
                baseClass.IdentityProperties.Add(id);

                //Name
                DataPropertyDefinition name = new DataPropertyDefinition("Name", "");
                name.DataType = DataType.DataType_String;
                name.Nullable = true;
                name.Length = 100;

                baseClass.Properties.Add(name);

                ClassDefinition derivedClass = new Class("Derived", "");
                derivedClass.BaseClass = baseClass;

                //DOB
                DataPropertyDefinition dob = new DataPropertyDefinition("DOB", "");
                dob.DataType = DataType.DataType_DateTime;
                dob.Nullable = true;

                derivedClass.Properties.Add(dob);

                schema.Classes.Add(baseClass);
                schema.Classes.Add(derivedClass);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[newSchema.Classes.IndexOf("Derived")];

                //Base class properties should be copied to derived class
                Assert.AreEqual("BASE_ID", newClass.IdentityProperties[0].Name);
                Assert.AreEqual(3, newClass.Properties.Count);
                Assert.IsTrue(newClass.Properties.IndexOf("BASE_ID") >= 0);
                Assert.IsTrue(newClass.Properties.IndexOf("BASE_Name") >= 0);
            }
        }

        [Test]
        public void TestAlterSchemaNullable()
        {
            IConnection conn = CreateMockedConnection2();

            using (FeatureService service = new FeatureService(conn))
            {
                FeatureSchema schema = new FeatureSchema("Default", "");

                ClassDefinition cls = new Class("Test", "");

                //ID
                DataPropertyDefinition id = new DataPropertyDefinition("ID", "");
                id.DataType = DataType.DataType_Int64;
                id.Nullable = false;

                cls.Properties.Add(id);
                cls.IdentityProperties.Add(id);

                //Name
                DataPropertyDefinition name = new DataPropertyDefinition("Name", "");
                name.DataType = DataType.DataType_String;
                name.Nullable = true;
                name.Length = 100;

                cls.Properties.Add(name);

                schema.Classes.Add(cls);

                IncompatibleSchema incSchema = null;
                bool canApply = service.CanApplySchema(schema, out incSchema);
                Assert.IsFalse(canApply);
                Assert.IsNotNull(incSchema);

                FeatureSchema newSchema = service.AlterSchema(schema, incSchema);

                ClassDefinition newClass = newSchema.Classes[0];

                //Name should not be nullable anymore
                DataPropertyDefinition name2 = newClass.Properties[newClass.Properties.IndexOf("Name")] as DataPropertyDefinition;
                Assert.IsFalse(name2.Nullable);
            }
        }

        private static IConnection CreateMockedConnection2()
        {
            Mockery mock = new Mockery();

            ClassType[] ctypes = new ClassType[] { ClassType.ClassType_Class, ClassType.ClassType_FeatureClass };
            DataType[] dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            DataType[] idTypes = new DataType[] { DataType.DataType_Int64, DataType.DataType_Int32, DataType.DataType_String };
            DataType[] atypes = new DataType[] { DataType.DataType_Int64 };

            ISchemaCapabilities mockSchemaCapabilities = mock.NewMock<ISchemaCapabilities>();

            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("ClassTypes").Will(Return.Value(ctypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("DataTypes").Will(Return.Value(dtypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportedAutoGeneratedTypes").Will(Return.Value(atypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("MaximumDecimalPrecision").Will(Return.Value(20));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("MaximumDecimalScale").Will(Return.Value(20));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportedIdentityPropertyTypes").Will(Return.Value(idTypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsAssociationProperties").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsAutoIdGeneration").Will(Return.Value(true));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsCompositeId").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsDefaultValue").Will(Return.Value(true));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsInheritance").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsMultipleSchemas").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsExclusiveValueRangeConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsInclusiveValueRangeConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsNullValueConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsObjectProperties").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsUniqueValueConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsValueConstraintsList").Will(Return.Value(false));

            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_BLOB).Will(Return.Value((long)int.MaxValue));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Boolean).Will(Return.Value((long)sizeof(bool)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Byte).Will(Return.Value((long)sizeof(byte)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_CLOB).Will(Return.Value((long)int.MaxValue));

            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Decimal).Will(Return.Value((long)sizeof(decimal)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Double).Will(Return.Value((long)sizeof(double)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int16).Will(Return.Value((long)sizeof(short)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int32).Will(Return.Value((long)sizeof(int)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int64).Will(Return.Value((long)sizeof(long)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Single).Will(Return.Value((long)sizeof(float)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_String).Will(Return.Value((long)int.MaxValue));

            IConnection conn = mock.NewMock<IConnection>();
            Expect.AtLeastOnce.On(conn).GetProperty("ConnectionState").Will(Return.Value(ConnectionState.ConnectionState_Open));
            Expect.AtLeastOnce.On(conn).GetProperty("SchemaCapabilities").Will(Return.Value(mockSchemaCapabilities));
            return conn;
        }

        private static IConnection CreateMockedConnection()
        {
            Mockery mock = new Mockery();

            ClassType[] ctypes = new ClassType[] { ClassType.ClassType_Class, ClassType.ClassType_FeatureClass };
            DataType[] dtypes = (DataType[])Enum.GetValues(typeof(DataType));
            DataType[] idTypes = new DataType[] { DataType.DataType_Int64, DataType.DataType_Int32, DataType.DataType_String };
            DataType[] atypes = new DataType[] { DataType.DataType_Int64 };

            ISchemaCapabilities mockSchemaCapabilities = mock.NewMock<ISchemaCapabilities>();

            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("ClassTypes").Will(Return.Value(ctypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("DataTypes").Will(Return.Value(dtypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportedAutoGeneratedTypes").Will(Return.Value(atypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("MaximumDecimalPrecision").Will(Return.Value(20));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("MaximumDecimalScale").Will(Return.Value(20));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportedIdentityPropertyTypes").Will(Return.Value(idTypes));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsAssociationProperties").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsAutoIdGeneration").Will(Return.Value(true));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsCompositeId").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsDefaultValue").Will(Return.Value(true));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsInheritance").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsMultipleSchemas").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsExclusiveValueRangeConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsInclusiveValueRangeConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsNullValueConstraints").Will(Return.Value(true));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsObjectProperties").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsUniqueValueConstraints").Will(Return.Value(false));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).GetProperty("SupportsValueConstraintsList").Will(Return.Value(false));

            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_BLOB).Will(Return.Value((long)1000000000));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Boolean).Will(Return.Value((long)sizeof(bool)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Byte).Will(Return.Value((long)sizeof(byte)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_CLOB).Will(Return.Value((long)500000000));

            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Decimal).Will(Return.Value((long)sizeof(decimal)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Double).Will(Return.Value((long)sizeof(double)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int16).Will(Return.Value((long)sizeof(short)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int32).Will(Return.Value((long)sizeof(int)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Int64).Will(Return.Value((long)sizeof(long)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_Single).Will(Return.Value((long)sizeof(float)));
            Expect.AtLeastOnce.On(mockSchemaCapabilities).Method("get_MaximumDataValueLength").With(DataType.DataType_String).Will(Return.Value((long)1289403958));

            IConnection conn = mock.NewMock<IConnection>();
            Expect.AtLeastOnce.On(conn).GetProperty("ConnectionState").Will(Return.Value(ConnectionState.ConnectionState_Open));
            Expect.AtLeastOnce.On(conn).GetProperty("SchemaCapabilities").Will(Return.Value(mockSchemaCapabilities));
            return conn;
        }

        [Test]
        public void TestFixDataProperty()
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.TableName = "Foobar";
            table.Columns.Add("BLOB", typeof(byte[]));
            table.Columns.Add("str", typeof(string));
            table.Columns.Add("dec", typeof(decimal));
            table.Columns.Add("ID", typeof(int));
            table.PrimaryKey = new System.Data.DataColumn[] { table.Columns["ID"] };

            FdoDataTable fdoTable = TableFactory.CreateTable(table);
            ClassDefinition classDef = fdoTable.GetClassDefinition();

            Assert.AreEqual("Foobar", classDef.Name);

            int iblob = classDef.Properties.IndexOf("BLOB");
            int istr = classDef.Properties.IndexOf("str");
            int idec = classDef.Properties.IndexOf("dec");
            int iid = classDef.Properties.IndexOf("ID");

            Assert.IsTrue(iblob >= 0);
            Assert.IsTrue(istr >= 0);
            Assert.IsTrue(idec >= 0);
            Assert.IsTrue(iid >= 0);

            DataPropertyDefinition blob = classDef.Properties[iblob] as DataPropertyDefinition;
            DataPropertyDefinition str = classDef.Properties[istr] as DataPropertyDefinition;
            DataPropertyDefinition dec = classDef.Properties[idec] as DataPropertyDefinition;
            DataPropertyDefinition id = classDef.Properties[iid] as DataPropertyDefinition;

            Assert.IsNotNull(blob);
            Assert.IsNotNull(str);
            Assert.IsNotNull(dec);
            Assert.IsNotNull(id);

            Assert.AreEqual(int.MaxValue, blob.Length);
            Assert.AreEqual(int.MaxValue, str.Length);
            Assert.AreEqual(int.MaxValue, dec.Scale);
            Assert.AreEqual(int.MaxValue, dec.Precision);

            IConnection conn = CreateMockedConnection();
            using (FeatureService service = new FeatureService(conn))
            {
                service.FixDataProperties(ref classDef);

                blob = classDef.Properties[iblob] as DataPropertyDefinition;
                str = classDef.Properties[istr] as DataPropertyDefinition;
                dec = classDef.Properties[idec] as DataPropertyDefinition;
                id = classDef.Properties[iid] as DataPropertyDefinition;

                Assert.IsNotNull(blob);
                Assert.IsNotNull(str);
                Assert.IsNotNull(dec);
                Assert.IsNotNull(id);

                //The lengths should now fall within the connection's limits.
                Assert.IsTrue(blob.Length < int.MaxValue);
                Assert.IsTrue(str.Length < int.MaxValue);
                Assert.IsTrue(dec.Scale < int.MaxValue);
                Assert.IsTrue(dec.Precision < int.MaxValue);
            }
        }

        [Test]
        public void TestAlterSchemaFail()
        {
            Assert.Fail("Not implemented");
        }
    }
}
