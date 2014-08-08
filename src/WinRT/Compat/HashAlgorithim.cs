using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
	public abstract class HashAlgorithm
	{

		public byte[] Key { get; set; }

		public abstract byte[] ComputeHash(byte[] data);

	}
}
