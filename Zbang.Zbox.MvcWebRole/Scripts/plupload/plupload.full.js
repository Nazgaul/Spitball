/*1.5.1.1*/
(function () {
    var f = 0, l = [], n = {}, j = {}, a = {
        "<": "lt", ">": "gt", "&": "amp", '"': "quot", "'": "#39"
    }
    , m = /[<>&\"\']/g, b, c = window.setTimeout, d = {}, e;
    function h() {
        this.returnValue = false
    }
    function k() {
        this.cancelBubble = true
    }
    (function (o) {
        var p = o.split(/,/), q, s, r;
        for (q = 0;
        q < p.length;
        q += 2) {
            r = p[q + 1].split(/ /);
            for (s = 0;
            s < r.length;
            s++) {
                j[r[s]] = p[q]
            }

        }

    })("application/msword,doc dot,application/pdf,pdf,application/pgp-signature,pgp,application/postscript,ps ai eps,application/rtf,rtf,application/vnd.ms-excel,xls xlb,application/vnd.ms-powerpoint,ppt pps pot,application/zip,zip,application/x-shockwave-flash,swf swfl,application/vnd.openxmlformats,docx pptx xlsx,audio/mpeg,mpga mpega mp2 mp3,audio/x-wav,wav,audio/mp4,m4a,image/bmp,bmp,image/gif,gif,image/jpeg,jpeg jpg jpe,image/photoshop,psd,image/png,png,image/svg+xml,svg svgz,image/tiff,tiff tif,text/html,htm html xhtml,text/rtf,rtf,video/mpeg,mpeg mpg mpe,video/quicktime,qt mov,video/mp4,mp4,video/x-m4v,m4v,video/x-flv,flv,video/x-ms-wmv,wmv,video/avi,avi,video/webm,webm,video/vnd.rn-realvideo,rv,text/csv,csv,text/plain,asc txt text diff log,application/octet-stream,exe");
    var g = {
        VERSION: "1.5.1.1", STOPPED: 1, STARTED: 2, QUEUED: 1, UPLOADING: 2, FAILED: 4, DONE: 5, GENERIC_ERROR: -100, HTTP_ERROR: -200, IO_ERROR: -300, SECURITY_ERROR: -400, INIT_ERROR: -500, FILE_SIZE_ERROR: -600, FILE_EXTENSION_ERROR: -601, IMAGE_FORMAT_ERROR: -700, IMAGE_MEMORY_ERROR: -701, IMAGE_DIMENSIONS_ERROR: -702, mimeTypes: j, ua: (function () {
            var s = navigator, r = s.userAgent, t = s.vendor, p, o, q;
            p = /WebKit/.test(r);
            q = p && t.indexOf("Apple") !== -1;
            o = window.opera && window.opera.buildNumber;
            return {
                windows: navigator.platform.indexOf("Win") !== -1, ie: !p && !o && (/MSIE/gi).test(r) && (/Explorer/gi).test(s.appName), webkit: p, gecko: !p && /Gecko/.test(r), safari: q, opera: !!o
            }

        }
        ()), extend: function (o) {
            g.each(arguments, function (p, q) {
                if (q > 0) {
                    g.each(p, function (s, r) {
                        o[r] = s
                    })
                }

            });
            return o
        }
        , cleanName: function (o) {
            var p, q;
            q = [/[\300-\306]/g, "A", /[\340-\346]/g, "a", /\307/g, "C", /\347/g, "c", /[\310-\313]/g, "E", /[\350-\353]/g, "e", /[\314-\317]/g, "I", /[\354-\357]/g, "i", /\321/g, "N", /\361/g, "n", /[\322-\330]/g, "O", /[\362-\370]/g, "o", /[\331-\334]/g, "U", /[\371-\374]/g, "u"];
            for (p = 0;
            p < q.length;
            p += 2) {
                o = o.replace(q[p], q[p + 1])
            }
            o = o.replace(/\s+/g, "_");
            o = o.replace(/[^a-z0-9_\-\.]+/gi, "");
            return o
        }
        , addRuntime: function (o, p) {
            p.name = o;
            l[o] = p;
            l.push(p);
            return p
        }
        , guid: function () {
            var o = new Date().getTime().toString(32), p;
            for (p = 0;
            p < 5;
            p++) {
                o += Math.floor(Math.random() * 65535).toString(32)
            }
            return (g.guidPrefix || "p") + o + (f++).toString(32)
        }
        , buildUrl: function (p, o) {
            var q = "";
            g.each(o, function (s, r) {
                q += (q ? "&" : "") + encodeURIComponent(r) + "=" + encodeURIComponent(s)
            });
            if (q) {
                p += (p.indexOf("?") > 0 ? "&" : "?") + q
            }
            return p
        }
        , each: function (r, s) {
            var q, p, o;
            if (r) {
                q = r.length;
                if (q === b) {
                    for (p in r) {
                        if (r.hasOwnProperty(p)) {
                            if (s(r[p], p) === false) {
                                return
                            }

                        }

                    }

                }
                else {
                    for (o = 0;
                    o < q;
                    o++) {
                        if (s(r[o], o) === false) {
                            return
                        }

                    }

                }

            }

        }
        , formatSize: function (o) {
            if (o === b || /\D/.test(o)) {
                return g.translate("N/A")
            }
            if (o > 1073741824) {
                return Math.round(o / 1073741824, 1) + " GB"
            }
            if (o > 1048576) {
                return Math.round(o / 1048576, 1) + " MB"
            }
            if (o > 1024) {
                return Math.round(o / 1024, 1) + " KB"
            }
            return o + " b"
        }
        , getPos: function (p, t) {
            var u = 0, s = 0, w, v = document, q, r;
            p = p;
            t = t || v.body;
            function o(C) {
                var A, B, z = 0, D = 0;
                if (C) {
                    B = C.getBoundingClientRect();
                    A = v.compatMode === "CSS1Compat" ? v.documentElement : v.body;
                    z = B.left + A.scrollLeft;
                    D = B.top + A.scrollTop
                }
                return {
                    x: z, y: D
                }

            }
            if (p && p.getBoundingClientRect && (navigator.userAgent.indexOf("MSIE") > 0 && v.documentMode !== 8)) {
                q = o(p);
                r = o(t);
                return {
                    x: q.x - r.x, y: q.y - r.y
                }

            }
            w = p;
            while (w && w != t && w.nodeType) {
                u += w.offsetLeft || 0;
                s += w.offsetTop || 0;
                w = w.offsetParent
            }
            w = p.parentNode;
            while (w && w != t && w.nodeType) {
                u -= w.scrollLeft || 0;
                s -= w.scrollTop || 0;
                w = w.parentNode
            }
            return {
                x: u, y: s
            }

        }
        , getSize: function (o) {
            return {
                w: o.offsetWidth || o.clientWidth, h: o.offsetHeight || o.clientHeight
            }

        }
        , parseSize: function (o) {
            var p;
            if (typeof (o) == "string") {
                o = /^([0-9]+)([mgk]?)$/.exec(o.toLowerCase().replace(/[^0-9mkg]/g, ""));
                p = o[2];
                o = +o[1];
                if (p == "g") {
                    o *= 1073741824
                }
                if (p == "m") {
                    o *= 1048576
                }
                if (p == "k") {
                    o *= 1024
                }

            }
            return o
        }
        , xmlEncode: function (o) {
            return o ? ("" + o).replace(m, function (p) {
                return a[p] ? "&" + a[p] + ";" : p
            }) : o
        }
        , toArray: function (q) {
            var p, o = [];
            for (p = 0;
            p < q.length;
            p++) {
                o[p] = q[p]
            }
            return o
        }
        , addI18n: function (o) {
            return g.extend(n, o)
        }
        , translate: function (o) {
            return n[o] || o
        }
        , isEmptyObj: function (o) {
            if (o === b) {
                return true
            }
            for (var p in o) {
                return false
            }
            return true
        }
        , hasClass: function (q, p) {
            var o;
            if (q.className == "") {
                return false
            }
            o = new RegExp("(^|\\s+)" + p + "(\\s+|$)");
            return o.test(q.className)
        }
        , addClass: function (p, o) {
            if (!g.hasClass(p, o)) {
                p.className = p.className == "" ? o : p.className.replace(/\s+$/, "") + " " + o
            }

        }
        , removeClass: function (q, p) {
            var o = new RegExp("(^|\\s+)" + p + "(\\s+|$)");
            q.className = q.className.replace(o, function (s, r, t) {
                return r === " " && t === " " ? " " : ""
            })
        }
        , getStyle: function (p, o) {
            if (p.currentStyle) {
                return p.currentStyle[o]
            }
            else {
                if (window.getComputedStyle) {
                    return window.getComputedStyle(p, null)[o]
                }

            }

        }
        , addEvent: function (t, o, u) {
            var s, r, q, p;
            p = arguments[3];
            o = o.toLowerCase();
            if (e === b) {
                e = "Plupload_" + g.guid()
            }
            if (t.addEventListener) {
                s = u;
                t.addEventListener(o, s, false)
            }
            else {
                if (t.attachEvent) {
                    s = function () {
                        var v = window.event;
                        if (!v.target) {
                            v.target = v.srcElement
                        }
                        v.preventDefault = h;
                        v.stopPropagation = k;
                        u(v)
                    };
                    t.attachEvent("on" + o, s)
                }

            }
            if (t[e] === b) {
                t[e] = g.guid()
            }
            if (!d.hasOwnProperty(t[e])) {
                d[t[e]] = {}
            }
            r = d[t[e]];
            if (!r.hasOwnProperty(o)) {
                r[o] = []
            }
            r[o].push({
                func: s, orig: u, key: p
            })
        }
        , removeEvent: function (t, o) {
            var r, u, q;
            if (typeof (arguments[2]) == "function") {
                u = arguments[2]
            }
            else {
                q = arguments[2]
            }
            o = o.toLowerCase();
            if (t[e] && d[t[e]] && d[t[e]][o]) {
                r = d[t[e]][o]
            }
            else {
                return
            }
            for (var p = r.length - 1;
            p >= 0;
            p--) {
                if (r[p].key === q || r[p].orig === u) {
                    if (t.detachEvent) {
                        t.detachEvent("on" + o, r[p].func)
                    }
                    else {
                        if (t.removeEventListener) {
                            t.removeEventListener(o, r[p].func, false)
                        }

                    }
                    r[p].orig = null;
                    r[p].func = null;
                    r.splice(p, 1);
                    if (u !== b) {
                        break
                    }

                }

            }
            if (!r.length) {
                delete d[t[e]][o]
            }
            if (g.isEmptyObj(d[t[e]])) {
                delete d[t[e]];
                try {
                    delete t[e]
                }
                catch (s) {
                    t[e] = b
                }

            }

        }
        , removeAllEvents: function (p) {
            var o = arguments[1];
            if (p[e] === b || !p[e]) {
                return
            }
            g.each(d[p[e]], function (r, q) {
                g.removeEvent(p, q, o)
            })
        }

    };
    g.Uploader = function (r) {
        var p = {}, u, t = [], q;
        u = new g.QueueProgress();
        r = g.extend({
            chunk_size: 0, multipart: true, multi_selection: true, file_data_name: "file", filters: []
        }
        , r);
        function s() {
            var w, x = 0, v;
            if (this.state == g.STARTED) {
                for (v = 0;
                v < t.length;
                v++) {
                    if (!w && t[v].status == g.QUEUED) {
                        w = t[v];
                        w.status = g.UPLOADING;
                        if (this.trigger("BeforeUpload", w)) {
                            this.trigger("UploadFile", w)
                        }

                    }
                    else {
                        x++
                    }

                }
                if (x == t.length) {
                    this.stop();
                    this.trigger("UploadComplete", t)
                }

            }

        }
        function o() {
            var w, v;
            u.reset();
            for (w = 0;
            w < t.length;
            w++) {
                v = t[w];
                if (v.size !== b) {
                    u.size += v.size;
                    u.loaded += v.loaded
                }
                else {
                    u.size = b
                }
                if (v.status == g.DONE) {
                    u.uploaded++
                }
                else {
                    if (v.status == g.FAILED) {
                        u.failed++
                    }
                    else {
                        u.queued++
                    }

                }

            }
            if (u.size === b) {
                u.percent = t.length > 0 ? Math.ceil(u.uploaded / t.length * 100) : 0
            }
            else {
                u.bytesPerSec = Math.ceil(u.loaded / ((+new Date() - q || 1) / 1000));
                u.percent = u.size > 0 ? Math.ceil(u.loaded / u.size * 100) : 0
            }

        }
        g.extend(this, {
            state: g.STOPPED, runtime: "", features: {}, files: t, settings: r, total: u, id: g.guid(), init: function () {
                var A = this, B, x, w, z = 0, y;
                if (typeof (r.preinit) == "function") {
                    r.preinit(A)
                }
                else {
                    g.each(r.preinit, function (D, C) {
                        A.bind(C, D)
                    })
                }
                r.page_url = r.page_url || document.location.pathname.replace(/\/[^\/]+$/g, "/");
                if (!/^(\w+:\/\/|\/)/.test(r.url)) {
                    r.url = r.page_url + r.url
                }
                r.chunk_size = g.parseSize(r.chunk_size);
                r.max_file_size = g.parseSize(r.max_file_size);
                A.bind("FilesAdded", function (C, F) {
                    var E, D, H = 0, I, G = r.filters;
                    if (G && G.length) {
                        I = [];
                        g.each(G, function (J) {
                            g.each(J.extensions.split(/,/), function (K) {
                                if (/^\s*\*\s*$/.test(K)) {
                                    I.push("\\.*")
                                }
                                else {
                                    I.push("\\." + K.replace(new RegExp("[" + ("/^$.*+?|()[]{}\\".replace(/./g, "\\$&")) + "]", "g"), "\\$&"))
                                }

                            })
                        });
                        I = new RegExp(I.join("|") + "$", "i")
                    }
                    for (E = 0;
                    E < F.length;
                    E++) {
                        D = F[E];
                        D.loaded = 0;
                        D.percent = 0;
                        D.status = g.QUEUED;
                        if (I && !I.test(D.name)) {
                            C.trigger("Error", {
                                code: g.FILE_EXTENSION_ERROR, message: g.translate("File extension error."), file: D
                            });
                            continue
                        }
                        if (D.size !== b && D.size > r.max_file_size) {
                            C.trigger("Error", {
                                code: g.FILE_SIZE_ERROR, message: g.translate("File size error."), file: D
                            });
                            continue
                        }
                        t.push(D);
                        H++
                    }
                    if (H) {
                        c(function () {
                            A.trigger("QueueChanged");
                            A.refresh()
                        }
                        , 1)
                    }
                    else {
                        return false
                    }

                });
                if (r.unique_names) {
                    A.bind("UploadFile", function (C, D) {
                        var F = D.name.match(/\.([^.]+)$/), E = "tmp";
                        if (F) {
                            E = F[1]
                        }
                        D.target_name = D.id + "." + E
                    })
                }
                A.bind("UploadProgress", function (C, D) {
                    D.percent = D.size > 0 ? Math.ceil(D.loaded / D.size * 100) : 100;
                    o()
                });
                A.bind("StateChanged", function (C) {
                    if (C.state == g.STARTED) {
                        q = (+new Date())
                    }
                    else {
                        if (C.state == g.STOPPED) {
                            for (B = C.files.length - 1;
                            B >= 0;
                            B--) {
                                if (C.files[B].status == g.UPLOADING) {
                                    C.files[B].status = g.QUEUED;
                                    o()
                                }

                            }

                        }

                    }

                });
                A.bind("QueueChanged", o);
                A.bind("Error", function (C, D) {
                    if (D.file) {
                        D.file.status = g.FAILED;
                        o();
                        if (C.state == g.STARTED) {
                            c(function () {
                                s.call(A)
                            }
                            , 1)
                        }

                    }

                });
                A.bind("FileUploaded", function (C, D) {
                    D.status = g.DONE;
                    D.loaded = D.size;
                    C.trigger("UploadProgress", D);
                    c(function () {
                        s.call(A)
                    }
                    , 1)
                });
                if (r.runtimes) {
                    x = [];
                    y = r.runtimes.split(/\s?,\s?/);
                    for (B = 0;
                    B < y.length;
                    B++) {
                        if (l[y[B]]) {
                            x.push(l[y[B]])
                        }

                    }

                }
                else {
                    x = l
                }
                function v() {
                    var F = x[z++], E, C, D;
                    if (F) {
                        E = F.getFeatures();
                        C = A.settings.required_features;
                        if (C) {
                            C = C.split(",");
                            for (D = 0;
                            D < C.length;
                            D++) {
                                if (!E[C[D]]) {
                                    v();
                                    return
                                }

                            }

                        }
                        F.init(A, function (G) {
                            if (G && G.success) {
                                A.features = E;
                                A.runtime = F.name;
                                A.trigger("Init", {
                                    runtime: F.name
                                });
                                A.trigger("PostInit");
                                A.refresh()
                            }
                            else {
                                v()
                            }

                        })
                    }
                    else {
                        A.trigger("Error", {
                            code: g.INIT_ERROR, message: g.translate("Init error.")
                        })
                    }

                }
                v();
                if (typeof (r.init) == "function") {
                    r.init(A)
                }
                else {
                    g.each(r.init, function (D, C) {
                        A.bind(C, D)
                    })
                }

            }
            , refresh: function () {
                this.trigger("Refresh")
            }
            , start: function () {
                if (this.state != g.STARTED) {
                    this.state = g.STARTED;
                    this.trigger("StateChanged");
                    s.call(this)
                }

            }
            , stop: function () {
                if (this.state != g.STOPPED) {
                    this.state = g.STOPPED;
                    this.trigger("StateChanged")
                }

            }
            , getFile: function (w) {
                var v;
                for (v = t.length - 1;
                v >= 0;
                v--) {
                    if (t[v].id === w) {
                        return t[v]
                    }

                }

            }
            , removeFile: function (w) {
                var v;
                for (v = t.length - 1;
                v >= 0;
                v--) {
                    if (t[v].id === w.id) {
                        return this.splice(v, 1)[0]
                    }

                }

            }
            , splice: function (x, v) {
                var w;
                w = t.splice(x === b ? 0 : x, v === b ? t.length : v);
                this.trigger("FilesRemoved", w);
                this.trigger("QueueChanged");
                return w
            }
            , trigger: function (w) {
                var y = p[w.toLowerCase()], x, v;
                if (y) {
                    v = Array.prototype.slice.call(arguments);
                    v[0] = this;
                    for (x = 0;
                    x < y.length;
                    x++) {
                        if (y[x].func.apply(y[x].scope, v) === false) {
                            return false
                        }

                    }

                }
                return true
            }
            , hasEventListener: function (v) {
                return !!p[v.toLowerCase()]
            }
            , bind: function (v, x, w) {
                var y;
                v = v.toLowerCase();
                y = p[v] || [];
                y.push({
                    func: x, scope: w || this
                });
                p[v] = y
            }
            , unbind: function (v) {
                v = v.toLowerCase();
                var y = p[v], w, x = arguments[1];
                if (y) {
                    if (x !== b) {
                        for (w = y.length - 1;
                        w >= 0;
                        w--) {
                            if (y[w].func === x) {
                                y.splice(w, 1);
                                break
                            }

                        }

                    }
                    else {
                        y = []
                    }
                    if (!y.length) {
                        delete p[v]
                    }

                }

            }
            , unbindAll: function () {
                var v = this;
                g.each(p, function (x, w) {
                    v.unbind(w)
                })
            }
            , destroy: function () {
                this.trigger("Destroy");
                this.unbindAll()
            }

        })
    };
    g.File = function (r, p, q) {
        var o = this;
        o.id = r;
        o.name = p;
        o.size = q;
        o.loaded = 0;
        o.percent = 0;
        o.status = 0
    };
    g.Runtime = function () {
        this.getFeatures = function () { };
        this.init = function (o, p) { }
    };
    g.QueueProgress = function () {
        var o = this;
        o.size = 0;
        o.loaded = 0;
        o.uploaded = 0;
        o.failed = 0;
        o.queued = 0;
        o.percent = 0;
        o.bytesPerSec = 0;
        o.reset = function () {
            o.size = o.loaded = o.uploaded = o.failed = o.queued = o.percent = o.bytesPerSec = 0
        }

    };
    g.runtimes = {};
    window.plupload = g
})();
(function () {
    if (window.google && google.gears) {
        return
    }
    var a = null;
    if (typeof GearsFactory != "undefined") {
        a = new GearsFactory()
    }
    else {
        try {
            a = new ActiveXObject("Gears.Factory");
            if (a.getBuildInfo().indexOf("ie_mobile") != -1) {
                a.privateSetGlobalObject(this)
            }

        }
        catch (b) {
            if ((typeof navigator.mimeTypes != "undefined") && navigator.mimeTypes["application/x-googlegears"]) {
                a = document.createElement("object");
                a.style.display = "none";
                a.width = 0;
                a.height = 0;
                a.type = "application/x-googlegears";
                document.documentElement.appendChild(a)
            }

        }

    }
    if (!a) {
        return
    }
    if (!window.google) {
        window.google = {}
    }
    if (!google.gears) {
        google.gears = {
            factory: a
        }

    }

})();
(function (e, b, c, d) {
    var f = {};
    function a(h, k, m) {
        var g, j, l, o;
        j = google.gears.factory.create("beta.canvas");
        try {
            j.decode(h);
            if (!k.width) {
                k.width = j.width
            }
            if (!k.height) {
                k.height = j.height
            }
            o = Math.min(width / j.width, height / j.height);
            if (o < 1 || (o === 1 && m === "image/jpeg")) {
                j.resize(Math.round(j.width * o), Math.round(j.height * o));
                if (k.quality) {
                    return j.encode(m, {
                        quality: k.quality / 100
                    })
                }
                return j.encode(m)
            }

        }
        catch (n) { } return h
    }
    c.runtimes.Gears = c.addRuntime("gears", {
        getFeatures: function () {
            return {
                dragdrop: true, jpgresize: true, pngresize: true, chunks: true, progress: true, multipart: true, multi_selection: true
            }

        }
        , init: function (j, l) {
            var k;
            if (!e.google || !google.gears) {
                return l({
                    success: false
                })
            }
            try {
                k = google.gears.factory.create("beta.desktop")
            }
            catch (h) {
                return l({
                    success: false
                })
            }
            function g(o) {
                var n, m, p = [], q;
                for (m = 0;
                m < o.length;
                m++) {
                    n = o[m];
                    q = c.guid();
                    f[q] = n.blob;
                    p.push(new c.File(q, n.name, n.blob.length))
                }
                j.trigger("FilesAdded", p)
            }
            j.bind("PostInit", function () {
                var n = j.settings, m = b.getElementById(n.drop_element);
                if (m) {
                    c.addEvent(m, "dragover", function (o) {
                        k.setDropEffect(o, "copy");
                        o.preventDefault()
                    }
                    , j.id);
                    c.addEvent(m, "drop", function (p) {
                        var o = k.getDragData(p, "application/x-gears-files");
                        if (o) {
                            g(o.files)
                        }
                        p.preventDefault()
                    }
                    , j.id);
                    m = 0
                }
                c.addEvent(b.getElementById(n.browse_button), "click", function (s) {
                    var r = [], p, o, q;
                    s.preventDefault();
                    no_type_restriction: for (p = 0;
                    p < n.filters.length;
                    p++) {
                        q = n.filters[p].extensions.split(",");
                        for (o = 0;
                        o < q.length;
                        o++) {
                            if (q[o] === "*") {
                                r = [];
                                break no_type_restriction
                            }
                            r.push("." + q[o])
                        }

                    }
                    k.openFiles(g, {
                        singleFile: !n.multi_selection, filter: r
                    })
                }
                , j.id)
            });
            j.bind("UploadFile", function (s, p) {
                var u = 0, t, q, r = 0, o = s.settings.resize, m;
                if (o && /\.(png|jpg|jpeg)$/i.test(p.name)) {
                    f[p.id] = a(f[p.id], o, /\.png$/i.test(p.name) ? "image/png" : "image/jpeg")
                }
                p.size = f[p.id].length;
                q = s.settings.chunk_size;
                m = q > 0;
                t = Math.ceil(p.size / q);
                if (!m) {
                    q = p.size;
                    t = 1
                }
                function n() {
                    var z, B, w = s.settings.multipart, v = 0, A = {
                        name: p.target_name || p.name
                    }
                    , x = s.settings.url;
                    function y(D) {
                        var C, I = "----pluploadboundary" + c.guid(), F = "--", H = "\r\n", E, G;
                        if (w) {
                            z.setRequestHeader("Content-Type", "multipart/form-data; boundary=" + I);
                            C = google.gears.factory.create("beta.blobbuilder");
                            c.each(c.extend(A, s.settings.multipart_params), function (K, J) {
                                C.append(F + I + H + 'Content-Disposition: form-data; name="' + J + '"' + H + H);
                                C.append(K + H)
                            });
                            G = c.mimeTypes[p.name.replace(/^.+\.([^.]+)/, "$1").toLowerCase()] || "application/octet-stream";
                            C.append(F + I + H + 'Content-Disposition: form-data; name="' + s.settings.file_data_name + '"; filename="' + p.name + '"' + H + "Content-Type: " + G + H + H);
                            C.append(D);
                            C.append(H + F + I + F + H);
                            E = C.getAsBlob();
                            v = E.length - D.length;
                            D = E
                        }                        
                       setTimeout(z.send(D),100)
                    }
                    if (p.status == c.DONE || p.status == c.FAILED || s.state == c.STOPPED) {
                        return
                    }
                    if (m) {
                        A.chunk = u;
                        A.chunks = t
                    }
                    B = Math.min(q, p.size - (u * q));
                    if (!w) {
                        x = c.buildUrl(s.settings.url, A)
                    }
                    z = google.gears.factory.create("beta.httprequest");
                    z.open("POST", x);
                    if (!w) {
                        z.setRequestHeader("Content-Disposition", 'attachment; filename="' + p.name + '"');
                        z.setRequestHeader("Content-Type", "application/octet-stream")
                    }
                    c.each(s.settings.headers, function (D, C) {
                        z.setRequestHeader(C, D)
                    });
                    z.upload.onprogress = function (C) {
                        p.loaded = r + C.loaded - v;
                        s.trigger("UploadProgress", p)
                    };
                    z.onreadystatechange = function () {
                        var C;
                        if (z.readyState == 4) {
                            if (z.status == 200) {
                                C = {
                                    chunk: u, chunks: t, response: z.responseText, status: z.status
                                };
                                s.trigger("ChunkUploaded", p, C);
                                if (C.cancelled) {
                                    p.status = c.FAILED;
                                    return
                                }
                                r += B;
                                if (++u >= t) {
                                    p.status = c.DONE;
                                    s.trigger("FileUploaded", p, {
                                        response: z.responseText, status: z.status
                                    })
                                }
                                else {
                                    n()
                                }

                            }
                            else {
                                s.trigger("Error", {
                                    code: c.HTTP_ERROR, message: c.translate("HTTP Error."), file: p, chunk: u, chunks: t, status: z.status
                                })
                            }

                        }

                    };
                    if (u < t) {
                        y(f[p.id].slice(u * q, B))
                    }

                }
                n()
            });
            j.bind("Destroy", function (m) {
                var n, o, p = {
                    browseButton: m.settings.browse_button, dropElm: m.settings.drop_element
                };
                for (n in p) {
                    o = b.getElementById(p[n]);
                    if (o) {
                        c.removeAllEvents(o, m.id)
                    }

                }

            });
            l({
                success: true
            })
        }

    })
})(window, document, plupload);
(function (g, b, d, e) {
    var a = {}, h = {};
    function c(o) {
        var n, m = typeof o, j, l, k;
        if (o === e || o === null) {
            return "null"
        }
        if (m === "string") {
            n = "\bb\tt\nn\ff\rr\"\"''\\\\";
            return '"' + o.replace(/([\u0080-\uFFFF\x00-\x1f\"])/g, function (r, q) {
                var p = n.indexOf(q);
                if (p + 1) {
                    return "\\" + n.charAt(p + 1)
                }
                r = q.charCodeAt().toString(16);
                return "\\u" + "0000".substring(r.length) + r
            }) + '"'
        }
        if (m == "object") {
            j = o.length !== e;
            n = "";
            if (j) {
                for (l = 0;
                l < o.length;
                l++) {
                    if (n) {
                        n += ","
                    }
                    n += c(o[l])
                }
                n = "[" + n + "]"
            }
            else {
                for (k in o) {
                    if (o.hasOwnProperty(k)) {
                        if (n) {
                            n += ","
                        }
                        n += c(k) + ":" + c(o[k])
                    }

                }
                n = "{" + n + "}"
            }
            return n
        }
        return "" + o
    }
    function f(s) {
        var v = false, j = null, o = null, k, l, m, u, n, q = 0;
        try {
            try {
                o = new ActiveXObject("AgControl.AgControl");
                if (o.IsVersionSupported(s)) {
                    v = true
                }
                o = null
            }
            catch (r) {
                var p = navigator.plugins["Silverlight Plug-In"];
                if (p) {
                    k = p.description;
                    if (k === "1.0.30226.2") {
                        k = "2.0.30226.2"
                    }
                    l = k.split(".");
                    while (l.length > 3) {
                        l.pop()
                    }
                    while (l.length < 4) {
                        l.push(0)
                    }
                    m = s.split(".");
                    while (m.length > 4) {
                        m.pop()
                    }
                    do {
                        u = parseInt(m[q], 10);
                        n = parseInt(l[q], 10);
                        q++
                    }
                    while (q < m.length && u === n);
                    if (u <= n && !isNaN(u)) {
                        v = true
                    }

                }

            }

        }
        catch (t) {
            v = false
        }
        return v
    }
    d.silverlight = {
        trigger: function (n, k) {
            var m = a[n], l, j;
            if (m) {
                j = d.toArray(arguments).slice(1);
                j[0] = "Silverlight:" + k;
                setTimeout(function () {
                    m.trigger.apply(m, j)
                }
                , 0)
            }

        }

    };
    d.runtimes.Silverlight = d.addRuntime("silverlight", {
        getFeatures: function () {
            return {
                jpgresize: true, pngresize: true, chunks: true, progress: true, multipart: true, multi_selection: true
            }

        }
        , init: function (p, q) {
            var o, m = "", n = p.settings.filters, l, k = b.body;
            if (!f("2.0.31005.0") || (g.opera && g.opera.buildNumber)) {
                q({
                    success: false
                });
                return
            }
            h[p.id] = false;
            a[p.id] = p;
            o = b.createElement("div");
            o.id = p.id + "_silverlight_container";
            d.extend(o.style, {
                position: "absolute", top: "0px", background: p.settings.shim_bgcolor || "transparent", zIndex: 99999, width: "100px", height: "100px", overflow: "hidden", opacity: p.settings.shim_bgcolor || b.documentMode > 8 ? "" : 0.01
            });
            o.className = "plupload silverlight";
            if (p.settings.container) {
                k = b.getElementById(p.settings.container);
                if (d.getStyle(k, "position") === "static") {
                    k.style.position = "relative"
                }

            }
            k.appendChild(o);
            for (l = 0;
            l < n.length;
            l++) {
                m += (m != "" ? "|" : "") + n[l].title + " | *." + n[l].extensions.replace(/,/g, ";*.")
            }
            o.innerHTML = '<object id="' + p.id + '_silverlight" data="data:application/x-silverlight," type="application/x-silverlight-2" style="outline:none;" width="1024" height="1024"><param name="source" value="' + p.settings.silverlight_xap_url + '"/><param name="background" value="Transparent"/><param name="windowless" value="true"/><param name="enablehtmlaccess" value="true"/><param name="initParams" value="id=' + p.id + ",filter=" + m + ",multiselect=" + p.settings.multi_selection + '"/></object>';
            function j() {
                return b.getElementById(p.id + "_silverlight").content.Upload
            }
            p.bind("Silverlight:Init", function () {
                var r, s = {};
                if (h[p.id]) {
                    return
                }
                h[p.id] = true;
                p.bind("Silverlight:StartSelectFiles", function (t) {
                    r = []
                });
                p.bind("Silverlight:SelectFile", function (t, w, u, v) {
                    var x;
                    x = d.guid();
                    s[x] = w;
                    s[w] = x;
                    r.push(new d.File(x, u, v))
                });
                p.bind("Silverlight:SelectSuccessful", function () {
                    if (r.length) {
                        p.trigger("FilesAdded", r)
                    }

                });
                p.bind("Silverlight:UploadChunkError", function (t, w, u, x, v) {
                    p.trigger("Error", {
                        code: d.IO_ERROR, message: "IO Error.", details: v, file: t.getFile(s[w])
                    })
                });
                p.bind("Silverlight:UploadFileProgress", function (t, x, u, w) {
                    var v = t.getFile(s[x]);
                    if (v.status != d.FAILED) {
                        v.size = w;
                        v.loaded = u;
                        t.trigger("UploadProgress", v)
                    }

                });
                p.bind("Refresh", function (t) {
                    var u, v, w;
                    u = b.getElementById(t.settings.browse_button);
                    if (u) {
                        v = d.getPos(u, b.getElementById(t.settings.container));
                        w = d.getSize(u);
                        d.extend(b.getElementById(t.id + "_silverlight_container").style, {
                            top: v.y + "px", left: v.x + "px", width: w.w + "px", height: w.h + "px"
                        })
                    }

                });
                p.bind("Silverlight:UploadChunkSuccessful", function (t, w, u, z, y) {
                    var x, v = t.getFile(s[w]);
                    x = {
                        chunk: u, chunks: z, response: y
                    };
                    t.trigger("ChunkUploaded", v, x);
                    if (v.status != d.FAILED) {
                        j().UploadNextChunk()
                    }
                    if (u == z - 1) {
                        v.status = d.DONE;
                        t.trigger("FileUploaded", v, {
                            response: y
                        })
                    }

                });
                p.bind("Silverlight:UploadSuccessful", function (t, w, u) {
                    var v = t.getFile(s[w]);
                    v.status = d.DONE;
                    t.trigger("FileUploaded", v, {
                        response: u
                    })
                });
                p.bind("FilesRemoved", function (t, v) {
                    var u;
                    for (u = 0;
                    u < v.length;
                    u++) {
                        j().RemoveFile(s[v[u].id])
                    }

                });
                p.bind("UploadFile", function (t, v) {
                    var w = t.settings, u = w.resize || {};
                    j().UploadFile(s[v.id], t.settings.url, c({
                        name: v.target_name || v.name, mime: d.mimeTypes[v.name.replace(/^.+\.([^.]+)/, "$1").toLowerCase()] || "application/octet-stream", chunk_size: w.chunk_size, image_width: u.width, image_height: u.height, image_quality: u.quality || 90, multipart: !!w.multipart, multipart_params: w.multipart_params || {}, file_data_name: w.file_data_name, headers: w.headers
                    }))
                });
                p.bind("Silverlight:MouseEnter", function (t) {
                    var u, v;
                    u = b.getElementById(p.settings.browse_button);
                    v = t.settings.browse_button_hover;
                    if (u && v) {
                        d.addClass(u, v)
                    }

                });
                p.bind("Silverlight:MouseLeave", function (t) {
                    var u, v;
                    u = b.getElementById(p.settings.browse_button);
                    v = t.settings.browse_button_hover;
                    if (u && v) {
                        d.removeClass(u, v)
                    }

                });
                p.bind("Silverlight:MouseLeftButtonDown", function (t) {
                    var u, v;
                    u = b.getElementById(p.settings.browse_button);
                    v = t.settings.browse_button_active;
                    if (u && v) {
                        d.addClass(u, v);
                        d.addEvent(b.body, "mouseup", function () {
                            d.removeClass(u, v)
                        })
                    }

                });
                p.bind("Sliverlight:StartSelectFiles", function (t) {
                    var u, v;
                    u = b.getElementById(p.settings.browse_button);
                    v = t.settings.browse_button_active;
                    if (u && v) {
                        d.removeClass(u, v)
                    }

                });
                p.bind("Destroy", function (t) {
                    var u;
                    d.removeAllEvents(b.body, t.id);
                    delete h[t.id];
                    delete a[t.id];
                    u = b.getElementById(t.id + "_silverlight_container");
                    if (u) {
                        k.removeChild(u)
                    }

                });
                q({
                    success: true
                })
            })
        }

    })
})(window, document, plupload);
(function (f, b, d, e) {
    var a = {}, g = {};
    function c() {
        var h;
        try {
            h = navigator.plugins["Shockwave Flash"];
            h = h.description
        }
        catch (k) {
            try {
                h = new ActiveXObject("ShockwaveFlash.ShockwaveFlash").GetVariable("$version")
            }
            catch (j) {
                h = "0.0"
            }

        }
        h = h.match(/\d+/g);
        return parseFloat(h[0] + "." + h[1])
    }
    d.flash = {
        trigger: function (k, h, j) {
            setTimeout(function () {
                var n = a[k], m, l;
                if (n) {
                    n.trigger("Flash:" + h, j)
                }

            }
            , 0)
        }

    };
    d.runtimes.Flash = d.addRuntime("flash", {
        getFeatures: function () {
            return {
                jpgresize: true, pngresize: true, maxWidth: 8091, maxHeight: 8091, chunks: true, progress: true, multipart: true, multi_selection: true
            }

        }
        , init: function (n, p) {
            var l, m, h = 0, j = b.body;
            if (c() < 10) {
                p({
                    success: false
                });
                return
            }
            g[n.id] = false;
            a[n.id] = n;
            l = b.getElementById(n.settings.browse_button);
            m = b.createElement("div");
            m.id = n.id + "_flash_container";
            d.extend(m.style, {
                position: "absolute", top: "0px", background: n.settings.shim_bgcolor || "transparent", zIndex: 99999, width: "100%", height: "100%"
            });
            m.className = "plupload flash";
            if (n.settings.container) {
                j = b.getElementById(n.settings.container);
                if (d.getStyle(j, "position") === "static") {
                    j.style.position = "relative"
                }

            }
            j.appendChild(m);
            (function () {
                var q, r;
                q = '<object id="' + n.id + '_flash" type="application/x-shockwave-flash" data="' + n.settings.flash_swf_url + '" ';
                if (d.ua.ie) {
                    q += 'classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" '
                }
                q += 'width="100%" height="100%" style="outline:0"><param name="movie" value="' + n.settings.flash_swf_url + '" /><param name="flashvars" value="id=' + escape(n.id) + '" /><param name="wmode" value="transparent" /><param name="allowscriptaccess" value="always" /></object>';
                if (d.ua.ie) {
                    r = b.createElement("div");
                    m.appendChild(r);
                    r.outerHTML = q;
                    r = null
                }
                else {
                    m.innerHTML = q
                }

            }
            ());
            function o() {
                return b.getElementById(n.id + "_flash")
            }
            function k() {
                if (h++ > 5000) {
                    p({
                        success: false
                    });
                    return
                }
                if (!g[n.id]) {
                    setTimeout(k, 1)
                }

            }
            k();
            l = m = null;
            n.bind("Flash:Init", function () {
                var r = {}, q;
                o().setFileFilters(n.settings.filters, n.settings.multi_selection);
                if (g[n.id]) {
                    return
                }
                g[n.id] = true;
                n.bind("UploadFile", function (s, u) {
                    var v = s.settings, t = n.settings.resize || {};
                    o().uploadFile(r[u.id], v.url, {
                        name: u.target_name || u.name, mime: d.mimeTypes[u.name.replace(/^.+\.([^.]+)/, "$1").toLowerCase()] || "application/octet-stream", chunk_size: v.chunk_size, width: t.width, height: t.height, quality: t.quality, multipart: v.multipart, multipart_params: v.multipart_params || {}, file_data_name: v.file_data_name, format: /\.(jpg|jpeg)$/i.test(u.name) ? "jpg" : "png", headers: v.headers, urlstream_upload: v.urlstream_upload
                    })
                });
                n.bind("Flash:UploadProcess", function (t, s) {
                    var u = t.getFile(r[s.id]);
                    if (u.status != d.FAILED) {
                        u.loaded = s.loaded;
                        u.size = s.size;
                        t.trigger("UploadProgress", u)
                    }

                });
                n.bind("Flash:UploadChunkComplete", function (s, u) {
                    var v, t = s.getFile(r[u.id]);
                    v = {
                        chunk: u.chunk, chunks: u.chunks, response: u.text
                    };
                    s.trigger("ChunkUploaded", t, v);
                    if (t.status != d.FAILED) {
                        o().uploadNextChunk()
                    }
                    if (u.chunk == u.chunks - 1) {
                        t.status = d.DONE;
                        s.trigger("FileUploaded", t, {
                            response: u.text
                        })
                    }

                });
                n.bind("Flash:SelectFiles", function (s, v) {
                    var u, t, w = [], x;
                    for (t = 0;
                    t < v.length;
                    t++) {
                        u = v[t];
                        x = d.guid();
                        r[x] = u.id;
                        r[u.id] = x;
                        w.push(new d.File(x, u.name, u.size))
                    }
                    if (w.length) {
                        n.trigger("FilesAdded", w)
                    }

                });
                n.bind("Flash:SecurityError", function (s, t) {
                    n.trigger("Error", {
                        code: d.SECURITY_ERROR, message: d.translate("Security error."), details: t.message, file: n.getFile(r[t.id])
                    })
                });
                n.bind("Flash:GenericError", function (s, t) {
                    n.trigger("Error", {
                        code: d.GENERIC_ERROR, message: d.translate("Generic error."), details: t.message, file: n.getFile(r[t.id])
                    })
                });
                n.bind("Flash:IOError", function (s, t) {
                    n.trigger("Error", {
                        code: d.IO_ERROR, message: d.translate("IO error."), details: t.message, file: n.getFile(r[t.id])
                    })
                });
                n.bind("Flash:ImageError", function (s, t) {
                    n.trigger("Error", {
                        code: parseInt(t.code, 10), message: d.translate("Image error."), file: n.getFile(r[t.id])
                    })
                });
                n.bind("Flash:StageEvent:rollOver", function (s) {
                    var t, u;
                    t = b.getElementById(n.settings.browse_button);
                    u = s.settings.browse_button_hover;
                    if (t && u) {
                        d.addClass(t, u)
                    }

                });
                n.bind("Flash:StageEvent:rollOut", function (s) {
                    var t, u;
                    t = b.getElementById(n.settings.browse_button);
                    u = s.settings.browse_button_hover;
                    if (t && u) {
                        d.removeClass(t, u)
                    }

                });
                n.bind("Flash:StageEvent:mouseDown", function (s) {
                    var t, u;
                    t = b.getElementById(n.settings.browse_button);
                    u = s.settings.browse_button_active;
                    if (t && u) {
                        d.addClass(t, u);
                        d.addEvent(b.body, "mouseup", function () {
                            d.removeClass(t, u)
                        }
                        , s.id)
                    }

                });
                n.bind("Flash:StageEvent:mouseUp", function (s) {
                    var t, u;
                    t = b.getElementById(n.settings.browse_button);
                    u = s.settings.browse_button_active;
                    if (t && u) {
                        d.removeClass(t, u)
                    }

                });
                n.bind("Flash:ExifData", function (s, t) {
                    n.trigger("ExifData", n.getFile(r[t.id]), t.data)
                });
                n.bind("Flash:GpsData", function (s, t) {
                    n.trigger("GpsData", n.getFile(r[t.id]), t.data)
                });
                n.bind("QueueChanged", function (s) {
                    n.refresh()
                });
                n.bind("FilesRemoved", function (s, u) {
                    var t;
                    for (t = 0;
                    t < u.length;
                    t++) {
                        o().removeFile(r[u[t].id])
                    }

                });
                n.bind("StateChanged", function (s) {
                    n.refresh()
                });
                n.bind("Refresh", function (s) {
                    var t, u, v;
                    o().setFileFilters(n.settings.filters, n.settings.multi_selection);
                    t = b.getElementById(s.settings.browse_button);
                    if (t) {
                        u = d.getPos(t, b.getElementById(s.settings.container));
                        v = d.getSize(t);
                        d.extend(b.getElementById(s.id + "_flash_container").style, {
                            top: u.y + "px", left: u.x + "px", width: v.w + "px", height: v.h + "px"
                        })
                    }

                });
                n.bind("Destroy", function (s) {
                    var t;
                    d.removeAllEvents(b.body, s.id);
                    delete g[s.id];
                    delete a[s.id];
                    t = b.getElementById(s.id + "_flash_container");
                    if (t) {
                        j.removeChild(t)
                    }

                });
                p({
                    success: true
                })
            })
        }

    })
})(window, document, plupload);
(function (a) {
    a.runtimes.BrowserPlus = a.addRuntime("browserplus", {
        getFeatures: function () {
            return {
                dragdrop: true, jpgresize: true, pngresize: true, chunks: true, progress: true, multipart: true, multi_selection: true
            }

        }
        , init: function (g, j) {
            var e = window.BrowserPlus, h = {}, d = g.settings, c = d.resize;
            function f(o) {
                var n, m, k = [], l, p;
                for (m = 0;
                m < o.length;
                m++) {
                    l = o[m];
                    p = a.guid();
                    h[p] = l;
                    k.push(new a.File(p, l.name, l.size))
                }
                if (m) {
                    g.trigger("FilesAdded", k)
                }

            }
            function b() {
                g.bind("PostInit", function () {
                    var n, l = d.drop_element, p = g.id + "_droptarget", k = document.getElementById(l), m;
                    function q(s, r) {
                        e.DragAndDrop.AddDropTarget({
                            id: s
                        }
                        , function (t) {
                            e.DragAndDrop.AttachCallbacks({
                                id: s, hover: function (u) {
                                    if (!u && r) {
                                        r()
                                    }

                                }
                                , drop: function (u) {
                                    if (r) {
                                        r()
                                    }
                                    f(u)
                                }

                            }
                            , function () { })
                        })
                    }
                    function o() {
                        document.getElementById(p).style.top = "-1000px"
                    }
                    if (k) {
                        if (document.attachEvent && (/MSIE/gi).test(navigator.userAgent)) {
                            n = document.createElement("div");
                            n.setAttribute("id", p);
                            a.extend(n.style, {
                                position: "absolute", top: "-1000px", background: "red", filter: "alpha(opacity=0)", opacity: 0
                            });
                            document.body.appendChild(n);
                            a.addEvent(k, "dragenter", function (s) {
                                var r, t;
                                r = document.getElementById(l);
                                t = a.getPos(r);
                                a.extend(document.getElementById(p).style, {
                                    top: t.y + "px", left: t.x + "px", width: r.offsetWidth + "px", height: r.offsetHeight + "px"
                                })
                            });
                            q(p, o)
                        }
                        else {
                            q(l)
                        }

                    }
                    a.addEvent(document.getElementById(d.browse_button), "click", function (w) {
                        var u = [], s, r, v = d.filters, t;
                        w.preventDefault();
                        no_type_restriction: for (s = 0;
                        s < v.length;
                        s++) {
                            t = v[s].extensions.split(",");
                            for (r = 0;
                            r < t.length;
                            r++) {
                                if (t[r] === "*") {
                                    u = [];
                                    break no_type_restriction
                                }
                                u.push(a.mimeTypes[t[r]])
                            }

                        }
                        e.FileBrowse.OpenBrowseDialog({
                            mimeTypes: u
                        }
                        , function (x) {
                            if (x.success) {
                                f(x.value)
                            }

                        })
                    });
                    k = n = null
                });
                g.bind("UploadFile", function (n, k) {
                    var m = h[k.id], s = {}, l = n.settings.chunk_size, o, p = [];
                    function r(t, v) {
                        var u;
                        if (k.status == a.FAILED) {
                            return
                        }
                        s.name = k.target_name || k.name;
                        if (l) {
                            s.chunk = "" + t;
                            s.chunks = "" + v
                        }
                        u = p.shift();
                        e.Uploader.upload({
                            url: n.settings.url, files: {
                                file: u
                            }
                            , cookies: document.cookies, postvars: a.extend(s, n.settings.multipart_params), progressCallback: function (y) {
                                var x, w = 0;
                                o[t] = parseInt(y.filePercent * u.size / 100, 10);
                                for (x = 0;
                                x < o.length;
                                x++) {
                                    w += o[x]
                                }
                                k.loaded = w;
                                n.trigger("UploadProgress", k)
                            }

                        }
                        , function (x) {
                            var w, y;
                            if (x.success) {
                                w = x.value.statusCode;
                                if (l) {
                                    n.trigger("ChunkUploaded", k, {
                                        chunk: t, chunks: v, response: x.value.body, status: w
                                    })
                                }
                                if (p.length > 0) {
                                    r(++t, v)
                                }
                                else {
                                    k.status = a.DONE;
                                    n.trigger("FileUploaded", k, {
                                        response: x.value.body, status: w
                                    });
                                    if (w >= 400) {
                                        n.trigger("Error", {
                                            code: a.HTTP_ERROR, message: a.translate("HTTP Error."), file: k, status: w
                                        })
                                    }

                                }

                            }
                            else {
                                n.trigger("Error", {
                                    code: a.GENERIC_ERROR, message: a.translate("Generic Error."), file: k, details: x.error
                                })
                            }

                        })
                    }
                    function q(t) {
                        k.size = t.size;
                        if (l) {
                            e.FileAccess.chunk({
                                file: t, chunkSize: l
                            }
                            , function (w) {
                                if (w.success) {
                                    var x = w.value, u = x.length;
                                    o = Array(u);
                                    for (var v = 0;
                                    v < u;
                                    v++) {
                                        o[v] = 0;
                                        p.push(x[v])
                                    }
                                    r(0, u)
                                }

                            })
                        }
                        else {
                            o = Array(1);
                            p.push(t);
                            r(0, 1)
                        }

                    }
                    if (c && /\.(png|jpg|jpeg)$/i.test(k.name)) {
                        BrowserPlus.ImageAlter.transform({
                            file: m, quality: c.quality || 90, actions: [{
                                scale: {
                                    maxwidth: c.width, maxheight: c.height
                                }

                            }
                            ]
                        }
                        , function (t) {
                            if (t.success) {
                                q(t.value.file)
                            }

                        })
                    }
                    else {
                        q(m)
                    }

                });
                j({
                    success: true
                })
            }
            if (e) {
                e.init(function (l) {
                    var k = [{
                        service: "Uploader", version: "3"
                    }
                    , {
                        service: "DragAndDrop", version: "1"
                    }
                    , {
                        service: "FileBrowse", version: "1"
                    }
                    , {
                        service: "FileAccess", version: "2"
                    }
                    ];
                    if (c) {
                        k.push({
                            service: "ImageAlter", version: "4"
                        })
                    }
                    if (l.success) {
                        e.require({
                            services: k
                        }
                        , function (m) {
                            if (m.success) {
                                b()
                            }
                            else {
                                j()
                            }

                        })
                    }
                    else {
                        j()
                    }

                })
            }
            else {
                j()
            }

        }

    })
})(plupload);
(function (h, k, j, e) {
    var c = {}, g;
    function m(o, p) {
        var n;
        if ("FileReader" in h) {
            n = new FileReader();
            n.readAsDataURL(o);
            n.onload = function () {
                p(n.result)
            }

        }
        else {
            return p(o.getAsDataURL())
        }

    }
    function l(o, p) {
        var n;
        if ("FileReader" in h) {
            n = new FileReader();
            n.readAsBinaryString(o);
            n.onload = function () {
                p(n.result)
            }

        }
        else {
            return p(o.getAsBinary())
        }

    }
    function d(r, p, n, v) {
        var q, o, u, s, t = this;
        m(c[r.id], function (w) {
            q = k.createElement("canvas");
            q.style.display = "none";
            k.body.appendChild(q);
            o = q.getContext("2d");
            u = new Image();
            u.onerror = u.onabort = function () {
                v({
                    success: false
                })
            };
            u.onload = function () {
                var B, x, z, y, A;
                if (!p.width) {
                    p.width = u.width
                }
                if (!p.height) {
                    p.height = u.height
                }
                s = Math.min(p.width / u.width, p.height / u.height);
                if (s < 1 || (s === 1 && n === "image/jpeg")) {
                    B = Math.round(u.width * s);
                    x = Math.round(u.height * s);
                    q.width = B;
                    q.height = x;
                    o.drawImage(u, 0, 0, B, x);
                    if (n === "image/jpeg") {
                        y = new f(atob(w.substring(w.indexOf("base64,") + 7)));
                        if (y.headers && y.headers.length) {
                            A = new a();
                            if (A.init(y.get("exif")[0])) {
                                A.setExif("PixelXDimension", B);
                                A.setExif("PixelYDimension", x);
                                y.set("exif", A.getBinary());
                                if (t.hasEventListener("ExifData")) {
                                    t.trigger("ExifData", r, A.EXIF())
                                }
                                if (t.hasEventListener("GpsData")) {
                                    t.trigger("GpsData", r, A.GPS())
                                }

                            }

                        }
                        if (p.quality) {
                            try {
                                w = q.toDataURL(n, p.quality / 100)
                            }
                            catch (C) {
                                w = q.toDataURL(n)
                            }

                        }

                    }
                    else {
                        w = q.toDataURL(n)
                    }
                    w = w.substring(w.indexOf("base64,") + 7);
                    w = atob(w);
                    if (y && y.headers && y.headers.length) {
                        w = y.restore(w);
                        y.purge()
                    }
                    q.parentNode.removeChild(q);
                    v({
                        success: true, data: w
                    })
                }
                else {
                    v({
                        success: false
                    })
                }

            };
            u.src = w
        })
    }
    j.runtimes.Html5 = j.addRuntime("html5", {
        getFeatures: function () {
            var s, o, r, q, p, n;
            o = r = p = n = false;
            if (h.XMLHttpRequest) {
                s = new XMLHttpRequest();
                r = !!s.upload;
                o = !!(s.sendAsBinary || s.upload)
            }
            if (o) {
                q = !!(s.sendAsBinary || (h.Uint8Array && h.ArrayBuffer));
                p = !!(File && (File.prototype.getAsDataURL || h.FileReader) && q);
                n = !!(File && (File.prototype.mozSlice || File.prototype.webkitSlice || File.prototype.slice))
            }
            g = j.ua.safari && j.ua.windows;
            return {
                html5: o, dragdrop: (function () {
                    var t = k.createElement("div");
                    return ("draggable" in t) || ("ondragstart" in t && "ondrop" in t)
                }
                ()), jpgresize: p, pngresize: p, multipart: p || !!h.FileReader || !!h.FormData, canSendBinary: q, cantSendBlobInFormData: !!(j.ua.gecko && h.FormData && h.FileReader && !FileReader.prototype.readAsArrayBuffer), progress: r, chunks: n, multi_selection: !(j.ua.safari && j.ua.windows), triggerDialog: (j.ua.gecko && h.FormData || j.ua.webkit)
            }

        }
        , init: function (p, q) {
            var n;
            function o(v) {
                var t, s, u = [], w, r = {};
                for (s = 0;
                s < v.length;
                s++) {
                    t = v[s];
                    if (r[t.name]) {
                        continue
                    }
                    r[t.name] = true;
                    w = j.guid();
                    c[w] = t;
                    u.push(new j.File(w, t.fileName || t.name, t.fileSize || t.size))
                }
                if (u.length) {
                    p.trigger("FilesAdded", u)
                }

            }
            n = this.getFeatures();
            if (!n.html5) {
                q({
                    success: false
                });
                return
            }
            p.bind("Init", function (v) {
                var F, E, B = [], u, C, s = v.settings.filters, t, A, r = k.body, D;
                F = k.createElement("div");
                F.id = v.id + "_html5_container";
                j.extend(F.style, {
                    position: "absolute", background: p.settings.shim_bgcolor || "transparent", width: "100px", height: "100px", overflow: "hidden", zIndex: 99999, opacity: p.settings.shim_bgcolor ? "" : 0
                });
                F.className = "plupload html5";
                if (p.settings.container) {
                    r = k.getElementById(p.settings.container);
                    if (j.getStyle(r, "position") === "static") {
                        r.style.position = "relative"
                    }

                }
                r.appendChild(F);
                no_type_restriction: for (u = 0;
                u < s.length;
                u++) {
                    t = s[u].extensions.split(/,/);
                    for (C = 0;
                    C < t.length;
                    C++) {
                        if (t[C] === "*") {
                            B = [];
                            break no_type_restriction
                        }
                        A = j.mimeTypes[t[C]];
                        if (A) {
                            B.push(A)
                        }

                    }

                }
                F.innerHTML = '<input id="' + p.id + '_html5"  style="font-size:999px" type="file" accept="' + B.join(",") + '" ' + (p.settings.multi_selection && p.features.multi_selection ? 'multiple="multiple"' : "") + " />";
                F.scrollTop = 100;
                D = k.getElementById(p.id + "_html5");
                if (v.features.triggerDialog) {
                    j.extend(D.style, {
                        position: "absolute", width: "100%", height: "100%"
                    })
                }
                else {
                    j.extend(D.style, {
                        cssFloat: "right", styleFloat: "right"
                    })
                }
                D.onchange = function () {
                    o(this.files);
                    this.value = ""
                };
                E = k.getElementById(v.settings.browse_button);
                if (E) {
                    var x = v.settings.browse_button_hover, z = v.settings.browse_button_active, w = v.features.triggerDialog ? E : F;
                    if (x) {
                        j.addEvent(w, "mouseover", function () {
                            j.addClass(E, x)
                        }
                        , v.id);
                        j.addEvent(w, "mouseout", function () {
                            j.removeClass(E, x)
                        }
                        , v.id)
                    }
                    if (z) {
                        j.addEvent(w, "mousedown", function () {
                            j.addClass(E, z)
                        }
                        , v.id);
                        j.addEvent(k.body, "mouseup", function () {
                            j.removeClass(E, z)
                        }
                        , v.id)
                    }
                    if (v.features.triggerDialog) {
                        j.addEvent(E, "click", function (y) {
                            k.getElementById(v.id + "_html5").click();
                            y.preventDefault()
                        }
                        , v.id)
                    }

                }

            });
            p.bind("PostInit", function () {
                var r = k.getElementById(p.settings.drop_element);
                if (r) {
                    if (g) {
                        j.addEvent(r, "dragenter", function (v) {
                            var u, s, t;
                            u = k.getElementById(p.id + "_drop");
                            if (!u) {
                                u = k.createElement("input");
                                u.setAttribute("type", "file");
                                u.setAttribute("id", p.id + "_drop");
                                u.setAttribute("multiple", "multiple");
                                j.addEvent(u, "change", function () {
                                    o(this.files);
                                    j.removeEvent(u, "change", p.id);
                                    u.parentNode.removeChild(u)
                                }
                                , p.id);
                                r.appendChild(u)
                            }
                            s = j.getPos(r, k.getElementById(p.settings.container));
                            t = j.getSize(r);
                            if (j.getStyle(r, "position") === "static") {
                                j.extend(r.style, {
                                    position: "relative"
                                })
                            }
                            j.extend(u.style, {
                                position: "absolute", display: "block", top: 0, left: 0, width: t.w + "px", height: t.h + "px", opacity: 0
                            })
                        }
                        , p.id);
                        return
                    }
                    j.addEvent(r, "dragover", function (s) {
                        s.preventDefault()
                    }
                    , p.id);
                    j.addEvent(r, "drop", function (t) {
                        var s = t.dataTransfer;
                        if (s && s.files) {
                            o(s.files)
                        }
                        t.preventDefault()
                    }
                    , p.id)
                }

            });
            p.bind("Refresh", function (r) {
                var s, t, u, w, v;
                s = k.getElementById(p.settings.browse_button);
                if (s) {
                    t = j.getPos(s, k.getElementById(r.settings.container));
                    u = j.getSize(s);
                    w = k.getElementById(p.id + "_html5_container");
                    j.extend(w.style, {
                        top: t.y + "px", left: t.x + "px", width: u.w + "px", height: u.h + "px"
                    });
                    if (p.features.triggerDialog) {
                        if (j.getStyle(s, "position") === "static") {
                            j.extend(s.style, {
                                position: "relative"
                            })
                        }
                        v = parseInt(j.getStyle(s, "z-index"), 10);
                        if (isNaN(v)) {
                            v = 0
                        }
                        j.extend(s.style, {
                            zIndex: v
                        });
                        j.extend(w.style, {
                            zIndex: v - 1
                        })
                    }

                }

            });
            p.bind("UploadFile", function (r, t) {
                var u = r.settings, x, s;
                function w(z, C, y) {
                    var A;
                    if (File.prototype.slice) {
                        try {
                            z.slice();
                            return z.slice(C, y)
                        }
                        catch (B) {
                            return z.slice(C, y - C)
                        }

                    }
                    else {
                        if (A = File.prototype.webkitSlice || File.prototype.mozSlice) {
                            return A.call(z, C, y)
                        }
                        else {
                            return null
                        }

                    }

                }
                function v(z) {
                    var C = 0, B = 0, y = ("FileReader" in h) ? new FileReader : null;
                    function A() {
                        var H, L, J, K, G, I, E, D = r.settings.url;
                        function F(V) {
                            var S = 0, T = new XMLHttpRequest, W = T.upload, M = "----pluploadboundary" + j.guid(), N, O = "--", U = "\r\n", Q = "";
                            if (W) {
                                W.onprogress = function (X) {
                                    t.loaded = Math.min(t.size, B + X.loaded - S);
                                    r.trigger("UploadProgress", t)
                                }

                            }
                            T.onreadystatechange = function () {
                                var X, Z;
                                if (T.readyState == 4) {
                                    try {
                                        X = T.status
                                    }
                                    catch (Y) {
                                        X = 0
                                    }
                                    if (X >= 400) {
                                        r.trigger("Error", {
                                            code: j.HTTP_ERROR, message: j.translate("HTTP Error."), file: t, status: X
                                        })
                                    }
                                    else {
                                        if (J) {
                                            Z = {
                                                chunk: C, chunks: J, response: T.responseText, status: X
                                            };
                                            r.trigger("ChunkUploaded", t, Z);
                                            B += I;
                                            if (Z.cancelled) {
                                                t.status = j.FAILED;
                                                return
                                            }
                                            t.loaded = Math.min(t.size, (C + 1) * G)
                                        }
                                        else {
                                            t.loaded = t.size
                                        }
                                        r.trigger("UploadProgress", t);
                                        V = H = N = Q = null;
                                        if (!J || ++C >= J) {
                                            t.status = j.DONE;
                                            r.trigger("FileUploaded", t, {
                                                response: T.responseText, status: X
                                            })
                                        }
                                        else {
                                            A()
                                        }

                                    }
                                    T = null
                                }

                            };
                            if (r.settings.multipart && n.multipart) {
                                K.name = t.target_name || t.name;
                                
                                T.open("post", D, true);
                                j.each(r.settings.headers, function (Y, X) {
                                    T.setRequestHeader(X, Y)
                                });
                                if (typeof (V) !== "string" && !!h.FormData) {
                                    N = new FormData();
                                    j.each(j.extend(K, r.settings.multipart_params), function (Y, X) {
                                        N.append(X, Y)
                                    });
                                    N.append(r.settings.file_data_name, V);                                    
                                    T.send(N)
                                    return
                                }
                                if (typeof (V) === "string") {
                                    T.setRequestHeader("Content-Type", "multipart/form-data; boundary=" + M);
                                    j.each(j.extend(K, r.settings.multipart_params), function (Y, X) {
                                        Q += O + M + U + 'Content-Disposition: form-data; name="' + X + '"' + U + U;
                                        Q += unescape(encodeURIComponent(Y)) + U
                                    });
                                    E = j.mimeTypes[t.name.replace(/^.+\.([^.]+)/, "$1").toLowerCase()] || "application/octet-stream";
                                    Q += O + M + U + 'Content-Disposition: form-data; name="' + r.settings.file_data_name + '"; filename="' + unescape(encodeURIComponent(t.name)) + '"' + U + "Content-Type: " + E + U + U + V + U + O + M + O + U;
                                    S = Q.length - V.length;
                                    V = Q;
                                    if (T.sendAsBinary) {                                        
                                       T.sendAsBinary(V)
                                    }
                                    else {
                                        if (n.canSendBinary) {
                                            var R = new Uint8Array(V.length);
                                            for (var P = 0;
                                            P < V.length;
                                            P++) {
                                                R[P] = (V.charCodeAt(P) & 255)
                                            }                                            
                                            T.send(R.buffer)
                                        }

                                    }
                                    return
                                }

                            }
                            D = j.buildUrl(r.settings.url, j.extend(K, r.settings.multipart_params));
                            T.open("post", D, true);
                            T.setRequestHeader("Content-Type", "application/octet-stream");
                            j.each(r.settings.headers, function (Y, X) {
                                T.setRequestHeader(X, Y)
                            });
                            
                            T.send(V)
                        }
                        if (t.status == j.DONE || t.status == j.FAILED || r.state == j.STOPPED) {
                            return
                        }
                        K = {
                            name: t.target_name || t.name
                        };
                        if (u.chunk_size && t.size > u.chunk_size && (n.chunks || typeof (z) == "string")) {
                            G = u.chunk_size;
                            J = Math.ceil(t.size / G);
                            I = Math.min(G, t.size - (C * G));
                            if (typeof (z) == "string") {
                                H = z.substring(C * G, C * G + I)
                            }
                            else {
                                H = w(z, C * G, C * G + I)
                            }
                            K.chunk = C;
                            K.chunks = J
                        }
                        else {
                            I = t.size;
                            H = z
                        }
                        if (typeof (H) !== "string" && y && n.cantSendBlobInFormData && n.chunks && r.settings.chunk_size) {
                            y.onload = function () {
                                F(y.result)
                            };
                            y.readAsBinaryString(H)
                        }
                        else {
                            F(H)
                        }

                    }
                    A()
                }
                x = c[t.id];
                if (n.jpgresize && r.settings.resize && /\.(png|jpg|jpeg)$/i.test(t.name)) {
                    d.call(r, t, r.settings.resize, /\.png$/i.test(t.name) ? "image/png" : "image/jpeg", function (y) {
                        if (y.success) {
                            t.size = y.data.length;
                            v(y.data)
                        }
                        else {
                            v(x)
                        }

                    })
                }
                else {
                    if (!n.chunks && n.jpgresize) {
                        l(x, v)
                    }
                    else {
                        v(x)
                    }

                }

            });
            p.bind("Destroy", function (r) {
                var t, u, s = k.body, v = {
                    inputContainer: r.id + "_html5_container", inputFile: r.id + "_html5", browseButton: r.settings.browse_button, dropElm: r.settings.drop_element
                };
                for (t in v) {
                    u = k.getElementById(v[t]);
                    if (u) {
                        j.removeAllEvents(u, r.id)
                    }

                }
                j.removeAllEvents(k.body, r.id);
                if (r.settings.container) {
                    s = k.getElementById(r.settings.container)
                }
                s.removeChild(k.getElementById(v.inputContainer))
            });
            q({
                success: true
            })
        }

    });
    function b() {
        var q = false, o;
        function r(t, v) {
            var s = q ? 0 : -8 * (v - 1), w = 0, u;
            for (u = 0;
            u < v;
            u++) {
                w |= (o.charCodeAt(t + u) << Math.abs(s + u * 8))
            }
            return w
        }
        function n(u, s, t) {
            var t = arguments.length === 3 ? t : o.length - s - 1;
            o = o.substr(0, s) + u + o.substr(t + s)
        }
        function p(t, u, w) {
            var x = "", s = q ? 0 : -8 * (w - 1), v;
            for (v = 0;
            v < w;
            v++) {
                x += String.fromCharCode((u >> Math.abs(s + v * 8)) & 255)
            }
            n(x, t, w)
        }
        return {
            II: function (s) {
                if (s === e) {
                    return q
                }
                else {
                    q = s
                }

            }
            , init: function (s) {
                q = false;
                o = s
            }
            , SEGMENT: function (s, u, t) {
                switch (arguments.length) {
                    case 1: return o.substr(s, o.length - s - 1);
                    case 2: return o.substr(s, u);
                    case 3: n(t, s, u);
                        break;
                    default: return o
                }

            }
            , BYTE: function (s) {
                return r(s, 1)
            }
            , SHORT: function (s) {
                return r(s, 2)
            }
            , LONG: function (s, t) {
                if (t === e) {
                    return r(s, 4)
                }
                else {
                    p(s, t, 4)
                }

            }
            , SLONG: function (s) {
                var t = r(s, 4);
                return (t > 2147483647 ? t - 4294967296 : t)
            }
            , STRING: function (s, t) {
                var u = "";
                for (t += s;
                s < t;
                s++) {
                    u += String.fromCharCode(r(s, 1))
                }
                return u
            }

        }

    }
    function f(s) {
        var u = {
            65505: {
                app: "EXIF", name: "APP1", signature: "Exif\0"
            }
            , 65506: {
                app: "ICC", name: "APP2", signature: "ICC_PROFILE\0"
            }
            , 65517: {
                app: "IPTC", name: "APP13", signature: "Photoshop 3.0\0"
            }

        }
        , t = [], r, n, p = e, q = 0, o;
        r = new b();
        r.init(s);
        if (r.SHORT(0) !== 65496) {
            return
        }
        n = 2;
        o = Math.min(1048576, s.length);
        while (n <= o) {
            p = r.SHORT(n);
            if (p >= 65488 && p <= 65495) {
                n += 2;
                continue
            }
            if (p === 65498 || p === 65497) {
                break
            }
            q = r.SHORT(n + 2) + 2;
            if (u[p] && r.STRING(n + 4, u[p].signature.length) === u[p].signature) {
                t.push({
                    hex: p, app: u[p].app.toUpperCase(), name: u[p].name.toUpperCase(), start: n, length: q, segment: r.SEGMENT(n, q)
                })
            }
            n += q
        }
        r.init(null);
        return {
            headers: t, restore: function (y) {
                r.init(y);
                var w = new f(y);
                if (!w.headers) {
                    return false
                }
                for (var x = w.headers.length;
                x > 0;
                x--) {
                    var z = w.headers[x - 1];
                    r.SEGMENT(z.start, z.length, "")
                }
                w.purge();
                n = r.SHORT(2) == 65504 ? 4 + r.SHORT(4) : 2;
                for (var x = 0, v = t.length;
                x < v;
                x++) {
                    r.SEGMENT(n, 0, t[x].segment);
                    n += t[x].length
                }
                return r.SEGMENT()
            }
            , get: function (x) {
                var y = [];
                for (var w = 0, v = t.length;
                w < v;
                w++) {
                    if (t[w].app === x.toUpperCase()) {
                        y.push(t[w].segment)
                    }

                }
                return y
            }
            , set: function (y, x) {
                var z = [];
                if (typeof (x) === "string") {
                    z.push(x)
                }
                else {
                    z = x
                }
                for (var w = ii = 0, v = t.length;
                w < v;
                w++) {
                    if (t[w].app === y.toUpperCase()) {
                        t[w].segment = z[ii];
                        t[w].length = z[ii].length;
                        ii++
                    }
                    if (ii >= z.length) {
                        break
                    }

                }

            }
            , purge: function () {
                t = [];
                r.init(null)
            }

        }

    }
    function a() {
        var q, n, o = {}, t;
        q = new b();
        n = {
            tiff: {
                274: "Orientation", 34665: "ExifIFDPointer", 34853: "GPSInfoIFDPointer"
            }
            , exif: {
                36864: "ExifVersion", 40961: "ColorSpace", 40962: "PixelXDimension", 40963: "PixelYDimension", 36867: "DateTimeOriginal", 33434: "ExposureTime", 33437: "FNumber", 34855: "ISOSpeedRatings", 37377: "ShutterSpeedValue", 37378: "ApertureValue", 37383: "MeteringMode", 37384: "LightSource", 37385: "Flash", 41986: "ExposureMode", 41987: "WhiteBalance", 41990: "SceneCaptureType", 41988: "DigitalZoomRatio", 41992: "Contrast", 41993: "Saturation", 41994: "Sharpness"
            }
            , gps: {
                0: "GPSVersionID", 1: "GPSLatitudeRef", 2: "GPSLatitude", 3: "GPSLongitudeRef", 4: "GPSLongitude"
            }

        };
        t = {
            ColorSpace: {
                1: "sRGB", 0: "Uncalibrated"
            }
            , MeteringMode: {
                0: "Unknown", 1: "Average", 2: "CenterWeightedAverage", 3: "Spot", 4: "MultiSpot", 5: "Pattern", 6: "Partial", 255: "Other"
            }
            , LightSource: {
                1: "Daylight", 2: "Fliorescent", 3: "Tungsten", 4: "Flash", 9: "Fine weather", 10: "Cloudy weather", 11: "Shade", 12: "Daylight fluorescent (D 5700 - 7100K)", 13: "Day white fluorescent (N 4600 -5400K)", 14: "Cool white fluorescent (W 3900 - 4500K)", 15: "White fluorescent (WW 3200 - 3700K)", 17: "Standard light A", 18: "Standard light B", 19: "Standard light C", 20: "D55", 21: "D65", 22: "D75", 23: "D50", 24: "ISO studio tungsten", 255: "Other"
            }
            , Flash: {
                0: "Flash did not fire.", 1: "Flash fired.", 5: "Strobe return light not detected.", 7: "Strobe return light detected.", 9: "Flash fired, compulsory flash mode", 13: "Flash fired, compulsory flash mode, return light not detected", 15: "Flash fired, compulsory flash mode, return light detected", 16: "Flash did not fire, compulsory flash mode", 24: "Flash did not fire, auto mode", 25: "Flash fired, auto mode", 29: "Flash fired, auto mode, return light not detected", 31: "Flash fired, auto mode, return light detected", 32: "No flash function", 65: "Flash fired, red-eye reduction mode", 69: "Flash fired, red-eye reduction mode, return light not detected", 71: "Flash fired, red-eye reduction mode, return light detected", 73: "Flash fired, compulsory flash mode, red-eye reduction mode", 77: "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected", 79: "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected", 89: "Flash fired, auto mode, red-eye reduction mode", 93: "Flash fired, auto mode, return light not detected, red-eye reduction mode", 95: "Flash fired, auto mode, return light detected, red-eye reduction mode"
            }
            , ExposureMode: {
                0: "Auto exposure", 1: "Manual exposure", 2: "Auto bracket"
            }
            , WhiteBalance: {
                0: "Auto white balance", 1: "Manual white balance"
            }
            , SceneCaptureType: {
                0: "Standard", 1: "Landscape", 2: "Portrait", 3: "Night scene"
            }
            , Contrast: {
                0: "Normal", 1: "Soft", 2: "Hard"
            }
            , Saturation: {
                0: "Normal", 1: "Low saturation", 2: "High saturation"
            }
            , Sharpness: {
                0: "Normal", 1: "Soft", 2: "Hard"
            }
            , GPSLatitudeRef: {
                N: "North latitude", S: "South latitude"
            }
            , GPSLongitudeRef: {
                E: "East longitude", W: "West longitude"
            }

        };
        function p(u, C) {
            var w = q.SHORT(u), z, F, G, B, A, v, x, D, E = [], y = {};
            for (z = 0;
            z < w;
            z++) {
                x = v = u + 12 * z + 2;
                G = C[q.SHORT(x)];
                if (G === e) {
                    continue
                }
                B = q.SHORT(x += 2);
                A = q.LONG(x += 2);
                x += 4;
                E = [];
                switch (B) {
                    case 1: case 7: if (A > 4) {
                            x = q.LONG(x) + o.tiffHeader
                        }
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.BYTE(x + F)
                        }
                        break;
                    case 2: if (A > 4) {
                            x = q.LONG(x) + o.tiffHeader
                        }
                        y[G] = q.STRING(x, A - 1);
                        continue;
                    case 3: if (A > 2) {
                            x = q.LONG(x) + o.tiffHeader
                        }
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.SHORT(x + F * 2)
                        }
                        break;
                    case 4: if (A > 1) {
                            x = q.LONG(x) + o.tiffHeader
                        }
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.LONG(x + F * 4)
                        }
                        break;
                    case 5: x = q.LONG(x) + o.tiffHeader;
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.LONG(x + F * 4) / q.LONG(x + F * 4 + 4)
                        }
                        break;
                    case 9: x = q.LONG(x) + o.tiffHeader;
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.SLONG(x + F * 4)
                        }
                        break;
                    case 10: x = q.LONG(x) + o.tiffHeader;
                        for (F = 0;
                    F < A;
                    F++) {
                            E[F] = q.SLONG(x + F * 4) / q.SLONG(x + F * 4 + 4)
                        }
                        break;
                    default: continue
                }
                D = (A == 1 ? E[0] : E);
                if (t.hasOwnProperty(G) && typeof D != "object") {
                    y[G] = t[G][D]
                }
                else {
                    y[G] = D
                }

            }
            return y
        }
        function s() {
            var v = e, u = o.tiffHeader;
            q.II(q.SHORT(u) == 18761);
            if (q.SHORT(u += 2) !== 42) {
                return false
            }
            o.IFD0 = o.tiffHeader + q.LONG(u += 2);
            v = p(o.IFD0, n.tiff);
            o.exifIFD = ("ExifIFDPointer" in v ? o.tiffHeader + v.ExifIFDPointer : e);
            o.gpsIFD = ("GPSInfoIFDPointer" in v ? o.tiffHeader + v.GPSInfoIFDPointer : e);
            return true
        }
        function r(w, u, z) {
            var B, y, x, A = 0;
            if (typeof (u) === "string") {
                var v = n[w.toLowerCase()];
                for (hex in v) {
                    if (v[hex] === u) {
                        u = hex;
                        break
                    }

                }

            }
            B = o[w.toLowerCase() + "IFD"];
            y = q.SHORT(B);
            for (i = 0;
            i < y;
            i++) {
                x = B + 12 * i + 2;
                if (q.SHORT(x) == u) {
                    A = x + 8;
                    break
                }

            }
            if (!A) {
                return false
            }
            q.LONG(A, z);
            return true
        }
        return {
            init: function (u) {
                o = {
                    tiffHeader: 10
                };
                if (u === e || !u.length) {
                    return false
                }
                q.init(u);
                if (q.SHORT(0) === 65505 && q.STRING(4, 5).toUpperCase() === "EXIF\0") {
                    return s()
                }
                return false
            }
            , EXIF: function () {
                var u;
                u = p(o.exifIFD, n.exif);
                if (u.ExifVersion) {
                    u.ExifVersion = String.fromCharCode(u.ExifVersion[0], u.ExifVersion[1], u.ExifVersion[2], u.ExifVersion[3])
                }
                return u
            }
            , GPS: function () {
                var u;
                u = p(o.gpsIFD, n.gps);
                if (u.GPSVersionID) {
                    u.GPSVersionID = u.GPSVersionID.join(".")
                }
                return u
            }
            , setExif: function (u, v) {
                if (u !== "PixelXDimension" && u !== "PixelYDimension") {
                    return false
                }
                return r("exif", u, v)
            }
            , getBinary: function () {
                return q.SEGMENT()
            }

        }

    }

})(window, document, plupload);
(function (d, a, b, c) {
    function e(f) {
        return a.getElementById(f)
    }
    b.runtimes.Html4 = b.addRuntime("html4", {
        getFeatures: function () {
            return {
                multipart: true, triggerDialog: (b.ua.gecko && d.FormData || b.ua.webkit)
            }

        }
        , init: function (f, g) {
            f.bind("Init", function (p) {
                var j = a.body, n, h = "javascript", k, x, q, z = [], r = /MSIE/.test(navigator.userAgent), t = [], m = p.settings.filters, o, l, s, w;
                no_type_restriction: for (o = 0;
                o < m.length;
                o++) {
                    l = m[o].extensions.split(/,/);
                    for (w = 0;
                    w < l.length;
                    w++) {
                        if (l[w] === "*") {
                            t = [];
                            break no_type_restriction
                        }
                        s = b.mimeTypes[l[w]];
                        if (s) {
                            t.push(s)
                        }

                    }

                }
                t = t.join(",");
                function v() {
                    var C, A, y, B;
                    q = b.guid();
                    z.push(q);
                    C = a.createElement("form");
                    C.setAttribute("id", "form_" + q);
                    C.setAttribute("method", "post");
                    C.setAttribute("enctype", "multipart/form-data");
                    C.setAttribute("encoding", "multipart/form-data");
                    C.setAttribute("target", p.id + "_iframe");
                    C.style.position = "absolute";
                    A = a.createElement("input");
                    A.setAttribute("id", "input_" + q);
                    A.setAttribute("type", "file");
                    A.setAttribute("accept", t);
                    A.setAttribute("size", 1);
                    B = e(p.settings.browse_button);
                    if (p.features.triggerDialog && B) {
                        b.addEvent(e(p.settings.browse_button), "click", function (D) {
                            A.click();
                            D.preventDefault()
                        }
                        , p.id)
                    }
                    b.extend(A.style, {
                        width: "100%", height: "100%", opacity: 0, fontSize: "999px"
                    });
                    b.extend(C.style, {
                        overflow: "hidden"
                    });
                    y = p.settings.shim_bgcolor;
                    if (y) {
                        C.style.background = y
                    }
                    if (r) {
                        b.extend(A.style, {
                            filter: "alpha(opacity=0)"
                        })
                    }
                    b.addEvent(A, "change", function (G) {
                        var E = G.target, D, F = [], H;
                        if (E.value) {
                            e("form_" + q).style.top = -1048575 + "px";
                            D = E.value.replace(/\\/g, "/");
                            D = D.substring(D.length, D.lastIndexOf("/") + 1);
                            F.push(new b.File(q, D));
                            if (!p.features.triggerDialog) {
                                b.removeAllEvents(C, p.id)
                            }
                            else {
                                b.removeEvent(B, "click", p.id)
                            }
                            b.removeEvent(A, "change", p.id);
                            v();
                            if (F.length) {
                                f.trigger("FilesAdded", F)
                            }

                        }

                    }
                    , p.id);
                    C.appendChild(A);
                    j.appendChild(C);
                    p.refresh()
                }
                function u() {
                    var y = a.createElement("div");
                    y.innerHTML = '<iframe id="' + p.id + '_iframe" name="' + p.id + '_iframe" src="' + h + ':&quot;&quot;" style="display:none"></iframe>';
                    n = y.firstChild;
                    j.appendChild(n);
                    b.addEvent(n, "load", function (D) {
                        var E = D.target, C, A;
                        if (!k) {
                            return
                        }
                        try {
                            C = E.contentWindow.document || E.contentDocument || d.frames[E.id].document
                        }
                        catch (B) {
                            p.trigger("Error", {
                                code: b.SECURITY_ERROR, message: b.translate("Security error."), file: k
                            });
                            return
                        }
                        A = C.body.innerHTML;
                        if (A) {
                            k.status = b.DONE;
                            k.loaded = 1025;
                            k.percent = 100;
                            p.trigger("UploadProgress", k);
                            p.trigger("FileUploaded", k, {
                                response: A
                            })
                        }

                    }
                    , p.id)
                }
                if (p.settings.container) {
                    j = e(p.settings.container);
                    if (b.getStyle(j, "position") === "static") {
                        j.style.position = "relative"
                    }

                }
                p.bind("UploadFile", function (y, B) {
                    var C, A;
                    if (B.status == b.DONE || B.status == b.FAILED || y.state == b.STOPPED) {
                        return
                    }
                    C = e("form_" + B.id);
                    A = e("input_" + B.id);
                    A.setAttribute("name", y.settings.file_data_name);
                    C.setAttribute("action", y.settings.url);
                    b.each(b.extend({
                        name: B.target_name || B.name
                    }
                    , y.settings.multipart_params), function (F, D) {
                        var E = a.createElement("input");
                        b.extend(E, {
                            type: "hidden", name: D, value: F
                        });
                        C.insertBefore(E, C.firstChild)
                    });
                    k = B;
                    e("form_" + q).style.top = -1048575 + "px";
                    C.submit();
                    C.parentNode.removeChild(C)
                });
                p.bind("FileUploaded", function (y) {
                    y.refresh()
                });
                p.bind("StateChanged", function (y) {
                    if (y.state == b.STARTED) {
                        u()
                    }
                    if (y.state == b.STOPPED) {
                        d.setTimeout(function () {
                            b.removeEvent(n, "load", y.id);
                            if (n.parentNode) {
                                n.parentNode.removeChild(n)
                            }

                        }
                        , 0)
                    }

                });
                p.bind("Refresh", function (A) {
                    var G, B, C, D, y, H, I, F, E;
                    G = e(A.settings.browse_button);
                    if (G) {
                        y = b.getPos(G, e(A.settings.container));
                        H = b.getSize(G);
                        I = e("form_" + q);
                        F = e("input_" + q);
                        b.extend(I.style, {
                            top: y.y + "px", left: y.x + "px", width: H.w + "px", height: H.h + "px"
                        });
                        if (A.features.triggerDialog) {
                            if (b.getStyle(G, "position") === "static") {
                                b.extend(G.style, {
                                    position: "relative"
                                })
                            }
                            E = parseInt(G.style.zIndex, 10);
                            if (isNaN(E)) {
                                E = 0
                            }
                            b.extend(G.style, {
                                zIndex: E
                            });
                            b.extend(I.style, {
                                zIndex: E - 1
                            })
                        }
                        C = A.settings.browse_button_hover;
                        D = A.settings.browse_button_active;
                        B = A.features.triggerDialog ? G : I;
                        if (C) {
                            b.addEvent(B, "mouseover", function () {
                                b.addClass(G, C)
                            }
                            , A.id);
                            b.addEvent(B, "mouseout", function () {
                                b.removeClass(G, C)
                            }
                            , A.id)
                        }
                        if (D) {
                            b.addEvent(B, "mousedown", function () {
                                b.addClass(G, D)
                            }
                            , A.id);
                            b.addEvent(a.body, "mouseup", function () {
                                b.removeClass(G, D)
                            }
                            , A.id)
                        }

                    }

                });
                f.bind("FilesRemoved", function (y, B) {
                    var A, C;
                    for (A = 0;
                    A < B.length;
                    A++) {
                        C = e("form_" + B[A].id);
                        if (C) {
                            C.parentNode.removeChild(C)
                        }

                    }

                });
                f.bind("Destroy", function (y) {
                    var A, B, C, D = {
                        inputContainer: "form_" + q, inputFile: "input_" + q, browseButton: y.settings.browse_button
                    };
                    for (A in D) {
                        B = e(D[A]);
                        if (B) {
                            b.removeAllEvents(B, y.id)
                        }

                    }
                    b.removeAllEvents(a.body, y.id);
                    b.each(z, function (F, E) {
                        C = e("form_" + F);
                        if (C) {
                            j.removeChild(C)
                        }

                    })
                });
                v()
            });
            g({
                success: true
            })
        }

    })
})(window, document, plupload);
