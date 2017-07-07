using System.Collections.Generic;

namespace Luppa.Services.BECList
{
    public sealed class BECListViewStateModel
    {
        public string EventTarget { get; set; }
        public string EventArgument { get; set; }
        public string LastFocus { get; set; }
        public string ViewState { get; set; }
        public string ViewStateGenerator { get; set; }
        public string ViewStateEncrypted { get; set; }
        public string EventValidation { get; set; }
        public string CT100ToolkitScript { get; set; }

        public Dictionary<string, string> AddicionalData { get; set; } = 
            new Dictionary<string, string>();

        public string FinalBodyString { get; set; }

        public Dictionary<string, string> GenerateFormData()
        {
            var form = new Dictionary<string, string>
            {
                ["ctl00_ToolkitScriptManager1_HiddenField"] = CT100ToolkitScript,
                ["__EVENTTARGET"] = EventTarget,
                ["__EVENTARGUMENT"] = EventArgument,
                ["__LASTFOCUS"] = LastFocus,
                ["__VIEWSTATE"] = ViewState,
                ["__VIEWSTATEGENERATOR"] = ViewStateGenerator,
                ["__VIEWSTATEENCRYPTED"] = ViewStateEncrypted,
                ["__EVENTVALIDATION"] = EventValidation,
            };

            if (AddicionalData.Count > 0)
            {
                foreach (var item in AddicionalData)
                    form.Add(item.Key, item.Value);
            }

            return form;
        }
    }
}