using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;

namespace FdoToolbox.Core.Tests
{
    public sealed class AssertHelper
    {
        public static void EqualSchemas(FeatureSchema schema, FeatureSchema schema2)
        {
            Assert.AreEqual(schema.Name, schema2.Name);
            Assert.AreEqual(schema.Classes.Count, schema2.Classes.Count);

            foreach (ClassDefinition classDef in schema.Classes)
            {
                int idx = schema2.Classes.IndexOf(classDef.Name);
                Assert.IsTrue(idx >= 0);

                ClassDefinition cd = schema2.Classes[idx];
                EqualClass(classDef, cd);
            }
        }

        public static void EqualClass(ClassDefinition classDef, ClassDefinition cd)
        {
            Assert.AreEqual(classDef.Name, cd.Name);
            Assert.AreEqual(classDef.Properties.Count, cd.Properties.Count);
            Assert.AreEqual(classDef.IdentityProperties.Count, cd.IdentityProperties.Count);
            Assert.AreEqual(classDef.ClassType, cd.ClassType);

            switch (classDef.ClassType)
            {
                case ClassType.ClassType_FeatureClass:
                    {
                        FeatureClass fc1 = classDef as FeatureClass;
                        FeatureClass fc2 = cd as FeatureClass;

                        Assert.AreEqual(fc1.GeometryProperty.Name, fc2.GeometryProperty.Name);
                    }
                    break;
            }

            foreach (PropertyDefinition propDef in classDef.Properties)
            {
                int pidx = cd.Properties.IndexOf(propDef.Name);
                Assert.IsTrue(pidx >= 0, "Could not find property named {0} in class {1}", propDef.Name, cd.Name);
                PropertyDefinition pd = cd.Properties[pidx];

                EqualProperty(propDef, pd);
            }
        }

        public static void EqualProperty(PropertyDefinition propDef, PropertyDefinition pd)
        {
            Assert.AreEqual(propDef.Name, pd.Name);
            Assert.AreEqual(propDef.PropertyType, pd.PropertyType);

            switch (propDef.PropertyType)
            {
                case PropertyType.PropertyType_DataProperty:
                    {
                        DataPropertyDefinition d1 = propDef as DataPropertyDefinition;
                        DataPropertyDefinition d2 = pd as DataPropertyDefinition;

                        Assert.AreEqual(d1.DefaultValue, d2.DefaultValue);
                        Assert.AreEqual(d1.Description, d2.Description);
                        Assert.AreEqual(d1.IsAutoGenerated, d2.IsAutoGenerated);
                        Assert.AreEqual(d1.IsSystem, d2.IsSystem);
                        Assert.AreEqual(d1.Length, d2.Length);
                        Assert.AreEqual(d1.Nullable, d2.Nullable);
                        Assert.AreEqual(d1.Precision, d2.Precision);
                        Assert.AreEqual(d1.ReadOnly, d2.ReadOnly);
                        Assert.AreEqual(d1.Scale, d2.Scale);

                        if (d1.ValueConstraint != null)
                        {
                            Assert.IsNotNull(d2.ValueConstraint);
                            Assert.AreEqual(d1.ValueConstraint.ConstraintType, d1.ValueConstraint.ConstraintType);
                            switch (d1.ValueConstraint.ConstraintType)
                            {
                                case PropertyValueConstraintType.PropertyValueConstraintType_List:
                                    {
                                        PropertyValueConstraintList list1 = (PropertyValueConstraintList)d1.ValueConstraint;
                                        PropertyValueConstraintList list2 = (PropertyValueConstraintList)d2.ValueConstraint;

                                        Assert.AreEqual(list1.ConstraintList.Count, list2.ConstraintList.Count);

                                        foreach (DataValue val in list1.ConstraintList)
                                        {
                                            Assert.IsTrue(list2.ConstraintList.Contains(val));
                                        }
                                    }
                                    break;
                                case PropertyValueConstraintType.PropertyValueConstraintType_Range:
                                    {
                                        PropertyValueConstraintRange range1 = (PropertyValueConstraintRange)d1.ValueConstraint;
                                        PropertyValueConstraintRange range2 = (PropertyValueConstraintRange)d2.ValueConstraint;

                                        Assert.AreEqual(range1.MaxInclusive, range2.MaxInclusive);
                                        Assert.AreEqual(range1.MaxValue.ToString(), range2.MaxValue.ToString());
                                        Assert.AreEqual(range1.MinInclusive, range2.MinInclusive);
                                        Assert.AreEqual(range1.MinValue.ToString(), range2.MinValue.ToString());
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Assert.IsNull(d2.ValueConstraint);
                        }
                    }
                    break;
                case PropertyType.PropertyType_GeometricProperty:
                    {
                        GeometricPropertyDefinition g1 = (GeometricPropertyDefinition)propDef;
                        GeometricPropertyDefinition g2 = (GeometricPropertyDefinition)pd;

                        Assert.AreEqual(g1.GeometryTypes, g2.GeometryTypes);
                        Assert.AreEqual(g1.HasElevation, g2.HasElevation);
                        Assert.AreEqual(g1.HasMeasure, g2.HasMeasure);
                        Assert.AreEqual(g1.SpatialContextAssociation, g2.SpatialContextAssociation);
                    }
                    break;
            }
        }

        public static bool BinaryEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }
    }
}