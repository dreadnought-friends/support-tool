namespace SupportTool.AppVersion
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Tools
    {

        private ToolsTool[] toolField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Tool")]
        public ToolsTool[] Tool
        {
            get
            {
                return this.toolField;
            }
            set
            {
                this.toolField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ToolsTool
    {

        private ToolsToolMotd motdField;

        private ToolsToolSetting[] settingsField;

        private string nameField;

        private string releasePageField;

        private string latestField;

        private string primaryIpField;

        /// <remarks/>
        public string primaryIp
        {
            get
            {
                return this.primaryIpField;
            }
            set
            {
                this.primaryIpField = value;
            }
        }

        /// <remarks/>
        public ToolsToolMotd motd
        {
            get
            {
                return this.motdField;
            }
            set
            {
                this.motdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("setting", IsNullable = false)]
        public ToolsToolSetting[] settings
        {
            get
            {
                return this.settingsField;
            }
            set
            {
                this.settingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ReleasePage
        {
            get
            {
                return this.releasePageField;
            }
            set
            {
                this.releasePageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Latest
        {
            get
            {
                return this.latestField;
            }
            set
            {
                this.latestField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ToolsToolMotd
    {

        private string bodyField;

        private string titleField;

        /// <remarks/>
        public string body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ToolsToolSetting
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
