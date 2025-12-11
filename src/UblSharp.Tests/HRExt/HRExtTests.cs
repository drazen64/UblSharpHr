using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using UblSharp.HRExt;
using UblSharp.SCSN;
using UblSharp.SEeF;
using UblSharp.Tests.Util;
using Xunit;
using Xunit.Abstractions;

namespace UblSharp.Tests.HRExt
{
    public class HRExtTests
    {
        private readonly ITestOutputHelper _output;

        public HRExtTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanDeserializePdv25()
        {
            var xmlSerializer = UblSharp.HRExt.XmlSerializerFactory.Default.GetSerializer();
            var pdv25Doc = ResourceHelper.GetResource("HRExt.Samples.eRacun-PDV25.xml");

            var pdv25 = UblDocument.Load<InvoiceType>(pdv25Doc);

            pdv25.Should().NotBeNull();
            pdv25.ID.Value.Should().Be("5-P1-1");

            pdv25.InvoiceLine[0].Item.CommodityClassification[0].ItemClassificationCode.Value.Should().Be("62.90.90");

            string outerXml = null;
            foreach(var ext in pdv25.UBLExtensions)
            {

                if (ext.GetType() == typeof(HRFISK20DataType))
                {
                    outerXml = ext.ExtensionContent.OuterXml;
                    break;
                }
            }
            if (outerXml != null)
            {
                //var extContent = pdv25.UBLExtensions[0].ExtensionContent.OuterXml;
                using (var sr = new StringReader(outerXml))
                {
                    var hrExt = (UblSharp.HRExt.HRFISK20DataType)xmlSerializer.Deserialize(sr);

                    hrExt.HRTaxTotal.TaxAmount.Value.Should().Be(25);
                    hrExt.HRTaxTotal.HRTaxSubtotal[0].TaxableAmount.Should().Be(100);

                }
            }
        }

        [Fact]
        public void CanDeserializePdveopPPTrosak()
        {
            var xmlSerializer = UblSharp.HRExt.XmlSerializerFactory.Default.GetSerializer();
            var pdv25Doc = ResourceHelper.GetResource("HRExt.Samples.eRacun-PDV-NEOP-PP-trosak.xml");

            var pdv25 = UblDocument.Load<InvoiceType>(pdv25Doc);

            pdv25.Should().NotBeNull();
            pdv25.ID.Value.Should().Be("10-P1-1");

            pdv25.InvoiceLine[0].Item.CommodityClassification[0].ItemClassificationCode.Value.Should().Be("11.07.11");

            string outerXml = null;
            foreach (var ext in pdv25.UBLExtensions)
            {
                if (ext.ExtensionContent != null && ext.ExtensionContent.OuterXml != null)
                {
                    // Check if the first element in ExtensionContent is the HRFISK20Data element
                    var firstElement = ext.ExtensionContent as System.Xml.XmlElement;
                    if (firstElement != null &&
                        firstElement.LocalName == "HRFISK20Data" &&
                        firstElement.NamespaceURI == "urn:hzn.hr:schema:xsd:HRExtensionAggregateComponents-1")
                    {
                        outerXml = ext.ExtensionContent.OuterXml;
                        break;
                    }
                }
            }
            if (outerXml != null)
            {
                //var extContent = pdv25.UBLExtensions[0].ExtensionContent.OuterXml;
                using (var sr = new StringReader(outerXml))
                {
                    var hrExt = (UblSharp.HRExt.HRFISK20DataType)xmlSerializer.Deserialize(sr);

                    hrExt.HRTaxTotal.TaxAmount.Value.Should().Be(25);
                    hrExt.HRTaxTotal.HRTaxSubtotal[0].TaxableAmount.Value.Should().Be(100);

                }
            }
        }
    }
}
