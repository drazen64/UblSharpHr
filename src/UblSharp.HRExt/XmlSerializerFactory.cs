using System.Reflection;
using System.Xml.Serialization;

namespace UblSharp.HRExt
{
    public class XmlSerializerFactory
    {
        // private const string XmlnsV1 = "urn:www.energie-efactuur.nl:profile:invoice:ver1.0";
        //private const string XmlnsMfin = "urn:mfin.gov.hr:schema:xsd:HRExtensionAggregateComponents-1";
        private const string XmlnsMfin = "urn:hzn.hr:schema:xsd:HRExtensionAggregateComponents-1";


        private static readonly XmlSerializer s_serializer;

        public static XmlSerializerFactory Default { get; } = new XmlSerializerFactory();

        static XmlSerializerFactory()
        {

#if NETSTANDARD1_0 || NETSTANDARD1_3
            var assembly = typeof(HRFISK20DataType).GetTypeInfo().Assembly;
#else
            var assembly = typeof(HRFISK20DataType).Assembly;
#endif

            var overridesV2 = CreateXmlAttributeOverrides(assembly, XmlnsMfin);
            s_serializer = new XmlSerializer(typeof(HRFISK20DataType), overridesV2);

        }

        public virtual XmlSerializer GetSerializer()
        {
            return s_serializer;
        }

        protected static XmlAttributeOverrides CreateXmlAttributeOverrides(Assembly assemblytoScan, string xmlns)
        {
            var overrides = new XmlAttributeOverrides();
#if NETSTANDARD1_0 || NETSTANDARD1_3
            var types = assemblytoScan.ExportedTypes;
#else
            var types = assemblytoScan.GetExportedTypes();
#endif
            foreach (var type in types)
            {
#if NETSTANDARD1_0 || NETSTANDARD1_3
                var rootAttrs = type.GetTypeInfo().GetCustomAttributes(typeof(XmlRootAttribute), false).ToArray();
                var typeAttrs = type.GetTypeInfo().GetCustomAttributes(typeof(XmlTypeAttribute), false).ToArray();
#else
                var rootAttrs = type.GetCustomAttributes(typeof(XmlRootAttribute), false);
                var typeAttrs = type.GetCustomAttributes(typeof(XmlTypeAttribute), false);
#endif
                if (rootAttrs.Length == 0
                    && typeAttrs.Length == 0)
                {
                    continue;
                }

                var attrs = new XmlAttributes();
                if (rootAttrs.Length > 0)
                {
                    var rootAttr = (XmlRootAttribute)rootAttrs[0];
                    rootAttr.Namespace = xmlns;
                    attrs.XmlRoot = rootAttr;
                }

                if (typeAttrs.Length > 0)
                {
                    var typeAttr = (XmlTypeAttribute)typeAttrs[0];
                    typeAttr.Namespace = xmlns;
                    attrs.XmlType = typeAttr;
                }

                overrides.Add(type, attrs);
            }

            return overrides;
        }

    }
}
