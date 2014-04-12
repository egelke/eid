.Net eID Client
===

Native .Net client of the Belgium eID card, directly uses the WinSCard library of Windows.  It allows to read the files ont the Belgium eID card.

Installing
---
Install the Egelke.Eid.Client NuGet package:

    PM> Install-Package Egelke.Eid.Client -Pre 

Prerequisites
---
* Compatible reader
* Fedict minidriver (part of eID Middleware)
* Resent windows (tested on Windows 8.1)

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

License
---
GNU Lesser General Public License
