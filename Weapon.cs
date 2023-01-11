using System.Xml.Serialization;

namespace SwordcraftBrisbane.Data
{
    public partial record Warband
    {
        public record Blank : Weapon
        {
            public Blank() : base() { }

            public Blank(string? type) : base(type)
            {
            }

            protected override string Prefix => "a";
        }

        public abstract record RangedWeapon : Weapon
        {
            public RangedWeapon() : base() { }

            public RangedWeapon(string? type) : base(type)
            {
            }

            protected override string Prefix => "r";
        }

        public record Shield : Weapon
        {
            public Shield() : base() { }

            public Shield(string? type) : base(type)
            {
            }

            protected override string Prefix => "s";

            public override string ImageName => $"{Prefix}_{(Type??"round").ToLower()}";
        }

        public record Special : Weapon
        {
            public Special() : base() { }

            public Special(string? type) : base(type)
            {
            }

            protected override string Prefix => "x";

            public override string ImageName => Type == null
                ? "a_default" : $"{Prefix}_{Type.ToLower()}";
        }

        public abstract record Weapon : ListElement
        {
            // todo: add images

            public Weapon() : base() { }

            public Weapon(string? type) : base(type)
            {
            }

            public string DisplayName
            {
                get
                {
                    if (!string.IsNullOrEmpty(Type))
                    {
                        return $"{GetType().Name} ({Type})";
                    }

                    return GetType().Name;
                }
            }

            protected virtual string Prefix => "m";

            public virtual string ImageName
                => $"{Prefix}_{GetType().Name.ToLower()}";

            public string ToHtml()
            {
                var result = $"\n<span name='weapon' class='autoSpace'>" +
                    $"<img class='inBlock p100' alt='{DisplayName}' " +
                    $"src='../Images/weapons/{ImageName}.png'/>" +
                    $"</span>";
                return result;
            }

            public record Sword : Weapon;
            public record Rapier : Weapon;
            public record Curved : Weapon;
            public record Mace : Weapon;
            public record Axe : Weapon;
            public record Dagger : Weapon;
            public record Warhammer : Weapon;
            public record Greatsword : Weapon;
            public record Longaxe : Weapon;
            public record Halberd : Weapon;
            public record Pike : Weapon;
            public record Staff : Weapon;
            public record Bow : RangedWeapon;
            public record Crossbow : RangedWeapon;
            public record Pistol : RangedWeapon;
            public record Rifle : RangedWeapon;
        }
    }
}
