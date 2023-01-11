using System.Xml.Serialization;

namespace SwordcraftBrisbane.Data
{
    public partial record Warband
    {
        public record Role : ListElement
        {
            [XmlText]
            public string Name;

            public Role() : base()
            {
                Name = string.Empty;
            }

            public Role(string name, string? type = null) : base(type)
            {
                Name = name;
            }

            public string ToHtml()
            {
                return $"<div class='card-text'><label>{Name}</label></div>";
            }

            public static Role LightInfantry => new("Infantry (Light)");
            public static Role HeavyInfantry => new("Infantry (Heavy)");
            public static Role Archer => new("Archer");
            public static Role Arbalist => new("Arbalist");
            public static Role Gunner => new("Gunner");
            public static Role Healer => new("Healer");
            public static Role Priest => new("Priest");
            public static Role Mage => new("Mage");
        }
    }
}
