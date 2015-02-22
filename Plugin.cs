using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BluRip
{
    public class PluginPushover : PluginBase
    {
        public PluginPushover()
            : base()
        {
        }

        public override string GetDescription()
        {
            return "Send Pushover Messages at various stages";
        }

        public override string GetName()
        {
            return "Pushover";
        }

        public override string GetVersion()
        {
            return "v0.1";
        }

        // adjust when plugin is called
        public override PluginType getPluginType()
        {
            return PluginType.All;
        }

        public override Type GetSettingsType()
        {
            return typeof(PushoverSettings);
        }

        protected override string FileName
        {
            get { return "pushover.xml"; }
        }

        public override PluginSettingsBase GetNewSettings()
        {
            return new PushoverSettings();
        }

        public override bool EditSettings()
        {
            try
            {
                SettingsWindow sw = new SettingsWindow(Settings);
                sw.ShowDialog();
                if (sw.DialogResult == true)
                {
                    this.settings = new PushoverSettings(sw.Settings);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Process()
        {
            try
            {
                var PushoverParameters = new System.Collections.Specialized.NameValueCollection {
                    { "token", ((PushoverSettings)settings).AppToken},
                    { "user", ((PushoverSettings)settings).UserToken },
                    { "message", ""}
                };
                
                DateTime now = DateTime.Now;
                switch (requestPluginType)
                {
                    case PluginType.BeforeDemux:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has started at " + now);
                        break;

                    case PluginType.AfterDemux:
                        //PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has finished demultiplexing at " + now);
                        break;

                    case PluginType.BeforeAutoCrop:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has started indexing and cropping at " + now);
                        break;

                    case PluginType.AfterAutoCrop:
                        //PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has finished indexing and cropping at " + now);
                        break;

                    case PluginType.BeforeSubtitle:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has started processing subtitles at " + now);
                        break;

                    case PluginType.AfterSubtitle:
                        //PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has finished processing subtitles at " + now);
                        break;

                    case PluginType.BeforeEncode:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has started encoding at " + now);
                        break;

                    case PluginType.AfterEncode:
                        //PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has finished encoding at " + now);
                        break;

                    case PluginType.BeforeMux:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has started multiplexing at " + now);
                        break;

                    case PluginType.AfterMux:
                        //PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has Finished multiplexing at " + now);
                        break;

                    case PluginType.ErrorEncode:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has encountered an error at " + now);
                        break;

                    case PluginType.FinishedAll:
                        PushoverParameters.Set("message", "BluRip Project: " + project.settings.movieTitle + " has successfully compleated at " + now);
                        break;

                }

                using (var client = new System.Net.WebClient())
                {
                    client.UploadValues("https://api.pushover.net/1/messages.json", PushoverParameters);
                }

            }
            catch (Exception)
            {
            }
        }
    }

    public class PushoverSettings : PluginSettingsBase
    {
        public string AppToken = "";
        public string UserToken = "";

        public PushoverSettings(PluginSettingsBase orig)
            : base(orig)
        {
            try
            {
                this.AppToken = ((PushoverSettings)orig).AppToken;
                this.UserToken = ((PushoverSettings)orig).UserToken;
            }
            catch (Exception)
            {
            }
        }

        public PushoverSettings()
            : base()
        {
        }
    }
}
