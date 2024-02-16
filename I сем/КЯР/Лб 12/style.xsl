<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
          <![CDATA[
            body {
              font-family: 'Arial', sans-serif;
            }
            h1 {
              color: blue;
            }
            p {
              font-size: 14px;
            }
          ]]>
        </style>
      </head>
      <body>
        <xsl:apply-templates select="information"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="information">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="topic">
    <h1><xsl:value-of select="title"/></h1>
    <p><xsl:value-of select="content"/></p>
  </xsl:template>

</xsl:stylesheet>
