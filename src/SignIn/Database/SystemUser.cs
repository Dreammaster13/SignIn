using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignIn;
using Starcounter;
using System.Security.Cryptography;

namespace SignIn
{
    [Database]
    public class SystemUser
    {
        public string Username;
        public string Password;
        public string PasswordSalt;

        //static public SystemUser RegisterSystemUser(string Username, string Email, string Password, out EmailAddress EmailAddress)
        static public SystemUser RegisterSystemUser(string Username, string Email, string Password)
        {
            string hash;
            string salt = GeneratePasswordSalt(16);
            //Person person = new Person();
            //string relationTypeName = "Primary";
            //EmailAddressRelationType type = Db.SQL<EmailAddressRelationType>("SELECT t FROM Simplified.Ring3.EmailAddressRelationType t WHERE t.Name = ?", relationTypeName).First;

            hash = GeneratePasswordHash(Username, Password, salt);

            //if (type == null)
            //{
            //    type = new EmailAddressRelationType()
            //    {
            //        Name = relationTypeName
            //    };
            //}

            //EmailAddress email = new EmailAddress()
            //{
            //    Name = Email
            //};

            //EmailAddressRelation relation = new EmailAddressRelation()
            //{
            //    ContactInfo = email,
            //    Somebody = person,
            //    ContactInfoRelationType = type
            //};

            SystemUser user = new SystemUser()
            {
                Username = Username,
                //WhoIs = person,
                Password = hash,
                PasswordSalt = salt
            };

            //EmailAddress = email;

            return user;
        }

        static public SystemUserSession SignInSystemUser(string Username, string Password)
        {
            string newHash = null;
            SystemUser systemUser = Db.SQL<SystemUser>("SELECT o FROM SignIn.SystemUser o WHERE o.Username = ?", Username).First;

            if (systemUser == null)
            {
                return null;
            }

            string salt = systemUser.PasswordSalt;
            string hash = systemUser.Password;
            bool valid = ValidatePasswordHash(Username, Password, salt, hash, out newHash);

            if (!valid)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(newHash))
            {
                Db.Transact(() =>
                {
                    systemUser.Password = newHash;
                });
            }

            SystemUserSession userSession = null;

            Db.Transact(() =>
            {
                //SystemUserTokenKey token = new SystemUserTokenKey();

                //token.Expires = DateTime.UtcNow.AddDays(TokenValidDays);
                //token.Created = token.LastUsed = DateTime.UtcNow;
                //token.Token = CreateAuthToken(systemUser.Username);
                //token.User = systemUser;

                userSession = AssureSystemUserSession(systemUser);
            });

            return userSession;
        }

        public static string GeneratePasswordSalt(int size)
        {
            string salt;

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[size];

                rng.GetBytes(data);
                salt = Convert.ToBase64String(data);
            }

            return salt;
        }

        static public SystemUserSession SignInSystemUser(string Username)
        {
            SystemUser systemUser = Db.SQL<SystemUser>("SELECT o FROM SignIn.SystemUser o WHERE o.Username = ?", Username).First;

            if (systemUser == null)
            {
                return null;
            }

            //if (!ValidateAuthToken(token))
            //{
            //    return null;
            //}

            SystemUserSession session = null;

            Db.Transact(() =>
            {
                session = AssureSystemUserSession(systemUser);
            });

            return session;
        }

        static private SystemUserSession AssureSystemUserSession(SystemUser systemUser)
        {
            //SystemUser systemUser = Db.SQL<SystemUser>("SELECT o FROM SignIn.SystemUser o WHERE o.Username = ?", Username).First;

            if (systemUser == null)
            {
                return null;
            }

            SystemUserSession userSession = null;

            Db.Transact(() =>
            {
                userSession = Db.SQL<SystemUserSession>("SELECT o FROM SignIn.SystemUserSession o WHERE o.SessionId=? and o.ExpiresAt > ?",
                    Session.Current?.SessionId, DateTime.UtcNow).First;
                if (userSession == null)
                {
                    userSession = new SystemUserSession();
                    userSession.ExpiresAt = DateTime.UtcNow.AddDays(7);
                }
                else
                {
                    userSession.ExpiresAt = DateTime.UtcNow.AddDays(1);
                }
                userSession.User = systemUser;
                userSession.SessionId = Session.Current.SessionId;

            });

            return userSession;
        }

        public static bool ValidatePasswordHash(string userId, string password, string salt, string hash)
        {
            string newHash;
            bool valid = ValidatePasswordHash(userId, password, salt, hash, out newHash);

            return valid;
        }

        private static bool ValidatePasswordHash(string userId, string password, string salt, string hash, out string newHash)
        {
            bool valid = false;
            Scrypt.ScryptEncoder encoder = new Scrypt.ScryptEncoder();
            string newSalt = GetPasswordSalt(userId, password, salt);
            string passwordAndSalt = password + salt;

            try
            {
                if (encoder.Compare(passwordAndSalt, hash))
                {
                    newHash = null;
                    return true;
                }
            }
            catch (ArgumentException ex) when (ex.ParamName == "hashedPassword")
            {
            }

            PasswordHashType[] types = new PasswordHashType[] { PasswordHashType.SHA1PlusSHA256, PasswordHashType.SHA256 };

            foreach (PasswordHashType type in types)
            {
                string h = GeneratePasswordHash(userId, password, salt, type);

                if (h == hash)
                {
                    valid = true;
                    break;
                }
            }

            if (valid)
            {
                newHash = GeneratePasswordHash(userId, password, salt, PasswordHashType.Scrypt);
            }
            else
            {
                newHash = null;
            }

            return valid;
        }

        public static string GeneratePasswordHash(string userId, string password, string salt)
        {
            return GeneratePasswordHash(userId, password, salt, PasswordHashType.Scrypt);
        }

        private static string GeneratePasswordHash(string userId, string password, string salt, PasswordHashType type)
        {
            string hash;
            string newSlat = GetPasswordSalt(userId, password, salt);

            switch (type)
            {
                case PasswordHashType.SHA1PlusSHA256:
                    hash = GenerateSHA1PlusSHA256PasswordHash(password, newSlat);
                    break;
                case PasswordHashType.SHA256:
                    hash = GenerateSHA256PasswordHash(password, newSlat);
                    break;
                case PasswordHashType.Scrypt:
                    hash = GenerateScryptPasswordHash(password, salt);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return hash;
        }

        private static string GetPasswordSalt(string userId, string password, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                salt = userId.ToLowerInvariant() + ":" + password;
            }

            return salt;
        }

        private enum PasswordHashType
        {
            SHA256,
            SHA1PlusSHA256,
            Scrypt
        }

        private static string GenerateScryptPasswordHash(string password, string salt)
        {
            Scrypt.ScryptEncoder encoder = new Scrypt.ScryptEncoder();
            string passwordAndSalt = password + salt;
            string hash = encoder.Encode(passwordAndSalt);

            return hash;
        }

        private static string GenerateSHA256PasswordHash(string password, string salt)
        {
            byte[] passwordData = GetBytes(password);
            byte[] saltData = GetBytes(salt);

            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes = new byte[passwordData.Length + saltData.Length];

            for (int i = 0; i < passwordData.Length; i++)
            {
                plainTextWithSaltBytes[i] = passwordData[i];
            }

            for (int i = 0; i < saltData.Length; i++)
            {
                plainTextWithSaltBytes[passwordData.Length + i] = saltData[i];
            }

            byte[] hashData = algorithm.ComputeHash(plainTextWithSaltBytes);
            string hash = Convert.ToBase64String(hashData);

            return hash;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GenerateSHA1PlusSHA256PasswordHash(string password, string salt)
        {
            string sha1Hash = GenerateSHA1PasswordHash(password);
            string hash = GenerateSHA256PasswordHash(sha1Hash, salt);

            return hash;
        }

        private static string GenerateSHA1PasswordHash(string password)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        static public bool SignOutSystemUser()
        {
            SystemUserSession session = GetCurrentSystemUserSession();

            if (session != null)
            {
                Db.Transact(() =>
                {
                    session.ExpiresAt = DateTime.UtcNow.AddDays(-1);
                });
            }

            return false;
        }

        static public SystemUserSession GetCurrentSystemUserSession()
        {
            if (Session.Current == null)
            {
                return null;
            }

            return Db.SQL<SystemUserSession>("SELECT o FROM SignIn.SystemUserSession o WHERE o.SessionId=? and o.ExpiresAt > ?",
                    Session.Current?.SessionId, DateTime.UtcNow).First;
        }
    }
}