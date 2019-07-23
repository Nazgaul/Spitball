<template>
    <div>
        <v-btn @click="change" color="success">text</v-btn>
        <codemirror class="code-editor-cont" :options="optionObj"/>
    </div>
</template>

<script>
    import { codemirror } from 'vue-codemirror';
    // mode:
    import "codemirror/mode/xml/xml.js";
    // adds:
    import "codemirror/addon/edit/closetag.js";
    import "codemirror/addon/edit/closebrackets.js";
    import "codemirror/addon/display/autorefresh.js";
    // theme css
    import "codemirror/theme/base16-dark.css";
    import 'codemirror/theme/xq-light.css'
    import 'codemirror/theme/material.css'
    import 'codemirror/theme/monokai.css'

    export default {
        name: "codeEditor",
        data() {
            return {
                optionObj: {
                    tabSize: 1,
                    mode: "xml",
                    theme: 'material',
                    lineNumbers: true,
                    line: true,
                    autoRefresh: true,
                    autoCloseBrackets: true,
                    autoCloseTags: true
                }
            }
        },
        props: {
            roomId: {
                required: true,
                type: String
            }
        },
        computed: {
        },
        methods: {
            change(){
                this.optionObj.theme = 'xq-light' 
            }
        },
        components:{
            codemirror
        },
        created(){
        }
    }

</script>

<style lang="less">
.code-editor-cont{
    .CodeMirror{
        height: 85vh !important;
        font-size: 20px;
    }
}
</style>