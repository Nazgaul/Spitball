<template>
    <div>
        <codemirror class="code-editor-cont" 
                    :options="optionObj"
                     @gutterClick='gutterClick'/>
    </div>
</template>
<script>
    import { codemirror } from 'vue-codemirror';
    const CodeMirror = codemirror.CodeMirror
    
    import "codemirror/lib/codemirror.css";
    import "codemirror/addon/display/autorefresh.js";

    const mixedMode = {
        name: "htmlmixed",
        scriptTypes: [{matches: /\/x-handlebars-template|\/x-mustache/i,
                       mode: null},
                      {matches: /(text|application)\/(x-)?vb(a|script)/i,
                       mode: "vbscript"}]
      };


    // languages:
    import "codemirror/mode/xml/xml.js"; // xml
    import 'codemirror/addon/fold/xml-fold.js'; // xml fold
    import 'codemirror/addon/hint/xml-hint.js' // xml hint

    import "codemirror/mode/javascript/javascript.js" // javascript
    import 'codemirror/addon/hint/javascript-hint.js' // javascript hint 

    import "codemirror/mode/css/css.js" // css
    import 'codemirror/addon/hint/css-hint.js' // css hint

    import 'codemirror/addon/hint/html-hint.js' // html hint



    import "codemirror/mode/htmlmixed/htmlmixed.js" // htmlmixed
    import "codemirror/mode/vbscript/vbscript.js" // vbscript

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

    import 'codemirror/addon/scroll/annotatescrollbar.js' // ?
    import 'codemirror/addon/search/matchesonscrollbar.js' // ?
    import 'codemirror/addon/search/matchesonscrollbar.css' // ?

    // themes:
    import "codemirror/theme/base16-dark.css"; // dark theme
    import "codemirror/theme/base16-light.css"; // light theme
    import 'codemirror/addon/dialog/dialog.css'; // for dialog
    


















    import { mapGetters } from 'vuex';

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
                    theme: 'base16-dark',
                    lineNumbers: true,
                    line: true,
                    autoRefresh: true,
                    autoCloseBrackets: true,
                    autoCloseTags: true,
                    styleActiveLine: true,
                    matchBrackets: true,
                    matchTags: {bothTags: true},
                    extraKeys: {"Alt-F": "findPersistent",
                                "Ctrl-J": "toMatchingTag",
                                "F11": cm => {cm.setOption("fullScreen", !cm.getOption("fullScreen"))},
                                "Esc": cm => {if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false)},
                                "Ctrl-Space": "autocomplete",
                                },
                    selectionPointer: true,
                    gutters: ["CodeMirror-linenumbers", "breakpoints"]
                }
            }
        },
        computed: {
            ...mapGetters(['getIsDarkTheme']),
            themeMode(){
                return this.getIsDarkTheme
            }
        },
        methods: {
            makeMarker() {
                const marker = document.createElement("div")
                marker.style.color = "#FA7A6D"
                marker.innerHTML = "●"
                return marker
            },
            gutterClick(cm, n) {
                const info = cm.lineInfo(n)
                cm.setGutterMarker(n, "breakpoints", info.gutterMarkers ? null : this.makeMarker())
            }
        },

        watch: {
            themeMode: function(val){
                if(val){
                    this.optionObj.theme = 'base16-dark'
                } else{
                   this.optionObj.theme = 'base16-light'
                }
            }
        },
    }

</script>

<style lang="less">
@import "./themes/monokai.less";
.code-editor-cont{
    .CodeMirror{
        height: 85vh !important;
        font-size: 16px;
    }
}
</style>