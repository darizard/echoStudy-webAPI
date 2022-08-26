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
        * Gets a presigned URL for the audio file related to the text and language provided
        * Throws an error if the file doesn't exist
        */
        public static string getPresignedUrl(string text, Language language)
        {
            // Create a GetPresignedUrlRequest and intialize it
            GetPreSignedUrlRequest urlReq = new GetPreSignedUrlRequest();
            urlReq.BucketName = Resources.bucketName;
            urlReq.Key = getFileName(text, language);
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
         * Gets the file name that should be used for a given text and language
         */
        public static string getFileName(string text, Language language)
        {
            string fileName = language.ToString() + " " + text + ".mp3";

            return fileName;
        }
    }
}

