using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.S3;
using Amazon.S3.Model;
using echoStudy_webAPI.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Data
{
    /**
     * Used for uploading files to Amazon's S3 service
     */
    public static class AmazonUploader
    {
        private static AmazonS3Client client = new AmazonS3Client(Resources.AWSAccessKeyId, EncryptionHelper.Decrypt(Resources.AWSSecretKey), RegionEndpoint.USWest2);

        /**
         * Uploads the audio stream associated with the SynthesizeSpeechResponse to an s3 bucket with a key name 
         */
        public static void uploadAudioFile(SynthesizeSpeechResponse speechResp, string text, Language language)
        {
            // The bytes of the file
            byte[] fileBytes = ToArrayBytes(speechResp.AudioStream);

            // Create a PutObjectRequest and initialize its parameters
            PutObjectRequest objReq = new PutObjectRequest();
            objReq.BucketName = Resources.bucketName;
            objReq.InputStream = new MemoryStream(fileBytes);
            objReq.Key = getFileName(text, language);
            objReq.ContentType = "audio/mpeg";

            // Send the request to upload the file
            PutObjectResponse objResp = client.PutObjectAsync(objReq).Result;
        }

        /**
        * Uploads the custom audio file provided by the given user.
        */
        public static string uploadAudioFile(byte[] fileBytes, string key)
        {
            // Create a PutObjectRequest and initialize its parameters
            PutObjectRequest objReq = new PutObjectRequest();
            objReq.BucketName = Resources.bucketName;
            objReq.InputStream = new MemoryStream(fileBytes);
            objReq.Key = getFileName(key);
            objReq.ContentType = "audio/mpeg";

            // Send the request to upload the file
            PutObjectResponse objResp = client.PutObjectAsync(objReq).Result;
            return objReq.Key;
        }

        /**
         * Deletes an audio file from S3 given text and language
         */
        public static void deleteAudioFile(string text, Language language)
        {
            // Request to delete file
            DeleteObjectRequest objReq = new DeleteObjectRequest();
            objReq.BucketName = Resources.bucketName;
            objReq.Key = getFileName(text, language);

            // Send the request to delete the file
            DeleteObjectResponse objResp = client.DeleteObjectAsync(objReq).Result;
        }

        /**
        * Deletes a custom audio file from S3 given username, text, and language
        */
        public static void deleteAudioFile(string key)
        {
            // Request to delete file
            DeleteObjectRequest objReq = new DeleteObjectRequest();
            objReq.BucketName = Resources.bucketName;
            objReq.Key = getFileName(key);

            // Send the request to delete the file
            DeleteObjectResponse objResp = client.DeleteObjectAsync(objReq).Result;
        }

        /**
        * Gets a presigned URL for the audio file related to the text and language provided
        * Throws an error if the file doesn't exist
        */
        public static string getPresignedUrl(string text, Language language)
        {
            // Create a GetPresignedUrlRequest and intialize it
            GetPreSignedUrlRequest urlReq = new GetPreSignedUrlRequest();
            urlReq.BucketName = Resources.bucketName;
            urlReq.Key = getFileName(text, language);
            urlReq.Expires = DateTime.Now.AddMinutes(60);
            urlReq.Protocol = Protocol.HTTP; 

            // Send the request to get the url
            return client.GetPreSignedURL(urlReq);
        }

        /**
        * Gets a presigned URL for the custom audio file from the info provided
        */
        public static string getPresignedUrl(string key)
        {
            // Create a GetPresignedUrlRequest and intialize it
            GetPreSignedUrlRequest urlReq = new GetPreSignedUrlRequest();
            urlReq.BucketName = Resources.bucketName;
            urlReq.Key = getFileName(key);
            urlReq.Expires = DateTime.Now.AddMinutes(10);
            urlReq.Protocol = Protocol.HTTP;

            // Send the request to get the url
            return client.GetPreSignedURL(urlReq);
        }

        /**
         * Checks if the given key exists in the given bucket
         */
        public static async Task<bool> fileExists(string bucket, string text, Language language)
        {
            // An error is thrown if it doesn't exist
            // Currently, there seems to be no other way to see if the file exists other than trying to grab it
            try
            {
                await client.GetObjectMetadataAsync(bucket, getFileName(text, language));

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (string.Equals(ex.ErrorCode, "NotFound"))
                {
                    return false;
                }

                throw;
            }
        }

        /**
        * Converts a stream to an array of bytes
        */
        private static byte[] ToArrayBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /**
        * Taken from https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
        * Converts a byte array to a a hexadecimal string
        */
        private static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        /**
         * Gets the file name that should be used for a given term's audio (text and language).
         */
        public static string getFileName(string text, Language language)
        {
            string fileName = language.ToString() + " " + text;

            // Ensure no illegal characters
            fileName = ByteArrayToHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(fileName)));

            return fileName + ".mp3";
        }
        public static string getFileName(string key)
        {
            string fileName = key;

            // Ensure no illegal characters
            fileName = ByteArrayToHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(fileName)));

            return fileName + ".mp3";
        }
    }
}

