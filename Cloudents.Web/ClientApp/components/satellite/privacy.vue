<template>
    <div class="term" v-if="showPage" v-html="privacyHtml"></div>
</template>
<script>
import { GetDictionary, LanguageService } from '../../services/language/languageService'
export default {
    data(){
        return {
            showPage: null,
            privacyHtml: null
        }
    },
    created(){
        GetDictionary('privacy').then((returned)=>{
            this.privacyHtml = LanguageService.getValueByKey('privacy_page');
            this.showPage = true;
        })
    }
}
</script>
<style lang="less" src="./term.less">
</style>