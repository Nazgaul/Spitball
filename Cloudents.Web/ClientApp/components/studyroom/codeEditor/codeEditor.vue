<template>
    <div class="code-editor-wrap">
        <codemirror v-model="code" class="code-editor-cont" 
                    :options="optionObj"
                     @gutterClick='gutterClick'/>
    </div>
</template>
<script>
    import debounce from "lodash/debounce";
    import { codemirror } from 'vue-codemirror';
    import "codemirror/addon/display/autorefresh.js";
    
    // languages:
    // c / c++ / c# / java / objective-c / scala : // check scala
    import "codemirror/mode/clike/clike.js";
    // Clojure:
    import "codemirror/mode/clojure/clojure.js";
    // css
    import "codemirror/mode/css/css.js";
    import 'codemirror/addon/hint/css-hint.js' // css hint
    // erlang
    import "codemirror/mode/erlang/erlang.js";
    // html
    import "codemirror/mode/xml/xml.js"; // xml
    import 'codemirror/addon/fold/xml-fold.js'; // xml fold
    import 'codemirror/addon/hint/xml-hint.js' // xml hint
    import 'codemirror/addon/hint/html-hint.js' // html hint
    // mixed
    import "codemirror/mode/htmlmixed/htmlmixed.js" // htmlmixed
    import "codemirror/mode/vbscript/vbscript.js" // vbscript
    const mixedMode = {
        name: "htmlmixed",
        scriptTypes: [{matches: /\/x-handlebars-template|\/x-mustache/i,
                       mode: null},
                      {matches: /(text|application)\/(x-)?vb(a|script)/i,
                       mode: "vbscript"}]
      };
    // javascript:
    import "codemirror/mode/javascript/javascript.js"; // javascript
    import 'codemirror/addon/hint/javascript-hint.js'; // javascript hint 
    // php
    import "codemirror/mode/php/php.js" 
    // perl
    import 'codemirror/mode/perl/perl.js'
    // python
    import 'codemirror/mode/python/python.js'
    const python = {name: "python",
               version: 3,
               singleLineStringErrors: false}
    // r
    import 'codemirror/mode/r/r.js';
    // ruby
    import 'codemirror/mode/ruby/ruby.js';
    // sql
    import 'codemirror/mode/sql/sql.js';
    import 'codemirror/addon/hint/sql-hint.js';
    // shell
    import 'codemirror/mode/shell/shell.js';
    // swift
    import 'codemirror/mode/swift/swift.js';

    // features:
    import "codemirror/addon/edit/closetag.js"; // auto close tags
    import "codemirror/addon/edit/closebrackets.js"; // auto close brackets
    import "codemirror/addon/edit/matchtags.js"; // match tags when click
    import "codemirror/addon/selection/active-line.js" // show active line
    import 'codemirror/addon/edit/matchbrackets.js' // match brackets when click
    import 'codemirror/addon/dialog/dialog.js' // dialog
    import 'codemirror/addon/search/search.js' // search
    import 'codemirror/addon/search/searchcursor.js' // searchcursor by curser selection
    import 'codemirror/addon/search/jump-to-line.js' // jump to line search / match tag
    import 'codemirror/addon/selection/selection-pointer.js'; // selection
    import 'codemirror/addon/display/fullscreen.css'; // full screen css
    import 'codemirror/addon/display/fullscreen.js'; // fullscreen
    import 'codemirror/addon/hint/show-hint.css'; // show hint css
    import 'codemirror/addon/hint/show-hint.js'; // show hint
    import 'codemirror/addon/comment/continuecomment.js'
    import 'codemirror/addon/comment/comment.js'
    import 'codemirror/addon/search/match-highlighter.js';

    import 'codemirror/addon/scroll/annotatescrollbar.js' // ?
    import 'codemirror/addon/search/matchesonscrollbar.js' // ?
    import 'codemirror/addon/search/matchesonscrollbar.css' // ?
    import 'codemirror/addon/dialog/dialog.css'; // for dialog  


    //STORE
    import { mapGetters,mapActions } from 'vuex';

    export default {
        name: "codeEditor",
        components:{
            codemirror
        },
        data() {
            return {
                optionObj: {
                    tabSize: 1,
                    mode: mixedMode,
                    theme: 'vscode-dark',
                    lineNumbers: true,
                    line: true,
                    autoRefresh: true,
                    autoCloseBrackets: true,
                    autoCloseTags: true,
                    styleActiveLine: true,
                    matchBrackets: true,
                    matchTags: {bothTags: true},
                    continueComments: "Enter",
                    extraKeys: {"Alt-F": "findPersistent",
                                "Ctrl-J": "toMatchingTag",
                                "F11": cm => {cm.setOption("fullScreen", !cm.getOption("fullScreen"))},
                                "Esc": cm => {if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false)},
                                "Ctrl-Space": "autocomplete",
                                "Ctrl-/": "toggleComment"
                                },
                    selectionPointer: true,
                    gutters: ["CodeMirror-linenumbers", "breakpoints"],
                    indentWithTabs: true,
                    smartIndent: true,
                    autofocus: true,
                }
            }
        },
        computed: {
            ...mapGetters(['getIsDarkTheme', 'getCurrentLang', 'getCode']),
            themeMode(){
                return this.getIsDarkTheme
            },
            currentLang(){
                return this.getCurrentLang
            },
            code:{
                get(){
                    return this.getCode
                },
                set: debounce(function (val) {
                        this.updateCode(val)
                    }, 125)
            }
        },
        methods: {
            ...mapActions(['updateCode']),
            makeMarker() {
                const marker = document.createElement("div")
                marker.style.color = "#FA7A6D"
                marker.innerHTML = "●"
                return marker
            },
            gutterClick(cm, n) {
                const info = cm.lineInfo(n)
                cm.setGutterMarker(n, "breakpoints", info.gutterMarkers ? null : this.makeMarker())
            },
            changeLang(lang){
                if(lang.langMode === 'htmlmixed'){
                    this.optionObj.mode = mixedMode
                } else if(lang.langMode === 'text/x-python'){
                    this.optionObj.mode = python
                } else{
                    this.optionObj.mode = lang.langMode
                }
            }
        },

        watch: {
            themeMode: function(val){
                if(val){
                    this.optionObj.theme = 'vscode-dark'
                } else{
                   this.optionObj.theme = 'coda'
                }
            },
            currentLang: function(val){
                this.changeLang(val)
            }
        },
        mounted() {
            this.changeLang(this.currentLang)
        },
    }

</script>

<style lang="less">

@import './helperStyles/codeMirror.less';
@import './themes/vscode-dark.css';
@import './themes/coda.css';

.code-editor-wrap {
    height: ~"calc(100vh - 108px)";
    width: ~"calc(100% - 333px)";
    position: relative;
    text-align: left /*rtl:ignore*/;
    direction: ltr /*rtl:ignore*/;
}
.code-editor-cont{
        position: absolute;
        top: 0;
        bottom: 0;
        right: 0;
        left: 0;
   
    .CodeMirror{
        height: 100%;
        font-size: 16px;
        //Same as iframe
    }
      .CodeMirror {border-top: 1px solid black; border-bottom: 1px solid black;}
      .CodeMirror-focused .cm-matchhighlight {
        background-position: bottom;
        background-repeat: repeat-x;
      }
      .cm-matchhighlight {background-color: rgb(59, 104, 59)}
      .CodeMirror-selection-highlight-scrollbar {background-color: rgb(255, 255, 255)}

}
</style>