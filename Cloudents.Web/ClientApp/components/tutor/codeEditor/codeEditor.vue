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
    import { mapGetters, mapActions } from 'vuex';
    import { syntaxEnum } from './syntaxEnums.js'
    export default {
        name: "codeEditor",
        data() {
            return {
                fireBaseConfig: {
                    apiKey: "AIzaSyASxWQgsnGpwngB3TOWfS49Nkbs_gSQhh4",
                    authDomain: "codeeditor-44dab.firebaseapp.com",
                    databaseURL: "https://codeeditor-44dab.firebaseio.com",
                    projectId: "codeeditor-44dab",
                    storageBucket: "codeeditor-44dab.appspot.com",
                    messagingSenderId: "895562016590"
                },
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
            ...mapGetters(['roomLinkID', 'firepadLoadedOnce'])
        },
        methods: {
            ...mapActions(['updateCodeLoadedOnce']),
            // changeLanguageSyntax(codeSyntax) {
            //     let self = this;
            //     let loadCodeLang = codeSyntax;
            //     self.$loadScript(`${loadCodeLang.link}`).then((loaded) => {
            //         if(self.codeMirror){
            //             self.codeMirror.setOption("mode", `${loadCodeLang.value}`);
            //         }
                    // wont work cause recreateing firepad with not clean codeMirror
                    // self.firepad = Firepad.fromCodeMirror(self.firepadRef, self.codeMirror, self.codeMirror.doc.getValue());
                    // console.log(self.codeMirror)
            //     })
            // },

            loadFirePad() {
                let self = this;
                let loadCodeLang = this.defaultSyntax;
                let roomId = self.roomLinkID || '';
                self.$loadScript(`https://www.gstatic.com/firebasejs/5.8.5/firebase.js`).then(
                    (data) => {
                        // Initialize Firebase
                        firebase.initializeApp(self.fireBaseConfig);
                        // Get Firebase Database reference.
                        self.firepadRef = firebase.database().ref(roomId);
                        self.$loadScript(`https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.17.0/codemirror.js`)
                            .then((data) => {
                                //load syntax mode
                                self.$loadScript(`${loadCodeLang.link}`).then((loaded) => {
                                    self.codeMirror = CodeMirror(document.getElementById('firepad'), {
                                        lineNumbers: true,
                                        matchBrackets: true,
                                        styleActiveLine: false,
                                        smartIndent: true,
                                        theme: "monokai",
                                        direction: "ltr",
                                        tabSize: 3,
                                        mode: `${loadCodeLang.value}`
                                    });
                                    self.codeMirror.focus();
                                    self.codeMirror.refresh();
                                    self.codeMirror.setCursor(self.codeMirror.lineCount(), 1);
                                    self.$loadScript(`https://cdn.firebase.com/libs/firepad/1.4.0/firepad.min.js`).then(
                                        () => {
                                            self.firepad = Firepad.fromCodeMirror(self.firepadRef, self.codeMirror)
                                            self.updateCodeLoadedOnce(true)
                                        })
                                })
                            });
                    });
            }
        },
        created() {
            //if was loaded before prevent calls
            if(!this.firepadLoadedOnce){
                this.loadFirePad();
            }
        }
    }

</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    @import './themes/monokai.less';
    @import './helperStyles/firepad.less';
    @import './helperStyles/codeMirror.less';
</style>