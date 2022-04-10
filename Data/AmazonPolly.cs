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
         * If the text has already been translated, nothing will happen
         */
        public static void createTextToSpeechAudio(string text, Language language)
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
                speechReq.Engine = voiceReq.Engine = Engine.Standard;
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
                    default:
                        return;
                }
                

                // Send the voice request and retrieve the voiceId for the speech request
                DescribeVoicesResponse voiceResp = client.DescribeVoicesAsync(voiceReq).Result;
                foreach (Voice voice in voiceResp.Voices)
                {
                    if (voice.SupportedEngines.Contains(Engine.Standard))
                    {
                        speechReq.VoiceId = voice.Id;
                        break;
                    }
                }

                // Send the request and upload the file
                SynthesizeSpeechResponse resp = client.SynthesizeSpeechAsync(speechReq).Result;
                AmazonUploader.uploadAudioFile(resp, text, language);
            }
        }
    }
}
