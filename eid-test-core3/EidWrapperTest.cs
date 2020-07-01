using Egelke.Eid.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.IO;

namespace Egelke.Eid.Client.Test
{
    
    
    /// <summary>
    ///This is a test class for EidWrapperTest and is intended
    ///to contain all EidWrapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class EidWrapperTest
    {
        private static Readers readers;

        //private static Card card;

        //private static EventWaitHandle newCard = new AutoResetEvent(false);

        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            readers = new Readers(ReaderScope.User);
        }

        public static void Cleanup()
        {
            readers.Dispose();
        }


        [TestMethod]
        [Timeout(60000)]
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
            Assert.AreEqual(typeof(EidCard), card.GetType());
        }


        /// <summary>
        ///A test for ReadCertificate
        ///</summary>
        [TestMethod]
        public void ReadProperties()
        {
            EidCard target = (EidCard)readers.ListCards().Where(c => c is EidCard).FirstOrDefault();
            using (target)
            {
                target.Open();
                X509Certificate2 auth = target.AuthCert;
                X509Certificate2 sign = target.SignCert;
                X509Certificate2 ca = target.CaCert;
                X509Certificate2 root = target.RootCert;
                X509Certificate2 rrn = target.RrnCert;
                Image pic = Image.FromStream(new MemoryStream(target.Picture));
                Model.Address address = target.Address;
                Model.Identity identity = target.Identity;

                Assert.AreNotEqual(auth.Subject, sign.Subject);
                Assert.AreEqual(sign.Issuer, ca.Subject);
                Assert.AreEqual(auth.Issuer, ca.Subject);
                Assert.AreEqual(ca.Issuer, root.Subject);
                Assert.AreEqual(root.Issuer, root.Subject);
                Assert.AreEqual(rrn.Issuer, root.Subject);
                Assert.AreEqual(new Size(140, 200), pic.Size);
                Assert.IsTrue(address.StreetAndNumber.Length > 0);
                Assert.IsTrue(address.Zip.Length >= 4);
                Assert.IsTrue(address.Municipality.Length > 0);

                Assert.IsTrue(identity.CardNr.Length == 12);
                Assert.IsTrue(identity.ChipNr.Length > 10);
                Assert.IsTrue(identity.ValidityBeginDate > DateTime.Now.AddYears(-10));
                Assert.IsTrue(identity.ValidityEndDate < DateTime.Now.AddYears(10));
                Assert.IsTrue(identity.IssuingMunicipality.Length > 0);
                Assert.IsTrue(identity.NationalNr.Length == 11);
                Assert.IsTrue(auth.Subject.Contains(identity.Surname));
                Assert.IsTrue(auth.Subject.Contains(identity.FirstNames));
                Assert.IsTrue(identity.FirstLetterOfThirdGivenName.Length == 1);
                Assert.IsTrue(identity.Nationality.Length > 0);
                Assert.IsTrue(identity.Nationality.Length > 0);
                Assert.IsTrue(identity.LocationOfBirth.Length > 0);
                Assert.IsTrue(identity.DateOfBirth < DateTime.Now);
                Assert.IsNotNull(identity.Gender);
                Assert.IsNotNull(identity.Nobility);
                Assert.IsNotNull(identity.DocumentType);
                Assert.IsNotNull(identity.SpecialStatus);
            }
        }


        [TestMethod]
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
                    Assert.AreNotEqual(0, fileData.Length);
                }
                Assert.AreEqual(10, i);
            }
        }


    }
}
