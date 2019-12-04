using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordHasher
{
    class Program
    {
        public static void Main(string[] args)
        {

            //string hassedPw = "AErbImYMthiY/MTaOV3uH3JWbfu0VtA2NX5KBa6QfV5w40157aMLN8QAVJDw8jymeQ==";
            //string beforehashPw = "wP]cU,?iJ)TpH3j#5C@P,5GZf?33f}";


            //string hassedPw = "ALjDQR0qXr1F8Xp3hcSnGjYzXiA0tmwDNpq+oKU0fVYlAzHy7JL6+VkdHn1Rp+O2TA==";
            string beforehashPw = "Passw0rd#01";
            string password2bhashed = "Passw0rd#01";

			// hash password
            string hashedPassword = HashPassword(password2bhashed);

            if (VerifyHashedPassword(hashedPassword, beforehashPw))
                Console.WriteLine("password is good");
            else
                Console.WriteLine("bad password");
            
            CustomPassword.CheckPassword("Pwd@1234");

            string password = "Test#1234";
            string myhashedpassword = HashPassword(password);
            
            //CustomPassword.hashPassword(password); 
            bool matched = testHash(myhashedpassword, password);

			Console.ReadKey();
        }

        private static bool testHash(string hasedString, string password)
        {
            bool matched = false; 
            try
            {
                 if (VerifyHashedPassword(hasedString, password))
                {
                    matched = true; 
                }
            }
            catch (Exception)
            {

                throw new NotImplementedException();
            }
            return matched; 
            
        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

			// Rfc2898DeriveBytes takes a password, a salt, and an iteration count, and then
			// genrates keys through calls to the GetBytes method
			// 0x10 = 16, 0x3e8 = 1000, 0x20 = 32 
			using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];

			// Buffer.BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count): 
			// Copies a specified number of bytes from a source array starting at a particular offset to a destination array starting at a particular offset.

			// copy 16 digits salt to dst, leave the first digit as 0
			Buffer.BlockCopy(salt, 0, dst, 1, 0x10);

			// copy 32 digits buffer2 to dst after salt
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

			// now dst = 0 + salt (16 digits) + buffer2 (32 digits)
            return Convert.ToBase64String(dst);
        }
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] src = Convert.FromBase64String(hashedPassword);

			// if the length of src is not 49, or the first is not zero, then false
			if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }

			// dst = salt
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);

			// buffer3 = buffer2 in the original bytes
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);

			// check if two bytes are equal
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }
        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }

    public static class CustomPassword
    {
        public static string hashPassword(string password)
        {
            // Encode password & generate password salt

            // encoding a set of characters into a sequence of bytes
            byte[] passwordData = Encoding.Unicode.GetBytes(password);

            // using a array with 16 random bytes as salt
            byte[] passwordSaltData = _PasswordSaltGenerate();

            byte[] passwordDataSalted = new byte[passwordData.Length + passwordSaltData.Length];

            // Put password & salt together
            Buffer.BlockCopy(passwordSaltData, 0, passwordDataSalted, 0, passwordSaltData.Length);
            Buffer.BlockCopy(passwordData, 0, passwordDataSalted, passwordSaltData.Length, passwordData.Length);

            // Hash password & salt
            HashAlgorithm hash = HashAlgorithm.Create("SHA1");
            byte[] passwordHashed = hash.ComputeHash(passwordDataSalted);

            // Encode hashed password/salt & salt for storage
            string passwordFinal = Convert.ToBase64String(passwordHashed);
            string passwordSaltFinal = Convert.ToBase64String(passwordSaltData);
            Console.WriteLine(string.Format("Password hased string is {0}", passwordFinal));
            Console.WriteLine(string.Format("Password salt string is {0}", passwordSaltFinal));
            return passwordFinal; 

        }

        // generate sale with random values
        private static byte[] _PasswordSaltGenerate()
        {
            RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
            byte[] retVal = new byte[16];

            // Generate salt
            // fill the array with a random value
            rnd.GetBytes(retVal);

            return retVal;
        }

        public static bool CheckPassword(string password)
        {
            HashAlgorithm hash = HashAlgorithm.Create("SHA1");
            bool matchFound = false;

            string passwordOldEncoded = "N99Kd9wyWz1/UbQYXle8d6QtUFY=";
            string passwordSaltEncoded = "hGGfEhZ5TodPW+P6SkpF6g==";

            // Encode new password & decode old salt
            byte[] passwordData = Encoding.Unicode.GetBytes(password);
            byte[] passwordSaltData = Convert.FromBase64String(passwordSaltEncoded);

            byte[] passwordDataSalted = new byte[passwordData.Length + passwordSaltData.Length];

            // Apply old salt to new password
            Buffer.BlockCopy(passwordSaltData, 0, passwordDataSalted, 0, passwordSaltData.Length);
            Buffer.BlockCopy(passwordData, 0, passwordDataSalted, passwordSaltData.Length, passwordData.Length);

            // Hash new password w/ old salt & encode
            byte[] passwordHashed = hash.ComputeHash(passwordDataSalted);
            string passwordFinal = Convert.ToBase64String(passwordHashed);

            // Compare new password w/ old salt (hashed & encoded) against old password w/ old salt (hashed & encoded)
            matchFound = (passwordFinal == passwordOldEncoded);
            return matchFound; 
        }
    }
}
