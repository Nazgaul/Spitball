using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class CspViolationReport
    {
        [JsonProperty("csp-report")]
        public CspReport Cspreport { get; set; }


        public override string ToString()
        {
            return $"{nameof(Cspreport)}: {Cspreport}";
        }
    }

    public class CspReport
    {
        [JsonProperty("document-uri")]
        public string Documenturi { get; set; }
        [JsonProperty("referrer")]
        public string Referrer { get; set; }
        [JsonProperty("violated-directive")]
        public string Violateddirective { get; set; }
        [JsonProperty("original-policy")]
        public string Effectivedirective { get; set; }

        [JsonProperty("original-policy")]
        public string Originalpolicy { get; set; }
        [JsonProperty("disposition")]
        public string Disposition { get; set; }
        [JsonProperty("blocked-uri")]
        public string Blockeduri { get; set; }
        [JsonProperty("line-number")]
        public int Linenumber { get; set; }
        [JsonProperty("source-file")]
        public string Sourcefile { get; set; }
        [JsonProperty("status-code")]
        public int Statuscode { get; set; }
        [JsonProperty("script-sample")]
        public string Scriptsample { get; set; }

        public override string ToString()
        {
            return $"{nameof(Documenturi)}: {Documenturi}, {nameof(Referrer)}: {Referrer}, {nameof(Violateddirective)}: {Violateddirective}, {nameof(Effectivedirective)}: {Effectivedirective}, {nameof(Originalpolicy)}: {Originalpolicy}, {nameof(Disposition)}: {Disposition}, {nameof(Blockeduri)}: {Blockeduri}, {nameof(Linenumber)}: {Linenumber}, {nameof(Sourcefile)}: {Sourcefile}, {nameof(Statuscode)}: {Statuscode}, {nameof(Scriptsample)}: {Scriptsample}";
        }
    }



}
