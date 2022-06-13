using System;

namespace WantedPersonsBot
{
    public class WantedPerson : Person
    {
        public string OVD { get; set; }
        public string CATEGORY { get; set; }
        public string LOST_DATE { get; set; }
        public string LOST_PLACE { get; set; }
        public string ARTICLE_CRIM { get; set; }
        public string RESTRAINT { get; set; }
        public string CONTACT { get; set; }

        public WantedPerson(string ID, string FIRST_NAME_U, string LAST_NAME_U, string MIDDLE_NAME_U, string BIRTH_DATE, string SEX, 
            string OVD, string CATEGORY, string LOST_DATE, string LOST_PLACE, string ARTICLE_CRIM, string RESTRAINT, string CONTACT)
            :base(ID, FIRST_NAME_U, LAST_NAME_U, MIDDLE_NAME_U, BIRTH_DATE, SEX)
        {
            this.OVD = OVD;
            this.CATEGORY = CATEGORY;
            this.LOST_DATE = LOST_DATE;
            this.LOST_PLACE = LOST_PLACE;
            this.ARTICLE_CRIM = ARTICLE_CRIM;
            this.RESTRAINT = RESTRAINT;
            this.CONTACT = CONTACT;
        }

        public string Print()
        {
            return "\nПІБ: " + FIRST_NAME_U + " " + LAST_NAME_U + " " + MIDDLE_NAME_U + "\nСТАТЬ: " + SEX +
                "\nДАТА НАРОДЖЕННЯ: " + DateTime.Parse(BIRTH_DATE).ToString("dd.MM.yyyy") +
                "\nВІДДІЛ ПОЛІЦІЇ: " + OVD + "\nКАТЕГОРІЯ: " + CATEGORY + "\nДАТА ЗНИКНЕННЯ: " + DateTime.Parse(LOST_DATE).ToString("dd.MM.yyyy") + 
                "\nМІСЦЕ ЗНИКНЕННЯ: " + LOST_PLACE + "\nСТАТТЯ: " + ARTICLE_CRIM + 
                "\nМІРА ПОКАРАННЯ: " + RESTRAINT + "\nКОНТАКТИ: " + CONTACT;
        }

        public string PrintHtml()
        {
            return "\n<b>ПІБ</b>: " + FIRST_NAME_U + " " + LAST_NAME_U + " " + MIDDLE_NAME_U + "\n<b>СТАТЬ</b>: " + SEX +
                "\n<b>ДАТА НАРОДЖЕННЯ</b>: " + DateTime.Parse(BIRTH_DATE).ToString("dd.MM.yyyy") +
                "\n<b>ВІДДІЛ ПОЛІЦІЇ</b>: " + OVD + "\n<b>КАТЕГОРІЯ</b>: " + CATEGORY + "\n<b>ДАТА ЗНИКНЕННЯ</b>: " + DateTime.Parse(LOST_DATE).ToString("dd.MM.yyyy") +
                "\n<b>МІСЦЕ ЗНИКНЕННЯ</b>: " + LOST_PLACE + "\n<b>СТАТТЯ</b>: " + ARTICLE_CRIM +
                "\n<b>МІРА ПОКАРАННЯ</b>: " + RESTRAINT + "\n<b>КОНТАКТИ</b>: " + CONTACT;
        }
    }
}
