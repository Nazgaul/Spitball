<template>
    <div>
        <!--<v-layout class="ml-3" row align-start>-->
        <!--<v-flex xs1 sm1 md1 v-for="codeSyntax in codeSet" @click="changeLanguageSyntax(codeSyntax)">-->
        <!--{{codeSyntax.name}}-->
        <!--</v-flex>-->
        <!--</v-layout>-->
        <div id="firepad"></div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import { syntaxEnum } from './syntaxEnums.js'

    export default {
        name: "codeEditor",
        data() {
            return {
                codeSet: syntaxEnum,
                codeItem: {},
                firepad: {},
                codeMirror: {},
                defaultSyntax: {
                    name: 'C#',
                    value: 'text/x-csharp',
                    link: 'https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.44.0/mode/clike/clike.min.js'
                },
            }
        },
        computed: {
            ...mapGetters(['roomLinkID'])
        },
        methods: {
            changeLanguageSyntax(codeSyntax) {
                let self = this;
                let loadCodeLang = codeSyntax;
                self.$loadScript(`${loadCodeLang.link}`).then((loaded) => {
                    self.codeMirror.setOption("mode", `${loadCodeLang.value}`);
                    // self.$loadScript(`https://cdn.firebase.com/libs/firepad/1.4.0/firepad.min.js`).then(
                    //     () => {
                    //         self.firepad = Firepad.fromCodeMirror(self.firepadRef, self.codeMirror)
                    //     })
                })
            },

            loadFirePad() {
                let self = this;
                let loadCodeLang = this.defaultSyntax;
                let roomId = self.roomLinkID || '';
                //load vue recaptha
                self.$loadScript(`https://www.gstatic.com/firebasejs/5.8.5/firebase.js`).then(
                    (data) => {
                        // Initialize Firebase
                        var config = {
                            apiKey: "AIzaSyASxWQgsnGpwngB3TOWfS49Nkbs_gSQhh4",
                            authDomain: "codeeditor-44dab.firebaseapp.com",
                            //init unique room by room id
                            databaseURL: "https://codeeditor-44dab.firebaseio.com",
                            // databaseURL: "https://codeeditor-44dab.firebaseio.com/",
                            projectId: "codeeditor-44dab",
                            storageBucket: "codeeditor-44dab.appspot.com",
                            messagingSenderId: "895562016590"
                        };
                        firebase.initializeApp(config);
                        // Get Firebase Database reference.
                        self.firepadRef = firebase.database().ref(roomId);
                        self.$loadScript(`https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.17.0/codemirror.js`)
                            .then((data) => {
                                //load syntax mode
                                self.$loadScript(`${loadCodeLang.link}`).then((loaded) => {
                                    self.codeMirror = CodeMirror(document.getElementById('firepad'), {
                                        lineNumbers: true,
                                        matchBrackets: true,
                                        styleActiveLine: true,
                                        theme: "monokai",
                                        mode: `${loadCodeLang.value}`
                                    });
                                    self.codeMirror.focus();
                                    self.codeMirror.setCursor(self.codeMirror.lineCount(), 0);
                                    self.$loadScript(`https://cdn.firebase.com/libs/firepad/1.4.0/firepad.min.js`).then(
                                        () => {
                                            self.firepad = Firepad.fromCodeMirror(self.firepadRef, self.codeMirror)
                                        })
                                })
                            });
                    });
            }
        },
        created() {
            this.loadFirePad();
        }
    }

</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    @import './themes/monokai.less';

    .firepad {
        margin-top: 24px;
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
        width: 100%;
        height: 100%;
        position: relative;
        text-align: left;
        line-height: normal;
    }

    #firepad {
        width: 70%;
        height: 88vh;
        overflow-y: hidden;
        margin-left: 90px;
        margin-top: 36px;
        .scrollBarStyle(3px, #0085D1);

    }

    .firepad .CodeMirror {
        position: absolute;
        top: 0;
        bottom: 0;
        right: 0;
        left: 0;
        height: auto;
    }

    .firepad-with-toolbar .CodeMirror {
        top: 83px;
        padding-left: 10px;
    }

    .firepad-with-toolbar .CodeMirror-lines {
        padding-top: 10px;
    }

    .firepad-toolbar {
        height: 82px;
        line-height: 82px;
        padding-left: 10px;
        border-bottom: 1px solid #c9c9c9;

        /* Don't select text when double-clicking in toolbar */
        -webkit-touch-callout: none;
        -webkit-user-select: none;
        -khtml-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
    }

    .firepad-toolbar-wrapper {
        display: inline-block;
        line-height: 14px;
    }

    .firepad-richtext .CodeMirror {
        font-family: Verdana, sans-serif;
        font-size: 14px;
    }

    .firepad-spacer {
        height: 10px;
        width: 1px;
    }

    /** Styles for all of the rich-text formatting we support. */
    .firepad-b {
        font-weight: bold;
    }

    .firepad-i {
        font-style: italic;
    }

    .firepad-u {
        text-decoration: underline;
    }

    .firepad-s {
        text-decoration: line-through;
    }

    .firepad-u.firepad-s {
        text-decoration: underline line-through;
    }

    .firepad-f-arial {
        font-family: Arial, Helvetica, sans-serif;
    }

    .firepad-f-comic-sans-ms {
        font-family: "Comic Sans MS", cursive, sans-serif;
    }

    .firepad-f-courier-new {
        font-family: "Courier New", Courier, monospace;
    }

    .firepad-f-impact {
        font-family: Impact, Charcoal, sans-serif;
    }

    .firepad-f-times-new-roman {
        font-family: "Times New Roman", Times, serif;
    }

    .firepad-f-verdana {
        font-family: Verdana, Geneva, sans-serif;
    }

    .firepad-la-left {
        text-align: left;
    }

    .firepad-la-center {
        text-align: center;
    }

    .firepad-la-right {
        text-align: right;
    }

    /** Line Styles */
    pre.firepad-lt-o, pre.firepad-lt-u, pre.firepad-lt-t, pre.firepad-lt-tc {
        padding-left: 40px;
    }

    .firepad-list-left {
        display: inline-block;
        margin-left: -40px;
        width: 40px;
        padding-right: 5px;
        text-align: right;
    }

    .firepad-todo-left {
        display: inline-block;
        margin-left: -20px;
        width: 20px;
    }

    .powered-by-firepad {
        position: absolute;
        display: block;
        z-index: 5;
        right: 20px;
        bottom: 20px;
        width: 129px;
        height: 23px;
        opacity: 0.5;
    }

    .powered-by-firepad:hover {
        opacity: 1;
    }

    .firepad-btn-group {
        margin: 5px 7px 0 0;
        display: inline-block;
    }

    a.firepad-btn, a.firepad-btn:visited, a.firepad-btn:active {
        /*font-family: "Arial" sans-serif;*/
        cursor: pointer;
        text-decoration: none;
        display: inline-block;
        padding: 6px 6px 4px 6px;
        text-align: center;
        vertical-align: middle;
        font-size: 16px;
        background-color: #fcfcfc;
        border: 1px solid #c9c9c9;
        border-bottom-width: 4px;
        color: #9c9c9c;
    }

    a.firepad-btn:hover {
        color: #fff;
        background-color: #ffbf86;
        border-color: #e6a165;
        text-decoration: none;
    }

    a.firepad-btn:active {
        -webkit-box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
        -moz-box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
        box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15), 0 1px 2px rgba(0, 0, 0, 0.05);
    }

    .firepad-btn-group > .firepad-btn {
        -webkit-border-radius: 0;
        -moz-border-radius: 0;
        border-radius: 0;
        margin-left: -1px;
    }

    .firepad-btn-group > .firepad-btn:first-child {
        border-bottom-left-radius: 6px;
        border-top-left-radius: 6px;
        -webkit-border-bottom-left-radius: 6px;
        -webkit-border-top-left-radius: 6px;
        -moz-border-radius-bottomleft: 6px;
        -moz-border-radius-topleft: 6px;
        margin-left: 0px;
    }

    .firepad-btn-group > .firepad-btn:last-child {
        border-bottom-right-radius: 6px;
        border-top-right-radius: 6px;
        -webkit-border-bottom-right-radius: 6px;
        -webkit-border-top-right-radius: 6px;
        -moz-border-radius-bottomright: 6px;
        -moz-border-radius-topright: 6px;
    }

    .firepad-dropdown {
        position: relative;
    }

    .firepad-dropdown-menu {
        position: absolute;
        top: 100%;
        left: 0;
        z-index: 1000;
        display: none;
        float: left;
        padding: 4px 0;
        margin: 4px 0 0;
        list-style: none;
        background-color: #ffffff;
        border: 1px solid #ccc;
        border: 1px solid rgba(0, 0, 0, 0.2);
        *border-right-width: 2px;
        *border-bottom-width: 2px;
        -webkit-border-radius: 5px;
        -moz-border-radius: 5px;
        border-radius: 5px;
        -webkit-box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
        -moz-box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
        box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
        -webkit-background-clip: padding-box;
        -moz-background-clip: padding;
        background-clip: padding-box;
    }

    .firepad-dropdown-menu a {
        text-align: left;
        display: block;
        padding: 3px 15px;
        clear: both;
        font-weight: normal;
        line-height: 18px;
        color: #333333;
        white-space: nowrap;
    }

    .firepad-dropdown-menu a:hover {
        color: #fff;
        text-decoration: none;
        background-color: #ffbf86;
    }

    .firepad-color-dropdown-item {
        height: 25px;
        width: 25px;
    }

    .firepad-dialog {
        position: absolute;
        left: 0px;
        top: 0px;
        width: 100%;
        height: 100%;
        z-index: 1000;
    }

    .firepad-dialog-div {
        position: relative;
        width: 400px;
        height: 100px;
        margin: 100px auto;
        background-color: #fff;
        border: 1px solid #000;
        padding: 15px;
    }

    .firepad-dialog-input {
        width: 80%;
        display: block;
        padding: 5px 5px;
        margin: 10px 10px 10px 5px;
        clear: both;
        font-weight: normal;
        line-height: 25px;
        color: #333333;
        white-space: nowrap;
    }

    /********************************************************************
     * Generated via icomoon.io.
      If you want to make changes, you can go to http://icomoon.io/app/, go to the bottom right and click the
      database-looking icon, then "Load Session" and use the checked-in font/firepad-icomoon.json file.

      Note: When you download the generated font, turn on the "Base 64 Encode ..." option to generate the font inline
      in the CSS (to avoid needing to distribute a font file with firepad).
     */
    /*@font-face {*/
    /*font-family: 'firepad';*/
    /*src:url('firepad.eot');*/
    /*}*/
    /*@font-face {*/
    /*font-family: 'firepad';*/
    /*src: url(data:application/font-woff;charset=utf-8;base64,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) format('woff'),*/
    /*url(data:application/x-font-ttf;charset=utf-8;base64,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) format('truetype');*/
    /*font-weight: normal;*/
    /*font-style: normal;*/
    /*}*/

    .firepad-tb-bold, .firepad-tb-italic, .firepad-tb-underline, .firepad-tb-strikethrough, .firepad-tb-list, .firepad-tb-list-2, .firepad-tb-numbered-list, .firepad-tb-paragraph-left, .firepad-tb-paragraph-center, .firepad-tb-paragraph-right, .firepad-tb-paragraph-justify, .firepad-tb-menu, .firepad-tb-link, .firepad-tb-undo, .firepad-tb-redo, .firepad-tb-box-add, .firepad-tb-box-remove, .firepad-tb-print, .firepad-tb-indent-decrease, .firepad-tb-indent-increase, .firepad-tb-insert-image, .firepad-tb-bubble {
        font-family: 'firepad';
        speak: none;
        font-style: normal;
        font-weight: normal;
        font-variant: normal;
        text-transform: none;
        line-height: 1;
        -webkit-font-smoothing: antialiased;
    }

    .firepad-tb-bold:before {
        content: "\e000";
    }

    .firepad-tb-italic:before {
        content: "\e001";
    }

    .firepad-tb-underline:before {
        content: "\e002";
    }

    .firepad-tb-strikethrough:before {
        content: "\e003";
    }

    .firepad-tb-list:before {
        content: "\e004";
    }

    .firepad-tb-list-2:before {
        content: "\e005";
    }

    .firepad-tb-numbered-list:before {
        content: "\e006";
    }

    .firepad-tb-paragraph-left:before {
        content: "\e007";
    }

    .firepad-tb-paragraph-center:before {
        content: "\e008";
    }

    .firepad-tb-paragraph-right:before {
        content: "\e009";
    }

    .firepad-tb-paragraph-justify:before {
        content: "\e00a";
    }

    .firepad-tb-menu:before {
        content: "\e00b";
    }

    .firepad-tb-link:before {
        content: "\e00c";
    }

    .firepad-tb-undo:before {
        content: "\e00d";
    }

    .firepad-tb-redo:before {
        content: "\e00e";
    }

    .firepad-tb-box-add:before {
        content: "\e010";
    }

    .firepad-tb-box-remove:before {
        content: "\e011";
    }

    .firepad-tb-print:before {
        content: "\e012";
    }

    .firepad-tb-indent-decrease:before {
        content: "\e013";
    }

    .firepad-tb-indent-increase:before {
        content: "\e014";
    }

    .firepad-tb-insert-image:before {
        content: "\e015";
    }

    .firepad-tb-bubble:before {
        content: "\e00f";
    }

    /* BASICS */

    .CodeMirror {
        /* Set height, width, borders, and global font properties here */
        font-family: monospace;
        height: 300px;
        color: black;
    }

    /* PADDING */

    .CodeMirror-lines {
        padding: 4px 0; /* Vertical padding around content */
    }

    .CodeMirror pre {
        padding: 0 4px; /* Horizontal padding of content */
    }

    .CodeMirror-scrollbar-filler, .CodeMirror-gutter-filler {
        background-color: white; /* The little square between H and V scrollbars */
    }

    /* GUTTER */

    .CodeMirror-gutters {
        border-right: 1px solid #ddd;
        background-color: #f7f7f7;
        white-space: nowrap;
    }

    .CodeMirror-linenumbers {
    }

    .CodeMirror-linenumber {
        padding: 0 3px 0 5px;
        min-width: 20px;
        text-align: right;
        color: #999;
        white-space: nowrap;
    }

    .CodeMirror-guttermarker {
        color: black;
    }

    .CodeMirror-guttermarker-subtle {
        color: #999;
    }

    /* CURSOR */

    .CodeMirror-cursor {
        border-left: 1px solid black;
        border-right: none;
        width: 0;
    }

    /* Shown when moving in bi-directional text */
    .CodeMirror div.CodeMirror-secondarycursor {
        border-left: 1px solid silver;
    }

    .cm-fat-cursor .CodeMirror-cursor {
        width: auto;
        border: 0 !important;
        background: #7e7;
    }

    .cm-fat-cursor div.CodeMirror-cursors {
        z-index: 1;
    }

    .cm-animate-fat-cursor {
        width: auto;
        border: 0;
        -webkit-animation: blink 1.06s steps(1) infinite;
        -moz-animation: blink 1.06s steps(1) infinite;
        animation: blink 1.06s steps(1) infinite;
        background-color: #7e7;
    }

    @-moz-keyframes blink {
        0% {
        }
        50% {
            background-color: transparent;
        }
        100% {
        }
    }

    @-webkit-keyframes blink {
        0% {
        }
        50% {
            background-color: transparent;
        }
        100% {
        }
    }

    @keyframes blink {
        0% {
        }
        50% {
            background-color: transparent;
        }
        100% {
        }
    }

    /* Can style cursor different in overwrite (non-insert) mode */
    .CodeMirror-overwrite .CodeMirror-cursor {
    }

    .cm-tab {
        display: inline-block;
        text-decoration: inherit;
    }

    .CodeMirror-rulers {
        position: absolute;
        left: 0;
        right: 0;
        top: -50px;
        bottom: -20px;
        overflow: hidden;
    }

    .CodeMirror-ruler {
        border-left: 1px solid #ccc;
        top: 0;
        bottom: 0;
        position: absolute;
    }

    /* DEFAULT THEME */

    .cm-s-default .cm-header {
        color: blue;
    }

    .cm-s-default .cm-quote {
        color: #090;
    }

    .cm-negative {
        color: #d44;
    }

    .cm-positive {
        color: #292;
    }

    .cm-header, .cm-strong {
        font-weight: bold;
    }

    .cm-em {
        font-style: italic;
    }

    .cm-link {
        text-decoration: underline;
    }

    .cm-strikethrough {
        text-decoration: line-through;
    }

    .cm-s-default .cm-keyword {
        color: #708;
    }

    .cm-s-default .cm-atom {
        color: #219;
    }

    .cm-s-default .cm-number {
        color: #164;
    }

    .cm-s-default .cm-def {
        color: #00f;
    }

    .cm-s-default .cm-variable,
    .cm-s-default .cm-punctuation,
    .cm-s-default .cm-property,
    .cm-s-default .cm-operator {
    }

    .cm-s-default .cm-variable-2 {
        color: #05a;
    }

    .cm-s-default .cm-variable-3 {
        color: #085;
    }

    .cm-s-default .cm-comment {
        color: #a50;
    }

    .cm-s-default .cm-string {
        color: #a11;
    }

    .cm-s-default .cm-string-2 {
        color: #f50;
    }

    .cm-s-default .cm-meta {
        color: #555;
    }

    .cm-s-default .cm-qualifier {
        color: #555;
    }

    .cm-s-default .cm-builtin {
        color: #30a;
    }

    .cm-s-default .cm-bracket {
        color: #997;
    }

    .cm-s-default .cm-tag {
        color: #170;
    }

    .cm-s-default .cm-attribute {
        color: #00c;
    }

    .cm-s-default .cm-hr {
        color: #999;
    }

    .cm-s-default .cm-link {
        color: #00c;
    }

    .cm-s-default .cm-error {
        color: #f00;
    }

    .cm-invalidchar {
        color: #f00;
    }

    .CodeMirror-composing {
        border-bottom: 2px solid;
    }

    /* Default styles for common addons */

    div.CodeMirror span.CodeMirror-matchingbracket {
        color: #0f0;
    }

    div.CodeMirror span.CodeMirror-nonmatchingbracket {
        color: #f22;
    }

    .CodeMirror-matchingtag {
        background: rgba(255, 150, 0, .3);
    }

    .CodeMirror-activeline-background {
        background: #e8f2ff;
    }

    /* STOP */

    /* The rest of this file contains styles related to the mechanics of
       the editor. You probably shouldn't touch them. */

    .CodeMirror {
        position: relative;
        overflow: hidden;
        background: white;
    }

    .CodeMirror-scroll {
        overflow: scroll !important; /* Things will break if this is overridden */
        /* 30px is the magic margin used to hide the element's real scrollbars */
        /* See overflow: hidden in .CodeMirror */
        margin-bottom: -30px;
        margin-right: -30px;
        padding-bottom: 30px;
        height: 100%;
        outline: none; /* Prevent dragging from highlighting the element */
        position: relative;
    }

    .CodeMirror-sizer {
        position: relative;
        border-right: 30px solid transparent;
    }

    /* The fake, visible scrollbars. Used to force redraw during scrolling
       before actual scrolling happens, thus preventing shaking and
       flickering artifacts. */
    .CodeMirror-vscrollbar, .CodeMirror-hscrollbar, .CodeMirror-scrollbar-filler, .CodeMirror-gutter-filler {
        position: absolute;
        z-index: 6;
        display: none;
    }

    .CodeMirror-vscrollbar {
        right: 0;
        top: 0;
        overflow-x: hidden;
        overflow-y: scroll;
    }

    .CodeMirror-hscrollbar {
        bottom: 0;
        left: 0;
        overflow-y: hidden;
        overflow-x: scroll;
    }

    .CodeMirror-scrollbar-filler {
        right: 0;
        bottom: 0;
    }

    .CodeMirror-gutter-filler {
        left: 0;
        bottom: 0;
    }

    .CodeMirror-gutters {
        position: absolute;
        left: 0;
        top: 0;
        min-height: 100%;
        z-index: 3;
    }

    .CodeMirror-gutter {
        white-space: normal;
        height: 100%;
        display: inline-block;
        vertical-align: top;
        margin-bottom: -30px;
        /* Hack to make IE7 behave */
        *zoom: 1;
        *display: inline;
    }

    .CodeMirror-gutter-wrapper {
        position: absolute;
        z-index: 4;
        background: none !important;
        border: none !important;
    }

    .CodeMirror-gutter-background {
        position: absolute;
        top: 0;
        bottom: 0;
        z-index: 4;
    }

    .CodeMirror-gutter-elt {
        position: absolute;
        cursor: default;
        z-index: 4;
    }

    .CodeMirror-gutter-wrapper {
        -webkit-user-select: none;
        -moz-user-select: none;
        user-select: none;
    }

    .CodeMirror-lines {
        cursor: text;
        min-height: 1px; /* prevents collapsing before first draw */
    }

    .CodeMirror pre {
        /* Reset some styles that the rest of the page might have set */
        -moz-border-radius: 0;
        -webkit-border-radius: 0;
        border-radius: 0;
        border-width: 0;
        background: transparent;
        font-family: inherit;
        font-size: inherit;
        margin: 0;
        white-space: pre;
        word-wrap: normal;
        line-height: inherit;
        color: inherit;
        z-index: 2;
        position: relative;
        overflow: visible;
        -webkit-tap-highlight-color: transparent;
        -webkit-font-variant-ligatures: none;
        font-variant-ligatures: none;
    }

    .CodeMirror-wrap pre {
        word-wrap: break-word;
        white-space: pre-wrap;
        word-break: normal;
    }

    .CodeMirror-linebackground {
        position: absolute;
        left: 0;
        right: 0;
        top: 0;
        bottom: 0;
        z-index: 0;
    }

    .CodeMirror-linewidget {
        position: relative;
        z-index: 2;
        overflow: auto;
    }

    .CodeMirror-widget {
    }

    .CodeMirror-code {
        outline: none;
    }

    /* Force content-box sizing for the elements where we expect it */
    .CodeMirror-scroll,
    .CodeMirror-sizer,
    .CodeMirror-gutter,
    .CodeMirror-gutters,
    .CodeMirror-linenumber {
        -moz-box-sizing: content-box;
        box-sizing: content-box;
    }

    .CodeMirror-measure {
        position: absolute;
        width: 100%;
        height: 0;
        overflow: hidden;
        visibility: hidden;
    }

    .CodeMirror-cursor {
        position: absolute;
        pointer-events: none;
    }

    .CodeMirror-measure pre {
        position: static;
    }

    div.CodeMirror-cursors {
        visibility: hidden;
        position: relative;
        z-index: 3;
    }

    div.CodeMirror-dragcursors {
        visibility: visible;
    }

    .CodeMirror-focused div.CodeMirror-cursors {
        visibility: visible;
    }

    .CodeMirror-selected {
        background: #d9d9d9;
    }

    .CodeMirror-focused .CodeMirror-selected {
        background: #d7d4f0;
    }

    .CodeMirror-crosshair {
        cursor: crosshair;
    }

    .CodeMirror-line::selection, .CodeMirror-line > span::selection, .CodeMirror-line > span > span::selection {
        background: #d7d4f0;
    }

    .CodeMirror-line::-moz-selection, .CodeMirror-line > span::-moz-selection, .CodeMirror-line > span > span::-moz-selection {
        background: #d7d4f0;
    }

    .cm-searching {
        background: #ffa;
        background: rgba(255, 255, 0, .4);
    }

    /* IE7 hack to prevent it from returning funny offsetTops on the spans */
    .CodeMirror span {
        *vertical-align: text-bottom;
    }

    /* Used to force a border model for a node */
    .cm-force-border {
        padding-right: .1px;
    }

    @media print {
        /* Hide the cursor when printing */
        .CodeMirror div.CodeMirror-cursors {
            visibility: hidden;
        }
    }

    /* See issue #2901 */
    .cm-tab-wrap-hack:after {
        content: '';
    }

    /* Help users use markselection to safely style text background */
    span.CodeMirror-selectedtext {
        background: none;
    }

</style>