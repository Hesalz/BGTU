<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <style>
          .red {
            background-color: red;
          }

          .green {
            background-color: green;
          }

          table {
            border-collapse:collapse;
            text-align:center;
          }

          th {
            padding:8px;
            width:100px;
          }
        </style>
      </head>
      <body>
        <table border="1">
          <tr>
            <th>Фамилия</th>
            <th>Оценка</th>
          </tr>
          <xsl:apply-templates select="students/student"/>
        </table>
      </body>
    </html>
  </xsl:template>
  
  <xsl:template match="student">
    <tr>
      <td>
        <xsl:value-of select="name"/>
      </td>
      <td>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="grade &lt; 4">red</xsl:when>
            <xsl:when test="grade &gt; 8">green</xsl:when>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="grade"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>