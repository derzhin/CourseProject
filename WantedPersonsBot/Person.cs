
namespace WantedPersonsBot
{
    public class Person
    {
        public string ID { get; set; }
        public string FIRST_NAME_R { get; set; }
        public string LAST_NAME_R { get; set; }
        public string MIDDLE_NAME_R { get; set; }
        public string FIRST_NAME_E { get; set; }
        public string LAST_NAME_E { get; set; }
        public string MIDDLE_NAME_E { get; set; }
        public string FIRST_NAME_U { get; set; }
        public string LAST_NAME_U { get; set; }
        public string MIDDLE_NAME_U { get; set; }
        public string BIRTH_DATE { get; set; }
        public string SEX { get; set; }
        public string PHOTOID { get; set; }

        public Person(string ID, string FIRST_NAME_U, string LAST_NAME_U, string MIDDLE_NAME_U, string BIRTH_DATE, string SEX,
            string FIRST_NAME_E = "", string LAST_NAME_E = "", string MIDDLE_NAME_E = "", string FIRST_NAME_R = "", 
            string LAST_NAME_R = "", string MIDDLE_NAME_R = "", string PHOTOID = "")
        {
            this.ID = ID;
            this.FIRST_NAME_U = FIRST_NAME_U;
            this.LAST_NAME_U = LAST_NAME_U;
            this.MIDDLE_NAME_U = MIDDLE_NAME_U;
            this.BIRTH_DATE = BIRTH_DATE;
            this.SEX = SEX;
            this.FIRST_NAME_E = FIRST_NAME_E;
            this.LAST_NAME_E = LAST_NAME_E;
            this.MIDDLE_NAME_E = MIDDLE_NAME_E;
            this.FIRST_NAME_R = FIRST_NAME_R;
            this.LAST_NAME_R = LAST_NAME_R;
            this.MIDDLE_NAME_R = MIDDLE_NAME_R;
            this.PHOTOID = PHOTOID;
        }

        public bool IsEqualID(string ID)
        {
            return this.ID == ID;
        }
    }
}
