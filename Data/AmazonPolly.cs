using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using echoStudy_webAPI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Data
{
    /**
     * Provides methods to create and save audio files
     */
    public static class AmazonPolly
    {
        private static AmazonPollyClient client = new AmazonPollyClient(Resources.AWSAccessKeyId, Resources.AWSSecretKey, RegionEndpoint.USWest2);

        /**
         * Creates an audio file of the given text utilizing Amazon Polly 
         * This audio file will be saved to an S3 bucket
         * If the text has already been translated, nothing will happen.
         * Returns the file name
         */
        public static string createTextToSpeechAudio(string text, Language language)
        {
            // Only fetch the audio from the API if it doesn't already exist
            bool fileExists = AmazonUploader.fileExists(Resources.bucketName, text, language).Result;
            if (!fileExists)
            {
                // Create the requests needed for the audio file request
                SynthesizeSpeechRequest speechReq = new SynthesizeSpeechRequest();
                DescribeVoicesRequest voiceReq = new DescribeVoicesRequest();

                // Initialize the parameters of both requests
                voiceReq.IncludeAdditionalLanguageCodes = false;
                speechReq.Engine = Engine.Neural;
                speechReq.OutputFormat = OutputFormat.Mp3;
                speechReq.Text = text;
                speechReq.TextType = TextType.Text;
                switch (language)
                {
                    case Language.English:
                        speechReq.LanguageCode = voiceReq.LanguageCode = LanguageCode.EnUS;
                        break;
                    case Language.Spanish:
                        speechReq.LanguageCode = voiceReq.LanguageCode = LanguageCode.EsES;
                        break;
                    case Language.German:
                        speechReq.LanguageCode = voiceReq.LanguageCode = LanguageCode.DeDE;
                        break;
                    case Language.Japanese:
                        speechReq.LanguageCode = voiceReq.LanguageCode = LanguageCode.JaJP;
                        break;
                }
                

                // Send the voice request and retrieve the voiceId for the speech request
                DescribeVoicesResponse voiceResp = client.DescribeVoicesAsync(voiceReq).Result;
                bool neuralFound = false;
                foreach (Voice voice in voiceResp.Voices)
                {
                    if (voice.SupportedEngines.Contains(Engine.Neural))
                    {
                        speechReq.VoiceId = voice.Id;
                        neuralFound = true;
                        break;
                    }
                }
                if (!neuralFound)
                {
                    speechReq.Engine = Engine.Standard;
                    speechReq.VoiceId = voiceResp.Voices.First().Id;
                }

                // Send the request
                SynthesizeSpeechResponse resp = client.SynthesizeSpeechAsync(speechReq).Result;

                // Upload the file
                AmazonUploader.uploadAudioFile(resp, text, language);
            }
            // Return the file name
            return AmazonUploader.getFileName(text, language);
        }
    }
}
