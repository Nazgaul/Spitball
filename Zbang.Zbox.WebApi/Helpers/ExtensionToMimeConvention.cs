using System.Collections.Generic;


namespace Zbang.Zbox.WebApi.Helpers
{
    public static class ExtensionToMimeConvention
    {
        private static readonly Dictionary<string, string> ExtentionToMimeType = new Dictionary<string, string>();

        static ExtensionToMimeConvention()
        {
            ExtentionToMimeType.Add(".3dm", "x-world/x-3dmf");
            ExtentionToMimeType.Add(".3dmf", "x-world/x-3dmf");
            ExtentionToMimeType.Add(".a", "application/octet-stream");
            ExtentionToMimeType.Add(".aab", "application/x-authorware-bin");
            ExtentionToMimeType.Add(".aam", "application/x-authorware-map");
            ExtentionToMimeType.Add(".aas", "application/x-authorware-seg");
            ExtentionToMimeType.Add(".abc", "text/vnd.abc");
            ExtentionToMimeType.Add(".acgi", "text/html");
            ExtentionToMimeType.Add(".afl", "video/animaflex");
            ExtentionToMimeType.Add(".ai", "application/postscript");
            ExtentionToMimeType.Add(".aif", "audio/aiff");

            ExtentionToMimeType.Add(".aifc", "audio/aiff");

            ExtentionToMimeType.Add(".aiff", "audio/aiff");

            ExtentionToMimeType.Add(".aim", "application/x-aim");
            ExtentionToMimeType.Add(".aip", "text/x-audiosoft-intra");
            ExtentionToMimeType.Add(".ani", "application/x-navi-animation");
            ExtentionToMimeType.Add(".aos", "application/x-nokia-9000-communicator-add-on-software");
            ExtentionToMimeType.Add(".aps", "application/mime");
            ExtentionToMimeType.Add(".arc", "application/octet-stream");
            ExtentionToMimeType.Add(".arj", "application/arj");

            ExtentionToMimeType.Add(".art", "image/x-jg");
            ExtentionToMimeType.Add(".asf", "video/x-ms-asf");
            ExtentionToMimeType.Add(".asm", "text/x-asm");
            ExtentionToMimeType.Add(".asp", "text/asp");
            ExtentionToMimeType.Add(".asx", "application/x-mplayer2");


            ExtentionToMimeType.Add(".au", "audio/basic");


            ExtentionToMimeType.Add(".avi", "video/avi");
            ExtentionToMimeType.Add(".avs", "video/avs-video");
            ExtentionToMimeType.Add(".bcpio", "application/x-bcpio");
            ExtentionToMimeType.Add(".bin", "application/x-binary");

            ExtentionToMimeType.Add(".bm", "image/bmp");
            ExtentionToMimeType.Add(".bmp", "image/bmp");
            ExtentionToMimeType.Add(".boo", "application/book");
            ExtentionToMimeType.Add(".book", "application/book");
            ExtentionToMimeType.Add(".boz", "application/x-bzip2");
            ExtentionToMimeType.Add(".bsh", "application/x-bsh");
            ExtentionToMimeType.Add(".bz", "application/x-bzip");
            ExtentionToMimeType.Add(".bz2", "application/x-bzip2");
            ExtentionToMimeType.Add(".c", "text/plain");

            ExtentionToMimeType.Add(".c++", "text/plain");
            ExtentionToMimeType.Add(".cat", "application/vnd.ms-pki.seccat");
            ExtentionToMimeType.Add(".cc", "text/plain");

            ExtentionToMimeType.Add(".ccad", "application/clariscad");
            ExtentionToMimeType.Add(".cco", "application/x-cocoa");
            ExtentionToMimeType.Add(".cdf", "application/cdf");

            ExtentionToMimeType.Add(".cer", "application/pkix-cert");

            ExtentionToMimeType.Add(".cha", "application/x-chat");
            ExtentionToMimeType.Add(".chat", "application/x-chat");
            ExtentionToMimeType.Add(".class", "application/java");
            ExtentionToMimeType.Add(".com", "application/octet-stream");

            ExtentionToMimeType.Add(".conf", "text/plain");
            ExtentionToMimeType.Add(".cpio", "application/x-cpio");
            ExtentionToMimeType.Add(".cpp", "text/x-c");
            ExtentionToMimeType.Add(".cpt", "application/mac-compactpro");

            ExtentionToMimeType.Add(".crl", "application/pkcs-crl");

            ExtentionToMimeType.Add(".crt", "application/pkix-cert");

            ExtentionToMimeType.Add(".csh", "application/x-csh");
            ExtentionToMimeType.Add(".css", "text/css");
            ExtentionToMimeType.Add(".cxx", "text/plain");
            ExtentionToMimeType.Add(".dcr", "application/x-director");
            ExtentionToMimeType.Add(".deepv", "application/x-deepv");
            ExtentionToMimeType.Add(".def", "text/plain");
            ExtentionToMimeType.Add(".dif", "video/x-dv");
            ExtentionToMimeType.Add(".dir", "application/x-director");
            ExtentionToMimeType.Add(".dl", "video/dl");
            ExtentionToMimeType.Add(".doc", "application/msword");
            ExtentionToMimeType.Add(".dot", "application/msword");
            ExtentionToMimeType.Add(".dp", "application/commonground");
            ExtentionToMimeType.Add(".drw", "application/drafting");
            ExtentionToMimeType.Add(".dump", "application/octet-stream");
            ExtentionToMimeType.Add(".dv", "video/x-dv");
            ExtentionToMimeType.Add(".dvi", "application/x-dvi");
            ExtentionToMimeType.Add(".dwf", "model/vnd.dwf");
            ExtentionToMimeType.Add(".dwg", "image/x-dwg");
            ExtentionToMimeType.Add(".dxf", "image/x-dwg");
            ExtentionToMimeType.Add(".el", "text/x-script.elisp");
            ExtentionToMimeType.Add(".elc", "application/x-elc");
            ExtentionToMimeType.Add(".env", "application/x-envoy");
            ExtentionToMimeType.Add(".eps", "application/postscript");
            ExtentionToMimeType.Add(".es", "application/x-esrehber");
            ExtentionToMimeType.Add(".etx", "text/x-setext");
            ExtentionToMimeType.Add(".evy", "application/envoy");
            ExtentionToMimeType.Add(".exe", "application/octet-stream");
            ExtentionToMimeType.Add(".f", "text/plain");

            ExtentionToMimeType.Add(".f77", "text/x-fortran");
            ExtentionToMimeType.Add(".f90", "text/plain");
            ExtentionToMimeType.Add(".fdf", "application/vnd.fdf");
            ExtentionToMimeType.Add(".fif", "image/fif");
            ExtentionToMimeType.Add(".fli", "video/fli");
            ExtentionToMimeType.Add(".flo", "image/florian");
            ExtentionToMimeType.Add(".flx", "text/vnd.fmi.flexstor");
            ExtentionToMimeType.Add(".fmf", "video/x-atomic3d-feature");
            ExtentionToMimeType.Add(".for", "text/plain");
            ExtentionToMimeType.Add(".fpx", "image/vnd.fpx");
            ExtentionToMimeType.Add(".frl", "application/freeloader");
            ExtentionToMimeType.Add(".funk", "audio/make");
            ExtentionToMimeType.Add(".g", "text/plain");
            ExtentionToMimeType.Add(".g3", "image/g3fax");
            ExtentionToMimeType.Add(".gif", "image/gif");
            ExtentionToMimeType.Add(".gl", "video/gl");

            ExtentionToMimeType.Add(".gsd", "audio/x-gsm");
            ExtentionToMimeType.Add(".gsm", "audio/x-gsm");
            ExtentionToMimeType.Add(".gsp", "application/x-gsp");
            ExtentionToMimeType.Add(".gss", "application/x-gss");
            ExtentionToMimeType.Add(".gtar", "application/x-gtar");
            ExtentionToMimeType.Add(".gz", "application/x-compressed");

            ExtentionToMimeType.Add(".gzip", "application/x-gzip");

            ExtentionToMimeType.Add(".h", "text/plain");

            ExtentionToMimeType.Add(".hdf", "application/x-hdf");
            ExtentionToMimeType.Add(".help", "application/x-helpfile");
            ExtentionToMimeType.Add(".hgl", "application/vnd.hp-hpgl");
            ExtentionToMimeType.Add(".hh", "text/plain");

            ExtentionToMimeType.Add(".hlb", "text/x-script");
            ExtentionToMimeType.Add(".hpg", "application/vnd.hp-hpgl");
            ExtentionToMimeType.Add(".hpgl", "application/vnd.hp-hpgl");
            ExtentionToMimeType.Add(".hqx", "application/binhex");
            ExtentionToMimeType.Add(".hta", "application/hta");
            ExtentionToMimeType.Add(".htc", "text/x-component");
            ExtentionToMimeType.Add(".htm", "text/html");
            ExtentionToMimeType.Add(".html", "text/html");
            ExtentionToMimeType.Add(".htmls", "text/html");
            ExtentionToMimeType.Add(".htt", "text/webviewhtml");
            ExtentionToMimeType.Add(".htx", "text/html");
            ExtentionToMimeType.Add(".ice", "x-conference/x-cooltalk");
            ExtentionToMimeType.Add(".ico", "image/x-icon");
            ExtentionToMimeType.Add(".idc", "text/plain");
            ExtentionToMimeType.Add(".ief", "image/ief");
            ExtentionToMimeType.Add(".iefs", "image/ief");
            ExtentionToMimeType.Add(".iges", "application/iges");
            ExtentionToMimeType.Add(".igs", "application/iges");
            ExtentionToMimeType.Add(".ima", "application/x-ima");
            ExtentionToMimeType.Add(".imap", "application/x-httpd-imap");
            ExtentionToMimeType.Add(".inf", "application/inf");
            ExtentionToMimeType.Add(".ins", "application/x-internett-signup");
            ExtentionToMimeType.Add(".ip", "application/x-ip2");
            ExtentionToMimeType.Add(".isu", "video/x-isvideo");
            ExtentionToMimeType.Add(".it", "audio/it");
            ExtentionToMimeType.Add(".iv", "application/x-inventor");
            ExtentionToMimeType.Add(".ivr", "i-world/i-vrml");
            ExtentionToMimeType.Add(".ivy", "application/x-livescreen");
            ExtentionToMimeType.Add(".jam", "audio/x-jam");
            ExtentionToMimeType.Add(".jav", "text/plain");
            ExtentionToMimeType.Add(".java", "text/plain");
            ExtentionToMimeType.Add(".jcm", "application/x-java-commerce");
            ExtentionToMimeType.Add(".jfif", "image/jpeg");
            ExtentionToMimeType.Add(".jfif-tbnl", "image/jpeg");
            ExtentionToMimeType.Add(".jpe", "image/jpeg");

            ExtentionToMimeType.Add(".jpeg", "image/jpeg");

            ExtentionToMimeType.Add(".jpg", "image/jpeg");

            ExtentionToMimeType.Add(".jps", "image/x-jps");
            ExtentionToMimeType.Add(".js", "application/x-javascript");
            ExtentionToMimeType.Add(".jut", "image/jutvision");
            ExtentionToMimeType.Add(".kar", "audio/midi");
            ExtentionToMimeType.Add(".ksh", "text/x-script.ksh");
            ExtentionToMimeType.Add(".la", "audio/nspaudio");

            ExtentionToMimeType.Add(".lam", "audio/x-liveaudio");
            ExtentionToMimeType.Add(".latex", "application/x-latex");
            ExtentionToMimeType.Add(".lha", "application/octet-stream");
            ExtentionToMimeType.Add(".lhx", "application/octet-stream");
            ExtentionToMimeType.Add(".list", "text/plain");
            ExtentionToMimeType.Add(".lma", "audio/nspaudio");
            ExtentionToMimeType.Add(".log", "text/plain");
            ExtentionToMimeType.Add(".lsp", "text/x-script.lisp");
            ExtentionToMimeType.Add(".lst", "text/plain");
            ExtentionToMimeType.Add(".lsx", "text/x-la-asf");
            ExtentionToMimeType.Add(".ltx", "application/x-latex");
            ExtentionToMimeType.Add(".lzh", "application/octet-stream");
            ExtentionToMimeType.Add(".lzx", "application/octet-stream");
            ExtentionToMimeType.Add(".m", "text/plain");

            ExtentionToMimeType.Add(".m1v", "video/mpeg");
            ExtentionToMimeType.Add(".m2a", "audio/mpeg");
            ExtentionToMimeType.Add(".m2v", "video/mpeg");
            ExtentionToMimeType.Add(".m3u", "audio/x-mpequrl");
            ExtentionToMimeType.Add(".man", "application/x-troff-man");
            ExtentionToMimeType.Add(".map", "application/x-navimap");
            ExtentionToMimeType.Add(".mar", "text/plain");
            ExtentionToMimeType.Add(".mbd", "application/mbedlet");
            ExtentionToMimeType.Add(".mc$", "application/x-magic-cap-package-1.0");
            ExtentionToMimeType.Add(".mcd", "application/mcad");
            ExtentionToMimeType.Add(".mcf", "text/mcf");
            ExtentionToMimeType.Add(".mcp", "application/netmc");
            ExtentionToMimeType.Add(".me", "application/x-troff-me");
            ExtentionToMimeType.Add(".mht", "message/rfc822");
            ExtentionToMimeType.Add(".mhtml", "message/rfc822");
            ExtentionToMimeType.Add(".mid", "application/x-midi");
            ExtentionToMimeType.Add(".midi", "application/x-midi");
            ExtentionToMimeType.Add(".mif", "application/x-frame");


            ExtentionToMimeType.Add(".mime", "www/mime");
            ExtentionToMimeType.Add(".mjf", "audio/x-vnd.audioexplosion.mjuicemediafile");
            ExtentionToMimeType.Add(".mjpg", "video/x-motion-jpeg");

            ExtentionToMimeType.Add(".mm", "application/x-meme");
            ExtentionToMimeType.Add(".mme", "application/base64");
            ExtentionToMimeType.Add(".mod", "audio/mod");

            ExtentionToMimeType.Add(".moov", "video/quicktime");
            ExtentionToMimeType.Add(".mov", "video/quicktime");
            ExtentionToMimeType.Add(".movie", "video/x-sgi-movie");
            ExtentionToMimeType.Add(".mp2", "audio/mpeg");

            ExtentionToMimeType.Add(".mp3", "video/mpeg");
            ExtentionToMimeType.Add(".mpa", "video/mpeg");
            ExtentionToMimeType.Add(".mpc", "application/x-project");
            ExtentionToMimeType.Add(".mpe", "video/mpeg");
            ExtentionToMimeType.Add(".mpeg", "video/mpeg");

            ExtentionToMimeType.Add(".mpg", "video/mpeg");
            ExtentionToMimeType.Add(".mpga", "audio/mpeg");
            ExtentionToMimeType.Add(".mpp", "application/vnd.ms-project");
            ExtentionToMimeType.Add(".mpt", "application/x-project");
            ExtentionToMimeType.Add(".mpv", "application/x-project");
            ExtentionToMimeType.Add(".mpx", "application/x-project");
            ExtentionToMimeType.Add(".mrc", "application/marc");
            ExtentionToMimeType.Add(".ms", "application/x-troff-ms");
            ExtentionToMimeType.Add(".mv", "video/x-sgi-movie");
            ExtentionToMimeType.Add(".my", "audio/make");
            ExtentionToMimeType.Add(".mzz", "application/x-vnd.audioexplosion.mzz");
            ExtentionToMimeType.Add(".nap", "image/naplps");
            ExtentionToMimeType.Add(".naplps", "image/naplps");
            ExtentionToMimeType.Add(".nc", "application/x-netcdf");
            ExtentionToMimeType.Add(".ncm", "application/vnd.nokia.configuration-message");
            ExtentionToMimeType.Add(".nif", "image/x-niff");
            ExtentionToMimeType.Add(".niff", "image/x-niff");
            ExtentionToMimeType.Add(".nix", "application/x-mix-transfer");
            ExtentionToMimeType.Add(".nsc", "application/x-conference");
            ExtentionToMimeType.Add(".nvd", "application/x-navidoc");
            ExtentionToMimeType.Add(".o", "application/octet-stream");
            ExtentionToMimeType.Add(".oda", "application/oda");
            ExtentionToMimeType.Add(".omc", "pplication/x-omc");
            ExtentionToMimeType.Add(".omcd", "application/x-omcdatamaker");
            ExtentionToMimeType.Add(".omcr", "application/x-omcregerator");
            ExtentionToMimeType.Add(".p", "text/x-pascal");
            ExtentionToMimeType.Add(".p10", "application/pkcs10");
            ExtentionToMimeType.Add(".p12", "application/pkcs-12");
            ExtentionToMimeType.Add(".p7a", "application/x-pkcs7-signature");
            ExtentionToMimeType.Add(".p7c", "application/pkcs7-mime");
            ExtentionToMimeType.Add(".p7m", "application/pkcs7-mime");
            ExtentionToMimeType.Add(".p7r", "application/x-pkcs7-certreqresp");
            ExtentionToMimeType.Add(".p7s", "application/pkcs7-signature");
            ExtentionToMimeType.Add(".part", "application/pro_eng");
            ExtentionToMimeType.Add(".pas", "text/pascal");
            ExtentionToMimeType.Add(".pbm", "image/x-portable-bitmap");
            ExtentionToMimeType.Add(".pcl", "application/x-pcl");
            ExtentionToMimeType.Add(".pct", "image/x-pict");
            ExtentionToMimeType.Add(".pcx", "image/x-pcx");
            ExtentionToMimeType.Add(".pdb", "chemical/x-pdb");
            ExtentionToMimeType.Add(".pdf", "application/pdf");
            ExtentionToMimeType.Add(".pfunk", "audio/make");
            ExtentionToMimeType.Add(".pgm", "image/x-portable-graymap");

            ExtentionToMimeType.Add(".pic", "image/pict");
            ExtentionToMimeType.Add(".pict", "image/pict");
            ExtentionToMimeType.Add(".pkg", "application/x-newton-compatible-pkg");
            ExtentionToMimeType.Add(".pko", "application/vnd.ms-pki.pko");
            ExtentionToMimeType.Add(".pl", "text/plain");

            ExtentionToMimeType.Add(".plx", "application/x-pixclscript");
            ExtentionToMimeType.Add(".pm", "image/x-xpixmap");

            ExtentionToMimeType.Add(".pm4", "application/x-pagemaker");
            ExtentionToMimeType.Add(".pm5", "application/x-pagemaker");
            ExtentionToMimeType.Add(".png", "image/png");
            ExtentionToMimeType.Add(".pnm", "application/x-portable-anymap");
            ExtentionToMimeType.Add(".pot", "application/mspowerpoint");
            ExtentionToMimeType.Add(".pov", "model/x-pov");
            ExtentionToMimeType.Add(".ppa", "application/vnd.ms-powerpoint");
            ExtentionToMimeType.Add(".ppm", "image/x-portable-pixmap");
            ExtentionToMimeType.Add(".pps", "application/mspowerpoint");
            ExtentionToMimeType.Add(".ppt", "application/powerpoint");
            ExtentionToMimeType.Add(".ppz", "application/mspowerpoint");
            ExtentionToMimeType.Add(".pre", "application/x-freelance");
            ExtentionToMimeType.Add(".prt", "application/pro_eng");
            ExtentionToMimeType.Add(".ps", "application/postscript");
            ExtentionToMimeType.Add(".psd", "application/octet-stream");
            ExtentionToMimeType.Add(".pvu", "paleovu/x-pv");
            ExtentionToMimeType.Add(".pwz", "application/vnd.ms-powerpoint");
            ExtentionToMimeType.Add(".py", "text/x-script.phyton");
            ExtentionToMimeType.Add(".pyc", "applicaiton/x-bytecode.python");
            ExtentionToMimeType.Add(".qcp", "audio/vnd.qcelp");
            ExtentionToMimeType.Add(".qd3", "x-world/x-3dmf");
            ExtentionToMimeType.Add(".qd3d", "x-world/x-3dmf");
            ExtentionToMimeType.Add(".qif", "image/x-quicktime");
            ExtentionToMimeType.Add(".qt", "video/quicktime");
            ExtentionToMimeType.Add(".qtc", "video/x-qtc");
            ExtentionToMimeType.Add(".qti", "image/x-quicktime");
            ExtentionToMimeType.Add(".qtif", "image/x-quicktime");
            ExtentionToMimeType.Add(".ra", "audio/x-pn-realaudio");

            ExtentionToMimeType.Add(".ram", "audio/x-pn-realaudio");
            ExtentionToMimeType.Add(".ras", "application/x-cmu-raster");
            ExtentionToMimeType.Add(".rast", "image/cmu-raster");
            ExtentionToMimeType.Add(".rexx", "text/x-script.rexx");
            ExtentionToMimeType.Add(".rf", "image/vnd.rn-realflash");
            ExtentionToMimeType.Add(".rgb", "image/x-rgb");
            ExtentionToMimeType.Add(".rm", "application/vnd.rn-realmedia");

            ExtentionToMimeType.Add(".rmi", "audio/mid");
            ExtentionToMimeType.Add(".rmm", "audio/x-pn-realaudio");
            ExtentionToMimeType.Add(".rmp", "audio/x-pn-realaudio");
            ExtentionToMimeType.Add(".rng", "application/ringing-tones");
            ExtentionToMimeType.Add(".rnx", "application/vnd.rn-realplayer");
            ExtentionToMimeType.Add(".roff", "application/x-troff");
            ExtentionToMimeType.Add(".rp", "image/vnd.rn-realpix");
            ExtentionToMimeType.Add(".rpm", "audio/x-pn-realaudio-plugin");
            ExtentionToMimeType.Add(".rt", "text/richtext");

            ExtentionToMimeType.Add(".rtf", "application/rtf");
            ExtentionToMimeType.Add(".rtx", "application/rtf");
            ExtentionToMimeType.Add(".rv", "video/vnd.rn-realvideo");
            ExtentionToMimeType.Add(".s", "text/x-asm");
            ExtentionToMimeType.Add(".s3m", "audio/s3m");
            ExtentionToMimeType.Add(".saveme", "application/octet-stream");
            ExtentionToMimeType.Add(".sbk", "application/x-tbook");
            ExtentionToMimeType.Add(".scm", "application/x-lotusscreencam");
            ExtentionToMimeType.Add(".sdml", "text/plain");
            ExtentionToMimeType.Add(".sdp", "application/sdp");
            ExtentionToMimeType.Add(".sdr", "application/sounder");
            ExtentionToMimeType.Add(".sea", "application/sea");
            ExtentionToMimeType.Add(".set", "application/set");
            ExtentionToMimeType.Add(".sgm", "text/sgml");
            ExtentionToMimeType.Add(".sgml", "text/sgml");
            ExtentionToMimeType.Add(".sh", "text/x-script.sh");
            ExtentionToMimeType.Add(".shar", "application/x-bsh");

            ExtentionToMimeType.Add(".shtml", "text/html");

            ExtentionToMimeType.Add(".sid", "audio/x-psid");
            ExtentionToMimeType.Add(".sit", "application/x-sit");
            ExtentionToMimeType.Add(".skd", "application/x-koan");
            ExtentionToMimeType.Add(".skm", "application/x-koan");
            ExtentionToMimeType.Add(".skp", "application/x-koan");
            ExtentionToMimeType.Add(".skt", "application/x-koan");
            ExtentionToMimeType.Add(".sl", "application/x-seelogo");
            ExtentionToMimeType.Add(".smi", "application/smil");
            ExtentionToMimeType.Add(".smil", "application/smil");
            ExtentionToMimeType.Add(".snd", "audio/basic");
            ExtentionToMimeType.Add(".sol", "application/solids");
            ExtentionToMimeType.Add(".spc", "text/x-speech");
            ExtentionToMimeType.Add(".spl", "application/futuresplash");
            ExtentionToMimeType.Add(".spr", "application/x-sprite");
            ExtentionToMimeType.Add(".sprite", "application/x-sprite");
            ExtentionToMimeType.Add(".src", "application/x-wais-source");
            ExtentionToMimeType.Add(".ssi", "text/x-server-parsed-html");
            ExtentionToMimeType.Add(".ssm", "application/streamingmedia");
            ExtentionToMimeType.Add(".sst", "application/vnd.ms-pki.certstore");
            ExtentionToMimeType.Add(".step", "application/step");
            ExtentionToMimeType.Add(".stl", "application/sla");
            ExtentionToMimeType.Add(".stp", "application/step");
            ExtentionToMimeType.Add(".sv4cpio", "application/x-sv4cpio");
            ExtentionToMimeType.Add(".sv4crc", "application/x-sv4crc");
            ExtentionToMimeType.Add(".svf", "image/x-dwg");
            ExtentionToMimeType.Add(".svr", "application/x-world");
            ExtentionToMimeType.Add(".swf", "application/x-shockwave-flash");
            ExtentionToMimeType.Add(".t", "application/x-troff");
            ExtentionToMimeType.Add(".talk", "text/x-speech");
            ExtentionToMimeType.Add(".tar", "application/x-tar");
            ExtentionToMimeType.Add(".tbk", "application/toolbook");
            ExtentionToMimeType.Add(".tcl", "application/x-tcl");
            ExtentionToMimeType.Add(".tcsh", "text/x-script.tcsh");
            ExtentionToMimeType.Add(".tex", "application/x-tex");
            ExtentionToMimeType.Add(".texi", "application/x-texinfo");
            ExtentionToMimeType.Add(".texinfo", "application/x-texinfo");
            ExtentionToMimeType.Add(".text", "text/plain");
            ExtentionToMimeType.Add(".tgz", "application/x-compressed");
            ExtentionToMimeType.Add(".tif", "image/tiff");
            ExtentionToMimeType.Add(".tiff", "image/tiff");
            ExtentionToMimeType.Add(".tr", "application/x-troff");
            ExtentionToMimeType.Add(".tsi", "audio/tsp-audio");
            ExtentionToMimeType.Add(".tsp", "application/dsptype");
            ExtentionToMimeType.Add(".tsv", "text/tab-separated-values");
            ExtentionToMimeType.Add(".turbot", "image/florian");
            ExtentionToMimeType.Add(".txt", "text/plain");
            ExtentionToMimeType.Add(".uil", "text/x-uil");
            ExtentionToMimeType.Add(".uni", "text/uri-list");
            ExtentionToMimeType.Add(".unis", "text/uri-list");
            ExtentionToMimeType.Add(".unv", "application/i-deas");
            ExtentionToMimeType.Add(".uri", "text/uri-list");
            ExtentionToMimeType.Add(".uris", "text/uri-list");
            ExtentionToMimeType.Add(".ustar", "application/x-ustar");
            ExtentionToMimeType.Add(".uu", "text/x-uuencode");
            ExtentionToMimeType.Add(".uue", "text/x-uuencode");
            ExtentionToMimeType.Add(".vcd", "application/x-cdlink");
            ExtentionToMimeType.Add(".vcs", "text/x-vcalendar");
            ExtentionToMimeType.Add(".vda", "application/vda");
            ExtentionToMimeType.Add(".vdo", "video/vdo");
            ExtentionToMimeType.Add(".vew", "application/groupwise");
            ExtentionToMimeType.Add(".viv", "video/vivo");
            ExtentionToMimeType.Add(".vivo", "video/vivo");
            ExtentionToMimeType.Add(".vmd", "application/vocaltec-media-desc");
            ExtentionToMimeType.Add(".vmf", "application/vocaltec-media-file");
            ExtentionToMimeType.Add(".voc", "audio/voc");
            ExtentionToMimeType.Add(".vos", "video/vosaic");
            ExtentionToMimeType.Add(".vox", "audio/voxware");
            ExtentionToMimeType.Add(".vqe", "audio/x-twinvq-plugin");
            ExtentionToMimeType.Add(".vqf", "audio/x-twinvq");
            ExtentionToMimeType.Add(".vql", "audio/x-twinvq-plugin");
            ExtentionToMimeType.Add(".vrml", "application/x-vrml");
            ExtentionToMimeType.Add(".vrt", "x-world/x-vrt");
            ExtentionToMimeType.Add(".vsd", "application/x-visio");
            ExtentionToMimeType.Add(".vst", "application/x-visio");
            ExtentionToMimeType.Add(".vsw", "application/x-visio");
            ExtentionToMimeType.Add(".w60", "application/wordperfect6.0");
            ExtentionToMimeType.Add(".w61", "application/wordperfect6.1");
            ExtentionToMimeType.Add(".w6w", "application/msword");
            ExtentionToMimeType.Add(".wav", "audio/wav");
            ExtentionToMimeType.Add(".wb1", "application/x-qpro");
            ExtentionToMimeType.Add(".wbmp", "image/vnd.wap.wbmp");
            ExtentionToMimeType.Add(".web", "application/vnd.xara");
            ExtentionToMimeType.Add(".wiz", "application/msword");
            ExtentionToMimeType.Add(".wk1", "application/x-123");
            ExtentionToMimeType.Add(".wmf", "windows/metafile");
            ExtentionToMimeType.Add(".wml", "text/vnd.wap.wml");
            ExtentionToMimeType.Add(".wmlc", "application/vnd.wap.wmlc");
            ExtentionToMimeType.Add(".wmls", "text/vnd.wap.wmlscript");
            ExtentionToMimeType.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
            ExtentionToMimeType.Add(".word", "application/msword");
            ExtentionToMimeType.Add(".wp", "application/wordperfect");
            ExtentionToMimeType.Add(".wp5", "application/wordperfect");
            ExtentionToMimeType.Add(".wp6", "application/wordperfect");
            ExtentionToMimeType.Add(".wpd", "application/wordperfect");
            ExtentionToMimeType.Add(".wq1", "application/x-lotus");
            ExtentionToMimeType.Add(".wri", "application/x-wri");
            ExtentionToMimeType.Add(".wrl", "model/vrml");
            ExtentionToMimeType.Add(".wrz", "model/vrml");
            ExtentionToMimeType.Add(".wsc", "text/scriplet");
            ExtentionToMimeType.Add(".wsrc", "application/x-wais-source");
            ExtentionToMimeType.Add(".wtk", "application/x-wintalk");
            ExtentionToMimeType.Add(".xbm", "image/xbm");
            ExtentionToMimeType.Add(".xdr", "video/x-amt-demorun");
            ExtentionToMimeType.Add(".xgz", "xgl/drawing");
            ExtentionToMimeType.Add(".xif", "image/vnd.xiff");
            ExtentionToMimeType.Add(".xl", "application/excel");
            ExtentionToMimeType.Add(".xla", "application/excel");
            ExtentionToMimeType.Add(".xlb", "application/excel");

            ExtentionToMimeType.Add(".xlc", "application/excel");
            ExtentionToMimeType.Add(".xld", "application/excel");
            ExtentionToMimeType.Add(".xlk", "application/excel");
            ExtentionToMimeType.Add(".xll", "application/excel");
            ExtentionToMimeType.Add(".xlm", "application/excel");
            ExtentionToMimeType.Add(".xls", "application/excel");
            ExtentionToMimeType.Add(".xlt", "application/excel");
            ExtentionToMimeType.Add(".xlv", "application/excel");
            ExtentionToMimeType.Add(".xlw", "application/excel");
            ExtentionToMimeType.Add(".xm", "audio/xm");
            ExtentionToMimeType.Add(".xml", "text/xml");
            ExtentionToMimeType.Add(".xmz", "xgl/movie");
            ExtentionToMimeType.Add(".xpix", "application/x-vnd.ls-xpix");
            ExtentionToMimeType.Add(".xpm", "image/xpm");
            ExtentionToMimeType.Add(".x-png", "image/png");
            ExtentionToMimeType.Add(".xsr", "video/x-amt-showrun");
            ExtentionToMimeType.Add(".xwd", "image/x-xwd");

            ExtentionToMimeType.Add(".xyz", "chemical/x-pdb");
            ExtentionToMimeType.Add(".z", "application/x-compress");

            ExtentionToMimeType.Add(".zip", "application/zip");
            ExtentionToMimeType.Add(".zoo", "application/octet-stream");
            ExtentionToMimeType.Add(".zsh", "text/x-script.zsh");
        }

        public static string GetMimeType(string extenstion)
        {
            string retVal = "application/octet-stream";
            ExtentionToMimeType.TryGetValue(extenstion, out retVal);

            return retVal;
        }


    }
}