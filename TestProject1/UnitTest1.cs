using System.Collections.Generic;
using Xunit;
using WantedPersonsBot;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void FindPersonByNameTest()
        {
            // Arrange
            WantedPerson wp = new WantedPerson("123", "Name", "LastName", "MiddleName", "22.03.2000", "male", "", "", "", "", "", "", "");
            List<WantedPerson> l = new List<WantedPerson>() { wp };
            TelegramBotMethods.wPersons = l;
            string testStr = "Name LastName MiddleName";

            //Act
            List<WantedPerson> res = TelegramBotMethods.FindWantedPerson(name: testStr);

            //Assert
            Assert.Equal(res[0].ID, wp.ID);
        }
        [Fact]
        public void FindPersonByDateTest()
        {
            // Arrange
            WantedPerson wp = new WantedPerson("123", "Name", "LastName", "MiddleName", "22.03.2000", "male", "", "", "", "", "", "", "");
            List<WantedPerson> l = new List<WantedPerson>() { wp };
            TelegramBotMethods.wPersons = l;
            string testStr = "22.03.2000";

            //Act
            List<WantedPerson> res = TelegramBotMethods.FindWantedPerson(birthdate: testStr);

            //Assert
            Assert.Equal(res[0].ID, wp.ID);
        }
        [Fact]
        public void FindPhotoTest()
        {
            // Arrange
            Photo ph = new Photo();
            ph.ID = "1";
            ph.PHOTOBASE64ENCODE = "123";
            List<Photo> l = new List<Photo>() { ph };
            TelegramBotMethods.photos = l;
            string testStr = "1";

            //Act
            Photo res = TelegramBotMethods.FindPhoto(testStr);

            //Assert
            Assert.Equal(res.ID, ph.ID);
        }
        [Fact]
        public void AddWantedPersonTest()
        {
            // Arrange
            WantedPerson wp = new WantedPerson("123", "Name", "LastName", "MiddleName", "22.03.2000", "male", "", "", "", "", "", "", "");
            TelegramBotMethods.wPersons = new List<WantedPerson>() { wp};
            List<string> l = new List<string>() { "Name LastName MiddleName", "", "", "", "", "", "", "", "", "" };
            string testStr = "Name";

            //Act
            TelegramBotMethods.AddWantedPerson(l);

            //Assert
            Assert.Equal(TelegramBotMethods.wPersons[0].FIRST_NAME_U, testStr);
        }
    }
}
