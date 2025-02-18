using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Xml.Xsl;

namespace PayrollEngine.WebApp;

/// <summary>
/// Xml tools
/// </summary>
public static class XmlTool
{
    private static readonly Encoding XmlEncoding = Encoding.UTF8;

    /// <summary>
    /// Retrieve data set XML
    /// </summary>
    /// <param name="dataSet">The data set</param>
    /// <returns>Data set XML</returns>
    public static string TransformXml(DataSet dataSet) =>
        DataSetToXml(dataSet).InnerXml;

    /// <summary>
    /// Retrieve transformed XML string based on XSL
    /// </summary>
    /// <param name="dataSet">The data set</param>
    /// <param name="xslString">XSL string used for transformation</param>
    /// <returns>Transformed XML</returns>
    public static string TransformXmlFromXsl(DataSet dataSet, string xslString)
    {
        if (dataSet == null)
        {
            throw new ArgumentException(nameof(dataSet));
        }
        if (xslString == null)
        {
            throw new ArgumentException(nameof(xslString));
        }

        var xmlDocument = DataSetToXml(dataSet);

        // style sheet
        using var xslStream = new MemoryStream(Convert.FromBase64String(xslString));
        using var xslReader = XmlReader.Create(xslStream);
        XslCompiledTransform xslt = new();
        xslt.Load(xslReader);

        // transformation
        using var stringWriter = new StringWriter();
        xslt.Transform(xmlDocument, null, stringWriter);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Validate XML against XSD Schema
    /// </summary>
    /// <param name="xmlString">XML string to be checked</param>
    /// <param name="xsdString">XSD schema string</param>
    /// <returns>Returns true if XML is valid</returns>
    public static bool ValidateXmlString(string xmlString, string xsdString)
    {
        var xsdStream = new MemoryStream(Convert.FromBase64String(xsdString));
        using var xmlReader = XmlReader.Create(xsdStream);

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlString);
        xmlDocument.Schemas.Add(null, xmlReader);
        xmlDocument.Validate(null);

        return true;
    }

    /// <summary>
    /// Return XML string as memory stream
    /// </summary>
    /// <param name="xml">XML string</param>
    /// <returns>Memory stream based on XML string input</returns>
    public static MemoryStream XmlToMemoryStream(string xml) =>
        new(XmlEncoding.GetBytes(xml));

    /// <summary>
    /// Convert data set to xml
    /// </summary>
    /// <param name="dataSet">Data set to convert</param>
    private static XmlNode DataSetToXml(DataSet dataSet)
    {
        using var stream = new MemoryStream();
        dataSet.WriteXml(stream);
        var xml = XmlEncoding.GetString(stream.ToArray());
        var document = new XmlDocument();
        document.LoadXml(xml);
        return document;
    }
}