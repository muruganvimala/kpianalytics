using System;
namespace BusinessIntelligence_API.Service
{
	public class EncodeandDecode
	{
		public static string EncodePassword(string password)
		{			
			try
			{
				// Convert the password string to byte array
				byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(password);

				// Encode the byte array to Base64 string
				return Convert.ToBase64String(bytesToEncode);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static string DecodePassword(string base64EncodedPassword)
		{
			// Decode the Base64 encoded password to get the actual password
			try
			{
				byte[] bytesToDecode = Convert.FromBase64String(base64EncodedPassword);
				return System.Text.Encoding.UTF8.GetString(bytesToDecode);
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
