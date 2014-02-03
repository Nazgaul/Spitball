using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.WCFServiceWebRole
{
    public static class ExtensionToMimeConvention
    {
        private static Dictionary<string, string> m_ExtentionToMimeType = new Dictionary<string,string>();

        static ExtensionToMimeConvention()
        {
            m_ExtentionToMimeType.Add(".3dm", "x-world/x-3dmf");
            m_ExtentionToMimeType.Add(".3dmf", "x-world/x-3dmf");
            m_ExtentionToMimeType.Add(".a", "application/octet-stream");
            m_ExtentionToMimeType.Add(".aab", "application/x-authorware-bin");
            m_ExtentionToMimeType.Add(".aam", "application/x-authorware-map");
            m_ExtentionToMimeType.Add(".aas", "application/x-authorware-seg");
            m_ExtentionToMimeType.Add(".abc", "text/vnd.abc");
            m_ExtentionToMimeType.Add(".acgi", "text/html");
            m_ExtentionToMimeType.Add(".afl", "video/animaflex");
            m_ExtentionToMimeType.Add(".ai", "application/postscript");
            m_ExtentionToMimeType.Add(".aif", "audio/aiff");

            m_ExtentionToMimeType.Add(".aifc", "audio/aiff");

            m_ExtentionToMimeType.Add(".aiff", "audio/aiff");

            m_ExtentionToMimeType.Add(".aim", "application/x-aim");
            m_ExtentionToMimeType.Add(".aip", "text/x-audiosoft-intra");
            m_ExtentionToMimeType.Add(".ani", "application/x-navi-animation");
            m_ExtentionToMimeType.Add(".aos", "application/x-nokia-9000-communicator-add-on-software");
            m_ExtentionToMimeType.Add(".aps", "application/mime");
            m_ExtentionToMimeType.Add(".arc", "application/octet-stream");
            m_ExtentionToMimeType.Add(".arj", "application/arj");

            m_ExtentionToMimeType.Add(".art", "image/x-jg");
            m_ExtentionToMimeType.Add(".asf", "video/x-ms-asf");
            m_ExtentionToMimeType.Add(".asm", "text/x-asm");
            m_ExtentionToMimeType.Add(".asp", "text/asp");
            m_ExtentionToMimeType.Add(".asx", "application/x-mplayer2");


            m_ExtentionToMimeType.Add(".au", "audio/basic");


            m_ExtentionToMimeType.Add(".avi", "video/avi");
            m_ExtentionToMimeType.Add(".avs", "video/avs-video");
            m_ExtentionToMimeType.Add(".bcpio", "application/x-bcpio");
            m_ExtentionToMimeType.Add(".bin", "application/x-binary");

            m_ExtentionToMimeType.Add(".bm", "image/bmp");
            m_ExtentionToMimeType.Add(".bmp", "image/bmp");
            m_ExtentionToMimeType.Add(".boo", "application/book");
            m_ExtentionToMimeType.Add(".book", "application/book");
            m_ExtentionToMimeType.Add(".boz", "application/x-bzip2");
            m_ExtentionToMimeType.Add(".bsh", "application/x-bsh");
            m_ExtentionToMimeType.Add(".bz", "application/x-bzip");
            m_ExtentionToMimeType.Add(".bz2", "application/x-bzip2");
            m_ExtentionToMimeType.Add(".c", "text/plain");

            m_ExtentionToMimeType.Add(".c++", "text/plain");
            m_ExtentionToMimeType.Add(".cat", "application/vnd.ms-pki.seccat");
            m_ExtentionToMimeType.Add(".cc", "text/plain");

            m_ExtentionToMimeType.Add(".ccad", "application/clariscad");
            m_ExtentionToMimeType.Add(".cco", "application/x-cocoa");
            m_ExtentionToMimeType.Add(".cdf", "application/cdf");
            
            m_ExtentionToMimeType.Add(".cer", "application/pkix-cert");
            
            m_ExtentionToMimeType.Add(".cha", "application/x-chat");
            m_ExtentionToMimeType.Add(".chat", "application/x-chat");
            m_ExtentionToMimeType.Add(".class", "application/java");
            m_ExtentionToMimeType.Add(".com", "application/octet-stream");

            m_ExtentionToMimeType.Add(".conf", "text/plain");
            m_ExtentionToMimeType.Add(".cpio", "application/x-cpio");
            m_ExtentionToMimeType.Add(".cpp", "text/x-c");
            m_ExtentionToMimeType.Add(".cpt", "application/mac-compactpro");
            
            m_ExtentionToMimeType.Add(".crl", "application/pkcs-crl");
            
            m_ExtentionToMimeType.Add(".crt", "application/pkix-cert");
            
            m_ExtentionToMimeType.Add(".csh", "application/x-csh");
            m_ExtentionToMimeType.Add(".css", "text/css");
            m_ExtentionToMimeType.Add(".cxx", "text/plain");
            m_ExtentionToMimeType.Add(".dcr", "application/x-director");
            m_ExtentionToMimeType.Add(".deepv", "application/x-deepv");
            m_ExtentionToMimeType.Add(".def", "text/plain");
            m_ExtentionToMimeType.Add(".dif", "video/x-dv");
            m_ExtentionToMimeType.Add(".dir", "application/x-director");
            m_ExtentionToMimeType.Add(".dl", "video/dl");
            m_ExtentionToMimeType.Add(".doc", "application/msword");
            m_ExtentionToMimeType.Add(".dot", "application/msword");
            m_ExtentionToMimeType.Add(".dp", "application/commonground");
            m_ExtentionToMimeType.Add(".drw", "application/drafting");
            m_ExtentionToMimeType.Add(".dump", "application/octet-stream");
            m_ExtentionToMimeType.Add(".dv", "video/x-dv");
            m_ExtentionToMimeType.Add(".dvi", "application/x-dvi");
            m_ExtentionToMimeType.Add(".dwf", "model/vnd.dwf");
            m_ExtentionToMimeType.Add(".dwg", "image/x-dwg");
            m_ExtentionToMimeType.Add(".dxf", "image/x-dwg");
            m_ExtentionToMimeType.Add(".el", "text/x-script.elisp");
            m_ExtentionToMimeType.Add(".elc", "application/x-elc");
            m_ExtentionToMimeType.Add(".env", "application/x-envoy");
            m_ExtentionToMimeType.Add(".eps", "application/postscript");
            m_ExtentionToMimeType.Add(".es", "application/x-esrehber");
            m_ExtentionToMimeType.Add(".etx", "text/x-setext");
            m_ExtentionToMimeType.Add(".evy", "application/envoy");
            m_ExtentionToMimeType.Add(".exe", "application/octet-stream");
            m_ExtentionToMimeType.Add(".f", "text/plain");

            m_ExtentionToMimeType.Add(".f77", "text/x-fortran");
            m_ExtentionToMimeType.Add(".f90", "text/plain");
            m_ExtentionToMimeType.Add(".fdf", "application/vnd.fdf");
            m_ExtentionToMimeType.Add(".fif", "image/fif");
            m_ExtentionToMimeType.Add(".fli", "video/fli");
            m_ExtentionToMimeType.Add(".flo", "image/florian");
            m_ExtentionToMimeType.Add(".flx", "text/vnd.fmi.flexstor");
            m_ExtentionToMimeType.Add(".fmf", "video/x-atomic3d-feature");
            m_ExtentionToMimeType.Add(".for", "text/plain");
            m_ExtentionToMimeType.Add(".fpx", "image/vnd.fpx");
            m_ExtentionToMimeType.Add(".frl", "application/freeloader");
            m_ExtentionToMimeType.Add(".funk", "audio/make");
            m_ExtentionToMimeType.Add(".g", "text/plain");
            m_ExtentionToMimeType.Add(".g3", "image/g3fax");
            m_ExtentionToMimeType.Add(".gif", "image/gif");
            m_ExtentionToMimeType.Add(".gl", "video/gl");

            m_ExtentionToMimeType.Add(".gsd", "audio/x-gsm");
            m_ExtentionToMimeType.Add(".gsm", "audio/x-gsm");
            m_ExtentionToMimeType.Add(".gsp", "application/x-gsp");
            m_ExtentionToMimeType.Add(".gss", "application/x-gss");
            m_ExtentionToMimeType.Add(".gtar", "application/x-gtar");
            m_ExtentionToMimeType.Add(".gz", "application/x-compressed");

            m_ExtentionToMimeType.Add(".gzip", "application/x-gzip");

            m_ExtentionToMimeType.Add(".h", "text/plain");

            m_ExtentionToMimeType.Add(".hdf", "application/x-hdf");
            m_ExtentionToMimeType.Add(".help", "application/x-helpfile");
            m_ExtentionToMimeType.Add(".hgl", "application/vnd.hp-hpgl");
            m_ExtentionToMimeType.Add(".hh", "text/plain");

            m_ExtentionToMimeType.Add(".hlb", "text/x-script");
            m_ExtentionToMimeType.Add(".hpg", "application/vnd.hp-hpgl");
            m_ExtentionToMimeType.Add(".hpgl", "application/vnd.hp-hpgl");
            m_ExtentionToMimeType.Add(".hqx", "application/binhex");
            m_ExtentionToMimeType.Add(".hta", "application/hta");
            m_ExtentionToMimeType.Add(".htc", "text/x-component");
            m_ExtentionToMimeType.Add(".htm", "text/html");
            m_ExtentionToMimeType.Add(".html", "text/html");
            m_ExtentionToMimeType.Add(".htmls", "text/html");
            m_ExtentionToMimeType.Add(".htt", "text/webviewhtml");
            m_ExtentionToMimeType.Add(".htx", "text/html");
            m_ExtentionToMimeType.Add(".ice", "x-conference/x-cooltalk");
            m_ExtentionToMimeType.Add(".ico", "image/x-icon");
            m_ExtentionToMimeType.Add(".idc", "text/plain");
            m_ExtentionToMimeType.Add(".ief", "image/ief");
            m_ExtentionToMimeType.Add(".iefs", "image/ief");
            m_ExtentionToMimeType.Add(".iges", "application/iges");
            m_ExtentionToMimeType.Add(".igs", "application/iges");
            m_ExtentionToMimeType.Add(".ima", "application/x-ima");
            m_ExtentionToMimeType.Add(".imap", "application/x-httpd-imap");
            m_ExtentionToMimeType.Add(".inf", "application/inf");
            m_ExtentionToMimeType.Add(".ins", "application/x-internett-signup");
            m_ExtentionToMimeType.Add(".ip", "application/x-ip2");
            m_ExtentionToMimeType.Add(".isu", "video/x-isvideo");
            m_ExtentionToMimeType.Add(".it", "audio/it");
            m_ExtentionToMimeType.Add(".iv", "application/x-inventor");
            m_ExtentionToMimeType.Add(".ivr", "i-world/i-vrml");
            m_ExtentionToMimeType.Add(".ivy", "application/x-livescreen");
            m_ExtentionToMimeType.Add(".jam", "audio/x-jam");
            m_ExtentionToMimeType.Add(".jav", "text/plain");
            m_ExtentionToMimeType.Add(".java", "text/plain");
            m_ExtentionToMimeType.Add(".jcm", "application/x-java-commerce");
            m_ExtentionToMimeType.Add(".jfif", "image/jpeg");
            m_ExtentionToMimeType.Add(".jfif-tbnl", "image/jpeg");
            m_ExtentionToMimeType.Add(".jpe", "image/jpeg");

            m_ExtentionToMimeType.Add(".jpeg", "image/jpeg");

            m_ExtentionToMimeType.Add(".jpg", "image/jpeg");

            m_ExtentionToMimeType.Add(".jps", "image/x-jps");
            m_ExtentionToMimeType.Add(".js", "application/x-javascript");
            m_ExtentionToMimeType.Add(".jut", "image/jutvision");
            m_ExtentionToMimeType.Add(".kar", "audio/midi");
            m_ExtentionToMimeType.Add(".ksh", "text/x-script.ksh");
            m_ExtentionToMimeType.Add(".la", "audio/nspaudio");

            m_ExtentionToMimeType.Add(".lam", "audio/x-liveaudio");
            m_ExtentionToMimeType.Add(".latex", "application/x-latex");
            m_ExtentionToMimeType.Add(".lha", "application/octet-stream");
            m_ExtentionToMimeType.Add(".lhx", "application/octet-stream");
            m_ExtentionToMimeType.Add(".list", "text/plain");
            m_ExtentionToMimeType.Add(".lma", "audio/nspaudio");
            m_ExtentionToMimeType.Add(".log", "text/plain");
            m_ExtentionToMimeType.Add(".lsp", "text/x-script.lisp");
            m_ExtentionToMimeType.Add(".lst", "text/plain");
            m_ExtentionToMimeType.Add(".lsx", "text/x-la-asf");
            m_ExtentionToMimeType.Add(".ltx", "application/x-latex");
            m_ExtentionToMimeType.Add(".lzh", "application/octet-stream");
            m_ExtentionToMimeType.Add(".lzx", "application/octet-stream");
            m_ExtentionToMimeType.Add(".m", "text/plain");

            m_ExtentionToMimeType.Add(".m1v", "video/mpeg");
            m_ExtentionToMimeType.Add(".m2a", "audio/mpeg");
            m_ExtentionToMimeType.Add(".m2v", "video/mpeg");
            m_ExtentionToMimeType.Add(".m3u", "audio/x-mpequrl");
            m_ExtentionToMimeType.Add(".man", "application/x-troff-man");
            m_ExtentionToMimeType.Add(".map", "application/x-navimap");
            m_ExtentionToMimeType.Add(".mar", "text/plain");
            m_ExtentionToMimeType.Add(".mbd", "application/mbedlet");
            m_ExtentionToMimeType.Add(".mc$", "application/x-magic-cap-package-1.0");
            m_ExtentionToMimeType.Add(".mcd", "application/mcad");
            m_ExtentionToMimeType.Add(".mcf", "text/mcf");
            m_ExtentionToMimeType.Add(".mcp", "application/netmc");
            m_ExtentionToMimeType.Add(".me", "application/x-troff-me");
            m_ExtentionToMimeType.Add(".mht", "message/rfc822");
            m_ExtentionToMimeType.Add(".mhtml", "message/rfc822");
            m_ExtentionToMimeType.Add(".mid", "application/x-midi");
            m_ExtentionToMimeType.Add(".midi", "application/x-midi");
            m_ExtentionToMimeType.Add(".mif", "application/x-frame");


            m_ExtentionToMimeType.Add(".mime", "www/mime");
            m_ExtentionToMimeType.Add(".mjf", "audio/x-vnd.audioexplosion.mjuicemediafile");
            m_ExtentionToMimeType.Add(".mjpg", "video/x-motion-jpeg");

            m_ExtentionToMimeType.Add(".mm", "application/x-meme");
            m_ExtentionToMimeType.Add(".mme", "application/base64");
            m_ExtentionToMimeType.Add(".mod", "audio/mod");

            m_ExtentionToMimeType.Add(".moov", "video/quicktime");
            m_ExtentionToMimeType.Add(".mov", "video/quicktime");
            m_ExtentionToMimeType.Add(".movie", "video/x-sgi-movie");
            m_ExtentionToMimeType.Add(".mp2", "audio/mpeg");

            m_ExtentionToMimeType.Add(".mp3", "video/mpeg");
            m_ExtentionToMimeType.Add(".mpa", "video/mpeg");
            m_ExtentionToMimeType.Add(".mpc", "application/x-project");
            m_ExtentionToMimeType.Add(".mpe", "video/mpeg");
            m_ExtentionToMimeType.Add(".mpeg", "video/mpeg");

            m_ExtentionToMimeType.Add(".mpg", "video/mpeg");
            m_ExtentionToMimeType.Add(".mpga", "audio/mpeg");
            m_ExtentionToMimeType.Add(".mpp", "application/vnd.ms-project");
            m_ExtentionToMimeType.Add(".mpt", "application/x-project");
            m_ExtentionToMimeType.Add(".mpv", "application/x-project");
            m_ExtentionToMimeType.Add(".mpx", "application/x-project");
            m_ExtentionToMimeType.Add(".mrc", "application/marc");
            m_ExtentionToMimeType.Add(".ms", "application/x-troff-ms");
            m_ExtentionToMimeType.Add(".mv", "video/x-sgi-movie");
            m_ExtentionToMimeType.Add(".my", "audio/make");
            m_ExtentionToMimeType.Add(".mzz", "application/x-vnd.audioexplosion.mzz");
            m_ExtentionToMimeType.Add(".nap", "image/naplps");
            m_ExtentionToMimeType.Add(".naplps", "image/naplps");
            m_ExtentionToMimeType.Add(".nc", "application/x-netcdf");
            m_ExtentionToMimeType.Add(".ncm", "application/vnd.nokia.configuration-message");
            m_ExtentionToMimeType.Add(".nif", "image/x-niff");
            m_ExtentionToMimeType.Add(".niff", "image/x-niff");
            m_ExtentionToMimeType.Add(".nix", "application/x-mix-transfer");
            m_ExtentionToMimeType.Add(".nsc", "application/x-conference");
            m_ExtentionToMimeType.Add(".nvd", "application/x-navidoc");
            m_ExtentionToMimeType.Add(".o", "application/octet-stream");
            m_ExtentionToMimeType.Add(".oda", "application/oda");
            m_ExtentionToMimeType.Add(".omc", "pplication/x-omc");
            m_ExtentionToMimeType.Add(".omcd", "application/x-omcdatamaker");
            m_ExtentionToMimeType.Add(".omcr", "application/x-omcregerator");
            m_ExtentionToMimeType.Add(".p", "text/x-pascal");
            m_ExtentionToMimeType.Add(".p10", "application/pkcs10");
            m_ExtentionToMimeType.Add(".p12", "application/pkcs-12");
            m_ExtentionToMimeType.Add(".p7a", "application/x-pkcs7-signature");
            m_ExtentionToMimeType.Add(".p7c", "application/pkcs7-mime");
            m_ExtentionToMimeType.Add(".p7m", "application/pkcs7-mime");
            m_ExtentionToMimeType.Add(".p7r", "application/x-pkcs7-certreqresp");
            m_ExtentionToMimeType.Add(".p7s", "application/pkcs7-signature");
            m_ExtentionToMimeType.Add(".part", "application/pro_eng");
            m_ExtentionToMimeType.Add(".pas", "text/pascal");
            m_ExtentionToMimeType.Add(".pbm", "image/x-portable-bitmap");
            m_ExtentionToMimeType.Add(".pcl", "application/x-pcl");
            m_ExtentionToMimeType.Add(".pct", "image/x-pict");
            m_ExtentionToMimeType.Add(".pcx", "image/x-pcx");
            m_ExtentionToMimeType.Add(".pdb", "chemical/x-pdb");
            m_ExtentionToMimeType.Add(".pdf", "application/pdf");
            m_ExtentionToMimeType.Add(".pfunk", "audio/make");
            m_ExtentionToMimeType.Add(".pgm", "image/x-portable-graymap");

            m_ExtentionToMimeType.Add(".pic", "image/pict");
            m_ExtentionToMimeType.Add(".pict", "image/pict");
            m_ExtentionToMimeType.Add(".pkg", "application/x-newton-compatible-pkg");
            m_ExtentionToMimeType.Add(".pko", "application/vnd.ms-pki.pko");
            m_ExtentionToMimeType.Add(".pl", "text/plain");

            m_ExtentionToMimeType.Add(".plx", "application/x-pixclscript");
            m_ExtentionToMimeType.Add(".pm", "image/x-xpixmap");

            m_ExtentionToMimeType.Add(".pm4", "application/x-pagemaker");
            m_ExtentionToMimeType.Add(".pm5", "application/x-pagemaker");
            m_ExtentionToMimeType.Add(".png", "image/png");
            m_ExtentionToMimeType.Add(".pnm", "application/x-portable-anymap");
            m_ExtentionToMimeType.Add(".pot", "application/mspowerpoint");
            m_ExtentionToMimeType.Add(".pov", "model/x-pov");
            m_ExtentionToMimeType.Add(".ppa", "application/vnd.ms-powerpoint");
            m_ExtentionToMimeType.Add(".ppm", "image/x-portable-pixmap");
            m_ExtentionToMimeType.Add(".pps", "application/mspowerpoint");
            m_ExtentionToMimeType.Add(".ppt", "application/powerpoint");
            m_ExtentionToMimeType.Add(".ppz", "application/mspowerpoint");
            m_ExtentionToMimeType.Add(".pre", "application/x-freelance");
            m_ExtentionToMimeType.Add(".prt", "application/pro_eng");
            m_ExtentionToMimeType.Add(".ps", "application/postscript");
            m_ExtentionToMimeType.Add(".psd", "application/octet-stream");
            m_ExtentionToMimeType.Add(".pvu", "paleovu/x-pv");
            m_ExtentionToMimeType.Add(".pwz", "application/vnd.ms-powerpoint");
            m_ExtentionToMimeType.Add(".py", "text/x-script.phyton");
            m_ExtentionToMimeType.Add(".pyc", "applicaiton/x-bytecode.python");
            m_ExtentionToMimeType.Add(".qcp", "audio/vnd.qcelp");
            m_ExtentionToMimeType.Add(".qd3", "x-world/x-3dmf");
            m_ExtentionToMimeType.Add(".qd3d", "x-world/x-3dmf");
            m_ExtentionToMimeType.Add(".qif", "image/x-quicktime");
            m_ExtentionToMimeType.Add(".qt", "video/quicktime");
            m_ExtentionToMimeType.Add(".qtc", "video/x-qtc");
            m_ExtentionToMimeType.Add(".qti", "image/x-quicktime");
            m_ExtentionToMimeType.Add(".qtif", "image/x-quicktime");
            m_ExtentionToMimeType.Add(".ra", "audio/x-pn-realaudio");

            m_ExtentionToMimeType.Add(".ram", "audio/x-pn-realaudio");
            m_ExtentionToMimeType.Add(".ras", "application/x-cmu-raster");
            m_ExtentionToMimeType.Add(".rast", "image/cmu-raster");
            m_ExtentionToMimeType.Add(".rexx", "text/x-script.rexx");
            m_ExtentionToMimeType.Add(".rf", "image/vnd.rn-realflash");
            m_ExtentionToMimeType.Add(".rgb", "image/x-rgb");
            m_ExtentionToMimeType.Add(".rm", "application/vnd.rn-realmedia");

            m_ExtentionToMimeType.Add(".rmi", "audio/mid");
            m_ExtentionToMimeType.Add(".rmm", "audio/x-pn-realaudio");
            m_ExtentionToMimeType.Add(".rmp", "audio/x-pn-realaudio");
            m_ExtentionToMimeType.Add(".rng", "application/ringing-tones");
            m_ExtentionToMimeType.Add(".rnx", "application/vnd.rn-realplayer");
            m_ExtentionToMimeType.Add(".roff", "application/x-troff");
            m_ExtentionToMimeType.Add(".rp", "image/vnd.rn-realpix");
            m_ExtentionToMimeType.Add(".rpm", "audio/x-pn-realaudio-plugin");
            m_ExtentionToMimeType.Add(".rt", "text/richtext");

            m_ExtentionToMimeType.Add(".rtf", "application/rtf");
            m_ExtentionToMimeType.Add(".rtx", "application/rtf");
            m_ExtentionToMimeType.Add(".rv", "video/vnd.rn-realvideo");
            m_ExtentionToMimeType.Add(".s", "text/x-asm");
            m_ExtentionToMimeType.Add(".s3m", "audio/s3m");
            m_ExtentionToMimeType.Add(".saveme", "application/octet-stream");
            m_ExtentionToMimeType.Add(".sbk", "application/x-tbook");
            m_ExtentionToMimeType.Add(".scm", "application/x-lotusscreencam");
            m_ExtentionToMimeType.Add(".sdml", "text/plain");
            m_ExtentionToMimeType.Add(".sdp", "application/sdp");
            m_ExtentionToMimeType.Add(".sdr", "application/sounder");
            m_ExtentionToMimeType.Add(".sea", "application/sea");
            m_ExtentionToMimeType.Add(".set", "application/set");
            m_ExtentionToMimeType.Add(".sgm", "text/sgml");
            m_ExtentionToMimeType.Add(".sgml", "text/sgml");
            m_ExtentionToMimeType.Add(".sh", "text/x-script.sh");
            m_ExtentionToMimeType.Add(".shar", "application/x-bsh");

            m_ExtentionToMimeType.Add(".shtml", "text/html");

            m_ExtentionToMimeType.Add(".sid", "audio/x-psid");
            m_ExtentionToMimeType.Add(".sit", "application/x-sit");
            m_ExtentionToMimeType.Add(".skd", "application/x-koan");
            m_ExtentionToMimeType.Add(".skm", "application/x-koan");
            m_ExtentionToMimeType.Add(".skp", "application/x-koan");
            m_ExtentionToMimeType.Add(".skt", "application/x-koan");
            m_ExtentionToMimeType.Add(".sl", "application/x-seelogo");
            m_ExtentionToMimeType.Add(".smi", "application/smil");
            m_ExtentionToMimeType.Add(".smil", "application/smil");
            m_ExtentionToMimeType.Add(".snd", "audio/basic");
            m_ExtentionToMimeType.Add(".sol", "application/solids");
            m_ExtentionToMimeType.Add(".spc", "text/x-speech");
            m_ExtentionToMimeType.Add(".spl", "application/futuresplash");
            m_ExtentionToMimeType.Add(".spr", "application/x-sprite");
            m_ExtentionToMimeType.Add(".sprite", "application/x-sprite");
            m_ExtentionToMimeType.Add(".src", "application/x-wais-source");
            m_ExtentionToMimeType.Add(".ssi", "text/x-server-parsed-html");
            m_ExtentionToMimeType.Add(".ssm", "application/streamingmedia");
            m_ExtentionToMimeType.Add(".sst", "application/vnd.ms-pki.certstore");
            m_ExtentionToMimeType.Add(".step", "application/step");
            m_ExtentionToMimeType.Add(".stl", "application/sla");
            m_ExtentionToMimeType.Add(".stp", "application/step");
            m_ExtentionToMimeType.Add(".sv4cpio", "application/x-sv4cpio");
            m_ExtentionToMimeType.Add(".sv4crc", "application/x-sv4crc");
            m_ExtentionToMimeType.Add(".svf", "image/x-dwg");
            m_ExtentionToMimeType.Add(".svr", "application/x-world");
            m_ExtentionToMimeType.Add(".swf", "application/x-shockwave-flash");
            m_ExtentionToMimeType.Add(".t", "application/x-troff");
            m_ExtentionToMimeType.Add(".talk", "text/x-speech");
            m_ExtentionToMimeType.Add(".tar", "application/x-tar");
            m_ExtentionToMimeType.Add(".tbk", "application/toolbook");
            m_ExtentionToMimeType.Add(".tcl", "application/x-tcl");
            m_ExtentionToMimeType.Add(".tcsh", "text/x-script.tcsh");
            m_ExtentionToMimeType.Add(".tex", "application/x-tex");
            m_ExtentionToMimeType.Add(".texi", "application/x-texinfo");
            m_ExtentionToMimeType.Add(".texinfo", "application/x-texinfo");
            m_ExtentionToMimeType.Add(".text", "text/plain");
            m_ExtentionToMimeType.Add(".tgz", "application/x-compressed");
            m_ExtentionToMimeType.Add(".tif", "image/tiff");
            m_ExtentionToMimeType.Add(".tiff", "image/tiff");
            m_ExtentionToMimeType.Add(".tr", "application/x-troff");
            m_ExtentionToMimeType.Add(".tsi", "audio/tsp-audio");
            m_ExtentionToMimeType.Add(".tsp", "application/dsptype");
            m_ExtentionToMimeType.Add(".tsv", "text/tab-separated-values");
            m_ExtentionToMimeType.Add(".turbot", "image/florian");
            m_ExtentionToMimeType.Add(".txt", "text/plain");
            m_ExtentionToMimeType.Add(".uil", "text/x-uil");
            m_ExtentionToMimeType.Add(".uni", "text/uri-list");
            m_ExtentionToMimeType.Add(".unis", "text/uri-list");
            m_ExtentionToMimeType.Add(".unv", "application/i-deas");
            m_ExtentionToMimeType.Add(".uri", "text/uri-list");
            m_ExtentionToMimeType.Add(".uris", "text/uri-list");
            m_ExtentionToMimeType.Add(".ustar", "application/x-ustar");
            m_ExtentionToMimeType.Add(".uu", "text/x-uuencode");
            m_ExtentionToMimeType.Add(".uue", "text/x-uuencode");
            m_ExtentionToMimeType.Add(".vcd", "application/x-cdlink");
            m_ExtentionToMimeType.Add(".vcs", "text/x-vcalendar");
            m_ExtentionToMimeType.Add(".vda", "application/vda");
            m_ExtentionToMimeType.Add(".vdo", "video/vdo");
            m_ExtentionToMimeType.Add(".vew", "application/groupwise");
            m_ExtentionToMimeType.Add(".viv", "video/vivo");
            m_ExtentionToMimeType.Add(".vivo", "video/vivo");
            m_ExtentionToMimeType.Add(".vmd", "application/vocaltec-media-desc");
            m_ExtentionToMimeType.Add(".vmf", "application/vocaltec-media-file");
            m_ExtentionToMimeType.Add(".voc", "audio/voc");
            m_ExtentionToMimeType.Add(".vos", "video/vosaic");
            m_ExtentionToMimeType.Add(".vox", "audio/voxware");
            m_ExtentionToMimeType.Add(".vqe", "audio/x-twinvq-plugin");
            m_ExtentionToMimeType.Add(".vqf", "audio/x-twinvq");
            m_ExtentionToMimeType.Add(".vql", "audio/x-twinvq-plugin");
            m_ExtentionToMimeType.Add(".vrml", "application/x-vrml");
            m_ExtentionToMimeType.Add(".vrt", "x-world/x-vrt");
            m_ExtentionToMimeType.Add(".vsd", "application/x-visio");
            m_ExtentionToMimeType.Add(".vst", "application/x-visio");
            m_ExtentionToMimeType.Add(".vsw", "application/x-visio");
            m_ExtentionToMimeType.Add(".w60", "application/wordperfect6.0");
            m_ExtentionToMimeType.Add(".w61", "application/wordperfect6.1");
            m_ExtentionToMimeType.Add(".w6w", "application/msword");
            m_ExtentionToMimeType.Add(".wav", "audio/wav");
            m_ExtentionToMimeType.Add(".wb1", "application/x-qpro");
            m_ExtentionToMimeType.Add(".wbmp", "image/vnd.wap.wbmp");
            m_ExtentionToMimeType.Add(".web", "application/vnd.xara");
            m_ExtentionToMimeType.Add(".wiz", "application/msword");
            m_ExtentionToMimeType.Add(".wk1", "application/x-123");
            m_ExtentionToMimeType.Add(".wmf", "windows/metafile");
            m_ExtentionToMimeType.Add(".wml", "text/vnd.wap.wml");
            m_ExtentionToMimeType.Add(".wmlc", "application/vnd.wap.wmlc");
            m_ExtentionToMimeType.Add(".wmls", "text/vnd.wap.wmlscript");
            m_ExtentionToMimeType.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
            m_ExtentionToMimeType.Add(".word", "application/msword");
            m_ExtentionToMimeType.Add(".wp", "application/wordperfect");
            m_ExtentionToMimeType.Add(".wp5", "application/wordperfect");
            m_ExtentionToMimeType.Add(".wp6", "application/wordperfect");
            m_ExtentionToMimeType.Add(".wpd", "application/wordperfect");
            m_ExtentionToMimeType.Add(".wq1", "application/x-lotus");
            m_ExtentionToMimeType.Add(".wri", "application/x-wri");
            m_ExtentionToMimeType.Add(".wrl", "model/vrml");
            m_ExtentionToMimeType.Add(".wrz", "model/vrml");
            m_ExtentionToMimeType.Add(".wsc", "text/scriplet");
            m_ExtentionToMimeType.Add(".wsrc", "application/x-wais-source");
            m_ExtentionToMimeType.Add(".wtk", "application/x-wintalk");
            m_ExtentionToMimeType.Add(".xbm", "image/xbm");
            m_ExtentionToMimeType.Add(".xdr", "video/x-amt-demorun");
            m_ExtentionToMimeType.Add(".xgz", "xgl/drawing");
            m_ExtentionToMimeType.Add(".xif", "image/vnd.xiff");
            m_ExtentionToMimeType.Add(".xl", "application/excel");
            m_ExtentionToMimeType.Add(".xla", "application/excel");
            m_ExtentionToMimeType.Add(".xlb", "application/excel");
            
            m_ExtentionToMimeType.Add(".xlc", "application/excel");
            m_ExtentionToMimeType.Add(".xld", "application/excel");
            m_ExtentionToMimeType.Add(".xlk", "application/excel");
            m_ExtentionToMimeType.Add(".xll", "application/excel");
            m_ExtentionToMimeType.Add(".xlm", "application/excel");
            m_ExtentionToMimeType.Add(".xls", "application/excel");
            m_ExtentionToMimeType.Add(".xlt", "application/excel");
            m_ExtentionToMimeType.Add(".xlv", "application/excel");
            m_ExtentionToMimeType.Add(".xlw", "application/excel");
            m_ExtentionToMimeType.Add(".xm", "audio/xm");
            m_ExtentionToMimeType.Add(".xml", "text/xml");
            m_ExtentionToMimeType.Add(".xmz", "xgl/movie");
            m_ExtentionToMimeType.Add(".xpix", "application/x-vnd.ls-xpix");
            m_ExtentionToMimeType.Add(".xpm", "image/xpm");
            m_ExtentionToMimeType.Add(".x-png", "image/png");
            m_ExtentionToMimeType.Add(".xsr", "video/x-amt-showrun");
            m_ExtentionToMimeType.Add(".xwd", "image/x-xwd");
            
            m_ExtentionToMimeType.Add(".xyz", "chemical/x-pdb");
            m_ExtentionToMimeType.Add(".z", "application/x-compress");
            
            m_ExtentionToMimeType.Add(".zip", "application/zip");
            m_ExtentionToMimeType.Add(".zoo", "application/octet-stream");
            m_ExtentionToMimeType.Add(".zsh", "text/x-script.zsh");
        }

        public static string GetMimeType(string extenstion)
        {
            string retVal = "application/octet-stream";
            m_ExtentionToMimeType.TryGetValue(extenstion, out retVal);

            return retVal; 
        }


    }
}