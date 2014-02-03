using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CodeGenerator
{
    public partial class Cloudents : Form
    {
        const string generatorKey = "genKey";
        const string prefix = "O";
        const string KeyForEncrpt = "Syed Moshiur Murshed";
        const string fileName = "Code.dat";

        int nextCode = 1000;
        private string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public Cloudents()
        {
            InitializeComponent();
            
            appDataFolder = Path.Combine(appDataFolder, "Zbang it");
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            nextCode = LoadFromConfig();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //config.AppSettings.Settings.Add("test", "x");
            //config.Save();



            nextCode = generateNextCode(nextCode);
            labelResult.Text = prefix + nextCode.ToString();
            //ConfigurationManager.AppSettings.Add("test", "x");

        }
        private int generateNextCode(int p)
        {
            bool isValid = false;
            int nextCode = 0;
            do
            {
                p++;
                nextCode = p.ToString().Where((e) => e >= '0' && e <= '9')
                          .Reverse()
                          .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                          .Sum((e) => e / 10 + e % 10);





                isValid = nextCode % 13 == 0;

            } while (!isValid);
            return p;
        }

        private int LoadFromConfig()
        {
            int retVal = 10000;
            var fileLocation = Path.Combine(appDataFolder, fileName);
            if (!File.Exists(fileLocation))
            {
                return retVal;

            }
            var key = File.ReadAllText(fileLocation);
            // var key = Properties.Settings.Default.Code;
            if (string.IsNullOrEmpty(key))
            {
                return retVal;
            }
            //var code = AESEncrypt(nextCode.ToString(),KeyForEncrpt,"IDYNER","SHA1",5,"SHAYELSHELDON286",192);
            var realKey = AESDecrypt(key, KeyForEncrpt, "IDYNER", "SHA1", 5, "SHAYELSHELDON286", 192);
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //AppSettingsSection section = config.AppSettings;

            //if (section != null && section.SectionInformation.IsProtected)
            //{
            //    section.SectionInformation.UnprotectSection();

            //    //config.Save();
            //}

            //var key = section.Settings[generatorKey];
            //if (key == null)
            //{
            //    return retVal;
            //}

            int.TryParse(realKey, out retVal);

            return retVal;

        }

        private void SaveConfig()
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //AppSettingsSection section = config.AppSettings;

            //if (section != null && section.SectionInformation.IsProtected)
            //{
            //    section.SectionInformation.UnprotectSection();

            //    config.Save();
            //}
            //section.Settings.Clear();
            //config.Save(ConfigurationSaveMode.Modified,false);
            //section.Settings.Add(generatorKey, nextCode.ToString());
            //section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

            //config.Save(ConfigurationSaveMode.Modified, false);
            var code = AESEncrypt(nextCode.ToString(), KeyForEncrpt, "IDYNER", "SHA1", 5, "SHAYELSHELDON286", 192);
            var fileLocation = Path.Combine(appDataFolder, fileName);
            File.WriteAllText(fileLocation, code);
            //Properties.Settings.Default["Code"] = code;
            //Properties.Settings.Default.Upgrade();
            //Properties.Settings.Default.Save();


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }

        // <summary>  
        // Encrypts a string          
        // </summary>        
        // <param name="CipherText">Text to be Encrypted</param>         
        // <param name="Password">Password to Encrypt with</param>         
        // <param name="Salt">Salt to Encrypt with</param>          
        // <param name="HashAlgorithm">Can be either SHA1 or MD5</param>         
        // <param name="PasswordIterations">Number of iterations to do</param>          
        // <param name="InitialVector">Needs to be 16 ASCII characters long</param>          
        // <param name="KeySize">Can be 128, 192, or 256</param>          
        // <returns>A decrypted string</returns>       
        public string AESEncrypt(string PlainText, string Password, string Salt, string HashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            if (string.IsNullOrEmpty(PlainText))
            {
                return "The Text to be Decryped by AES must not be null...";
            }
            else if (string.IsNullOrEmpty(Password))
            {
                return "The Password for AES Decryption must not be null...";
            }
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.CBC;

            byte[] CipherTextBytes = null;

            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {

                using (MemoryStream MemStream = new MemoryStream())
                {
                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                    {
                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                        CryptoStream.FlushFinalBlock();
                        CipherTextBytes = MemStream.ToArray();
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            return Convert.ToBase64String(CipherTextBytes);

        }


        // <summary>  
        // Decrypts a string          
        // </summary>        
        // <param name="CipherText">Text to be decrypted</param>         
        // <param name="Password">Password to decrypt with</param>         
        // <param name="Salt">Salt to decrypt with</param>          
        // <param name="HashAlgorithm">Can be either SHA1 or MD5</param>         
        // <param name="PasswordIterations">Number of iterations to do</param>          
        // <param name="InitialVector">Needs to be 16 ASCII characters long</param>          
        // <param name="KeySize">Can be 128, 192, or 256</param>          
        // <returns>A decrypted string</returns>        
        public static string AESDecrypt(string CipherText, string Password, string Salt, string HashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            if (string.IsNullOrEmpty(CipherText))
            {
                return "The Text to be Decryped by AES must not be null...";
            }
            else if (string.IsNullOrEmpty(Password))
            {
                return "The Password for AES Decryption must not be null...";
            }
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
            int ByteCount = 0;
            try
            {

                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {
                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return "Please Enter the Correct Password and Salt..." + "The Following Error Occured: " + "/n" + e;
            }
            SymmetricKey.Clear();
            return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);

        }


    }
}
