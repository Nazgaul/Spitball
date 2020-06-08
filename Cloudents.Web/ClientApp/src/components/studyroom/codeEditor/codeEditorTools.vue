<template>
    <div class="nav-container">
            <!-- <v-tooltip bottom>
                <template v-slot:activator="{on}" id="languagesListPop" > -->
                    <v-btn text @click="show = !show" class="selected-lang nav-action"  id="languagesListPop" >
                        <div class="name_img">
                        <img style="width: 24px; margin-right: 5px;" :src="getLangImg(currentLang.langIcon)" >
                            {{currentLang.langName}}
                        </div>
                        <v-icon>{{ show ? 'sbf-arrow-up' : 'sbf-arrow-down' }}</v-icon>
                    </v-btn>
               <!-- </template> -->
                <!-- <span v-language:inner="'tutor_tooltip_code_langauge'"/> -->
            <!-- </v-tooltip>
            <v-tooltip bottom> -->
                <!-- <template v-slot:activator="{on}"> -->
                    <!-- <button v-on="on" class="nav-action" >
                        <v-switch class="switch" color="primary" v-model="themeMode" :label="switchThemeResx" hide-details />
                    </button> -->
                <!-- </template> -->
                <!-- <span v-t="'tutor_tooltip_code_theme'"/> -->
            <!-- </v-tooltip> -->
            <v-card v-if="show" class="list-lang-cont" id="languagesListPop" max-width="none">
                <div v-for="lang in languagesList" :key="lang.langName" class="list-lang" @click="selectLang(lang)"> 
                    <img style="width: 24px; margin-right: 5px;" v-if="lang.langIcon" :src="getLangImg(lang.langIcon)">
                    {{lang.langName}}
                </div>
            </v-card>
        </div>
</template>

<script>
import {LanguageService} from '../../../services/language/languageService.js'
import {mapGetters, mapActions} from 'vuex';
export default {
    data() {
        return {
            switchThemeResx: LanguageService.getValueByKey("tutor_tooltip_code_theme_switch"),
            show:false,
            languagesList: [
                {langName: 'C', langMode: 'text/x-c++src', langIcon:'./images/c.png'},
                {langName: 'C++', langMode: 'text/x-c++src', langIcon:'./images/c++.png'},
                {langName: 'C#', langMode: 'text/x-csharp', langIcon:'./images/c#.png'},
                {langName: 'CSS', langMode: 'text/css', langIcon:'./images/css.png'},
                {langName: 'Clojure', langMode: 'text/x-clojure', langIcon:'./images/clojure.png'},
                {langName: 'Erlang', langMode: 'text/x-erlang', langIcon:'./images/erlang.png'},
                {langName: 'HTML', langMode: 'text/html', langIcon:'./images/html.png'},
                {langName: 'HTML & JS', langMode: 'htmlmixed', langIcon:'./images/mixed.png'},
                {langName: 'Java', langMode: 'text/x-java', langIcon:'./images/java.png'},
                {langName: 'Javascript', langMode: 'javascript', langIcon:'./images/javascript.png'},
                {langName: 'Objective-C', langMode: 'text/x-objectivec', langIcon:'./images/objc.png'},
                {langName: 'PHP', langMode: 'text/x-php', langIcon:'./images/php.png'},
                {langName: 'Perl', langMode: 'text/x-perl', langIcon:'./images/perl.png'},
                {langName: 'Python', langMode: 'text/x-python', langIcon:'./images/python.png'},
                {langName: 'R', langMode: 'text/x-rsrc', langIcon:'./images/r.png'},
                {langName: 'Ruby', langMode: 'text/x-ruby', langIcon:'./images/ruby.png'},
                {langName: 'Scala', langMode: 'text/x-scala"', langIcon:'./images/scala.png'},
                {langName: 'SQL', langMode: 'text/x-mysql', langIcon:'./images/sql.png'},
                {langName: 'Shell', langMode: 'text/x-sh', langIcon:'./images/sh.png'},
                {langName: 'Swift', langMode: 'text/x-swift', langIcon:'./images/swift.png'},
            ]
        }
    },
    methods:{
        ...mapActions(['updateLang']),
        getLangImg(src){
            return require(`${src}`)
        },
        selectLang(lang){
            this.$ga.event("tutoringRoom", `selectLang:${lang}`);
            this.updateLang(lang)
            this.show = false
        },
        clodeSelector(event) {
            if(!this.show) return;
            let isInside = event.path.some(el=>el.id === 'languagesListPop')
            if(!isInside){
                this.show = false
            }
        }
    },
    computed:{
        ...mapGetters(['getCurrentLang']),
        currentLang(){
            return this.getCurrentLang
        },
        // themeMode:{
        //     get(){
        //         return !this.getIsDarkTheme
        //     },
        //     set(val){
        //         this.updateThemeMode(!val)
        //     }
        // }
    },
    mounted() {
        document.addEventListener("click", this.clodeSelector);
    },
    beforeDestroy() {
        document.removeEventListener("click", this.clodeSelector);
    }
}
</script>

// <style lang="less">
// @import '../../../styles/colors.less';
// .nav-container {
//     //position: relative;
//         .selected-lang{
//                 display: flex;
//                 align-items: center;
//                 font-size: 16px;
//                 border-radius: 4px;
//                 border: solid 1px @color-main;
//                 padding: 0 5px;
//                 width: 160px;
//                 height: 36px;
//                 align-self: center;
//                 .name_img{
//                         display: flex;
//                         align-items: inherit;
//                 }
//                 .v-btn__content{
//                     justify-content: space-between;
//                     padding: 0 8px;
//                 }
//         }

//     .list-lang-cont{
//         width: 410px;
//         position: absolute;
//         padding: 5px;
//         top: 52px;
//         left: 8px;
//         font-size: 14px;
//         .list-lang{
//             cursor: pointer;
//             // padding: 5px;
//             width: 120px;
//             float: left;
//             display: flex;
//             padding: 8px;
//             margin-right: 9px;
//         }
//         .list-lang:hover{
//             background-color: rgb(204, 204, 204);
//         }
//     }
//             .nav-action {
//                 .switch{
//                     margin: 0;
//                 }
//             }
//         }
// </style>
