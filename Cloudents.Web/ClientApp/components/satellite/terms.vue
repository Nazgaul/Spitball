<template>
    <div class="term" v-if="showPage" v-html="termsHtml"></div>
</template>
<script>
import { GetDictionary, LanguageService } from '../../services/language/languageService'
export default {
    data(){
        return {
            termsHtml: null,
            showPage: false
        }
    },
    created(){
        GetDictionary('termsofservice').then((returned)=>{
            this.termsHtml = LanguageService.getValueByKey('terms_page');
            this.showPage = true;
        })
    }
}
</script>

<style lang="less" src="./term.less"></style>