using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

class CryptoOperations
{

    // Диффи — Хеллман: Обмен ключами (ECDH)
    static void ExecuteKeyExchange()
    {
        Console.WriteLine("\n--- Обмен ключами ECDH (Диффи — Хеллман) ---");
        var timer = Stopwatch.StartNew();

        using (var partyA = new ECDiffieHellmanCng())
        using (var partyB = new ECDiffieHellmanCng())
        {
            partyA.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            partyA.HashAlgorithm = CngAlgorithm.Sha384;
            partyB.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            partyB.HashAlgorithm = CngAlgorithm.Sha384;

            var keyA = partyA.DeriveKeyMaterial(partyB.PublicKey);
            var keyB = partyB.DeriveKeyMaterial(partyA.PublicKey);

            Console.WriteLine("Ключи идентичны: " + keyA.SequenceEqual(keyB));
        }

        Console.WriteLine($"Затрачено времени: {timer.ElapsedMilliseconds} мс");
    }



    // Эль-Гамаль: Шифрование и расшифровка
    static void ExecuteElGamalCrypto()
    {
        Console.WriteLine("\n--- Шифрование ElGamal ---");
        var timer = Stopwatch.StartNew();

        var random = new SecureRandom();
        var paramGen = new ElGamalParametersGenerator();
        paramGen.Init(512, 25, random);

        var parameters = paramGen.GenerateParameters();
        var keyGenParams = new ElGamalKeyGenerationParameters(random, parameters);

        var keyGenerator = new ElGamalKeyPairGenerator();
        keyGenerator.Init(keyGenParams);

        var keyPair = keyGenerator.GenerateKeyPair();
        var publicKey = (ElGamalPublicKeyParameters)keyPair.Public;
        var privateKey = (ElGamalPrivateKeyParameters)keyPair.Private;

        var message = "Секретное сообщение";
        var inputData = Encoding.UTF8.GetBytes(message);

        var engine = new ElGamalEngine();
        engine.Init(true, new ParametersWithRandom(publicKey, random));
        var cipherText = engine.ProcessBlock(inputData, 0, inputData.Length);

        engine.Init(false, privateKey);
        var plainText = engine.ProcessBlock(cipherText, 0, cipherText.Length);

        Console.WriteLine($"Шифр: {Convert.ToBase64String(cipherText)}");
        Console.WriteLine($"Расшифровано: {Encoding.UTF8.GetString(plainText)}");

        Console.WriteLine($"Затрачено времени: {timer.ElapsedMilliseconds} мс");
    }



    // RSA: Асимметричное шифрование
    static void ExecuteRSAOperations()
    {
        Console.WriteLine("\n--- Асимметричное шифрование RSA ---");
        var timer = Stopwatch.StartNew();

        using (var algorithm = RSA.Create(2048))
        {
            var plainText = "Пример текста для RSA";
            var dataBytes = Encoding.UTF8.GetBytes(plainText);

            var encryptedData = algorithm.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
            var decryptedData = algorithm.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);

            Console.WriteLine($"Оригинал: {plainText}");
            Console.WriteLine($"Шифр: {Convert.ToBase64String(encryptedData)}");
            Console.WriteLine($"Расшифровано: {Encoding.UTF8.GetString(decryptedData)}");
        }

        Console.WriteLine($"Затрачено времени: {timer.ElapsedMilliseconds} мс");
    }



    // RSA: Цифровая подпись с использованием алгоритма RSA + SHA-512
    static void ExecuteDigitalSignature()
    {
        Console.WriteLine("\n--- Цифровая подпись (RSA + SHA-512) ---");

        var document = "Важный документ для подписи";
        var documentBytes = Encoding.UTF8.GetBytes(document);
        Console.WriteLine($"Оригинальный документ: {document}");

        using (var rsaProvider = new RSACryptoServiceProvider(2048))
        {
            var publicKey = rsaProvider.ToXmlString(false);
            var privateKey = rsaProvider.ToXmlString(true);

            byte[] signature;
            using (var signer = new RSACryptoServiceProvider())
            {
                signer.FromXmlString(privateKey);
                signature = signer.SignData(documentBytes, CryptoConfig.MapNameToOID("SHA512"));
            }

            Console.WriteLine($"Подпись: {Convert.ToBase64String(signature)}");

            bool isValidOriginal;
            using (var verifier = new RSACryptoServiceProvider())
            {
                verifier.FromXmlString(publicKey);
                isValidOriginal = verifier.VerifyData(documentBytes, CryptoConfig.MapNameToOID("SHA512"), signature);
            }

            Console.WriteLine(isValidOriginal
                ? "✅ Подпись оригинального документа: ПОДТВЕРЖДЕНА"
                : "❌ Подпись оригинального документа: НЕВЕРНА");

            // подделка
            var tamperedDocument = "Важный документ для подписи";
            var tamperedBytes = Encoding.UTF8.GetBytes(tamperedDocument);
            Console.WriteLine($"\n🚨 Подделанный документ: {tamperedDocument}");

            bool isValidTampered;
            using (var verifier = new RSACryptoServiceProvider())
            {
                verifier.FromXmlString(publicKey);
                isValidTampered = verifier.VerifyData(tamperedBytes, CryptoConfig.MapNameToOID("SHA512"), signature);
            }

            Console.WriteLine(isValidTampered
                ? " ✅Подпись подделанного документа: ПОДТВЕРЖДЕНА"
                : " ❌Подпись подделанного документа: НЕВЕРНА");
        }
    }


    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Демонстрация криптографических операций");

        ExecuteKeyExchange();
        ExecuteElGamalCrypto();
        ExecuteRSAOperations();
        ExecuteDigitalSignature();

        Console.WriteLine("\nВсе операции завершены");
    }
}