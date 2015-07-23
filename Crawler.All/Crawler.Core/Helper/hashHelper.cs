using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;


namespace Crawler.API.Helper
{
	public static class HashHelper
	{
		public static byte[] ObjectToMD5Hash(object input)
		{
			var serializedObject = ObjectToByteArray(input);

			byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(serializedObject);

			return resultArr;
		}

		private static readonly Object locker = new Object();

		private static byte[] ObjectToByteArray(object objectToSerialize)
		{
			MemoryStream fs = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
				lock (locker)
				{
					formatter.Serialize(fs, objectToSerialize);
				}
				return fs.ToArray();
			}
			catch (SerializationException se)
			{
				Console.WriteLine("Error occurred during serialization. Message " + se.Message);
				return null;
			}
			finally
			{
				fs.Close();
			}
		}
}
}