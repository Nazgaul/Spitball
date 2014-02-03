﻿/* Modernizr 2.0.6 (Custom Build) | MIT & BSD
* Build: http://www.modernizr.com/download/#-history-audio-iepp-cssclasses-load
*/
; window.Modernizr = function (a, b, c) { function x(a, b) { return !! ~("" + a).indexOf(b) } function w(a, b) { return typeof a === b } function v(a, b) { return u(prefixes.join(a + ";") + (b || "")) } function u(a) { k.cssText = a } var d = "2.0.6", e = {}, f = !0, g = b.documentElement, h = b.head || b.getElementsByTagName("head")[0], i = "modernizr", j = b.createElement(i), k = j.style, l, m = Object.prototype.toString, n = {}, o = {}, p = {}, q = [], r, s = {}.hasOwnProperty, t; !w(s, c) && !w(s.call, c) ? t = function (a, b) { return s.call(a, b) } : t = function (a, b) { return b in a && w(a.constructor.prototype[b], c) }, n.history = function () { return !!a.history && !!history.pushState }, n.audio = function () { var a = b.createElement("audio"), c = !1; try { if (c = !!a.canPlayType) c = new Boolean(c), c.ogg = a.canPlayType('audio/ogg; codecs="vorbis"'), c.mp3 = a.canPlayType("audio/mpeg;"), c.wav = a.canPlayType('audio/wav; codecs="1"'), c.m4a = a.canPlayType("audio/x-m4a;") || a.canPlayType("audio/aac;") } catch (d) { } return c }; for (var y in n) t(n, y) && (r = y.toLowerCase(), e[r] = n[y](), q.push((e[r] ? "" : "no-") + r)); u(""), j = l = null, a.attachEvent && function () { var a = b.createElement("div"); a.innerHTML = "<elem></elem>"; return a.childNodes.length !== 1 } () && function (a, b) { function s(a) { var b = -1; while (++b < g) a.createElement(f[b]) } a.iepp = a.iepp || {}; var d = a.iepp, e = d.html5elements || "abbr|article|aside|audio|canvas|datalist|details|figcaption|figure|footer|header|hgroup|mark|meter|nav|output|progress|section|summary|time|video", f = e.split("|"), g = f.length, h = new RegExp("(^|\\s)(" + e + ")", "gi"), i = new RegExp("<(/*)(" + e + ")", "gi"), j = /^\s*[\{\}]\s*$/, k = new RegExp("(^|[^\\n]*?\\s)(" + e + ")([^\\n]*)({[\\n\\w\\W]*?})", "gi"), l = b.createDocumentFragment(), m = b.documentElement, n = m.firstChild, o = b.createElement("body"), p = b.createElement("style"), q = /print|all/, r; d.getCSS = function (a, b) { if (a + "" === c) return ""; var e = -1, f = a.length, g, h = []; while (++e < f) { g = a[e]; if (g.disabled) continue; b = g.media || b, q.test(b) && h.push(d.getCSS(g.imports, b), g.cssText), b = "all" } return h.join("") }, d.parseCSS = function (a) { var b = [], c; while ((c = k.exec(a)) != null) b.push(((j.exec(c[1]) ? "\n" : c[1]) + c[2] + c[3]).replace(h, "$1.iepp_$2") + c[4]); return b.join("\n") }, d.writeHTML = function () { var a = -1; r = r || b.body; while (++a < g) { var c = b.getElementsByTagName(f[a]), d = c.length, e = -1; while (++e < d) c[e].className.indexOf("iepp_") < 0 && (c[e].className += " iepp_" + f[a]) } l.appendChild(r), m.appendChild(o), o.className = r.className, o.id = r.id, o.innerHTML = r.innerHTML.replace(i, "<$1font") }, d._beforePrint = function () { p.styleSheet.cssText = d.parseCSS(d.getCSS(b.styleSheets, "all")), d.writeHTML() }, d.restoreHTML = function () { o.innerHTML = "", m.removeChild(o), m.appendChild(r) }, d._afterPrint = function () { d.restoreHTML(), p.styleSheet.cssText = "" }, s(b), s(l); d.disablePP || (n.insertBefore(p, n.firstChild), p.media = "print", p.className = "iepp-printshim", a.attachEvent("onbeforeprint", d._beforePrint), a.attachEvent("onafterprint", d._afterPrint)) } (a, b), e._version = d, g.className = g.className.replace(/\bno-js\b/, "") + (f ? " js " + q.join(" ") : ""); return e } (this, this.document), function (a, b, c) { function k(a) { return !a || a == "loaded" || a == "complete" } function j() { var a = 1, b = -1; while (p.length - ++b) if (p[b].s && !(a = p[b].r)) break; a && g() } function i(a) { var c = b.createElement("script"), d; c.src = a.s, c.onreadystatechange = c.onload = function () { !d && k(c.readyState) && (d = 1, j(), c.onload = c.onreadystatechange = null) }, m(function () { d || (d = 1, j()) }, H.errorTimeout), a.e ? c.onload() : n.parentNode.insertBefore(c, n) } function h(a) { var c = b.createElement("link"), d; c.href = a.s, c.rel = "stylesheet", c.type = "text/css"; if (!a.e && (w || r)) { var e = function (a) { m(function () { if (!d) try { a.sheet.cssRules.length ? (d = 1, j()) : e(a) } catch (b) { b.code == 1e3 || b.message == "security" || b.message == "denied" ? (d = 1, m(function () { j() }, 0)) : e(a) } }, 0) }; e(c) } else c.onload = function () { d || (d = 1, m(function () { j() }, 0)) }, a.e && c.onload(); m(function () { d || (d = 1, j()) }, H.errorTimeout), !a.e && n.parentNode.insertBefore(c, n) } function g() { var a = p.shift(); q = 1, a ? a.t ? m(function () { a.t == "c" ? h(a) : i(a) }, 0) : (a(), j()) : q = 0 } function f(a, c, d, e, f, h) { function i() { !o && k(l.readyState) && (r.r = o = 1, !q && j(), l.onload = l.onreadystatechange = null, m(function () { u.removeChild(l) }, 0)) } var l = b.createElement(a), o = 0, r = { t: d, s: c, e: h }; l.src = l.data = c, !s && (l.style.display = "none"), l.width = l.height = "0", a != "object" && (l.type = d), l.onload = l.onreadystatechange = i, a == "img" ? l.onerror = i : a == "script" && (l.onerror = function () { r.e = r.r = 1, g() }), p.splice(e, 0, r), u.insertBefore(l, s ? null : n), m(function () { o || (u.removeChild(l), r.r = r.e = o = 1, j()) }, H.errorTimeout) } function e(a, b, c) { var d = b == "c" ? z : y; q = 0, b = b || "j", C(a) ? f(d, a, b, this.i++, l, c) : (p.splice(this.i++, 0, a), p.length == 1 && g()); return this } function d() { var a = H; a.loader = { load: e, i: 0 }; return a } var l = b.documentElement, m = a.setTimeout, n = b.getElementsByTagName("script")[0], o = {}.toString, p = [], q = 0, r = "MozAppearance" in l.style, s = r && !!b.createRange().compareNode, t = r && !s, u = s ? l : n.parentNode, v = a.opera && o.call(a.opera) == "[object Opera]", w = "webkitAppearance" in l.style, x = w && "async" in b.createElement("script"), y = r ? "object" : v || x ? "img" : "script", z = w ? "img" : y, A = Array.isArray || function (a) { return o.call(a) == "[object Array]" }, B = function (a) { return Object(a) === a }, C = function (a) { return typeof a == "string" }, D = function (a) { return o.call(a) == "[object Function]" }, E = [], F = {}, G, H; H = function (a) { function f(a) { var b = a.split("!"), c = E.length, d = b.pop(), e = b.length, f = { url: d, origUrl: d, prefixes: b }, g, h; for (h = 0; h < e; h++) g = F[b[h]], g && (f = g(f)); for (h = 0; h < c; h++) f = E[h](f); return f } function e(a, b, e, g, h) { var i = f(a), j = i.autoCallback; if (!i.bypass) { b && (b = D(b) ? b : b[a] || b[g] || b[a.split("/").pop().split("?")[0]]); if (i.instead) return i.instead(a, b, e, g, h); e.load(i.url, i.forceCSS || !i.forceJS && /css$/.test(i.url) ? "c" : c, i.noexec), (D(b) || D(j)) && e.load(function () { d(), b && b(i.origUrl, h, g), j && j(i.origUrl, h, g) }) } } function b(a, b) { function c(a) { if (C(a)) e(a, h, b, 0, d); else if (B(a)) for (i in a) a.hasOwnProperty(i) && e(a[i], h, b, i, d) } var d = !!a.test, f = d ? a.yep : a.nope, g = a.load || a.both, h = a.callback, i; c(f), c(g), a.complete && b.load(a.complete) } var g, h, i = this.yepnope.loader; if (C(a)) e(a, 0, i, 0); else if (A(a)) for (g = 0; g < a.length; g++) h = a[g], C(h) ? e(h, 0, i, 0) : A(h) ? H(h) : B(h) && b(h, i); else B(a) && b(a, i) }, H.addPrefix = function (a, b) { F[a] = b }, H.addFilter = function (a) { E.push(a) }, H.errorTimeout = 1e4, b.readyState == null && b.addEventListener && (b.readyState = "loading", b.addEventListener("DOMContentLoaded", G = function () { b.removeEventListener("DOMContentLoaded", G, 0), b.readyState = "complete" }, 0)), a.yepnope = d() } (this, this.document), Modernizr.load = function () { yepnope.apply(window, [].slice.call(arguments, 0)) };