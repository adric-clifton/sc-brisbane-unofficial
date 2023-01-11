using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SwordcraftBrisbane.Data
{
    [Serializable]
    public partial record Warband
    {
        [XmlAttribute]
        public int Id;

        public string Name = "XXXXXX";
        public string ShortName = "XXXXXX";

        public string Leader;
        public bool ShowLeader = false;

        public string Hook = "XXXXXX";
        public string Active = "XXXXXX";
        public string Size = "XXXXXX";
        public string Location = "XXXXXX";

        public string? Description;
        public string Style = "XXXXXX";
        public string? Accent;

        public Trait Traits;

        public List<Group> Roles;

        public Colour Colours;

        [XmlArray]
        [XmlArrayItem(ElementName = "Blank", Type = typeof(Blank))]
        [XmlArrayItem(ElementName = "Sword", Type = typeof(Weapon.Sword))]
        [XmlArrayItem(ElementName = "Rapier", Type = typeof(Weapon.Rapier))]
        [XmlArrayItem(ElementName = "Curved", Type = typeof(Weapon.Curved))]
        [XmlArrayItem(ElementName = "Mace", Type = typeof(Weapon.Mace))]
        [XmlArrayItem(ElementName = "Axe", Type = typeof(Weapon.Axe))]
        [XmlArrayItem(ElementName = "Dagger", Type = typeof(Weapon.Dagger))]
        [XmlArrayItem(ElementName = "Shield", Type = typeof(Shield))]
        [XmlArrayItem(ElementName = "Warhammer", Type = typeof(Weapon.Warhammer))]
        [XmlArrayItem(ElementName = "Greatsword", Type = typeof(Weapon.Greatsword))]
        [XmlArrayItem(ElementName = "Longaxe", Type = typeof(Weapon.Longaxe))]
        [XmlArrayItem(ElementName = "Halberd", Type = typeof(Weapon.Halberd))]
        [XmlArrayItem(ElementName = "Pike", Type = typeof(Weapon.Pike))]
        [XmlArrayItem(ElementName = "Staff", Type = typeof(Weapon.Staff))]
        [XmlArrayItem(ElementName = "Bow", Type = typeof(Weapon.Bow))]
        [XmlArrayItem(ElementName = "Crossbow", Type = typeof(Weapon.Crossbow))]
        [XmlArrayItem(ElementName = "Pistol", Type = typeof(Weapon.Pistol))]
        [XmlArrayItem(ElementName = "Rifle", Type = typeof(Weapon.Rifle))]
        [XmlArrayItem(ElementName = "Special", Type = typeof(Special))]
        public List<Weapon> Weapons;
        
        private string _htmlString = "";

        public string ToShortHtml()
        {
            if (_htmlString.Length == 0)
            {
                var result = new StringBuilder("\n");
                using (new TableRow(result))
                {
                    using (new TableDiv(result))
                    using (new Div(result, "class='autoSpace p100'"))
                    {
                        result.AppendLine($"<img class='autoSpace p100' src='./Images/{Id:D2}.jpg'>");
                    }

                    using (new TableDiv(result))
                    {
                        result.AppendLine($"<div><b>{Name}</b></div>");
                        result.AppendLine($"<a href='./Warbands/{Id:D2}.html'>");
                        result.AppendLine($"<div>({ShortName})</div>");
                        result.AppendLine("</a>");
                    }

                    using (new TableDiv(result))
                    {
                        result.AppendLine(Hook);
                    }

                    using (new TableDiv(result))
                    {
                        using var list = new UnorderedList(result);
                        if (!string.IsNullOrEmpty(Traits.Noise))
                            using (new ListItem(result))
                                result.AppendLine($"<b>{Traits.Noise}</b>");
                        using (new ListItem(result))
                            result.AppendLine(Traits.Values);
                        using (new ListItem(result))
                            result.AppendLine(Traits.Feelings);
                        using (new ListItem(result))
                            result.AppendLine(Traits.Combat);
                        if (!string.IsNullOrEmpty(Traits.Equip))
                            using (new ListItem(result))
                                result.AppendLine($"<b>{Traits.Equip}</b>");
                    }

                    using (new TableDiv(result))
                    {
                        foreach (var weapon in Weapons)
                        {
                            result.Append("<span class='autoSpace'>");
                            result.Append($"<img class='inBlock fit6' src='./Images/weapons/{weapon.ImageName}.png'>");
                            result.Append("</span>");
                        }
                    }
                }

                _htmlString = result.ToString();
            }

            return _htmlString;
        }

        #region inner classes
        public record Trait
        {
            public string Values = "???";
            public string Feelings = "???";
            public string Combat = "???";
            public string? Equip;
            public string? Noise;
        }

        public record Group
        {
            [XmlAttribute]
            public string? Name;

            [XmlElement("Role")]
            public List<Role> Roles = new List<Role>();

            private string _htmlString = "";

            public string ToHtml()
            {
                if (_htmlString.Length == 0)
                {
                    var result = new StringBuilder("\n");
                    using (new Div(result, "class='card'"))
                    {
                        using (new Div(result, "class='card-header'"))
                        {
                            using (new Label(result, "class='control-label'"))
                                result.Append(this.Name ?? "<i>(Main Group)</i>");
                        }

                        using (new Div(result, "class='card-body'"))
                        {
                            foreach (var role in Roles)
                                result.AppendLine(role.ToHtml());
                        }
                    }

                    _htmlString = result.ToString();
                }

                return _htmlString;
            }
        }

        public abstract record ListElement
        {
            [XmlAttribute]
            public string? Type;

            public ListElement()
            {
            }

            public ListElement(string? type)
            {
                Type = type;
            }
        }

        public abstract record ColourDetail
        {
            [XmlText]
            public string Name;

            [XmlAttribute]
            public string code;
        }

        public record Primary : ColourDetail;
        public record Secondary : ColourDetail;
        public record Tertiary : ColourDetail;
        public record Quarternary : ColourDetail;

        public record Colour
        {
            public Primary Primary;
            public Secondary Secondary;
            public Tertiary? Tertiary;
            public Quarternary? Quarternary;
        }
        #endregion
    }
}