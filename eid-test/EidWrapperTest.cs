using Egelke.Eid.Client;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.IO;
using Xunit;
using System.Xml;
using System.Globalization;

namespace Egelke.Eid.Client.Test
{
    
    
    /// <summary>
    ///This is a test class for EidWrapperTest and is intended
    ///to contain all EidWrapperTest Unit Tests
    ///</summary>
    public class EidWrapperTest : IClassFixture<ReadersFixture>
    {
        private Readers readers;

        public EidWrapperTest(ReadersFixture readersFixture)
        {
            readers = readersFixture.Readers;
        }


        [Fact(Timeout = 60000)]
        public void AAA_WaitForEid()
        {
            var newCard = new AutoResetEvent(false);
            Card card = readers.ListCards().Where(c => c is EidCard).FirstOrDefault();
            readers.CardInsert += (s, e) =>
            {
                if (e.Card is EidCard) {
                    card = e.Card;
                    newCard.Set();
                }
            };
            readers.StartListen();

            if (card == null)
            {
                newCard.WaitOne();
            }
            Assert.Equal(typeof(EidCard), card.GetType());
        }


        /// <summary>
        ///A test for ReadCertificate
        ///</summary>
        [Fact]
        public void ReadProperties()
        {
            //prep
            EidCard target = (EidCard)readers.ListCards().Where(c => c is EidCard).FirstOrDefault();

            //exec
            X509Certificate2 auth;
            X509Certificate2 sign;
            X509Certificate2 ca;
            X509Certificate2 root;
            X509Certificate2 rrn;
            Image pic;
            Model.Address address;
            Model.Identity identity;
            using (target)
            {
                target.Open();
                auth = target.AuthCert;
                sign = target.SignCert;
                ca = target.CaCert;
                root = target.RootCert;
                rrn = target.RrnCert;
                pic = Image.FromStream(new MemoryStream(target.Picture));
                address = target.Address;
                identity = target.Identity;
            }

            //verify
            Assert.NotEqual(auth.Subject, sign.Subject);
            Assert.Equal(sign.Issuer, ca.Subject);
            Assert.Equal(auth.Issuer, ca.Subject);
            Assert.Equal(ca.Issuer, root.Subject);
            Assert.Equal(root.Issuer, root.Subject);
            Assert.Equal(rrn.Issuer, root.Subject);

            //loads the eID-Viewer export file (put yours in the root if you want to test with your eID)
            XmlDocument eidExp = new XmlDocument();
            eidExp.Load(identity.CardNr + ".eid");

            Image refPic = Image.FromStream(new MemoryStream(Convert.FromBase64String(eidExp.SelectSingleNode("/eid/identity/photo").InnerText)));
            Assert.Equal(refPic.Size, pic.Size);

            Assert.Equal(eidExp.SelectSingleNode("/eid/address/streetandnumber").InnerText.TrimEnd(), address.StreetAndNumber);
            Assert.Equal(eidExp.SelectSingleNode("/eid/address/zip").InnerText, address.Zip);
            Assert.Equal(eidExp.SelectSingleNode("/eid/address/municipality").InnerText, address.Municipality);

            Assert.Equal(eidExp.SelectSingleNode("/eid/card/@cardnumber").InnerText, identity.CardNr);
            Assert.Equal(eidExp.SelectSingleNode("/eid/card/@chipnumber").InnerText, BitConverter.ToString(identity.ChipNr).Replace("-", ""));
            Assert.Equal(DateTime.ParseExact(eidExp.SelectSingleNode("/eid/card/@validitydatebegin").InnerText, "yyyyMMdd", CultureInfo.InvariantCulture), identity.ValidityBeginDate);
            Assert.Equal(DateTime.ParseExact(eidExp.SelectSingleNode("/eid/card/@validitydateend").InnerText, "yyyyMMdd", CultureInfo.InvariantCulture), identity.ValidityEndDate);
            Assert.Equal(eidExp.SelectSingleNode("/eid/card/deliverymunicipality").InnerText, identity.IssuingMunicipality);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/@nationalnumber").InnerText, identity.NationalNr);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/name").InnerText, identity.Surname);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/firstname").InnerText, identity.FirstNames);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/middlenames").InnerText, identity.FirstLetterOfThirdGivenName);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/nationality").InnerText, identity.Nationality);
            Assert.Equal(eidExp.SelectSingleNode("/eid/identity/placeofbirth").InnerText, identity.LocationOfBirth);
            Assert.Equal(DateTime.ParseExact(eidExp.SelectSingleNode("/eid/identity/@dateofbirth").InnerText, "yyyyMMdd", CultureInfo.InvariantCulture), identity.DateOfBirth);

            //TODO, make a little more resilient
            Assert.NotNull(identity.Gender);
            Assert.NotNull(identity.Nobility);
            Assert.NotNull(identity.DocumentType);
            Assert.NotNull(identity.SpecialStatus);
        }


        [Fact]
        public void ReadAllFiles()
        {
            EidCard target = (EidCard)readers.ListCards().Where(c => c is EidCard).FirstOrDefault();
            using (target)
            {
                target.Open();
                int i = 0;
                foreach (EidFile fileId in Enum.GetValues(typeof(EidFile)))
                {
                    i++;
                    byte[] fileData = target.ReadRaw(fileId);
                    Assert.NotEmpty(fileData);
                }
                Assert.Equal(10, i);
            }
        }


    }
}
