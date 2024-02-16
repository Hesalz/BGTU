<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="yes"/>

    <xsl:template match="/">
    <html>
      <head>
        <style>
          table {
            border-collapse: collapse;
            width: 100%;
          }
          th {
            background-color: yellow;
          }
          th, td {
            border: 1px solid black;
            padding: 8px;
            text-align: left;
          }

        </style>
      </head>
      <body>
        <table>
          <tr>
            <th><xsl:value-of select="books/head/nauthor"/></th>
            <th><xsl:value-of select="books/head/ntitle"/></th>
            <th><xsl:value-of select="books/head/nnumber"/></th>
          </tr>
          <xsl:apply-templates select="books/book">
            <xsl:sort select="number" data-type="number" order="ascending"/>
          </xsl:apply-templates>
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="book">
    <tr>
      <td><xsl:value-of select="author"/></td>
      <td><xsl:value-of select="title"/></td>
      <td><xsl:value-of select="number"/></td>
    </tr>
  </xsl:template>

</xsl:stylesheet>