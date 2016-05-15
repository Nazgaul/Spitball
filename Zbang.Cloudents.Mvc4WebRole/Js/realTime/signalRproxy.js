/*!
 * ASP.NET SignalR JavaScript Library v2.2.0
 * http://signalr.net/
 *
 * Copyright Microsoft Open Technologies, Inc. All rights reserved.
 * Licensed under the Apache 2.0
 * https://github.com/SignalR/SignalR/blob/master/LICENSE.md
 *
 */
(function (n) { "use strict"; function r(t, i) { return function () { i.apply(t, n.makeArray(arguments)) } } function i(t, i) { var e, u, f, o, s; for (e in t) if (t.hasOwnProperty(e)) { if (u = t[e], !u.hubName) continue; s = i ? u.on : u.off; for (f in u.client) if (u.client.hasOwnProperty(f)) { if (o = u.client[f], !n.isFunction(o)) continue; s.call(u, f, r(u, o)) } } } if (typeof n.signalR != "function") throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js."); var t = n.signalR; n.hubConnection.prototype.createHubProxies = function () { var t = {}; return this.starting(function () { i(t, !0); this._registerSubscribedHubs() }).disconnected(function () { i(t, !1) }), t.chatHub = this.createHubProxy("chatHub"), t.chatHub.client = {}, t.chatHub.server = { hello: function () { return t.chatHub.invoke.apply(t.chatHub, n.merge(["Hello"], n.makeArray(arguments))) }, send: function () { return t.chatHub.invoke.apply(t.chatHub, n.merge(["Send"], n.makeArray(arguments))) } }, t.spitballHub = this.createHubProxy("spitballHub"), t.spitballHub.client = {}, t.spitballHub.server = { hello: function () { return t.spitballHub.invoke.apply(t.spitballHub, n.merge(["Hello"], n.makeArray(arguments))) } }, t }; t.hub = n.hubConnection("/s/signalr", { useDefaultPath: !1 }); n.extend(t, t.hub.createHubProxies()) })(window.jQuery, window)