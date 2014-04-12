using Egelke.Eid.Client;
using NUnit.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Egelke.Eid.Client.Test
{
    
    
    /// <summary>
    ///This is a test class for EidWrapperTest and is intended
    ///to contain all EidWrapperTest Unit Tests
    ///</summary>
    [TestFixture]
    public class EidWrapperTest
    {

        [Test, ExpectedException(typeof(ReaderException))]
        public void ReadNotFound()
        {
            EidReader target = new EidReader("This reader that can't be found!");
        }

        /// <summary>
        ///A test for ReadCertificate
        ///</summary>
        [Test]
        public void ReadCertificates()
        {
            String[] readers = EidReader.Readers;
            if (readers.Length != 1) Assert.Inconclusive("Can't select a reader, " + readers.Length + " present: " + String.Join(", ", readers));

            EidReader target = new EidReader(readers[0]);
            using (target)
            {
                target.CardAction += new EventHandler<DeviceEventArgs>(target_CardAction);
                target.Connect();

                X509Certificate2 auth = target.ReadCertificate(Certificate.Authentication);
                X509Certificate2 sign = target.ReadCertificate(Certificate.Signature);
                X509Certificate2 ca = target.ReadCertificate(Certificate.CA);
                X509Certificate2 root = target.ReadCertificate(Certificate.Root);

                Assert.AreNotEqual(auth.Subject, sign.Subject);
                Assert.AreEqual(sign.Issuer, ca.Subject);
                Assert.AreEqual(auth.Issuer, ca.Subject);
                Assert.AreEqual(ca.Issuer, root.Subject);
                Assert.AreEqual(root.Issuer, root.Subject);
            }
        }

        EventWaitHandle waitChange = new EventWaitHandle(false, EventResetMode.AutoReset);

        [Test, Explicit]
        public void CardChange()
        {
            EidReader target = new EidReader("ACS CCID USB Reader 0");
            using (target)
            {
                target.CardAction += new EventHandler<DeviceEventArgs>(target_CardAction);
                target.ReaderAction += new EventHandler<DeviceEventArgs>(target_ReaderAction);

                //Wait for 2 events...
                waitChange.WaitOne(new TimeSpan(0, 1, 0));
                waitChange.WaitOne(new TimeSpan(0, 1, 0));
            }
        }

        void target_ReaderAction(object sender, DeviceEventArgs e)
        {
            System.Console.Out.WriteLine(String.Format("Reader {2} status changed from {0} to {1}", e.PreviousState, e.NewState, e.DeviceName));
            waitChange.Set();
        }

        void target_CardAction(object sender, DeviceEventArgs e)
        {
            System.Console.Out.WriteLine(String.Format("Card {2} status changed from {0} to {1}", e.PreviousState, e.NewState, e.DeviceName));
            waitChange.Set();
        }
    }
}
