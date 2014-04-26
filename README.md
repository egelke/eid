.Net eID Client
===

Native .Net client of the Belgium eID card, directly uses the WinSCard library of Windows. It allows to read the files ont the Belgium eID card.

Installing
---

Install the Egelke.Eid.Client NuGet package:

    PM> Install-Package Egelke.Eid.Client -Pre 

Prerequisites
---
* Compatible reader
* Resent Windows OS (tested on Windows 7, 8 & 8.1)

Getting Started
---
    using (Readers readers = new Readers(ReaderScope.User))
    {
        readers.EidCardRequest += readers_EidCardRequest;
        readers.EidCardRequestCancellation += readers_EidCardRequestCancellation;
        EidCard target = readers.WaitForEid(new TimeSpan(0, 5, 0));
        using (target)
        {
            X509Certificate2 auth = target.ReadCertificate(CertificateId.Authentication);
        }
    }

Testing
---

You'll need a compatible reader and eID card to be able to run all tests. Check out the README in the test project folder for testing instructions.

License
---
GNU Lesser General Public License
