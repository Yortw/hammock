using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Hammock.WinRT.Compat
{
	public class HMACSHA1 : HashAlgorithm
	{

		public override byte[] ComputeHash(byte[] data)
		{
			if (this.Key == null || this.Key.Length == 0) throw new InvalidOperationException("Key must be set.");

 			var crypt = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
			var buffer = CryptographicBuffer.CreateFromByteArray(data);
			var keyBuffer = CryptographicBuffer.CreateFromByteArray(this.Key);
			var key = crypt.CreateKey(keyBuffer);

			var signedBuffer = CryptographicEngine.Sign(key, buffer);
			
			var retVal = new byte[signedBuffer.Length];
			CryptographicBuffer.CopyToByteArray(signedBuffer, out retVal);
			return retVal;
		}
	}
}
